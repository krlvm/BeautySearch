using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeautySearch.Installer
{
    class SearchAppManager
    {
        public const string SEARCH_APP_REGISTRY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Search\";

        private const string VALUE_BING_SEARCH_ENABLED = "BingSearchEnabled";
        private const int BING_SEARCH_DISABLED = 0;
        private const int BING_SEARCH_ENABLED = 1;

        private const string VALUE_SEARCH_BOX_TEXT_REGISTRY = "SearchBoxText";
        private const string VALUE_SEARCH_BOX_TASKBAR_MODE_REGISTRY = "SearchboxTaskbarMode";

        public static string SEARCH_APP_NAME;
        public static string SEARCH_APP_EXEC;

        static SearchAppManager()
        {
            if (SystemInfo.BUILD_NUMBER >= OSBuild.V11_21H2)
            {
                SEARCH_APP_EXEC = "SearchHost";
                SEARCH_APP_NAME = "MicrosoftWindows.Client.CBS_cw5n1h2txyewy";
            }
            else if (SystemInfo.BUILD_NUMBER >= OSBuild.V20H1)
            {
                SEARCH_APP_EXEC = "SearchApp";
                SEARCH_APP_NAME = "Microsoft.Windows.Search_cw5n1h2txyewy";
            }
            else if (SystemInfo.BUILD_NUMBER >= OSBuild.V19H1)
            {
                SEARCH_APP_EXEC = "SearchUI";
                SEARCH_APP_NAME = "Microsoft.Windows.Cortana_cw5n1h2txyewy";
            }
        }

        public static bool KillSearchApp()
        {
            bool success = true;
            foreach (var process in Process.GetProcessesByName(SEARCH_APP_EXEC))
            {
                try
                {
                    process.Kill();
                }
                catch
                {
                    // We will try to kill the Search App using TASKKILL instead of asking to re-login
                    //success = false;
                    Utility.ExecuteCommand("taskkill.exe", "/F /IM " + SEARCH_APP_EXEC + ".exe");
                }
            }
            return success;
        }

        public static void SetBingSearchEnabled(bool isEnabled)
        {
            SetBingSearchEnabled(isEnabled ? BING_SEARCH_ENABLED : BING_SEARCH_DISABLED);
        }

        public static void SetBingSearchEnabled(int val)
        {
            // When Bing Web Search is enabled, the Search App doesn't use the local search instance
            using (var key = Utility.OpenCurrentUserRegistryKey(SEARCH_APP_REGISTRY, true))
            {
                if (key != null)
                {
                    key.SetValue(VALUE_BING_SEARCH_ENABLED, val, RegistryValueKind.DWord);
                    key.Close();
                }
            }
        }

        public static bool IsBingSearchEnabled()
        {
            using (var key = Utility.OpenCurrentUserRegistryKey(SEARCH_APP_REGISTRY, false))
            {
                if (key != null)
                {
                    return (int)key.GetValue(VALUE_BING_SEARCH_ENABLED) == 1;
                }
            }
            return true;
        }

        public static string GetCurrentUserSearchAppDataDirectory()
        {
            return $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Packages\{SEARCH_APP_NAME}";
        }

        public static void ClearIconCache()
        {
            string ICON_CACHE_DIR = $@"{GetCurrentUserSearchAppDataDirectory()}\LocalState\AppIconCache";
            if (Directory.Exists(ICON_CACHE_DIR))
            {
                Directory.Delete(ICON_CACHE_DIR, true);
            }
        }

        public static string GetSearchBoxText()
        {
            using (var key = Utility.OpenCurrentUserRegistryKey(SEARCH_APP_REGISTRY + @"Flighting\Override", false))
            {
                return key == null ? "" : (string)key.GetValue(VALUE_SEARCH_BOX_TEXT_REGISTRY, "");
            }
        }
        public static void SetSearchBoxText(string text)
        {
            using (var key = Utility.OpenCurrentUserRegistryKey(SEARCH_APP_REGISTRY + @"Flighting\Override", true))
            {
                if (key != null)
                {
                    if (text != null)
                    {
                        key.SetValue(VALUE_SEARCH_BOX_TEXT_REGISTRY, text, RegistryValueKind.String);
                    }
                    else if (key.GetValue(VALUE_SEARCH_BOX_TEXT_REGISTRY) != null)
                    {
                        key.DeleteValue(VALUE_SEARCH_BOX_TEXT_REGISTRY);
                    }
                    key.Close();
                }
            }
        }

        public static bool IsSearchBoxVisible()
        {
            using (RegistryKey key = Utility.OpenCurrentUserRegistryKey(SEARCH_APP_REGISTRY, true))
            {
                if (key == null) return false;
                object value = key.GetValue(VALUE_SEARCH_BOX_TASKBAR_MODE_REGISTRY);
                return value == null ? false : value.ToString() == "2";
            }
        }
        public static void SetSearchBoxVisibility(bool visible)
        {
            using (RegistryKey key = Utility.OpenCurrentUserRegistryKey(SEARCH_APP_REGISTRY, true))
            {
                if (key == null) return;
                key.SetValue(VALUE_SEARCH_BOX_TASKBAR_MODE_REGISTRY, visible ? 2 : 1, RegistryValueKind.DWord);
            }
        }

        public static void ShowSearchWindow()
        {
            KeyboardSend.KeyDown(Keys.LWin);
            KeyboardSend.KeyDown(Keys.S);
            KeyboardSend.KeyUp(Keys.LWin);
            KeyboardSend.KeyUp(Keys.S);
        }

        public static void HideSearchWindow()
        {
            KeyboardSend.KeyDown(Keys.Escape);
            KeyboardSend.KeyUp(Keys.Escape);

            NativeHelper.SetForegroundWindow(NativeHelper.FindWindow("Shell_TrayWnd", null));
        }
    }
}
