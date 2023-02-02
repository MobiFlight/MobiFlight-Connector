using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        
        public const String WasmEventsTxtUrl = @"https://hubhop-api-mgtm.azure-api.net/api/v1/export/presets?type=wasm";
        public const String WasmEventsTxtFolder = @"mobiflight-event-module\modules";
        public const String WasmEventsTxtFile = "events.txt";

        public const String WasmEventsCipUrl = @"https://hubhop-api-mgtm.azure-api.net/api/v1/export/presets?type=cip";
        public const String WasmEventsCipFolder = @".\presets";
        public const String WasmEventsCipFileName = @"msfs2020_eventids.cip";

        public const String WasmEventsSimVarsUrl = @"https://hubhop-api-mgtm.azure-api.net/api/v1/export/presets?type=simVars";
        public const String WasmEventsSimVarsFolder = @".\presets";
        public const String WasmEventsSimVarsFileName = @"msfs2020_simvars.cip";

        public const String WasmEventHubHHopUrl = @"https://hubhop-api-mgtm.azure-api.net/api/v1/msfs2020/presets?type=json";
        public const String WasmEventsHubHopFolder = @".\presets";
        public const String WasmEventsHubHopFileName = @"msfs2020_hubhop_presets.json";

        public const String WasmEventsXplaneHubHHopUrl = @"https://hubhop-api-mgtm.azure-api.net/api/v1/xplane/presets?type=json";
        public const String WasmEventsXplaneHubHopFileName = @"xplane_hubhop_presets.json";

        public const String WasmModuleName = @"MobiFlightWasmModule.wasm";
        public const String WasmModuleNameOld = @"StandaloneModule.wasm";

        public event EventHandler<ProgressUpdateEvent> DownloadAndInstallProgress;

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
                Log.Instance.log($"WASM module cannot be installed. WASM module folder {WasmModuleFolder} not found.", LogSeverity.Error);
                return false;
            }

            if (!Directory.Exists(CommunityFolder))
            {
                Log.Instance.log($"WASM module cannot be installed. Community folder {CommunityFolder} not found.", LogSeverity.Error);
                return false;
            }

            String destFolder = CommunityFolder + @"\mobiflight-event-module";
            CopyFolder(new DirectoryInfo(WasmModuleFolder), new DirectoryInfo(destFolder));

            // Remove the old Wasm File
            DeleteOldWasmFile();

            Log.Instance.log("WASM module has been installed successfully.", LogSeverity.Info);
            return true;
        }

        private void DeleteOldWasmFile()
        {
            String installedWASM = CommunityFolder + $@"\mobiflight-event-module\modules\{WasmModuleNameOld}";
            if(System.IO.File.Exists(installedWASM))
                System.IO.File.Delete(installedWASM);
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


            if (!File.Exists(CommunityFolder + $@"\mobiflight-event-module\modules\{WasmModuleName}"))
                return true;

            installedWASM = CalculateMD5(CommunityFolder + $@"\mobiflight-event-module\modules\{WasmModuleName}");
            mobiflightWASM = CalculateMD5($@".\MSFS2020-module\mobiflight-event-module\modules\{WasmModuleName}");

            installedEvents = CalculateMD5(CommunityFolder + $@"\mobiflight-event-module\modules\{WasmEventsTxtFile}");
            mobiflightEvents = CalculateMD5($@".\MSFS2020-module\mobiflight-event-module\modules\{WasmEventsTxtFile}");

            return (installedWASM != mobiflightWASM || installedEvents != mobiflightEvents);
        }

        public bool InstallWasmEvents()
        {
            String destFolder = Path.Combine(CommunityFolder, WasmEventsTxtFolder);

            try
            {

                if (!Directory.Exists(destFolder))
                {
                    Log.Instance.log($"WASM events cannot be installed. WASM module {destFolder} folder not found.", LogSeverity.Error);
                    return false;
                }

                if (!Directory.Exists(WasmModuleFolder))
                {
                    Log.Instance.log($"WASM events cannot be installed. WASM module folder {WasmModuleFolder} not found.", LogSeverity.Error);
                    return false;
                }

                if (!Directory.Exists(CommunityFolder))
                {
                    Log.Instance.log($"WASM events cannot be installed. Community folder {CommunityFolder} not found.", LogSeverity.Error);
                    return false;
                }

                if (!DownloadWasmEvents())
                {
                    Log.Instance.log($"WASM events cannot be installed. Download to {CommunityFolder} was not successful.", LogSeverity.Error);
                    return false;
                }

                BackupAndInstallWasmEventsTxt(destFolder);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"WASM events cannot be installed: {ex.Message}", LogSeverity.Error);
                return false;
            }

            Log.Instance.log($"WASM events have been installed successfully.", LogSeverity.Info);
            return true;
        }

        private static void BackupAndInstallWasmEventsTxt(string destFolder)
        {
            // Copy wasm events.txt to community folder
            // and create a backup file
            String sourceFile = Path.Combine(WasmModuleFolder, "modules", WasmEventsTxtFile);
            String destFile = Path.Combine(destFolder, WasmEventsTxtFile);

            System.IO.File.Delete(destFile + ".bak");

            if (System.IO.File.Exists(destFile))
                System.IO.File.Move(destFile, destFile + ".bak");

            System.IO.File.Copy(sourceFile, destFile);
        }

        public bool DownloadWasmEvents()
        {
            ProgressUpdateEvent progress = new ProgressUpdateEvent();

            progress.ProgressMessage = "Downloading WASM events.txt (legacy)";
            progress.Current = 5;
            DownloadAndInstallProgress?.Invoke(this, progress);

            if (!DownloadSingleFile(new Uri(WasmEventsTxtUrl), WasmEventsTxtFile, WasmModuleFolder + @"\modules")) return false;
            Log.Instance.log("WASM events.txt has been downloaded and installed successfully.", LogSeverity.Debug);

            progress.ProgressMessage = "Downloading EventIDs (legacy)";
            progress.Current = 33;
            DownloadAndInstallProgress?.Invoke(this, progress);
            if (!DownloadSingleFile(new Uri(WasmEventsCipUrl), WasmEventsCipFileName, WasmEventsCipFolder)) return false;
            Log.Instance.log("WASM msfs2020_eventids.cip has been downloaded and installed successfully.", LogSeverity.Debug);

            progress.ProgressMessage = "Downloading SimVars (legacy)";
            progress.Current = 66;
            DownloadAndInstallProgress?.Invoke(this, progress);
            if (!DownloadSingleFile(new Uri(WasmEventsSimVarsUrl), WasmEventsSimVarsFileName, WasmEventsSimVarsFolder)) return false;
            Log.Instance.log("WASM msfs2020_simvars.cip has been downloaded and installed successfully.", LogSeverity.Debug);

            progress.ProgressMessage = "Downloading done";
            progress.Current = 100;
            DownloadAndInstallProgress?.Invoke(this, progress);
            return true;
        }

        public bool DownloadHubHopPresets()
        {
            ProgressUpdateEvent progress = new ProgressUpdateEvent();

            progress.ProgressMessage = "Downloading HubHop Presets (MSFS2020)";
            progress.Current = 33;
            DownloadAndInstallProgress?.Invoke(this, progress);
            if (!DownloadSingleFile(new Uri(WasmEventHubHHopUrl), WasmEventsHubHopFileName, WasmEventsHubHopFolder)) return false;
            Log.Instance.log($"WASM {WasmEventsHubHopFileName} has been downloaded and installed successfully.", LogSeverity.Info);

            progress.ProgressMessage = "Downloading HubHop Presets (XPlane)";
            progress.Current = 66;
            DownloadAndInstallProgress?.Invoke(this, progress);
            if (!DownloadSingleFile(new Uri(WasmEventsXplaneHubHHopUrl), WasmEventsXplaneHubHopFileName, WasmEventsHubHopFolder)) return false;
            Log.Instance.log($"WASM {WasmEventsXplaneHubHopFileName} has been downloaded and installed successfully.", LogSeverity.Info);

            progress.ProgressMessage = "Downloading done";
            progress.Current = 100;
            DownloadAndInstallProgress?.Invoke(this, progress);
            return true;
        }

        private bool DownloadSingleFile(Uri uri, String filename, String targetPath)
        {
            SecurityProtocolType oldType = System.Net.ServicePointManager.SecurityProtocol;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebClient webClient = new WebClient();
            string tmpFile = Directory.GetCurrentDirectory() + targetPath + @"\" + filename + ".tmp";

            webClient.DownloadFile(uri, tmpFile);
            webClient.Dispose();

            System.IO.File.Delete($@"{targetPath}\{filename}");
            System.IO.File.Move(tmpFile, $@"{targetPath}\{filename}");

            System.Net.ServicePointManager.SecurityProtocol = oldType;
            return true;
        }
    }
}
