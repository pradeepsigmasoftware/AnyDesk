# SysConnector MVP

A .NET 8 Windows solution containing a WPF client shell, shared identity helpers, a server health endpoint, and an explicit host placeholder. This is an early MVP foundation, not a usable remote desktop product.

## Included now

- WPF home screen with a persistent six digit numeric user ID and editable, persistent four digit access PIN. Both values can be copied or generated from the client.
- The connect action validates input and clearly reports that session signaling is not available yet.
- ASP.NET Core server with `/health` endpoint.
- Client, host, and server publish profiles produce self-contained Windows x64 single-file executables.

## Not implemented

Remote host consent dialogs, device registry and signaling, TLS-authenticated session negotiation, peer-to-peer or relay transport, screen capture/streaming, mouse/keyboard control, clipboard, chat, file transfer, history, reconnection, and multiple monitors. The displayed access PIN is not validated by a server or used for authentication. No remote access listener is started. Do not expose this MVP to the internet or use it for sensitive access.

## Build

Requires .NET 8 SDK or later on Windows:

```powershell
dotnet build RemoteDesktopSolution.slnx -c Release
```

## Publish standalone executables

```powershell
dotnet publish RemoteDesktop.Client -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o artifacts/client
dotnet publish RemoteDesktop.Host -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o artifacts/host
dotnet publish RemoteDesktop.Server -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o artifacts/server
```

The server needs a TLS reverse proxy or a configured HTTPS endpoint before any public deployment. The current health-only service has no signaling data or authentication.

## Installer

The usable x64 client installer is `artifacts/SysConnector-Setup-x64.exe`. It installs the complete WPF publish output, creates a Start menu shortcut, and can optionally create a desktop shortcut. It installs per-user, so administrator rights are not required.

To rebuild it, install Inno Setup 6 and run:

```powershell
& "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe" installer\RemoteDesktop.iss
```

## Two-computer evaluation

At this MVP stage, start the client on a Windows computer and verify its six digit user ID remains stable across restarts. Save a four digit PIN of your choice, restart the app, and verify that it is retained. The Connect action intentionally reports that transport is unavailable. A true two-computer remote session cannot be tested until the host, signaling, encryption, consent, and streaming services are implemented.
