@ECHO OFF
SET TARGET_DIR=%1
SET SOLUTION_DIR=%2
SET RELEASE_DIR=%SOLUTION_DIR%Release
SET ICONFILE=%SOLUTION_DIR%mobiflight.ico
SET BUILD_TOOLS_DIR=%~dp0
SET CMD_RAR=winrar.exe
SET VERSION=
for /f "delims=" %%A in ('%BUILD_TOOLS_DIR%VersionInfo.exe %TARGET_DIR%MFConnector.exe') do set "VERSION=%%A"

echo -----------------------------------------------------------
echo Running command to create release package version %VERSION%
echo -----------------------------------------------------------
echo Removing existing file %RELEASE_DIR%\MobiFlightConnector-%VERSION%.exe...
echo del %RELEASE_DIR%\MobiFlightConnector-%VERSION%.exe /Q
del %RELEASE_DIR%\MobiFlightConnector-%VERSION%.exe /Q
echo OK
echo %BUILD_TOOLS_DIR%%CMD_RAR% a -o+ -sfxDefault64.sfx -ep1 -r %RELEASE_DIR%\MobiFlightSetup-%VERSION% -z%BUILD_TOOLS_DIR%sfx-mobiflight.diz -iicon%ICONFILE% %TARGET_DIR%*.*
%BUILD_TOOLS_DIR%%CMD_RAR% a -o+ -sfxDefault64.sfx -ep1 -r %RELEASE_DIR%\MobiFlightSetup-%VERSION% -z%BUILD_TOOLS_DIR%sfx-mobiflight.diz -iicon%ICONFILE% %TARGET_DIR%*.*

echo -----------------------------------------------------------
echo Writing DIZ File for updater
echo -----------------------------------------------------------
echo ;The comment below contains SFX script commands > %BUILD_TOOLS_DIR%sfx-mf-updater.diz
echo Setup=MobiFlight-Updater.exe %VERSION% >> %BUILD_TOOLS_DIR%sfx-mf-updater.diz
echo Silent=1 >> %BUILD_TOOLS_DIR%sfx-mf-updater.diz
echo Overwrite=1 >> %BUILD_TOOLS_DIR%sfx-mf-updater.diz

echo -----------------------------------------------------------
echo %BUILD_TOOLS_DIR%%CMD_RAR% a -o+ -m0 -sfxDefault64.sfx -ep1 -r %RELEASE_DIR%\MobiFlightConnector-%VERSION% -z%BUILD_TOOLS_DIR%sfx-mf-updater.diz -iicon%ICONFILE% %BUILD_TOOLS_DIR%\MobiFlight-Updater.exe %RELEASE_DIR%\MobiFlightSetup-%VERSION%.exe
%BUILD_TOOLS_DIR%%CMD_RAR% a -o+ -sfxDefault64.sfx -m0 -ep1 -r %RELEASE_DIR%\MobiFlightConnector-%VERSION% -z%BUILD_TOOLS_DIR%sfx-mf-updater.diz -iicon%ICONFILE% %BUILD_TOOLS_DIR%\MobiFlight-Updater.exe %RELEASE_DIR%\MobiFlightSetup-%VERSION%.exe

echo -----------------------------------------------------------
echo DONE
echo -----------------------------------------------------------
