@ECHO OFF
SET TARGET_DIR=%1
SET SOLUTION_DIR=%2
SET RELEASE_DIR=%SOLUTION_DIR%Release
SET ICONFILE=%SOLUTION_DIR%\mobiflight.ico
SET BUILD_TOOLS_DIR=%~dp0
SET VERSION=
for /f "delims=" %%A in ('%BUILD_TOOLS_DIR%VersionInfo.exe %TARGET_DIR%MFConnector.exe') do set "VERSION=%%A"

echo -----------------------------------------------------------
echo Running command to create release package version %VERSION%
echo -----------------------------------------------------------
echo Removing existing file %RELEASE_DIR%\MobiFlightConnector-%VERSION%.exe...
echo del %RELEASE_DIR%\MobiFlightConnector-%VERSION%.exe /Q
del %RELEASE_DIR%\MobiFlightConnector-%VERSION%.exe /Q
echo OK
echo %BUILD_TOOLS_DIR%rar.exe a -o+ -sfx -ep1 -r %RELEASE_DIR%\MobiFlightConnector-%VERSION% -z%BUILD_TOOLS_DIR%sfx-mobiflight.diz -iiconc:%ICONFILE% %TARGET_DIR%*.*
%BUILD_TOOLS_DIR%rar.exe a -o+ -sfx -ep1 -r %RELEASE_DIR%\MobiFlightConnector-%VERSION% -z%BUILD_TOOLS_DIR%sfx-mobiflight.diz -iiconc:%ICONFILE% %TARGET_DIR%*.*
echo -----------------------------------------------------------
echo DONE
echo -----------------------------------------------------------
