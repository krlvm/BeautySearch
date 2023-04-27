using BeautySearch.Installer;
using System;
using System.Windows.Forms;

namespace BeautySearch
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "auto":
                        {
                            var features = new FeatureControl();

                            if (args.Length != 3 || !args[2].Equals("disable-enhancements"))
                            {
                                features.Enable("enhancedAcrylic");
                            }
                            features.Enable("contextMenuFluent");
                            features.Enable("unifyMenuWidth");
                            features.Enable("hideOutlines");
                            features.Enable("topAppsCardsOutline");
                            features.Set("theme", args.Length > 1 ? args[1].ToLower() : "auto");
                            features.Set("corners", "sharp");
                            if (Utility.IsPersonalizationFeatureEnabled("EnableTransparency"))
                            {
                                features.Set("acrylicMode", ScriptInstaller.ACRYLIC_SUPPORTED ? "true" : "fake");
                                features.Enable("contextMenuAcrylic");
                                features.Enable("contextMenuShadows");
                            }
                            if (Utility.IsPersonalizationFeatureEnabled("ColorPrevalence"))
                            {
                                features.Enable("backgroundMode");
                            }
                            if (SystemInfo.BUILD_NUMBER >= OSBuild.V20H1 + 1)
                            {
                                features.Enable("disableTilesBackground");
                            }
                            if (SystemInfo.BUILD_NUMBER >= OSBuild.V19H2 && Utility.GetDPIScaling() == 120)
                            {
                                features.Enable("explorerSearchFixes");
                            }
                            if (SystemInfo.BUILD_NUMBER >= OSBuild.V20H1)
                            {
                                features.Enable("useController");
                            }

                            ScriptInstaller.Install(features);

                            Console.WriteLine("BeautySearch has been installed");
                            break;
                        }
                    case "Install":
                        {
                            Environment.Exit(ScriptInstaller.Install(FeatureControl.Parse(args[1])));
                            break;
                        }
                    case "Uninstall":
                        {
                            Environment.Exit(ScriptInstaller.Uninstall(args.Length == 2 && "-Silent".Equals(args[1])));
                        }
                        break;
                    case "FileExplorerSearch":
                        {
                            FileExplorerSearchControl.Toggle();
                        }
                        break;
                    default:
                        break;
                }
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (SystemInfo.BUILD_NUMBER >= OSBuild.V11_21H2)
            {
                Application.Run(new InstallationForm11());
            }
            else
            {
                Application.Run(new InstallationForm());
            }
        }
    }
}
