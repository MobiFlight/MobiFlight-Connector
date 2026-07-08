using System;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;


namespace MobiFlight.UpdateChecker
{
    static class AutoUpdateChecker
    {
        static readonly string mobiFlightInstaller = "MobiFlight-Installer.exe";
        static readonly int UpdateCheckTimeoutInMs = 5000;
        private static bool VersionCheck(string output, out string newVersion, out string betaOrRelease)
        {
            newVersion = null;
            betaOrRelease = null;

            if (!output.Contains("##RESULT##|1"))
                return false;
            string[] outputArray = output.Split('|'); // Get the version number
            newVersion = outputArray[2];
            string[] versionArray = newVersion.Split('.'); // Split the version number to get the last number
            int versionLastNumber = Int32.Parse(versionArray[3]);

            betaOrRelease = (outputArray.Length == 4) && (versionLastNumber > 0)
                ? "BETA"
                : "RELEASE";
            return true;
            
        }
        public static void CheckForUpdate(bool silent = false)
        {
            String hash = (Environment.UserName + Environment.MachineName).GetHashCode().ToString();
            if (Properties.Settings.Default.CacheId == "0") Properties.Settings.Default.CacheId = Guid.NewGuid().ToString();
            var trackingParams = $"{hash}-{Properties.Settings.Default.CacheId}-{Properties.Settings.Default.Started}";

            var CurVersion = Assembly.GetExecutingAssembly().GetName().Version;

            // Issue 1365: Don't check for updates if the build came from a pull request. These builds are
            // identified by the major version being 0.
            if (CurVersion.Major == 0)
            {
                Log.Instance.log("Skipping update check since this is an unreleased build.", LogSeverity.Info);
                return;
            }

            var CommandToSend = $"/check /version {CurVersion} /cacheId {trackingParams}";

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

            if (VersionCheck(output, out string newVersion, out string betaOrRelease))
            {
                Log.Instance.log($"Found a new version: {newVersion} {betaOrRelease}.", LogSeverity.Info);

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
