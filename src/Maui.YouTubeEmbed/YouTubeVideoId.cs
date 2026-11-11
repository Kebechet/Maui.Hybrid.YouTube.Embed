using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Kebechet.Maui.YouTubeEmbed;

/// <summary>
/// Checks the shape of a YouTube video id, such as <c>M7lc1UVf-VE</c>.
/// </summary>
public static partial class YouTubeVideoId
{
	/// <summary>
	/// Whether <paramref name="videoId"/> has the shape of a YouTube video id: exactly 11 characters from
	/// <c>A-Z</c>, <c>a-z</c>, <c>0-9</c>, <c>-</c> and <c>_</c>. It says nothing about whether the video exists.
	/// </summary>
	public static bool IsValid([NotNullWhen(true)] string? videoId)
	{
		return videoId is not null && _videoIdRegex.IsMatch(videoId);
	}

	/// <summary>
	/// <c>\z</c>, not <c>$</c>: <c>$</c> also matches before a trailing newline, which would let an id with one
	/// appended through.
	/// </summary>
	[GeneratedRegex(@"^[A-Za-z0-9_-]{11}\z")]
	private static partial Regex _videoIdRegex { get; }
}
