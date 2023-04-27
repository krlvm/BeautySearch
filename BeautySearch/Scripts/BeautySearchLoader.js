/**
 * BeautySearch - Windows 10 Search App Tweaker
 * Improved Search Window appearance for Windows 10
 * 
 * This script is injecting the actual BeautySearch script
 * installed for the current user profile
 * 
 * Licensed under the GNU GPLv3 License
 * https://github.com/krlvm/BeautySearch
 *
 * @author krlvm
 **/

const script = document.createElement('script');
script.src = CortanaApp.queryFormulationView.deviceSearch.getUserSID() + '.js';
document.head.appendChild(script);