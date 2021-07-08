using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
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
        }
    }

    static class NativeHelper
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

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
