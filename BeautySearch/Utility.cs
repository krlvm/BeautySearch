using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
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
        public static int GetBuildMinorVersion()
        {
            return int.Parse(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR", "0").ToString());
        }

        public static string InsertAfter(string target, string hook, string s)
        {
            return target.Insert(target.IndexOf(hook) + hook.Length, s);
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
        
        public static bool CheckIfMachineHasKey(string key)
        {
            return Registry.LocalMachine.OpenSubKey(key, true) != null;
        }

        public static bool CheckIfCurrentUserHasKey(string key)
        {
            return Registry.Users.OpenSubKey(ScriptInstaller.SID + "\\" + key, false) != null;
        }

        public static void DeleteCurrentUserSubKeyTree(string key)
        {
            Registry.Users.DeleteSubKeyTree(ScriptInstaller.SID + "\\" + key);
        }

        public static RegistryKey OpenCurrentUserRegistryKey(string key, bool writable)
        {
            string path = ScriptInstaller.SID + "\\" + key;
            RegistryKey hKey = Registry.Users.OpenSubKey(path, writable);
            return hKey != null ? hKey : Registry.Users.CreateSubKey(path, writable);
        }

        public static int GetDPIScaling()
        {
            using (RegistryKey key = OpenCurrentUserRegistryKey(@"Control Panel\Desktop\", false))
            {
                return key == null ? 0 : (int)key.GetValue("LogPixels", 0);
            }
        }

        public static bool IsPersonalizationFeatureEnabled(string feature)
        {
            using (RegistryKey key = OpenCurrentUserRegistryKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize\", false))
            {
                return key == null ? false : ((int)key.GetValue(feature, 0)) == 1;
            }
        }

        public static int GetWallpaperStyle()
        {
            using (RegistryKey key = OpenCurrentUserRegistryKey(@"Control Panel\Desktop", false))
            {
                return Int32.Parse(key == null ? "10" : ((string)key.GetValue("WallpaperStyle", "10")));
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

        public static void ExecuteCommand(string exec, string args)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = exec;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
            proc.WaitForExit();
        }

        public static string GetUsername()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
            ManagementObjectCollection collection = searcher.Get();
            return (string)collection.Cast<ManagementBaseObject>().First()["UserName"];
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
                ExecuteCommand("takeown.exe", "/F \"" + filepath + "\"");
                ExecuteCommand("icacls.exe", "\"" + filepath + "\" /grant " + Environment.UserName + ":F");
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

        public enum TaskbarSide { TOP, BOTTOM, LEFT, RIGHT }

        public static TaskbarSide GetTaskbarSide()
        {
            TaskbarSide side = TaskbarSide.BOTTOM;
            if (Screen.PrimaryScreen.WorkingArea.Width == Screen.PrimaryScreen.Bounds.Width)
            {
                if (Screen.PrimaryScreen.WorkingArea.Top > 0) side = TaskbarSide.TOP;
            }
            else
            {
                side = Screen.PrimaryScreen.WorkingArea.Left > 0 ? TaskbarSide.LEFT : TaskbarSide.RIGHT;
            }
            return side;
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

    static class NativeHelper
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }

    // https://social.msdn.microsoft.com/Forums/sqlserver/en-US/f2d88949-2de7-451a-be47-a7372ce457ff/send-windows-key?forum=csharpgeneral
    static class KeyboardSend
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private const int KEYEVENTF_EXTENDEDKEY = 1;
        private const int KEYEVENTF_KEYUP = 2;

        public static void KeyDown(Keys vKey)
        {
            keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY, 0);
        }

        public static void KeyUp(Keys vKey)
        {
            keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }
    }
}
