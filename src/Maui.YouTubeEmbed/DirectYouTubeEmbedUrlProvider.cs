namespace Kebechet.Maui.YouTubeEmbed;

/// <summary>
/// Hands out YouTube's own embed URL. Right for every host whose page is an http(s) document, so the iframe request
/// carries a Referer: a website, and a BlazorWebView on Android and Windows (served from <c>https://0.0.0.1</c>).
/// </summary>
internal sealed class DirectYouTubeEmbedUrlProvider : IYouTubeEmbedUrlProvider
{
	public ValueTask<string> GetEmbedUrlAsync(string videoId, TimeSpan? start = null, TimeSpan? end = null, CancellationToken cancellationToken = default)
	{
		var youTubePlayback = YouTubePlayback.Create(videoId, start, end);
		return ValueTask.FromResult(youTubePlayback.YouTubeEmbedUrl);
	}

	public bool IsEmbedUrl(Uri url)
	{
		return YouTubePlayback.IsYouTubeEmbedUrl(url);
	}
}
