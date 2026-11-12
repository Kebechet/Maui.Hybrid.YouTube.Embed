using Microsoft.AspNetCore.Components;

namespace Kebechet.Maui.YouTubeEmbed;

/// <summary>
/// Renders an <c>&lt;iframe&gt;</c> that plays a YouTube video, with the URL from <see cref="IYouTubeEmbedUrlProvider"/>.
/// Renders nothing until that URL is known. Style it like any iframe: <c>class</c>, <c>style</c>, <c>loading</c> and
/// any other attribute pass through to it.
/// </summary>
public partial class YouTubeEmbed
{
	/// <summary>
	/// The YouTube video id, such as <c>M7lc1UVf-VE</c>. Rendering throws <see cref="ArgumentException"/> for anything
	/// <see cref="YouTubeVideoId.IsValid"/> rejects; check ids that come from users or content first.
	/// </summary>
	[Parameter, EditorRequired] public required string VideoId { get; set; }

	/// <summary>Where playback starts. Whole seconds; a fraction is dropped.</summary>
	[Parameter] public TimeSpan? Start { get; set; }

	/// <summary>Where playback stops. Whole seconds; a fraction is dropped.</summary>
	[Parameter] public TimeSpan? End { get; set; }

	/// <summary>The iframe's accessible name.</summary>
	[Parameter] public string Title { get; set; } = "YouTube video player";

	/// <summary>Attributes passed through to the iframe.</summary>
	[Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

	private string? _embedUrl;
	private (string VideoId, TimeSpan? Start, TimeSpan? End)? _embeddedPlayback;

	/// <inheritdoc />
	protected override async Task OnParametersSetAsync()
	{
		var playback = (VideoId, Start, End);
		if (_embeddedPlayback == playback)
		{
			return;
		}

		_embedUrl = await _youTubeEmbedUrlProvider.GetEmbedUrlAsync(VideoId, Start, End);
		_embeddedPlayback = playback;
	}
}
