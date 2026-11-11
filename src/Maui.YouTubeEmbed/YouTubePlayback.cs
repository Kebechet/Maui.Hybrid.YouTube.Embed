using System.Globalization;

namespace Kebechet.Maui.YouTubeEmbed;

/// <summary>
/// What to play: a validated video id and the optional range of it, in the whole seconds YouTube accepts.
/// </summary>
internal readonly record struct YouTubePlayback(string VideoId, int? StartSeconds, int? EndSeconds)
{
	private const string YouTubeEmbedBaseUrl = "https://www.youtube.com/embed/";

	private static readonly Uri _youTubeEmbedBaseUri = new(YouTubeEmbedBaseUrl);

	/// <summary>YouTube's own embed page for this playback.</summary>
	public string YouTubeEmbedUrl => $"{YouTubeEmbedBaseUrl}{VideoId}{Query}";

	/// <summary>The range as a query string (<c>?start=30&amp;end=45</c>), or empty when there is none.</summary>
	public string Query
	{
		get
		{
			var parameters = new List<string>();
			if (StartSeconds is not null)
			{
				parameters.Add(FormatParameter(YouTubePlayerParameters.Start, StartSeconds.Value));
			}

			if (EndSeconds is not null)
			{
				parameters.Add(FormatParameter(YouTubePlayerParameters.End, EndSeconds.Value));
			}

			return parameters.Any()
				? $"?{string.Join('&', parameters)}"
				: string.Empty;
		}
	}

	/// <summary>
	/// Validates the arguments a caller passed in and converts the range to the whole seconds YouTube accepts,
	/// dropping any fraction.
	/// </summary>
	/// <exception cref="ArgumentException"><paramref name="videoId"/> is not a YouTube video id.</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> or <paramref name="end"/> is negative.</exception>
	public static YouTubePlayback Create(string videoId, TimeSpan? start, TimeSpan? end)
	{
		if (!YouTubeVideoId.IsValid(videoId))
		{
			throw new ArgumentException($"'{videoId}' is not a YouTube video id: it must be 11 characters from A-Z, a-z, 0-9, '-' and '_'.", nameof(videoId));
		}

		return new YouTubePlayback(videoId, ToWholeSeconds(start, nameof(start)), ToWholeSeconds(end, nameof(end)));
	}

	/// <summary>Whether <paramref name="url"/> is YouTube's embed player, the URL <see cref="YouTubeEmbedUrl"/> builds.</summary>
	public static bool IsYouTubeEmbedUrl(Uri url)
	{
		return url.IsAbsoluteUri && _youTubeEmbedBaseUri.IsBaseOf(url);
	}

	/// <summary>
	/// Reads one range parameter of a request: absent is fine, anything but plain digits that fit an <see cref="int"/>
	/// is not (no sign, no whitespace, no fraction).
	/// </summary>
	public static bool TryParseSeconds(string? value, out int? seconds)
	{
		seconds = null;
		if (value is null)
		{
			return true;
		}

		if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var parsedSeconds))
		{
			return false;
		}

		seconds = parsedSeconds;
		return true;
	}

	private static int? ToWholeSeconds(TimeSpan? position, string parameterName)
	{
		if (position is null)
		{
			return null;
		}

		ArgumentOutOfRangeException.ThrowIfLessThan(position.Value, TimeSpan.Zero, parameterName);
		return (int) Math.Floor(position.Value.TotalSeconds);
	}

	private static string FormatParameter(string name, int seconds)
	{
		return $"{name}={seconds.ToString(CultureInfo.InvariantCulture)}";
	}
}
