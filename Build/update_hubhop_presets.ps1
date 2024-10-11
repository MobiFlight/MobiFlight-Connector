# Specify the URL of the files to download
$url1 = "https://hubhop-api-mgtm.azure-api.net/api/v1/msfs2020/presets?type=json"
$url2 = "https://hubhop-api-mgtm.azure-api.net/api/v1/xplane/presets?type=json"

# Specify the local folder where you want to save the files
$localFolder = "..\Presets"

# Create the local subfolder if it doesn't exist
if (-not (Test-Path -Path $localFolder)) {
    New-Item -ItemType Directory -Path $localFolder | Out-Null
}

# Download the first file
Invoke-WebRequest -Uri $url1 -OutFile "$localFolder\msfs2020_hubhop_presets.json"

# Download the second file
Invoke-WebRequest -Uri $url2 -OutFile "$localFolder\xplane_hubhop_presets.json"

Write-Host "Files downloaded and saved to $localFolder"
