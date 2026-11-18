# SysConnector Server Hosting Guide

## Current status

`RemoteDesktop.Server` is an ASP.NET Core service. At present it only exposes a health check at `/health`; it does not yet register User IDs, verify PINs, send approval requests, or relay a remote desktop session.

You can host it now to confirm that the server deployment works. A working remote desktop connection needs the signaling, authentication, host consent, streaming, and relay features to be implemented first.

## 1. Local network testing

Use this when the server and test computers are on the same Wi-Fi or LAN.

1. Open PowerShell on the server computer.
2. Run the following commands from the project folder:

```powershell
cd D:\_Codex\AnyDesk
dotnet run --project .\RemoteDesktop.Server\RemoteDesktop.Server.csproj --urls "http://0.0.0.0:5080"
```

3. Find the server computer's LAN IPv4 address:

```powershell
ipconfig
```

4. From another computer on the same network, open:

```text
http://SERVER-LAN-IP:5080/health
```

Expected response:

```json
{"status":"ok","service":"RemoteDesktop.Server","version":"0.1.0"}
```

5. If the browser cannot connect, allow TCP port `5080` through Windows Firewall:

```powershell
New-NetFirewallRule -DisplayName "SysConnector Server (TCP 5080)" -Direction Inbound -Protocol TCP -LocalPort 5080 -Action Allow
```

Do not expose this HTTP test service directly to the internet.

## 2. Publish the server

Publish a self-contained Windows x64 deployment package:

```powershell
cd D:\_Codex\AnyDesk
dotnet publish .\RemoteDesktop.Server\RemoteDesktop.Server.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o .\artifacts\server
```

Copy the contents of `artifacts\server` to a folder on the Windows server, for example:

```text
C:\SysConnector\Server
```

Run it for a local check:

```powershell
cd C:\SysConnector\Server
$env:ASPNETCORE_URLS = "http://127.0.0.1:5080"
.\RemoteDesktop.Server.exe
```

Then browse to `http://127.0.0.1:5080/health` on that server.

## 3. Public deployment with HTTPS

For internet use, use a Windows Server or VPS with:

- A static public IP address.
- A domain such as `connect.example.com`.
- A DNS `A` record that points `connect.example.com` to the server's public IP.
- TCP ports 80 and 443 open in the cloud firewall and Windows Firewall.
- A TLS certificate, such as a Let's Encrypt certificate.

Run SysConnector behind IIS or another HTTPS reverse proxy. The reverse proxy terminates TLS on port 443 and forwards requests to SysConnector only on `127.0.0.1:5080`.

Recommended IIS deployment steps:

1. Install IIS and the ASP.NET Core Hosting Bundle for the matching .NET runtime.
2. Create an IIS website bound to `https://connect.example.com` on port 443.
3. Attach a valid TLS certificate to that binding.
4. Configure the site to proxy requests to `http://127.0.0.1:5080`.
5. Run `RemoteDesktop.Server.exe` as a Windows service so it starts after a restart.
6. Verify `https://connect.example.com/health` from an external network.

Never use a self-signed certificate for the production client connection. Clients must validate the server certificate and hostname.

## 4. Run as a Windows service

Use a service manager such as NSSM, or add Windows Service hosting support to the server project. With NSSM:

```powershell
nssm install SysConnectorServer "C:\SysConnector\Server\RemoteDesktop.Server.exe"
nssm set SysConnectorServer AppEnvironmentExtra "ASPNETCORE_URLS=http://127.0.0.1:5080"
nssm start SysConnectorServer
```

Check its status:

```powershell
Get-Service SysConnectorServer
```

## 5. What must be added before real remote access

The server must implement these endpoints and secure session flows before deploying SysConnector for real users:

1. User ID registration and presence tracking.
2. TLS-only SignalR or WebSocket signaling.
3. PIN verification using a salted password hash; do not store PINs as plain text.
4. Short-lived signed session tokens.
5. An incoming-session request that the Host must explicitly accept or reject.
6. An authenticated TLS relay fallback for screen, input, chat, and files.
7. Rate limiting, audit logs, connection expiry, and abuse monitoring.

Until those features exist, the User ID and PIN shown by the client are local display values only. They do not authenticate a remote session.

## 6. Deployment checklist

- Confirm `/health` returns status `ok` over HTTPS.
- Restrict the app's direct listener to `127.0.0.1` when using a reverse proxy.
- Keep the operating system, .NET runtime, and TLS certificate current.
- Use a firewall that exposes only ports 80 and 443 publicly.
- Store configuration and secrets outside the published application folder.
- Test Host approval, disconnect, authentication failure, and reconnect flows before inviting users.
