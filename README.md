[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/kebechet)

# Maui.YouTubeEmbed
[![NuGet Version](https://img.shields.io/nuget/v/Kebechet.Maui.YouTubeEmbed)](https://www.nuget.org/packages/Kebechet.Maui.YouTubeEmbed/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Kebechet.Maui.YouTubeEmbed)](https://www.nuget.org/packages/Kebechet.Maui.YouTubeEmbed/)
[![Build](https://github.com/Kebechet/Maui.YouTubeEmbed/actions/workflows/build.yml/badge.svg)](https://github.com/Kebechet/Maui.YouTubeEmbed/actions/workflows/build.yml)
[![codecov](https://codecov.io/gh/Kebechet/Maui.YouTubeEmbed/graph/badge.svg)](https://codecov.io/gh/Kebechet/Maui.YouTubeEmbed)
![Last updated](https://img.shields.io/github/last-commit/Kebechet/Maui.YouTubeEmbed/main?label=last%20updated)
[![Twitter](https://img.shields.io/twitter/url/https/twitter.com/samuel_sidor.svg?style=social&label=Follow%20samuel_sidor)](https://x.com/samuel_sidor)

Plays YouTube videos inside a .NET MAUI Blazor Hybrid app without YouTube's **Error 153** on iOS and Mac Catalyst,
and without a proxy page on your own website. One component and one URL provider for every platform.

<img src="https://raw.githubusercontent.com/Kebechet/Maui.YouTubeEmbed/main/docs/ios-package-vs-plain-iframe.png" alt="The same video on an iPhone: the package's player is ready to play, a plain YouTube iframe shows Error 153" width="360" />

*An iPhone running the demo app: the package's player above, a plain YouTube iframe with Error 153 ("Chyba 153" on
this Czech phone) below.*

## Contents

- [The problem](#the-problem)
- [Why the usual fixes do not work in a BlazorWebView](#why-the-usual-fixes-do-not-work-in-a-blazorwebview)
- [How other frameworks deal with it](#how-other-frameworks-deal-with-it)
- [What this package does](#what-this-package-does)
- [Installation](#installation) and [usage](#usage)
- [How the local server behaves](#how-the-local-server-behaves)
- [Load time compared with a proxy page](#load-time-compared-with-a-proxy-page)
- [Trade-off](#trade-off) and [tested devices](#tested-on)

## The problem

Put a YouTube `<iframe>` into a MAUI Blazor Hybrid app and it plays on Android and Windows, but on iOS and Mac Catalyst
it shows *"Error 153 - Video player configuration error"*. Three facts combine:

1. **YouTube requires a `Referer` header on the embed request.** Its
   [Required Minimum Functionality](https://developers.google.com/youtube/terms/required-minimum-functionality#embedded-player-api-client-identity)
   says API clients that use the embedded player "must provide identification through the HTTP Referer request header".
   The player refuses a request without one, and Error 153 is how it says so
   ([Simon Willison's write-up](https://til.simonwillison.net/youtube/fixing-153-embed) covers the website side of it).
2. **A BlazorWebView on iOS and Mac Catalyst serves the app from a custom scheme, `app://0.0.0.1`**
   ([`BlazorWebViewHandler.iOS.cs`](https://github.com/dotnet/maui/blob/7807f3dc65038dff3c2185a1730f4ff73b487f65/src/BlazorWebView/src/Maui/iOS/BlazorWebViewHandler.iOS.cs#L30)).
   On Android and Windows it is `https://0.0.0.1`
   ([`AndroidWebKitWebViewManager.cs`](https://github.com/dotnet/maui/blob/7807f3dc65038dff3c2185a1730f4ff73b487f65/src/BlazorWebView/src/Maui/Android/AndroidWebKitWebViewManager.cs#L25)),
   which is why those platforms are not affected.
3. **WebKit sends a `Referer` only from an `http` or `https` document.** A page on `app://` sends none, whatever its
   referrer policy says
   ([`SecurityPolicy::generateReferrerHeader`](https://github.com/WebKit/WebKit/blob/bc9227468be6ba96e8353429081befffeaf14964/Source/WebCore/page/SecurityPolicy.cpp#L94)).

## Why the usual fixes do not work in a BlazorWebView

YouTube's own guidance for apps
([Set the Referer](https://developers.google.com/youtube/terms/required-minimum-functionality#set_the_referer)) assumes
you control the WebView that loads the player: give `loadHTMLString:baseURL:` an `https` base URL, or load
`youtube.com/embed/...` with `loadRequest:` and a `Referer` header. A BlazorWebView's page is MAUI's `app://` document,
and the player is an iframe inside it, so neither applies. What else was checked, on an iPhone with iOS 26.2:

| Idea | Why it fails |
|---|---|
| `referrerpolicy="unsafe-url"`, `origin` / `widget_referrer` player parameters | Nothing to send: the `app://` document has no Referer to give, and YouTube checks only the header |
| Serve the app from `https://` instead of `app://` | WebKit refuses a URL scheme handler for a scheme it handles itself: "It is a programmer error to register a handler for a scheme WebKit already handles, such as https" ([`setURLSchemeHandler(_:forURLScheme:)`](https://developer.apple.com/documentation/webkit/wkwebviewconfiguration/seturlschemehandler(_:forurlscheme:))) |
| Intercept the iframe request and add the header | `BlazorWebView.WebResourceRequested` cannot see `https` requests on iOS, and `WKWebView` has no public API to change a subframe's request headers |
| `WKContentRuleList` with a `modify-headers` action | Ignored unless a private WebKit setting activates it |
| `WKWebExtension` `declarativeNetRequest` rules ([used on macOS 15.4+](https://github.com/filipnavara/wkwebview-http-interceptor)) | No rule fired on iOS 26.2, not even a plain redirect |
| Private API (`_setOverrideReferrerForAllRequests:` and similar) | Works, but App Review rejects private API use |

## How other frameworks deal with it

Every hybrid framework that serves its app from a custom scheme on iOS runs into the same wall:

| Framework | Origin on iOS | What projects do |
|---|---|---|
| **Capacitor / Ionic** | `capacitor://localhost`, `ionic://localhost` | A proxy page on their own website ([example](https://dev.to/davidvesely/fixing-youtube-error-153-in-ios-capacitor-apps-a-simple-proxy-solution-607)), or a native player plugin ([Cap-go/capacitor-youtube-player](https://github.com/Cap-go/capacitor-youtube-player)). The framework issue ([ionic-team/capacitor#8205](https://github.com/ionic-team/capacitor/issues/8205)) was closed without a framework change; its last answer points to that plugin |
| **Tauri** | `tauri://localhost` | A page served from a local HTTP server that embeds the iframe, suggested in [tauri-apps/tauri#14422](https://github.com/tauri-apps/tauri/issues/14422) and shipped with full hardening in [eigendeck](https://github.com/dgleich/eigendeck/issues/152) - the approach this package takes |
| **A plain `WKWebView` / MAUI `WebView`** | Whatever you load | YouTube's `loadHTMLString:baseURL:` technique. In MAUI that is `HtmlWebViewSource.BaseUrl`, which iOS ignores until [dotnet/maui#40150](https://github.com/dotnet/maui/pull/40150) fixes [dotnet/maui#19792](https://github.com/dotnet/maui/issues/19792) |
| **MAUI Blazor Hybrid** | `app://0.0.0.1` | Until now, a proxy page on your own website. This package moves that page into the app |

A proxy page on your website works, but it is one more thing to host, it fails when the site is down or slow, and every
video load waits for your server first.

## What this package does

On iOS and Mac Catalyst it serves the embed page from a small HTTP server inside the app, listening on `127.0.0.1`
([Shiny.Net.HttpServer](https://github.com/shinyorg/httpserver)). That page is an `http` document, so the YouTube iframe
in it is requested with `Referer: http://127.0.0.1:<port>/`. Everywhere else the iframe loads YouTube directly.

```
iOS, Mac Catalyst
  app://0.0.0.1 (your Blazor page)
    └─ <iframe src="http://127.0.0.1:<port>/<token>/<videoId>">   ← served by the app itself
         └─ <iframe src="https://www.youtube.com/embed/<videoId>">   Referer: http://127.0.0.1:<port>/

Android, Windows, Blazor WebAssembly, any website
  <iframe src="https://www.youtube.com/embed/<videoId>">
```

The package is one `net10.0` assembly and picks the provider at runtime, so a Razor class library shared between a
Blazor WebAssembly site and a MAUI app references it once.

## Installation

```bash
dotnet add package Kebechet.Maui.YouTubeEmbed
```

⚠️ Requires .NET 10 and `Microsoft.Extensions.*` **10.0.10 or later**, the floor of
[Shiny.Net.HttpServer](https://www.nuget.org/packages/Shiny.Net.HttpServer). An app that pins an older version (with
central package management, say) gets `NU1605: Detected package downgrade` and has to raise those pins.

## Usage

### 1. Register the provider

```csharp
builder.Services.AddYouTubeEmbed();
```

In `MauiProgram`, and in the `Program` of a Blazor WebAssembly app that shares the same components.

### 2. ⚠️ Keep the embeds in the WebView (MAUI, iOS and Mac Catalyst)

⚠️ **Without this step the video never shows on iOS.** A BlazorWebView on iOS and Mac Catalyst raises `UrlLoading` for
every frame, iframes included, and by default opens any URL outside the app in the browser
([`IOSWebViewManager.DecidePolicy`](https://github.com/dotnet/maui/blob/7807f3dc65038dff3c2185a1730f4ff73b487f65/src/BlazorWebView/src/Maui/iOS/IOSWebViewManager.cs#L282),
[routing docs](https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/routing?view=aspnetcore-10.0)). Ask the
provider which URLs belong to an embed:

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

`Start` and `End` are optional and whole seconds (a fraction is dropped) - YouTube's
[`start` and `end` player parameters](https://developers.google.com/youtube/player_parameters#start). Every other
attribute (`class`, `style`, `loading`, ...) goes to the `<iframe>`.

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
  id of exactly 11 characters from `A-Z a-z 0-9 - _`. Anything else is a `404` and any method but `GET` a `405`.
- A request whose `Host` is not `127.0.0.1` gets a `400` before routing (DNS rebinding), through Shiny's
  `HostFilteringMiddleware` narrowed to that one address - `localhost` and other IP addresses are refused too.
- The page sends `Content-Security-Policy` (YouTube frames only), `Referrer-Policy: strict-origin-when-cross-origin`
  (YouTube gets the origin, never the token), `X-Content-Type-Options: nosniff` and `Cache-Control: no-store`.
- iOS may reclaim a suspended app's listening socket
  ([TN2277](https://developer.apple.com/library/archive/technotes/tn2277/_index.html)). Before handing out a URL the
  provider checks that the listener still accepts, and restarts the server on the same port if it does not.
- No `Info.plist` entry is needed: no App Transport Security exception, no local network usage description, and no
  local network permission prompt appears (verified on iOS 26.2).

## Load time compared with a proxy page

Measured in the demo app: an iframe inserted, timed to its `load` event, which waits for YouTube's player document
inside it. 10 rounds per approach after two warm-up rounds, alternating which approach went first. The proxy is an
existing page on a production website that embeds the same video; it redirects a Czech phone to a localized URL first.

| Device | Proxy page | This package | |
|---|---:|---:|---:|
| iPhone 11, iOS 26.2 (local server) | 522 ms | 327 ms | **-37%** |
| Samsung Galaxy A53, Android 16 (direct embed) | 687 ms | 434 ms | **-37%** |
| First load of each in the run, iPhone / Samsung (the proxy page's own files not yet cached) | 866 / 1206 ms | 307 / 466 ms | |

The local page arrives in about 7 ms; handing out its URL (the listener check) takes about 10 ms.

## Trade-off

⚠️ YouTube receives `http://127.0.0.1:<port>/` as the embedding site. Its
[Referer format](https://developers.google.com/youtube/terms/required-minimum-functionality#referer_format) asks apps
to send `https://<bundle id>`, and an iOS BlazorWebView has no public way to send that for an iframe. YouTube plays it
today. A proxy page on a website does not send the bundle id either: YouTube then sees the website's address.

## Tested on

| Device | Result |
|---|---|
| iPhone 11, iOS 26.2 | The package's player plays the range inline, also after 75 s in the background; a plain YouTube iframe on the same page shows Error 153 |
| Samsung Galaxy A53, Android 16 | Plays the range |

Mac Catalyst and Windows use the same code paths but have not been run on a device.

## License

[MIT](LICENSE)
