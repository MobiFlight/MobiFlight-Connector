################################################################################
##  File:  Install-NSIS.ps1
##  Desc:  Install NSIS
################################################################################
$NsisVersion = "3.04"
# Get-Help Install-Binary -Full

Install-Binary -Url "https://downloads.sourceforge.net/project/nsis/NSIS%203/${NsisVersion}/nsis-${NsisVersion}-setup.exe" -Type EXE -InstallArgs ('/S')

$NsisPath = "${env:ProgramFiles(x86)}\NSIS\"
$env:PATH += ";$NsisPath"

# Write out the version that's now on the path for confirmation of the installed version
# in GitHub action logs.
makensis.exe /VERSION