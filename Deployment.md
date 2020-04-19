# How to deploy a new version
In this description I am showing how to deploy version **1.1** when the older version was **1.0**

## What to do in Visual studio
1. Change the assembly and file versions
    1. left click on WinStrip project and select properties
    2. In Application tab click "Assembly information..." button
        1. In Assembly version increment the Minor version number from 0 to 1
        2. In File     version increment the Minor version number from 0 to 1
2. select Release in Solution Configurations
3. Right-Click WinStrip project and select Rebuild
4. Click Setup project and change setup Version in the Properties window from 1.0.0 to 1.1.0
    - Click Yes when asked to generate a new UpgradeCode (GUID)
5. Right-Click Setup project and select Rebuild

## Getting the release to the project webpage.
This is important because older versions will check if there is a newer versions of the application 
and if you will not, for example update them file [release.json](https://guttih.com/public/projects/winstrip/release/release.json) older versions will not figure out that there is a new update released.

5. Create a new release on the server
    1. open WinSCP and navigate to the folder "/var/www/web-guttih/public/projects/winstrip/release"
    2. Right-Click the folder "x.x" and select "Duplicate"
    3. Change the last directory name from "x.x" to "1.1" and press "OK"
    4. Edit the file "/var/www/web-guttih/public/projects/winstrip/release/1.1/release.json"
        1. Change the version form "x.x" to "1.1"
        2. Change the description to an overall text on what and why you where updating.
        3. Change the list of newFeatures to what features you where updating.
        4. Change the list of bugFixes to what bugs you where fixing.
        5. Save the file.
    5. Copy the file WinStripSetup.msi to the server release folder
        1. Open the folder "C:\Users\gutti\source\repos\WinStrip\Setup\Release"
        2. Drag the file  WinStripSetup.msi from the folder you just opened to the WinSCP right window which is opened at "/var/www/web-guttih/public/projects/winstrip/release/1.1"
    6. Update the release.json file which the application WinStrip uses to check if there is a newer version.
        1. Copy the new file "release/1.1/release.json"
            - In WinSCP select the file "/var/www/web-guttih/public/projects/winstrip/release/1.1/release.json" and press Ctrl+C
        2. Paste over the file "release/release.json" 
            - In WinSCP navigate to "/var/www/web-guttih/public/projects/winstrip/release"
            - Click on a white part in the window and press Ctrl-v
    7. A Good Idea would be to save the old release help files, so you will be able to update them in new releases.
        1. Copy all content except the file "/var/www/web-guttih/public/projects/winstrip/release/release.json" from "/var/www/web-guttih/public/projects/winstrip/release" to "/var/www/web-guttih/public/projects/winstrip/release/1.0"
    8. In the file menu.html, update the download and help file links to include this new version 1.1 
