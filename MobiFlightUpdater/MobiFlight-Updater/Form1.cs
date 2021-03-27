using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Security.Cryptography;

namespace MobiFlight_Updater
{
    public partial class UpdaterForm : Form
    {
        readonly string InstallerUpdateUrl = ""; // Set to empty to avoid installer autoUpgrade
        //readonly string MobiFlightUpdateUrlRelease = "https://connectoo.fr/mobiflight_zip_list.xml";
        public static readonly string MobiFlightUpdateUrl = "https://www.mobiflight.com/tl_files/download/releases/mobiflightconnector.xml";
        public static readonly string MobiFlightUpdateBetasUrl = "https://www.mobiflight.com/tl_files/download/releases/beta/mobiflightconnector.xml";
        public static readonly string MobiFlightUpdateDebugUrl = "https://www.mobiflight.com/tl_files/download/releases/debug/mobiflightconnector.xml";
        string ActualInstalledVersion = "0.0.0";
        
        private string _downloadURL = "";
        private string _downloadChecksum = "";
        private string _downloadNotes = "";
        private string _tempPath;
        private WebClient _webClient;
        private string CurrentFileName = "";

        public UpdaterForm()
        {
            CmdLineParams cmdParams = new CmdLineParams(Environment.GetCommandLineArgs());

            // You can use this for static testing.
            cmdParams = new CmdLineParams(new []{ "MobiFlightUpdater.exe", 
                                                  "/expert" });

            if (cmdParams.HasParams()) // if args are present
            {
                MobiFlightUpdaterModel.DownloadVersionsList(MobiFlightUpdateUrl);

                // install specific version by command line
                if (cmdParams.Install != null) 
                {
                    MobiFlightUpdaterModel.ManualUpgradeFromCommandLine(cmdParams.Install);
                }
                else if (cmdParams.Check || cmdParams.CheckBeta)
                {
                    MobiFlightUpdaterModel.ExternAskToCheckLastVersion(cmdParams.CheckBeta);
                }
                // expert mode, start interface to choose version
                else if (cmdParams.ExpertMode) 
                {
                    InitializeComponent();
                    if (InstallerUpdateUrl != "")
                        MobiFlightUpdaterModel.InstallerCheckForUpgrade(InstallerUpdateUrl);
                    ActualInstalledVersion = MobiFlightUpdaterModel.GetActualInstalledVersion();
                    if (ActualInstalledVersion != "0.0.0")
                    {
                        label1.Text = "MobiFlight " + ActualInstalledVersion + " is actually installed !";
                        ButtonStartMF.Enabled = true;
                    }
                    else
                    {
                        label1.Text = "Choose wich version do you want to install";
                        ButtonStartMF.Enabled = false;
                    }
                    ButtonInstall.Enabled = false;
                    if (MobiFlightUpdaterModel.VerifyCurrentFolderRight())
                    {
                        MobiFlightUpdaterModel.DownloadVersionsList(MobiFlightUpdateUrl);
                        PopulateListVersions();
                    }
                    else
                    {
                        MessageBox.Show("You don't have right on this folder, Mobiflight can't install here !");
                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                if (!File.Exists(MobiFlightUpdaterModel.ProcessName)) // direct install last release version if MF don't exist.
                {
                    if (MobiFlightUpdaterModel.VerifyCurrentFolderRight())
                    {
                        MobiFlightUpdaterModel.DownloadVersionsList(MobiFlightUpdateUrl);
                        MobiFlightUpdaterModel.ManualUpgradeFromCommandLine(MobiFlightUpdaterModel.GetTheLastVersionNumberAvailable());
                    }
                    else
                    {
                        MessageBox.Show("You don't have right on this folder, Mobiflight can't install here !");
                    }
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
            StatusCurrent.Text = "Downloading ...";
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
                Console.WriteLine("Installing package ...");
                StatusCurrent.Text = "Installing package ...";
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
                MobiFlightUpdaterModel.CloseMobiflightAndWait();
                MobiFlightUpdaterModel.GoExtractToDirectory(Directory.GetCurrentDirectory() + "\\" + CurrentFileName, Directory.GetCurrentDirectory());
                Console.WriteLine("Package is installed !");
                StatusCurrent.Text = "Package is installed !";
                ActualInstalledVersion = MobiFlightUpdaterModel.GetActualInstalledVersion();
                if (ActualInstalledVersion != "0.0.0")
                {
                    label1.Text = "MobiFlight " + ActualInstalledVersion + " is actually installed !";
                    ButtonStartMF.Enabled = true;
                }
                else
                {
                    label1.Text = "Choose wich version do you want to install";
                    ButtonStartMF.Enabled = false;
                }
            }
            else if (Path.GetExtension(CurrentFileName) == ".exe")
            {
                MobiFlightUpdaterModel.StartProcessAndClose(CurrentFileName);
            }
            else
            {
                Console.WriteLine("Extention Package is not reconize");
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
            MobiFlightUpdaterModel.DownloadVersionsList(MobiFlightUpdateUrl);
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
            MobiFlightUpdaterModel.StartProcessAndClose(MobiFlightUpdaterModel.ProcessName);
        }
    }   
}
