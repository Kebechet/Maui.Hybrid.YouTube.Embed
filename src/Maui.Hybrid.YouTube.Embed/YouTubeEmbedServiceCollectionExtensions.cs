using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kebechet.Maui.Hybrid.YouTube.Embed;

/// <summary>
/// Registers <see cref="IYouTubeEmbedUrlProvider"/>.
/// </summary>
public static class YouTubeEmbedServiceCollectionExtensions
{
	/// <summary>
	/// Registers <see cref="IYouTubeEmbedUrlProvider"/> for the platform the app is built for. In the iOS and Mac
	/// Catalyst builds of this package it serves the player from a server inside the app, started on first use; every
	/// other build embeds YouTube directly. Call it once, from the place that registers the app's services.
	/// </summary>
	/// <exception cref="PlatformNotSupportedException">
	/// The app runs on iOS or Mac Catalyst with the net10.0 build of this package, where every video would show Error 153.
	/// </exception>
	public static IServiceCollection AddYouTubeEmbed(this IServiceCollection services)
	{
#if IOS || MACCATALYST
		services.TryAddSingleton<IYouTubeEmbedUrlProvider, LocalServerYouTubeEmbedUrlProvider>();
#else
		// NuGet hands an app the net10.0 build when its own target is older than this package's iOS or Mac Catalyst
		// one. That build cannot serve the page, so stop here rather than play every video into Error 153.
		if (OperatingSystem.IsIOS() || OperatingSystem.IsMacCatalyst())
		{
			throw new PlatformNotSupportedException("The app runs the net10.0 build of Kebechet.Maui.Hybrid.YouTube.Embed on iOS or Mac Catalyst, so YouTube would show Error 153. Target net10.0-ios or net10.0-maccatalyst with an SDK at least as new as the one this package was built with.");
		}

		services.TryAddSingleton<IYouTubeEmbedUrlProvider, DirectYouTubeEmbedUrlProvider>();
#endif
		return services;
	}
}
