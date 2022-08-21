/**
 * BeautySearch - Windows 10+ Search App Tweaker
 * Improved Search Window appearance for Windows 10 and 11
 * 
 * Tested on:
 * - Windows 11 22H2
 * 
 * You should have received an .EXE installer that
 * automatically injects this script and applies your preferences.
 * 
 * The installer modifies files of the Search application.
 * If you are unable to run Search after installation
 * run "sfc /scannow" command in terminal to repair the application.
 *
 * You have to configure folder access permission youself if you prefer
 * manual installation.
 *
 * Optional: you can patch the Search App files and integrate the controller
 * for more correct work of the Dark Theme, Accent Background and other features
 * 
 * Path of install:
 * 22H2: C:\Windows\SystemApps\MicrosoftWindows.Client.CBS_cw5n1h2txyewy\Cortana.UI\cache\SVLocal\Desktop\2.html
 * 
 * Installation can be corrupted after Windows update, system repair or
 * Search application update.
 * You should reinstall BeautySearch after update.
 * 
 * Licensed under the GNU GPLv3 License
 * https://github.com/krlvm/BeautySearch
 *
 * @version 1.15
 * @author krlvm
 **/

const SETTINGS_DEFAULTS = {
    useController: true,           // true | false
    hideOutlines: true,            // true | false
    unifyMenuWidth: true,          // true | false
    contextMenuAcrylic: true,      // true | false
    compactContextMenus: true,     // true | false
    improveContextMenus: true,     // true | false
    improveAppearance: true,       // true | false
    alwaysShowActivityPath: true,  // true | false
    hideUWPReviewShare: true,      // true | false
    globalInstall: true,
}

// Setting 'useController' should be enabled
// only if you hooked the controller when patching
// the Search App files
//
// Settings should be written to the file by the installer
// Use defaults if the script is injected manually
const SETTINGS = SETTINGS_DEFAULTS;

const VERSION = '1.15';

console.log('BeautySearch v' + VERSION + ' is loaded');

/** Helper functions */

let _controller = null;
let _controllerQueue = [];
const getController = (callback) => {
    const isAvailable = typeof bsController !== 'undefined' && bsController != null;
    if(isAvailable) {
        _controller = bsController;
        callback(_controller);
    } else {
        showTemporaryMessage('<b>WARNING:</b> BeautySearch Controller is not available, some features may not work correctly. Reinstall BeautySearch or disable Controller Integration.');
    }
}
const awaitController = (callback) => {
    if (_controller) {
        callback(_controller);
    } else {
        setTimeout(() => getController(callback), 2800);
    }
}

const launchUri = (uri) => {
    getController(controller => controller.launchUri(uri));
}

const DEF_LIGHT = 'lightTheme';
const DEF_DARK  = 'darkTheme';

const isSystemLightTheme = () => {
    return document.getElementById('root').classList.contains(DEF_LIGHT);
}

const executeOnShown = (callback) => {
    if(SETTINGS.useController) {
        awaitController(controller => controller.bindShown(callback));
    } else {
        window.addEventListener('click', () => callback);
    }
}
const executeOnLoad = (callback, always = true) => {
    window.addEventListener('load', () => setTimeout(callback, 50));
    if(!always) return;
    executeOnShown(callback);
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

const CLASS_VISUAL_EFFECTS_NAME = 'bsVisualEffects';
const CLASS_VISUAL_EFFECTS = '.' + CLASS_VISUAL_EFFECTS_NAME;

const PREVIEW_CONTAINER = (SETTINGS.version2022 ? '.' : '#') + 'previewContainer';
const FLUENT_NOISE_TEXTURE = 'data:image/png;base64,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';

const DARK_THEME_CLASS_NAME = 'darkTheme', DARK_THEME_CLASS = '.' + DARK_THEME_CLASS_NAME;

const HOME_CLASS_NAME = 'zeroInput19H1', HOME_CLASS = '.' + HOME_CLASS_NAME;

const selectors = {
    light: (target, prefix='') => target.map(t => `${prefix}.${DEF_LIGHT} ${t}, ${prefix}.${DEF_DARK}:not(${HOME_CLASS}):not(${DARK_THEME_CLASS}) ${t}`),
    dark:  (target, prefix='') => target.map(t => `${prefix}.${DEF_DARK}.${HOME_CLASS_NAME} ${t}, ${prefix}${DARK_THEME_CLASS} ${t}`)
}

const getAcrylicBackgrounds = (tint) => [
    `url(${FLUENT_NOISE_TEXTURE})`,              // L5
    layeredColor(tint),                          // L4, Opacity: 80-60%
    //layeredColor('rgba(255, 255, 255, 0.1)'),  // L3
]
const injectAcrylicStyle = (parents, tint, prefix) => {
    // background: front, ..., back
    // background-blend-mode is not supported on EdgeHTML
    const isCommonTint = !!tint && !Array.isArray(tint);
    let lightTint, darkTint;
    if (tint) {
        if (Array.isArray(tint)) {
            lightTint = tint[0], darkTint = tint[1];
        } else {
            lightTint = tint, darkTint = tint;
        }
    } else {
        lightTint = 'rgba(255, 255, 255, 0.6)';
        darkTint = 'rgba(0, 0, 0, 0.6)';
    }
    
    let wrapper, parent;
    if (Array.isArray(parents)) {
        wrapper = parents[0];
        parent = parents[1];
    } else {
        wrapper = parent = parents;
    }

    console.log(wrapper);
    return injectStyle(`
        ${wrapper} {
            -webkit-backdrop-filter: blur(60px) saturate(150%) !important;
        }
        ${(isCommonTint ? `
            ${parent} {
                background: ${getAcrylicBackgrounds(tint)} !important;
            }
        ` : `
            ${selectors.light([parent], prefix)} {
                background: ${getAcrylicBackgrounds(lightTint)} !important;
            }
            ${selectors.dark([parent], prefix)} {
                background: ${getAcrylicBackgrounds(darkTint)} !important;
            }
        `)}
    `);
}

/** Tweaks */

if(SETTINGS.unifyMenuWidth) {
    const width = 290;
    injectStyle(`
        .contextMenu:not(.moreScopesDropdown):not(.advancedScopesDropdown) {
            width: ${width}px !important;
            max-width: ${width}px !important;
        }
    `);
}

if(SETTINGS.compactContextMenus) {
    injectStyle(`
        body[dir] :not(.fileExplorer)#root .contextMenu :not(.menuInfo):not(.removeHover):not(.menuItemWithButton):not(.menuLabel).menuItem {
            padding: 3px 6px !important;
        }
    `);
}

if(SETTINGS.improveContextMenus) {
    document.body.classList.add(CLASS_VISUAL_EFFECTS_NAME);
}

if(SETTINGS.contextMenuAcrylic || SETTINGS.globalInstall) {
    const tint = [ 'rgba(255, 255, 255, 0.5)', 'rgba(0, 0, 0, 0.45)' ];
    injectAcrylicStyle(['.contextMenu::before', '.contextMenu'], tint, CLASS_VISUAL_EFFECTS + ' ', '');

    const prefix = CLASS_VISUAL_EFFECTS + ' ';

    injectStyle(`
        ${selectors.light(['.contextMenu .menuItem.focusable:hover'], prefix)} {
            background-color: rgba(0, 0, 0, 0.05) !important;
        }
        ${selectors.light(['.contextMenu .menuItem.focusable:active'], prefix)} {
            background-color: rgba(0, 0, 0, 0.03) !important;
        }

        ${selectors.dark(['.contextMenu .menuItem.focusable:hover'], prefix)} {
            background-color: rgba(255, 255, 255, 0.05) !important;
        }
        ${selectors.dark(['.contextMenu .menuItem.focusable:active'], prefix)} {
            background-color: rgba(255, 255, 255, 0.03) !important;
        }
    `);
}

if (SETTINGS.improveContextMenus) {
    const prefix = `body:not(${CLASS_VISUAL_EFFECTS}) `;

    injectStyle(`
        ${selectors.light(['.contextMenu'], prefix)} {
            background-color: rgb(249, 249, 249) !important;
        }
        ${selectors.dark(['.contextMenu'], prefix)} {
            background-color: rgb(44, 44, 44) !important;
        }

        ${prefix}.contextMenu {
            box-shadow: unset !important;
        }
        ${prefix}.contextMenu::before {
            -webkit-backdrop-filter: unset !important;
        }
    `);

    injectStyle(`
        .contextMenu, .contextMenu::before {
            border-radius: 8px !important;
        }

        ${selectors.light(['.contextMenu .menuItem.focusable:active'])} {
            color: black !important;
        }
        ${selectors.dark(['.contextMenu .menuItem.focusable:active'])} {
            color: white !important;
        }
    `);
}

if (SETTINGS.improveAppearance) {
    // Search App Home Page Tweaks
    injectStyle(`
        .group[data-region="TopApps"] .suggestion {
            border-radius: 4px !important;
        }

        ${selectors.light(['.group:not([data-region="MRUHistory"]) .suggestion'], HOME_CLASS)} {
            box-shadow: none !important;
            border: 0.5px solid rgba(0, 0, 0, 0.15) !important;
        }
        ${selectors.light(['.group:not([data-region="MRUHistory"]) .suggestion:hover'], HOME_CLASS)} {
            border-color: rgba(0, 0, 0, 0.125) !important;
        }
        ${selectors.light(['.group:not([data-region="MRUHistory"]) .suggestion:active'], HOME_CLASS)} {
            border-color: rgba(0, 0, 0, 0.1) !important;
        }

        ${selectors.dark(['.group:not([data-region="MRUHistory"]) .suggestion'], HOME_CLASS)} {
            box-shadow: none !important;
            border: 0.5px solid rgba(255, 255, 255, 0.06) !important;
        }
        ${selectors.dark(['.group:not([data-region="MRUHistory"]) .suggestion:hover'], HOME_CLASS)} {
            border-color: rgba(255, 255, 255, 0.03) !important;
        }
        ${selectors.dark(['.group:not([data-region="MRUHistory"]) .suggestion:active'], HOME_CLASS)} {
            border-color: rgba(255, 255, 255, 0.02) !important;
        }
        ${selectors.dark(['.group:not([data-region="MRUHistory"]) .suggestion:active *'], HOME_CLASS)} {
            color: rgba(255, 255, 255, 0.75) !important;
        }
    `);

    // Search App Details Text Tweaks
    injectStyle(`
        .previewDataSection .groupTitle {
            font-weight: bold !important;
        }
        
        ${selectors.light(['.previewDataSection .secondaryText'])} {
            color: black !important;
        }
        ${selectors.dark(['.previewDataSection .secondaryText'])} {
            color: white !important;
        }
    `);
}

if (SETTINGS.alwaysShowActivityPath) {
    injectStyle(`
        ${HOME_CLASS} .suggestion .additionalInfoText {
            display: block !important;
            line-height: 16px;
            font-size: 13px;
        }
        ${HOME_CLASS}.${DEF_LIGHT} .suggestion .additionalInfoText {
            color: rgba(0, 0, 0, 0.6) !important;
        }
    `);
}

document.body.classList.add('mouseNavigation');
window.addEventListener('keydown', () => document.body.classList.remove('mouseNavigation'));
window.addEventListener('mousedown', () => document.body.classList.add('mouseNavigation'));
if(SETTINGS.hideOutlines) {
    injectStyle(`
        .mouseNavigation * {
            outline: none !important;
        }
        .mouseNavigation *:active,
        .mouseNavigation *:focus {
            border-color: transparent !important;
        }
    `);
}

if(SETTINGS.hideUWPReviewShare) {
    injectStyle(`
        .mouseNavigation .menuItem#Review, .mouseNavigation .menuItem#Share,
        .mouseNavigation .selectable#pp_Review, .mouseNavigation .selectable#pp_Share {
            display: none;
        }
    `);
}

injectStyle(`
    #preBootstrapBuildInfo {
        display: none;
    }

    .previewDataSection .label {
        -ms-user-select: none;
    }
`);

if (SETTINGS.globalInstall) {
    let bs_config = localStorage.getItem('beautysearch');
    if (bs_config == null) {
        bs_config = { visualEffects: SETTINGS.contextMenuShadows && SETTINGS.contextMenuAcrylic };
        localStorage.setItem('beautysearch', JSON.stringify(bs_config));
    } else {
        bs_config = JSON.parse(bs_config);
    }

    if (bs_config.visualEffects) {
        document.body.classList.add(CLASS_VISUAL_EFFECTS_NAME);
    }

    const toggleVisualEffects = () => {
        document.body.classList.toggle(CLASS_VISUAL_EFFECTS_NAME);
        bs_config.visualEffects = !bs_config.visualEffects;
        localStorage.setItem('beautysearch', JSON.stringify(bs_config));
    }

    executeOnShown(() => {
        setTimeout(() => {
            const button = document.getElementById('optionsButton');
            if (!button || button.dataset.bsOptions) return;
            let last_rclick = 0;
            button.addEventListener('contextmenu', () => {
                if (Date.now() - last_rclick <= 250) {
                    toggleVisualEffects();
                } else {
                    last_rclick = Date.now();
                }
            });
            button.addEventListener('keydown', (event) => {
                if(event.key === 'b') {
                    toggleVisualEffects();
                }
            });
            button.dataset.bsOptions = true;
        }, 500);
    });
}