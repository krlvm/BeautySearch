using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;

namespace BeautySearch
{
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
            if (true)
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
