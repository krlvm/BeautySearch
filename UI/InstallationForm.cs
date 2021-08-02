using System;
using System.Collections.Generic;
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

            List<string> disabledByDefault = new List<string>() { "topAppsCardsOutline", "explorerSearchFixes", "restyleOnLoad" };
            if (ScriptInstaller.CURRENT_BUILD >= ScriptInstaller.BUILD_20H1 && ScriptInstaller.CURRENT_BUILD < 19541)
            {
                disabledByDefault.Add("acrylicMode");
            }

            featureBox.Items.Add(new ListItem("Show accent color on Search Window", "backgroundMode"));
            featureBox.Items.Add(new ListItem("Enable Acrylic (or Fake Acrylic on 20H1+)", "acrylicMode"));
            featureBox.Items.Add(new ListItem("Remove background from UWP application icons", "disableTilesBackground"));
            featureBox.Items.Add(new ListItem("Fluent Context Menu", "contextMenuFluent"));
            featureBox.Items.Add(new ListItem("Acrylic Context Menu", "contextMenuAcrylic"));
            featureBox.Items.Add(new ListItem("Context Menu Shadows", "contextMenuShadows"));
            featureBox.Items.Add(new ListItem("Align widths of context menus", "unifyMenuWidth"));
            featureBox.Items.Add(new ListItem("Hide control outlines when using mouse", "hideOutlines"));
            featureBox.Items.Add(new ListItem("Make Top Apps look like Start Menu tiles", "topAppsCardsOutline"));
            if (ScriptInstaller.CURRENT_BUILD > ScriptInstaller.BUILD_19H2)
            {
                featureBox.Items.Add(new ListItem("[19H2+] Improve Explorer Search look (for 125% DPI Scaling)", "explorerSearchFixes"));
            }
            featureBox.Items.Add(new ListItem("Check theme changes more frequently (19H1, 19H2)", "restyleOnLoad"));
            featureBox.Items.Add(new ListItem("Controller Integration (Recommended)", "useController"));

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

            features.Set("theme", themeGroup.Controls.OfType<System.Windows.Forms.RadioButton>()
                                      .FirstOrDefault(r => r.Checked).Text.ToLower());
            features.Set("corners", cornersGroup.Controls.OfType<System.Windows.Forms.RadioButton>()
                                      .FirstOrDefault(r => r.Checked).Text.ToLower());

            if (features.Get("acrylicMode") != null && ScriptInstaller.CURRENT_BUILD >= ScriptInstaller.BUILD_20H1 && ScriptInstaller.CURRENT_BUILD < 19541)
            {
                features.Set("acrylicMode", "fake");
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
                    MessageBox.Show("BeautySearch can be installed only on Windows 10 May 2019 Update (19H1, Build 18363) and higher", "Unsupported Build", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
