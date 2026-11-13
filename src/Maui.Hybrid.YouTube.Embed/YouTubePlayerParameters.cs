namespace Kebechet.Maui.Hybrid.YouTube.Embed;

/// <summary>
/// Query parameters of YouTube's embed URL (https://developers.google.com/youtube/player_parameters) - a fixed format
/// contract: the local server reads them from the URL the provider hands out and passes them on to YouTube unchanged.
/// </summary>
internal static class YouTubePlayerParameters
{
	/// <summary>Whole seconds from the start of the video at which playback begins.</summary>
	public const string Start = "start";

	/// <summary>Whole seconds from the start of the video at which playback stops.</summary>
	public const string End = "end";
}
