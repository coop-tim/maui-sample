# maui-sample

🥳This repository contains a working implementation of MAUI .NET 8 for iOS and Android with the following:
- Firebase Cloud Messaging 
- Firebase Analytics
- Localization samples
 
 As well as the relevant pipelines to build in Azure DevOps and push out to Visual Studio App Center.

After struggling with this for multiple days and slogging through misleading errors, fiddly/brittle config and red herring warnings, I thought it best to give back a working solution to the community. The solution comprises workarounds from several other contributors, though I forget the source of most of them. If you'd like credit let me know.

I would like to see more seamless integration from Microsoft soon for this basic functionality - it definitely feels awkward requiring a reference to a Xamarin package in MAUI and it should not be this hard to get it working.

# Local Dev Setup for Windows Users

- Windows 11 
- VS 17.9.0 Preview 2.1

⚠️ These steps are necessary at time of writing to ensure you can build the project for both Android and iOS.

If you have already tried to get this working and failed, clear all your nuget caches first, and close Visual Studio. Delete bin and obj folders.

## Long paths

Problems with the Xamarin.Firebase.iOS.Core package mean that installation can fail on Windows due to long paths See https://github.com/dotnet/maui/issues/17828. To combat this, you need to enable long paths in the registry and move your local nuget cache. You should also keep the path to your project as short as possible.:

**Powershell**

Run in an elevated prompt
```
New-ItemProperty `
    -Path "HKLM:\SYSTEM\CurrentControlSet\Control\FileSystem" `
    -Name "LongPathsEnabled" `
    -Value 1 `
    -PropertyType DWORD `
    -Force
```
**Nuget cache**

Create a folder named `C:\n`. Add an environment variable:

`NUGET_PACKAGES` = `C:\n`

**Install package !!!manually via CLI!!!**

Don't open or build the project in VS until you have done the next part.

Navigate to your project folder and run in a command line:

`dotnet add package Xamarin.Firebase.iOS.Core`

## Connecting to Mac

### First time Mac setup

1. Install XCode version 15.0 on the mac (go to Apple Developer site to download it)
1. Install the relevant XCode command line tools version
1. Go to XCode > Settings > Platforms and install iOS 17.0
1. Ensure on the mac that the ~/Library/Caches/Xamarin is deleted (it may be hidden so you may have to run `open "$HOME/Library/Caches/"` to see it). It has a counterpart on windows at %LOCALAPPDATA%\Xamarin, delete that too.

# Firebase set up

 1. Create a new project in firebase
 1. Make sure the application ID matches your project's one
1. Add an app each for the iOS and Android versions
1. Add your Apple Team ID to the iOS one
1. Upload an APN Auth Key to the Cloud Messaging settings for the iOS app
1. Download the google-services.json and GoogleServices-Info.plist files and overwrite them in the solution. These will be the files used for running locally

# Run the solution

1. Plug in a real iOS device via USB (this project is untested on Simulators, there are other issues trying to build for those)
1. Open "Pair to Mac" on latest Windows VS 2022 Preview
1. Pair with the mac. It might take a while. If it hangs, restart the mac and visual studio and try again until it works. If not, check they are on the same network and there is no firewall interference.
1. Ensure you set up the Apple provisioning for the project. This is out of scope of this repo.
1. In the Run dropdown, choose iOS Remote Device -> `<Your device name>`
1. Run the build as a Release build, debugging does not work well and I've had no success with hot reload
1. You should now be able to see a token being generated and you should be able to send push notifications from Firebase, as well as view the Analytics data.

# Pipelines

The `.yml` files in `build` folder give you a basis on which to build the app in DevOps and push out to App Center.

You will need to configure variable libraries and secure files for you signing certificates and provisioning profiles. These allow you to target different environments with different settings.

1. Create a variable library called App-Alpha  and add the following config settings, which you'll be able to find in your downloaded `google-services.json` and `GoogleServices-Info.plist`

|Name|Description|
|---|---|
|FirebaseProjectId|google-services.json: `$.project_info.project_id`|
|FirebaseProjectNumber|google-services.json: `$.project_info.project_number`|
|FirebaseStorageBucket|google-services.json: `$.project_info.storage_bucket`|

2. Create a variable library called App-Android-Alpha with the following:

|Name|Description|
|---|---|
|AppCenterSecret|The secret for you App Center app|
|AppCenterSlug|The path to your App Center app (essentially the URL when you're in App Center)|
|ApplicationId|eg `com.companyname.mauisample`|
|DestinationType|groups|
|DistributionGroups|App Center Group GUIDs you want to distribute Android app too|
|FirebaseApiKey|google-services.json: `$.client[0].api_key.current_key` *mark as secret*|
|FirebaseAppId|google-services.json: `$.client[0].client_info.android_client_info.package_name`|
|KeyStorePassword|Password for your keystore *mark as secret*|
|SigningKeyAlias|Signing key alias|
|SigningKeyPassword|Signing key password *mark as secret*|

3. Create a variable library called App-iOS-Alpha with the following:

|Name|Description|
|---|---|
|AppCenterSecret|The secret for you App Center app|
|AppCenterSlug|The path to your App Center app (essentially the URL when you're in App Center)|
|ApplicationId|eg `com.companyname.mauisample`|
|FirebaseGcmSenderId|GoogleServices-Info.plist: `GCM_SENDER_ID`|
|FirebaseApiKey|GoogleServices-Info.plist: `API_KEY` *mark as secret*|
|FirebaseAppId|GoogleServices-Info.plist: `GOOGLE_APP_ID`|
|DestinationType|groups|
|DistributionGroups|App Center Group GUIDs you want to distribute iOS app too|
|P12Password|Password for the P12 cert *mark as secret*|
|ProvisionProfileName|Name of the provisioning profile|

4. Upload your secure files to the Library/Secure Files (generation of these files is out of scope of this repo)

- ios.p12
- ios.mobileprovision
- keystore_alpha.keystore

5. Create App Center service connections to each project in the DevOps project settings

6. Tag your commit with a version, Create and run the pipelines (one for each platform)
    1. On first run, you will need to give permission for the pipelines to access the secure files and libraries
    1. When running, you are asked to fill in the names of the secure files and environment name. They should be prefilled. It's not possible to put these variables in the library as they need to be pre-compiled into the yaml at time of writing due to a long standing bug https://github.com/microsoft/azure-pipelines-tasks/issues/6885. 

7. Profit 💸

# Licence


The MIT License (MIT)

Copyright (c) 2024 coop-tim

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
