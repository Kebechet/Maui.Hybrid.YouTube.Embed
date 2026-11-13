namespace Kebechet.Maui.Hybrid.YouTube.Embed;

/// <summary>
/// Gives the URL to load in an <c>&lt;iframe&gt;</c> to play a YouTube video in the current host. Register it with
/// <see cref="YouTubeEmbedServiceCollectionExtensions.AddYouTubeEmbed"/>; <see cref="YouTubeEmbed"/> uses it.
/// </summary>
public interface IYouTubeEmbedUrlProvider
{
	/// <summary>
	/// Returns the iframe URL that plays <paramref name="videoId"/>, limited to the range between
	/// <paramref name="start"/> and <paramref name="end"/> when given (whole seconds; a fraction is dropped).
	/// <para>
	/// On iOS and Mac Catalyst it points to a page on a server inside the app (<c>http://127.0.0.1</c>) and is valid
	/// while the app runs; elsewhere it is YouTube's own embed URL. Ask again rather than storing it.
	/// </para>
	/// </summary>
	/// <exception cref="ArgumentException"><paramref name="videoId"/> is not a YouTube video id.</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> or <paramref name="end"/> is negative.</exception>
	ValueTask<string> GetEmbedUrlAsync(string videoId, TimeSpan? start = null, TimeSpan? end = null, CancellationToken cancellationToken = default);

	/// <summary>
	/// Whether <paramref name="url"/> is a frame an embed from this provider loads: the page on the in-app server, or
	/// YouTube's player.
	/// <para>
	/// ⚠️ A MAUI BlazorWebView on iOS and Mac Catalyst raises <c>UrlLoading</c> for every frame, iframes included, and by
	/// default opens any URL outside the app in the browser - the video then never shows. Keep these in the WebView
	/// from the app's <c>UrlLoading</c> handler:
	/// <code>
	/// if (youTubeEmbedUrlProvider.IsEmbedUrl(e.Url))
	/// {
	///     e.UrlLoadingStrategy = UrlLoadingStrategy.OpenInWebView;
	/// }
	/// </code>
	/// </para>
	/// </summary>
	bool IsEmbedUrl(Uri url);
}
