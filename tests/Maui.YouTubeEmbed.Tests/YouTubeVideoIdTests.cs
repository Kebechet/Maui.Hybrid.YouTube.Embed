using Shouldly;
using Xunit;

namespace Kebechet.Maui.YouTubeEmbed.Tests;

public class YouTubeVideoIdTests
{
	[Theory]
	[InlineData("M7lc1UVf-VE")]
	[InlineData("dQw4w9WgXcQ")]
	[InlineData("___________")]
	public void IsValid_ElevenAllowedCharacters_ReturnsTrue(string videoId)
	{
		// Act
		var isValid = YouTubeVideoId.IsValid(videoId);

		// Assert
		isValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("M7lc1UVf-V")]
	[InlineData("M7lc1UVf-VEE")]
	[InlineData("M7lc1UVf-VE\n")]
	[InlineData("M7lc1UVf VE")]
	[InlineData("M7lc1UVf/VE")]
	[InlineData("<script>xx")]
	public void IsValid_AnythingElse_ReturnsFalse(string? videoId)
	{
		// Act
		var isValid = YouTubeVideoId.IsValid(videoId);

		// Assert
		isValid.ShouldBeFalse();
	}
}
