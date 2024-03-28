# use this to quickly build the project and restart PowerToys
# run from administrator VS Dev pwsh in project root

$project = "JetBrainsRecentProjects"
$fullName = "Community.PowerToys.Run.Plugin.$project"

Write-Output "Building plugin"
cd $fullName
dotnet build -p:Platform=x64 > $null
cd ../

Write-Output "Killing PowerToys"
Get-Process PowerToys -ErrorAction SilentlyContinue | Stop-Process -PassThru > $null
# needs some time to stop the process?
Sleep 0.5 

$ptPath = "C:\Program Files\PowerToys"
$debug = ".\$fullName\bin\x64\Debug\net8.0-windows"
$dest = "$env:LOCALAPPDATA\Microsoft\PowerToys\PowerToys Run\Plugins\$project"
$files = @(
    "$fullName.deps.json",
    "$fullName.dll",
    "plugin.json",
    "Images"
)

Push-Location

Write-Output "Copying items"
Set-Location $debug
mkdir $dest -Force -ErrorAction Ignore | Out-Null
Copy-Item $files $dest -Force -Recurse

Write-Output "Starting PowerToys"
& "$ptPath\PowerToys.exe"

Pop-Location
