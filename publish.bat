@echo off
cls

rem Release Debug
set config=Release

set msbuild="D:\Applications\VS2022\MSBuild\Current\Bin\amd64\MSBuild.exe"
if not exist %msbuild% call :findMsbuild
if not exist %msbuild% (
	echo Failed to locate MSBuild.exe. Update publish.bat.
	pause
	exit /b 1
)

set solutionDir=%CD%\source
set publish=%CD%\bin\publish

set netVer=net8.0
set netVerFull=net8.0-windows10.0.18362.0

if exist "%publish%" (
	rmdir /q /s "%publish%"
	if errorlevel 1 (pause & exit /b 1)
)
if exist bin\launcher (
	rmdir /q /s bin\launcher
	if errorlevel 1 (pause & exit /b 1)
)


set platform=x64
call :publish
if errorlevel 1 (pause & exit /b 1)

rem Since BCU is now on .NET8, realistically only Arm64 and x64 Windows systems are supported now, so there's no point in building x86
rem set platform=x86
rem call :publish

copy bin\launcher\BCU-launcher.exe "%publish%\BCUninstaller.exe"
if errorlevel 1 (echo Failed to copy BCU-launcher.exe & pause & exit /b 1)
copy "%target%\BCU_manual.html" "%publish%\BCU_manual.html"
copy "%target%\Licence.txt" "%publish%\Licence.txt"
copy "%target%\PrivacyPolicy.txt" "%publish%\PrivacyPolicy.txt"
copy "%target%\NOTICE" "%publish%\NOTICE"

if exist bin\launcher rmdir /q /s bin\launcher

IF %config%==Release (del /f /s /q "%publish%\*.pdb")


rem --- AnyCPU --------------------------------------------------

set target=%CD%\bin\publish-AnyCPU-%netVer%

if exist "%target%" (
	rmdir /q /s "%target%"
	if errorlevel 1 (pause & exit /b 1)
)

set platform=Any CPU
set selfContained=False
set runtime=

echo ====== Building AnyCPU ======

call :publishProjects
if errorlevel 1 (pause & exit /b 1)

IF %config%==Release (del /f /s /q "%target%\*.pdb")

echo ====== Publishing finished! ======
pause
exit /b 0



rem -------------------------------------------------------------

:publish
set identifier=win-%platform%
set target=%CD%\bin\publish\%identifier%
set selfContained=True
set runtime=/p:RuntimeIdentifier=%identifier%

echo ====== Building %identifier% ======

call :buildLauncher
if errorlevel 1 goto :eof

call :publishProjects
goto :eof

rem -------------------------------------------------------------

:publishProjects
for /r "%solutionDir%" %%F in (*.csproj) do (
	call :publishProjectIfEligible "%%~fF"
	if errorlevel 1 goto :eof
)

exit /b 0

:publishProjectIfEligible
findstr /i /c:"exe</OutputType>" "%~1" >nul
if errorlevel 1 exit /b 0

call :publishProject "%~1"
if errorlevel 1 exit /b 1

exit /b 0

:publishProject
%msbuild% "%~1" /restore /m /p:filealignment=512 /t:Publish /p:DeployOnBuild=true /p:PublishSingleFile=False /p:SelfContained=%selfContained% /p:PublishProtocol=FileSystem /p:Configuration=%config% /p:Platform="%platform%" /p:TargetFramework=%netVerFull% /p:PublishDir="%target%" %runtime% /p:PublishReadyToRun=false /p:PublishTrimmed=False /verbosity:minimal

goto :eof

rem -------------------------------------------------------------

:buildLauncher
rem Build via solution so platform mapping is respected automatically.
%msbuild% "source\BulkCrapUninstaller.sln" /t:BCU-launcher /m /p:Configuration=%config% /p:Platform=%platform% "/p:SolutionDir=%solutionDir%\\" /verbosity:minimal

goto :eof

rem -------------------------------------------------------------

:findMsbuild
set vswhere=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe

if exist "%vswhere%" (
	for /f "delims=" %%I in ('"%vswhere%" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\amd64\MSBuild.exe') do (
		set msbuild="%%~fI"
		goto :eof
	)
)

for /f "delims=" %%I in ('where msbuild 2^>nul') do (
	set msbuild="%%~fI"
	goto :eof
)

goto :eof

rem -------------------------------------------------------------
