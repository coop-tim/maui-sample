#!/bin/bash

Dir=$1
echo "Powershell location: $Dir"

if [ -z "$Dir" ]; then
    echo "Directory empty"
    exit 1
fi

if [ ! -d "$Dir" ]; then
    echo "Directory not found: $Dir"
    exit 1
fi

searchDirectory="$Dir/../../MauiSample"

echo "Copying $Dir/GoogleService-Info.plist to $searchDirectory"
cp -f "$Dir/GoogleService-Info.plist" "$searchDirectory"
echo "Copying $Dir/google-services.json to $searchDirectory"
cp -f "$Dir/google-services.json" "$searchDirectory"

searchDirectory="$Dir/../../MauiSample/Platforms"

# Read CSV file
while IFS=, read -r filename searchtext replacementtext; do
    # Construct the file path
    filePath=$(find "$searchDirectory" -type f -name "$filename")

    # Check if the file exists
    if [ -n "$filePath" ]; then
        # Read the content of the file
        content=$(< "$filePath")

        # Replace the search text with the replacement text
        newContent="${content//$searchtext/$replacementtext}"

		echo "$newContent"

        # Write the updated content back to the file
        echo "$newContent" > "$filePath"

        echo "Text replaced in $filePath"
    else
        echo "File not found: $filePath"
    fi
done < "$Dir/replacements.csv"