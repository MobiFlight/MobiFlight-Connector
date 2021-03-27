@ECHO OFF
SET TARGET_DIR=%1
SET SOLUTION_DIR=%2
SET RELEASE_DIR=%SOLUTION_DIR%Release
SET BUILD_TOOLS_DIR=%~dp0
SET CMD_RAR=utils\7z\7z.exe
SET VERSION=
for /f "delims=" %%A in ('%BUILD_TOOLS_DIR%VersionInfo.exe %TARGET_DIR%MFConnector.exe') do set "VERSION=%%A"

echo -----------------------------------------------------------
echo CLEAN Release Directory
echo -----------------------------------------------------------
del %RELEASE_DIR%\MobiFlightConnector-%VERSION%.* /Q
del %RELEASE_DIR%\MobiFlight-Updater.exe /Q
echo OK

echo -----------------------------------------------------------
echo Building MobiFlight-Connector ZIP package
echo -----------------------------------------------------------
%BUILD_TOOLS_DIR%%CMD_RAR% a %RELEASE_DIR%\MobiFlightConnector-%VERSION%.zip %TARGET_DIR%*.* -r %SOLUTION_DIR%MobiFlightUpdater\MobiFlight-Updater\bin\Release\MobiFlight-Installer.exe
copy %SOLUTION_DIR%MobiFlightUpdater\MobiFlight-Updater\bin\Release\MobiFlight-Installer.exe %RELEASE_DIR%\
echo OK

echo -----------------------------------------------------------
echo CLEAN build folder
echo -----------------------------------------------------------

echo -----------------------------------------------------------
echo DONE
echo -----------------------------------------------------------