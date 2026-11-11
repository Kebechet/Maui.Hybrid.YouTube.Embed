using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Xunit;
using BunitContext = Bunit.TestContext;

namespace Kebechet.Maui.YouTubeEmbed.Tests;

public sealed class YouTubeEmbedTests : IDisposable
{
	private const string VideoId = "M7lc1UVf-VE";
	private const string EmbedUrl = "http://127.0.0.1:5555/token/M7lc1UVf-VE?start=30&end=45";

	private readonly BunitContext _bunitContext = new();
	private readonly IYouTubeEmbedUrlProvider _youTubeEmbedUrlProvider = Substitute.For<IYouTubeEmbedUrlProvider>();

	public YouTubeEmbedTests()
	{
		_youTubeEmbedUrlProvider
			.GetEmbedUrlAsync(Arg.Any<string>(), Arg.Any<TimeSpan?>(), Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(EmbedUrl));
		_bunitContext.Services.AddSingleton(_youTubeEmbedUrlProvider);
	}

	public void Dispose()
	{
		_bunitContext.Dispose();
	}

	[Fact]
	public void Render_WithRange_LoadsProviderUrlInIframe()
	{
		// Act
		var renderedComponent = _bunitContext.RenderComponent<YouTubeEmbed>(parameters => parameters
			.Add(x => x.VideoId, VideoId)
			.Add(x => x.Start, TimeSpan.FromSeconds(30))
			.Add(x => x.End, TimeSpan.FromSeconds(45)));

		// Assert
		var iframe = renderedComponent.Find("iframe");
		iframe.GetAttribute("src").ShouldBe(EmbedUrl);
		iframe.GetAttribute("allow").ShouldBe(YouTubeEmbedPage.AllowedFeatures);
		iframe.HasAttribute("allowfullscreen").ShouldBeTrue();
		_youTubeEmbedUrlProvider.Received(1).GetEmbedUrlAsync(VideoId, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(45), Arg.Any<CancellationToken>());
	}

	[Fact]
	public void Render_AdditionalAttributes_PassesThemToIframe()
	{
		// Act
		var renderedComponent = _bunitContext.RenderComponent<YouTubeEmbed>(parameters => parameters
			.Add(x => x.VideoId, VideoId)
			.Add(x => x.Title, "Squat tutorial")
			.AddUnmatched("class", "video")
			.AddUnmatched("loading", "lazy"));

		// Assert
		var iframe = renderedComponent.Find("iframe");
		iframe.GetAttribute("title").ShouldBe("Squat tutorial");
		iframe.GetAttribute("class").ShouldBe("video");
		iframe.GetAttribute("loading").ShouldBe("lazy");
	}

	[Fact]
	public void SetParameters_SamePlayback_DoesNotAskForUrlAgain()
	{
		// Arrange
		var renderedComponent = _bunitContext.RenderComponent<YouTubeEmbed>(parameters => parameters
			.Add(x => x.VideoId, VideoId));

		// Act
		renderedComponent.SetParametersAndRender(parameters => parameters
			.Add(x => x.Title, "Another title"));

		// Assert
		_youTubeEmbedUrlProvider.Received(1).GetEmbedUrlAsync(Arg.Any<string>(), Arg.Any<TimeSpan?>(), Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
	}

	[Fact]
	public void SetParameters_DifferentVideo_AsksForNewUrl()
	{
		// Arrange
		var renderedComponent = _bunitContext.RenderComponent<YouTubeEmbed>(parameters => parameters
			.Add(x => x.VideoId, VideoId));

		// Act
		renderedComponent.SetParametersAndRender(parameters => parameters
			.Add(x => x.VideoId, "dQw4w9WgXcQ"));

		// Assert
		_youTubeEmbedUrlProvider.Received(1).GetEmbedUrlAsync("dQw4w9WgXcQ", Arg.Any<TimeSpan?>(), Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
	}
}
