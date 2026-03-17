using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlightInstaller
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            if (MobiFlightUpdaterModel.VerifyCurrentFolderRight())
            {
                MobiFlightUpdaterModel.DeleteLogFileIfIsTooBig();
                LogAppenderFile logAppenderFile = new LogAppenderFile(false);
                Log.Instance.AddAppender(logAppenderFile);
                Log.Instance.Enabled = true;
                Log.Instance.Severity = LogSeverity.Debug;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Log.Instance.log("Installer start", LogSeverity.Debug);

                CmdLineParams cmdParams = new CmdLineParams(Environment.GetCommandLineArgs());
                MobiFlightUpdaterModel.InstallOnly = cmdParams.InstallOnly;

                var updateUrl = MobiFlightUpdaterModel.MobiFlightUpdateUrl;
                if (cmdParams.CacheId != null)
                {
                    updateUrl += "?cache=" + cmdParams.CacheId;
                }
                
                MobiFlightUpdaterModel.DownloadVersionsList(updateUrl);
                
                if (cmdParams.HasParams()) // if args are present
                {
                    // install specific version by command line
                    if (cmdParams.Install != null)
                    {
                        Log.Instance.log("ManualUpgradeFromCommandLine START from args -> " + cmdParams.Install, LogSeverity.Debug);
                        Application.Run(new UI.UpdaterMainForm());
                    }
                    else if (cmdParams.Check || cmdParams.CheckBeta)
                    {
                        Log.Instance.log("ExternAskToCheckLastVersion START from args -> Check BETA " + cmdParams.CheckBeta, LogSeverity.Debug);
                        MobiFlightUpdaterModel.ExternAskToCheckLastVersion(cmdParams.CheckBeta);
                    }
                    // expert mode, start interface to choose version
                    else if (cmdParams.ExpertMode)
                    {
                        Log.Instance.log("EXPERT mode enable", LogSeverity.Debug);
                        Application.Run(new UI.ExpertMainForm());
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
                else
                {
                    Application.Run(new UI.UpdaterMainForm());
                }
            }
            else
            {
                MessageBox.Show(i18n.tr("NotEnoughRightsForInstallation"));
                Environment.Exit(0);
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
