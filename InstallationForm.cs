using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            List<string> disabledByDefault = new List<string>() { "disableContextMenuBorder", "explorerSearchBorder", "fakeBackgroundAcrylic", "fluentNoise" };

            featureBox.Items.Add(new ListItem("Controller Integration", "useController"));
            featureBox.Items.Add(new ListItem("Show accent color on Search Window", "accentBackground"));
            featureBox.Items.Add(new ListItem("Show search results in Dark Theme", "darkTheme"));
            featureBox.Items.Add(new ListItem(" - Follow system theme (inaccurate, enable the previous option)", "dynamicDarkTheme"));
            featureBox.Items.Add(new ListItem("Dark Theme Acrylic Sidebar (and accent color when enabled)", "darkThemeAcrylic"));
            featureBox.Items.Add(new ListItem("Remove background from UWP application icons", "disableTilesBackground"));
            featureBox.Items.Add(new ListItem("Add shadows to context menus", "contextMenuShadow"));
            featureBox.Items.Add(new ListItem("Make context menu's corners rounded", "contextMenuRound"));
            featureBox.Items.Add(new ListItem("Add acrylic effect to context menus", "contextMenuAcrylic"));
            featureBox.Items.Add(new ListItem("Hide context menu's borders", "disableContextMenuBorder"));
            featureBox.Items.Add(new ListItem("Hide button outlines when using mouse", "hideOutlines"));
            featureBox.Items.Add(new ListItem("Fix missing 19H2+ Explorer Search Box bottom border on HiDPI", "explorerSearchBorder"));
            featureBox.Items.Add(new ListItem("20H1+ Broken Acrylic Workaround", "fakeBackgroundAcrylic"));
            featureBox.Items.Add(new ListItem("Fluent Design Noise Texture on Acrylic Surface", "fluentNoise"));

            if (ScriptInstaller.CURRENT_BUILD >= 19041 && ScriptInstaller.CURRENT_BUILD < 19541)
            {
                disabledByDefault.Remove("fakeBackgroundAcrylic");
            }

            for (int i = 0; i < featureBox.Items.Count; i++)
            {
                featureBox.SetItemChecked(i, !disabledByDefault.Contains((featureBox.Items[i] as ListItem).Value));
            }            

            UpdateInstallStatus();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(linkLabel1.Text));
        }

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


            switch (ScriptInstaller.Install(features))
            {
                case 0:
                    MessageBox.Show("BeautySearch successfully installed", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show("BeautySearch can be installed only on Windows 10 May 2020 Update (20H1, Build 19041) and higher", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

            UpdateInstallStatus();
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

            UpdateInstallStatus();
        }

        private void UpdateInstallStatus()
        {
            installBtn.Text = ScriptInstaller.IsInstalled() ? "Reinstall" : "Install";
        }
    }
}
