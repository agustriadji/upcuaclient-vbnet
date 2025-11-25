[Setup]
AppName=AirLM
AppVersion=1.1.1
DefaultDirName={autopf}\AirLM
DefaultGroupName=AirLM
OutputDir=Output
OutputBaseFilename=AirLM-Setup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64compatible

[Files]
Source: "upcuaclient-vbnet\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "upcuaclient-vbnet\data\*"; DestDir: "{app}\data"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "upcuaclient-vbnet\Config\*"; DestDir: "{app}\Config"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\AirLM"; Filename: "{app}\upcuaclient-vbnet.exe"
Name: "{autodesktop}\AirLM"; Filename: "{app}\upcuaclient-vbnet.exe"

[Run]
Filename: "{app}\upcuaclient-vbnet.exe"; Description: "AirLM"; Flags: nowait postinstall skipifsilent

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; \
    ValueType: string; ValueName: "AirLM"; \
    ValueData: """{app}\upcuaclient-vbnet.exe"""; Flags: uninsdeletevalue

[Prerequisites]
Name: "dotnetfx472"; Title: ".NET Framework 4.7.2"; Url: "https://download.microsoft.com/download/6/E/4/6E48E8AB-DC00-419E-9704-06DD46E5F81D/NDP472-KB4054530-x86-x64-AllOS-ENU.exe"; Check: not IsDotNet472Installed

[Code]
function IsDotNet472Installed: Boolean;
var
  Release: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) and (Release >= 461808);
end;