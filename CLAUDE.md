# CLAUDE.md

Guidance for Claude Code (claude.ai/code) when working in this repository.

`Kebechet.Maui.Hybrid.YouTube.Embed` plays YouTube videos in a .NET MAUI hybrid app (BlazorWebView, HybridWebView)
without Error 153 on iOS and Mac Catalyst. The README explains the why; this file is about working on it.

## Layout

| Path | What it is |
|---|---|
| `src/Maui.Hybrid.YouTube.Embed/` | The package. A Razor class library for `net10.0`, `net10.0-ios` and `net10.0-maccatalyst`, namespace `Kebechet.Maui.Hybrid.YouTube.Embed` |
| `src/Maui.Hybrid.YouTube.Embed/LocalServerYouTubeEmbedUrlProvider.cs` | iOS / Mac Catalyst only: the loopback page on Shiny.Net.HttpServer |
| `src/Maui.Hybrid.YouTube.Embed/DirectYouTubeEmbedUrlProvider.cs` | Every other host: YouTube's own embed URL |
| `src/Maui.Hybrid.YouTube.Embed/YouTubeLink.cs` | Reads the video and range from any YouTube link |
| `tests/Maui.Hybrid.YouTube.Embed.Tests/` | xUnit v3 + Shouldly + bUnit + NSubstitute. The local server tests use real sockets |
| `demo/DemoApp/` | MAUI app (Android + iOS) for device checks: a BlazorWebView tab and a HybridWebView tab. Not in the solution |

The iOS and Mac Catalyst builds carry the local server and Shiny; the `net10.0` build hands out YouTube's URL and has
neither. `AddYouTubeEmbed` picks the provider with `#if IOS || MACCATALYST`. ⚠️ The public API must be the same in all
three builds: a shared Razor library compiles against `net10.0`, and the iOS app runs the iOS build in its place.

⚠️ No `UseMaui` and no MAUI package reference: the `net10.0` build would carry MAUI into Blazor WebAssembly.

The tests run on `net10.0`, where the package has no local server, so the test project compiles
`LocalServerYouTubeEmbedUrlProvider.cs` itself (a linked source file) together with Shiny.

## Build & test

Needs the `ios` and `maccatalyst` workloads, so CI runs on `windows-latest` (they do not install on Linux).

```bash
dotnet build src/Maui.Hybrid.YouTube.Embed.slnx -c Release
dotnet test src/Maui.Hybrid.YouTube.Embed.slnx -c Release
```

## What the tests cannot see

Only a device shows whether YouTube plays. The unit tests prove the server's contract, not that WebKit loads the page,
that YouTube accepts the Referer, that the web views keep the iframes, or that an iOS app picks the iOS build. Verify on
a device:

- **iOS:** build `demo/DemoApp` for `net10.0-ios` on a Mac with a wildcard development profile (the app id is
  `com.kebechet.youtubeembeddemo`), launch it, tap play on "Through the package" in both tabs. The control iframe below
  it must show Error 153, which proves the page is the thing making the difference. Background the app for a minute,
  come back, tap "Next range" and play again: that exercises the listener check after a suspension.
- **Android:** `-p:EmbedAssembliesIntoApk=true` for a Debug APK installed with `adb install -r`, or the app runs stale
  assemblies.

⚠️ The demo's `MainPage` `UrlLoading` handler is required, not decoration: without it MAUI on iOS opens every iframe of
a BlazorWebView in Safari. Consumers need the same handler; the README says so.
