using MobiFlight.Joysticks.Winwing;
using MobiFlight.SimConnectMSFS;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MobiFlight.Scripts
{
    internal class ScriptRunner
    {
        private string PythonExecutable;
        private const string WINWING_CDUS_KEYWORD = "WinwingCDUs";
        private const string CONFIG_FILE_PATH = @"Scripts\ScriptMappings.json";
        private const string SCRIPTS_DIRECTORY = "Scripts";
        private const string SCRIPT_EXTENSION = "*.py";
        private const int STARTUP_DELAY_MS = 2000;
        private const int PROCESS_POLLING_DELAY_MS = 300;
        private const int PROCESS_KILL_TIMEOUT_MS = 1000;

        private JoystickManager JsManager;
        private SimConnectCacheInterface MsfsCache;
        private string AircraftName = string.Empty;
        private string AircraftPath = string.Empty;

        private Dictionary<string, List<ScriptMapping>> MappingDictionary = new Dictionary<string, List<ScriptMapping>>();
        private Dictionary<string, string> ScriptDictionary = new Dictionary<string, string>();

        private ConcurrentBag<Process> ActiveProcesses = new ConcurrentBag<Process>();
        private ConcurrentDictionary<int, string> ProcessTable = new ConcurrentDictionary<int, string>();
        private ConcurrentQueue<string> NewAircraftRequestQueue = new ConcurrentQueue<string>();
        private CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        private IChildProcessMonitor ChildProcMon;

        private volatile bool IsInPlayMode = false;

        private ConcurrentBag<Joystick> GameControllersWithScripts = new ConcurrentBag<Joystick>();

        public ScriptRunner(JoystickManager joystickManager, SimConnectCacheInterface msfsCache)
        {
            PythonExecutable = PythonEnvironment.PathPythonExecutable;
            JsManager = joystickManager;
            MsfsCache = msfsCache;
            ReadConfiguration();
            GetAvailableScripts();
        }

        private string[] SubstituteKeywords(string[] productIds)
        {
            if (productIds[0] != WINWING_CDUS_KEYWORD)
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
            try
            {
                int vId = Convert.ToInt32(vendorId, 16);
                int pId = Convert.ToInt32(productId, 16);
                return $"{vId}{pId}";
            }
            catch (FormatException ex)
            {
                Log.Instance.log($"ScriptRunner - Invalid hardware ID format: VendorId={vendorId}, ProductId={productId}. {ex.Message}", LogSeverity.Error);
                return string.Empty;
            }
            catch (OverflowException ex)
            {
                Log.Instance.log($"ScriptRunner - Hardware ID value overflow: {ex.Message}", LogSeverity.Error);
                return string.Empty;
            }
        }

        private void ReadConfiguration()
        {
            string json = File.ReadAllText(CONFIG_FILE_PATH);
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
            var filesFullPath = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SCRIPTS_DIRECTORY), SCRIPT_EXTENSION, SearchOption.AllDirectories);

            foreach (var fullPath in filesFullPath)
            {
                string fileName = Path.GetFileName(fullPath);
                Log.Instance.log($"ScriptRunner - Add script {fileName}.", LogSeverity.Debug);
                ScriptDictionary.Add(Path.GetFileName(fileName), fullPath);
            }
        }

        public void OnSimAircraftChanged(object sender, string aircraftName)
        {
            AircraftName = aircraftName.ToLower();
            if (!MsfsCache.IsConnected() && IsInPlayMode)
            {
                NewAircraftRequestQueue.Enqueue(AircraftName);
            }
        }

        public void OnSimAircraftPathChanged(object sender, string aircraftPath)
        {
            AircraftPath = aircraftPath.ToLower();

            if (MsfsCache.IsConnected() && IsInPlayMode)
            {
                NewAircraftRequestQueue.Enqueue(AircraftPath);
            }
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

            // ChildProcessMonitor necessary, that in case of MobiFlight crash, all child processes are terminated
            ChildProcMon = new ChildProcessMonitor();

            SendUserMessage(UserMessageCodes.STARTING_SCRIPT, string.Join(" ", executionList));

            foreach (var script in executionList)
            {
                if (!ScriptDictionary.ContainsKey(script))
                {
                    Log.Instance.log($"ScriptRunner - Script not found in dictionary: {script}", LogSeverity.Error);
                    SendUserMessage(UserMessageCodes.SCRIPT_START_FAILED, script);
                    continue;
                }

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = PythonExecutable,
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

                try
                {
                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    ProcessTable[process.Id] = script;
                    ActiveProcesses.Add(process);

                    try
                    {
                        if (!process.HasExited)
                        {
                            ChildProcMon.AddChildProcess(process);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        Log.Instance.log($"ScriptRunner - Cannot add child process, process may have already exited: {ex.Message}", LogSeverity.Error);
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.log($"ScriptRunner - Exception in ChildProcessMonitor AddChildProcess: {ex.Message}", LogSeverity.Error);
                    }
                }
                catch (System.ComponentModel.Win32Exception ex)
                {
                    Log.Instance.log($"ScriptRunner - Failed to start script '{script}': Python executable not found. {ex.Message}", LogSeverity.Error);
                    SendUserMessage(UserMessageCodes.SCRIPT_START_FAILED, script);
                    process.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"ScriptRunner - Failed to start script '{script}': {ex.Message}", LogSeverity.Error);
                    SendUserMessage(UserMessageCodes.SCRIPT_START_FAILED, script);
                    process.Dispose();
                }
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            try
            {
                Process process = (Process)sender;
                string processName = string.Empty;
                ProcessTable.TryGetValue(process.Id, out processName);

                int exitCode = -1;
                try
                {
                    exitCode = process.ExitCode;
                }
                catch (InvalidOperationException)
                {
                    Log.Instance.log($"ScriptRunner - Cannot access exit code for process: {processName}", LogSeverity.Debug);
                }

                Log.Instance.log($"ScriptRunner - ExitCode: {exitCode}, Name: {processName}", LogSeverity.Error);
                SendUserMessage(UserMessageCodes.PROCESS_TERMINATED, processName);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"ScriptRunner - Error in Process_Exited handler: {ex.Message}", LogSeverity.Error);
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Log.Instance.log($"ScriptRunner - Output: {e.Data}", LogSeverity.Info);
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Log.Instance.log($"ScriptRunner - StandardOutput: {e.Data}", LogSeverity.Info);
            }
        }

        private void CheckAndExecuteScripts(string aircraftDescription)
        {
            var executionList = new List<string>();

            Log.Instance.log($"ScriptRunner - Current aircraft description: {aircraftDescription}.", LogSeverity.Debug);

            // Empty bag
            while (GameControllersWithScripts.TryTake(out _)) { }

            // Get all game controllers. GetJoysticks is ThreadSafe
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

            if (executionList.Count > 0)
            {
                ExecuteScripts(executionList);
            }
        }

        public void StartUp()
        {
            Log.Instance.log($"ScriptRunner - StartUp().", LogSeverity.Debug);
            // Delay because establishing MsfsCache connection state does need some time.           
            Task.Run(async () =>
            {
                await Task.Delay(STARTUP_DELAY_MS);
                Start();
            });
        }

        public void Start()
        {
            Log.Instance.log($"ScriptRunner - Start().", LogSeverity.Debug);
            IsInPlayMode = true;
            string currentAircraftDescription = MsfsCache.IsConnected() ? AircraftPath : AircraftName;
            currentAircraftDescription = "_fnx_3_";
            NewAircraftRequestQueue.Enqueue(currentAircraftDescription);
            Task myTask = Task.Run(async () => { await ProcessAircraftRequests(CancellationTokenSource.Token); });
        }

        private void StopActiveProcesses()
        {
            foreach (var process in ActiveProcesses)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.OutputDataReceived -= Process_OutputDataReceived;
                        process.ErrorDataReceived -= Process_ErrorDataReceived;
                        process.Exited -= Process_Exited;
                        process.Kill();
                        process.WaitForExit(PROCESS_KILL_TIMEOUT_MS);
                    }
                    process.Dispose();
                }
                catch (InvalidOperationException ex)
                {
                    Log.Instance.log($"ScriptRunner - Process already exited: {ex.Message}", LogSeverity.Debug);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"ScriptRunner - Error stopping process: {ex.Message}", LogSeverity.Error);
                }
            }

            // Empty bag
            while (ActiveProcesses.TryTake(out _)) { }
            ProcessTable.Clear();
        }

        public void Stop()
        {
            Log.Instance.log("ScriptRunner - Stop().", LogSeverity.Debug);
            IsInPlayMode = false;

            try
            {
                CancellationTokenSource.Cancel();
            }
            catch (ObjectDisposedException ex)
            {
                Log.Instance.log($"ScriptRunner - CancellationTokenSource already disposed: {ex.Message}", LogSeverity.Error);
            }

            CancellationTokenSource = new CancellationTokenSource();
            StopActiveProcesses();
        }

        public void Shutdown()
        {
            Stop();
        }

        private async Task ProcessAircraftRequests(CancellationToken token)
        {
            Log.Instance.log($"ScriptRunner - Start processing thread.", LogSeverity.Debug);
            while (!token.IsCancellationRequested)
            {
                string aircraftString = null;

                while (NewAircraftRequestQueue.TryDequeue(out string nextAircraft))
                {
                    aircraftString = nextAircraft;
                }

                if (aircraftString != null)
                {
                    Log.Instance.log($"ProcessAircraftRequest: {aircraftString}", LogSeverity.Info);
                    StopActiveProcesses();
                    CheckAndExecuteScripts(aircraftString);
                }

                await Task.Delay(PROCESS_POLLING_DELAY_MS);
            }
            Log.Instance.log($"ScriptRunner - Stop processing thread.", LogSeverity.Debug);
        }
    }
}
