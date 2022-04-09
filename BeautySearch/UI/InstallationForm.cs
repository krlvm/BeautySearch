using BeautySearch.UI;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Media;
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
            AddFeature("Enhanced design with more Acrylic", "enhancedAcrylic", true);
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
                AddFeature("Automatically recent last activity items count", "adaptiveActivityCount", true);
                AddFeature("Hide user profile icon", "hideUserProfile", true);
                AddFeature("Hide close button", "hideCloseButton", true);
                AddFeature("Install for all users", "globalInstall", true);
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
            SystemSounds.Asterisk.Play();
        }

        private void optionsBtn_Click(object sender, EventArgs e)
        {
            optionsMenu.Show(Cursor.Position.X, Cursor.Position.Y);
        }

        private void optionsMenu_ClearIconCache_Click(object sender, EventArgs e)
        {
            ScriptInstaller.ClearIconCache();
            Utility.ShowSearchWindow();
        }

        private void optionsMenu_EditTopApps_Click(object sender, EventArgs e)
        {
            TopAppsEditorForm.Launch();
        }

        private void optionsMenu_EditSearchBoxText_Click(object sender, EventArgs e)
        {
            SearchBoxTextInputForm dialog = new SearchBoxTextInputForm();
            dialog.textInput.Text = ScriptInstaller.GetSearchBoxText();
            DialogResult result = dialog.ShowDialog(this);

            if (result != DialogResult.Cancel)
            {
                ScriptInstaller.SetSearchBoxText(result == DialogResult.OK ? dialog.textInput.Text : null);
                MessageBox.Show("Search Box Text has been changed.\nRestart File Explorer to fully apply the changes.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            dialog.Dispose();
        }


        private void optionsMenu_SearchBoxTheme_CheckedChanged(object sender, EventArgs e)
        {
            using (RegistryKey key = Utility.OpenCurrentUserRegistryKey(ScriptInstaller.SEARCH_APP_REGISTRY, true))
            {
                if (key != null)
                {
                    key.SetValue("SearchBoxTheme", optionsMenu_SearchBoxTheme.Checked ? 2 : 0, RegistryValueKind.DWord);
                }
            }
        }
        private void optionsMenu_SearchBoxTheme_UpdateStatus()
        {
            using (RegistryKey key = Utility.OpenCurrentUserRegistryKey(ScriptInstaller.SEARCH_APP_REGISTRY, false))
            {
                optionsMenu_SearchBoxTheme.Checked = key != null && (int)key.GetValue("SearchBoxTheme") == 2;
            }
        }

        private const string NEW_SEARCH_DISABLE_KEY_ROOT = @"Software\Classes\CLSID\{1d64637d-31e9-4b06-9124-e83fb178ac6e}\";
        private const string NEW_SEARCH_DISABLE_KEY = NEW_SEARCH_DISABLE_KEY_ROOT + "TreatAs";
        private void optionsMenu_ToggleFileExplorerClassicSearch_Click(object sender, EventArgs e)
        {
            if (ScriptInstaller.CURRENT_BUILD < ScriptInstaller.BUILD_20H1)
            {
                MessageBox.Show(
                    "This is supported only on 20H1 and higher",
                    "BeautySearch",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // 0 - new search, 1 - old search, 2 - old search (all users)
            int state = Utility.CheckIfMachineHasKey(NEW_SEARCH_DISABLE_KEY_ROOT) ? 2 : (
                Utility.CheckIfCurrentUserHasKey(NEW_SEARCH_DISABLE_KEY_ROOT) ? 1 : 0
            );

            if (state > 0)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "New File Explorer Search Experience is disabled.\nDo you want to re-enable it?",
                    "BeautySearch",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (dialogResult == DialogResult.Yes)
                {
                    if (state == 1)
                    {
                        Utility.DeleteCurrentUserSubKeyTree(NEW_SEARCH_DISABLE_KEY_ROOT);
                    }
                    else
                    {
                        Registry.LocalMachine.DeleteSubKeyTree(NEW_SEARCH_DISABLE_KEY_ROOT);
                    }
                    MessageBox.Show("Restart File Explorer to apply the changes", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Do you want to disable the new File Explorer Search Experience introduced in 19H2?",
                    "BeautySearch",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (dialogResult == DialogResult.Yes)
                {
                    dialogResult = MessageBox.Show(
                        "Do you want to disable it for all users?",
                        "BeautySearch",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question
                    );

                    RegistryKey key = null;
                    if (dialogResult == DialogResult.Yes)
                    {
                        key = Registry.LocalMachine.OpenSubKey(NEW_SEARCH_DISABLE_KEY, true);
                        if (key == null)
                        {
                            key = Registry.LocalMachine.CreateSubKey(NEW_SEARCH_DISABLE_KEY, true);
                        }
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        Utility.OpenCurrentUserRegistryKey(NEW_SEARCH_DISABLE_KEY, true);
                    }

                    if (key != null)
                    {
                        key.SetValue("", "{64bc32b5-4eec-4de7-972d-bd8bd0324537}", RegistryValueKind.String);
                        MessageBox.Show("Restart File Explorer to apply the changes", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        #endregion

        private void UpdateInstallationStatus()
        {
            installBtn.Text = ScriptInstaller.IsInstalled() ? "Reinstall" : "Install";
            optionsMenu_SearchBoxTheme_UpdateStatus();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(linkLabel1.Text));
        }
    }
}
