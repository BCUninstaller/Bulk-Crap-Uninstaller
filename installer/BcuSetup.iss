; Tested with innosetup-6.4.3

; Normal: include self-contained binaries for both x86 and x64
; Light: include only AnyCPU binaries and automatically download net8 if needed
#define Light

; =============================================================================

#define MyAppName          "BCUninstaller"
#define MyAppNameShort     "BCUninstaller"
#define MyAppPublisher     "Marcin Szeniak"
#define MyAppURL           "https://github.com/Klocman/Bulk-Crap-Uninstaller"
#define MyAppExeName       "BCUninstaller.exe"
#define CurrentYear        GetDateTimeString('yyyy','','')
#define MyAppCopyright     "Copyright " + CurrentYear + " " + MyAppPublisher

#ifdef Light
#define InputDir           "..\bin\publish-AnyCPU-net8.0"
#define MainExePath        InputDir+'\'+MyAppExeName
; Downloading net8 is only necessary in light mode
#include "CodeDependencies.iss"
#else
#define InputDir           "..\bin\publish"
#define MainExePath        InputDir+'\win-x64\'+MyAppExeName
; Portable page only works in normal mode
#include "PortablePage.iss"
#endif

#define                    MajorVersion    
#define                    MinorVersion    
#define                    RevisionVersion    
#define                    BuildVersion    
#define TempVersion        ParseVersion(MainExePath, MajorVersion, MinorVersion, RevisionVersion, BuildVersion)
#define MyAppVersion       str(MajorVersion) + "." + str(MinorVersion) + "." + str(RevisionVersion) + "." + str(BuildVersion)
#define MyAppVersionShort  str(MajorVersion) + "." + str(MinorVersion) + "." + str(RevisionVersion)


[Setup]
AppId={{f4fef76c-1aa9-441c-af7e-d27f58d898d1}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}

AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}

AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}

DefaultDirName={commonpf}\{#MyAppName}
DefaultGroupName={#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}

WizardImageFile=assets\bigImage.bmp
WizardSmallImageFile=assets\smallImage.bmp
SetupIconFile=assets\logo.ico

AllowNoIcons=yes
DisableDirPage=no
LicenseFile={#InputDir}\Licence.txt
OutputBaseFilename={#MyAppNameShort}_{#MyAppVersionShort}_setup

Compression=lzma2/ultra
SolidCompression=yes
LZMAUseSeparateProcess=yes
LZMADictionarySize=548570
LZMANumFastBytes=273
LZMANumBlockThreads=8

PrivilegesRequired=admin
ArchitecturesAllowed=x86compatible
ArchitecturesInstallIn64BitMode=x64compatible

VersionInfoCompany={#MyAppPublisher}
;VersionInfoDescription=desc
VersionInfoCopyright={#MyAppCopyright}
VersionInfoProductName={#MyAppName}
VersionInfoProductTextVersion={#MyAppVersion}
VersionInfoTextVersion={#MyAppVersion}

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "fr"; MessagesFile: "compiler:Languages\French.isl"
Name: "pl"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"
Name: "sl"; MessagesFile: "compiler:Languages\Slovenian.isl"
Name: "nl"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "es"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "bpt"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "ru"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "it"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "hu"; MessagesFile: "compiler:Languages\Hungarian.isl"
Name: "tr"; MessagesFile: "compiler:Languages\Turkish.isl"
Name: "vi"; MessagesFile: "lang\Vietnamese.isl"
Name: "hi"; MessagesFile: "lang\Hindi.isl"
Name: "zh_cn"; MessagesFile: "lang\ChineseSimplified.isl"

[Components]
Name: "main"; Description: "{cm:MainFiles}"; Types: full compact custom; Flags: fixed
Name: "lang"; Description: "{cm:ExtraLanguages}"; Types: full

[Files]
#ifdef Light

; Need to do this to separate the language resource folders from main app file
Source: "{#InputDir}\*";                DestDir: "{app}\";          Components: main; Flags: ignoreversion recursesubdirs; Excludes: "CleanLogs.bat,\??\*,\??-??\*,\??-????\*";
; If installing languages, copy everything
Source: "{#InputDir}\*";                DestDir: "{app}\";          Components: lang; Flags: ignoreversion recursesubdirs; Excludes: "CleanLogs.bat";

#else 

Source: "{#InputDir}\*";                        DestDir: "{app}"; Components: main; Flags: ignoreversion; Check: IsPortable or not IsPortable
Source: "{#InputDir}\BCU_manual.html";          DestDir: "{app}"; Components: main; Flags: ignoreversion isreadme; Check: IsPortable or not IsPortable

; Need to do this to separate the language resource folders from main app files
Source: "{#InputDir}\win-x64\*";                DestDir: "{app}\win-x64";           Components: main; Flags: ignoreversion; Excludes: "CleanLogs.bat"; Check: Is64BitInstallMode or IsPortable
Source: "{#InputDir}\win-x64\Resources\*";      DestDir: "{app}\win-x64\Resources"; Components: main; Flags: ignoreversion recursesubdirs;             Check: Is64BitInstallMode or IsPortable
Source: "{#InputDir}\win-x86\*";                DestDir: "{app}\win-x86";           Components: main; Flags: ignoreversion; Excludes: "CleanLogs.bat"; Check: not Is64BitInstallMode or IsPortable
Source: "{#InputDir}\win-x86\Resources\*";      DestDir: "{app}\win-x86\Resources"; Components: main; Flags: ignoreversion recursesubdirs;             Check: not Is64BitInstallMode or IsPortable

; If installing languages, copy everything
Source: "{#InputDir}\win-x64\*";                DestDir: "{app}\win-x64"; Components: lang; Flags: ignoreversion recursesubdirs; Excludes: "CleanLogs.bat"; Check: Is64BitInstallMode or IsPortable
Source: "{#InputDir}\win-x86\*";                DestDir: "{app}\win-x86"; Components: lang; Flags: ignoreversion recursesubdirs; Excludes: "CleanLogs.bat"; Check: not Is64BitInstallMode or IsPortable

; Only copy the cleaning script if installing as portable
Source: "{#InputDir}\win-x64\CleanLogs.bat";    DestDir: "{app}\win-x64"; Components: main; Flags: ignoreversion; Check: IsPortable
Source: "{#InputDir}\win-x86\CleanLogs.bat";    DestDir: "{app}\win-x86"; Components: main; Flags: ignoreversion; Check: IsPortable

#endif

[InstallDelete]
Name: {app}\BCU-launcher.exe; Type: files
; Make sure there are no old stale versions. Settings file is kept in the root directory so it will survive.
Name: {app}\win-x64; Type: filesandordirs
Name: {app}\win-x86; Type: filesandordirs

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; Check: IsNotPortable

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Check: IsNotPortable;
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"; Check: IsNotPortable;
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon; Check: IsNotPortable;

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent shellexec

#ifdef Light
[Code]
function IsPortable(): Boolean;
begin
  Result := False
end;
function IsNotPortable(): Boolean;
begin
  Result := True
end;

function InitializeSetup: Boolean;
begin
  Dependency_AddDotNet80Desktop;
  Result := True;
end;
#endif

[CustomMessages] 
en.MainFiles=Main Files
pl.MainFiles=Główne pliki
fr.MainFiles=Principaux fichiers
de.MainFiles=Haupt Dateien 
hu.MainFiles=Fő fájlok
sl.MainFiles=Glavne datoteke
nl.MainFiles=Hoofdbestanden
es.MainFiles=Archivos principales
bpt.MainFiles=Arquivos principais
ru.MainFiles=Основные файлы программы
it.MainFiles=File programma
vi.MainFiles=Các tập tin chương trình chính
tr.MainFiles=Ana Dosyalar
zh_cn.MainFiles=主文件
hi.MainFiles=मुख्य फ़ाइलें

en.ExtraLanguages=Extra Languages
pl.ExtraLanguages=Dodatkowe języki
fr.ExtraLanguages=Langues supplémentaires
de.ExtraLanguages=Zusätzliche Sprachen 
hu.ExtraLanguages=Extra nyelvek
sl.ExtraLanguages=Dodatni jeziki
nl.ExtraLanguages=Extra talen
es.ExtraLanguages=Idiomas adicionales
bpt.ExtraLanguages=Línguas extras
ru.ExtraLanguages=Дополнительные языки
it.ExtraLanguages=Lingue aggiuntive
vi.ExtraLanguages=Ngôn ngữ bổ sung
tr.ExtraLanguages=İlave Diller
zh_cn.ExtraLanguages=其他语言
hi.ExtraLanguages=अतिरिक्त भाषाएँ
