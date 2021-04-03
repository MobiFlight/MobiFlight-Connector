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

namespace MobiFlightInstaller
{
    static class MobiFlightUpdaterModel
    {
        public static readonly string MobiFlightUpdateUrl = "https://www.mobiflight.com/tl_files/download/releases/mobiflightconnector-updates.xml";

        public static readonly string ProcessName = "MFConnector";
        public static readonly string OldMobiFlightUpdaterName = "MobiFlight-Updater.exe";
        public static readonly string OptionBetaEnableSearch = "/configuration/userSettings/MobiFlight.Properties.Settings/setting[@name='BetaUpdates']";
        
        public static string CacheId = null;

        static Char[] s_Base32Char = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h',
            'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
            'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
            'y', 'z', '0', '1', '2', '3', '4', '5'};

        static public Dictionary<string, Dictionary<string, string>> resultList = new Dictionary<string, Dictionary<string, string>>();

        static public bool CheckIfFileIsHere(string FileName, string ShaOne)
        {
            string FileChecksum = "";
            if (File.Exists(FileName))
            {
                using (FileStream fs = File.OpenRead(FileName))
                {
                    SHA1 sha = new SHA1Managed();
                    FileChecksum = BitConverter.ToString(sha.ComputeHash(fs)).Replace("-", "");
                }
                if (ShaOne == FileChecksum)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public static string GetInstalledVersion()
        {
            if (File.Exists(ProcessName + ".exe"))
            {
                return AssemblyName.GetAssemblyName(ProcessName + ".exe").Version.ToString();
            }
            else
            {
                return "0.0.0";
            }
        }

        public static bool VerifyCurrentFolderRight()
        {
            try
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\test");
                Directory.Delete(Directory.GetCurrentDirectory() + "\\test", true);
            }
            catch
            {
                return false;
            }
            return true;

        }

        public static void GoExtractToDirectory(string zipPath, string destinationDirectoryName)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry file in archive.Entries)
                {
                    string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                    if (!System.IO.Directory.Exists(Path.GetDirectoryName(completeFileName)))
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
                    if (file.Name != OldMobiFlightUpdaterName)
                    {
                        if (file.Name == "MobiFlight-Installer.exe")
                        {
                            System.IO.FileInfo FileInstaller = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                            if (File.Exists(FileInstaller.DirectoryName + "\\" + FileInstaller.Name.Replace(FileInstaller.Extension, ".old"))) 
                                System.IO.File.Delete(FileInstaller.DirectoryName + "\\" + FileInstaller.Name.Replace(FileInstaller.Extension, ".old"));

                            System.IO.File.Move(FileInstaller.FullName, FileInstaller.DirectoryName + "\\" + FileInstaller.Name.Replace(FileInstaller.Extension, ".old"));
                        }

                        try
                        {
                            file.ExtractToFile(completeFileName, true);
                        }
                        catch
                        {
                            Thread.Sleep(2000);
                            try
                            {
                                file.ExtractToFile(completeFileName, true);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show("Error installation failed -> " + e.Message);
                            }
                        }
                    }

                }
                archive.Dispose();
            }
        }

        public static string GetFileName(string url)
        {
            Uri uri = new Uri(url);
            string filename = System.IO.Path.GetFileName(uri.LocalPath);
            return filename;
        }

        public static void UpgradeMyselfAndRestart(string SourceFile)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            System.IO.File.Move(file.FullName, file.DirectoryName + "\\" + file.Name.Replace(file.Extension, ".old"));
            System.IO.File.Move(SourceFile, file.FullName);
            StartProcessAndClose(file.FullName);
        }

        public static void StartProcessAndClose(string ProcessName, string Args = "")
        {
            String ProcessEXEName = ProcessName + ".exe";

            if (File.Exists(Directory.GetCurrentDirectory() + "\\" + ProcessEXEName))
            {
                if (Args == "")
                {
                    Process.Start(Directory.GetCurrentDirectory() + "\\" + ProcessEXEName);
                }
                else
                {
                    Process.Start(Directory.GetCurrentDirectory() + "\\" + ProcessEXEName, Args);
                }
                Environment.Exit(0);
            }
        }

        public static void DownloadVersionsList(string UrlXmlFile)
        {
            WebRequest webRequest = WebRequest.Create(UrlXmlFile);
            webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            WebResponse webResponse;
            webResponse = webRequest.GetResponse();
            Stream ContentStream = webResponse.GetResponseStream();
            if (ContentStream != null)
            {
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
            else
            {
                MessageBox.Show("Error download Mobiflight list failed");
            }
        }

        public static void InstallerCheckForUpgrade(String InstallerUpdateUrl)
        {
            WebRequest webRequest = WebRequest.Create(InstallerUpdateUrl);
            webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            WebResponse webResponse;
            webResponse = webRequest.GetResponse();
            Stream ContentStream = webResponse.GetResponseStream();
            string LastFindVersion = "";
            string VersionDownloadURL = "";
            if (File.Exists(Directory.GetCurrentDirectory() + "\\MobiFlight-Installer.tmp"))
                System.IO.File.Delete(Directory.GetCurrentDirectory() + "\\MobiFlight-Installer.tmp");
            if (File.Exists(Directory.GetCurrentDirectory() + "\\MobiFlight-Installer.old"))
                System.IO.File.Delete(Directory.GetCurrentDirectory() + "\\MobiFlight-Installer.old");
            if (ContentStream != null)
            {
                var ReceivedContent = new XmlDocument();
                ReceivedContent.Load(ContentStream);
                XmlNode x = ReceivedContent.DocumentElement;
                foreach (XmlNode a in x)
                {
                    if (a.HasChildNodes)
                    {
                        LastFindVersion = a.Attributes["version"].Value;
                        foreach (XmlNode b in a)
                        {
                            if (b.Name == "url")
                            {
                                VersionDownloadURL = b.InnerText;
                            }
                        }
                    }
                    break;
                }
                if ((LastFindVersion != "") & (VersionDownloadURL != ""))
                {
                    var CurVersion = new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    var TargetVersion = new Version(LastFindVersion);
                    var result = CurVersion.CompareTo(TargetVersion);
                    if (result < 0) //Target is greater than current
                    {
                        WebClient I_webClient = new WebClient();
                        var uri = new Uri(VersionDownloadURL);
                        if (File.Exists(Directory.GetCurrentDirectory() + "\\MobiFlight-Installer.tmp"))
                            System.IO.File.Delete(Directory.GetCurrentDirectory() + "\\MobiFlight-Installer.tmp");
                        if (File.Exists(Directory.GetCurrentDirectory() + "\\MobiFlight-Installer.old"))
                            System.IO.File.Delete(Directory.GetCurrentDirectory() + "\\MobiFlight-Installer.old");
                        string I_tempPath = Directory.GetCurrentDirectory() + "\\MobiFlight-Installer.tmp";
                        I_webClient.DownloadFile(uri, I_tempPath);
                        I_webClient.Dispose();
                        UpgradeMyselfAndRestart(I_tempPath);
                    }
                }

            }
            else
            {
                MessageBox.Show("ERROR : Impossible to connect to the server...");
            }
        }

        static public void ManualUpgradeFromCommandLine(string Version)
        {
            if (resultList[Version]["url"].Length > 5)
            {
                String _downloadURL = resultList[Version]["url"];
                String _downloadChecksum = resultList[Version]["checksum"];
                WebClient _webClient = new WebClient();
                var uri = new Uri(_downloadURL);

                String CurrentFileName = MobiFlightUpdaterModel.GetFileName(_downloadURL);
                String _tempPath = Directory.GetCurrentDirectory() + "\\" + CurrentFileName;
                if (!MobiFlightUpdaterModel.CheckIfFileIsHere(Directory.GetCurrentDirectory() + "\\" + CurrentFileName, _downloadChecksum)) //compare checksum if download the file is needeed
                {
                    _webClient.DownloadFile(uri, _tempPath); // Download the file and extract in current directory
                    _webClient.Dispose();
                    if (MobiFlightUpdaterModel.CheckIfFileIsHere(Directory.GetCurrentDirectory() + "\\" + CurrentFileName, _downloadChecksum)) //compare checksum if download is correct
                    {
                        CloseMobiFlightAndWait();
                        MobiFlightUpdaterModel.GoExtractToDirectory(Directory.GetCurrentDirectory() + "\\" + CurrentFileName, Directory.GetCurrentDirectory());
                        MobiFlightUpdaterModel.StartProcessAndClose(ProcessName);
                    }
                    else
                    {
                        MobiFlightUpdaterModel.StartProcessAndClose(ProcessName);
                    }
                }
                else // if zip file already here, installed it
                {
                    CloseMobiFlightAndWait();
                    MobiFlightUpdaterModel.GoExtractToDirectory(Directory.GetCurrentDirectory() + "\\" + CurrentFileName, Directory.GetCurrentDirectory());
                }
                MobiFlightUpdaterModel.StartProcessAndClose(ProcessName);
            }
            else
            {
                MessageBox.Show("Impossible to find this version, install canceled...");
            }
        }

        static public string GetTheLastVersionNumberAvailable(bool IncludeBeta = false)
        {
            foreach (KeyValuePair<string, Dictionary<string, string>> a in resultList)
            {
                if (IncludeBeta)
                {
                    Log.Instance.log("GetTheLastVersionNumberAvailable -> " + a.Key, LogSeverity.Info);
                    return a.Key;
                }
                else
                {
                    if (a.Value["beta"] == "no")
                    {
                        Log.Instance.log("GetTheLastVersionNumberAvailable -> " + a.Key, LogSeverity.Info);
                        return a.Key;
                    }
                }
            }
            Log.Instance.log("GetTheLastVersionNumberAvailable -> Can't find any version", LogSeverity.Info);
            return "0.0.0";
        }
        public static void CloseMobiFlightAndWait()
        {
            var Processes = Process.GetProcesses().Where(pr => pr.ProcessName == ProcessName);
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

        static public bool GetMfBetaOptionValue()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "\\" + ProcessName))
            {
                string PatchConfigFile = GetExeLocalAppDataUserConfigPath(Directory.GetCurrentDirectory() + "\\" + ProcessName);
                Log.Instance.log("Check BETA option in " + PatchConfigFile, LogSeverity.Info);
                if (File.Exists(PatchConfigFile))
                {
                    bool result = ExtractConfigBetaValueFromXML(PatchConfigFile);
                    if (result) Log.Instance.log("BETA enable", LogSeverity.Info);
                    else Log.Instance.log("BETA disable", LogSeverity.Info);
                    return result;
                }
                else
                {
                    Log.Instance.log("Impossible to read the file does not exist -> BETA disable", LogSeverity.Info);
                    return false;
                }
            }
            else
            {
                Log.Instance.log("MFConnector.exe not found, BETA disable", LogSeverity.Info);
                return false;
            }
        }

        static public bool ExtractConfigBetaValueFromXML(string PatchConfigFile)
        {
            string xmlFile = File.ReadAllText(@PatchConfigFile);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xmlFile);
            XmlNodeList nodeList = xmldoc.SelectNodes(OptionBetaEnableSearch);
            string Result = string.Empty;
            foreach (XmlNode node in nodeList)
            {
                Result = node.InnerText;
            }
            if (Result == "True") return true;
            else return false;
        }
        static public string GetExeLocalAppDataUserConfigPath(string fullExePath)
        {
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var versionInfo = FileVersionInfo.GetVersionInfo(fullExePath);
            var companyName = versionInfo.CompanyName;
            var exeName = versionInfo.OriginalFilename;

            var assemblyName = AssemblyName.GetAssemblyName(fullExePath);
            var version = assemblyName.Version.ToString();

            var uri = "file:///" + fullExePath; 
            uri = uri.ToUpperInvariant();

            var ms = new MemoryStream();
            var bSer = new BinaryFormatter();
            bSer.Serialize(ms, uri);
            ms.Position = 0;
            var sha1 = new SHA1CryptoServiceProvider();
            var hash = sha1.ComputeHash(ms);
            var hashstring = ToBase32StringSuitableForDirName(hash);

            var userConfigLocalAppDataPath = Path.Combine(localAppDataPath, companyName, exeName + "_Url_" + hashstring, version, "user.config");

            return userConfigLocalAppDataPath;
        }
        private static string ToBase32StringSuitableForDirName(byte[] buff)
        {
            StringBuilder sb = new StringBuilder();
            byte b0, b1, b2, b3, b4;
            int l, i;

            l = buff.Length;
            i = 0;

            // Create l chars using the last 5 bits of each byte.  
            // Consume 3 MSB bits 5 bytes at a time.

            do
            {
                b0 = (i < l) ? buff[i++] : (byte)0;
                b1 = (i < l) ? buff[i++] : (byte)0;
                b2 = (i < l) ? buff[i++] : (byte)0;
                b3 = (i < l) ? buff[i++] : (byte)0;
                b4 = (i < l) ? buff[i++] : (byte)0;

                // Consume the 5 Least significant bits of each byte
                sb.Append(s_Base32Char[b0 & 0x1F]);
                sb.Append(s_Base32Char[b1 & 0x1F]);
                sb.Append(s_Base32Char[b2 & 0x1F]);
                sb.Append(s_Base32Char[b3 & 0x1F]);
                sb.Append(s_Base32Char[b4 & 0x1F]);

                // Consume 3 MSB of b0, b1, MSB bits 6, 7 of b3, b4
                sb.Append(s_Base32Char[(
                    ((b0 & 0xE0) >> 5) |
                    ((b3 & 0x60) >> 2))]);

                sb.Append(s_Base32Char[(
                    ((b1 & 0xE0) >> 5) |
                    ((b4 & 0x60) >> 2))]);

                // Consume 3 MSB bits of b2, 1 MSB bit of b3, b4

                b2 >>= 5;

                if ((b3 & 0x80) != 0)
                    b2 |= 0x08;
                if ((b4 & 0x80) != 0)
                    b2 |= 0x10;

                sb.Append(s_Base32Char[b2]);

            } while (i < l);

            return sb.ToString();
        }
    }
}
