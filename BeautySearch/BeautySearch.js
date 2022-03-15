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
 * Optional: you can patch the Search App files and integrate the controller
 * for more correct work of the Dark Theme, Accent Background and other features
 * 
 * Path of install:
 * 19H1+: C:\Windows\SystemApps\Microsoft.Windows.Cortana_cw5n1h2txyewy\cache\Local\Desktop\2.html
 * 20H1+: C:\Windows\SystemApps\Microsoft.Windows.Search_cw5n1h2txyewy\cache\Local\Desktop\2.html
 * 
 * Installation can be corrupted after Windows update, system repair or
 * Search application update.
 * You should reinstall BeautySearch after update.
 * 
 * Licensed under the GNU GPLv3 License
 * https://github.com/krlvm/BeautySearch
 *
 * @version 1.9
 * @author krlvm
 **/

 const SETTINGS_DEFAULTS = {
    version2022: false,            // true | false
    useController: true,           // true | false
    restyleOnLoadAcrylic: true,    // true | false
    restyleOnLoadAccent: true,     // true | false
    restyleOnLoadTheme: true,      // true | false
    unifyMenuWidth: true,          // true | false
    disableTilesBackground: true,  // true | false
    contextMenuFluent: true,       // true | false
    contextMenuShadows: true,      // true | false
    contextMenuAcrylic: true,      // true | false
    explorerSearchFixes: false,    // true | false
    topAppsCardsOutline: false,    // true | false
    hideOutlines: true,            // true | false
    acrylicMode: false,            // true | false | 'fake'
    backgroundMode: true,          // true | false | 'system' | color: String
    enhancedAcrylic: true,         // true | false
    corners: 'sharp',              // 'default' | 'sharp' | 'round'
    theme: 'auto',                 // 'auto'    | 'light' | 'dark'
}

// Setting 'useController' should be enabled
// only if you hooked the controller when patching
// the Search App files
//
// Settings should be written to the file by the installer
// Use defaults if the script is injected manually
const SETTINGS = SETTINGS_DEFAULTS;

const VERSION = '1.10';

console.log('BeautySearch v' + VERSION + ' is loaded');

/** Helper functions */

const getController = (callback) => {
    const isAvailable = typeof bsController !== 'undefined' && bsController != null;
    if(isAvailable) {
        callback(bsController);
    } else {
        showTemporaryMessage('<b>WARNING:</b> BeautySearch Controller is not available, some features may not work correctly. Reinstall BeautySearch or disable Controller Integration.');
    }
}
const awaitController = (callback) => {
    setTimeout(() => getController(callback), 50);
}

const launchUri = (uri) => {
    getController(controller => controller.launchUri(uri));
}

const isSystemLightTheme = () => {
    return document.getElementById('root').classList.contains('${DEF_LIGHT}');
}

const executeOnLoad = (callback, executeOnShown = true) => {
    window.addEventListener('load', () => setTimeout(callback, 50));
    if(!executeOnShown) return;
    if(SETTINGS.useController) {
        awaitController(controller => controller.bindShown(callback));
    } else {
        window.addEventListener('click', () => callback);
    }
}
const executeOnRestyle = (callback, condition) => {
    if(!condition || !SETTINGS.useController) {
        executeOnLoad(callback);
        return;
    }
    window.addEventListener('load', () => setTimeout(callback, 100));
    if(SETTINGS.useController) {
        awaitController(controller => controller.bindAccentColorAndThemeRefreshed(callback));
    } else {
        window.addEventListener('click', () => callback);
    }
}

const listenQuery = (callback) => {
    awaitController(controller => controller.bindQueryChangedOrInitialized(query => callback(query.originalQuery)));
}

const isExplorer = () => {
    return document.getElementById('root').classList.contains('panelCanResize');
}

const injectStyle = (styleString) => {
    const style = document.createElement('style');
    style.textContent = styleString;
    document.head.append(style);
    return style;
}

const showTemporaryMessage = (text, icon = 'ⓘ') => {
    document.getElementById('temporaryMessageWrapper').className = '';
    document.getElementById('temporaryMessage').innerHTML = `
        <div class="visible slideInMessage">
            <div class="message">
                <div class="iconContainer"><div class="icon"><span class="iconContent cortanaFontIcon">${icon}</span></div></div>
                <div class="details"><div class="primaryText">${text}</div></div>
            </div>
        </div>
    `;
}

const hexToRGBA = (hex, alpha) => {
    return `rgba(${hex.match(/\w\w/g).map(x => parseInt(x, 16))}, ${alpha})`;
}

const layeredColor = (color) => {
    return `linear-gradient(${color} 100%, #000 0%)`;
}

const DEF_LIGHT = 'lightTheme' + (SETTINGS.version2022 ? '' : '19H1');
const DEF_DARK  = 'darkTheme'  + (SETTINGS.version2022 ? '' : '19H1');

const PREVIEW_CONTAINER = (SETTINGS.version2022 ? '.' : '#') + 'previewContainer';
const FLUENT_NOISE_TEXTURE = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAYAAAAeP4ixAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAA5OSURBVGhDfdlXq9VcFwXgnB1L7L333hsiKipY0BsFQUQU77zw7/jPBHvvvfd2bN9+5suQg8gXCEnWmmXMMcdaydln4MyZMxM/f/7cvH37tpk4cWLz/v37Zvr06fU8c+bM5ubNm83KlSubly9fNuxcx44dW+eIESOax48fN4sXL27u37/fjB49uvw3btzYXL58uWxmz55d9+bEXLt2bXP37t2KNXXq1ObXr1915dfr9ZrXr183c+fOLfuvX79WDrGHDRtWz2yCc/z48eUPZ3vkyJFucHCwAvz48aNZsGBBOTh//vxZTk+ePKlkgs+ZM6fuv3//XuOjRo0qwLNmzWqePXtW458+fWpGjhzZTJs2rWI8f/685ufNm1c+ipNv3LhxdQJ07dq1KgBRcj969KjwsBNzYGCgQI8ZM6b5/ft38+XLl7p/+vRp8+HDh6bdvn17h6F169ZVUImvX79eXXnx4kXTtm0FwCZwrhiSbMaMGWWDEc/Ymz9/fjNp0qQCkY7xUxwCXOV49+5dAb93714VigyAly1b1ty5c6e67NmhcDnEmjJlSuFxwoZIBLUbNmzosHz79u2qbPjw4cXQt2/fqmKsCsDx1atXNQaoK5a6ritQfBWGFDEAVMybN2+ahQsXFovm+H38+LGSY1/HJ0yYUB0TDzH8dF8cQBUPNOXApnCk85UXvp7KTWALgzSYtQC8Z/rENECY0xW27LDlyg4Z7iVbv359zbFXLE0vX7684sopBh+HWGfPnq0C+MozefLkkpV4CuCvg7CwMZ4x8dt+wm7JkiVVMUYEU6lWSoYtXbl48WKxo+2YMo5tAePPDrOIefjwYXUO+2xoWmyFkS57QMmMzjdt2lQbiwPLQOqwOUDZKQgRcCqUxClHrLa/w3SSAQa4AGTgZCSAeYeAAAkimY1BYQpnhy0FY4mvglasWFFzCiQtvvIgY/Xq1bUWAbazWR8PHjwoYpAgtu44kEZOCgGcRJ1y13o5dOhQl11EMdosifXAEcvWgMIEWLRo0Z8FbjFafJcuXfrDnli3bt2qBQ7QjRs3alw8axERKU5Oa+H8+fPFsrzAZZeS2z3J6iQf+bIxwOqQq92yZUun5drP2ckIUxhVBE1iUiEWHkYlxLDgxnVDN8lP5zwrmC+mrQc7m245bNXmgNddGABHpnvbtHsEwKOjlCGvwjw7kFQknzx5sgNMcs6qVIAkKpUIUIkFdq/IYqF/DyzQ5nTFbkUGCDFPImGVH1vrR0dJR4FIIqELFy7UvXF2diNXsWCECRlywSMeMnW1XbNmTWcCsw7S4KhD9CgwEK6eAaN/cso6Yk+nJGXeof3mvNgAcC+PBYso9rqHZeQhgORI0dpBAkykrBiy9NJ0IEEnFBeJtYcPH+60T0e0i3TomEx8mmBcyyUE3L2FKRFwguqaRIKSCRI8Y09hEvG15hSBQaSQWd4XCjdna4bHGFLldy8WTOzN6yCcrpTQ7tixo7P7ACCQBHYCAGxvGCGFsCeYZNlOgUeChWtOUAsSIU6JzWGVDJYuXVodAoYEEZNi5BDDOoBFXAUgTQzzyNYNHUWEDorVIxk6dUrgBNgzY2Cx7oXJFnMBRALZOehVB10BQAi2zIujSxKnKHExnjngrAn+wJONopykzR42zw5+3jvB2fYZ7yxc+sUQRrPos9V5NyhEZ+hWNyTHBM1jFPukxgbL1oYkACIiO45n3ZcPaB1VAF9KsA5gyFpFnp2TXQrzBS0+ZeiQuO3mzZs74AAGUDCgMQqIq0Pg2AlAep6xTwak4fAp480NBIKwK6arTpCH7yy5bA7sjSEicxY+8OyRqvvywmMsuyFsdi9He/z48Q6DEklu61O1Zx3iiE3Ms/OMUafAOqB70S4GFWeevUWvMOTognEdENsL0zPgGFeENzuG2euSAoKDPzvFUQS5+X6Dtz169GhHOsCq0kky5OBFpmJgzGu5vR5QcsIMO+wiwc6k4EgUy5IMnY/MsvtE0roXQnQKSU76V5iOe+Yjr61ap+XQvXb//v2dKk1IylGLFUA6mMEYdm2F1g1ta6vtVBeAZG9xC64rxiUwhnUJAcYyduVwtfjlUJwFzsYzKWPdM3KAd3UoKpsGP2S0+/bt6yTEhpYrBCsWkwKBTWIJgJLAQYbkIRCmAAFUkWLapRQJlB3HNo95zHrmj6xIRE458hKljhTKhxLYm4eBUkjbfH3GAwqAA5sqxjyAWCAHNilAcEXqjDFJBReYDwmZFxOTpCLG1atXy4fGddmnOz+FmpODXyQNB38xbfkI0nFSg1OX+VaenTt31q7FgK45YA1LgmFFMRYswBiI/v+1nbJ1Koq9jtjVsivqlHuSABxo9sbE01mdIyn3bBSgIKowxjfSpCBktlu3bq0/dYdKBnggrIm01rwAGMPG/9tO2SHDC8sVeONkKq5xPvmSABqgLG4EimvbNSYfTKSrSOTogg0Dsb4+6hMFYMxgBDParkOK8M7QjVWrVpUz1n26RHoKHLqdAu3UMcAlByh2gCkaOHPyAIksxABnnkoU5F4e8XRDDGOkypaM4WxPnTrVYUWVwJiIZh2C2bFIjWZ1ROfY/Ws7RQRidEDhCgDela2dEXD52LgCCaAuiCkGuemeDomnaMUiBU7xHOYU2h44cKBjQCoA0rwdTLuABj6L3xxbwLAOOLCRmHGgbOW6FoIkApAve4yK5y9DhZmPP0Lk94xpzwrj65mvInTbGFJsGG3/Q6/DFC1LbiGbFJSWq9r+QhMcS8ArmMTIThFss89HFt4fAABsDCAS1T357FwKNkdeiFSkDgBJCWIiUVEUQimwsaMS27E/KervlxMnTnQ+1FSnA/b+K1euFPuKACYvt/yRA6iCrCn3kgMUqSGFj4UIjPaLDYA5PpGOuO5JRzyE5VtNB9iKlV9ZFKwABSnOO48s6wc6Ex4YORRgMQImsHnMaCm5AK1w64Ze2WKYPabEAgyTWFS8xIrEtG4CaNfSFfcA2frlRpJ8//qVRZ58LciHIFjabdu2dSYktusIRgZYAAqLGBMY8Ow22u9QCLACY1QiPgjAuHnPGAY82jf/968s4pBtissaGfori+eQJJ4Oe253797dWaBajDXJJQFUcIxEEu4tTgxYF75U2QkIRIoiT11hx9ez+DpjPYjvnuTkw7R4FMHWoXPmogQkIlbHPLPVEXHsfj2Lz+EnTlonHUxY+A5JPQMrmIQYtHb8RahbwLnqlnFJXCXhI1bimFOkvGKx0wXryT179whx5ePevyqsHf5wIpSSPMNTPwcJjFnt44x599l+rRH3GOSovcCREQKM6YYdSgIdpmsxgFUkfwvfVXfF0K18ILrHtkLg4SuvgpHgJa1zrmzZwWzdGWv37Nnz5z2iG/6MdGXEWHBgLHAtpEcgSQCDJCPQUEkALJk42MMcEtiLywbLcpIzoGRiDjk6KZ+Y5pwwKJBdfhtAim462l27dnWco/FsowyAxJa1osVAkVFkpit5fygQcIfEWPdsHjBAgNYt8TGuEKzz1Xl5dErByAJcfrGMyavr8KajYula/UBH26qTTGDsAc1IAYBhVVAFaCdbu5RgOgoMOwnEANJ7x+7Chq9xRYtpV/IBKB456gaS5BbLM+BwucKCFApxzTvKBlAqOH36dD/+f6zYDbDmMIY1BWijL1tJcq97kSDm2GGU/BCDYcWyJREL1WF7//u/Y3m/KAhwZJhzr3C2vtB1VncQSDn80vX6t4LgAAJATgBhF1tY8rsSZ0ExEjYFU7D3QchABEJ0wBeCw64CBLkCJB4bxbITF3gkGAcMDmsBBp/8bMjSvLxs+JOaYnoARMMSODCoWqfkZJZFhX1F0yX5aHE+/nyZKsa9KxD+hHUgxrcZmZCiZznFNaZYzxlXhPjmfIqQIz9rxbir03+6kND221w/Pjix44zEOKhaMp0QSIcwkzc4O2/iLLp0SRy+YmTn02ndwybZpuO6xM960m1j7pEmtnWgQDJFknkK0XHd0oh27969HSZr5fcTRJt0iW3rwri2ew543ZJMQXwBZ6cI7baGan/vjwHqU4QvEhSWziFAx+XDfCRtLSgOqWwVxY+9qzj85Na1+hXFQlOhScl8wPm3AVmkC9n7OTtIgQ8ADppWlEQOUiAPRUlEViQgNk0DAzR/oOW2y1mvciATKYAiLjud4vKa0GW+8rTHjh2rn4MCVjLvBy3lkE8V8pIYOw5JgMCe4jGnUDYOY06xnWKbU7zO6oYY2UZ1N2deB9aiohWJADJSFCnx964RQ9z24MGDnUHV0bRK+5/2FYABlgI47wC22AHKb1ORCxlKZC2Y0x0A2MpBwuIjRaeQZ1xc4HyEAu+ZHQwUIT8CqUCBCBKTvJEtX/0PUUCnhWmSEWOSECCyIx0MSA6Ig/4lSkKJLE4LE2DdsAvSuMPO6J0jHxusiq8TOuO9AhgpAi4uuek2tdipdN+VBBXMr4dhA65AYsaBFeNY1H7j5oECRLcUys9fbuwjQ38E+WZjAzBfX6+uxhDhzCaR7suDSHaKAF5hQGeblk9x8rGxCSmwlw9BBgI6OGixQwCadbX4JSMz9hKkcDHEAsiBBHPA8nWv45KKbQxJroqyY5GsQx4YEIc08woWm78OOSxyJHquXxoZCkIutknOKhZI212B18LsbJ51R/HWhwIxZc2IBSC5iHXu3LkCwM+6sdMAZk3ZBEjZWvJuYRNJIYuNd09e1uJEOebEqT/MDPjbmCPmTfzNojc3cE4FGkvbBWZLOlgkDZ3REc9syQuogHA1H/n6mwaZ5uQWQ9ecyekeKYq03tiZi+x6jCRMexXi3qEwjAuSonQCMG0WyLgxwQQ1DjRbHRNLUkTxYwe8/8nIZZws+cqFVJjEEVfRwDv5sDPmNA/z4OBg8z9ueiVWZxOyPgAAAABJRU5ErkJggg==';
const ACCENT_BACKGROUND_CLASS_NAME = 'bsAccentBackground';
const DARK_THEME_CLASS_NAME = SETTINGS.version2022 || SETTINGS.theme == 'dark' ? DEF_DARK : 'bsDark', DARK_THEME_CLASS = '.' + DARK_THEME_CLASS_NAME;
const injectDarkTheme = (parent = '') => {
    const backdropShade = 'rgba(245, 245, 245, 0.1)';
    injectStyle(`
        ${parent} #qfContainer, ${parent} #qfPreviewPane {
            color: white !important;
        }
        ${parent} #qfContainer {
            background-color: rgba(255, 255, 255, 0.05);
        }
        ${parent} #qfPreviewPane {
            background-color: ${SETTINGS.enhancedAcrylic ? (SETTINGS.backgroundMode ? 'rgba(39, 39, 39, 0.25)' : 'rgba(200, 200, 200, 0.1)') : '#272727' };
        }
        ${parent} ${PREVIEW_CONTAINER} {
            background-color: ${SETTINGS.enhancedAcrylic ? 'rgba(31, 31, 31, 0.25)' : '#1F1F1F' } !important;
        }

        ${parent} .previewDataSection .secondaryText {
            color: white !important;
        }
        ${parent} .previewDataSection .uninstallMessage .message, ${parent} .previewDataSection .uninstallMessage .message .iconContent {
            color: rgba(255, 255, 255, 0.6) !important;
        }
        ${parent} .expanderCircle {
            border-radius: 15px;
            background-color: ${SETTINGS.enhancedAcrylic ? backdropShade : '#272727' };
            ${SETTINGS.enhancedAcrylic ? '-webkit-backdrop-filter: blur(5px)' : '' }
        }

        ${parent} ${PREVIEW_CONTAINER} .divider {
            border: 1px solid ${SETTINGS.enhancedAcrylic ? backdropShade : '#272727' };
            //background: ${SETTINGS.enhancedAcrylic ? backdropShade : '#272727' };
        }

        ${parent} .annotation, ${parent} .iconContainer:not(.accentColor) {
            color: rgba(255, 255, 255, 0.6) !important;
        }
        ${parent} .clickable:hover {
            color: rgba(255, 255, 255, 0.75) !important;
        }

        ${parent} .openPreviewPaneBtn:hover,
        ${parent} .suggDetailsContainer:hover .openPreviewPaneBtn,
        ${parent} .withOpenPreviewPaneBtn:hover .openPreviewPaneBtn {
            border-color: rgba(255, 255, 255, 0.2) !important;
        }
        ${parent} .selected .openPreviewPaneBtn {
            border-color: rgba(255, 255, 255, 0.1) !important;
        }

        ${parent} .contextMenu .menuItem * {
            color: white !important;
        }
        ${parent} .contextMenu .menuItem:not(.focusable):not(.menuLabel) * {
            color: #B1B6B0 !important;
        }
        ${parent} .contextMenu .divider {
            border-color: #6F777D !important;
        }

        ${parent} .sectionItem:hover {
            background-color: rgba(255, 255, 255, 0.25) !important;
        }
        ${parent} .sectionItem:focus {
            background-color: rgba(255, 255, 255, 0.3) !important;
        }

        ${parent}:not(.zeroInput19H1) .suggestions .selectable:not(.sa_hv):hover,
        ${parent}:not(.zeroInput19H1) .suggestions .previewPaneOpened:not(.sa_hv) {
            background-color: rgba(255, 255, 255, 0.1) !important;
        }
        ${parent}:not(.zeroInput19H1) .suggestions .selectable:not(.sa_hv):active {
            background-color: rgba(255, 255, 255, 0.25) !important;
        }
        
        .${ACCENT_BACKGROUND_CLASS_NAME} ${parent}:not(.zeroInput19H1) .suggestions .selectable:not(.sa_hv):hover,
        .${ACCENT_BACKGROUND_CLASS_NAME} ${parent}:not(.zeroInput19H1) .suggestions .previewPaneOpened:not(.sa_hv) {
            background-color: rgba(255, 255, 255, 0.2) !important;
        }
        .${ACCENT_BACKGROUND_CLASS_NAME} ${parent}:not(.zeroInput19H1) .suggestions .selectable:not(.sa_hv):active {
            background-color: rgba(255, 255, 255, 0.3) !important;
        }
    `);
}

const selectors = {
    light: (target) => target.map(t => `.${DEF_LIGHT} ${t}, .${DEF_DARK}:not(.zeroInput19H1):not(${DARK_THEME_CLASS}) ${t}`),
    dark:  (target) => target.map(t => `.${DEF_DARK}.zeroInput19H1 ${t}, ${DARK_THEME_CLASS} ${t}`)
}

const getAcrylicBackgrounds = (tint) => [
    `url(${FLUENT_NOISE_TEXTURE})`,            // L5
    layeredColor(tint),                        // L4, Opacity: 80-60%
    layeredColor('rgba(255, 255, 255, 0.1)'),  // L3
]
const injectAcrylicStyle = (parent, tint) => {
    // background: front, ..., back
    // background-blend-mode is not supported on EdgeHTML
    return injectStyle(`
        ${parent} {
            -webkit-backdrop-filter: blur(30px) saturate(150%);
        }
        ${(tint ? `
            ${parent} {
                background: ${getAcrylicBackgrounds(tint)};
            }
        ` : `
            ${selectors.light([parent])} {
                background: ${getAcrylicBackgrounds('rgba(255, 255, 255, 0.6)')};
            }
            ${selectors.dark([parent])} {
                background: ${getAcrylicBackgrounds('rgba(0, 0, 0, 0.6)')};
            }
        `)}
    `);
}

const SID = CortanaApp.queryFormulationView.deviceSearch.getUserSID();

let backgroundColor = null;
let backgroundAcrylicStyle = null;
const applyFakeAcrylic = (tint) => {
    if(isExplorer()) return;

    if(tint == null) tint = isSystemLightTheme() ? 'rgba(255, 255, 255, 0.75)' : 'rgba(0, 0, 0, 0.78)';
    
    if(backgroundColor == tint) return;
    backgroundColor = tint;

    if(backgroundAcrylicStyle) backgroundAcrylicStyle.remove();
    backgroundAcrylicStyle = injectAcrylicStyle('#root', tint);

    injectStyle(`
        body {
            background-image: url(${SID}.png);
            background-size: cover;
        }
    `);
}

/** Tweaks */

if(SETTINGS.enhancedAcrylic) {
    const backdropShade = 'rgba(194, 194, 194, 0.3)';
    injectStyle(`
        .${DEF_LIGHT} #qfPreviewPane {
            background-color: rgba(245, 245, 245, 0.3);
        }
        .${DEF_LIGHT} ${PREVIEW_CONTAINER} {
            background-color: rgba(255, 255, 255, 0.3) !important;
        }
        .${DEF_LIGHT} .expanderCircle {
            border-radius: 15px;
            background-color: ${backdropShade};
            -webkit-backdrop-filter: blur(5px);
        }
        .${DEF_LIGHT} .previewContainer .divider {
            border: 1px solid ${backdropShade};
            //background: ${backdropShade};
        }
    `);
}

if(SETTINGS.disableTilesBackground) {
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

if(SETTINGS.unifyMenuWidth) {
    const width = 275;
    injectStyle(`
        .contextMenu:not(.moreScopesDropdown):not(.advancedScopesDropdown) {
            width: ${width}px !important;
            max-width: ${width}px !important;
        }
    `);
}

if(SETTINGS.contextMenuFluent) {
    injectStyle(`
        ${selectors.light(['.contextMenu', '#dialog_overlay > div'])} {
            border: 1px solid #C5C5C5;
        }
        ${selectors.dark(['.contextMenu', '#dialog_overlay > div'])} {
            border: 1px solid #1C1C1C;
        }

        ${selectors.light(['#dialog_overlay *:not(input)'])} {
            color: black !important;
        }
        ${selectors.dark(['#dialog_overlay *:not(input)'])} {
            color: white !important;
        }
    `);
    
    if(!SETTINGS.contextMenuAcrylic) {
        injectStyle(`
            ${selectors.light(['.contextMenu', '#dialog_overlay > div'])} {
                background-color: #E4E4E4 !important;
            }
            ${selectors.light(['.contextMenu .menuItem.focusable:hover'])} {
                background-color: #F4F4F4 !important;
            }
            ${selectors.light(['.contextMenu .menuItem.focusable:focus'])} {
                background-color: #FAFAFA !important;
            }

            ${selectors.dark(['.contextMenu', '#dialog_overlay > div'])} {
                background-color: #2B2B2B !important;
            }
            ${selectors.dark(['.contextMenu .menuItem.focusable:hover'])} {
                background-color: #404040 !important;
            }
            ${selectors.dark(['.contextMenu .menuItem.focusable:focus'])} {
                background-color: #464646 !important;
            }
        `);
    }
}

if(SETTINGS.contextMenuShadows) {
    injectStyle(`
        #menuContainer, #dialog_overlay > div {
            box-shadow: 0px 5px 16px 1px rgba(0, 0, 0, 0.3);
        }
    `);
}

if(SETTINGS.contextMenuAcrylic) {
    injectAcrylicStyle('.contextMenu');
    injectAcrylicStyle('#dialog_overlay > div');
    injectStyle(`
        ${selectors.dark(['.contextMenu .menuItem.focusable:hover'])} {
            background-color: rgba(255, 255, 255, 0.2) !important;
        }
    `);
}

if(SETTINGS.explorerSearchFixes) {
    // In 19H2 Microsoft introduced new Explorer Search experience
    // Now it is based on web-technologies too (SearchUI.exe / SearchApp.exe)
    // It is very slow and has HiDPI scaling bug which hiding the bottom border
    // of the Explorer Search Box
    injectStyle(`
        .panelCanResize.${DEF_LIGHT} .scr {
            border-top: 1px solid #D9D9D9;
        }
        .panelCanResize.${DEF_DARK} .scr {
            border-top: 1px solid #535353;
        }
    `);
}

if(SETTINGS.topAppsCardsOutline) {
    injectStyle(`
        .${DEF_LIGHT} .topItemsGroup .selectable {
            background-color: rgba(255, 255, 255, 0.4) !important;
        }
        .${DEF_DARK} .topItemsGroup .selectable {
            background-color: rgba(255, 255, 255, 0.1) !important;
        }

        .topItemsGroup .selectable:hover {
            box-shadow: 0 0 0 2px rgba(255, 255, 255, 0.5) inset !important;
        }
        .topItemsGroup .selectable:active {
            box-shadow: none !important;
            transform: scale(0.98);
        }

        .suggsList > li {
            box-shadow: none !important;
        }
    `);
    if(SETTINGS.acrylicMode) {
        injectStyle(`
            .topItemsGroup .selectable {
                transition: all 25ms ease-out;
                -webkit-backdrop-filter: brightness(101%);
            }
        
            .${DEF_LIGHT} .topItemsGroup .selectable:hover, {
                background-color: rgba(255, 255, 255, 0.48) !important;
            }
            .${DEF_LIGHT} .topItemsGroup .selectable:active {
                background-color: rgba(255, 255, 255, 0.5) !important;
            }

            .${DEF_DARK} .topItemsGroup .selectable:hover {
                background-color: rgba(255, 255, 255, 0.14) !important;
            }
            .${DEF_DARK} .topItemsGroup .selectable:active {
                background-color: rgba(255, 255, 255, 0.18) !important;
            }
        `);
    } else {
        injectStyle(`
            .topItemsGroup .selectable {
                transition: transform 25ms ease-out;
            }

            .${DEF_LIGHT} .topItemsGroup .selectable:hover,
            .${DEF_LIGHT} .topItemsGroup .selectable:active {
                background-color: rgba(255, 255, 255, 0.4) !important;
            }
            .${DEF_DARK} .topItemsGroup .selectable:hover,
            .${DEF_DARK} .topItemsGroup .selectable:active {
                background-color: rgba(255, 255, 255, 0.1) !important;
            }
        `);
    }
}

if(SETTINGS.hideOutlines) {
    injectStyle(`
        .hideOutline * {
            outline: none !important;
        }
    `);
    if (SETTINGS.version2022) {
        injectStyle(`
            .hideOutline #root.darkTheme:not(.zeroInput19H1) .sa_hv.arrowOrTabAction,
            .hideOutline #root.darkTheme:not(.zeroInput19H1) .groupContainer .sa_hv.selectable,
            .hideOutline #root.darkTheme:not(.zeroInput19H1) .sectionItem.selectable.sa_hv,
            .hideOutline #root:not(.zeroInput19H1) .sa_hv.arrowOrTabAction,
            .hideOutline #root:not(.zeroInput19H1) .groupContainer .sa_hv.selectable,
            .hideOutline #root:not(.zeroInput19H1) .sectionItem.selectable.sa_hv {
                border-color: transparent !important;
            }
        `);
    }
    document.body.classList.add('hideOutline');
    // Show outlines on keyboard and hide on mouse
    window.addEventListener('keydown', () => document.body.classList.remove('hideOutline'));
    window.addEventListener('mousedown', () => document.body.classList.add('hideOutline'));
}

if(SETTINGS.acrylicMode == 'fake') {
    // It will be called from Accent Background Tweak if it is enabled
    // Prevent this tweak from being applied to Explorer Search popup
    executeOnRestyle(() => applyFakeAcrylic(), SETTINGS.restyleOnLoadAcrylic);
}

if(SETTINGS.backgroundMode) {
    const root = document.getElementById('rootContainer');
    let backgroundListener;

    if(typeof SETTINGS.backgroundMode == 'boolean') {
        backgroundListener = () => {
            if(isExplorer()) return;

            if(isSystemLightTheme()) {
                root.style.backgroundColor = null;
                root.classList.remove(ACCENT_BACKGROUND_CLASS_NAME);
                if(SETTINGS.acrylicMode == 'fake') {
                    applyFakeAcrylic();
                }
            } else {
                let color = '#' + CortanaApp.themeColors.accentDark1.substr(3);
                root.classList.add(ACCENT_BACKGROUND_CLASS_NAME);
                
                if(SETTINGS.acrylicMode == 'fake') {
                    applyFakeAcrylic(hexToRGBA(color, 0.84));
                } else {
                    root.style.backgroundColor = color;
                }
            }
        };
    } else {
        if(SETTINGS.backgroundMode == 'system') {
            if(!SETTINGS.acrylicMode) {
                injectStyle(`
                    .zeroInput19H1.${DEF_LIGHT} #qfContainer {
                        background-color: #E4E4E4 !important;
                    }
                    .zeroInput19H1.${DEF_DARK} #qfContainer {
                        background-color: #1F1F1F !important;
                    }
                `);
            }
        } else {
            injectStyle(`
                #rootContainer {
                    background-color: ${SETTINGS.accentBackground} !important;
                }
            `);
            backgroundListener = () => root.style.backgroundColor = SETTINGS.accentBackground;
        }
    }

    executeOnRestyle(backgroundListener, SETTINGS.restyleOnLoadAccent);
}

if(SETTINGS.corners != 'default') {
    // + #menuContainer
    const radius = SETTINGS.corners == 'sharp' ? '0' : '4px';
    injectStyle(`
        #menuContainer, .contextMenu {
            border-radius: ${radius};
        }
        body[dir] #dialog_overlay input[type=button] {
            border-radius: ${radius};
        }
        .selectable {
            border-radius: ${radius} !important;
        }
    `);
    if(SETTINGS.contextMenuShadows && SETTINGS.corners == 'sharp') {
        injectStyle(`
            ${selectors.light(['#menuContainer, .contextMenu'])} {
                border-radius: 0.5px !important;
            }
        `);
    }
}

if(SETTINGS.version2022) {
    injectStyle(`
        #preBootstrapBuildInfo {
            display: none;
        }
        .${DEF_LIGHT} .selected .openPreviewPaneBtn {
            border-color: rgba(255, 255, 255, 0.5) !important;
        }
    `);
    if (SETTINGS.hideCloseButton) {
        injectStyle(`
            .scopes-list {
                font-size: 0;
            }
        `);
    }
    injectDarkTheme(DARK_THEME_CLASS);
} else if(SETTINGS.theme) {
    // Dark theme support for search results

    if(SETTINGS.theme != 'light') {
        injectDarkTheme(DARK_THEME_CLASS);

        let darkThemeListener = () => {
            let useDarkTheme = (SETTINGS.theme == 'dark' || !CortanaApp.appsUseLightTheme) && !isSystemLightTheme();
            let rootClasses = document.getElementById('root').classList;
            if(rootClasses.contains(DARK_THEME_CLASS_NAME) && !useDarkTheme) {
                rootClasses.remove(DARK_THEME_CLASS_NAME);
            } else if(!rootClasses.contains(DARK_THEME_CLASS_NAME) && useDarkTheme) {
                rootClasses.add(DARK_THEME_CLASS_NAME);
            }
        };

        executeOnRestyle(darkThemeListener, SETTINGS.restyleOnLoadTheme);
    }
}