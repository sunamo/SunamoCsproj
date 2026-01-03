$warnings = Get-Content "E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoCsproj\rebuild-output.txt" | Select-String 'warning CS8'
$fileWarnings = @{}

foreach ($warning in $warnings) {
    if ($warning -match '([^:]+\.cs)\(\d+,\d+\): .*warning CS8(\d+)') {
        $file = $matches[1]
        $code = "CS8$($matches[2])"

        if (-not $fileWarnings.ContainsKey($file)) {
            $fileWarnings[$file] = @{}
        }
        if (-not $fileWarnings[$file].ContainsKey($code)) {
            $fileWarnings[$file][$code] = 0
        }
        $fileWarnings[$file][$code]++
    }
}

Write-Host "Files with most CS8xxx warnings:"
$fileWarnings.GetEnumerator() | Sort-Object { ($_.Value.Values | Measure-Object -Sum).Sum } -Descending | ForEach-Object {
    $total = ($_.Value.Values | Measure-Object -Sum).Sum
    Write-Host "`n$($_.Key): $total warnings"
    $_.Value.GetEnumerator() | Sort-Object Value -Descending | ForEach-Object {
        Write-Host "  $($_.Key): $($_.Value)"
    }
}
