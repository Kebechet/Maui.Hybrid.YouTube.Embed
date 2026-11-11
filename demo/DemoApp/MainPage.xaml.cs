using Kebechet.Maui.YouTubeEmbed;
using Microsoft.AspNetCore.Components.WebView;

namespace DemoApp;

public partial class MainPage : ContentPage, IDisposable
{
	private readonly IYouTubeEmbedUrlProvider _youTubeEmbedUrlProvider;

	public MainPage(IYouTubeEmbedUrlProvider youTubeEmbedUrlProvider)
	{
		_youTubeEmbedUrlProvider = youTubeEmbedUrlProvider;
		InitializeComponent();
		blazorWebView.UrlLoading += OnUrlLoading;
	}

	public void Dispose()
	{
		blazorWebView.UrlLoading -= OnUrlLoading;
	}

	/// <summary>
	/// On iOS the BlazorWebView raises UrlLoading for iframes too, and opens anything outside the app in the browser
	/// by default; the embeds stay in the WebView.
	/// </summary>
	private void OnUrlLoading(object? sender, UrlLoadingEventArgs urlLoadingEventArgs)
	{
		if (_youTubeEmbedUrlProvider.IsEmbedUrl(urlLoadingEventArgs.Url))
		{
			urlLoadingEventArgs.UrlLoadingStrategy = UrlLoadingStrategy.OpenInWebView;
		}
	}
}
