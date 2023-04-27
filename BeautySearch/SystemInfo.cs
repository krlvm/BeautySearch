using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeautySearch
{
    class SystemInfo
    {
        public static readonly int BUILD_NUMBER
            = int.Parse(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuildNumber", "0")
                .ToString());
        public static readonly int BUILD_NUMBER_MINOR
            = int.Parse(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR", "0").ToString());

        public enum TaskbarSide { TOP, BOTTOM, LEFT, RIGHT }

        public static TaskbarSide GetTaskbarSide()
        {
            var side = TaskbarSide.BOTTOM;
            if (Screen.PrimaryScreen.WorkingArea.Width == Screen.PrimaryScreen.Bounds.Width)
            {
                if (Screen.PrimaryScreen.WorkingArea.Top > 0)
                {
                    side = TaskbarSide.TOP;
                }
            }
            else
            {
                side = Screen.PrimaryScreen.WorkingArea.Left > 0 ? TaskbarSide.LEFT : TaskbarSide.RIGHT;
            }
            return side;
        }

        #region User / SID

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
        public static string SID
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

        #endregion
    }

    class OSBuild
    {
        public const int V19H1 = 18362;
        public const int V19H2 = 18363;
        public const int V20H1 = 19041;

        public const int V_POST_20H1 = 19500;

        public const int V11_21H2 = 22621;
        public const int V11_22H2 = 22621;
    }
}
