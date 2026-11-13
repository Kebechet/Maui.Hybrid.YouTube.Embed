using Kebechet.Maui.Hybrid.YouTube.Embed;

namespace DemoApp;

/// <summary>
/// The same embed from a page with no Blazor in it: plain JavaScript in MAUI's HybridWebView asks .NET for the URL.
/// </summary>
public partial class HybridPage : ContentPage
{
	public HybridPage(IYouTubeEmbedUrlProvider youTubeEmbedUrlProvider)
	{
		InitializeComponent();
		hybridWebView.SetInvokeJavaScriptTarget(new YouTubeEmbedBridge(youTubeEmbedUrlProvider));
	}
}

/// <summary>
/// What the page's JavaScript may call through <c>window.HybridWebView.InvokeDotNet</c>: this one method, rather than
/// every public member of the page.
/// </summary>
public sealed class YouTubeEmbedBridge(IYouTubeEmbedUrlProvider youTubeEmbedUrlProvider)
{
	public async Task<string> GetEmbedUrl(string videoId, int startSeconds, int endSeconds)
	{
		return await youTubeEmbedUrlProvider.GetEmbedUrlAsync(videoId, TimeSpan.FromSeconds(startSeconds), TimeSpan.FromSeconds(endSeconds));
	}
}
