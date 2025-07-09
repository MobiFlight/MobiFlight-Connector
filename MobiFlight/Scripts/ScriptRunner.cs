﻿using MobiFlight.Joysticks.Winwing;
using MobiFlight.SimConnectMSFS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.Scripts
{
    internal class ScriptRunner
    {
        private JoystickManager JsManager;
        private SimConnectCacheInterface MsfsCache;
        private string AircraftName = string.Empty;
        private string AircraftPath = string.Empty;

        private Dictionary<string, List<ScriptMapping>> MappingDictionary = new Dictionary<string, List<ScriptMapping>>();
        private Dictionary<string, string> ScriptDictionary = new Dictionary<string, string>();
      
        private List<Process> ActiveProcesses = new List<Process>();
        private Dictionary<int, string> ProcessTable = new Dictionary<int, string>();

        private IChildProcessMonitor ChildProcMon;

        private volatile bool IsInPlayMode = false;
        private volatile bool PythonCheckCompleted = false; 
        private object ProcessLock = new object();    

        private const int LINE_LENGTH = 24; // 24 characters per line on CDU

        private List<Joystick> GameControllersWithScripts = new List<Joystick>();

        private Dictionary<string, Tuple<int, int>> RequiredPackages = new Dictionary<string, Tuple<int, int>>()
        {
            { "websockets", new Tuple<int, int>(14,0) },
            { "gql", new Tuple<int, int>(3,5) },
            { "SimConnect", new Tuple<int, int>(0,4) },
        };


        public ScriptRunner(JoystickManager joystickManager, SimConnectCacheInterface msfsCache, IChildProcessMonitor childProcMon)
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

                Log.Instance.log($"ScriptRunner - Add mapping {mapping.ScriptName}.", LogSeverity.Debug);

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
            var filesFullPath = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts"), "*.py", SearchOption.AllDirectories);           

            foreach (var fullPath in filesFullPath)
            {
                string fileName = Path.GetFileName(fullPath);
                Log.Instance.log($"ScriptRunner - Add script {fileName}.", LogSeverity.Debug);
                ScriptDictionary.Add(Path.GetFileName(fileName), fullPath);
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
                    //python --version returns "Python x.xx.x"
                    string output = reader.ReadToEnd();
                    var outputParts = output.Split(' ');
                    if (outputParts.Length > 1)
                    {
                        Log.Instance.log($"Python version: {outputParts[1]}.", LogSeverity.Info);
                        if (Version.TryParse(outputParts[1], out Version version))
                        {
                            if (version.CompareTo(new Version(3, 11, 0)) >= 0)
                            {
                                return true;
                            }
                            else
                            {
                                Log.Instance.log($"Python version not supported: {outputParts[1]}.", LogSeverity.Warn);
                                return false;
                            }
                        }
                    }
                    Log.Instance.log($"Failed to parse Python version: '{output}'.", LogSeverity.Warn);
                }
            }
            return false;
        }

        private bool IsPythonPathSet()
        {
            string pathVariable = Environment.GetEnvironmentVariable("PATH").ToLower();
            if (pathVariable.Contains("python"))
            {
                Log.Instance.log($"ScriptRunner - Python Path is set.", LogSeverity.Debug);
                return true;
            }
            else
            {
                Log.Instance.log($"ScriptRunner - Python Path not set.", LogSeverity.Error);
                return false;
            }
        }

        private bool IsPythonMicrosoftStoreInstalled()
        {
            string powerShellCommand = "Get-AppxPackage -Name '*python*' | Select-Object Name";

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"{powerShellCommand}\"",
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

                    if (!string.IsNullOrEmpty(output) && output.Contains("Python"))
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
                    Log.Instance.log($"ScriptRunner - Python installed packages: {Environment.NewLine}{result}", LogSeverity.Debug);
                    string[] lines = result.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines)
                    {
                        var parts = line.Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 1)
                        {
                            var v = parts[1].Split('.');
                            if (v.Length > 1)
                            {
                                var majorSuccess = int.TryParse(v[0], out int major);
                                var minorSuccess = int.TryParse(v[1], out int minor);
                                if (majorSuccess && minorSuccess)
                                {
                                    installedPackages.Add(parts[0], new Tuple<int, int>(major, minor));
                                }
                                else
                                {
                                    Log.Instance.log($"ScriptRunner - Package version cannot be parsed: '{parts[1]}'", LogSeverity.Info);
                                }
                            }
                            else
                            {
                                Log.Instance.log($"ScriptRunner - Package version has not two elements: '{parts[1]}'", LogSeverity.Error);
                            }
                        }
                        else
                        {
                            Log.Instance.log($"ScriptRunner - Package info has not two elements: '{line}'", LogSeverity.Error);
                        }
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
                        Log.Instance.log($"ScriptRunner - Python package version too low: '{package}'", LogSeverity.Error);
                    }
                }
                else
                {
                    necessaryPackagesAvailable = false;
                    Log.Instance.log($"ScriptRunner - Necessary Python package not installed: '{package}'", LogSeverity.Error);
                }
            }
            
            return necessaryPackagesAvailable;
        }

        private void ShowPythonInstructionsMessageBox()
        {
            Log.Instance.log($"ShowPythonInstructionsMessageBox", LogSeverity.Debug);
            if (MessageBox.Show(i18n._tr("uiMessagePythonInstructions"),
                    i18n._tr("uiMessagePythonHint"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning) == DialogResult.OK) ;
            {
                Process.Start("https://docs.mobiflight.com/guides/installing-python/");
            }
        }

        private bool IsPythonReady()
        {
            if (!(IsPythonMicrosoftStoreInstalled() || IsPythonPathSet()))
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

        private void SendUserMessage(int messageCode, params string[] parameters)
        {
            foreach (var gameController in GameControllersWithScripts)
            {
                gameController.ShowUserMessage(messageCode, parameters);
            }
        }

        private void ExecuteScripts(List<string> executionList)
        {
            lock (ProcessLock)
            {
                SendUserMessage(UserMessageCodes.STARTING_SCRIPT, string.Join(" ", executionList));

                if (!PythonCheckCompleted)
                {
                    if (!IsPythonReady())
                    {
                        Log.Instance.log($"ScriptRunner - Python not ready!", LogSeverity.Error);
                        SendUserMessage(UserMessageCodes.PYTHON_NOT_READY);
                        return;
                    }
                    else
                    {
                        PythonCheckCompleted = true;
                    }
                }
                                      
                foreach (var script in executionList)
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = @"python",
                        Arguments = ($"\"{ScriptDictionary[script]}\""),
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    };

                    Process process = new Process
                    {
                        StartInfo = psi
                    };

                    process.EnableRaisingEvents = true;
                    process.OutputDataReceived += Process_OutputDataReceived;
                    process.ErrorDataReceived += Process_ErrorDataReceived;
                    process.Exited += Process_Exited;

                    Log.Instance.log($"ScriptRunner - Start Process: {script}", LogSeverity.Info);
                    Log.Instance.log($"ScriptRunner - Start Process FullPath: {psi.Arguments}", LogSeverity.Debug);
                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    ProcessTable.Add(process.Id, script);
                    ActiveProcesses.Add(process);

                    try
                    {
                        ChildProcMon.AddChildProcess(process);
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.log($"ScriptRunner - Exception in ChildProcessMonitor AddChildProcess: {ex.Message}", LogSeverity.Error);
                    }
                }
            } // end lock
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Process process = (Process)sender;   
            string processName = string.Empty;
            ProcessTable.TryGetValue(process.Id, out processName);            
            Log.Instance.log($"ScriptRunner - ExitCode: {process.ExitCode}, Name: {processName}", LogSeverity.Error);
            SendUserMessage(UserMessageCodes.PROCESS_TERMINATED, processName);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {           
            Log.Instance.log($"ScriptRunner - Output: {e.Data}", LogSeverity.Info);          
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log.Instance.log($"ScriptRunner - StandardOutput: {e.Data}", LogSeverity.Info);       
        }

        private void CheckAndExecuteScripts()
        {
            var executionList = new List<string>();

            lock (ProcessLock)
            {
                string aircraftDescription = MsfsCache.IsConnected() ? AircraftPath : AircraftName;
                Log.Instance.log($"ScriptRunner - Current aircraft description: {aircraftDescription}.", LogSeverity.Debug);                
                GameControllersWithScripts.Clear();

                // Get all game controllers
                var gameControllers = JsManager.GetJoysticks();
                foreach (var gameController in gameControllers)
                {
                    var jsDef = gameController.GetJoystickDefinition();
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
                                    if (!GameControllersWithScripts.Contains(gameController))
                                    {
                                        GameControllersWithScripts.Add(gameController);
                                    }

                                    // Only add if not already there
                                    if (!executionList.Contains(config.ScriptName))
                                    {
                                        Log.Instance.log($"ScriptRunner - Add {config.ScriptName} to execution list.", LogSeverity.Info);
                                        executionList.Add(config.ScriptName);
                                    }
                                }
                            }
                        }
                    }
                }
            } // end lock

            if (executionList.Count > 0)
            {
                var task = Task.Run(() =>
                {
                    ExecuteScripts(executionList);
                });
            }
        }
        

        public void Start()
        {
            Log.Instance.log($"ScriptRunner - Start().", LogSeverity.Debug);
            IsInPlayMode = true;
            CheckAndExecuteScripts();
        }


        private void StopActiveProcesses()
        {
            lock (ProcessLock)
            {
                foreach (var process in ActiveProcesses)
                {
                    if (!process.HasExited)
                    {
                        process.OutputDataReceived -= Process_OutputDataReceived;
                        process.ErrorDataReceived -= Process_ErrorDataReceived;  
                        process.Exited -= Process_Exited;
                        process.Kill();
                    }
                }
                ActiveProcesses.Clear();
                ProcessTable.Clear();
            }
        }


        public void Stop()
        {
            Log.Instance.log($"ScriptRunner - Stop().", LogSeverity.Debug);
            IsInPlayMode = false;
            StopActiveProcesses();          
        }


        public void Shutdown()
        {
            Stop();
        }

    }
}
