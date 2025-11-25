[Setup]
AppName=UPC UA Client
AppVersion=1.0
DefaultDirName={autopf}\UPC UA Client
DefaultGroupName=UPC UA Client
OutputDir=Output
OutputBaseFilename=UPCUAClient-Setup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64compatible

[Files]
Source: "upcuaclient-vbnet\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "upcuaclient-vbnet\data\*"; DestDir: "{app}\data"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "upcuaclient-vbnet\Config\*"; DestDir: "{app}\Config"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\UPC UA Client"; Filename: "{app}\upcuaclient-vbnet.exe"
Name: "{autodesktop}\UPC UA Client"; Filename: "{app}\upcuaclient-vbnet.exe"

[Run]
Filename: "{app}\upcuaclient-vbnet.exe"; Description: "Launch UPC UA Client"; Flags: nowait postinstall skipifsilent

[Prerequisites]
Name: "dotnetfx472"; Title: ".NET Framework 4.7.2"; Url: "https://download.microsoft.com/download/6/E/4/6E48E8AB-DC00-419E-9704-06DD46E5F81D/NDP472-KB4054530-x86-x64-AllOS-ENU.exe"; Check: not IsDotNet472Installed

[Code]
function IsDotNet472Installed: Boolean;
var
  Release: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) and (Release >= 461808);
end;