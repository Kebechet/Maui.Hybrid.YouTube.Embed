using System.Net;
using Shouldly;
using Xunit;

namespace Kebechet.Maui.Hybrid.YouTube.Embed.Tests;

/// <summary>
/// Runs the real loopback server: every request goes through a socket on 127.0.0.1.
/// </summary>
public sealed class LocalServerYouTubeEmbedUrlProviderTests : IAsyncDisposable
{
	private const string VideoId = "M7lc1UVf-VE";

	private readonly LocalServerYouTubeEmbedUrlProvider _localServerYouTubeEmbedUrlProvider = new();
	private readonly HttpClient _httpClient = new();

	private static CancellationToken _cancellationToken => TestContext.Current.CancellationToken;

	public async ValueTask DisposeAsync()
	{
		_httpClient.Dispose();
		await _localServerYouTubeEmbedUrlProvider.DisposeAsync();
	}

	[Fact]
	public async Task GetEmbedUrlAsync_WithRange_ReturnsLoopbackUrlCarryingVideoAndRange()
	{
		// Act
		var embedUrl = await _localServerYouTubeEmbedUrlProvider.GetEmbedUrlAsync(VideoId, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(45), _cancellationToken);

		// Assert
		var embedUri = new Uri(embedUrl);
		embedUri.Scheme.ShouldBe("http");
		embedUri.Host.ShouldBe("127.0.0.1");
		embedUri.Segments.Length.ShouldBe(3);
		embedUri.Segments.Last().ShouldBe(VideoId);
		embedUri.Query.ShouldBe("?start=30&end=45");
	}

	[Fact]
	public async Task GetEmbedUrlAsync_CalledConcurrently_ServesEveryUrlFromOnePort()
	{
		// Act
		var embedUrls = await Task.WhenAll(Enumerable.Range(0, 10)
			.Select(_ => _localServerYouTubeEmbedUrlProvider.GetEmbedUrlAsync(VideoId, cancellationToken: _cancellationToken).AsTask()));

		// Assert
		embedUrls
			.Distinct()
			.Count()
			.ShouldBe(1);
	}

	[Fact]
	public async Task Get_HandedOutUrl_ServesPageEmbeddingYouTubeWithRange()
	{
		// Arrange
		var embedUrl = await _localServerYouTubeEmbedUrlProvider.GetEmbedUrlAsync(VideoId, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(45), _cancellationToken);

		// Act
		using var response = await _httpClient.GetAsync(embedUrl, _cancellationToken);

		// Assert
		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		response.Content.Headers.ContentType!.MediaType.ShouldBe("text/html");
		var page = await response.Content.ReadAsStringAsync(_cancellationToken);
		page.ShouldContain($"src=\"https://www.youtube.com/embed/{VideoId}?start=30&amp;end=45\"");
		page.ShouldContain("allowfullscreen");
	}

	[Fact]
	public async Task Get_HandedOutUrl_SendsSecurityHeaders()
	{
		// Arrange
		var embedUrl = await _localServerYouTubeEmbedUrlProvider.GetEmbedUrlAsync(VideoId, cancellationToken: _cancellationToken);

		// Act
		using var response = await _httpClient.GetAsync(embedUrl, _cancellationToken);

		// Assert
		response.Headers.GetValues("Content-Security-Policy").Single().ShouldStartWith("default-src 'none'; frame-src https://*.youtube.com;");
		response.Headers.GetValues("Referrer-Policy").Single().ShouldBe("strict-origin-when-cross-origin");
		response.Headers.GetValues("X-Content-Type-Options").Single().ShouldBe("nosniff");
		response.Headers.GetValues("Cache-Control").Single().ShouldBe("no-store");
	}

	[Fact]
	public async Task Get_DifferentPathToken_ReturnsNotFound()
	{
		// Arrange
		var embedUri = new Uri(await _localServerYouTubeEmbedUrlProvider.GetEmbedUrlAsync(VideoId, cancellationToken: _cancellationToken));
		var foreignTokenUrl = $"http://127.0.0.1:{embedUri.Port}/{new string('0', 32)}/{VideoId}";

		// Act
		using var response = await _httpClient.GetAsync(foreignTokenUrl, _cancellationToken);

		// Assert
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	[Theory]
	[InlineData("M7lc1UVf-V")]
	[InlineData("M7lc1UVf-VEE")]
	[InlineData("M7lc1UVf%3CVE")]
	public async Task Get_InvalidVideoId_ReturnsNotFound(string invalidVideoId)
	{
		// Arrange
		var embedUrl = await _localServerYouTubeEmbedUrlProvider.GetEmbedUrlAsync(VideoId, cancellationToken: _cancellationToken);

		// Act
		using var response = await _httpClient.GetAsync(embedUrl.Replace(VideoId, invalidVideoId), _cancellationToken);

		// Assert
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	[Theory]
	[InlineData("?start=abc")]
	[InlineData("?start=-1")]
	[InlineData("?end=1.5")]
	[InlineData("?end=99999999999")]
	public async Task Get_MalformedRange_ReturnsBadRequest(string query)
	{
		// Arrange
		var embedUrl = await _localServerYouTubeEmbedUrlProvider.GetEmbedUrlAsync(VideoId, cancellationToken: _cancellationToken);

		// Act
		using var response = await _httpClient.GetAsync($"{embedUrl}{query}", _cancellationToken);

		// Assert
		response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
	}

	[Theory]
	[InlineData("attacker.example")]
	[InlineData("localhost")]
	[InlineData("192.168.1.20")]
	public async Task Get_ForeignHostHeader_ReturnsBadRequest(string host)
	{
		// Arrange
		var embedUrl = await _localServerYouTubeEmbedUrlProvider.GetEmbedUrlAsync(VideoId, cancellationToken: _cancellationToken);
		using var request = new HttpRequestMessage(HttpMethod.Get, embedUrl);
		request.Headers.Host = host;

		// Act
		using var response = await _httpClient.SendAsync(request, _cancellationToken);

		// Assert
		response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task Get_ForeignHostHeaderOnUnknownPath_ReturnsBadRequestBeforeRouting()
	{
		// Arrange
		var embedUri = new Uri(await _localServerYouTubeEmbedUrlProvider.GetEmbedUrlAsync(VideoId, cancellationToken: _cancellationToken));
		using var request = new HttpRequestMessage(HttpMethod.Get, $"http://127.0.0.1:{embedUri.Port}/no-such-path");
		request.Headers.Host = "attacker.example";

		// Act
		using var response = await _httpClient.SendAsync(request, _cancellationToken);

		// Assert
		response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task Post_HandedOutUrl_ReturnsMethodNotAllowed()
	{
		// Arrange
		var embedUrl = await _localServerYouTubeEmbedUrlProvider.GetEmbedUrlAsync(VideoId, cancellationToken: _cancellationToken);

		// Act
		using var response = await _httpClient.PostAsync(embedUrl, new StringContent(string.Empty), _cancellationToken);

		// Assert
		response.StatusCode.ShouldBe(HttpStatusCode.MethodNotAllowed);
	}

	[Fact]
	public async Task IsEmbedUrl_HandedOutUrlAndYouTubePlayer_ReturnsTrue()
	{
		// Arrange
		var embedUrl = await _localServerYouTubeEmbedUrlProvider.GetEmbedUrlAsync(VideoId, TimeSpan.FromSeconds(30), cancellationToken: _cancellationToken);

		// Act
		var isEmbedPageUrl = _localServerYouTubeEmbedUrlProvider.IsEmbedUrl(new Uri(embedUrl));
		var isYouTubePlayerUrl = _localServerYouTubeEmbedUrlProvider.IsEmbedUrl(new Uri($"https://www.youtube.com/embed/{VideoId}?start=30"));

		// Assert
		isEmbedPageUrl.ShouldBeTrue();
		isYouTubePlayerUrl.ShouldBeTrue();
	}

	[Fact]
	public async Task IsEmbedUrl_OtherLoopbackUrl_ReturnsFalse()
	{
		// Arrange
		var embedUri = new Uri(await _localServerYouTubeEmbedUrlProvider.GetEmbedUrlAsync(VideoId, cancellationToken: _cancellationToken));
		var otherPortUri = new UriBuilder(embedUri) { Port = embedUri.Port + 1 }.Uri;
		var otherTokenUri = new Uri($"http://127.0.0.1:{embedUri.Port}/{new string('0', 32)}/{VideoId}");

		// Act
		var isOtherPortEmbedUrl = _localServerYouTubeEmbedUrlProvider.IsEmbedUrl(otherPortUri);
		var isOtherTokenEmbedUrl = _localServerYouTubeEmbedUrlProvider.IsEmbedUrl(otherTokenUri);

		// Assert
		isOtherPortEmbedUrl.ShouldBeFalse();
		isOtherTokenEmbedUrl.ShouldBeFalse();
	}

	[Fact]
	public async Task DisposeAsync_AfterServing_StopsListening()
	{
		// Arrange
		var embedUrl = await _localServerYouTubeEmbedUrlProvider.GetEmbedUrlAsync(VideoId, cancellationToken: _cancellationToken);

		// Act
		await _localServerYouTubeEmbedUrlProvider.DisposeAsync();

		// Assert
		await Should.ThrowAsync<HttpRequestException>(() => _httpClient.GetAsync(embedUrl, _cancellationToken));
	}
}
