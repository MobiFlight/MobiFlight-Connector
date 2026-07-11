using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using MobiFlight.UI.Dialogs;
using Newtonsoft.Json.Linq;

namespace MobiFlight.UpdateChecker
{
    static class AutoUpdateChecker
    {
        private const string GitHubLatestReleaseUrl = "https://api.github.com/repos/MobiFlight/MobiFlight-Connector/releases/latest";
        private const string GitHubReleasesUrl = "https://api.github.com/repos/MobiFlight/MobiFlight-Connector/releases";
        private const string MobiFlightInstaller = "MobiFlight-Installer.exe";

        public static void CheckForUpdate(bool silent = false)
        {
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (currentVersion.Major == 0)
            {
                Log.Instance.log("Skipping update check since this is an unreleased build.", LogSeverity.Info);
                return;
            }

            if (!TryGetLatestRelease(out var latestVersion, out var latestReleaseUrl, out var releaseNotes))
            {
                Log.Instance.log("Unable to check for updates from GitHub.", LogSeverity.Error);
                return;
            }

            var hasUpdate = currentVersion.CompareTo(latestVersion) < 0;
            if (hasUpdate)
            {
                using (var dialog = CreateReleaseNotesDialog(latestReleaseUrl, latestVersion, releaseNotes, true))
                {
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                    {
                        Process.Start(MobiFlightInstaller, "/install " + latestVersion);
                        Environment.Exit(0);
                    }
                }
                return;
            }

            if (!silent)
            {
                using (var dialog = CreateReleaseNotesDialog(latestReleaseUrl, latestVersion, releaseNotes, false))
                {
                    dialog.ShowDialog();
                }
            }

            Log.Instance.log("MobiFlight is up to date.", LogSeverity.Info);
        }

        private static WelcomeDialog CreateReleaseNotesDialog(string releaseUrl, Version version, string releaseNotes, bool showUpdateButtons)
        {
            var dialog = new WelcomeDialog
            {
                ReleaseNotes = releaseNotes,
                StartPosition = System.Windows.Forms.FormStartPosition.CenterParent,
                ShowUpdateButtons = showUpdateButtons,
                Text = "MobiFlight Release Notes " + version
            };
            if (string.IsNullOrEmpty(releaseNotes))
                dialog.WebsiteUrl = releaseUrl;
            dialog.ReleaseNotesClicked += (sender, e) => Process.Start(releaseUrl);
            return dialog;
        }

        private static bool TryGetLatestRelease(out Version latestVersion, out string releaseUrl, out string releaseNotes)
        {
            latestVersion = null;
            releaseUrl = null;
            releaseNotes = null;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("MobiFlight-Connector");
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var url = Properties.Settings.Default.BetaUpdates
                        ? GitHubReleasesUrl
                        : GitHubLatestReleaseUrl;
                    var response = client.GetAsync(url).GetAwaiter().GetResult();
                    if (!response.IsSuccessStatusCode)
                    {
                        Log.Instance.log($"GitHub update check returned HTTP {(int)response.StatusCode}.", LogSeverity.Error);
                        return !Properties.Settings.Default.BetaUpdates && TryGetLatestReleaseFromRedirect(client, out latestVersion, out releaseUrl);
                    }

                    var releaseJson = JToken.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    var releases = releaseJson is JArray
                        ? (JArray)releaseJson
                        : new JArray { releaseJson };

                    foreach (var release in releases.Where(IsEligibleRelease))
                    {
                        var tag = (string)release["tag_name"];
                        if (TryParseVersion(tag, out var version) && (latestVersion == null || version > latestVersion))
                        {
                            latestVersion = version;
                            releaseUrl = (string)release["html_url"];
                            releaseNotes = (string)release["body"];
                        }
                    }

                    Log.Instance.log($"Update check: current release candidate {latestVersion}.", LogSeverity.Info);
                }
            }
            catch (Exception ex)
            {
                Log.Instance.log("GitHub update check failed: " + ex.Message, LogSeverity.Error);
                if (!Properties.Settings.Default.BetaUpdates)
                {
                    using (var fallbackClient = new HttpClient())
                        return TryGetLatestReleaseFromRedirect(fallbackClient, out latestVersion, out releaseUrl);
                }
            }

            return latestVersion != null && !string.IsNullOrWhiteSpace(releaseUrl);
        }

        private static bool TryGetLatestReleaseFromRedirect(HttpClient client, out Version latestVersion, out string releaseUrl)
        {
            latestVersion = null;
            releaseUrl = null;
            try
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("MobiFlight-Connector");
                var response = client.GetAsync("https://github.com/MobiFlight/MobiFlight-Connector/releases/latest").GetAwaiter().GetResult();
                releaseUrl = response.RequestMessage.RequestUri.ToString();
                var tag = releaseUrl.Substring(releaseUrl.LastIndexOf('/') + 1);
                return TryParseVersion(tag, out latestVersion);
            }
            catch (Exception ex)
            {
                Log.Instance.log("GitHub latest release fallback failed: " + ex.Message, LogSeverity.Error);
                return false;
            }
        }

        private static bool IsEligibleRelease(JToken release)
        {
            if ((bool?)release["draft"] == true)
                return false;

            if (Properties.Settings.Default.BetaUpdates)
                return true;

            return (bool?)release["prerelease"] != true;
        }

        private static bool TryParseVersion(string tag, out Version version)
        {
            version = null;
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            tag = tag.Trim().TrimStart('v', 'V');
            return Version.TryParse(tag, out version);
        }
    }
}
