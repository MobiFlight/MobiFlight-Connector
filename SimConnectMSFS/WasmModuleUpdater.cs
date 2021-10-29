using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.SimConnectMSFS
{
    public class WasmModuleUpdater
    {
        public const String WasmModuleFolder = @".\MSFS2020-module\mobiflight-event-module";
        
        public const String WasmEventsTxtUrl = @"https://raw.githubusercontent.com/Mobiflight/MobiFlight-Connector/main/MSFS2020-module/mobiflight-event-module/modules/events.txt";    
        public const String WasmEventsCipUrl = @"https://raw.githubusercontent.com/Mobiflight/MobiFlight-Connector/main/Presets/msfs2020_eventids.cip";
        public const String WasmEventsTxtFolder = @"mobiflight-event-module\modules";
        public const String WasmEventsTxtFile = "events.txt";
        public const String WasmEventsCipFolder = @".\presets";
        
        public String CommunityFolder { get; set; }

        private String ExtractCommunityFolderFromUserCfg(String UserCfg)
        {
            string CommunityFolder = null;
            string line;
            string InstalledPackagesPath = "";
            StreamReader file = new StreamReader(UserCfg);

            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains("InstalledPackagesPath"))
                {
                    InstalledPackagesPath = line;
                }
            }

            if (InstalledPackagesPath == "")
                return CommunityFolder;

            InstalledPackagesPath = InstalledPackagesPath.Substring(23);
            char[] charsToTrim = { '"' };
            
            InstalledPackagesPath = InstalledPackagesPath.TrimEnd(charsToTrim);
            
            if (Directory.Exists(InstalledPackagesPath + @"\Community"))
            {
                CommunityFolder = InstalledPackagesPath + @"\Community";
            }

            return CommunityFolder;
        }
        public bool AutoDetectCommunityFolder()
        {
            string searchpath = Environment.GetEnvironmentVariable("AppData") + @"\Microsoft Flight Simulator\UserCfg.opt";

            if (!File.Exists(searchpath))
            {
                searchpath = Environment.GetEnvironmentVariable("LocalAppData") + @"\Packages\Microsoft.FlightSimulator_8wekyb3d8bbwe\LocalCache\UserCfg.opt";
                if (!File.Exists(searchpath))
                {
                    return false;
                }
            }

            CommunityFolder = ExtractCommunityFolderFromUserCfg(searchpath);
            return true;
        }

        public bool InstallWasmModule()
        {
            if (!Directory.Exists(WasmModuleFolder))
            {
                Log.Instance.log("WASM module cannot be installed. WASM Module Folder not found. " + WasmModuleFolder, LogSeverity.Error);
                return false;
            }

            if (!Directory.Exists(CommunityFolder))
            {
                Log.Instance.log("WASM module cannot be installed. Community Folder not found. " + CommunityFolder, LogSeverity.Error);
                return false;
            }

            String destFolder = CommunityFolder + @"\mobiflight-event-module";
            CopyFolder(new DirectoryInfo(WasmModuleFolder), new DirectoryInfo(destFolder));

            Log.Instance.log("WASM module has been installed successfully.", LogSeverity.Info);
            return true;
        }

        public static void CopyFolder(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyFolder(diSourceSubDir, nextTargetSubDir);
            }
        }

        static string CalculateMD5(string filename)
        {
            var md5 = MD5.Create();
            using (var stream = File.OpenRead(filename))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }   
        }

        public bool WasmModulesAreDifferent()
        {
            Console.WriteLine("Check if WASM module needs to be updated");

            string installedWASM;
            string mobiflightWASM;
            string installedEvents;
            string mobiflightEvents;

            if (CommunityFolder == null) return true;


            if (!File.Exists(CommunityFolder + @"\mobiflight-event-module\modules\StandaloneModule.wasm"))
                return true;

            installedWASM = CalculateMD5(CommunityFolder + @"\mobiflight-event-module\modules\StandaloneModule.wasm");
            mobiflightWASM = CalculateMD5(@".\MSFS2020-module\mobiflight-event-module\modules\StandaloneModule.wasm");

            installedEvents = CalculateMD5(CommunityFolder + @"\mobiflight-event-module\modules\events.txt");
            mobiflightEvents = CalculateMD5(@".\MSFS2020-module\mobiflight-event-module\modules\events.txt");

            return (installedWASM != mobiflightWASM || installedEvents != mobiflightEvents);
        }

        public bool InstallWasmEvents()
        {
            String destFolder = Path.Combine(CommunityFolder, WasmEventsTxtFolder);

            if (!Directory.Exists(destFolder))
            {
                Log.Instance.log("WASM events cannot be installed. WASM module folder not found. " + destFolder, LogSeverity.Error);
                return false;
            }

            if (!Directory.Exists(WasmModuleFolder))
            {
                Log.Instance.log("WASM events cannot be installed. WASM module folder not found. " + WasmModuleFolder, LogSeverity.Error);
                return false;
            }

            if (!Directory.Exists(CommunityFolder))
            {
                Log.Instance.log("WASM events cannot be installed. Community folder not found. " + CommunityFolder, LogSeverity.Error);
                return false;
            }

            if (!DownloadWasmEvents())
            {
                Log.Instance.log("WASM events cannot be installed. Download was not successful. " + CommunityFolder, LogSeverity.Error);
                return false;
            }

            String sourceFile = Path.Combine(WasmModuleFolder, "modules", WasmEventsTxtFile);
            String destFile = Path.Combine(destFolder, WasmEventsTxtFile);

            try
            {
                System.IO.File.Delete(destFile + ".bak");

                if (System.IO.File.Exists(destFile))
                    System.IO.File.Move(destFile, destFile + ".bak");

                System.IO.File.Copy(sourceFile, destFile);
            }
            catch (Exception ex)
            {
                Log.Instance.log("WASM events cannot be installed. " + ex.Message, LogSeverity.Error);
                return false;
            }

            Log.Instance.log("WASM events have been installed successfully.", LogSeverity.Info);
            return true;
        }

        public bool DownloadWasmEvents()
        {
            if(!DownloadSingleFile(new Uri(WasmEventsTxtUrl), WasmModuleFolder + @"\modules")) return false;
            Log.Instance.log("WASM events.txt has been downloaded and installed successfully.", LogSeverity.Info);

            if (!DownloadSingleFile(new Uri(WasmEventsCipUrl), WasmEventsCipFolder)) return false;
            Log.Instance.log("WASM msfs2020_eventids.cip has been downloaded and installed successfully.", LogSeverity.Info);
            
            return true;
        }

        private bool DownloadSingleFile(Uri uri, String targetPath)
        {
            var filename = System.IO.Path.GetFileName(uri.LocalPath);

            SecurityProtocolType oldType = System.Net.ServicePointManager.SecurityProtocol;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebClient webClient = new WebClient();
            string tmpFile = Directory.GetCurrentDirectory() + targetPath + @"\" + filename + ".tmp";
            webClient.DownloadFile(uri, tmpFile);
            webClient.Dispose();
            System.IO.File.Delete(targetPath + @"\" + filename);
            System.IO.File.Move(tmpFile, targetPath + @"\" + filename);

            System.Net.ServicePointManager.SecurityProtocol = oldType;
            return true;
        }
    }
}
