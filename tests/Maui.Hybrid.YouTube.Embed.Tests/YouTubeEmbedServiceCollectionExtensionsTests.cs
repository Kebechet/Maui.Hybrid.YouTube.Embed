using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Kebechet.Maui.Hybrid.YouTube.Embed.Tests;

public class YouTubeEmbedServiceCollectionExtensionsTests
{
	[Fact]
	public void AddYouTubeEmbed_NotOnAppleWebKitHost_RegistersDirectProvider()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddYouTubeEmbed();

		// Assert
		using var serviceProvider = services.BuildServiceProvider();
		serviceProvider.GetRequiredService<IYouTubeEmbedUrlProvider>().ShouldBeOfType<DirectYouTubeEmbedUrlProvider>();
	}

	[Fact]
	public void AddYouTubeEmbed_CalledTwice_KeepsOneRegistration()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddYouTubeEmbed();
		services.AddYouTubeEmbed();

		// Assert
		services
			.Count(x => x.ServiceType == typeof(IYouTubeEmbedUrlProvider))
			.ShouldBe(1);
	}
}
