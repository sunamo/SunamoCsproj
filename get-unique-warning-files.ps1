$warnings = Get-Content "E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoCsproj\full-build.txt" | Select-String 'warning CS8'
$files = $warnings | ForEach-Object {
    if ($_ -match '([^:]+\.cs)\(\d+,\d+\): .*warning CS8') {
        $matches[1]
    }
} | Select-Object -Unique | Sort-Object

Write-Host "Files with CS8xxx warnings:"
$files | ForEach-Object { Write-Host $_ }
Write-Host "`nTotal files: $($files.Count)"
