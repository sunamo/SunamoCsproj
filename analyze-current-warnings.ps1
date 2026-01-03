$warnings = Get-Content "E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoCsproj\rebuild-output.txt" | Select-String 'warning CS'
$warningCodes = $warnings | ForEach-Object {
    if ($_ -match 'warning (CS\d+):') {
        $matches[1]
    }
}
$grouped = $warningCodes | Group-Object | Sort-Object Count -Descending
Write-Host "Total warnings: $($warnings.Count)"
Write-Host "`nWarning breakdown:"
$grouped | Format-Table Count, Name -AutoSize
