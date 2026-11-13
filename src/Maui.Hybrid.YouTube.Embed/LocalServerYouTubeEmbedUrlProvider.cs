using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Shiny.Net.HttpServer;
using Shiny.Net.HttpServer.Security;

namespace Kebechet.Maui.Hybrid.YouTube.Embed;

/// <summary>
/// Hands out URLs of a page on a loopback HTTP server inside the app, and that page embeds YouTube's player.
/// <para>
/// MAUI's web views (BlazorWebView, HybridWebView) on iOS and Mac Catalyst serve the app from <c>app://0.0.0.1</c>,
/// and WebKit sends a Referer only from http(s) documents. YouTube refuses an embed request without one (Error 153,
/// "embedder identity"). The page served from <c>http://127.0.0.1</c> is an http document, so its iframe request
/// carries <c>Referer: http://127.0.0.1:port/</c>. WebKit offers no public API to set the header instead, and it
/// refuses a scheme handler for http or https.
/// </para>
/// </summary>
[UnsupportedOSPlatform("browser")]
internal sealed class LocalServerYouTubeEmbedUrlProvider : IYouTubeEmbedUrlProvider, IAsyncDisposable
{
	private const string VideoIdRouteKey = "videoId";

	/// <summary>
	/// A live listener answers a loopback connect at once; this only bounds the wait on a socket the system reclaimed.
	/// </summary>
	private static readonly TimeSpan _acceptCheckTimeout = TimeSpan.FromSeconds(1);

	private readonly HttpServer _httpServer;
	private readonly SemaphoreSlim _startLock = new(1, 1);

	/// <summary>
	/// Random per process and the first segment of every path, so the page answers only URLs this provider handed out -
	/// another app or a web page probing loopback ports finds nothing to embed.
	/// </summary>
	private readonly string _pathToken = Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(16));

	public LocalServerYouTubeEmbedUrlProvider(ILoggerFactory? loggerFactory = null)
	{
		var httpServerOptions = new HttpServerOptions
		{
			Address = IPAddress.Loopback,
			Port = 0,
			ServerHeader = null,
		};
		_httpServer = new HttpServer(httpServerOptions, loggerFactory: loggerFactory);

		// A web page that points a host name of its own at 127.0.0.1 (DNS rebinding) sends that name, not this one. Only
		// the address the URLs are built from is admitted; Shiny's defaults would also admit localhost and any IP address.
		var hostFilteringOptions = new HostFilteringOptions
		{
			AllowLoopbackHosts = false,
			AllowIpAddressHosts = false,
			AllowEmptyHosts = false,
			AllowedHosts = { IPAddress.Loopback.ToString() },
		};
		_httpServer.Use(new HostFilteringMiddleware(hostFilteringOptions, loggerFactory?.CreateLogger<HostFilteringMiddleware>()));

		_httpServer.MapGet($"/{_pathToken}/{{{VideoIdRouteKey}}}", ServeEmbedPageAsync);
	}

	public async ValueTask<string> GetEmbedUrlAsync(string videoId, TimeSpan? start = null, TimeSpan? end = null, CancellationToken cancellationToken = default)
	{
		var youTubePlayback = YouTubePlayback.Create(videoId, start, end);
		var port = await EnsureAcceptingAsync(cancellationToken);
		return $"http://{IPAddress.Loopback}:{port}/{_pathToken}/{youTubePlayback.VideoId}{youTubePlayback.Query}";
	}

	public bool IsEmbedUrl(Uri url)
	{
		return YouTubePlayback.IsYouTubeEmbedUrl(url) || IsEmbedPageUrl(url);
	}

	public async ValueTask DisposeAsync()
	{
		await _httpServer.DisposeAsync();
		_startLock.Dispose();
	}

	/// <summary>
	/// Makes sure the server accepts connections and returns its port. The first call starts it. A later call checks
	/// that the listener still accepts: iOS may reclaim a suspended app's listening socket
	/// (https://developer.apple.com/library/archive/technotes/tn2277/_index.html), and then the server is restarted.
	/// </summary>
	private async ValueTask<int> EnsureAcceptingAsync(CancellationToken cancellationToken)
	{
		await _startLock.WaitAsync(cancellationToken);
		try
		{
			if (_httpServer.IsRunning)
			{
				var isAccepting = await IsAcceptingAsync(_httpServer.Options.Port, cancellationToken);
				if (isAccepting)
				{
					return _httpServer.Options.Port;
				}
			}

			await _httpServer.StopAsync(cancellationToken);
			await StartAsync(cancellationToken);
			return _httpServer.Options.Port;
		}
		finally
		{
			_startLock.Release();
		}
	}

	private async Task StartAsync(CancellationToken cancellationToken)
	{
		try
		{
			await _httpServer.StartAsync(cancellationToken);
		}
		catch (IOException) when (_httpServer.Options.Port != 0)
		{
			// The previous port was taken while the server was down. Any free port will do: the URLs handed out from
			// now on carry the new one.
			_httpServer.Options.Port = 0;
			await _httpServer.StartAsync(cancellationToken);
		}

		// Pinned, so that a restart - this provider's or the server's own recovery from a dead listener - comes back on
		// the port the URLs already handed out point to.
		_httpServer.Options.Port = new Uri(_httpServer.ListenUrl!).Port;
	}

	private bool IsEmbedPageUrl(Uri url)
	{
		return url.IsAbsoluteUri &&
			url.Scheme == Uri.UriSchemeHttp &&
			url.Host == IPAddress.Loopback.ToString() &&
			url.Port == _httpServer.Options.Port &&
			url.AbsolutePath.StartsWith($"/{_pathToken}/", StringComparison.Ordinal);
	}

	private static async Task<bool> IsAcceptingAsync(int port, CancellationToken cancellationToken)
	{
		using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		timeout.CancelAfter(_acceptCheckTimeout);
		using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		try
		{
			await socket.ConnectAsync(IPAddress.Loopback, port, timeout.Token);
			return true;
		}
		catch (SocketException)
		{
			return false;
		}
		catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
		{
			return false;
		}
	}

	private async ValueTask ServeEmbedPageAsync(HttpContext httpContext)
	{
		var request = httpContext.Request;
		var response = httpContext.Response;

		var videoId = request.RouteValues[VideoIdRouteKey];
		if (!YouTubeVideoId.IsValid(videoId))
		{
			response.StatusCode = StatusCodes.Status404NotFound;
			return;
		}

		var isStartValid = YouTubePlayback.TryParseSeconds(request.Query.GetFirst(YouTubePlayerParameters.Start), out var startSeconds);
		var isEndValid = YouTubePlayback.TryParseSeconds(request.Query.GetFirst(YouTubePlayerParameters.End), out var endSeconds);
		if (!isStartValid || !isEndValid)
		{
			response.StatusCode = StatusCodes.Status400BadRequest;
			return;
		}

		foreach (var (name, value) in YouTubeEmbedPage.Headers)
		{
			response.Headers.Set(name, value);
		}

		var youTubePlayback = new YouTubePlayback(videoId, startSeconds, endSeconds);
		await response.WriteTextAsync(YouTubeEmbedPage.Render(youTubePlayback), YouTubeEmbedPage.ContentType, httpContext.RequestAborted);
	}
}
