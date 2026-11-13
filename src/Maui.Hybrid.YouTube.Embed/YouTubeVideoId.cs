using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Kebechet.Maui.Hybrid.YouTube.Embed;

/// <summary>
/// Checks the shape of a YouTube video id, such as <c>M7lc1UVf-VE</c>.
/// </summary>
public static class YouTubeVideoId
{
	private const int Length = 11;

	/// <summary>The characters a video id is made of: <c>A-Z</c>, <c>a-z</c>, <c>0-9</c>, <c>-</c> and <c>_</c>.</summary>
	internal static SearchValues<char> Characters { get; } = SearchValues.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_");

	/// <summary>
	/// Whether <paramref name="videoId"/> has the shape of a YouTube video id: exactly 11 characters from
	/// <c>A-Z</c>, <c>a-z</c>, <c>0-9</c>, <c>-</c> and <c>_</c>. It says nothing about whether the video exists.
	/// </summary>
	public static bool IsValid([NotNullWhen(true)] string? videoId)
	{
		return videoId is not null &&
			videoId.Length == Length &&
			!videoId.AsSpan().ContainsAnyExcept(Characters);
	}
}
