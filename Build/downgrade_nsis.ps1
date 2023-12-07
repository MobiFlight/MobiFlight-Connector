################################################################################
##  File:  Install-NSIS.ps1
##  Desc:  Install NSIS
################################################################################

$NsisVersion = "3.04"
Invoke-WebRequest "https://netcologne.dl.sourceforge.net/project/nsis/NSIS%203/${NsisVersion}/nsis-${NsisVersion}-setup.exe" -OutFile "C:\WINDOWS\Temp\nsis-${NsisVersion}-setup.exe"
Start-Process -Wait -FilePath "C:\WINDOWS\Temp\nsis-${NsisVersion}-setup.exe" -ArgumentList "/S"

# Add the newly installed version to the path.
$NsisPath = "${env:ProgramFiles(x86)}\NSIS\"
Add-MachinePathItem $NsisPath
$env:Path = Get-MachinePath

# Write out the version that's now on the path for confirmation of the installed version
# in GitHub action logs.
makensis.exe /VERSION