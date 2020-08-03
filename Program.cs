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

        private const string TARGET_FILE = @"C:\Windows\SystemApps\Microsoft.Windows.Search_cw5n1h2txyewy\cache\Local\Desktop\2.html";
        private const string SCRIPT_DEST = @"C:\Windows\SystemApps\Microsoft.Windows.Search_cw5n1h2txyewy\cache\Local\Desktop\BeautySearch.js";

        private const string INJECT_LINE = "<script type=\"text/javascript\" src=\"ms-appx-web:///cache/local/Desktop/BeautySearch.js\"></script>";

        public static int Install(FeatureControl features)
        {
            KillSearchApp();

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

            KillSearchApp();

            return 0;
        }

        public static int Uninstall()
        {
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

            KillSearchApp();

            return 0;
        }

        public static void KillSearchApp()
        {
            foreach (var process in Process.GetProcessesByName("SearchApp"))
            {
                process.Kill();
            }
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
