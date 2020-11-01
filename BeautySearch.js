/**
 * BeautySearch - Windows 10 Search App Tweaker
 * Improved Search Window appearance for Windows 10
 * 
 * Tested on:
 * - Windows 10 May 2019 Update (19H1, Build 18362)
 * - Windows 10 Nov 2019 Update (19H2, Build 18363)
 * - Windows 10 May 2020 Update (20H1, Build 19041)
 * - Windows 10 ??? 2020 Update (20H2, Build 19042)
 * 
 * You should have received an .EXE installer that
 * automatically injects this script and applies your preferences.
 * 
 * The installer modifies files of the Search application.
 * If you are unable to run Search after installation
 * run "sfc /scannow" command in terminal to repair the application.
 * If it does not help, run PowerShell as administrator and execute
 * following command to re-register and repair the Search application:

 * 1903, 1909: Get-AppxPackage -AllUsers Microsoft.Windows.Cortana | Foreach {Add-AppxPackage -DisableDevelopmentMode -Register "$($_.InstallLocation)\AppXManifest.xml"}
 * 2004, 2009: Get-AppxPackage -AllUsers Microsoft.Windows.Search | Foreach {Add-AppxPackage -DisableDevelopmentMode -Register "$($_.InstallLocation)\AppXManifest.xml"}
 * 
 * You have to configure folder access permission youself if you prefer
 * manual installation.
 * 
 * Path of install:
 * C:\Windows\SystemApps\Microsoft.Windows.Search_cw5n1h2txyewy\cache\Local\Desktop\2.html
 * 
 * Installation can be corrupted after Windows update, system repair or
 * Search application update.
 * You should reinstall BeautySearch after update.
 * 
 * Licensed under the GNU GPLv3 License
 * https://github.com/krlvm/BeautySearch
 *
 * @version 1.2.3
 * @author krlvm
 **/

const SETTINGS_DEFAULTS = {
    accentBackground: true,
    darkTheme: true,
    dynamicDarkTheme: true,
    disableTilesBackground: true,
    contextMenuShadow: true,
    contextMenuRound: true,
    contextMenuAcrylic: true,
    disableContextMenuBorder: false,
    hideOutlines: true
}

// Settings should be written to the file by the installer
// Use defaults if the script is injected manually
const SETTINGS = SETTINGS_DEFAULTS;

const VERSION = '1.2.3';
const VERSION_CODE = 7;

console.log('BeautySearch v' + VERSION + ' is loaded');

/** Helper functions */

const showTemporaryMessage = (text, icon = 'ⓘ') => {
    document.getElementById('temporaryMessageWrapper').className = '';
    document.getElementById('temporaryMessage').innerHTML = `
        <div class="visible slideInMessage">
            <div class="message">
                <div class="iconContainer">
                    <div class="icon">
                        <span class="iconContent cortanaFontIcon">${icon}</span>
                    </div>
                </div>
                <div class="details">
                    <div class="primaryText">
                        ${text}
                    </div>
                </div>
            </div>
        </div>
    `;
}

const subtractPercent = (number, percent) => {
    return number - (number*percent);
}

const getRGB = (str) => {
    return str.replace('rgb(', '').replace(')').split(', ').map(function(item) {
        return parseInt(item);
    });
}

const toRGB = (r, g, b) => {
    return 'rgb(' + r + ', ' + g + ', ' + b + ')';
}

const toRGBa = (r, g, b, a) => {
    return 'rgba(' + r + ', ' + g + ', ' + b + ', ' + a + ')';
}

const injectStyle = (styleString) => {
    const style = document.createElement('style');
    style.textContent = styleString;
    document.head.append(style);
}

const isSystemLightTheme = () => {
    return document.getElementById('root').classList.contains('lightTheme19H1');
}

const DARK_THEME_CLASS = '.bsDark';
const injectDarkTheme = (parent = '') => {
    parent += ' ';
    injectStyle(`
        {parent}#qfContainer, {parent}#qfPreviewPane {
            color: white !important;
        }
        {parent}#qfContainer {
            background-color: #3B3B3B;
        }
        {parent}#qfPreviewPane {
            background-color: #2B2B2B;
        }
        {parent}#previewContainer {
            background-color: black !important;
        }
        {parent}.previewDataSection .secondaryText {
            color: white !important;
        }
        {parent}.expanderCircle {
            background-color: #2B2B2B;
            border-radius: 15px;
        }
        {parent}#previewContainer .divider:not(:hover):not(:focus),  {
            border: 1px solid #2B2B2B !important;
        }
        {parent}#previewContainer .divider {
            border: 1px solid #2B2B2B;
        }
        {parent}.annotation, {parent}.iconContainer:not(.accentColor) {
            color: rgba(255, 255, 255, 0.6) !important;
        }
        {parent}.clickable:hover {
            color: rgba(255, 255, 255, 0.75) !important;
        }
        {parent}.openPreviewPaneBtn:hover,
        {parent}.suggDetailsContainer:hover .openPreviewPaneBtn,
        {parent}.withOpenPreviewPaneBtn:hover .openPreviewPaneBtn {
            border-color: rgba(0, 0, 0, 0.3) !important;
        }
        {parent}.contextMenu .menuItem:not(.focusable, .menuLabel) {
            color: #6F777D !important;
        }
        {parent}.contextMenu .divider {
            border-color: #6F777D !important;
        }
        {parent}.sectionItem:hover {
            background-color: rgba(255, 255, 255, 0.25) !important;
        }
        {parent}.sectionItem:focus {
            background-color: rgba(255, 255, 255, 0.3) !important;
        }
    `.replace(/{parent}/g, parent));
}

/** Tweaks */

let lastAccent;
if(SETTINGS.accentBackground) {
    // Search window background to follow accent color

    let backgroundListener = () => {
        // May not work in the future
        //let color = '#' + SearchAppWrapper.CortanaApp.systemTheme.substring(7);
    
        let scopes = document.querySelectorAll('.accentColor');
        if(scopes.length > 0) {
            let base = isSystemLightTheme() ? -1 : window.getComputedStyle(scopes[0]).color;
            console.log(`Base: ${base} | Last: ${lastAccent}`)
            if(lastAccent != base) {
                if(base === -1) {
                    document.getElementById('rootContainer').style.backgroundColor = '';
                } else {
                    let rgb = getRGB(base);
                    document.getElementById('rootContainer').style.backgroundColor = toRGBa(
                        rgb[0],
                        rgb[1],
                        rgb[2],
                        0.5
                    );
                    lastAccent = base;
                }
            }
        }
    };

    window.addEventListener('load', () => {
        setTimeout(backgroundListener, 25);
    });
    window.addEventListener('click', backgroundListener);
    window.addEventListener('keydown', backgroundListener);
}

if(SETTINGS.darkTheme) {
    // Dark theme support for search results

    if(SETTINGS.dynamicDarkTheme) {
        injectDarkTheme(DARK_THEME_CLASS);

        let darkThemeListener = () => {
            let systemDarkTheme = !SearchAppWrapper.CortanaApp.appsUseLightTheme && !isSystemLightTheme();
            let rootClasses = document.getElementById('root').classList;
            if(rootClasses.contains('bsDark') && !systemDarkTheme) {
                rootClasses.remove('bsDark');
            } else if(!rootClasses.contains('bsDark') && systemDarkTheme) {
                rootClasses.add('bsDark');
            }
        };

        window.addEventListener('load', darkThemeListener);
        window.addEventListener('click', darkThemeListener);
        window.addEventListener('keydown', darkThemeListener);
    } else {
        injectDarkTheme('.darkTheme19H1');
    }
}

if(SETTINGS.disableTilesBackground) {
    // Disable background for tiles to be consistent with the new Start menu design
    injectStyle(`
        .icon {
            background: none !important;
        }
        .iconWithBackground {
            transform: scale(1.3);
        }
        .activityFeedGroup .iconWithBackground {
            transform: none !important;
        }
    `);
}

if(SETTINGS.contextMenuShadow) {
    injectStyle(`
        #menuContainer {
            box-shadow: 5px 5px 25px 0px rgba(0, 0, 0, 0.3);
        }
    `);
}

if(SETTINGS.contextMenuRound) {
    // + #menuContainer
    injectStyle(`
        #menuContainer, .contextMenu {
            border-radius: 5px;
        }
    `);
}

if(SETTINGS.contextMenuAcrylic) {
    // Dark  context menu: rgb(48, 48, 48)
    // Light context menu: rgb(243, 243, 243)=
    let targetDark = SETTINGS.dynamicDarkTheme ? DARK_THEME_CLASS : '';
    injectStyle(`
        .lightTheme19H1 .contextMenu,${targetDark ? '.darkTheme19H1:not(.bsDark) .contextMenu' : ''} {
            background-color: rgba(243, 243, 243, 0.1) !important;
            -webkit-backdrop-filter: blur(50px) saturate(175%);
        }

        .zeroInput19H1.darkTheme19H1 .contextMenu,${targetDark ? ' ' + targetDark : ''} .contextMenu {
            background-color: rgba(48, 48, 48, 0.65) !important;
            -webkit-backdrop-filter: blur(50px) saturate(50%);
        }
        .zeroInput19H1.darkTheme19H1 .contextMenu .menuItem *,${targetDark ? ' ' + targetDark : ''} .contextMenu .menuItem * {
            color: white !important;
        }
        .zeroInput19H1.darkTheme19H1 .contextMenu .menuItem:not(.focusable):not(.menuLabel) *,${targetDark ? ' ' + targetDark : ''} .contextMenu .menuItem:not(.focusable):not(.menuLabel) * {
            color: #B1B6B0 !important;
        }
    `);
}

if(SETTINGS.disableContextMenuBorder) {
    injectStyle(`
        .contextMenu {
            border: none !important;
        }
    `);
}

if(SETTINGS.hideOutlines) {
    injectStyle(`
        .hideOutline * {
            outline: none !important;
        }
    `);
    document.body.classList.add('hideOutline');
    // Show outlines on keyboard and hide on mouse
    window.addEventListener('keydown', () => {
        document.body.classList.remove('hideOutline');
    });
    window.addEventListener('mousedown', () => {
        document.body.classList.add('hideOutline');
    });
}