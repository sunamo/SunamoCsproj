# EN: Script to automatically fix common nullable reference warnings
# CZ: Skript pro automatickou opravu běžných nullable reference warningů

$projectRoot = "E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoCsproj\SunamoCsproj"

# EN: Get all CS files excluding _sunamo folder which has external code
# CZ: Získat všechny CS soubory kromě _sunamo složky která má externí kód
$csFiles = Get-ChildItem -Path $projectRoot -Filter "*.cs" -Recurse | Where-Object { $_.FullName -notmatch '\\_sunamo\\' }

Write-Host "Processing $($csFiles.Count) files..."

foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content

    # EN: Pattern 1: Add null checks before Path.GetDirectoryName usage
    # CZ: Pattern 1: Přidat null kontroly před použitím Path.GetDirectoryName
    $content = $content -replace '(\s+)(var\s+\w+\s+=\s+Path\.GetDirectoryName\([^)]+\);)', '$1$2' + "`n" + '$1if ($2 == null) throw new InvalidOperationException("Directory path is null");'

    # EN: Pattern 2: Add ! operator for known non-null returns
    # CZ: Pattern 2: Přidat ! operátor pro známé non-null návratové hodnoty
    # (This is manual for now, will handle separately)

    if ($content -ne $originalContent) {
        Write-Host "Modified: $($file.FullName)"
        Set-Content -Path $file.FullName -Value $content -NoNewline
    }
}

Write-Host "Done!"
