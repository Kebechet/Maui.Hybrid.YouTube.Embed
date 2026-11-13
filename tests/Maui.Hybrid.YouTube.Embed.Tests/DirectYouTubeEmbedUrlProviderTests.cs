using Shouldly;
using Xunit;

namespace Kebechet.Maui.Hybrid.YouTube.Embed.Tests;

public class DirectYouTubeEmbedUrlProviderTests
{
	private readonly DirectYouTubeEmbedUrlProvider _directYouTubeEmbedUrlProvider = new();

	[Fact]
	public async Task GetEmbedUrlAsync_VideoIdOnly_ReturnsYouTubeEmbedUrl()
	{
		// Act
		var embedUrl = await _directYouTubeEmbedUrlProvider.GetEmbedUrlAsync("M7lc1UVf-VE", cancellationToken: TestContext.Current.CancellationToken);

		// Assert
		embedUrl.ShouldBe("https://www.youtube.com/embed/M7lc1UVf-VE");
	}

	[Fact]
	public async Task GetEmbedUrlAsync_WithRange_AppendsStartAndEndInWholeSeconds()
	{
		// Act
		var embedUrl = await _directYouTubeEmbedUrlProvider.GetEmbedUrlAsync("M7lc1UVf-VE", TimeSpan.FromSeconds(30.9), TimeSpan.FromMinutes(1), TestContext.Current.CancellationToken);

		// Assert
		embedUrl.ShouldBe("https://www.youtube.com/embed/M7lc1UVf-VE?start=30&end=60");
	}

	[Fact]
	public async Task GetEmbedUrlAsync_EndOnly_AppendsEnd()
	{
		// Act
		var embedUrl = await _directYouTubeEmbedUrlProvider.GetEmbedUrlAsync("M7lc1UVf-VE", end: TimeSpan.FromSeconds(45), cancellationToken: TestContext.Current.CancellationToken);

		// Assert
		embedUrl.ShouldBe("https://www.youtube.com/embed/M7lc1UVf-VE?end=45");
	}

	[Fact]
	public async Task GetEmbedUrlAsync_InvalidVideoId_ThrowsArgumentException()
	{
		// Act
		var getEmbedUrl = async () => await _directYouTubeEmbedUrlProvider.GetEmbedUrlAsync("not-an-id", cancellationToken: TestContext.Current.CancellationToken);

		// Assert
		var exception = await getEmbedUrl.ShouldThrowAsync<ArgumentException>();
		exception.ParamName.ShouldBe("videoId");
	}

	[Theory]
	[InlineData("https://www.youtube.com/embed/M7lc1UVf-VE")]
	[InlineData("https://www.youtube.com/embed/M7lc1UVf-VE?start=30&end=45")]
	public void IsEmbedUrl_YouTubeEmbedPlayer_ReturnsTrue(string url)
	{
		// Act
		var isEmbedUrl = _directYouTubeEmbedUrlProvider.IsEmbedUrl(new Uri(url));

		// Assert
		isEmbedUrl.ShouldBeTrue();
	}

	[Theory]
	[InlineData("https://www.youtube.com/watch?v=M7lc1UVf-VE")]
	[InlineData("https://www.youtube.com/embedded/M7lc1UVf-VE")]
	[InlineData("http://www.youtube.com/embed/M7lc1UVf-VE")]
	[InlineData("https://www.youtube.com.example/embed/M7lc1UVf-VE")]
	[InlineData("https://example.com/embed/M7lc1UVf-VE")]
	[InlineData("about:blank")]
	public void IsEmbedUrl_AnythingElse_ReturnsFalse(string url)
	{
		// Act
		var isEmbedUrl = _directYouTubeEmbedUrlProvider.IsEmbedUrl(new Uri(url));

		// Assert
		isEmbedUrl.ShouldBeFalse();
	}

	[Fact]
	public async Task GetEmbedUrlAsync_NegativeStart_ThrowsArgumentOutOfRangeException()
	{
		// Act
		var getEmbedUrl = async () => await _directYouTubeEmbedUrlProvider.GetEmbedUrlAsync("M7lc1UVf-VE", TimeSpan.FromSeconds(-1), cancellationToken: TestContext.Current.CancellationToken);

		// Assert
		var exception = await getEmbedUrl.ShouldThrowAsync<ArgumentOutOfRangeException>();
		exception.ParamName.ShouldBe("start");
	}
}
