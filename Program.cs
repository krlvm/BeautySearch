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
                throw new System.InvalidOperationException("FeatureControl is not writeable");
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
        // Windows 10 May 2020 Update, 20H1
        public const int MIN_REQUIRED_BUILD = 19041;

        // Error codes
        public const int ERR_READ          = 1;
        public const int ERR_WRITE         = 2;
        public const int ERR_NOT_INSTALLED = 3;
        public const int ERR_KILL_FAILED   = 4;

        // Bing Search
        private const string BING_SEARCH_REGISTRY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Search";
        private const int BING_SEARCH_DISABLED = 0;
        private const int BING_SEARCH_ENABLED  = 1;

        private static string SEARCH_APP_NAME = "Microsoft.Windows.Search_cw5n1h2txyewy";

        private static string TARGET_DIR  = @"C:\Windows\SystemApps\" + SEARCH_APP_NAME + @"\cache\Local\Desktop";
        private static string TARGET_FILE = TARGET_DIR + @"\2.html";
        private static string SCRIPT_DEST = TARGET_DIR + @"\BeautySearch.js";

        private const string INJECT_LINE = "<script type=\"text/javascript\" src=\"ms-appx-web:///cache/local/Desktop/BeautySearch.js\"></script>";

        public static int Install(FeatureControl features)
        {
            SetBingSearchEnabled(BING_SEARCH_DISABLED);
            KillSearchApp();

            Utility.TakeOwnership(TARGET_DIR);
            Utility.TakeOwnership(TARGET_FILE);
            Utility.TakeOwnership(SCRIPT_DEST);

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
                }
            }
        }

        public static bool KillSearchApp()
        {
            bool success = true;
            foreach (var process in Process.GetProcessesByName("SearchApp"))
            {
                try
                {
                    process.Kill();
                } catch(Exception e)
                {
                    // We will try to kill the Search App using TASKKILL instead of asking to re-login
                    //success = false;
                    Process.Start("cmd.exe", "/c taskkill /F /IM SearchApp.exe");
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
        public static string ReadFile(string filepath)
        {
            return File.ReadAllText(filepath);
        }

        public static bool WriteFile(string filepath, string text)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filepath);
                sw.WriteLine(text);
                sw.Close();
                return true;
            }
            catch (Exception)
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
            FileSecurity fileS = File.GetAccessControl(filepath);
            SecurityIdentifier cu = WindowsIdentity.GetCurrent().User;
            fileS.SetOwner(cu);
            fileS.SetAccessRule(new FileSystemAccessRule(cu, FileSystemRights.FullControl, AccessControlType.Allow));
            File.SetAccessControl(filepath, fileS);
        }
    }
}
