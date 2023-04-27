using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace BeautySearch
{
    static class Utility
    {
        public static string InsertAfter(string s, string target, string insertion)
        {
            return s.Insert(s.IndexOf(target) + target.Length, insertion);
        }

        public static bool WriteToFile(string filepath, string text)
        {
            using (var writer = new StreamWriter(filepath))
            {
                writer.Write(text);
            }
            return true;
        }

        public static void ExecuteCommand(string exec, string args)
        {
            var process = Process.Start(new ProcessStartInfo()
            {
                FileName = exec,
                Arguments = args,
                WindowStyle = ProcessWindowStyle.Hidden
            });
            process.WaitForExit();
        }

        #region Administrator Rights

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
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

        public static bool RunElevated(string args, out int exitCode)
        {
            try
            {
                var process = Process.Start(new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName)
                {
                    UseShellExecute = true,
                    Verb = "runas",
                    Arguments = args
                });
                process.WaitForExit();
                exitCode = process.ExitCode;
                return true;
            }
            catch
            {
                exitCode = -1;
                return false;
            }
        }

        #endregion

        #region File Permissions

        public static void TakeOwnership(string filepath)
        {
            if (!File.Exists(filepath))
            {
                return;
            }
            try
            {
                GainFullAccess(filepath);
            }
            catch
            {
                ExecuteCommand("takeown.exe", $"/F \"{filepath}\"");
                ExecuteCommand("icacls.exe", $"\"{filepath}\" /grant {Environment.UserName}:F");
            }
        }

        private static void GainFullAccess(string filepath)
        {
            var user = WindowsIdentity.GetCurrent().User;

            var security = File.GetAccessControl(filepath);
            security.SetOwner(user);
            security.SetAccessRule(new FileSystemAccessRule(user, FileSystemRights.FullControl, AccessControlType.Allow));

            File.SetAccessControl(filepath, security);
        }

        public static void RestoreTrustedInstallerOwnership(string filepath)
        {
            ExecuteCommand("icacls.exe", $"\"{filepath}\" /setowner \"NT SERVICE\\TrustedInstaller\"");
            ExecuteCommand("icacls.exe", $"\"{filepath}\" /remove {Environment.UserName}:F");
            ExecuteCommand("icacls.exe", $"\"{filepath}\" /t /q /c /reset");
        }

        #endregion

        #region Registry

        public static bool CheckIfMachineHasKey(string key)
        {
            return Registry.LocalMachine.OpenSubKey(key, false) != null;
        }

        public static bool CheckIfCurrentUserHasKey(string key)
        {
            return Registry.Users.OpenSubKey(SystemInfo.SID + "\\" + key, false) != null;
        }

        public static void DeleteCurrentUserSubKeyTree(string key)
        {
            Registry.Users.DeleteSubKeyTree(SystemInfo.SID + "\\" + key);
        }

        public static RegistryKey OpenCurrentUserRegistryKey(string key, bool writable)
        {
            string path = SystemInfo.SID + "\\" + key;
            var hKey = Registry.Users.OpenSubKey(path, writable);
            return hKey != null ? hKey : Registry.Users.CreateSubKey(path, writable);
        }

        #endregion

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

        public static string GetUsername()
        {
            return NativeHelper.GetUsername();
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

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentProcess();
        public const UInt32 TOKEN_QUERY = 0x0008;
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);
        public const UInt32 TokenUser = 1;
        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool GetTokenInformation(IntPtr TokenHandle, UInt32 TokenInformationClass, IntPtr TokenInformation, uint TokenInformationLength, out uint ReturnLength);
        public struct TOKEN_USER
        {
            public SID_AND_ATTRIBUTES User;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SID_AND_ATTRIBUTES
        {

            public IntPtr Sid;
            public int Attributes;
        }
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool LookupAccountSid(string lpSystemName, IntPtr Sid, StringBuilder lpName, ref uint cchName, StringBuilder ReferencedDomainName, ref uint cchReferencedDomainName, out uint peUse);

        #region
        public static string GetUsername()
        {
            string rv = null;
            var hProcess = GetCurrentProcess();
            if (hProcess != IntPtr.Zero)
            {
                IntPtr hToken = IntPtr.Zero;
                if (OpenProcessToken(hProcess, TOKEN_QUERY, out hToken) && hToken != IntPtr.Zero)
                {
                    uint lenTokenInfo = 0;
                    GetTokenInformation(hToken, TokenUser, IntPtr.Zero, lenTokenInfo, out lenTokenInfo);
                    if (lenTokenInfo > 0)
                    {
                        IntPtr pTokenInfo = Marshal.AllocHGlobal((int)lenTokenInfo);
                        if (pTokenInfo != IntPtr.Zero)
                        {
                            if (GetTokenInformation(hToken, TokenUser, pTokenInfo, lenTokenInfo, out lenTokenInfo) && lenTokenInfo > 0)
                            {
                                TOKEN_USER TokenUser = (TOKEN_USER)Marshal.PtrToStructure(pTokenInfo, typeof(TOKEN_USER));
                                if (TokenUser.User.Sid != IntPtr.Zero)
                                {
                                    uint peUse = 0;
                                    uint cchName = 0;
                                    uint cchReferencedDomainName = 0;
                                    LookupAccountSid(null, TokenUser.User.Sid, null, ref cchName, null, ref cchReferencedDomainName, out peUse);
                                    if (cchName > 0 && cchReferencedDomainName > 0)
                                    {
                                        StringBuilder strName = new StringBuilder((int)cchName);
                                        StringBuilder strReferencedDomainName = new StringBuilder((int)cchReferencedDomainName);
                                        if (LookupAccountSid(null, TokenUser.User.Sid, strName, ref cchName, strReferencedDomainName, ref cchReferencedDomainName, out peUse) && cchName > 0 && cchReferencedDomainName > 0)
                                        {
                                            rv = strReferencedDomainName + "\\\\" + strName;
                                        }
                                    }
                                }
                            }
                            Marshal.FreeHGlobal(pTokenInfo);
                        }
                    }
                    CloseHandle(hToken);
                }
            }
            if (rv == null)
            {
                throw new Exception("Failed to get your account's user name.");
            }
            return rv;
        }
        #endregion
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
