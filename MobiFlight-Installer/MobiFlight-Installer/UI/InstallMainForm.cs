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
using System.Reflection;

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
            if (!IsOfflineInstallOK())
            {
                BoxInstallOffline.Hide();
                BoxInstallFromInternet.Hide();
                Log.Instance.log("SETUP : OFFLINE install is NOK, disable it", LogSeverity.Debug);
            }
            BoxTargetDirectory.Checked = MobiFlightUpdaterModel.VerifyCurrentFolderRight();
            IfReadyForTakeOFF();
        }

        private void button1_Click(object sender, EventArgs e)
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

        private void button2_Click(object sender, EventArgs e) // Start installation
        {
            if (BoxInstallOffline.Checked) // install Offline
            {
                PathToInstall = SelectedPath.Text;
                MobiFlightUpdaterModel.SetInstallPath(PathToInstall);
                Log.Instance.log("SETUP : Start new OFFLINE installation to " + SelectedPath.Text, LogSeverity.Debug);
                OfflineInstallation();
            }
            else // Install from internet
            {
                Log.Instance.log("SETUP : Start new installation from internet to " + SelectedPath.Text, LogSeverity.Debug);
                var TargetVersion = new Version(MobiFlightUpdaterModel.GetTheLastVersionNumberAvailable(false));
                PathToInstall = SelectedPath.Text;
                MobiFlightUpdaterModel.SetInstallPath(PathToInstall);
                ManualUpgradeFromCommandLine(TargetVersion.ToString());
            }
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

        private void OfflineInstallation()
        {
            var resourceName = "";
            var assembly = Assembly.GetExecutingAssembly();
            string[] rlist = assembly.GetManifestResourceNames();
            foreach (string value in rlist)
            {
                if (value.Contains("MobiFlightConnector-"))
                {
                    resourceName = value;
                }
            }
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            MobiFlightUpdaterModel.CloseMobiFlightAndWait();
            MobiFlightUpdaterModel.GoExtractToDirectoryFromStream(stream, PathToInstall);
            UpdaterCurrentTask.Text = "Start MobiFlight";
            MobiFlightUpdaterModel.StartProcessAndClose(MobiFlightHelperMethods.ProcessName);
        }

        private bool IsOfflineInstallOK()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string[] rlist = assembly.GetManifestResourceNames();
            foreach (string value in rlist)
            {
                if (value.Contains("MobiFlightConnector-"))
                {
                    return true;
                }
            }
            return false;
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

        private void BoxInstallFromInternet_CheckedChanged(object sender, EventArgs e)
        {
            if (BoxInstallFromInternet.Checked)
            {
                BoxInstallOffline.Checked = false;
            }
            
        }

        private void BoxInstallOffline_CheckedChanged(object sender, EventArgs e)
        {
            if (BoxInstallOffline.Checked)
            {
                BoxInstallFromInternet.Checked = false;
            }
        }
    }
}
