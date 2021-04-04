using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.SimConnectMSFS
{
    public class WasmModuleUpdater
    {
        public const String WasmModuleFolder = @".\MSFS2020-module\mobiflight-event-module\";

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
            }

            if (!Directory.Exists(CommunityFolder))
            {
                Log.Instance.log("WASM module cannot be installed. Community Folder not found. " + CommunityFolder, LogSeverity.Error);
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
            var stream = File.OpenRead(filename);
            var hash = md5.ComputeHash(stream);
                    
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public bool WasmModulesAreDifferent()
        {
            Console.WriteLine("Check if WASM module needs to be updated");

            string installedWASM;
            string mobiflightWASM;

            if (CommunityFolder == null) return true;


            if (!File.Exists(CommunityFolder + @"\mobiflight-event-module\modules\StandaloneModule.wasm"))
                return true;

            installedWASM = CalculateMD5(CommunityFolder + @"\mobiflight-event-module\modules\StandaloneModule.wasm");
            mobiflightWASM = CalculateMD5(@".\MSFS2020-module\mobiflight-event-module\modules\StandaloneModule.wasm");

            return (installedWASM != mobiflightWASM);
        }
    }
}
