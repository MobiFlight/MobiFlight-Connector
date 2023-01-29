# define name of installer
Unicode True
!define APPNAME "MobiFlight Connector"
OutFile "MobiFlight-Setup.exe"

!include LogicLib.nsh
!include MUI2.nsh
 
# define installation directory
InstallDir "$LOCALAPPDATA\MobiFlight\${APPNAME}"
 
# For removing Start Menu shortcut in Windows 7
RequestExecutionLevel admin

; The icon in the top right corner of the installer windows and the desktop icon.
!define MUI_ICON "..\mobiflight.ico"
; The icon for the installer in the directory, not in Add or Remove Programs.
!define MUI_UNICON "..\mobiflight.ico"
!define MUI_ABORTWARNING
; !insertmacro MUI_PAGE_DIRECTORY
!define MUI_WELCOMEFINISHPAGE_BITMAP "setup-welcome.bmp"
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_RUN "$INSTDIR\MFConnector.exe"
!define MUI_FINISHPAGE_SHOWREADME
!define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED
!define MUI_FINISHPAGE_SHOWREADME_TEXT "Create Desktop Shortcut"
!define MUI_FINISHPAGE_SHOWREADME_FUNCTION CreateDesktopOnFinish

!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

Name "${APPNAME}"
Icon "..\mobiflight.ico"
UninstallIcon "..\mobiflight.ico"

Function CreateDesktopOnFinish
CreateShortcut "$DESKTOP\${APPNAME}.lnk" "$INSTDIR\MFConnector.exe"
FunctionEnd
 

Function .onInit
    System::Call 'kernel32::OpenMutex(i 0x100000, i 0, t "{57699317-1D72-4B54-82BC-CF6B38254550}")p.R0'
    IntPtrCmp $R0 0 notRunning
        System::Call 'kernel32::CloseHandle(p $R0)'
        MessageBox MB_OK|MB_ICONEXCLAMATION "${APPNAME} is running. Please close it first" /SD IDOK
        Abort
    notRunning:
FunctionEnd

# start default section
Section "install"
    SetRegView 64
    # set the installation directory as the destination for the following actions
    SetOutPath $INSTDIR

    # Add the MobiFlight installer
    File "..\Release\MobiFlight-Installer.exe"
    File "..\mobiflight.ico"
 
    # create the uninstaller
    WriteUninstaller "$INSTDIR\uninstall.exe"

    ExecWait '"$INSTDIR\MobiFlight-Installer.exe" /installOnly'
 
    # create a shortcut named "new shortcut" in the start menu programs directory
    # point the new shortcut at the program uninstaller
    CreateShortcut "$SMPROGRAMS\${APPNAME}.lnk" "$INSTDIR\MFConnector.exe"

    WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
    WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayIcon" "$instdir\mobiflight.ico"
    WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
    WriteRegDWORD HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoModify" 1
	WriteRegDWORD HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoRepair" 1
SectionEnd

Function un.onInit
    System::Call 'kernel32::OpenMutex(i 0x100000, i 0, t "{57699317-1D72-4B54-82BC-CF6B38254550}")p.R0'
    IntPtrCmp $R0 0 notRunning
        System::Call 'kernel32::CloseHandle(p $R0)'
        MessageBox MB_OK|MB_ICONEXCLAMATION "${APPNAME} is running. Please close it first" /SD IDOK
        Abort
    notRunning:
FunctionEnd
# uninstaller section start
Section "uninstall"
    SetRegView 64
    
    # Remove the link from the start menu
    Delete "$SMPROGRAMS\${APPNAME}.lnk"

    Delete "$DESKTOP\${APPNAME}.lnk"
 
    # Delete the uninstall
    Delete $INSTDIR\uninstall.exe
 
    # Delete all files from the installation folder
    RMDir /r $INSTDIR

    # Delete all setting files
    RMDir /r $LOCALAPPDATA\MobiFlight

    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
# uninstaller section end
SectionEnd