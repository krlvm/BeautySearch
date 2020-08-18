using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace BeautySearch
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new InstallationForm());
        }
    }

    public class FeatureControl
    {
        private StringBuilder sb = new StringBuilder("const SETTINGS = {");
        private bool writeable = true;

        public void Enable(string feature)
        {
            if(!writeable)
            {
                throw new InvalidOperationException("FeatureControl is not writeable");
            }
            sb.Append(feature + ":true,");
        }

        public string Build()
        {
            writeable = false;
            sb.Append("};");
            return sb.ToString();
        }

        override public string ToString()
        {
            return Build();
        }
    }

    static class ScriptInstaller
    {
        public static int CURRENT_BUILD;

        private const int BUILD_MAY_2019 = 18362;
        private const int BUILD_MAY_2020 = 19041;

        private const int MIN_REQUIRED_BUILD = BUILD_MAY_2019;

        // Error codes
        public const int ERR_READ          = 1;
        public const int ERR_WRITE         = 2;
        public const int ERR_NOT_INSTALLED = 3;
        public const int ERR_KILL_FAILED   = 4;
        public const int ERR_OLD_BUILD     = 5;

        // Bing Search
        private const string BING_SEARCH_REGISTRY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Search\";
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
            if(CURRENT_BUILD >= BUILD_MAY_2020)
            {
                SEARCH_APP_EXEC = "SearchApp";
                SEARCH_APP_NAME = "Microsoft.Windows.Search_cw5n1h2txyewy";
            }
            else if(CURRENT_BUILD >= BUILD_MAY_2019)
            {
                SEARCH_APP_EXEC = "SearchUI";
                SEARCH_APP_NAME = "Microsoft.Windows.Cortana_cw5n1h2txyewy";
            }

            TARGET_DIR = Path.GetPathRoot(Environment.SystemDirectory) + @"Windows\SystemApps\" + SEARCH_APP_NAME + @"\cache\Local\Desktop";
            TARGET_FILE = TARGET_DIR + @"\2.html";
            SCRIPT_DEST = TARGET_DIR + @"\BeautySearch.js";
        }

        public static int Install(FeatureControl features)
        {
            if (CURRENT_BUILD < MIN_REQUIRED_BUILD)
            {
                return ERR_OLD_BUILD;
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

            string script = LoadScript();
            script = script.Replace("const SETTINGS = SETTINGS_DEFAULTS;", features.Build());
            Utility.WriteFile(SCRIPT_DEST, script);

            if(!KillSearchApp())
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
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(BING_SEARCH_REGISTRY, true))
            {
                if (key != null)  //must check for null key
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
                    Process.Start("taskkill.exe", "/F /IM " + SEARCH_APP_EXEC + ".exe");
                }
            }
            return success;
        }

        public static bool IsInstalled()
        {
            return File.Exists(SCRIPT_DEST);
        }

        private static string LoadScript()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            const string NAME = "BeautySearch.BeautySearch.js";

            using (Stream stream = assembly.GetManifestResourceStream(NAME))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }

    static class Utility
    {
        public static int GetBuildNumber()
        {
            return int.Parse(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuildNumber", "0").ToString());
        }

        public static string ReadFile(string filepath)
        {
            return File.ReadAllText(filepath);
        }

        public static bool WriteFile(string filepath, string text)
        {
            if(true)
            {
                StreamWriter sw = new StreamWriter(filepath);
                sw.WriteLine(text);
                sw.Close();
                return true;
            }
            try
            {
                StreamWriter sw = new StreamWriter(filepath);
                sw.WriteLine(text);
                sw.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool RequireAdministrator()
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("Please, restart this application as administrator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            return false;
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void TakeOwnership(string filepath)
        {
            if (!File.Exists(filepath))
            {
                return;
            }
            try
            {
                GetFullAccess(filepath);
            }
            catch
            {
                Process.Start("takeown.exe", "/F \"" + filepath + "\"");
                Process.Start("icacls.exe", "\"" + filepath + "\" /grant " + Environment.UserName + ":F");
            }
        }
        private static void GetFullAccess(string filepath)
        {
            FileSecurity security = File.GetAccessControl(filepath);
            SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
            security.SetOwner(user);
            security.SetAccessRule(new FileSystemAccessRule(user, FileSystemRights.FullControl, AccessControlType.Allow));
            File.SetAccessControl(filepath, security);
        }
    }
}
