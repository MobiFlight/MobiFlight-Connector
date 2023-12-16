using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.ComponentModel;

namespace MobiFlightInstaller.UI
{
    public partial class UpdaterMainForm : Form
    {

        private string _downloadURL = "";
        private string _downloadChecksum = "";
        private WebClient _webClient;
        private string CurrentFileName = "";

        public UpdaterMainForm()
        {
            InitializeComponent();

            Log.Instance.log("No args, Updater start auto install last version", LogSeverity.Debug);

            bool IsMfHaveBetaEnable = MobiFlightHelperMethods.GetMfBetaOptionValue();
            var CurVersion = new Version(MobiFlightUpdaterModel.GetInstalledVersion());
            var TargetVersion = new Version(MobiFlightUpdaterModel.GetTheLastVersionNumberAvailable(IsMfHaveBetaEnable));
            var result = CurVersion.CompareTo(TargetVersion);
            if (result < 0) // direct install last release version if MF don't exist OR if a new version is available
            {
                ManualUpgradeFromCommandLine(TargetVersion.ToString());
            }
            else
            {
                MobiFlightUpdaterModel.StartProcessAndClose(MobiFlightHelperMethods.ProcessName);
            }
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            UpdaterProgressBar.Value = e.ProgressPercentage;
        }

        private void OnDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            Log.Instance.log("Download finished.", LogSeverity.Debug);
            UpdaterCurrentTask.Text = "Download finished.";
            if (MobiFlightUpdaterModel.CheckIfFileIsHere(CurrentFileName, _downloadChecksum)) //compare checksum if download is correct
            {
                Log.Instance.log("DOWNLOAD is OK.", LogSeverity.Debug);
                ExtractAndInstall(CurrentFileName);
            }
            else
            {
                Log.Instance.log("DOWNLOAD is incorrect, installation aborted", LogSeverity.Debug);
                MessageBox.Show("Download failed, installation aborted! Please run installer again or unzip manually.");
            }
            _webClient.Dispose();
        }

        private void ExtractAndInstall(string currentFileName)
        {
            UpdaterCurrentTask.Text = "Extracting files...";
            MobiFlightUpdaterModel.CloseMobiFlightAndWait();
            MobiFlightUpdaterModel.GoExtractToDirectory(currentFileName, Directory.GetCurrentDirectory());
            UpdaterCurrentTask.Text = "Start MobiFlight";
            MobiFlightUpdaterModel.StartProcessAndClose(MobiFlightHelperMethods.ProcessName);
        }

        private void ManualUpgradeFromCommandLine(string Version)
        {
            if (MobiFlightUpdaterModel.resultList[Version]["url"].Length > 5)
            {
                _downloadURL = MobiFlightUpdaterModel.resultList[Version]["url"];
                _downloadChecksum = MobiFlightUpdaterModel.resultList[Version]["checksum"];
                CurrentFileName = Directory.GetCurrentDirectory() + "\\" + MobiFlightUpdaterModel.GetFileName(_downloadURL);

                if (!MobiFlightUpdaterModel.CheckIfFileIsHere(CurrentFileName, _downloadChecksum)) //compare checksum if download the file is needeed
                {

                    UpdaterProgressBar.Value = 0;
                    UpdaterCurrentTask.Text = "Downloading...";
                    Log.Instance.log("Downloading " + CurrentFileName, LogSeverity.Debug); 
                    _webClient = new WebClient();
                    var uri = new Uri(_downloadURL);
                    _webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnDownloadProgressChanged);
                    _webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(OnDownloadComplete);
                    _webClient.DownloadFileAsync(uri, CurrentFileName); // Download the file

                    // Extract and install is done in the OnDownloadFinished
                    // 
                    return;
                }
                UpdaterCurrentTask.Text = $"MobiFlight file {Version} already available. No download needed.";
                ExtractAndInstall(CurrentFileName);
            }
            else
            {
                Log.Instance.log("URL is incorrect, installation aborted", LogSeverity.Debug);
                MessageBox.Show("URL is incorrect, impossible to download the file, installation aborted!");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
