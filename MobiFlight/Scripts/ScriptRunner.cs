using MobiFlight.Joysticks.Winwing;
using MobiFlight.SimConnectMSFS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MobiFlight.Scripts
{
    internal class ScriptRunner
    {
        private JoystickManager JsManager;
        private SimConnectCache MsfsCache;
        private string AircraftName = string.Empty;
        private string AircraftPath = string.Empty;

        private Dictionary<string, List<ScriptMapping>> MappingDictionary = new Dictionary<string, List<ScriptMapping>>();
        private Dictionary<string, string> ScriptDictionary = new Dictionary<string, string>();

        private List<string> ExecutionList = new List<string>();
        private List<Process> ActiveProcesses = new List<Process>();

        private IChildProcessMonitor ChildProcMon;

        private bool IsInPlayMode = false;
        private bool PythonCheckCompleted = false;

        private Dictionary<string, Tuple<int, int>> RequiredPackages = new Dictionary<string, Tuple<int, int>>()
        {
            { "websockets", new Tuple<int, int>(14,0) },
            { "gql", new Tuple<int, int>(3,5) },
            { "SimConnect", new Tuple<int, int>(0,4) },
        };


        public ScriptRunner(JoystickManager joystickManager, SimConnectCache msfsCache, IChildProcessMonitor childProcMon)
        {
            JsManager = joystickManager;
            MsfsCache = msfsCache;  
            ChildProcMon = childProcMon;
            ReadConfiguration();
            GetAvailableScripts();         
        }


        private string[] SubstituteKeywords(string[] productIds)
        {
            if (productIds[0] != "WinwingCDUs")
            {
                return productIds;
            }
            else
            {               
                return WinwingConstants.CDU_PRODUCTIDS.Select(p => p.ToString("X")).ToArray();
            }
        }


        private string GetHardwareId(string vendorId, string productId)
        {
            int vId = Convert.ToInt32(vendorId, 16);
            int pId = Convert.ToInt32(productId, 16);   
            return vId.ToString() + pId.ToString();
        }

        private void ReadConfiguration()
        {
            string json = File.ReadAllText(@"Scripts\ScriptMappings.json");
            ScriptMappings definitions = JsonConvert.DeserializeObject<ScriptMappings>(json);

            foreach (var mapping in definitions.Mappings)
            {
                // Replace keyword
                mapping.ProductIds = SubstituteKeywords(mapping.ProductIds);

                foreach (var productId in mapping.ProductIds)
                {                    
                    string hardwareId = GetHardwareId(mapping.VendorId, productId);
                    if (!MappingDictionary.ContainsKey(hardwareId))
                    {
                        MappingDictionary.Add(hardwareId, new List<ScriptMapping>() { mapping });
                    }
                    else
                    {
                        MappingDictionary[hardwareId].Add(mapping);
                    }
                }
            }
        }

        private void GetAvailableScripts()
        {
            var filesFullPath = Directory.GetFiles(@"Scripts\", "*.py", SearchOption.AllDirectories);
            foreach (var fullPath in filesFullPath)
            {
                ScriptDictionary.Add(Path.GetFileName(fullPath), fullPath);
            }
        }


        private void CheckForRestart()
        {
            if (IsInPlayMode)
            {
                StopActiveProcesses();
                CheckAndExecuteScripts();
            }
        }

        
        public void OnSimAircraftChanged(object sender, string aircraftName)
        {
            AircraftName = aircraftName.ToLower();
            if (!MsfsCache.IsConnected())
            {
                CheckForRestart();
            }
        }


        public void OnSimAircraftPathChanged(object sender, string aircraftPath)
        {
            AircraftPath = aircraftPath.ToLower();

            if (MsfsCache.IsConnected())
            {
                CheckForRestart();
            }            
        }

        private bool IsMinimumPythonVersion()
        {
            bool result = false;
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string output = reader.ReadToEnd();
                    var x = output.Split(' ');
                    var v = x[1].Split('.');
                    Log.Instance.log($"Python version: {x[1]}.", LogSeverity.Debug);
                    if ( (int.Parse(v[0]) >= 3) && (int.Parse(v[1]) >= 10))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        private bool IsPythonPathSet()
        {
            string pathVariable = Environment.GetEnvironmentVariable("PATH").ToLower();
            if (pathVariable.Contains("python"))
            {
                Log.Instance.log($"Python Path is set.", LogSeverity.Debug);
                return true;
            }
            else
            {
                Log.Instance.log($"Python Path not set.", LogSeverity.Error);
                return false;
            }
        }

        private bool AreNecessaryPythonPackagesInstalled()
        {
            var installedPackages = new Dictionary<string, Tuple<int, int>>();

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "pip",
                Arguments = "freeze",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Log.Instance.log($"Python installed packages: {Environment.NewLine}{result}", LogSeverity.Debug);
                    string[] lines = result.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines)
                    {
                        var parts = line.Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries);
                        var v = parts[1].Split('.');
                        installedPackages.Add(parts[0], new Tuple<int, int>(int.Parse(v[0]), int.Parse(v[1])));                      
                    }                    
                }
            }

            bool necessaryPackagesAvailable = true;

            foreach (var package in RequiredPackages.Keys)
            {
                if (installedPackages.ContainsKey(package))
                {
                    if ( !((installedPackages[package].Item1 >= RequiredPackages[package].Item1) &&
                          (installedPackages[package].Item2 >= RequiredPackages[package].Item2)))
                    {
                        necessaryPackagesAvailable = false;
                        Log.Instance.log($"Python package version too low: '{package}'", LogSeverity.Error);
                    }
                }
                else
                {
                    necessaryPackagesAvailable = false;
                    Log.Instance.log($"Necessary Python package not installed: '{package}'", LogSeverity.Error);
                }
            }
            
            return necessaryPackagesAvailable;
        }

        private void ShowPythonInstructionsMessageBox()
        {
            if (MessageBox.Show(i18n._tr("uiMessagePythonInstructions"),
                    i18n._tr("uiMessagePythonHint"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning) == DialogResult.OK) ;
            {
                Process.Start("https://github.com/MobiFlight/MobiFlight-Connector/wiki/Installing-Python");
            }
        }

        private bool IsPythonReady()
        {
            if (!IsPythonPathSet())
            {
                ShowPythonInstructionsMessageBox();
                return false;
            }

            if (!IsMinimumPythonVersion())
            {
                ShowPythonInstructionsMessageBox();
                return false;
            }

            if (!AreNecessaryPythonPackagesInstalled())
            {
                ShowPythonInstructionsMessageBox();
                return false;
            }

            return true;
        }

        private void ExecuteScripts()
        {           
            if (!PythonCheckCompleted)
            {
                if (!IsPythonReady())
                {
                    Log.Instance.log($"Python not ready!", LogSeverity.Error);
                    return;
                }
                else
                {
                    PythonCheckCompleted = true;
                }
            }

            foreach (var script in ExecutionList)
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {                                       
                    FileName = @"python",                    
                    Arguments = ScriptDictionary[script],
                    WindowStyle = ProcessWindowStyle.Minimized,
                };

                Process process = new Process
                {
                    StartInfo = psi
                };

                process.Start();   
                
                ActiveProcesses.Add(process);
                ChildProcMon.AddChildProcess(process);               
            }
        }


        private void CheckAndExecuteScripts()
        {
            string aircraftDescription = MsfsCache.IsConnected() ? AircraftPath : AircraftName;
            ExecutionList.Clear();

            // Get all joysticks
            List<Joystick> joysticks = JsManager.GetJoysticks();
            foreach (Joystick joystick in joysticks)
            {
                var jsDef = joystick.GetJoystickDefinition();
                if (jsDef != null)
                {
                    string hardwareId = jsDef.VendorId.ToString() + jsDef.ProductId.ToString();
                    if (MappingDictionary.ContainsKey(hardwareId))
                    {
                        // Hardware found, now compare aircraft 
                        foreach (var config in MappingDictionary[hardwareId])
                        {
                            if (aircraftDescription.Contains(config.AircraftIdSnippet))
                            {
                                ExecutionList.Add(config.ScriptName);
                            }
                        }
                    }
                }
            }

            if (ExecutionList.Count > 0)
            {
                ExecuteScripts();
            }
        }


        public void Start()
        {
            IsInPlayMode = true;
            CheckAndExecuteScripts();
        }


        private void StopActiveProcesses()
        {
            foreach (var process in ActiveProcesses)
            {
                if (!process.HasExited)
                {
                    process.Kill();                   
                }                
            }
        }


        public void Stop()
        {
            IsInPlayMode = false;
            StopActiveProcesses();          
        }


        public void Shutdown()
        {
            Stop();
        }

    }
}
