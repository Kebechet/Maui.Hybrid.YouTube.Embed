using Shouldly;
using Xunit;

namespace Kebechet.Maui.Hybrid.YouTube.Embed.Tests;

public class YouTubeLinkTests
{
	[Theory]
	[InlineData("https://www.youtube.com/shorts/7bzoSi4nzGo", "7bzoSi4nzGo")]
	[InlineData("http://www.youtube.com/watch?v=-wtIMTCHWuI", "-wtIMTCHWuI")]
	[InlineData("http://youtube.com/watch?v=-wtIMTCHWuI", "-wtIMTCHWuI")]
	[InlineData("http://m.youtube.com/watch?v=-wtIMTCHWuI", "-wtIMTCHWuI")]
	[InlineData("https://www.youtube.com/watch?v=lalOy8Mbfdc", "lalOy8Mbfdc")]
	[InlineData("https://youtube.com/watch?v=lalOy8Mbfdc", "lalOy8Mbfdc")]
	[InlineData("https://m.youtube.com/watch?v=lalOy8Mbfdc", "lalOy8Mbfdc")]
	[InlineData("http://www.youtube.com/watch?v=yZv2daTWRZU&feature=em-uploademail", "yZv2daTWRZU")]
	[InlineData("http://youtube.com/watch?v=yZv2daTWRZU&feature=em-uploademail", "yZv2daTWRZU")]
	[InlineData("http://m.youtube.com/watch?v=yZv2daTWRZU&feature=em-uploademail", "yZv2daTWRZU")]
	[InlineData("https://www.youtube.com/watch?v=yZv2daTWRZU&feature=em-uploademail", "yZv2daTWRZU")]
	[InlineData("https://youtube.com/watch?v=yZv2daTWRZU&feature=em-uploademail", "yZv2daTWRZU")]
	[InlineData("https://m.youtube.com/watch?v=yZv2daTWRZU&feature=em-uploademail", "yZv2daTWRZU")]
	[InlineData("http://www.youtube.com/watch?v=0zM3nApSvMg&feature=feedrec_grec_index", "0zM3nApSvMg")]
	[InlineData("http://youtube.com/watch?v=0zM3nApSvMg&feature=feedrec_grec_index", "0zM3nApSvMg")]
	[InlineData("http://m.youtube.com/watch?v=0zM3nApSvMg&feature=feedrec_grec_index", "0zM3nApSvMg")]
	[InlineData("https://www.youtube.com/watch?v=0zM3nApSvMg&feature=feedrec_grec_index", "0zM3nApSvMg")]
	[InlineData("https://youtube.com/watch?v=0zM3nApSvMg&feature=feedrec_grec_index", "0zM3nApSvMg")]
	[InlineData("https://m.youtube.com/watch?v=0zM3nApSvMg&feature=feedrec_grec_index", "0zM3nApSvMg")]
	[InlineData("http://www.youtube.com/watch?v=0zM3nApSvMg#t=0m10s", "0zM3nApSvMg")]
	[InlineData("http://youtube.com/watch?v=0zM3nApSvMg#t=0m10s", "0zM3nApSvMg")]
	[InlineData("http://m.youtube.com/watch?v=0zM3nApSvMg#t=0m10s", "0zM3nApSvMg")]
	[InlineData("https://www.youtube.com/watch?v=0zM3nApSvMg#t=0m10s", "0zM3nApSvMg")]
	[InlineData("https://youtube.com/watch?v=0zM3nApSvMg#t=0m10s", "0zM3nApSvMg")]
	[InlineData("https://m.youtube.com/watch?v=0zM3nApSvMg#t=0m10s", "0zM3nApSvMg")]
	[InlineData("http://www.youtube.com/watch?v=cKZDdG9FTKY&feature=channel", "cKZDdG9FTKY")]
	[InlineData("http://youtube.com/watch?v=cKZDdG9FTKY&feature=channel", "cKZDdG9FTKY")]
	[InlineData("http://m.youtube.com/watch?v=cKZDdG9FTKY&feature=channel", "cKZDdG9FTKY")]
	[InlineData("https://www.youtube.com/watch?v=oTJRivZTMLs&feature=channel", "oTJRivZTMLs")]
	[InlineData("https://youtube.com/watch?v=oTJRivZTMLs&feature=channel", "oTJRivZTMLs")]
	[InlineData("https://m.youtube.com/watch?v=oTJRivZTMLs&feature=channel", "oTJRivZTMLs")]
	[InlineData("http://www.youtube.com/watch?v=lalOy8Mbfdc&playnext_from=TL&videos=osPknwzXEas&feature=sub", "lalOy8Mbfdc")]
	[InlineData("http://youtube.com/watch?v=lalOy8Mbfdc&playnext_from=TL&videos=osPknwzXEas&feature=sub", "lalOy8Mbfdc")]
	[InlineData("http://m.youtube.com/watch?v=lalOy8Mbfdc&playnext_from=TL&videos=osPknwzXEas&feature=sub", "lalOy8Mbfdc")]
	[InlineData("https://www.youtube.com/watch?v=lalOy8Mbfdc&playnext_from=TL&videos=osPknwzXEas&feature=sub", "lalOy8Mbfdc")]
	[InlineData("https://youtube.com/watch?v=lalOy8Mbfdc&playnext_from=TL&videos=osPknwzXEas&feature=sub", "lalOy8Mbfdc")]
	[InlineData("https://m.youtube.com/watch?v=lalOy8Mbfdc&playnext_from=TL&videos=osPknwzXEas&feature=sub", "lalOy8Mbfdc")]
	[InlineData("http://www.youtube.com/watch?v=lalOy8Mbfdc&feature=youtu.be", "lalOy8Mbfdc")]
	[InlineData("http://youtube.com/watch?v=lalOy8Mbfdc&feature=youtu.be", "lalOy8Mbfdc")]
	[InlineData("http://m.youtube.com/watch?v=lalOy8Mbfdc&feature=youtu.be", "lalOy8Mbfdc")]
	[InlineData("https://www.youtube.com/watch?v=lalOy8Mbfdc&feature=youtu.be", "lalOy8Mbfdc")]
	[InlineData("https://youtube.com/watch?v=lalOy8Mbfdc&feature=youtu.be", "lalOy8Mbfdc")]
	[InlineData("https://m.youtube.com/watch?v=lalOy8Mbfdc&feature=youtu.be", "lalOy8Mbfdc")]
	[InlineData("http://www.youtube.com/watch?v=dQw4w9WgXcQ&feature=youtube_gdata_player", "dQw4w9WgXcQ")]
	[InlineData("http://youtube.com/watch?v=dQw4w9WgXcQ&feature=youtube_gdata_player", "dQw4w9WgXcQ")]
	[InlineData("http://m.youtube.com/watch?v=dQw4w9WgXcQ&feature=youtube_gdata_player", "dQw4w9WgXcQ")]
	[InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ&feature=youtube_gdata_player", "dQw4w9WgXcQ")]
	[InlineData("https://youtube.com/watch?v=dQw4w9WgXcQ&feature=youtube_gdata_player", "dQw4w9WgXcQ")]
	[InlineData("https://m.youtube.com/watch?v=dQw4w9WgXcQ&feature=youtube_gdata_player", "dQw4w9WgXcQ")]
	[InlineData("http://www.youtube.com/watch?v=ishbTyLs6ps&list=PLGup6kBfcU7Le5laEaCLgTKtlDcxMqGxZ&index=106&shuffle=2655", "ishbTyLs6ps")]
	[InlineData("http://youtube.com/watch?v=ishbTyLs6ps&list=PLGup6kBfcU7Le5laEaCLgTKtlDcxMqGxZ&index=106&shuffle=2655", "ishbTyLs6ps")]
	[InlineData("http://m.youtube.com/watch?v=ishbTyLs6ps&list=PLGup6kBfcU7Le5laEaCLgTKtlDcxMqGxZ&index=106&shuffle=2655", "ishbTyLs6ps")]
	[InlineData("https://www.youtube.com/watch?v=ishbTyLs6ps&list=PLGup6kBfcU7Le5laEaCLgTKtlDcxMqGxZ&index=106&shuffle=2655", "ishbTyLs6ps")]
	[InlineData("https://youtube.com/watch?v=ishbTyLs6ps&list=PLGup6kBfcU7Le5laEaCLgTKtlDcxMqGxZ&index=106&shuffle=2655", "ishbTyLs6ps")]
	[InlineData("https://m.youtube.com/watch?v=ishbTyLs6ps&list=PLGup6kBfcU7Le5laEaCLgTKtlDcxMqGxZ&index=106&shuffle=2655", "ishbTyLs6ps")]
	[InlineData("http://www.youtube.com/watch?feature=player_embedded&v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("http://youtube.com/watch?feature=player_embedded&v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("http://m.youtube.com/watch?feature=player_embedded&v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("https://www.youtube.com/watch?feature=player_embedded&v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("https://youtube.com/watch?feature=player_embedded&v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("https://m.youtube.com/watch?feature=player_embedded&v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("http://www.youtube.com/watch?app=desktop&v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("http://youtube.com/watch?app=desktop&v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("http://m.youtube.com/watch?app=desktop&v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("https://www.youtube.com/watch?app=desktop&v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("https://youtube.com/watch?app=desktop&v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("https://m.youtube.com/watch?app=desktop&v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("http://www.youtube.com/v/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("http://youtube.com/v/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("http://m.youtube.com/v/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("https://www.youtube.com/v/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("https://youtube.com/v/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("https://m.youtube.com/v/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("http://www.youtube.com/v/-wtIMTCHWuI?version=3&autohide=1", "-wtIMTCHWuI")]
	[InlineData("http://youtube.com/v/-wtIMTCHWuI?version=3&autohide=1", "-wtIMTCHWuI")]
	[InlineData("http://m.youtube.com/v/-wtIMTCHWuI?version=3&autohide=1", "-wtIMTCHWuI")]
	[InlineData("https://www.youtube.com/v/-wtIMTCHWuI?version=3&autohide=1", "-wtIMTCHWuI")]
	[InlineData("https://youtube.com/v/-wtIMTCHWuI?version=3&autohide=1", "-wtIMTCHWuI")]
	[InlineData("https://m.youtube.com/v/-wtIMTCHWuI?version=3&autohide=1", "-wtIMTCHWuI")]
	[InlineData("http://www.youtube.com/v/0zM3nApSvMg?fs=1&hl=en_US&rel=0", "0zM3nApSvMg")]
	[InlineData("http://youtube.com/v/0zM3nApSvMg?fs=1&hl=en_US&rel=0", "0zM3nApSvMg")]
	[InlineData("http://m.youtube.com/v/0zM3nApSvMg?fs=1&hl=en_US&rel=0", "0zM3nApSvMg")]
	[InlineData("https://www.youtube.com/v/0zM3nApSvMg?fs=1&amp;hl=en_US&amp;rel=0", "0zM3nApSvMg")]
	[InlineData("https://www.youtube.com/v/0zM3nApSvMg?fs=1&hl=en_US&rel=0", "0zM3nApSvMg")]
	[InlineData("https://youtube.com/v/0zM3nApSvMg?fs=1&hl=en_US&rel=0", "0zM3nApSvMg")]
	[InlineData("https://m.youtube.com/v/0zM3nApSvMg?fs=1&hl=en_US&rel=0", "0zM3nApSvMg")]
	[InlineData("http://www.youtube.com/v/dQw4w9WgXcQ?feature=youtube_gdata_player", "dQw4w9WgXcQ")]
	[InlineData("http://youtube.com/v/dQw4w9WgXcQ?feature=youtube_gdata_player", "dQw4w9WgXcQ")]
	[InlineData("http://m.youtube.com/v/dQw4w9WgXcQ?feature=youtube_gdata_player", "dQw4w9WgXcQ")]
	[InlineData("https://www.youtube.com/v/dQw4w9WgXcQ?feature=youtube_gdata_player", "dQw4w9WgXcQ")]
	[InlineData("https://youtube.com/v/dQw4w9WgXcQ?feature=youtube_gdata_player", "dQw4w9WgXcQ")]
	[InlineData("https://m.youtube.com/v/dQw4w9WgXcQ?feature=youtube_gdata_player", "dQw4w9WgXcQ")]
	[InlineData("http://www.youtube.com/oembed?url=http://www.youtube.com/watch?v=-wtIMTCHWuI&format=json", "-wtIMTCHWuI")]
	[InlineData("http://youtube.com/oembed?url=http://www.youtube.com/watch?v=-wtIMTCHWuI&format=json", "-wtIMTCHWuI")]
	[InlineData("http://m.youtube.com/oembed?url=http://www.youtube.com/watch?v=-wtIMTCHWuI&format=json", "-wtIMTCHWuI")]
	[InlineData("https://www.youtube.com/oembed?url=http://www.youtube.com/watch?v=-wtIMTCHWuI&format=json", "-wtIMTCHWuI")]
	[InlineData("https://youtube.com/oembed?url=http://www.youtube.com/watch?v=-wtIMTCHWuI&format=json", "-wtIMTCHWuI")]
	[InlineData("https://m.youtube.com/oembed?url=http://www.youtube.com/watch?v=-wtIMTCHWuI&format=json", "-wtIMTCHWuI")]
	[InlineData("http://www.youtube.com/attribution_link?a=JdfC0C9V6ZI&u=/watch?v=EhxJLojIE_o&feature=share", "EhxJLojIE_o")]
	[InlineData("http://youtube.com/attribution_link?a=JdfC0C9V6ZI&u=/watch?v=EhxJLojIE_o&feature=share", "EhxJLojIE_o")]
	[InlineData("http://m.youtube.com/attribution_link?a=JdfC0C9V6ZI&u=/watch?v=EhxJLojIE_o&feature=share", "EhxJLojIE_o")]
	[InlineData("https://www.youtube.com/attribution_link?a=JdfC0C9V6ZI&u=/watch?v=EhxJLojIE_o&feature=share", "EhxJLojIE_o")]
	[InlineData("https://youtube.com/attribution_link?a=JdfC0C9V6ZI&u=/watch?v=EhxJLojIE_o&feature=share", "EhxJLojIE_o")]
	[InlineData("https://m.youtube.com/attribution_link?a=JdfC0C9V6ZI&u=/watch?v=EhxJLojIE_o&feature=share", "EhxJLojIE_o")]
	[InlineData("http://www.youtube.com/attribution_link?a=8g8kPrPIi-ecwIsS&u=/watch?v=yZv2daTWRZU&feature=em-uploademail", "yZv2daTWRZU")]
	[InlineData("http://youtube.com/attribution_link?a=8g8kPrPIi-ecwIsS&u=/watch?v=yZv2daTWRZU&feature=em-uploademail", "yZv2daTWRZU")]
	[InlineData("http://m.youtube.com/attribution_link?a=8g8kPrPIi-ecwIsS&u=/watch?v=yZv2daTWRZU&feature=em-uploademail", "yZv2daTWRZU")]
	[InlineData("https://www.youtube.com/attribution_link?a=8g8kPrPIi-ecwIsS&u=/watch?v=yZv2daTWRZU&feature=em-uploademail", "yZv2daTWRZU")]
	[InlineData("https://youtube.com/attribution_link?a=8g8kPrPIi-ecwIsS&u=/watch?v=yZv2daTWRZU&feature=em-uploademail", "yZv2daTWRZU")]
	[InlineData("https://m.youtube.com/attribution_link?a=8g8kPrPIi-ecwIsS&u=/watch?v=yZv2daTWRZU&feature=em-uploademail", "yZv2daTWRZU")]
	[InlineData("http://www.youtube.com/embed/lalOy8Mbfdc", "lalOy8Mbfdc")]
	[InlineData("http://youtube.com/embed/lalOy8Mbfdc", "lalOy8Mbfdc")]
	[InlineData("http://m.youtube.com/embed/lalOy8Mbfdc", "lalOy8Mbfdc")]
	[InlineData("https://www.youtube.com/embed/lalOy8Mbfdc", "lalOy8Mbfdc")]
	[InlineData("https://youtube.com/embed/lalOy8Mbfdc", "lalOy8Mbfdc")]
	[InlineData("https://m.youtube.com/embed/lalOy8Mbfdc", "lalOy8Mbfdc")]
	[InlineData("http://www.youtube.com/embed/nas1rJpm7wY?rel=0", "nas1rJpm7wY")]
	[InlineData("http://youtube.com/embed/nas1rJpm7wY?rel=0", "nas1rJpm7wY")]
	[InlineData("http://m.youtube.com/embed/nas1rJpm7wY?rel=0", "nas1rJpm7wY")]
	[InlineData("https://www.youtube.com/embed/nas1rJpm7wY?rel=0", "nas1rJpm7wY")]
	[InlineData("https://youtube.com/embed/nas1rJpm7wY?rel=0", "nas1rJpm7wY")]
	[InlineData("https://m.youtube.com/embed/nas1rJpm7wY?rel=0", "nas1rJpm7wY")]
	[InlineData("http://www.youtube.com/e/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("http://youtube.com/e/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("http://m.youtube.com/e/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("https://www.youtube.com/e/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("https://youtube.com/e/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("https://m.youtube.com/e/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("http://youtu.be/-wtIMTCHWuI", "-wtIMTCHWuI")]
	[InlineData("https://youtu.be/-wtIMTCHWuI", "-wtIMTCHWuI")]
	[InlineData("http://youtu.be/dQw4w9WgXcQ?feature=youtube_gdata_player", "dQw4w9WgXcQ")]
	[InlineData("https://youtu.be/dQw4w9WgXcQ?feature=youtube_gdata_player", "dQw4w9WgXcQ")]
	[InlineData("http://youtu.be/oTJRivZTMLs?list=PLToa5JuFMsXTNkrLJbRlB--76IAOjRM9b", "oTJRivZTMLs")]
	[InlineData("https://youtu.be/oTJRivZTMLs?list=PLToa5JuFMsXTNkrLJbRlB--76IAOjRM9b", "oTJRivZTMLs")]
	[InlineData("http://youtu.be/oTJRivZTMLs&feature=channel", "oTJRivZTMLs")]
	[InlineData("https://youtu.be/oTJRivZTMLs&feature=channel", "oTJRivZTMLs")]
	[InlineData("http://youtu.be/lalOy8Mbfdc?t=1", "lalOy8Mbfdc")]
	[InlineData("http://youtu.be/lalOy8Mbfdc?t=1s", "lalOy8Mbfdc")]
	[InlineData("https://youtu.be/lalOy8Mbfdc?t=1", "lalOy8Mbfdc")]
	[InlineData("https://youtu.be/lalOy8Mbfdc?t=1s", "lalOy8Mbfdc")]
	[InlineData("http://youtu.be/M9bq_alk-sw?si=B_RZg_I-lLaa7UU-", "M9bq_alk-sw")]
	[InlineData("https://youtu.be/M9bq_alk-sw?si=B_RZg_I-lLaa7UU-", "M9bq_alk-sw")]
	[InlineData("https://www.youtube.com/live/dQw4w9WgXcQ?si=abc", "dQw4w9WgXcQ")]
	[InlineData("https://music.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("https://www.youtube-nocookie.com/embed/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("HTTPS://WWW.YOUTUBE.COM/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("www.youtube.com/shorts/7bzoSi4nzGo", "7bzoSi4nzGo")]
	[InlineData("youtu.be/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
	[InlineData("  https://youtu.be/dQw4w9WgXcQ  ", "dQw4w9WgXcQ")]
	public void TryParse_LinkToOneVideo_ReadsItsId(string link, string expectedVideoId)
	{
		// Act
		var isParsed = YouTubeLink.TryParse(link, out var youTubeLink);

		// Assert
		isParsed.ShouldBeTrue();
		youTubeLink.ShouldNotBeNull();
		youTubeLink.VideoId.ShouldBe(expectedVideoId);
	}

	/// <summary>
	/// The id must be exactly 11 characters, as <see cref="YouTubeVideoId.IsValid"/> and the provider require: a
	/// mistyped link fails here instead of throwing later.
	/// </summary>
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData("not a link")]
	[InlineData("https://www.youtube.com/embed/M7lc1UV")]
	[InlineData("https://www.youtube.com/embed/M7lc1UVf-VEXTRA")]
	[InlineData("https://www.youtube.com/watch?v=M7lc1UV")]
	[InlineData("https://www.youtube.com/watch")]
	[InlineData("https://www.youtube.com/embed/")]
	[InlineData("https://youtu.be/")]
	[InlineData("https://www.youtube.com/embed/videoseries?list=PLx0sYbCqOb8TBPRdmBHs5Iftvv9TPboYG")]
	[InlineData("https://www.youtube.com/embed/live_stream?channel=UC4R8DWoMoI7CAwX8_LjQHig")]
	[InlineData("https://www.youtube.com/playlist?list=PLx0sYbCqOb8TBPRdmBHs5Iftvv9TPboYG")]
	[InlineData("https://www.youtube.com/@YouTube")]
	[InlineData("https://www.youtube.com/dQw4w9WgXcQ")]
	[InlineData("https://vimeo.com/76979871")]
	[InlineData("https://example.com/watch?v=dQw4w9WgXcQ")]
	[InlineData("https://youtube.com.example.com/watch?v=dQw4w9WgXcQ")]
	[InlineData("ftp://youtube.com/watch?v=dQw4w9WgXcQ")]
	[InlineData("javascript:alert('https://youtu.be/dQw4w9WgXcQ')")]
	[InlineData("https://www.youtube.com/oembed?url=https://www.youtube.com/oembed?url=https://youtu.be/dQw4w9WgXcQ")]
	public void TryParse_AnythingElse_ReturnsFalse(string? link)
	{
		// Act
		var isParsed = YouTubeLink.TryParse(link, out var youTubeLink);

		// Assert
		isParsed.ShouldBeFalse();
		youTubeLink.ShouldBeNull();
	}

	[Theory]
	[InlineData("https://www.youtube.com/embed/M7lc1UVf-VE?start=30&end=45", 30, 45)]
	[InlineData("https://www.youtube.com/embed/M7lc1UVf-VE?end=45", null, 45)]
	[InlineData("https://youtu.be/M7lc1UVf-VE?t=90", 90, null)]
	[InlineData("https://youtu.be/M7lc1UVf-VE?t=90s", 90, null)]
	[InlineData("https://www.youtube.com/watch?v=M7lc1UVf-VE&t=1m30s", 90, null)]
	[InlineData("https://www.youtube.com/watch?v=M7lc1UVf-VE&t=1h2m3s", 3723, null)]
	[InlineData("https://www.youtube.com/watch?v=M7lc1UVf-VE#t=0m10s", 10, null)]
	[InlineData("https://www.youtube.com/embed/M7lc1UVf-VE?start=30&t=90", 30, null)]
	[InlineData("https://www.youtube.com/embed/M7lc1UVf-VE?start=abc&end=-5", null, null)]
	[InlineData("https://www.youtube.com/watch?v=M7lc1UVf-VE&t=1x", null, null)]
	[InlineData("https://www.youtube.com/watch?v=M7lc1UVf-VE&t=99999999999", null, null)]
	[InlineData("https://www.youtube.com/watch?v=M7lc1UVf-VE", null, null)]
	public void TryParse_LinkWithRange_ReadsTheRange(string link, int? expectedStartSeconds, int? expectedEndSeconds)
	{
		// Act
		var isParsed = YouTubeLink.TryParse(link, out var youTubeLink);

		// Assert
		isParsed.ShouldBeTrue();
		youTubeLink.ShouldNotBeNull();
		youTubeLink.Start.ShouldBe(ToTimeSpan(expectedStartSeconds));
		youTubeLink.End.ShouldBe(ToTimeSpan(expectedEndSeconds));
	}

	[Fact]
	public void TryParse_TheEmbedUrlItBuilt_ReturnsAnEqualLink()
	{
		// Arrange
		var youTubeLink = new YouTubeLink("M7lc1UVf-VE", TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(45));

		// Act
		var isParsed = YouTubeLink.TryParse(youTubeLink.YouTubeEmbedUrl, out var parsedYouTubeLink);

		// Assert
		youTubeLink.YouTubeEmbedUrl.ShouldBe("https://www.youtube.com/embed/M7lc1UVf-VE?start=30&end=45");
		isParsed.ShouldBeTrue();
		parsedYouTubeLink.ShouldBe(youTubeLink);
	}

	[Fact]
	public void Constructor_FractionOfASecond_DropsIt()
	{
		// Act
		var youTubeLink = new YouTubeLink("M7lc1UVf-VE", TimeSpan.FromSeconds(30.9));

		// Assert
		youTubeLink.Start.ShouldBe(TimeSpan.FromSeconds(30));
		youTubeLink.YouTubeEmbedUrl.ShouldBe("https://www.youtube.com/embed/M7lc1UVf-VE?start=30");
	}

	[Fact]
	public void Constructor_InvalidVideoId_Throws()
	{
		// Act
		var construct = () => new YouTubeLink("M7lc1UV");

		// Assert
		construct.ShouldThrow<ArgumentException>();
	}

	[Fact]
	public void Constructor_NegativeStart_Throws()
	{
		// Act
		var construct = () => new YouTubeLink("M7lc1UVf-VE", TimeSpan.FromSeconds(-1));

		// Assert
		construct.ShouldThrow<ArgumentOutOfRangeException>();
	}

	private static TimeSpan? ToTimeSpan(int? seconds)
	{
		return seconds is not null
			? TimeSpan.FromSeconds(seconds.Value)
			: null;
	}
}
