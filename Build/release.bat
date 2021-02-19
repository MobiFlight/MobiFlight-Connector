@ECHO OFF
SET TARGET_DIR=%1
SET SOLUTION_DIR=%2
SET RELEASE_DIR=%SOLUTION_DIR%Release
SET ICONFILE=%SOLUTION_DIR%mobiflight.ico
SET BUILD_TOOLS_DIR=%~dp0
SET CMD_RAR=utils\7z\7z.exe
SET SFXMODULE=utils\7z\7zsd_All_x64.sfx
SET VERSION=
for /f "delims=" %%A in ('%BUILD_TOOLS_DIR%VersionInfo.exe %TARGET_DIR%MFConnector.exe') do set "VERSION=%%A"

echo -----------------------------------------------------------
echo Running command to create release package version %VERSION%
echo -----------------------------------------------------------
echo Removing existing file %RELEASE_DIR%\MobiFlightConnector-%VERSION%.exe...
echo del %RELEASE_DIR%\MobiFlightConnector-%VERSION%.exe /Q
del %RELEASE_DIR%\MobiFlightConnector-%VERSION%.exe /Q
echo OK

echo -----------------------------------------------------------
echo Build file informations and description and icon
echo -----------------------------------------------------------
del %BUILD_TOOLS_DIR%temp.sfx /q
del %BUILD_TOOLS_DIR%temp.exe /q
copy %BUILD_TOOLS_DIR%%SFXMODULE% %BUILD_TOOLS_DIR%temp.exe /Y
echo %BUILD_TOOLS_DIR%utils\rcedit\rcedit.exe %BUILD_TOOLS_DIR%temp.exe --set-icon %ICONFILE% --set-product-version "%VERSION%" --set-file-version "%VERSION%" --set-version-string "LegalCopyright" "" --set-version-string "ProductName" "MobiFlight Connector" --set-version-string "CompanyName" "MobiFlight" --set-version-string "FileDescription" "MobiFlight Connector Installer" --set-version-string "OriginalFilename" "" --set-version-string "PrivateBuild" "" --set-version-string "InternalName" ""
%BUILD_TOOLS_DIR%utils\rcedit\rcedit.exe %BUILD_TOOLS_DIR%temp.exe --set-icon %ICONFILE% --set-product-version "%VERSION%" --set-file-version "%VERSION%" --set-version-string "LegalCopyright" "" --set-version-string "ProductName" "MobiFlight Connector" --set-version-string "CompanyName" "MobiFlight" --set-version-string "FileDescription" "MobiFlight Connector Installer" --set-version-string "OriginalFilename" "" --set-version-string "PrivateBuild" "" --set-version-string "InternalName" ""
move %BUILD_TOOLS_DIR%temp.exe %BUILD_TOOLS_DIR%temp.sfx
echo OK

echo -----------------------------------------------------------
echo Update version number to config file for SFX packaging
echo -----------------------------------------------------------
echo ;!@Install@!UTF-8! > %BUILD_TOOLS_DIR%config-updater.txt
echo Title="MobiFlight" >> %BUILD_TOOLS_DIR%config-updater.txt
echo Path="." >> %BUILD_TOOLS_DIR%config-updater.txt
echo InstallPath="." >> %BUILD_TOOLS_DIR%config-updater.txt
echo Skip="no" >> %BUILD_TOOLS_DIR%config-updater.txt
echo SelfDelete="1" >> %BUILD_TOOLS_DIR%config-updater.txt
echo GUIMode="2" >> %BUILD_TOOLS_DIR%config-updater.txt
echo RunProgram="MobiFlight-Updater.exe %VERSION%" >> %BUILD_TOOLS_DIR%config-updater.txt
echo ;!@InstallEnd@! >> %BUILD_TOOLS_DIR%config-updater.txt
echo OK

echo -----------------------------------------------------------
echo Create setup config file
echo -----------------------------------------------------------
echo ;!@Install@!UTF-8! > %BUILD_TOOLS_DIR%config-setup.txt
echo Title="MobiFlight" >> %BUILD_TOOLS_DIR%config-setup.txt
echo Path="." >> %BUILD_TOOLS_DIR%config-setup.txt
echo InstallPath="." >> %BUILD_TOOLS_DIR%config-setup.txt
echo Skip="no" >> %BUILD_TOOLS_DIR%config-setup.txt
echo SelfDelete="0" >> %BUILD_TOOLS_DIR%config-setup.txt
echo GUIMode="2" >> %BUILD_TOOLS_DIR%config-setup.txt
echo RunProgram="MFconnector.exe" >> %BUILD_TOOLS_DIR%config-setup.txt
echo ;!@InstallEnd@! >> %BUILD_TOOLS_DIR%config-setup.txt

echo -----------------------------------------------------------
echo Building setup package
echo -----------------------------------------------------------
echo Building 7z archive
echo %BUILD_TOOLS_DIR%%CMD_RAR% a %RELEASE_DIR%\MobiFlightSetup-%VERSION%.7z %TARGET_DIR%*.* -r
%BUILD_TOOLS_DIR%%CMD_RAR% a %RELEASE_DIR%\MobiFlightSetup-%VERSION%.7z %TARGET_DIR%*.* -r
echo Building SFX
echo copy /b %BUILD_TOOLS_DIR%temp.sfx + %BUILD_TOOLS_DIR%config-setup.txt + %RELEASE_DIR%\MobiFlightSetup-%VERSION%.7z %RELEASE_DIR%\MobiFlightSetup-%VERSION%.exe
copy /b %BUILD_TOOLS_DIR%temp.sfx + %BUILD_TOOLS_DIR%config-setup.txt + %RELEASE_DIR%\MobiFlightSetup-%VERSION%.7z %RELEASE_DIR%\MobiFlightSetup-%VERSION%.exe

echo OK

echo -----------------------------------------------------------
echo Building updater package
echo -----------------------------------------------------------
echo Building 7z archive
echo %BUILD_TOOLS_DIR%%CMD_RAR% a %RELEASE_DIR%\MobiFlightConnector-%VERSION%.7z %BUILD_TOOLS_DIR%\MobiFlight-Updater.exe %RELEASE_DIR%\MobiFlightSetup-%VERSION%.exe
%BUILD_TOOLS_DIR%%CMD_RAR% a %RELEASE_DIR%\MobiFlightConnector-%VERSION%.7z %BUILD_TOOLS_DIR%\MobiFlight-Updater.exe %RELEASE_DIR%\MobiFlightSetup-%VERSION%.exe
echo Building SFX
echo copy /b %BUILD_TOOLS_DIR%\temp.sfx + %BUILD_TOOLS_DIR%config-updater.txt + %RELEASE_DIR%\MobiFlightConnector-%VERSION%.7z %RELEASE_DIR%\MobiFlightConnector-%VERSION%.exe
copy /b %BUILD_TOOLS_DIR%\temp.sfx + %BUILD_TOOLS_DIR%config-updater.txt + %RELEASE_DIR%\MobiFlightConnector-%VERSION%.7z %RELEASE_DIR%\MobiFlightConnector-%VERSION%.exe
echo OK

echo -----------------------------------------------------------
echo CLEAN build folder
echo -----------------------------------------------------------
del %BUILD_TOOLS_DIR%\config-setup.txt /Q
del %RELEASE_DIR%\MobiFlightSetup-%VERSION%.7z /Q
del %BUILD_TOOLS_DIR%\config-updater.txt /Q
del %RELEASE_DIR%\MobiFlightConnector-%VERSION%.7z /Q
del %BUILD_TOOLS_DIR%VersionInfo.exe /q
del %BUILD_TOOLS_DIR%MobiFlight-Updater.exe /q
del %RELEASE_DIR%\MobiFlightSetup-%VERSION%.exe /q
del %BUILD_TOOLS_DIR%temp.sfx /q

echo -----------------------------------------------------------
echo DONE
echo -----------------------------------------------------------
