using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace Kebechet.Maui.Hybrid.YouTube.Embed;

/// <summary>
/// A YouTube video and the range of it to play, such as one read from a link with <see cref="TryParse(string, out YouTubeLink)"/>.
/// </summary>
public sealed partial record YouTubeLink
{
	/// <summary>Query parameter of a watch link that holds the video id.</summary>
	private const string VideoIdParameter = "v";

	/// <summary>Query or fragment parameter of a shared link that holds where playback starts: <c>90</c>, <c>90s</c>, <c>1m30s</c>.</summary>
	private const string TimestampParameter = "t";

	/// <summary>Query parameter of an <c>attribution_link</c> that holds the watch link it wraps, relative to youtube.com.</summary>
	private const string AttributionTargetParameter = "u";

	/// <summary>Query parameter of an <c>oembed</c> link that holds the link it describes.</summary>
	private const string OEmbedTargetParameter = "url";

	private const string WatchPath = "watch";
	private const string AttributionLinkPath = "attribution_link";
	private const string OEmbedPath = "oembed";
	private const string ShortLinkHost = "youtu.be";

	private static readonly Uri _youTubeBaseUri = new("https://www.youtube.com/");

	private static readonly FrozenSet<string> _youTubeHosts = FrozenSet.Create(
		StringComparer.OrdinalIgnoreCase,
		"youtube.com",
		"www.youtube.com",
		"m.youtube.com",
		"music.youtube.com",
		"youtube-nocookie.com",
		"www.youtube-nocookie.com");

	/// <summary>First path segments followed by the video id: <c>/embed/{id}</c>, <c>/shorts/{id}</c> and so on.</summary>
	private static readonly FrozenSet<string> _videoIdPaths = FrozenSet.Create(StringComparer.OrdinalIgnoreCase, "embed", "shorts", "live", "v", "e");

	/// <summary>
	/// Embed paths that are not a video. Both happen to be 11 id characters long, so the id check alone would accept them.
	/// </summary>
	private static readonly FrozenSet<string> _nonVideoEmbedTargets = FrozenSet.Create(StringComparer.OrdinalIgnoreCase, "videoseries", "live_stream");

	private readonly YouTubePlayback _youTubePlayback;

	/// <summary>
	/// The video <paramref name="videoId"/>, limited to the range between <paramref name="start"/> and
	/// <paramref name="end"/> when given. Whole seconds; a fraction is dropped.
	/// </summary>
	/// <exception cref="ArgumentException"><paramref name="videoId"/> is not a YouTube video id.</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> or <paramref name="end"/> is negative.</exception>
	public YouTubeLink(string videoId, TimeSpan? start = null, TimeSpan? end = null)
	{
		_youTubePlayback = YouTubePlayback.Create(videoId, start, end);
	}

	/// <summary>The video id, such as <c>M7lc1UVf-VE</c>.</summary>
	public string VideoId => _youTubePlayback.VideoId;

	/// <summary>Where playback starts, or null for the beginning of the video.</summary>
	public TimeSpan? Start => ToTimeSpan(_youTubePlayback.StartSeconds);

	/// <summary>Where playback stops, or null for the end of the video.</summary>
	public TimeSpan? End => ToTimeSpan(_youTubePlayback.EndSeconds);

	/// <summary>
	/// YouTube's own embed URL for this video and range, such as <c>https://www.youtube.com/embed/M7lc1UVf-VE?start=30&amp;end=45</c>.
	/// The form to store. To play it in an app, load the URL <see cref="IYouTubeEmbedUrlProvider"/> returns instead:
	/// on iOS and Mac Catalyst this one shows YouTube's Error 153.
	/// </summary>
	public string YouTubeEmbedUrl => _youTubePlayback.YouTubeEmbedUrl;

	/// <summary>
	/// Reads the video and range from a link YouTube hands out: watch, <c>youtu.be</c>, shorts, live and embed links,
	/// on <c>www.</c>, <c>m.</c>, <c>music.</c> and <c>youtube-nocookie.com</c>, with or without the scheme. The range
	/// comes from <c>start</c> and <c>end</c>, or from the <c>t</c> of a shared link. Fails for anything that is not a
	/// single video, or whose id <see cref="YouTubeVideoId.IsValid"/> rejects; a range value it cannot read is left out.
	/// </summary>
	public static bool TryParse([NotNullWhen(true)] string? link, [NotNullWhen(true)] out YouTubeLink? youTubeLink)
	{
		return TryParse(link, canUnwrap: true, out youTubeLink);
	}

	/// <summary>
	/// <paramref name="canUnwrap"/>: whether an <c>attribution_link</c> or <c>oembed</c> link may be followed to the link
	/// it wraps. One level only.
	/// </summary>
	private static bool TryParse(string? link, bool canUnwrap, [NotNullWhen(true)] out YouTubeLink? youTubeLink)
	{
		youTubeLink = null;
		var uri = ToWebUri(link);
		if (uri is null)
		{
			return false;
		}

		var query = HttpUtility.ParseQueryString(uri.Query);
		var pathSegments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
		var firstPathSegment = pathSegments.FirstOrDefault();

		string? videoIdSource;
		if (uri.Host.Equals(ShortLinkHost, StringComparison.OrdinalIgnoreCase))
		{
			videoIdSource = firstPathSegment;
		}
		else if (!_youTubeHosts.Contains(uri.Host))
		{
			return false;
		}
		else if (canUnwrap && OEmbedPath.Equals(firstPathSegment, StringComparison.OrdinalIgnoreCase))
		{
			return TryParse(query[OEmbedTargetParameter], canUnwrap: false, out youTubeLink);
		}
		else if (canUnwrap && AttributionLinkPath.Equals(firstPathSegment, StringComparison.OrdinalIgnoreCase))
		{
			var attributionTarget = query[AttributionTargetParameter];
			var isTargetValid = Uri.TryCreate(_youTubeBaseUri, attributionTarget, out var targetUri);
			return attributionTarget is not null &&
				isTargetValid &&
				TryParse(targetUri!.AbsoluteUri, canUnwrap: false, out youTubeLink);
		}
		else if (WatchPath.Equals(firstPathSegment, StringComparison.OrdinalIgnoreCase))
		{
			videoIdSource = query[VideoIdParameter];
		}
		else if (firstPathSegment is not null && _videoIdPaths.Contains(firstPathSegment))
		{
			videoIdSource = pathSegments.ElementAtOrDefault(1);
		}
		else
		{
			return false;
		}

		var videoId = ReadLeadingVideoIdCharacters(videoIdSource);
		if (!YouTubeVideoId.IsValid(videoId) || _nonVideoEmbedTargets.Contains(videoId))
		{
			return false;
		}

		var fragment = HttpUtility.ParseQueryString(uri.Fragment.TrimStart('#'));
		var start = ReadSeconds(query[YouTubePlayerParameters.Start]) ??
			ReadTimestamp(query[TimestampParameter]) ??
			ReadTimestamp(fragment[TimestampParameter]);
		var end = ReadSeconds(query[YouTubePlayerParameters.End]);
		youTubeLink = new YouTubeLink(videoId, start, end);
		return true;
	}

	/// <summary>
	/// The link as an absolute http(s) URI. A link pasted without its scheme (<c>youtube.com/watch?v=...</c>) gets
	/// <c>https://</c> in front.
	/// </summary>
	private static Uri? ToWebUri(string? link)
	{
		if (string.IsNullOrWhiteSpace(link))
		{
			return null;
		}

		var trimmedLink = link.Trim();
		var isAbsolute = Uri.TryCreate(trimmedLink, UriKind.Absolute, out var uri);
		if (isAbsolute && (uri!.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp))
		{
			return uri;
		}

		var isSchemeless = Uri.TryCreate($"{Uri.UriSchemeHttps}://{trimmedLink}", UriKind.Absolute, out var schemelessUri);
		return isSchemeless
			? schemelessUri
			: null;
	}

	/// <summary>
	/// The id characters at the start of <paramref name="value"/>. Links in the wild run the id straight into other
	/// text, such as <c>youtu.be/oTJRivZTMLs&amp;feature=channel</c>; a run longer or shorter than an id still fails
	/// <see cref="YouTubeVideoId.IsValid"/> rather than being cut to fit.
	/// </summary>
	private static string? ReadLeadingVideoIdCharacters(string? value)
	{
		if (value is null)
		{
			return null;
		}

		var length = value.AsSpan().IndexOfAnyExcept(YouTubeVideoId.Characters);
		return length < 0
			? value
			: value[..length];
	}

	private static TimeSpan? ReadSeconds(string? value)
	{
		var isValid = YouTubePlayback.TryParseSeconds(value, out var seconds);
		return isValid && seconds is not null
			? TimeSpan.FromSeconds(seconds.Value)
			: null;
	}

	/// <summary>Reads <c>90</c>, <c>90s</c>, <c>1m30s</c> or <c>1h2m3s</c>; null for anything else.</summary>
	private static TimeSpan? ReadTimestamp(string? value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}

		var match = _timestampRegex.Match(value);
		if (!match.Success)
		{
			return null;
		}

		var totalSeconds = (ReadNumber(match.Groups["hours"]) * 3600) +
			(ReadNumber(match.Groups["minutes"]) * 60) +
			ReadNumber(match.Groups["seconds"]);
		return totalSeconds <= int.MaxValue
			? TimeSpan.FromSeconds(totalSeconds)
			: null;
	}

	/// <summary>A group the timestamp left out counts as zero. Capped digits keep the sum within a <see cref="long"/>.</summary>
	private static long ReadNumber(Group group)
	{
		return group.Success
			? long.Parse(group.Value, NumberStyles.None, CultureInfo.InvariantCulture)
			: 0;
	}

	private static TimeSpan? ToTimeSpan(int? seconds)
	{
		return seconds is not null
			? TimeSpan.FromSeconds(seconds.Value)
			: null;
	}

	/// <summary>
	/// Hours, minutes and seconds, each optional but at least one present; the unit of a bare number is seconds.
	/// Up to 9 digits each, so the total cannot overflow.
	/// </summary>
	[GeneratedRegex(@"^(?=\d)(?:(?<hours>\d{1,9})h)?(?:(?<minutes>\d{1,9})m)?(?:(?<seconds>\d{1,9})s?)?\z")]
	private static partial Regex _timestampRegex { get; }
}
