using System;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;


namespace MobiFlight.UpdateChecker
{
    static class AutoUpdateChecker
    {
        static string mobiFlightInstaller = "MobiFlight-Installer.exe";
        static int UpdateCheckTimeoutInMs = 5000;
        public static void CheckForUpdate(bool force = false, bool silent = false)
        {
            String hash = (Environment.UserName + Environment.MachineName).GetHashCode().ToString();
            if (Properties.Settings.Default.CacheId == "0") Properties.Settings.Default.CacheId = Guid.NewGuid().ToString();
            String trackingParams = hash + "-" + Properties.Settings.Default.CacheId + "-" + Properties.Settings.Default.Started;

            string CurVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string CommandToSend = "/check /version " + CurVersion + " /cacheId " + trackingParams;

            if (Properties.Settings.Default.BetaUpdates)
            {
                CommandToSend += " /beta";
                Log.Instance.log("Checking for BETA update...", LogSeverity.Info);
            }
            else
            {
                Log.Instance.log("Checking for RELEASE update...", LogSeverity.Info);
            }

            if (!File.Exists(mobiFlightInstaller))
            {
                Log.Instance.log("MobiFlight-Installer.exe does not exist, impossible to check for update.", LogSeverity.Error);
                return;
            }

            System.Diagnostics.Process p = new Process();
            p.StartInfo.FileName = mobiFlightInstaller;
            p.StartInfo.Arguments = CommandToSend;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();
            p.WaitForExit(UpdateCheckTimeoutInMs);

            Console.WriteLine(output + error);
            if (output.Contains("##RESULT##|1"))
            {
                string[] OutputArray = output.Split('|'); // Get the version number
                string newVersion = OutputArray[2];
                string[] VersionArray = newVersion.Split('.'); // Split the version number to get the last number
                int VersionLastnumber = Int32.Parse(VersionArray[3]);
                string BetaOrRelease;
                
                if ((OutputArray.Length == 4) & (VersionLastnumber > 0)) // If the last number is > 0 is a beta version
                    BetaOrRelease = "BETA";
                else
                    BetaOrRelease = "RELEASE";

                Log.Instance.log($"Found a new version: {newVersion} {BetaOrRelease}.", LogSeverity.Info);

                DialogResult dialogResult = MessageBox.Show(
                    String.Format(i18n._tr("uiMessageNewUpdateAvailablePleaseUpdate"), newVersion),
                    i18n._tr("uiMessageNewUpdateAvailable"),
                    MessageBoxButtons.YesNo
                );

                if (dialogResult == DialogResult.Yes)
                {
                    Process.Start(mobiFlightInstaller, "/install " + newVersion);
                    Environment.Exit(0);
                }
                return;
            }
            if (!silent)
                MessageBox.Show(
                    String.Format(i18n._tr("uiMessageNoUpdateNecessary"), MobiFlight.UI.MainForm.DisplayVersion()),
                    i18n._tr("Hint"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            Log.Instance.log("MobiFlight is up to date.", LogSeverity.Info);
            return;
        }
    }
}
