using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using static BeautySearch.Utility;
using static BeautySearch.NativeHelper;

namespace BeautySearch
{
    static class FakeBackgroundAcrylic
    {
        public static void CaptureWallpaper(string directory)
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Rectangle wkArea = Screen.PrimaryScreen.WorkingArea;

            // Minimizing all windows to take a good screenshot
            // and measuring Search Window size
            MinimizeAllWindows();
            Utility.ShowSearchWindow();
            System.Threading.Thread.Sleep(1000);
            RECT wnd;
            GetWindowRect(GetForegroundWindow(), out wnd);
            Utility.HideSearchWindow();
            System.Threading.Thread.Sleep(1000);

            int wndWidth = wnd.Right - wnd.Left;
            int wndHeight = wnd.Bottom - wnd.Top;

            bool searchBoxVisible = IsSearchBoxVisible();
            TaskbarSide side = GetTaskbarSide();
            if(side == TaskbarSide.BOTTOM)
            {
                if(!IsSearchBoxVisible())
                {
                    wndHeight -= bounds.Height - wkArea.Height; // -taskbarHeight
                }
            }

            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }

                using (Bitmap screen = bitmap.Clone(new Rectangle(wnd.Left, wnd.Top, wndWidth, wndHeight), bitmap.PixelFormat))
                {
                    screen.Save(directory + @"\" + ScriptInstaller.SID + ".png", ImageFormat.Png);
                }
            }
        }

        static void MinimizeAllWindows()
        {
            KeyboardSend.KeyDown(Keys.LWin);
            KeyboardSend.KeyDown(Keys.M);
            KeyboardSend.KeyUp(Keys.LWin);
            KeyboardSend.KeyUp(Keys.M);
        }

        static bool IsSearchBoxVisible()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(ScriptInstaller.SEARCH_APP_REGISTRY, true))
            {
                if (key == null) return false;
                return key.GetValue("SearchboxTaskbarMode").ToString() == "2";
            }
        }
    }
}
