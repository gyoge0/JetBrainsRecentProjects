Push-Location
Set-Location $PSScriptRoot

$ptPath = "C:\Program Files\PowerToys"

$project = "JetBrainsRecentProjects"
$fullName = "Community.PowerToys.Run.Plugin.$project"

$debug = ".\$fullName\bin\x64\Debug\net8.0-windows"

$dest = "$env:LOCALAPPDATA\Microsoft\PowerToys\PowerToys Run\Plugins\$project"

$files = @(
    "$fullName.deps.json",
    "$fullName.dll",
    "plugin.json",
    "Images"
)

Set-Location $debug
mkdir $dest -Force -ErrorAction Ignore | Out-Null
Copy-Item $files $dest -Force -Recurse

& "$ptPath\PowerToys.exe"

Pop-Location
