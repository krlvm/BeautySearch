using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
using System;
using System.Linq;
using static BeautySearch.NativeHelper;

namespace BeautySearch
{
    static class FakeBackgroundAcrylic
    {
        public static void CaptureWallpaper(string directory)
        {
            string WALLPAPER_CACHE_DIR = $@"{Path.GetPathRoot(Environment.SystemDirectory)}Users\{ScriptInstaller.localUsername}\AppData\Roaming\Microsoft\Windows\Themes\CachedFiles";
            if(!Directory.Exists(WALLPAPER_CACHE_DIR))
            {
                CaptureRealDesktop(directory);
                return;
            }

            DirectoryInfo info = new DirectoryInfo(WALLPAPER_CACHE_DIR);
            FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();

            string source = null;
            if (files.Length == 1)
            {
                source = files[0].FullName;
            }
            else
            {
                int target;
                switch (Utility.GetWallpaperStyle())
                {
                    case 6:
                        target = 3;
                        break;
                    case 2:
                        target = 2;
                        break;
                    case 0:
                        target = -1;
                        break;
                    case 22:
                        target = 5;
                        break;
                    case 10:
                    default:
                        target = 4;
                        break;
                }

                foreach (var file in files)
                {
                    if (target != -1)
                    {
                        if (file.Name.EndsWith(target + ".jpg"))
                        {
                            source = file.FullName;
                        }
                    }
                    else
                    {
                        if (true)
                        {
                            CaptureRealDesktop(directory);
                            return;
                        }
                        if (file.Name.EndsWith("1.jpg") || file.Name.EndsWith("0.jpg"))
                        {
                            source = file.FullName;
                        }
                    }
                }
            }
            if (source == null)
            {
                if (files.Length == 1)
                {
                    source = files[0].FullName;
                } 
                else
                {
                    CaptureRealDesktop(directory);
                    return;
                }
            }

            CaptureWallpaperFragment(new Bitmap(source), false, new int[2] { 500, 0 }, directory);
        }

        public static void CaptureRealDesktop(string directory)
        {
            MessageBox.Show(
                "We need to take a screenshot of the area behind the Search Window for acrylic effect, so all windows will be minimized.\nDon't move the mouse pointer until we're done. Click \"OK\" to continue.",
                "BeautySearch",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            CaptureWallpaperFragment(null, true, new int[2] { 700, 900 }, directory);
        }

        public static void CaptureWallpaperFragment(Bitmap bitmap, bool minimizeWindows, int[] sleep, string directory)
        {
            if(minimizeWindows)
            {
                // Minimizing all windows to take a good screenshot
                // and measuring Search Window size
                MinimizeAllWindows();
            }

            Utility.ShowSearchWindow();
            System.Threading.Thread.Sleep(sleep[0]);
            RECT wnd;
            GetWindowRect(GetForegroundWindow(), out wnd);
            Utility.HideSearchWindow();
            System.Threading.Thread.Sleep(sleep[1]);

            int wndWidth = wnd.Right - wnd.Left;
            int wndHeight = wnd.Bottom - wnd.Top;

            if (bitmap == null)
            {
                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                bitmap = new Bitmap(bounds.Width, bounds.Height);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
            }

            using (Bitmap screen = bitmap.Clone(new Rectangle(wnd.Left, wnd.Top, wndWidth, wndHeight), bitmap.PixelFormat))
            {
                screen.Save(GetCroppedWallpaperPath(directory), ImageFormat.Png);
            }
        }



        static void MinimizeAllWindows()
        {
            KeyboardSend.KeyDown(Keys.LWin);
            KeyboardSend.KeyDown(Keys.M);
            KeyboardSend.KeyUp(Keys.LWin);
            KeyboardSend.KeyUp(Keys.M);
        }

        static bool IsbSearchBoxVisible()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(ScriptInstaller.SEARCH_APP_REGISTRY, true))
            {
                if (key == null) return false;
                object value = key.GetValue("SearchboxTaskbarMode");
                return value == null ? false : value.ToString() == "2";
            }
        }

        public static string GetCroppedWallpaperPath(string rootDirectory)
        {
            return rootDirectory + @"\" + ScriptInstaller.SID + ".png";
        }
    }
}
