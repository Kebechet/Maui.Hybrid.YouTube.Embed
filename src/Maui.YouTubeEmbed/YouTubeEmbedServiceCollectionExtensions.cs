using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kebechet.Maui.YouTubeEmbed;

/// <summary>
/// Registers <see cref="IYouTubeEmbedUrlProvider"/>.
/// </summary>
public static class YouTubeEmbedServiceCollectionExtensions
{
	/// <summary>
	/// Registers <see cref="IYouTubeEmbedUrlProvider"/> for the platform the app runs on. On iOS and Mac Catalyst it
	/// serves the player from a server inside the app, started on first use; everywhere else it embeds YouTube
	/// directly. Call it once, from the place that registers the app's services.
	/// </summary>
	public static IServiceCollection AddYouTubeEmbed(this IServiceCollection services)
	{
		if (OperatingSystem.IsIOS() || OperatingSystem.IsMacCatalyst())
		{
			services.TryAddSingleton<IYouTubeEmbedUrlProvider, LocalServerYouTubeEmbedUrlProvider>();
			return services;
		}

		services.TryAddSingleton<IYouTubeEmbedUrlProvider, DirectYouTubeEmbedUrlProvider>();
		return services;
	}
}
