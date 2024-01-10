param (
    [string]$Dir
)

Write-Host "Powershell location: $($Dir)"

if (-not $Dir) {
	throw "Directory empty"
	return;
}

if (-not (Test-Path $Dir)) {
	throw "Directory not found: $($Dir)"
	return;
}

$searchDirectory = "$($Dir)\..\..\MauiSample"

Write-Host "Copying $($Dir)\GoogleService-Info.plist to $($searchDirectory)"
Copy-Item -Path "$($Dir)\GoogleService-Info.plist" -Destination $searchDirectory -Force
Write-Host "$($Dir)\google-services.json to $($searchDirectory)"
Copy-Item -Path "$($Dir)\google-services.json" -Destination $searchDirectory -Force

$searchDirectory = "$($Dir)\..\..\MauiSample\Platforms"

# Read CSV file
$data = Import-Csv -Path "$($Dir)\replacements.csv"

# Iterate through each row in the CSV
foreach ($row in $data) {
    # Construct the file path
	$filePath = Get-ChildItem -Path $searchDirectory -Recurse -Filter $row.filename -File | Select-Object -ExpandProperty FullName

    # Check if the file exists
    if (Test-Path $filePath) {
        # Read the content of the file
        $content = Get-Content -Path $filePath -Raw

        # Replace the search text with the replacement text
        $newContent = $content -replace $row.searchtext, $row.replacementtext

        # Write the updated content back to the file
        Set-Content -Path $filePath -Value $newContent -Encoding UTF8

        Write-Host "Text replaced in $($filePath)"
    } else {
        Write-Host "File not found: $($filePath)"
    }
}
