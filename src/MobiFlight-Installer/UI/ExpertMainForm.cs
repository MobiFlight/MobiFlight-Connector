using System;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace MobiFlightInstaller.UI
{
    public partial class ExpertMainForm : Form
    {
        private string ActualInstalledVersion = "0.0.0";
        private string _downloadURL = "";
        private string _downloadChecksum = "";
        private string _downloadNotes = "";
        private string _tempPath;
        private WebClient _webClient;
        private string CurrentFileName = "";

        public ExpertMainForm()
        {
            InitializeComponent();
            
            if (MobiFlightUpdaterModel.InstallerUpdateUrl != "") // Check Installer update if url is config
                MobiFlightUpdaterModel.InstallerCheckForUpgrade(MobiFlightUpdaterModel.InstallerUpdateUrl);
            
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
            PopulateListVersions();
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
            if (MobiFlightUpdaterModel.CheckIfFileIsHere(Directory.GetCurrentDirectory() + "\\" + CurrentFileName, _downloadChecksum)) //compare checksum if download is correct
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
