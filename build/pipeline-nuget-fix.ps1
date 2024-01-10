New-ItemProperty `
    -Path "HKLM:\SYSTEM\CurrentControlSet\Control\FileSystem" `
    -Name "LongPathsEnabled" `
    -Value 1 `
    -PropertyType DWORD `
    -Force
	
New-Item -ItemType Directory -Path "C:\n"

[Environment]::SetEnvironmentVariable("NUGET_PACKAGES", "C:\n", [EnvironmentVariableTarget]::Machine)