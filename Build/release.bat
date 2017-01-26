@ECHO OFF
SET RELEASE_DIR=%1
SET SOLUTION_DIR=%2
SET ICONFILE=%SOLUTION_DIR%\mobiflight.ico
SET BUILD_TOOLS_DIR=%~dp0
SET VERSION=
for /f "delims=" %%A in ('%BUILD_TOOLS_DIR%VersionInfo.exe %RELEASE_DIR%MFConnector.exe') do set "VERSION=%%A"

echo -----------------------------------------------------------
echo Running command to create release package version %VERSION%
echo -----------------------------------------------------------
del %RELEASE_DIR%\MobiFlightConnector-%VERSION%.exe /Q
echo %BUILD_TOOLS_DIR%rar.exe a -o+ -sfx -ep1 -r %SOLUTION_DIR%Release\MobiFlightConnector-%VERSION% -z%BUILD_TOOLS_DIR%sfx-mobiflight.diz -iiconc:%ICONFILE% %RELEASE_DIR%*.*
%BUILD_TOOLS_DIR%rar.exe a -o+ -sfx -ep1 -r %SOLUTION_DIR%Release\MobiFlightConnector-%VERSION% -z%BUILD_TOOLS_DIR%sfx-mobiflight.diz -iiconc:%ICONFILE% %RELEASE_DIR%*.*
echo -----------------------------------------------------------
echo DONE
echo -----------------------------------------------------------
