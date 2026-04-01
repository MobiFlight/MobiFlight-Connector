@ECHO OFF
SET TARGET_DIR=%1
SET SOLUTION_DIR=%2
SET RELEASE_DIR=%SOLUTION_DIR%Release
SET BUILD_TOOLS_DIR=%SOLUTION_DIR%Build\
SET CMD_RAR=utils\7z\7z.exe
SET INSTALLER_NAME=MobiFlight-Installer

echo -----------------------------------------------------------
echo CLEAN install log file
echo -----------------------------------------------------------
del %TARGET_DIR%\install.log.txt /Q

echo -----------------------------------------------------------
echo CLEAN Release Directory
echo -----------------------------------------------------------
del %RELEASE_DIR%\MobiFlightConnector-release.* /Q
del %RELEASE_DIR%\%INSTALLER_NAME%.exe /Q
echo OK

echo -----------------------------------------------------------
echo REMOVE hubhop json presets from release
echo -----------------------------------------------------------
del %TARGET_DIR%\Presets\msfs2020_hubhop_presets.json /Q
del %TARGET_DIR%\Presets\xplane_hubhop_presets.json /Q
echo OK

echo -----------------------------------------------------------
echo Building MobiFlight-Connector ZIP package
echo -----------------------------------------------------------
%BUILD_TOOLS_DIR%%CMD_RAR% a %RELEASE_DIR%\MobiFlightConnector-release.zip %TARGET_DIR%*.* -r
copy %SOLUTION_DIR%src\%INSTALLER_NAME%\bin\Release\%INSTALLER_NAME%.exe %RELEASE_DIR%\
echo OK

echo -----------------------------------------------------------
echo CLEAN build folder
echo -----------------------------------------------------------

echo -----------------------------------------------------------
echo DONE
echo -----------------------------------------------------------