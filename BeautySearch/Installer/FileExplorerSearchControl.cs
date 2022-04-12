﻿using Microsoft.Win32;
using System.Windows.Forms;

namespace BeautySearch.Installer
{
    static class FileExplorerSearchControl
    {
        private const string NEW_SEARCH_DISABLE_KEY_ROOT = @"Software\Classes\CLSID\{1d64637d-31e9-4b06-9124-e83fb178ac6e}\";
        private const string NEW_SEARCH_DISABLE_KEY = NEW_SEARCH_DISABLE_KEY_ROOT + "TreatAs";

        public static void Toggle()
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
            int state = Utility.CheckIfMachineHasKey(NEW_SEARCH_DISABLE_KEY) ? 2 : (
                Utility.CheckIfCurrentUserHasKey(NEW_SEARCH_DISABLE_KEY) ? 1 : 0
            );

            if (state > 0)
            {
                if (state == 2 && !Utility.IsAdministrator())
                {
                    Utility.RunElevated("FileExplorerSearch", out _);
                }
                else
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
                            Utility.DeleteCurrentUserSubKeyTree(NEW_SEARCH_DISABLE_KEY);
                        }
                        else
                        {
                            Registry.LocalMachine.DeleteSubKeyTree(NEW_SEARCH_DISABLE_KEY);
                        }
                        MessageBox.Show("Restart File Explorer to apply the changes", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Do you want to disable the new File Explorer Search Experience introduced in 19H2?" + (Utility.IsAdministrator() ? "" : "\n\n(Run as administrator to disable for all users)"),
                    "BeautySearch",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (dialogResult == DialogResult.Yes)
                {
                    if (Utility.IsAdministrator())
                    {
                        dialogResult = MessageBox.Show(
                            "Do you want to disable it for all users?",
                            "BeautySearch",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question
                        );
                    }
                    else
                    {
                        dialogResult = DialogResult.No;
                    }

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
    }
}
