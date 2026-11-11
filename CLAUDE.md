# CLAUDE.md

Guidance for Claude Code (claude.ai/code) when working in this repository.

`Kebechet.Maui.YouTubeEmbed` plays YouTube videos in a .NET MAUI Blazor Hybrid app without Error 153 on iOS and Mac
Catalyst. The README explains the why; this file is about working on it.

## Layout

| Path | What it is |
|---|---|
| `src/Maui.YouTubeEmbed/` | The package. One `net10.0` Razor class library, namespace `Kebechet.Maui.YouTubeEmbed` |
| `src/Maui.YouTubeEmbed/LocalServerYouTubeEmbedUrlProvider.cs` | iOS / Mac Catalyst: the loopback page on Shiny.Net.HttpServer |
| `src/Maui.YouTubeEmbed/DirectYouTubeEmbedUrlProvider.cs` | Everything else: YouTube's own embed URL |
| `tests/Maui.YouTubeEmbed.Tests/` | xUnit v3 + Shouldly + bUnit + NSubstitute. The local server tests use real sockets |
| `demo/DemoApp/` | MAUI Blazor Hybrid app (Android + iOS) for device checks. Not in the solution |

The package is deliberately one `net10.0` assembly, not per-platform targets: a shared Razor library (Blazor
WebAssembly + MAUI) references it once, and `AddYouTubeEmbed` picks the provider at runtime with
`OperatingSystem.IsIOS() || OperatingSystem.IsMacCatalyst()`.

## Build & test

```bash
dotnet build src/Maui.YouTubeEmbed.slnx -c Release
dotnet test src/Maui.YouTubeEmbed.slnx -c Release
```

## What the tests cannot see

Only a device shows whether YouTube plays. The unit tests prove the server's contract, not that WebKit loads the page,
that YouTube accepts the Referer, or that the BlazorWebView keeps the iframes. Verify on a device:

- **iOS:** build `demo/DemoApp` for `net10.0-ios` on a Mac with a wildcard development profile (the app id is
  `com.kebechet.youtubeembeddemo`), launch it, tap play on "Through the package". The control iframe below it must show
  Error 153, which proves the page is the thing making the difference. Background the app for a minute, come back, tap
  "Next range" and play again: that exercises the listener check after a suspension.
- **Android:** `-p:EmbedAssembliesIntoApk=true` for a Debug APK installed with `adb install -r`, or the app runs stale
  assemblies.

⚠️ The demo's `MainPage` `UrlLoading` handler is required, not decoration: without it MAUI on iOS opens every iframe in
Safari. Consumers need the same handler; the README says so.
