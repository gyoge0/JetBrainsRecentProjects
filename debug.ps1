# use this to quickly build the project and restart PowerToys
# run from administrator VS Dev pwsh in project root

$project = "JetBrainsRecentProjects"
$fullName = "Community.PowerToys.Run.Plugin.$project"

Write-Output "Starting build"
# build the project in the background
$build = Start-Job -ScriptBlock {
    cd $fullName
    msbuild /property:Platform=x64
}

Write-Output "Killing PowerToys"
# kill existing PowerToys process
Get-Process PowerToys -ErrorAction SilentlyContinue | Stop-Process -PassThru > $null

$ptPath = "C:\Program Files\PowerToys"
$debug = ".\$fullName\bin\x64\Debug\net8.0-windows"
$dest = "$env:LOCALAPPDATA\Microsoft\PowerToys\PowerToys Run\Plugins\$project"
$files = @(
    "$fullName.deps.json",
    "$fullName.dll",
    "plugin.json",
    "Images"
)

Write-Output "Waiting for build completion"
# kill build job if it doesn't finish in 10 seconds
if (-not (Wait-Job -Job $build -Timeout 10)) {
    Write-Error "Build job timed out"
    Stop-Job -Job $build
    return
}

Push-Location

Write-Output "Copying items"
Set-Location $debug
mkdir $dest -Force -ErrorAction Ignore | Out-Null
Copy-Item $files $dest -Force -Recurse

Write-Output "Starting PowerToys"
& "$ptPath\PowerToys.exe"

Pop-Location
