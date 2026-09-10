; EBUninstaller Pro - Modern Inno Setup Installer Script
; Professional Next-Generation Windows Uninstaller

#define MyAppName "EBUninstaller Pro"
#define MyAppVersion "7.0.0"
#define MyAppPublisher "EhabYT"
#define MyAppURL "https://github.com/EhabYT/EBUninstaller"
#define MyAppExeName "EBUninstaller.exe"
#define MyAppConsoleExeName "EBU-console.exe"

#include "CodeDependencies.iss"

[Setup]
AppId={{D3F9E17A-72A8-4A1B-8DF9-E342B286E90F}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppCopyright=Copyright (C) 2026 EhabYT. All rights reserved.
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}
WizardImageFile=assets\bigImage.bmp
WizardSmallImageFile=assets\smallImage.bmp
SetupIconFile=assets\logo.ico
AllowNoIcons=yes
LicenseFile=..\Licence.txt
OutputDir=..\build\installer
OutputBaseFilename=EBUninstallSetup
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible arm64
ArchitecturesInstallIn64BitMode=x64compatible arm64
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=commandline dialog
UsedUserAreasWarning=no

VersionInfoCompany={#MyAppPublisher}
VersionInfoCopyright=Copyright (C) 2026 EhabYT. All rights reserved.
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}
VersionInfoTextVersion={#MyAppVersion}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "arabic"; MessagesFile: "lang\Arabic.isl"
Name: "chinesesimplified"; MessagesFile: "lang\ChineseSimplified.isl"
Name: "ukrainian"; MessagesFile: "lang\Ukrainian.isl"
Name: "vietnamese"; MessagesFile: "lang\Vietnamese.isl"
Name: "hindi"; MessagesFile: "lang\Hindi.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode

[Files]
Source: "..\bin\Release\AnyCPU\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{#MyAppName} (Command Line)"; Filename: "{app}\{#MyAppConsoleExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}\Logs"
Type: filesandordirs; Name: "{app}\Backups"
Type: filesandordirs; Name: "{app}\InfoCache.xml"
Type: filesandordirs; Name: "{app}\CertCache.xml"
Type: files; Name: "{app}\*.log"

[Code]
function InitializeSetup: Boolean;
begin
  Dependency_AddDotNet80Desktop;
  Result := True;
end;
