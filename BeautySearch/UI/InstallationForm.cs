using BeautySearch.UI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace BeautySearch
{
    public partial class InstallationForm : Form
    {

        public InstallationForm()
        {
            InitializeComponent();
        }

        private void InstallationForm_Load(object sender, EventArgs e)
        {
            string flavour;
#if DEBUG
            flavour = "(Debug)";
#else
            flavour = "v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
#endif
            this.Text = "BeautySearch Installer " + flavour;

            EnumerateFeatures();
            UpdateInstallationStatus();
        }

        #region Features

        private void EnumerateFeatures()
        {
            bool acrylicEnabled = Utility.IsPersonalizationFeatureEnabled("EnableTransparency");

            if (!ScriptInstaller.is20H1() || !ScriptInstaller.is20H1Fixed())
            {
                AddFeature("Show accent color on Search Window", "backgroundMode", Utility.IsPersonalizationFeatureEnabled("ColorPrevalence"));
            }
            AddFeature("Enable Acrylic" + (!ScriptInstaller.is20H1Fixed() ? " (or Fake Acrylic on 20H1+)" : ""), "acrylicMode", acrylicEnabled);
            AddFeature("Enhanced design with more Acrylic", "enhancedAcrylic", acrylicEnabled);
            AddFeature("Remove background from UWP application icons", "disableTilesBackground", ScriptInstaller.CURRENT_BUILD >= ScriptInstaller.BUILD_20H1+1);
            AddFeature("Fluent Context Menu", "contextMenuFluent");
            AddFeature("Acrylic Context Menu", "contextMenuAcrylic", acrylicEnabled);
            AddFeature("Context Menu Shadows", "contextMenuShadows", acrylicEnabled);
            AddFeature("Align widths of context menus", "unifyMenuWidth");
            AddFeature("Hide control outlines when using mouse", "hideOutlines");
            AddFeature("Make Top Apps look like Start Menu tiles", "topAppsCardsOutline");
            if (ScriptInstaller.CURRENT_BUILD >= ScriptInstaller.BUILD_19H2)
            {
                AddFeature("[19H2+] Improve Explorer Search look (for 125% DPI Scaling)", "explorerSearchFixes", Utility.GetDPIScaling() == 120);
            }
            if (!ScriptInstaller.is20H1Fixed())
            {
                AddFeature("[20H1+] Capture desktop to apply Fake Acrylic", "fakeAcrylicDesktopCrop", false);
                AddFeature("[20H1+] Don't override Fake Acrylic wallpaper for now", "skipFakeAcrylic", false);
            }
            else
            {
                AddFeature("Disable two-panel mode", "disableTwoPanel", true);
                AddFeature("Limit recent activity items to 4", "limitActivity", true);
                AddFeature("Hide user profile icon", "hideUserProfile", true);
                AddFeature("Hide close button", "hideCloseButton", true);
            }
            /*
            AddFeature("Check theme changes more frequently [Accent Color]", "restyleOnLoadAccent", ScriptInstaller.CURRENT_BUILD < ScriptInstaller.BUILD_20H1);
            AddFeature("Check theme changes more frequently [Acrylic & Theme]", "restyleOnLoadAcrylic");
            AddFeature("Check theme changes more frequently [Theme]", "restyleOnLoadTheme");
            */
            AddFeature("Controller Integration (Recommended for 20H1+)", "useController", ScriptInstaller.CURRENT_BUILD >= ScriptInstaller.BUILD_20H1);
        }

        private void AddFeature(string name, string id)
        {
            AddFeature(name, id, true);
        }

        private void AddFeature(string name, string id, bool isEnabled)
        {
            featureBox.Items.Add(new ListItem(name, id));
            featureBox.SetItemChecked(featureBox.Items.Count - 1, isEnabled);
        }

        #endregion

        #region Install / Uninstall

        private void installBtn_Click(object sender, EventArgs e)
        {
            if (Utility.RequireAdministrator())
            {
                return;
            }

            FeatureControl features = new FeatureControl();

            foreach (int i in featureBox.CheckedIndices)
            {
                features.Enable((featureBox.Items[i] as ListItem).Value);
            }

            features.Set("theme", themeGroup.Controls.OfType<System.Windows.Forms.RadioButton>()
                                      .FirstOrDefault(r => r.Checked).Text.ToLower());
            features.Set("corners", cornersGroup.Controls.OfType<System.Windows.Forms.RadioButton>()
                                      .FirstOrDefault(r => r.Checked).Text.ToLower());

            if (features.Get("acrylicMode") != null && !ScriptInstaller.is20H1Fixed())
            {
                features.Set("acrylicMode", "fake");
            }

            switch (ScriptInstaller.Install(features))
            {
                case 0:
                    //MessageBox.Show("BeautySearch successfully installed", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Utility.ShowSearchWindow();
                    break;
                case ScriptInstaller.ERR_READ:
                    MessageBox.Show("Failed to read target file (not enough permissions?)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ScriptInstaller.ERR_WRITE:
                    MessageBox.Show("Failed to write target file (not enough permissions?)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ScriptInstaller.ERR_KILL_FAILED:
                    MessageBox.Show("Sign out and sign in to finish installation", "BeautySearch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case ScriptInstaller.ERR_OLD_BUILD:
                    MessageBox.Show("BeautySearch can be installed only on Windows 10 May 2019 Update (19H1, Build 18363) and higher", "Unsupported Build", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

            UpdateInstallationStatus();
        }

        private void uninstallBtn_Click(object sender, EventArgs e)
        {
            if (Utility.RequireAdministrator())
            {
                return;
            }

            switch (ScriptInstaller.Uninstall())
            {
                case 0:
                    MessageBox.Show("BeautySearch successfully uninstalled", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case ScriptInstaller.ERR_NOT_INSTALLED:
                    MessageBox.Show("BeautySearch is not installed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ScriptInstaller.ERR_READ:
                    MessageBox.Show("Failed to read target file (not enough permissions?)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ScriptInstaller.ERR_WRITE:
                    MessageBox.Show("Failed to write target file (not enough permissions?)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ScriptInstaller.ERR_KILL_FAILED:
                    MessageBox.Show("BeautySearch has been uninstalled, sign out and sign in for the change to take effect", "BeautySearch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }

            UpdateInstallationStatus();
        }

        #endregion

        #region Service Buttons

        private void searchRestartBtn_Click(object sender, EventArgs e)
        {
            ScriptInstaller.KillSearchApp();
            Utility.ShowSearchWindow();
        }

        private void clearIconCacheBtn_Click(object sender, EventArgs e)
        {
            ScriptInstaller.ClearIconCache();
            Utility.ShowSearchWindow();
        }

        private void topAppsBtn_Click(object sender, EventArgs e)
        {
            TopAppsEditorForm.Launch();
        }

        #endregion

        private void UpdateInstallationStatus()
        {
            installBtn.Text = ScriptInstaller.IsInstalled() ? "Reinstall" : "Install";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(linkLabel1.Text));
        }
    }
}
