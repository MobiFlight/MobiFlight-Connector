using System;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MobiFlight.UI.Dialogs;

namespace MobiFlight.UpdateChecker
{
    static class AutoUpdateChecker
    {
        private enum UpdatePath
        {
            StableToStable,
            StableToBeta,
            BetaToBeta,
            BetaToStable
        }

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

        private static string ReleaseVersion(string version)
        {
            if (!Version.TryParse(version, out var parsedVersion))
                return version;

            return parsedVersion.Revision > 0
                ? parsedVersion.ToString(4)
                : parsedVersion.ToString(3);
        }

        private static bool IsBetaVersion(Version version)
        {
            return version.Revision > 0;
        }

        private static UpdatePath GetUpdatePath(bool currentIsBeta, string targetChannel)
        {
            var targetIsBeta = targetChannel == "BETA";

            if (!currentIsBeta && !targetIsBeta)
                return UpdatePath.StableToStable;
            if (!currentIsBeta)
                return UpdatePath.StableToBeta;
            if (targetIsBeta)
                return UpdatePath.BetaToBeta;
            return UpdatePath.BetaToStable;
        }

        public static async Task CheckForUpdate(bool silent = false)
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

            string output;
            string error;
            try
            {
                (output, error) = await Task.Run(() => RunInstallerCheck(CommandToSend));
            }
            catch (Exception ex)
            {
                Log.Instance.log("Unable to run MobiFlight-Installer.exe: " + ex.Message, LogSeverity.Error);
                return;
            }

            Console.WriteLine(output + error);

            // Get the new version and the channel (BETA or RELEASE) from the installer output
            if (VersionCheck(output, out string newVersion, out string betaOrRelease))
            {
                Log.Instance.log($"Found a new version: {newVersion} {betaOrRelease}.", LogSeverity.Info);
                var updatePath = GetUpdatePath(IsBetaVersion(CurVersion), betaOrRelease);
                Log.Instance.log($"Update path: {updatePath}.", LogSeverity.Info);

                var releaseVersion = ReleaseVersion(newVersion);
                var releaseUrl = $"https://github.com/MobiFlight/MobiFlight-Connector/releases/tag/{releaseVersion}";
                var releaseLabel = betaOrRelease == "BETA" ? " (BETA)" : string.Empty;
                using (var dialog = new WelcomeDialog())
                {
                    dialog.WebsiteUrl = releaseUrl;
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    dialog.ShowUpdateButtons = true;
                    dialog.ShowDisableBetaButton = updatePath == UpdatePath.StableToBeta //Show "DisableBetaButton" if update from stable to beta
                        && Properties.Settings.Default.BetaUpdates;
                    dialog.Text = "MobiFlight Release Notes " + releaseVersion + releaseLabel;
                    dialog.ReleaseNotesClicked += (sender, e) => Process.Start(releaseUrl);
                    dialog.DisableBetaUpdatesClicked += (sender, e) =>
                    {
                        Properties.Settings.Default.BetaUpdates = false;
                        Properties.Settings.Default.Save();
                    };

                    if (dialog.ShowDialog() == DialogResult.Yes)
                    {
                        try
                        {
                            Process.Start(mobiFlightInstaller, "/install " + newVersion);
                            Environment.Exit(0);
                        }
                        catch (Exception ex)
                        {
                            Log.Instance.log("Unable to start MobiFlight-Installer.exe: " + ex.Message, LogSeverity.Error);
                            MessageBox.Show("The installer could not be started. Please update manually.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
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

        // Runs the installer to check for available updates
        private static (string output, string error) RunInstallerCheck(string arguments)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = mobiFlightInstaller;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit(UpdateCheckTimeoutInMs);
                return (output, error);
            }
        }
    }
}
