using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;

namespace MobiFlightInstaller
{
    static class MobiFlightUpdaterModel
    {
        public static readonly string MobiFlightUpdateUrl = "https://www.mobiflight.com/tl_files/download/releases/mobiflightconnector-updates.xml";
        public static readonly string OldMobiFlightUpdaterName = "MobiFlight-Updater.exe";
        public static readonly string InstallerLogFilePath = Directory.GetCurrentDirectory() + "\\install.log.txt";

        public static string InstallerUpdateUrl = ""; // URL to check for installer upgrade, Set to empty to avoid installer autoUpgrade
        public static string InstallerActualVersion = "0.0.0";
        public static int RequestTimeoutInMilliseconds = 5000;
        public static bool InstallOnly = false;
        
        public static string CacheId = null;

        static public Dictionary<string, Dictionary<string, string>> resultList = new Dictionary<string, Dictionary<string, string>>();

        static public void DeleteLogFileIfIsTooBig()
        {
            if (File.Exists(InstallerLogFilePath))
            {
                long LogLength = new System.IO.FileInfo(InstallerLogFilePath).Length;
                if (LogLength > 100000)
                {
                    System.IO.File.Delete(InstallerLogFilePath);
                }
            }
        }
        static public bool CheckIfFileIsHere(string FileName, string ShaOne)
        {
            string FileChecksum = "";

            if (!File.Exists(FileName))
            {
                Log.Instance.log("CheckIfFileIsHere : " + FileName + " -> Does not exist", LogSeverity.Debug);
                return false;
            }


            using (FileStream fs = File.OpenRead(FileName))
            {
                SHA1 sha = new SHA1Managed();
                FileChecksum = BitConverter.ToString(sha.ComputeHash(fs)).Replace("-", "");
            }


            if (ShaOne != FileChecksum)
            {
                Log.Instance.log("CheckIfFileIsHere : " + FileName + " -> Checksum missmatch", LogSeverity.Debug);
                return false;
            }
            
            Log.Instance.log("CheckIfFileIsHere : " + FileName + " -> Is already downloaded, checksum are equals", LogSeverity.Debug);
            return true; 
        }

        public static string GetInstalledVersion()
        {
            if (!File.Exists(MobiFlightHelperMethods.ProcessName + ".exe"))
            {
                Log.Instance.log("GetInstalledVersion : MFConnector does not exist ! return 0.0.0", LogSeverity.Debug);
                return "0.0.0";
            }

            string ReturnResult = AssemblyName.GetAssemblyName(MobiFlightHelperMethods.ProcessName + ".exe").Version.ToString();
            Log.Instance.log("GetInstalledVersion : detected -> " + ReturnResult, LogSeverity.Debug);
            return ReturnResult;
        }

        public static bool VerifyCurrentFolderRight()
        {
            if (InstallerHaveAdministratorRight())
                Log.Instance.log("InstallerHaveAdministratorRight : True", LogSeverity.Debug);
            else
                Log.Instance.log("InstallerHaveAdministratorRight : False", LogSeverity.Debug);
            try
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\test");
                Directory.Delete(Directory.GetCurrentDirectory() + "\\test", true);
            }
            catch
            {
                Log.Instance.log("VerifyCurrentFolderRight : ERROR Create directory test FAILED !!!", LogSeverity.Debug);
                return false;
            }
            Log.Instance.log("VerifyCurrentFolderRight : OK", LogSeverity.Debug);
            if (HaveWriteAccessToFolder())
            {
                return true;
            }
            return false;
        }

        private static bool HaveWriteAccessToFolder()
        {
            try
            {
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(Directory.GetCurrentDirectory());
                Log.Instance.log("HaveWriteAccessToFolder : True", LogSeverity.Debug);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                Log.Instance.log("HaveWriteAccessToFolder : False", LogSeverity.Debug);
                return false;
            }
        }
        public static bool InstallerHaveAdministratorRight()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void GoExtractToDirectory(string zipPath, string destinationDirectoryName)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry file in archive.Entries)
                {
                    string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                    if (!Directory.Exists(Path.GetDirectoryName(completeFileName)))
                    {
                        try
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Error installation failed -> " + e.Message);
                        }
                    }

                    // ignore the old update file.
                    if (file.Name == OldMobiFlightUpdaterName) continue;

                    if (file.Name == "MobiFlight-Installer.exe")
                    {
                        if (InstallerIsNewer(file)) // NewInstaller have greater version than current
                        {
                            var fileInstaller = new FileInfo(Assembly.GetExecutingAssembly().Location);
                            var backupFile = Path.Combine(fileInstaller.DirectoryName, fileInstaller.Name.Replace(fileInstaller.Extension, ".old"));

                            if (File.Exists(backupFile))
                                File.Delete(backupFile);

                            File.Move(fileInstaller.FullName, backupFile);
                        }
                        else
                        {
                            Log.Instance.log("Mobiflight-Installer no need to be upgrade, extracting file skipped", LogSeverity.Info);
                            continue;
                        }
                    }

                    if (file.Name == "install.log.txt")
                    {
                        continue;
                    }

                    try
                    {
                        Log.Instance.log("EXTRACTING : " + completeFileName, LogSeverity.Debug);
                        file.ExtractToFile(completeFileName, true);
                    }
                    catch
                    {
                        Log.Instance.log("Extracting failed ! : " + completeFileName + " -> Wait and try a second time ...", LogSeverity.Debug);
                        Thread.Sleep(2000);
                        try
                        {
                            Log.Instance.log("EXTRACTING (second try) : " + completeFileName, LogSeverity.Debug);
                            file.ExtractToFile(completeFileName, true);
                        }
                        catch (Exception e)
                        {
                            Log.Instance.log("FAILED to extract the file, installation FAILED !", LogSeverity.Debug);
                            MessageBox.Show("Error installation failed -> " + e.Message);
                        }
                    }
                }
                archive.Dispose();
            }
        }

        private static bool InstallerIsNewer(ZipArchiveEntry file)
        {
            var InstallerCurVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Log.Instance.log("Current Installer version : " + InstallerCurVersion, LogSeverity.Info);

            var fileInstaller = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var fileNew = Path.Combine(fileInstaller.DirectoryName, fileInstaller.Name.Replace(fileInstaller.Extension, ".new"));

            file.ExtractToFile(fileNew, true);

            var newInstallerVersion = AssemblyName.GetAssemblyName(fileNew).Version.ToString();
            
            File.Delete(fileNew);
            Log.Instance.log("New Installer version : " + newInstallerVersion, LogSeverity.Info);

            var InstallerCompareVersion = InstallerCurVersion.CompareTo(newInstallerVersion);

            return (InstallerCompareVersion < 0);
        }

        public static string GetFileName(string url)
        {
            Uri uri = new Uri(url);
            string filename = System.IO.Path.GetFileName(uri.LocalPath);
            return filename;
        }

        public static void StartProcessAndClose(string ProcessName, string Args = "")
        {
            var mobiFlightConnector = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, $"{ProcessName}.exe");

            if (!File.Exists(mobiFlightConnector))
                return;

            if (!InstallOnly)
                Process.Start(mobiFlightConnector, Args);

            Environment.Exit(0);
        }

        public static void DownloadVersionsList(string UrlXmlFile)
        {
            WebRequest webRequest = WebRequest.Create(UrlXmlFile);
            webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            webRequest.Timeout = RequestTimeoutInMilliseconds;

            WebResponse webResponse;
            Stream ContentStream = null;

            try
            {
                webResponse = webRequest.GetResponse();
                ContentStream = webResponse.GetResponseStream();
            }
            catch (WebException e)
            {
                _handleWebException(e);
                return;
            }

            if (ContentStream == null)
            {
                MessageBox.Show("Error download Mobiflight list failed");
                return;
            }

            var ReceivedContent = new XmlDocument();
            ReceivedContent.Load(ContentStream);
            XmlNode x = ReceivedContent.DocumentElement;

            foreach (XmlNode a in x)
            {
                if (a.HasChildNodes)
                {
                    resultList[a.Attributes["version"].Value] = new Dictionary<string, string>();
                    foreach (XmlNode b in a)
                    {
                        resultList[a.Attributes["version"].Value][b.Name] = b.InnerText;
                    }

                }
            }   
        }

        private static void _handleWebException(WebException e)
        {
            var reason = "";
            // e.g. request timed out
            if (e.Status == WebExceptionStatus.Timeout)
                reason = " due to timeout";
            Log.Instance.log($"Download FAILED{reason}, probably a connection error. ({e.Status.ToString()})", LogSeverity.Error);
        }

        static public string GetTheLastVersionNumberAvailable(bool IncludeBeta = false)
        {
            foreach (KeyValuePair<string, Dictionary<string, string>> a in resultList)
            {
                if (IncludeBeta)
                {
                    Log.Instance.log("GetTheLastVersionNumberAvailable -> " + a.Key, LogSeverity.Debug);
                    return a.Key;
                }
                else
                {
                    if (a.Value["beta"] == "no")
                    {
                        Log.Instance.log("GetTheLastVersionNumberAvailable -> " + a.Key, LogSeverity.Debug);
                        return a.Key;
                    }
                }
            }
            Log.Instance.log("GetTheLastVersionNumberAvailable -> Can't find any version", LogSeverity.Debug);
            return "0.0.0";
        }
        public static void CloseMobiFlightAndWait()
        {
            var Processes = Process.GetProcesses().Where(pr => pr.ProcessName == MobiFlightHelperMethods.ProcessName);
            foreach (var Process in Processes)
            {
                Process.CloseMainWindow();
                Process.WaitForExit();
            }
        }

        static public void ExternAskToCheckLastVersion(bool NeedCheckBeta = false)
        {
            string LastVersionAvailable = MobiFlightUpdaterModel.GetTheLastVersionNumberAvailable(NeedCheckBeta);
            var CurVersion = new Version(MobiFlightUpdaterModel.GetInstalledVersion());
            var TargetVersion = new Version(LastVersionAvailable);
            var result = CurVersion.CompareTo(TargetVersion);
            if (result < 0)
            {
                Console.WriteLine("##RESULT##|1|" + LastVersionAvailable + "|");
            }
            else
            {
                Console.WriteLine("##RESULT##|0|");
            }
            Environment.Exit(0);
        }
    }
}
