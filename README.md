<div align="center">
<img src="https://raw.githubusercontent.com/krlvm/BeautySearch/master/github-images/logo.png" height="150px" width="auto" />
<br><h1>BeautySearch</h1>
Windows 10 Search Window appearance tweaker
<br><br>
<a href="https://github.com/krlvm/BeautySearch/blob/master/LICENSE"><img src="https://img.shields.io/github/license/krlvm/BeautySearch?style=flat-square" alt="License"/></a>
<a href="https://github.com/krlvm/BeautySearch/releases/latest"><img src="https://img.shields.io/github/v/release/krlvm/BeautySearch?style=flat-square" alt="Latest release"/></a>
<a href="https://github.com/krlvm/BeautySearch/releases"><img src="https://img.shields.io/github/downloads/krlvm/BeautySearch/total?style=flat-square" alt="Downloads"/></a>
<!--<a href="https://github.com/krlvm/BeautySearch/wiki"><img src="https://img.shields.io/badge/help-wiki-yellow?style=flat-square" alt="Help on the Wiki"/></a>-->
<br>
<img src="https://raw.githubusercontent.com/krlvm/BeautySearch/master/github-images/ui.png" alt="BeautySearch Installer" />
</div>

Windows 10 Search User Interface is built on web technologies, so it can be easily tweaked by injecting a custom JavaScript file.\
The [BeautySearch script](https://github.com/krlvm/BeautySearch/blob/master/BeautySearch.js) can be installed automatically using the Installer written in C# or manually.

## Available tweaks

### Fake Acrylic for 20H1+
Search Window Acrylic effect was broken in late 20H1 Insider Builds and apparently won't be fixed, although a fix was already available in Build 19541.
BeautySearch has a special [*Fake Acrylic*](https://github.com/krlvm/BeautySearch/wiki/Acrylic-effect-in-Windows-10-20H1---Search-Window) tweak which re-implements the acrylic effect in a different way with some limitations.

<img src="https://raw.githubusercontent.com/krlvm/BeautySearch/master/github-images/tweaks/fake-acrylic/light.png" width="100%" />

### Show accent color on Search Window
You can enable accent color on Start Menu, Action Center and Taskbar with Windows 10, but not on Search Window.\
BeautySearch can fix this flaw.

<img src="https://raw.githubusercontent.com/krlvm/BeautySearch/master/github-images/tweaks/accent-background/disabled.png" width="100%" />
<img src="https://raw.githubusercontent.com/krlvm/BeautySearch/master/github-images/tweaks/accent-background/enabled.png" width="100%" />

### Show search results in Dark Theme
Windows 10 still show search results in Light Theme, although Dark Theme is enabled.\
BeautySearch implements the missing Dark Theme for search results.

<img src="https://raw.githubusercontent.com/krlvm/BeautySearch/master/github-images/tweaks/dark-theme/comparasion.png" width="100%" />

It also looks great with Accent Background enabled:

<img src="https://raw.githubusercontent.com/krlvm/BeautySearch/master/github-images/tweaks/dark-theme/accent.png" width="100%" />

### Remove background from UWP applications icons
UWP icons background was removed from the Start Menu in the last Insider Builds, but it still present in other pieces of the OS.\
BeautySearch helps you to get rid of that inconsistency for the Search Window.

<img src="https://raw.githubusercontent.com/krlvm/BeautySearch/master/github-images/tweaks/tiles-background/disabled.png" width="100%" />
<img src="https://raw.githubusercontent.com/krlvm/BeautySearch/master/github-images/tweaks/tiles-background/enabled.png" width="100%" />

### Context menu tweaking
The context menus of the Search Window are inconsistent with the others in the system.\
BeautySearch provides following tweaks for the context menus:
- Adding shadows to context menus
- Adding acrylic effect to the context menus
- Making context menus consistent with Fluent Design system
- Making the corners of context menus rounded
- Aligning the width of all Search context menus

<img src="https://raw.githubusercontent.com/krlvm/BeautySearch/master/github-images/tweaks/context-menus/enabled.png" width="100%" />

### Sharp and rounded corners
There are outlines in UI to help the people navigating using keyboard. However, outlines in the Search Window are visible when you navigating with mouse.\
BeautySearch can show the outlines only when you're using keyboard to make the UI look better.

### Adjusting corners
BeautySearch allows you to make all corners sharp to be consistent with the rest of Windows 10 Shell, or rounded to be consistent with the new Fluent Design guidelines.

## FAQ

### Does BeautySearch have a performance impact?
No. BeautySearch does not consume RAM and does not affect performance. It is a somewhat like a stylesheet which is embedded in Search App after installation.

### Does BeautySearch add to startup?
No. BeautySearch Installer is the only executable file distrubuted and only need for automatic and convenient script injection. After installation you can delete the Installer.\
You can also install the script manually without executing any binaries (see below).

### Does it violate my privacy?
No. BeautySearch doesn't have any analytics or telemetry, and doesn't even check for updates.

### Is it safe and stable enough to use?
Yes. As stated earlier, BeautySearch is something like a stylesheet.\
Just in case, check the compatibility of your system: at the time of publication, BeautySearch was tested on Windows 10 versions 19H1-21H1, as well as on the latest Windows 10 Insider Preview build (Build 21390).\
If you ran into a problem after installation, see below.
Windows 11 is not supported, and apparently there is no need for this.

### Bing Search stopped to work after installation
BeautySearch is incompatible with Bing Search. If you live in an unsupported region, you won't even notice.\
To enable it back, just uninstall BeautySearch.\
Also, work is underway to implement search for all regions with any search engine, details can be found [here](https://github.com/krlvm/BeautySearch/issues/1).

### VirusTotal marks BeautySearch Installer as malicious
Only one antivirus out of 71 considers the file to be malicious, and only because the installer writes files to the system directory (see which directory in the manual installation section).
The source code for both the installer and the script is published under the GNU GPLv3 license, and if you don't trust the published binaries, you can easily build the project using Visual Studio 2019.

## Instlallation

### Using Instaler
You need to run Windows 10 May 2019 Update (19H1 aka version 1903, build 18362) and higher to install BeautySearch using the Installer.

BeautySearch is tested on:
 - Windows 10 19H1, 19H2
 - Windows 10 20H1, 20H2, 21H1
 - Windows 10 Next, Insider Preview Build 21390

Download the Installer [here](https://github.com/krlvm/BeautySearch/releases/latest) and run it as administrator.\
Select the tweaks you want to apply and click "Install".

### Manual installation
Download the last version of the BeautySearch script [here](https://raw.githubusercontent.com/krlvm/BeautySearch/master/BeautySearch.js) and modify it in order to get the tweaks you're need.\
Navigate to `C:\Windows\SystemApps\Microsoft.Windows.Search_cw5n1h2txyewy\cache\Local\` (or `C:\Windows\SystemApps\Microsoft.Windows.Cortana_cw5n1h2txyewy\cache\Local\` on 19H1 and 19H2) and take full ownership & access over the `Desktop` folder.\
Open `2.html` in Notepad or any other text editor and add this line to the end of the file:\
`<script type=\"text/javascript\" src=\"ms-appx-web:///cache/local/Desktop/BeautySearch.js\"></script>`\
Copy the script to the folder and end task `SearchApp.exe` (you can execute `taskkill /f /im SearchApp.exe` in the terminal).\
Some additional instructions with screenshots can be found [there](https://github.com/krlvm/BeautySearch/issues/11#issuecomment-860113080).

Since version 1.7, you can install BeautySearch silently with standard options:
```
$ BeautySearch.exe auto
```

## Troubleshooting
If you're unable to open the Search Window after installation, the application files was corrupted. Most likely, BeautySearch is not compatible with your version of Windows 10.\
Execute `sfc /scannow` in the terminal to repair the application files. If you're still can't open the Search Window run this command in PowerShell to reinstall the Search app:\
`Get-AppxPackage -AllUsers Microsoft.Windows.Search | Foreach {Add-AppxPackage -DisableDevelopmentMode -Register "$($_.InstallLocation)\AppXManifest.xml"}`


If you're experiencing problems or want to suggest a feature, feel free to open a new issue.
