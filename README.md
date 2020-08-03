<div align="center">
<img src="https://raw.githubusercontent.com/krlvm/PowerTunnel/master/github-images/logo.png" height="192px" width="192px" />
<br><h1>BeautySearch</h1><br>
Windows 10 Search Window appearance tweaker
<br><br>
<a href="https://github.com/krlvm/BeautySearch/blob/master/LICENSE"><img src="https://img.shields.io/github/license/krlvm/PowerTunnel?style=flat-square" alt="License"/></a>
<a href="https://github.com/krlvm/BeautySearch/releases/latest"><img src="https://img.shields.io/github/v/release/krlvm/BeautySearch?style=flat-square" alt="Latest release"/></a>
<!--<a href="https://github.com/krlvm/BeautySearch/releases"><img src="https://img.shields.io/github/downloads/krlvm/PowerTunnel/total?style=flat-square" alt="Downloads"/></a>-->
<!--<a href="https://github.com/krlvm/BeautySearch/wiki"><img src="https://img.shields.io/badge/help-wiki-yellow?style=flat-square" alt="Help on the Wiki"/></a>-->
<br>
<img src="https://raw.githubusercontent.com/krlvm/PowerTunnel/master/github-images/ui.png" alt="BeautySearch Installer" />
</div>

Windows 10 Search User Interface is built on web technologies, so it can be easily tweaked by injecting a custom JavaScript file.\
The [BeautySearch script](https://github.com/krlvm/BeautySearch/blob/master/BeautySearch.js) can be installed automatically using by the Installer written in C# or manually.

## Available tweaks

### Show accent color on Search Window
You can show accent color on Start Menu, Action Center and Taskbar with Windows 10, but not on Search Window for now.\
BeautySearch can fix this flaw.

### Show search results in Dark Theme
Windows 10 still show search results in Light Theme, although Dark Theme is enabled.\
BeautySearch implements the missing Dark Theme for search results.

### Remove background from UWP applications icons
UWP icons background was removed from the Start Menu in the last Insider Builds, but it still present in other pieces of the OS.\
BeautySearch helps you to get rid of that inconsistency for the Search Window.

### Context menu tweaking
The context menus of the Search Window are unlike the others in the system.\
BeautySearch provides following tweaks for the context menus:
- Adding shadows to context menus
- Making the corners of context menus rounded
- Adding acrylic effect to the context menus

### Hide outlines when using mouse
There are outlines in UI to help the people navigating using keyboard. However, outlines in the Search Window are visible when you navigating with mouse.\
BeautySearch can show the outlines only when you're using keyboard to make the UI look better.

## Instlallation

### Using Instaler
You need to run Windows 10 May 2020 Update (version 2004, 20H1, build 19041) and higher to install BeautySearch using the Installer. BeautySearch was not tested with the older versions of Windows 10.

BeautySearch is tested on:
 - Windows 10 May 2020 Update (20H1, Build 19041)
 - Windows 10 ??? 2020 Update (20H2, Build 19042)

Download the Installer [here](https://github.com/krlvm/BeautySearch/releases/latest) and run it as administrator.\
Select the tweaks you want to apply and click "Install".

### Manual installation
Download the last version of the BeautySearch script [here](https://raw.githubusercontent.com/krlvm/BeautySearch/master/BeautySearch.js) and modify it in order to get the tweaks you're need.\
Navigate to `C:\Windows\SystemApps\Microsoft.Windows.Search_cw5n1h2txyewy\cache\Local\` and take full ownership & access over the `Desktop` folder.\
Open `2.html` in Notepad or any other text editor and add this line to the end of the file:\
`<script type=\"text/javascript\" src=\"ms-appx-web:///cache/local/Desktop/BeautySearch.js\"></script>`\
Copy the script to the folder and end task `SearchApp.exe` (you can execute `taskkill /f /im SearchApp.exe` in the terminal).

## Troubleshooting
If you're unable to open the Search Window after installation, the application files was corrupted. Most likely, BeautySearch is not compatible with your version of Windows 10.\
Execute `sfc /scannow` in the terminal to repair the application files. If you're still can't open the Search Window run this command in PowerShell to reinstall the Search app:\
`Get-AppxPackage -AllUsers Microsoft.Windows.Search | Foreach {Add-AppxPackage -DisableDevelopmentMode -Register "$($_.InstallLocation)\AppXManifest.xml"}`

If you're experiencing other problems, open a new issue.
