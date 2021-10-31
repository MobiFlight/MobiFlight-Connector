using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;

namespace MobiFlightInstaller.UI
{
    public partial class InstallMainForm : Form
    {
        private string _downloadURL = "";
        private string _downloadChecksum = "";
        private WebClient _webClient;
        private string CurrentFileName = "";
        private string PathToInstall = "";

        public InstallMainForm()
        {
            InitializeComponent();
            SelectedPath.Text = Directory.GetCurrentDirectory();
            AsyncCheckGithub();
            BoxTargetDirectory.Checked = MobiFlightUpdaterModel.VerifyCurrentFolderRight();
            IfReadyForTakeOFF();
        }

        private void buttonChooseFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = SelectedPath.Text;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                SelectedPath.Text = folderBrowserDialog1.SelectedPath;
                PathToInstall = SelectedPath.Text;
                MobiFlightUpdaterModel.SetInstallPath(PathToInstall);
            }
            BoxTargetDirectory.Checked = MobiFlightUpdaterModel.VerifyCurrentFolderRight();
            IfReadyForTakeOFF();
        }

        private void buttonStartInstall_Click(object sender, EventArgs e)
        {
            Log.Instance.log("SETUP : Start new installation to " + SelectedPath.Text, LogSeverity.Debug);

            var TargetVersion = new Version(MobiFlightUpdaterModel.GetTheLastVersionNumberAvailable(false));
            PathToInstall = SelectedPath.Text;
            MobiFlightUpdaterModel.SetInstallPath(PathToInstall);
            ManualUpgradeFromCommandLine(TargetVersion.ToString());
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            SetupProgressBar.Value = e.ProgressPercentage;
        }

        private void OnDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            Log.Instance.log("Download finished.", LogSeverity.Debug);
            UpdaterCurrentTask.Text = "Download finished.";
            if (MobiFlightUpdaterModel.CheckIfFileIsHere(CurrentFileName, _downloadChecksum)) //compare checksum if download is correct
            {
                UpdaterCurrentTask.Text = "Extracting files...";
                Log.Instance.log("DOWNLOAD is OK, start extracting ...", LogSeverity.Debug);
                MobiFlightUpdaterModel.CloseMobiFlightAndWait();
                MobiFlightUpdaterModel.GoExtractToDirectory(CurrentFileName, PathToInstall);
                UpdaterCurrentTask.Text = "Start MobiFlight";
                MobiFlightUpdaterModel.StartProcessAndClose(MobiFlightHelperMethods.ProcessName);
            }
            else
            {
                Log.Instance.log("DOWNLOAD is incorrect, installation aborted", LogSeverity.Debug);
                MessageBox.Show("Download failed, installation aborted! Please run installer again or unzip manually.");
            }
            _webClient.Dispose();
        }

        private void ManualUpgradeFromCommandLine(string Version)
        {
            if (MobiFlightUpdaterModel.resultList[Version]["url"].Length > 5)
            {
                _downloadURL = MobiFlightUpdaterModel.resultList[Version]["url"];
                _downloadChecksum = MobiFlightUpdaterModel.resultList[Version]["checksum"];
                CurrentFileName = PathToInstall + "\\" + MobiFlightUpdaterModel.GetFileName(_downloadURL);

                if (!MobiFlightUpdaterModel.CheckIfFileIsHere(CurrentFileName, _downloadChecksum)) //compare checksum if download the file is needeed
                {

                    SetupProgressBar.Value = 0;
                    UpdaterCurrentTask.Text = "Downloading...";
                    Log.Instance.log("Downloading " + CurrentFileName, LogSeverity.Debug);
                    _webClient = new WebClient();
                    var uri = new Uri(_downloadURL);
                    _webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnDownloadProgressChanged);
                    _webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(OnDownloadComplete);
                    _webClient.DownloadFileAsync(uri, CurrentFileName); // Download the file
                }
            }
            else
            {
                Log.Instance.log("URL is incorrect, installation aborted", LogSeverity.Debug);
                MessageBox.Show("URL is incorrect, impossible to download the file, installation aborted!");
            }
        }

        private async void AsyncCheckGithub()
        {
            await CheckGithub();
        }
        private async Task CheckGithub()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    await client.GetAsync("https://github.com");
                    BoxGithub.Checked = true;
                    IfReadyForTakeOFF();
                }
            }
            catch
            {
                BoxGithub.Checked = false;
                IfReadyForTakeOFF();
            }
        }

        private void IfReadyForTakeOFF()
        {
            if (BoxGithub.Checked & BoxTargetDirectory.Checked)
            {
                SetupTitle.Text = "Clear to take off MF9";
                SetupTitle.ForeColor = System.Drawing.Color.White;
            }
            else
            {
                SetupTitle.Text = "Take OFF aborted, something wrong";
                SetupTitle.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
