using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BeautySearch.UI
{
    public partial class TopAppsEditorForm : Form
    {
        private readonly string file;

        public TopAppsEditorForm(string file, string data)
        {
            InitializeComponent();

            this.file = file;

            webBrowser.DocumentStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BeautySearch.TopAppsEditor.html");
            //webBrowser.Navigate(@"C:\Users\krlvm\Desktop\TopAppsEditor.html");
            webBrowser.DocumentCompleted += (sender, e) =>
            {
                webBrowser.ObjectForScripting = new JavaScriptCallbackProvider();
                webBrowser.Document.InvokeScript("importJson", new[] { data });
            };
        }

        [ComVisible(true)]
        public sealed class JavaScriptCallbackProvider
        {
            public string LoadIcon(string app, bool isUWP)
            {
                if (app.Equals("N/A")) return null;

                if (isUWP)
                {
                    app = ExtractNormalPath("@{" + app.Replace("\\", "/") + "}");
                    if (app.Trim().Length == 0)
                    {
                        return null;
                    }
                    try
                    {
                        using (var img = Image.FromFile(app))
                        {
                            return BitmapToBase64(img);
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                {
                    Icon icon;
                    if (app.StartsWith("{"))
                    {
                        string guid = app.Substring(1, app.LastIndexOf("}") - 1);

                        string replacement;
                        switch (guid)
                        {
                            case "F38BF404-1D43-42F2-9305-67DE0B28FC23":
                                replacement = Environment.ExpandEnvironmentVariables("%WinDir%");
                                break;
                            case "6D809377-6AF0-444B-8957-A3773F02200E":
                                replacement = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
                                break;
                            case "7C5A40EF-A0FB-4BFC-874A-C0F2E0B9FA8E":
                                replacement = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");
                                break;
                            case "D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27":
                            case "1AC14E77-02E7-4E5D-B744-2EB1AE5198B7":
                                replacement = Environment.ExpandEnvironmentVariables("%WinDir%") + "\\system32";
                                break;
                            default:
                                return null;
                        }

                        app = replacement + "\\" + app.Substring(2 + guid.Length);
                    }

                    if (!File.Exists(app)) return null;

                    try
                    {
                        icon = Icon.ExtractAssociatedIcon(app);
                    }
                    catch
                    {
                        return null;
                    }


                    using (var bmp = icon.ToBitmap())
                    {
                        string result = BitmapToBase64(bmp);
                        icon.Dispose();
                        return result;
                    }
                }
            }
        }

        private static string BitmapToBase64(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Png);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            string json = webBrowser.Document.InvokeScript("exportJson").ToString();
            Utility.WriteFile(file, json);
            ScriptInstaller.KillSearchApp();
            MessageBox.Show("Changes to Top Apps have been saved", "BeautySearch", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        public static void Launch()
        {
            string directory = $@"{ScriptInstaller.GetCurrentUserSearchAppDataDirectory()}\LocalState\DeviceSearchCache";
            var file = Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly)
                .Where(filename => filename.Substring(directory.Length + 1).StartsWith("AppCache"))
                .OrderByDescending(filename => File.GetLastWriteTime(filename))
                .FirstOrDefault();
            if (file == null)
            {
                MessageBox.Show("Failed to read Top Apps section storage", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string data = File.ReadAllText(file);
            new TopAppsEditorForm(file, data).Show();
        }

        /** https://stackoverflow.com/a/48701660 **/

        [DllImport("shlwapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false, ThrowOnUnmappableChar = true)]
        public static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf, IntPtr ppvReserved);

        public static string ExtractNormalPath(string indirectString)
        {
            StringBuilder outBuff = new StringBuilder(1024);
            int result = SHLoadIndirectString(indirectString, outBuff, outBuff.Capacity, IntPtr.Zero);

            return outBuff.ToString();
        }
    }
}
