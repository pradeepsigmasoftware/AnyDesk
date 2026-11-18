#define AppName "SysConnector"
#define AppVersion "0.1.0"
#define PublishDir "..\artifacts\client"
[Setup]
AppId={{B184C7DD-9BEE-49A1-981F-30A8086F32F9}
AppName={#AppName}
AppVersion={#AppVersion}
DefaultDirName={autopf}\SysConnector
DefaultGroupName={#AppName}
OutputDir=..\artifacts
OutputBaseFilename=SysConnector-Setup-x64
ArchitecturesInstallIn64BitMode=x64
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=lowest
[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "*.pdb"
[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\RemoteDesktop.Client.exe"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\RemoteDesktop.Client.exe"; Tasks: desktopicon
[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"
[Run]
Filename: "{app}\RemoteDesktop.Client.exe"; Description: "Launch {#AppName}"; Flags: postinstall nowait skipifsilent
