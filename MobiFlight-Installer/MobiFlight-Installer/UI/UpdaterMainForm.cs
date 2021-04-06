using System;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;

namespace MobiFlightInstaller.UI
{
    public partial class UpdaterMainForm : Form
    {
        readonly string InstallerUpdateUrl = ""; // Set to empty to avoid installer autoUpgrade
        string ActualInstalledVersion = "0.0.0";
        
        private string _downloadURL = "";
        private string _downloadChecksum = "";
        private string _downloadNotes = "";
        private string _tempPath;
        private WebClient _webClient;
        private string CurrentFileName = "";

        public UpdaterMainForm()
        {
            
            Log.Instance.log("Installer start", LogSeverity.Debug);

            CmdLineParams cmdParams = new CmdLineParams(Environment.GetCommandLineArgs());

            var updateUrl = MobiFlightUpdaterModel.MobiFlightUpdateUrl;
            if (cmdParams.CacheId != null)
            {
                updateUrl += "?cache=" + cmdParams.CacheId;
            }

            if (cmdParams.HasParams()) // if args are present
            {
                MobiFlightUpdaterModel.DownloadVersionsList(updateUrl);

                // install specific version by command line
                if (cmdParams.Install != null)
                {
                    Log.Instance.log("ManualUpgradeFromCommandLine START from args -> Check BETA " + cmdParams.Install, LogSeverity.Debug);
                    MobiFlightUpdaterModel.ManualUpgradeFromCommandLine(cmdParams.Install);
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
                    InitializeComponent();
                    if (InstallerUpdateUrl != "")
                        MobiFlightUpdaterModel.InstallerCheckForUpgrade(InstallerUpdateUrl);
                    ActualInstalledVersion = MobiFlightUpdaterModel.GetInstalledVersion();
                    if (ActualInstalledVersion != "0.0.0")
                    {
                        label1.Text = String.Format(i18n.tr("CurrentlyInstalledVersion"), ActualInstalledVersion);
                        ButtonStartMF.Enabled = true;
                    }
                    else
                    {
                        label1.Text = i18n.tr("Choose_which_version_do_you_want_to_install");
                        ButtonStartMF.Enabled = false;
                    }
                    ButtonInstall.Enabled = false;
                    if (MobiFlightUpdaterModel.VerifyCurrentFolderRight())
                    {
                        MobiFlightUpdaterModel.DownloadVersionsList(updateUrl);
                        PopulateListVersions();
                    }
                    else
                    {
                        MessageBox.Show(i18n.tr("NotEnoughRightsForInstallation"));
                        Environment.Exit(0);
                    }
                } else
                {
                    Environment.Exit(0);
                }
            }
            else
            {
                Log.Instance.log("No args, SILENT mode enable", LogSeverity.Debug);
                MobiFlightUpdaterModel.DownloadVersionsList(updateUrl);
                bool IsMfHaveBetaEnable = MobiFlightHelperMethods.GetMfBetaOptionValue();
                var CurVersion = new Version(MobiFlightUpdaterModel.GetInstalledVersion());
                var TargetVersion = new Version(MobiFlightUpdaterModel.GetTheLastVersionNumberAvailable(IsMfHaveBetaEnable));
                var result = CurVersion.CompareTo(TargetVersion);
                if (result < 0) // direct install last release version if MF don't exist OR if a new version is available
                {
                    if (MobiFlightUpdaterModel.VerifyCurrentFolderRight())
                    {
                        MobiFlightUpdaterModel.DownloadVersionsList(updateUrl);
                        MobiFlightUpdaterModel.ManualUpgradeFromCommandLine(MobiFlightUpdaterModel.GetTheLastVersionNumberAvailable(IsMfHaveBetaEnable));
                    }
                    else
                    {
                        MessageBox.Show(i18n.tr("NotEnoughRightsForInstallation"));
                    }
                } else
                {
                    MobiFlightUpdaterModel.StartProcessAndClose(MobiFlightHelperMethods.ProcessName);
                }
                Environment.Exit(0);
            }
        }

        private void OpenNotes(string Url)
        {
            ShowNotes.Navigate(new Uri(Url));
        }

        private void DownloadSelectedPackage()
        {
            ButtonInstall.Enabled = false;
            StatusCurrent.Text = i18n.tr("Downloading");
            progressBar1.Value = 0;
            _webClient = new WebClient();
            var uri = new Uri(_downloadURL);
            CurrentFileName = MobiFlightUpdaterModel.GetFileName(_downloadURL);
            _tempPath = Directory.GetCurrentDirectory() + "\\" + CurrentFileName;
            if (!MobiFlightUpdaterModel.CheckIfFileIsHere(Directory.GetCurrentDirectory() + "\\" + CurrentFileName, _downloadChecksum)) //compare checksum if download the file is needeed
            {
                _webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnDownloadProgressChanged);
                _webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(OnDownloadComplete);
                _webClient.DownloadFileAsync(uri, _tempPath); // Download the file and extract in current directory
            }
            else // if zip file already here, installed it
            {
                Console.WriteLine(i18n.tr("InstallingPackage"));
                StatusCurrent.Text = i18n.tr("InstallingPackage");
                InstallPackage(CurrentFileName);
            }
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void OnDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (!MobiFlightUpdaterModel.CheckIfFileIsHere(Directory.GetCurrentDirectory() + "\\" + CurrentFileName, _downloadChecksum)) //compare checksum if download is correct
            {
                Console.WriteLine("Download Completed ! installing package ...");
                StatusCurrent.Text = "Download Completed ! installing package ...";
                InstallPackage(CurrentFileName);
                StatusCurrent.Text = "Package Installed !";
            }
            else
            {
                StatusCurrent.Text = "Download failed, checksum not match !";
            }
            _webClient.Dispose();
        }

        public void InstallPackage(string CurrentFileName)
        {
            if (Path.GetExtension(CurrentFileName) == ".zip")
            {
                MobiFlightUpdaterModel.CloseMobiFlightAndWait();
                MobiFlightUpdaterModel.GoExtractToDirectory(Directory.GetCurrentDirectory() + "\\" + CurrentFileName, Directory.GetCurrentDirectory());
                Console.WriteLine("Package is installed !");
                StatusCurrent.Text = "Package is installed !";
                ActualInstalledVersion = MobiFlightUpdaterModel.GetInstalledVersion();
                if (ActualInstalledVersion != "0.0.0")
                {
                    label1.Text = String.Format(i18n.tr("CurrentlyInstalledVersion"), ActualInstalledVersion);
                    ButtonStartMF.Enabled = true;
                }
                else
                {
                    label1.Text = i18n.tr("ChooseWhichVersionToInstall");
                    ButtonStartMF.Enabled = false;
                }
            }
            else if (Path.GetExtension(CurrentFileName) == ".exe")
            {
                MobiFlightUpdaterModel.StartProcessAndClose(CurrentFileName);
            }
            else
            {
                Console.WriteLine("Package Extension is not recognized. Only zip and exe files can be used.");
            }
        }

        private void PopulateListVersions()
        {
            ListVersions.Items.Clear();
            foreach (KeyValuePair<string, Dictionary<string, string>> a in MobiFlightUpdaterModel.resultList)
            {
                if (a.Value["beta"] == "yes")
                {
                    ListVersions.Items.AddRange(new object[] { a.Key + " Beta" });
                }
                else
                {
                    ListVersions.Items.AddRange(new object[] { a.Key });
                }
            }
            if (ListVersions.Items.Count > 0)
                ListVersions.SetSelected(0, true);
        }

        private void ButtonExit_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            MobiFlightUpdaterModel.DownloadVersionsList(MobiFlightUpdaterModel.MobiFlightUpdateUrl);
            PopulateListVersions();
        }

        private void ButtonInstall_Click(object sender, EventArgs e)
        {
            DownloadSelectedPackage();
        }

        private void ListVersions_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] VersionSelectIndexArray = ListVersions.SelectedItem.ToString().Split(' ');
            string VersionSelectIndex = VersionSelectIndexArray[0];
            _downloadURL = MobiFlightUpdaterModel.resultList[VersionSelectIndex]["url"];
            _downloadChecksum = MobiFlightUpdaterModel.resultList[VersionSelectIndex]["checksum"];
            _downloadNotes = MobiFlightUpdaterModel.resultList[VersionSelectIndex]["changelog"];
            ButtonInstall.Enabled = true;
            OpenNotes(_downloadNotes);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            MobiFlightUpdaterModel.StartProcessAndClose(MobiFlightHelperMethods.ProcessName);
        }
    }   
}
