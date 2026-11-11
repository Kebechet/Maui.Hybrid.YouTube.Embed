[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/kebechet)

# Maui.YouTubeEmbed
[![NuGet Version](https://img.shields.io/nuget/v/Kebechet.Maui.YouTubeEmbed)](https://www.nuget.org/packages/Kebechet.Maui.YouTubeEmbed/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Kebechet.Maui.YouTubeEmbed)](https://www.nuget.org/packages/Kebechet.Maui.YouTubeEmbed/)
[![Build](https://github.com/Kebechet/Maui.YouTubeEmbed/actions/workflows/build.yml/badge.svg)](https://github.com/Kebechet/Maui.YouTubeEmbed/actions/workflows/build.yml)
[![codecov](https://codecov.io/gh/Kebechet/Maui.YouTubeEmbed/graph/badge.svg)](https://codecov.io/gh/Kebechet/Maui.YouTubeEmbed)
![Last updated](https://img.shields.io/github/last-commit/Kebechet/Maui.YouTubeEmbed/main?label=last%20updated)
[![Twitter](https://img.shields.io/twitter/url/https/twitter.com/samuel_sidor.svg?style=social&label=Follow%20samuel_sidor)](https://x.com/samuel_sidor)

Plays YouTube videos inside a .NET MAUI Blazor Hybrid app without YouTube's **Error 153** on iOS and Mac Catalyst,
and without a proxy page on your own domain. One component and one URL provider for every platform.

## The problem

A YouTube iframe in a MAUI `BlazorWebView` on iOS shows *"Error 153 - video player configuration error"*. YouTube
refuses an embed request that carries no `Referer` header. A `BlazorWebView` on iOS and Mac Catalyst serves the app from
`app://0.0.0.1`, and WebKit sends a `Referer` only from `http(s)` documents. WebKit has no public API to set the header,
and it refuses a URL scheme handler for `http` or `https`. Android and Windows serve the app from `https://0.0.0.1`, so
they are not affected.

The usual workaround is a proxy page on your own website that embeds the video, which the app then iframes. This
package moves that page into the app: on iOS and Mac Catalyst it serves it from a small HTTP server on `127.0.0.1`
([Shiny.Net.HttpServer](https://github.com/shinyorg/httpserver)), so YouTube receives `Referer: http://127.0.0.1:<port>/`.

| Host | URL the iframe loads |
|---|---|
| iOS, Mac Catalyst | `http://127.0.0.1:<port>/<token>/<videoId>?start=&end=` - a page that embeds YouTube's player |
| Android, Windows, Blazor WebAssembly, any website | `https://www.youtube.com/embed/<videoId>?start=&end=` |

## Installation

```bash
dotnet add package Kebechet.Maui.YouTubeEmbed
```

## Usage

### 1. Register the provider

```csharp
builder.Services.AddYouTubeEmbed();
```

In `MauiProgram`, and in the `Program` of a Blazor WebAssembly app that shares the same components.

### 2. ⚠️ Keep the embeds in the WebView (MAUI, iOS and Mac Catalyst)

⚠️ **Without this step the video never shows on iOS.** A `BlazorWebView` on iOS and Mac Catalyst raises `UrlLoading` for
every frame, iframes included, and by default opens any URL outside the app in the browser. Ask the provider which URLs
belong to an embed:

```csharp
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

    private void OnUrlLoading(object? sender, UrlLoadingEventArgs urlLoadingEventArgs)
    {
        if (_youTubeEmbedUrlProvider.IsEmbedUrl(urlLoadingEventArgs.Url))
        {
            urlLoadingEventArgs.UrlLoadingStrategy = UrlLoadingStrategy.OpenInWebView;
        }
    }
}
```

### 3. Show a video

```razor
@using Kebechet.Maui.YouTubeEmbed

<YouTubeEmbed VideoId="M7lc1UVf-VE"
              Start="TimeSpan.FromSeconds(30)"
              End="TimeSpan.FromSeconds(45)"
              class="video" />
```

`Start` and `End` are optional and whole seconds (a fraction is dropped). Every other attribute (`class`, `style`,
`loading`, ...) goes to the `<iframe>`.

To render the iframe yourself (a loading skeleton, a tap-to-play overlay), ask for the URL instead:

```razor
@inject IYouTubeEmbedUrlProvider YouTubeEmbedUrlProvider

<iframe src="@_embedUrl" allow="autoplay; encrypted-media; fullscreen; picture-in-picture" allowfullscreen></iframe>

@code {
    private string? _embedUrl;

    protected override async Task OnInitializedAsync()
    {
        _embedUrl = await YouTubeEmbedUrlProvider.GetEmbedUrlAsync("M7lc1UVf-VE", TimeSpan.FromSeconds(30));
    }
}
```

Ask for the URL each time rather than storing it: on iOS it is valid while the app runs.

## How the local server behaves

- It starts on the first `GetEmbedUrlAsync` call, not at app start.
- It listens on `127.0.0.1` only, on a free port, and answers one route: a per-launch random path token, then a video
  id of exactly 11 characters from `A-Z a-z 0-9 - _`. Anything else is a `404`, a `Host` header other than
  `127.0.0.1:<port>` (DNS rebinding) a `400`, any method but `GET` a `405`.
- The page sends `Content-Security-Policy` (YouTube frames only), `Referrer-Policy: strict-origin-when-cross-origin`
  (YouTube gets the origin, never the token), `X-Content-Type-Options: nosniff` and `Cache-Control: no-store`.
- iOS may reclaim a suspended app's listening socket. Before handing out a URL the provider checks that the listener
  still accepts, and restarts the server on the same port if it does not.
- No `Info.plist` entry is needed: no App Transport Security exception, no local network usage description, and no
  local network permission prompt appears (verified on iOS 26.2).

⚠️ **Trade-off:** YouTube receives `http://127.0.0.1:<port>/` as the embedding site. Google asks embedding apps to
identify themselves ([Required Minimum Functionality](https://developers.google.com/youtube/terms/required-minimum-functionality)),
and an iOS app has no public way to send its own identity from a `BlazorWebView`.

## Tested on

| Device | Result |
|---|---|
| iPhone 11, iOS 26.2 | The package's player plays the range inline, also after 75 s in the background; a plain YouTube iframe on the same page shows Error 153 |
| Samsung Galaxy A53, Android 16 | Plays the range |

Mac Catalyst and Windows use the same code paths but have not been run on a device.

## License

[MIT](LICENSE)
