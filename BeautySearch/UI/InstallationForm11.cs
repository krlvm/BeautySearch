using BeautySearch.Installer;
using System;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace BeautySearch
{
    public partial class InstallationForm11 : Form
    {

        public InstallationForm11()
        {
            InitializeComponent();
        }

        private void InstallationForm11_Load(object sender, EventArgs e)
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

            AddFeature("Acrylic Context Menu", "contextMenuAcrylic", acrylicEnabled);
            AddFeature("Align widths of context menus", "unifyMenuWidth");
            AddFeature("Compact Context Menus", "compactContextMenus");
            AddFeature("Improve Context Menus appearance", "improveContextMenus");
            AddFeature("Improve overall appearance", "improveAppearance");
            AddFeature("Hide control outlines when using mouse", "hideOutlines");
            AddFeature("Hide Review and Share options for UWP apps", "hideUWPReviewShare");
            AddFeature("Always show recent files path", "alwaysShowActivityPath");
            AddFeature("Speed up loading", "speedLoad");
            AddFeature("Install for all users", "globalInstall");
            AddFeature("Controller Integration (Recommended)", "useController", ScriptInstaller.CURRENT_BUILD >= ScriptInstaller.BUILD_20H1);
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
            if (!Utility.IsAdministrator() && ScriptInstaller.IsInstalled() && ScriptInstaller.IsBingSearchEnabled())
            {
                var dialogResult = MessageBox.Show(
                    "Do you want to enable the existing BeautySearch installation? Press No to reinstall.",
                    "BeautySearch",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (dialogResult == DialogResult.Yes)
                {
                    ScriptInstaller.SetBingSearchEnabled(0);
                    ScriptInstaller.KillSearchApp();
                    Utility.ShowSearchWindow();
                    return;
                }
            }

            FeatureControl features = new FeatureControl();

            foreach (int i in featureBox.CheckedIndices)
            {
                features.Enable((featureBox.Items[i] as ListItem).Value);
            }

            int result;
            if (Utility.IsAdministrator())
            {
                result = ScriptInstaller.Install(features);
            }
            else if (!Utility.RunElevated($"Install \"{features.ToJson()}\"", out result)) return;
            switch (result)
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
                default:
                    MessageBox.Show("Unknown Error: " + result, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

            UpdateInstallationStatus();
        }

        private void uninstallBtn_Click(object sender, EventArgs e)
        {
            int result;
            if (Utility.IsAdministrator())
            {
                result = ScriptInstaller.Uninstall();
            }
            else if (!Utility.RunElevated("Uninstall", out result)) return;

            switch (result)
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
                default:
                    MessageBox.Show("Unknown Error: " + result, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            SystemSounds.Asterisk.Play();
        }

        private void optionsBtn_Click(object sender, EventArgs e)
        {
            optionsMenu.Show(Cursor.Position.X, Cursor.Position.Y);
        }

        private void optionsMenu_ToggleFileExplorerClassicSearch_Click(object sender, EventArgs e)
        {
            FileExplorerSearchControl.Toggle();
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
