/**
 * BeautySearch - Windows 10 Search App Tweaker
 * Improved Search Window appearance for Windows 10
 * 
 * Tested on:
 * - Windows 10 May 2019 Update (19H1, Build 18362)
 * - Windows 10 Nov 2019 Update (19H2, Build 18363)
 * - Windows 10 May 2020 Update (20H1, Build 19041)
 * - Windows 10 20H2, Build 19042
 * - Windows 10 21H1, Build 19043
 * - Windows 10 21H2, Build 19044
 * - Windows 10 Next, Build 21390
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
    darkThemeAcrylic: true,
    disableTilesBackground: true,
    contextMenuShadow: true,
    contextMenuRound: true,
    contextMenuAcrylic: true,
    disableContextMenuBorder: false,
    hideOutlines: true,
    explorerSearchBorder: false,
    fakeBackgroundAcrylic: false
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

const FLUENT_NOISE_TEXTURE = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAMAAAAp4XiDAAAAUVBMVEWFhYWDg4N3d3dtbW17e3t1dXWBgYGHh4d5eXlzc3OLi4ubm5uVlZWPj4+NjY19fX2JiYl/f39ra2uRkZGZmZlpaWmXl5dvb29xcXGTk5NnZ2c8TV1mAAAAG3RSTlNAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEAvEOwtAAAFVklEQVR4XpWWB67c2BUFb3g557T/hRo9/WUMZHlgr4Bg8Z4qQgQJlHI4A8SzFVrapvmTF9O7dmYRFZ60YiBhJRCgh1FYhiLAmdvX0CzTOpNE77ME0Zty/nWWzchDtiqrmQDeuv3powQ5ta2eN0FY0InkqDD73lT9c9lEzwUNqgFHs9VQce3TVClFCQrSTfOiYkVJQBmpbq2L6iZavPnAPcoU0dSw0SUTqz/GtrGuXfbyyBniKykOWQWGqwwMA7QiYAxi+IlPdqo+hYHnUt5ZPfnsHJyNiDtnpJyayNBkF6cWoYGAMY92U2hXHF/C1M8uP/ZtYdiuj26UdAdQQSXQErwSOMzt/XWRWAz5GuSBIkwG1H3FabJ2OsUOUhGC6tK4EMtJO0ttC6IBD3kM0ve0tJwMdSfjZo+EEISaeTr9P3wYrGjXqyC1krcKdhMpxEnt5JetoulscpyzhXN5FRpuPHvbeQaKxFAEB6EN+cYN6xD7RYGpXpNndMmZgM5Dcs3YSNFDHUo2LGfZuukSWyUYirJAdYbF3MfqEKmjM+I2EfhA94iG3L7uKrR+GdWD73ydlIB+6hgref1QTlmgmbM3/LeX5GI1Ux1RWpgxpLuZ2+I+IjzZ8wqE4nilvQdkUdfhzI5QDWy+kw5Wgg2pGpeEVeCCA7b85BO3F9DzxB3cdqvBzWcmzbyMiqhzuYqtHRVG2y4x+KOlnyqla8AoWWpuBoYRxzXrfKuILl6SfiWCbjxoZJUaCBj1CjH7GIaDbc9kqBY3W/Rgjda1iqQcOJu2WW+76pZC9QG7M00dffe9hNnseupFL53r8F7YHSwJWUKP2q+k7RdsxyOB11n0xtOvnW4irMMFNV4H0uqwS5ExsmP9AxbDTc9JwgneAT5vTiUSm1E7BSflSt3bfa1tv8Di3R8n3Af7MNWzs49hmauE2wP+ttrq+AsWpFG2awvsuOqbipWHgtuvuaAE+A1Z/7gC9hesnr+7wqCwG8c5yAg3AL1fm8T9AZtp/bbJGwl1pNrE7RuOX7PeMRUERVaPpEs+yqeoSmuOlokqw49pgomjLeh7icHNlG19yjs6XXOMedYm5xH2YxpV2tc0Ro2jJfxC50ApuxGob7lMsxfTbeUv07TyYxpeLucEH1gNd4IKH2LAg5TdVhlCafZvpskfncCfx8pOhJzd76bJWeYFnFciwcYfubRc12Ip/ppIhA1/mSZ/RxjFDrJC5xifFjJpY2Xl5zXdguFqYyTR1zSp1Y9p+tktDYYSNflcxI0iyO4TPBdlRcpeqjK/piF5bklq77VSEaA+z8qmJTFzIWiitbnzR794USKBUaT0NTEsVjZqLaFVqJoPN9ODG70IPbfBHKK+/q/AWR0tJzYHRULOa4MP+W/HfGadZUbfw177G7j/OGbIs8TahLyynl4X4RinF793Oz+BU0saXtUHrVBFT/DnA3ctNPoGbs4hRIjTok8i+algT1lTHi4SxFvONKNrgQFAq2/gFnWMXgwffgYMJpiKYkmW3tTg3ZQ9Jq+f8XN+A5eeUKHWvJWJ2sgJ1Sop+wwhqFVijqWaJhwtD8MNlSBeWNNWTa5Z5kPZw5+LbVT99wqTdx29lMUH4OIG/D86ruKEauBjvH5xy6um/Sfj7ei6UUVk4AIl3MyD4MSSTOFgSwsH/QJWaQ5as7ZcmgBZkzjjU1UrQ74ci1gWBCSGHtuV1H2mhSnO3Wp/3fEV5a+4wz//6qy8JxjZsmxxy5+4w9CDNJY09T072iKG0EnOS0arEYgXqYnXcYHwjTtUNAcMelOd4xpkoqiTYICWFq0JSiPfPDQdnt+4/wuqcXY47QILbgAAAABJRU5ErkJggg==';

const DARK_THEME_CLASS = '.bsDark';
const injectDarkTheme = (parent = '') => {
    parent += ' ';
    injectStyle(`
        {parent}#qfContainer, {parent}#qfPreviewPane {
            color: white !important;
        }
        {parent}#qfContainer {
            background-color: ${SETTINGS.darkThemeAcrylic ? 'rgba(59, 59, 59, 0.3)' : '#3B3B3B'};
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
    
        document.getElementById('root').classList.add('bsAccent');
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
                        0.5 + (SETTINGS.fakeBackgroundAcrylic ? 0.1 : 0)
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
        #menuContainer, #dialog_overlay > div {
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
        body[dir] #dialog_overlay input[type=button] {
            border-radius: 5px;
        }
    `);
}

if(SETTINGS.contextMenuAcrylic) {
    // Dark  context menu: rgb(48, 48, 48)
    // Light context menu: rgb(243, 243, 243)=
    let targetDark = SETTINGS.dynamicDarkTheme ? DARK_THEME_CLASS : '';
    injectStyle(`
        .lightTheme19H1 .contextMenu,${targetDark ? '.darkTheme19H1:not(.bsDark) .contextMenu' : ''},
        .lightTheme19H1 #dialog_overlay > div {
            background-color: rgba(243, 243, 243, 0.1) !important;
            -webkit-backdrop-filter: blur(50px) saturate(175%);
        }
        .lightTheme19H1 #dialog_overlay > div {
            color: #000 !important;
        }

        .zeroInput19H1.darkTheme19H1 .contextMenu,
        ${targetDark ? ' ' + targetDark : ''} .contextMenu,
        .zeroInput19H1.darkTheme19H1 #dialog_overlay > div,
        ${targetDark ? ' ' + targetDark : ''} #dialog_overlay > div {
            background-color: rgba(48, 48, 48, 0.65) !important;
            -webkit-backdrop-filter: blur(50px) saturate(50%);
        }
        .zeroInput19H1.darkTheme19H1 .contextMenu .menuItem *,
        ${targetDark ? ' ' + targetDark : ''} .contextMenu .menuItem *,
        .zeroInput19H1.darkTheme19H1 .contextMenu .menuItem *,
        ${targetDark ? ' ' + targetDark : ''} .contextMenu .menuItem *{
            color: white !important;
        }
        .zeroInput19H1.darkTheme19H1 .contextMenu .menuItem:not(.focusable):not(.menuLabel) *,
        ${targetDark ? ' ' + targetDark : ''} .contextMenu .menuItem:not(.focusable):not(.menuLabel) * {
            color: #B1B6B0 !important;
        }
    `);
}

if(SETTINGS.disableContextMenuBorder) {
    injectStyle(`
        .contextMenu, #dialog_overlay > div {
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

if(SETTINGS.explorerSearchBorder) {
    // In 19H2 Microsoft introduced new Explorer Search experience
    // Now it is based on web-technologies too
    // It is very slow and has HiDPI scaling bug which hiding the bottom border
    // of the Explorer Search Box
    injectStyle(`
        .panelCanResize .lightTheme19H1 .scr {
            border-top: 1px solid #D9D9D9;
        }
        .panelCanResize .darkTheme19H1 .scr {
            border-top: 1px solid #535353;
        }
    `);
}

if(SETTINGS.fakeBackgroundAcrylic) {
    // Base64 Encoded image is the acrylic noise texture
    injectStyle(`
        body {
            background-image: url(${FLUENT_NOISE_TEXTURE}), url(background.png);
            background-size: cover;
            -webkit-backdrop-filter: blur(50px) saturate(105%);
        }
        #root::before {
            content: '';
            position: absolute;
            width: 100vw;
            height: 100vh;
        }
        #root.lightTheme19H1 {
            background: rgba(230, 230, 230, 0.75);
        }
        #root.darkTheme19H1 {
            background: rgba(25, 25, 25, 0.75);
        }
        #root.darkTheme19H1.bsAccent {
            background: rgba(0, 0, 0, 0) !important;
        }
    `);
}