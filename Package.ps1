# Creates the release's .zip file
$ModName = "LongNap";
$PackageFolder = "./Package/$ModName"
$ArchiveName = "$ModName.zip"

Remove-Item -ErrorAction SilentlyContinue -Recurse "./Package/"
Remove-Item -ErrorAction SilentlyContinue $ArchiveName

mkdir $PackageFolder > $null
mkdir $PackageFolder/PackageBin > $null

dotnet publish .\src\CardSurvival-LongNap.csproj -o $PackageFolder/PackageBin -c Release

#Move items individually as dotnet publish always includes NuGet dependencies.
Move-Item $PackageFolder/PackageBin/Lang $PackageFolder
Move-Item $PackageFolder/PackageBin/CardSurvival_LongNap.dll $PackageFolder

Remove-Item -Recurse $PackageFolder/PackageBin

# English name since github strips Unicode for security purposes.
Compress-Archive -DestinationPath $ArchiveName -Path ./Package/*
