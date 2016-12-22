@ECHO OFF
SET RELEASE_DIR=%1
SET VERSION=%2
SET BUILD_TOOLS_DIR=%~dp0
SET ICONFILE=..\mobiflight.ico

echo %BUILD_TOOLS_DIR%rar.exe a -o+ -sfx -ep1 -r ..\Release\MobiFlightConnector-%VERSION% -zsfx-mobiflight.diz -iiconc:%ICONFILE% %RELEASE_DIR%\*.*