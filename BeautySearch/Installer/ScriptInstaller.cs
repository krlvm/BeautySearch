using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace BeautySearch
{
    static class ScriptInstaller
    {
        public static int CURRENT_BUILD;
        public static int CURRENT_BUILD_PATCH;

        public const int BUILD_19H1 = 18362;
        public const int BUILD_19H2 = 18363;
        public const int BUILD_20H1 = 19041;

        private const int MIN_REQUIRED_BUILD = BUILD_19H1;
        public const int MIN_20H1_PATCH_FIX = 1618;

        // Error codes
        public const int ERR_READ = 1;
        public const int ERR_WRITE = 2;
        public const int ERR_NOT_INSTALLED = 3;
        public const int ERR_KILL_FAILED = 4;
        public const int ERR_OLD_BUILD = 5;

        // Bing Search
        public const string SEARCH_APP_REGISTRY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Search\";
        private const int BING_SEARCH_DISABLED = 0;
        private const int BING_SEARCH_ENABLED = 1;

        private static string SEARCH_APP_NAME;
        private static string SEARCH_APP_EXEC;

        private static string TARGET_DIR;
        private static string TARGET_FILE;
        private static string SCRIPT_DEST;

        private const string INJECT_LINE = "<script type=\"text/javascript\" src=\"ms-appx-web:///cache/local/Desktop/BeautySearch.js\"></script>";

        static ScriptInstaller()
        {
            CURRENT_BUILD = Utility.GetBuildNumber();
            CURRENT_BUILD_PATCH = Utility.GetBuildMinorVersion();
            if (CURRENT_BUILD >= BUILD_20H1)
            {
                SEARCH_APP_EXEC = "SearchApp";
                SEARCH_APP_NAME = "Microsoft.Windows.Search_cw5n1h2txyewy";
            }
            else if (CURRENT_BUILD >= BUILD_19H1)
            {
                SEARCH_APP_EXEC = "SearchUI";
                SEARCH_APP_NAME = "Microsoft.Windows.Cortana_cw5n1h2txyewy";
            }

            TARGET_DIR = $@"{Path.GetPathRoot(Environment.SystemDirectory)}Windows\SystemApps\{SEARCH_APP_NAME}\cache\Local\Desktop";
            TARGET_FILE = TARGET_DIR + @"\2.html";
            SCRIPT_DEST = TARGET_DIR + @"\BeautySearch.js";
        }

        public static bool is20H1()
        {
            return CURRENT_BUILD >= BUILD_20H1 && CURRENT_BUILD < 19500;
        }
        public static bool is20H1Fixed()
        {
            return CURRENT_BUILD < BUILD_20H1 || CURRENT_BUILD >= 19500 || CURRENT_BUILD_PATCH >= MIN_20H1_PATCH_FIX;
        }

        private const int TIMEOUT_20H1F_DEFAULT = 2500;
        private const int TIMEOUT_20H1F_CUSTOM  = 200;
        private static string Get20H1FTimeout(int timeout)
        {
            return timeout + ",\"ShowWebViewTimer\"";
        }

        public static int Install(FeatureControl features)
        {
            if (CURRENT_BUILD < MIN_REQUIRED_BUILD)
            {
                return ERR_OLD_BUILD;
            }
            if (is20H1Fixed())
            {
                features.Enable("version2022");
            }

            if ("fake".Equals(features.Get("acrylicMode")))
            {
                if (CURRENT_BUILD < BUILD_20H1 || CURRENT_BUILD >= 19541)
                {
                    MessageBox.Show(
                        "Fake Acrylic is available only on 20H1 and higher because native Search Window acrylic was broken. Use Default mode instead.",
                        "BeautySearch",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return ERR_OLD_BUILD;
                }
                if (!features.IsEnabled("skipFakeAcrylic") || !File.Exists(FakeBackgroundAcrylic.GetCroppedWallpaperPath(TARGET_DIR)))
                {
                    if (features.IsEnabled("fakeAcrylicDesktopCrop"))
                    {
                        FakeBackgroundAcrylic.CaptureRealDesktop(TARGET_DIR);
                    }
                    else
                    {
                        FakeBackgroundAcrylic.CaptureWallpaper(TARGET_DIR);
                    }
                }
            }
            if (!features.IsEnabled("contextMenu19H1"))
            {
                features.Enable("contextMenuLightI");
            }
            features.Exclude("contextMenu19H1");

            SetBingSearchEnabled(BING_SEARCH_DISABLED);
            KillSearchApp();

            //Utility.TakeOwnership(TARGET_DIR);
            Utility.TakeOwnership(TARGET_FILE);
            //Utility.TakeOwnership(SCRIPT_DEST);

            // Read and modify HTML entry point

            string target = Utility.ReadFile(TARGET_FILE);
            if (target == null)
            {
                return ERR_READ;
            }

            if (!target.Contains(INJECT_LINE))
            {
                target += INJECT_LINE;
            }

            if (is20H1Fixed())
            {
                if (features.IsEnabled("speedLoad"))
                {
                    target = target.Replace(Get20H1FTimeout(TIMEOUT_20H1F_DEFAULT), Get20H1FTimeout(TIMEOUT_20H1F_CUSTOM));
                } 
                else
                {
                    target = target.Replace(Get20H1FTimeout(TIMEOUT_20H1F_CUSTOM), Get20H1FTimeout(TIMEOUT_20H1F_DEFAULT));
                }
            }

            //target = ToggleEntrypointFeature(target, "enableTwoPanesZI", !features.IsEnabled("disableTwoPanel"));
            //target = ToggleEntrypointFeature(target, "userProfileButtonEnabled", !features.IsEnabled("hideUserProfile"));
            //target = ToggleEntrypointFeature(target, "showCloseButton", !features.IsEnabled("hideCloseButton"));
            //
            //if (features.IsEnabled("limitActivity"))
            //{
            //    target = target.Replace("\"activityInZI\":9", "\"activityInZI\":4");
            //} else
            //{
            //    target = target.Replace("\"activityInZI\":4", "\"activityInZI\":9");
            //}
            features.Set("activityItemCount", features.IsEnabled("adaptiveActivityCount") ? "-1" : "8");

            if (!Utility.WriteFile(TARGET_FILE, target))
            {
                return ERR_WRITE;
            }

            if (features.IsEnabled("useController") && !InjectController())
            {
                features.Exclude("useController");
                MessageBox.Show(
                    "Failed to hook the Controller, try to reinstall BeautySearch",
                    "BeautySearch",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            bool globalInstall = features.IsEnabled("globalInstall");

            string script = LoadScript("BeautySearch");
            script = script.Replace("const SETTINGS = SETTINGS_DEFAULTS;", features.Build());
            if (globalInstall)
            {
                Utility.WriteFile(SCRIPT_DEST, script);
            }
            else
            {
                Utility.WriteFile(SCRIPT_DEST, LoadScript("BeautySearchLoader"));
                Utility.WriteFile(TARGET_DIR + $"\\{SID}.js", script);

                Utility.NormalizeOwnership(TARGET_DIR + $"\\{SID}.js");
            }

            Utility.NormalizeOwnership(TARGET_FILE);
            Utility.NormalizeOwnership(SCRIPT_DEST);

            if (!KillSearchApp())
            {
                return ERR_KILL_FAILED;
            }

            return 0;
        }

        public static int Uninstall()
        {
            DialogResult dialogResult = MessageBox.Show(
                "Online Bing Search was disabled after BeautySearch installation.\nEnable it again?",
                "BeautySearch",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (dialogResult == DialogResult.Yes)
            {
                SetBingSearchEnabled(BING_SEARCH_ENABLED);
            }

            KillSearchApp();

            if (!IsInstalled())
            {
                return ERR_NOT_INSTALLED;
            }

            Utility.TakeOwnership(SCRIPT_DEST);
            File.Delete(SCRIPT_DEST);

            string target = Utility.ReadFile(TARGET_FILE);
            if (target == null)
            {
                return ERR_READ;
            }

            target = target.Replace(INJECT_LINE, "");

            if (is20H1Fixed())
            {
                target = target.Replace(Get20H1FTimeout(TIMEOUT_20H1F_CUSTOM), Get20H1FTimeout(TIMEOUT_20H1F_DEFAULT));
            }

            Utility.TakeOwnership(TARGET_FILE);
            if (!Utility.WriteFile(TARGET_FILE, target))
            {
                return ERR_WRITE;
            }
            Utility.NormalizeOwnership(TARGET_FILE);

            if (!KillSearchApp())
            {
                return ERR_KILL_FAILED;
            }

            return 0;
        }

        private const string VALUE_BING_SEARCH_ENABLED = "BingSearchEnabled";
        public static void SetBingSearchEnabled(int val)
        {
            // When Bing Web Search is enabled, the Search App doesn't use the local search instance
            // 0 - disabled, 1 - enabled
            using (RegistryKey key = Utility.OpenCurrentUserRegistryKey(SEARCH_APP_REGISTRY, true))
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
            using (RegistryKey key = Utility.OpenCurrentUserRegistryKey(SEARCH_APP_REGISTRY, false))
            {
                if (key != null)
                {
                    return (int)key.GetValue(VALUE_BING_SEARCH_ENABLED) == 1;
                }
            }
            return true;
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

        public static bool IsInstalled()
        {
            return File.Exists(SCRIPT_DEST);
        }

        private static string LoadScript(string name)
        {
#if DEBUG
            return File.ReadAllText(Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\..\\" + $"{name}.js");
#else
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream($"BeautySearch.{name}.js"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
#endif
        }

        // Controller Injector

        private static bool InjectController()
        {
            string CONTROLLER_FILE = FindControllerFile();
            Utility.TakeOwnership(CONTROLLER_FILE);

            string controller = Utility.ReadFile(CONTROLLER_FILE);
            if (!controller.Contains("bsController"))
            {
                // Controller is accessible via 'bsController' global variable
                controller = "var bsController = null;" + controller;
                if (CURRENT_BUILD_PATCH < MIN_20H1_PATCH_FIX)
                {
                    controller = controller.Insert(controller.IndexOf("}return l.prototype"), ";bsController=l;");
                } else
                {
                    controller = controller.Insert(controller.IndexOf("return l.isBingWallpaperAppInstalled"), "bsController=this;");
                }
                return Utility.WriteFile(CONTROLLER_FILE, controller);
            }
            return true;
        }

        public static string GetCurrentUserSearchAppDataDirectory()
        {
            return $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Packages\{SEARCH_APP_NAME}";
        }

        public static string FindControllerFile()
        {
            // *laughs*
            foreach (var file in Directory.EnumerateFiles(TARGET_DIR, "*.js"))
            {
                string text = Utility.ReadFile(file);
                if (text.Contains("return l.prototype") || text.Contains("return l.isBingWallpaperAppInstalled")) return file;
            }
            return null;
        }

        private static string ToggleEntrypointFeature(string raw, string feature, bool isEnabled)
        {
            if (isEnabled)
            {
                return raw.Replace("\"" + feature + "\":0", "\"" + feature + "\":1");
            } else
            {
                return raw.Replace("\"" + feature + "\":1", "\"" + feature + "\":0");
            }
        }

        public static void ClearIconCache()
        {
            string ICON_CACHE_DIR = $@"{GetCurrentUserSearchAppDataDirectory()}\LocalState\AppIconCache";
            if (Directory.Exists(ICON_CACHE_DIR))
            {
                Directory.Delete(ICON_CACHE_DIR, true);
            }
        }

#region Search Box Text
        private const string SEARCH_BOX_TEXT_REGISTRY_VALUE = "SearchBoxText";
        private const string SEARCH_BOX_TASKBAR_MODE_REGISTRY_VALUE = "SearchboxTaskbarMode";

        public static string GetSearchBoxText()
        {
            using (RegistryKey key = Utility.OpenCurrentUserRegistryKey(SEARCH_APP_REGISTRY + @"Flighting\Override", false))
            {
                return key == null ? "" : (string)key.GetValue(SEARCH_BOX_TEXT_REGISTRY_VALUE, "");
            }
        }
        public static void SetSearchBoxText(string text)
        {
            using (RegistryKey key = Utility.OpenCurrentUserRegistryKey(SEARCH_APP_REGISTRY + @"Flighting\Override", true))
            {
                if (key != null)
                {
                    if (text != null)
                    {
                        key.SetValue(SEARCH_BOX_TEXT_REGISTRY_VALUE, text, RegistryValueKind.String);
                    }
                    else if (key.GetValue(SEARCH_BOX_TEXT_REGISTRY_VALUE) != null)
                    {
                        key.DeleteValue(SEARCH_BOX_TEXT_REGISTRY_VALUE);
                    }
                    key.Close();
                }
            }

            KillSearchApp();
        }

        public static bool IsSearchBoxVisible()
        {
            using (RegistryKey key = Utility.OpenCurrentUserRegistryKey(SEARCH_APP_REGISTRY, true))
            {
                if (key == null) return false;
                object value = key.GetValue(SEARCH_BOX_TASKBAR_MODE_REGISTRY_VALUE);
                return value == null ? false : value.ToString() == "2";
            }
        }
        public static void SetSearchBoxVisibility(bool visible)
        {
            using (RegistryKey key = Utility.OpenCurrentUserRegistryKey(SEARCH_APP_REGISTRY, true))
            {
                if (key == null) return;
                key.SetValue(SEARCH_BOX_TASKBAR_MODE_REGISTRY_VALUE, visible ? 2 : 1, RegistryValueKind.DWord);
            }
        }
#endregion


        // Multi User
        private static string _username;
        public static string username
        {
            get
            {
                if (_username == null)
                {
                    _username = Utility.GetUsername();
                }
                return _username;
            }
        }
        public static string localUsername
        {
            get
            {
                return username.Substring(username.IndexOf("\\"));
            }
        }

        private static string _sid;
        public  static string SID
        {
            get
            {
                if (_sid == null)
                {
                    _sid = (new System.Security.Principal.NTAccount(username)).Translate(typeof(System.Security.Principal.SecurityIdentifier)).Value.ToString();
                }
                return _sid;
            }
        }
    }
}
