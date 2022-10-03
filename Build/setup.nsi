# define name of installer
OutFile "mobiflight-setup.exe"
 
# define installation directory
InstallDir $DESKTOP
 
# For removing Start Menu shortcut in Windows 7
RequestExecutionLevel user
 
# start default section
Section
 
    # set the installation directory as the destination for the following actions
    SetOutPath $INSTDIR
 
    # create the uninstaller
    WriteUninstaller "$INSTDIR\uninstall.exe"
 
    # create a shortcut named "new shortcut" in the start menu programs directory
    # point the new shortcut at the program uninstaller
    CreateShortcut "$SMPROGRAMS\new shortcut.lnk" "$INSTDIR\uninstall.exe"
SectionEnd
 
# uninstaller section start
Section "uninstall"
 
    # Remove the link from the start menu
    Delete "$SMPROGRAMS\new shortcut.lnk"
 
    # Delete the uninstaller
    Delete $INSTDIR\uninstaller.exe
 
    RMDir $INSTDIR
# uninstaller section end
SectionEnd