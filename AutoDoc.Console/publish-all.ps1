# publish-all.ps1

# Runtimes
$runtimes = @("win-x64", "linux-x64", "osx-x64")

# Configurations
$configuration = "Release"
$selfContained = "true"
$projectPath = "AutoDoc.csproj"
$desktopPath = [Environment]::GetFolderPath("Desktop")
$outputBase = Join-Path $desktopPath "AutoDocBuilds"

# Create folder at Desktop
if (-Not (Test-Path $outputBase)) {
    New-Item -ItemType Directory -Path $outputBase | Out-Null
}

foreach ($rid in $runtimes) {
    Write-Host "🔨 Publishing for $rid..."

    # Create Folders
    $publishDir = Join-Path $outputBase "publish-$rid"
    $zipFile    = Join-Path $outputBase "AutoDoc-$rid.zip"

    # Remove last Folders if exists
    if (Test-Path $publishDir) {
        Remove-Item $publishDir
        Remove-Item $zipFile
    }
    
    # Publish
    dotnet publish $projectPath -c $configuration -r $rid --self-contained $selfContained -o $publishDir

    # Compress only publish folder
    Compress-Archive -Path (Join-Path $publishDir "*") -DestinationPath $zipFile

    Write-Host "✅ Generated: $zipFile"
}

Write-Host "🎉 All builds were done. Look at: $outputBase"
