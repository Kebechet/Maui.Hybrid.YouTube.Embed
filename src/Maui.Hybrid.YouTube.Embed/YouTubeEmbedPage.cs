using System.Net;

namespace Kebechet.Maui.Hybrid.YouTube.Embed;

/// <summary>
/// The page the local server returns: nothing but YouTube's player filling the frame.
/// </summary>
internal static class YouTubeEmbedPage
{
	/// <summary>
	/// What the player may use. Set on the app's iframe and on the page's own, because a feature is delegated one frame
	/// at a time: an iframe that leaves one out takes it away from every frame inside it.
	/// </summary>
	public const string AllowedFeatures = "accelerometer; autoplay; clipboard-write; encrypted-media; fullscreen; gyroscope; picture-in-picture; web-share";

	public const string ContentType = "text/html; charset=utf-8";

	/// <summary>
	/// Response headers of the page.
	/// <para>
	/// The policy permits YouTube frames and inline styles only. <c>strict-origin-when-cross-origin</c> is set
	/// explicitly because the whole page exists to send YouTube a Referer: this policy sends the origin
	/// (<c>http://127.0.0.1:port/</c>) and never the path with its token.
	/// </para>
	/// </summary>
	public static IReadOnlyDictionary<string, string> Headers { get; } = new Dictionary<string, string>
	{
		["Content-Security-Policy"] = "default-src 'none'; frame-src https://*.youtube.com; style-src 'unsafe-inline'; base-uri 'none'; form-action 'none'",
		["Referrer-Policy"] = "strict-origin-when-cross-origin",
		["X-Content-Type-Options"] = "nosniff",
		["Cache-Control"] = "no-store",
	};

	public static string Render(YouTubePlayback youTubePlayback)
	{
		var youTubeEmbedUrl = WebUtility.HtmlEncode(youTubePlayback.YouTubeEmbedUrl);
		return $$"""
			<!DOCTYPE html>
			<html>
			<head>
			<meta charset="utf-8">
			<meta name="viewport" content="width=device-width, initial-scale=1">
			<style>html,body{margin:0;height:100%;background:#000}iframe{display:block;width:100%;height:100%;border:0}</style>
			</head>
			<body>
			<iframe src="{{youTubeEmbedUrl}}" title="YouTube video player" allow="{{AllowedFeatures}}" referrerpolicy="strict-origin-when-cross-origin" allowfullscreen></iframe>
			</body>
			</html>
			""";
	}
}
