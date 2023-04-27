using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace BeautySearch.Installer
{
    static class ScriptInstaller
    {
        private const int MIN_REQUIRED_BUILD = OSBuild.V19H1;
        private const int MIN_20H1_PATCH_FIX = 1618;

        ///////////////////////////////////////////////
        public const int ERR_READ = 1;
        public const int ERR_WRITE = 2;
        public const int ERR_NOT_INSTALLED = 3;
        public const int ERR_KILL_FAILED = 4;
        public const int ERR_OLD_BUILD = 5;

        ///////////////////////////////////////////////
        private static string TARGET_DIR;
        private static string TARGET_FILE;
        private static string SCRIPT_DEST;
        ///////////////////////////////////////////////

        private static string INJECT_LINE;

        static ScriptInstaller()
        {
            if (SystemInfo.BUILD_NUMBER >= OSBuild.V11_21H2)
            {
                TARGET_DIR = $@"{Path.GetPathRoot(Environment.SystemDirectory)}Windows\SystemApps\MicrosoftWindows.Client.CBS_cw5n1h2txyewy\Cortana.UI\cache\SVLocal\Desktop";
                INJECT_LINE = "<script type=\"text/javascript\" src=\"ms-appx-web:///Cortana.UI/cache/svlocal/desktop/BeautySearch.js\"></script>";
            }
            else
            {
                TARGET_DIR = $@"{Path.GetPathRoot(Environment.SystemDirectory)}Windows\SystemApps\{SearchAppManager.SEARCH_APP_NAME}\cache\Local\Desktop";
                INJECT_LINE = "<script type=\"text/javascript\" src=\"ms-appx-web:///cache/local/Desktop/BeautySearch.js\"></script>";
            }

            TARGET_FILE = TARGET_DIR + @"\2.html";
            SCRIPT_DEST = TARGET_DIR + @"\BeautySearch.js";
        }

        ///////////////////////////////////////////////
        public static readonly bool IS_20H1 = SystemInfo.BUILD_NUMBER >= OSBuild.V20H1 && SystemInfo.BUILD_NUMBER < OSBuild.V_POST_20H1;
        public static readonly bool IS_20H1_PATCHED = SystemInfo.BUILD_NUMBER >= OSBuild.V20H1 && SystemInfo.BUILD_NUMBER < OSBuild.V_POST_20H1 && SystemInfo.BUILD_NUMBER_MINOR >= MIN_20H1_PATCH_FIX;
        public static readonly bool ACRYLIC_SUPPORTED = !IS_20H1 || IS_20H1_PATCHED;
        public static readonly bool IS_MODERN = IS_20H1_PATCHED || SystemInfo.BUILD_NUMBER >= OSBuild.V_POST_20H1; 

        ///////////////////////////////////////////////
        private const int TIMEOUT_20H1F_DEFAULT = 2500;
        private const int TIMEOUT_20H1F_CUSTOM  = 200;
        private static string Get20H1FTimeout(int timeout)
        {
            return timeout + ",\"ShowWebViewTimer\"";
        }
        ///////////////////////////////////////////////

        public static int Install(FeatureControl features)
        {
            if (SystemInfo.BUILD_NUMBER < MIN_REQUIRED_BUILD)
            {
                return ERR_OLD_BUILD;
            }
            if (IS_MODERN)
            {
                features.Enable("version2022");
            }

            if ("fake".Equals(features.Get("acrylicMode")))
            {
                if (SystemInfo.BUILD_NUMBER < OSBuild.V20H1 || SystemInfo.BUILD_NUMBER >= 19541)
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

            SearchAppManager.SetBingSearchEnabled(false);
            SearchAppManager.KillSearchApp();

            //Utility.TakeOwnership(TARGET_DIR);
            Utility.TakeOwnership(TARGET_FILE);
            //Utility.TakeOwnership(SCRIPT_DEST);

            // Read and modify HTML entry point

            string target = File.ReadAllText(TARGET_FILE);
            if (target == null)
            {
                return ERR_READ;
            }

            if (!target.Contains(INJECT_LINE))
            {
                target += INJECT_LINE;
            }

            if (IS_MODERN)
            {
                if (features.IsEnabled("speedLoad"))
                {
                    target = target.Replace(Get20H1FTimeout(TIMEOUT_20H1F_DEFAULT), Get20H1FTimeout(TIMEOUT_20H1F_CUSTOM));
                } 
                else
                {
                    target = target.Replace(Get20H1FTimeout(TIMEOUT_20H1F_CUSTOM), Get20H1FTimeout(TIMEOUT_20H1F_DEFAULT));
                }

                if (!features.IsEnabled("backgroundMode"))
                {
                    features.Set("backgroundMode", "dark2022");
                }
                else
                {
                    features.Exclude("backgroundMode");
                }
            }

            features.Set("activityItemCount", features.IsEnabled("adaptiveActivityCount") ? "-1" : "8");

            if (!Utility.WriteToFile(TARGET_FILE, target))
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

            string script = LoadScript("BeautySearch" + (SystemInfo.BUILD_NUMBER >= OSBuild.V11_21H2 ? "11" : ""));
            script = script.Replace("const SETTINGS = SETTINGS_DEFAULTS;", features.Build());
            if (globalInstall)
            {
                Utility.WriteToFile(SCRIPT_DEST, script);
            }
            else
            {
                Utility.WriteToFile(SCRIPT_DEST, LoadScript("BeautySearchLoader"));
                Utility.WriteToFile(TARGET_DIR + $"\\{SystemInfo.SID}.js", script);

                Utility.RestoreTrustedInstallerOwnership(TARGET_DIR + $"\\{SystemInfo.SID}.js");
            }

            Utility.RestoreTrustedInstallerOwnership(TARGET_FILE);
            Utility.RestoreTrustedInstallerOwnership(SCRIPT_DEST);

            if (!SearchAppManager.KillSearchApp())
            {
                return ERR_KILL_FAILED;
            }

            return 0;
        }

        public static int Uninstall(bool isSilent)
        {
            if (!isSilent)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Online Bing Search was disabled after BeautySearch installation.\nEnable it again?",
                    "BeautySearch",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (dialogResult == DialogResult.Yes)
                {
                    SearchAppManager.SetBingSearchEnabled(true);
                }
            }

            SearchAppManager.KillSearchApp();

            if (!IsInstalled())
            {
                return ERR_NOT_INSTALLED;
            }

            Utility.TakeOwnership(SCRIPT_DEST);
            File.Delete(SCRIPT_DEST);

            string target = File.ReadAllText(TARGET_FILE);
            if (target == null)
            {
                return ERR_READ;
            }

            target = target.Replace(INJECT_LINE, "");

            if (IS_MODERN)
            {
                target = target.Replace(Get20H1FTimeout(TIMEOUT_20H1F_CUSTOM), Get20H1FTimeout(TIMEOUT_20H1F_DEFAULT));
            }

            Utility.TakeOwnership(TARGET_FILE);
            if (!Utility.WriteToFile(TARGET_FILE, target))
            {
                return ERR_WRITE;
            }
            Utility.RestoreTrustedInstallerOwnership(TARGET_FILE);

            if (!SearchAppManager.KillSearchApp())
            {
                return ERR_KILL_FAILED;
            }

            return 0;
        }

        public static bool IsInstalled()
        {
            return File.Exists(SCRIPT_DEST);
        }

        private static string LoadScript(string name)
        {
#if DEBUG
            return File.ReadAllText(Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\..\\Scripts\\" + $"{name}.js");
#else
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream($"BeautySearch.Scripts.{name}.js"))
            {
                using (var reader = new StreamReader(stream))
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
            string controller = File.ReadAllText(CONTROLLER_FILE);

            if (controller.Contains("bsController"))
            {
                return true;
            }    

            // Controller is accessible via 'bsController' global variable
            controller = "var bsController = null;" + controller;
            if (IS_20H1 && SystemInfo.BUILD_NUMBER_MINOR < MIN_20H1_PATCH_FIX)
            {
                controller = controller.Insert(controller.IndexOf("}return l.prototype"), ";bsController=l;");
            }
            else
            {
                controller = controller.Insert(controller.IndexOf("return l.isBingWallpaperAppInstalled"), "bsController=l;");
            }

            Utility.TakeOwnership(CONTROLLER_FILE);
            bool success = Utility.WriteToFile(CONTROLLER_FILE, controller);
            Utility.RestoreTrustedInstallerOwnership(CONTROLLER_FILE);

            return success;
        }

        public static string FindControllerFile()
        {
            // *laughs*
            foreach (var file in Directory.EnumerateFiles(TARGET_DIR, "*.js"))
            {
                string text = File.ReadAllText(file);
                if (text.Contains("return l.prototype") || text.Contains("return l.isBingWallpaperAppInstalled")) return file;
            }
            return null;
        }

        private static string ToggleEntrypointFeature(string raw, string feature, bool isEnabled)
        {
            return isEnabled ? raw.Replace("\"" + feature + "\":0", "\"" + feature + "\":1")
                : raw.Replace("\"" + feature + "\":1", "\"" + feature + "\":0");
        }
    }
}
