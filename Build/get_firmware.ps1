param ($tag = "latest", $outDir = "../firmware")

# Create the output folder if it doesn't exist
if (!(Test-Path $outDir)) {
  New-Item -ItemType Directory -Force -Path $outDir | Out-Null
}

if ($tag -eq "latest") {
  Write-Output "Downloading latest release"
  $releaseDetails = Invoke-RestMethod -Uri "https://api.github.com/repos/MobiFlight/MobiFlight-FirmwareSource/releases/latest"
}
else {
  Write-Output "Downloading firmware tag $($tag)"
  $releaseDetails = Invoke-RestMethod -Uri "https://api.github.com/repos/MobiFlight/MobiFlight-FirmwareSource/releases/tags/${tag}"
}

# The list of assets also includes the zipped source code so filter it to just the
# firmware files
$firmwareFiles = $releaseDetails.assets | where-object { $_.name -match ".hex" }

# Download all the firmware files to the correct location
foreach ($firmware in $firmwareFiles) {
  Write-Output "Downloading $($firmware.name)"
  $outputFileName = Join-Path $outDir $firmware.name
  Invoke-WebRequest $firmware.browser_download_url -OutFile $outputFileName
}