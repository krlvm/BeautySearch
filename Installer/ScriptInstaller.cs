using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Windows.Forms;

namespace BeautySearch
{
    static class ScriptInstaller
    {
        public static int CURRENT_BUILD;

        public const  int BUILD_19H1 = 18362;
        public const  int BUILD_19H2 = 18363;
        public const  int BUILD_20H1 = 19041;

        private const int MIN_REQUIRED_BUILD = BUILD_19H1;

        // Error codes
        public const int ERR_READ = 1;
        public const int ERR_WRITE = 2;
        public const int ERR_NOT_INSTALLED = 3;
        public const int ERR_KILL_FAILED = 4;
        public const int ERR_OLD_BUILD = 5;

        // Bing Search
        public  const string SEARCH_APP_REGISTRY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Search\";
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

        public static int Install(FeatureControl features)
        {
            if (CURRENT_BUILD < MIN_REQUIRED_BUILD)
            {
                return ERR_OLD_BUILD;
            }

            if ("'fake'".Equals(features.Get("acrylicMode")))
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

            SetBingSearchEnabled(BING_SEARCH_DISABLED);
            KillSearchApp();

            //Utility.TakeOwnership(TARGET_DIR);
            Utility.TakeOwnership(TARGET_FILE);
            //Utility.TakeOwnership(SCRIPT_DEST);

            string target = Utility.ReadFile(TARGET_FILE);
            if (target == null)
            {
                return ERR_READ;
            }

            if (!target.Contains(INJECT_LINE))
            {
                target += INJECT_LINE;
            }
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

            string script = LoadScript("BeautySearch");
            script = script.Replace("const SETTINGS = SETTINGS_DEFAULTS;", features.Build());
            Utility.WriteFile(SCRIPT_DEST, LoadScript("BeautySearchLoader"));
            Utility.WriteFile(TARGET_DIR + @"\" + SID + ".js", script);

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

            File.Delete(SCRIPT_DEST);

            string target = Utility.ReadFile(TARGET_FILE);
            if (target == null)
            {
                return ERR_READ;
            }

            target = target.Replace(INJECT_LINE, "");

            if (!Utility.WriteFile(TARGET_FILE, target))
            {
                return ERR_WRITE;
            }

            if (!KillSearchApp())
            {
                return ERR_KILL_FAILED;
            }

            return 0;
        }

        public static void SetBingSearchEnabled(int val)
        {
            // When Bing Web Search is enabled, the Search App doesn't use the local search instance
            // 0 - disabled, 1 - enabled
            using (RegistryKey key = Utility.OpenCurrentUserRegistryKey(SEARCH_APP_REGISTRY, true))
            {
                if (key != null)
                {
                    key.SetValue("BingSearchEnabled", val, RegistryValueKind.DWord);
                    key.Close();
                }
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

        public static bool IsInstalled()
        {
            return File.Exists(SCRIPT_DEST);
        }

        private static string LoadScript(string name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream($"BeautySearch.{name}.js"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
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
                controller = controller.Insert(controller.IndexOf("}return l.prototype"), ";bsController=this;");
                return Utility.WriteFile(CONTROLLER_FILE, controller);
            }
            return true;
        }

        public static string FindControllerFile()
        {
            // *laughs*
            foreach (var file in Directory.EnumerateFiles(TARGET_DIR, "*.js"))
            {
                string text = Utility.ReadFile(file);
                if (text.Contains("return l.prototype")) return file;
            }
            return null;
        }

        public static void ClearIconCache()
        {
            string ICON_CACHE_DIR = $@"{Path.GetPathRoot(Environment.SystemDirectory)}Users\{localUsername}\AppData\Local\Packages\{SEARCH_APP_NAME}\LocalState\AppIconCache";
            if (Directory.Exists(ICON_CACHE_DIR))
            {
                Directory.Delete(ICON_CACHE_DIR, true);
            }
        }

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
