using MobiFlight.Base;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Incoming;
using MobiFlight.Execution;
using MobiFlight.FSUIPC;
using MobiFlight.InputConfig;
using MobiFlight.Scripts;
using MobiFlight.SimConnectMSFS;
using MobiFlight.xplane;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MobiFlight
{
    public interface IExecutionManager
    {
        Dictionary<String, MobiFlightVariable> GetAvailableVariables();
        JoystickManager GetJoystickManager();
        MobiFlightCache getMobiFlightModuleCache();
        MidiBoardManager GetMidiBoardManager();
        // Add other methods and properties as needed
    }
    public class ExecutionManager : IExecutionManager
    {
        public event EventHandler OnConfigHasChanged;
        public event EventHandler OnExecute;
        public event EventHandler OnStarted;
        public event EventHandler OnStopped;
        public event EventHandler OnTestModeStarted;
        public event EventHandler OnTestModeStopped;
        public event EventHandler OnTestModeException;

        public event EventHandler OnSimAvailable;
        public event EventHandler OnSimUnavailable;
        public event EventHandler OnSimCacheClosed;
        public event EventHandler OnSimCacheConnected;
        public event EventHandler OnSimCacheConnectionLost;
        public event EventHandler<string> OnSimAircraftChanged;
        public event EventHandler<string> OnSimAircraftPathChanged;

        public event EventHandler OnModuleConnected;
        public event EventHandler OnModuleRemoved;
        public event EventHandler OnModuleCacheAvailable;
        public event EventHandler OnShutdown;
        public event EventHandler OnInitialModuleLookupFinished;
        public event EventHandler OnJoystickConnectedFinished;
        public event EventHandler OnMidiBoardConnectedFinished;

        public event EventHandler SettingsDialogRequested;
        public event EventHandler<Project> OnProjectChanged;

        /// <summary>
        /// a semaphore to prevent multiple execution of timer callback
        /// </summary>
        protected bool isExecuting = false;

        /// <summary>
        /// the timer used for polling
        /// </summary>
        private readonly EventTimer timer = new EventTimer();

        /// <summary>
        /// the timer used for auto connect of FSUIPC and Arcaze
        /// </summary>
        private readonly Timer autoConnectTimer = new Timer();

        /// <summary>
        /// the timer used for execution of test mode
        /// </summary>
        private readonly Timer testModeTimer = new Timer();
        int testModeIndex = 0;

        private readonly Timer frontendUpdateTimer = new Timer();

        /// Window handle
        private IntPtr handle = new IntPtr(0);

        /// <summary>
        /// This list contains preparsed informations and cached values for the supervised FSUIPC offsets
        /// </summary>
        readonly Fsuipc2Cache fsuipcCache = new Fsuipc2Cache();

#if SIMCONNECT
        readonly SimConnectCache simConnectCache = new SimConnectCache();
#endif
        readonly XplaneCache xplaneCache = new XplaneCache();
        readonly ProSim.ProSimCache proSimCache = new ProSim.ProSimCache();

#if ARCAZE
        readonly ArcazeCache arcazeCache = new ArcazeCache();
#endif

#if MOBIFLIGHT
        readonly MobiFlightCache mobiFlightCache = new MobiFlightCache();
#endif
        readonly JoystickManager joystickManager = new JoystickManager();
        readonly MidiBoardManager midiBoardManager = new MidiBoardManager();
        private readonly Dictionary<ConfigFile, InputEventExecutor> _inputEventExecutors = new Dictionary<ConfigFile, InputEventExecutor>();
        readonly InputActionExecutionCache inputActionExecutionCache = new InputActionExecutionCache();
        private ScriptRunner scriptRunner = null;

        public List<IConfigItem> ConfigItems { 
            get { 
                return _project.ConfigFiles.Count > ActiveConfigIndex 
                    ? _project.ConfigFiles[ActiveConfigIndex].ConfigItems 
                    : new List<IConfigItem>(); } 
        }

        private Project _project = new Project();
        public Project Project
        {
            get { return _project; }
            set
            {
                if (_project == value) return;
                _project = value;
                _project.ProjectChanged += (s, e) =>
                {
                    OnProjectChanged?.Invoke(this, Project);
                };
                OnProjectChanged?.Invoke(this, Project);
            }
        }

        public int ActiveConfigIndex { get; private set; } = 0;

        private bool _autoConnectTimerRunning = false;

        FlightSimType LastDetectedSim = FlightSimType.NONE;

        OutputConfigItem ConfigItemInTestMode = null;
        Dictionary<string, IConfigItem> updatedValues = new Dictionary<string, IConfigItem>();
        bool updateFrontend = true;

        public ExecutionManager(IntPtr handle)
        {
            fsuipcCache.ConnectionLost += new EventHandler(FsuipcCache_ConnectionLost);
            fsuipcCache.Connected += new EventHandler(FsuipcCache_Connected);
            fsuipcCache.Closed += new EventHandler(FsuipcCache_Closed);
            fsuipcCache.AircraftChanged += new EventHandler<string>(sim_AirCraftChanged);

#if SIMCONNECT
            simConnectCache.SetHandle(handle);
            simConnectCache.ConnectionLost += new EventHandler(simConnect_ConnectionLost);
            simConnectCache.Connected += new EventHandler(simConnect_Connected);
            simConnectCache.Closed += new EventHandler(simConnect_Closed);
            simConnectCache.AircraftChanged += new EventHandler<string>(sim_AirCraftChanged);
            simConnectCache.AircraftPathChanged += new EventHandler<string>(simConnect_AirCraftPathChanged);
#endif

            xplaneCache.ConnectionLost += new EventHandler(simConnect_ConnectionLost);
            xplaneCache.Connected += new EventHandler(simConnect_Connected);
            xplaneCache.Closed += new EventHandler(simConnect_Closed);
            xplaneCache.AircraftChanged += new EventHandler<string>(sim_AirCraftChanged);

            proSimCache.ConnectionLost += new EventHandler(simConnect_ConnectionLost);
            proSimCache.Connected += new EventHandler(simConnect_Connected);
            proSimCache.Closed += new EventHandler(simConnect_Closed);
            proSimCache.AircraftChanged += new EventHandler<string>(sim_AirCraftChanged);

#if ARCAZE
            arcazeCache.OnAvailable += new EventHandler(ModuleCache_Available);
            arcazeCache.Closed += new EventHandler(ModuleCache_Closed);
            arcazeCache.ModuleConnected += new EventHandler(ModuleCache_ModuleConnected);
            arcazeCache.ModuleRemoved += new EventHandler(ModuleCache_ModuleRemoved);
            arcazeCache.Enabled = Properties.Settings.Default.ArcazeSupportEnabled;
#endif

            mobiFlightCache.OnAvailable += new EventHandler(ModuleCache_Available);
            mobiFlightCache.Closed += new EventHandler(ModuleCache_Closed);
            mobiFlightCache.ModuleConnected += new EventHandler(ModuleCache_ModuleConnected);
            mobiFlightCache.ModuleRemoved += new EventHandler(ModuleCache_ModuleRemoved);
            mobiFlightCache.LookupFinished += new EventHandler(mobiFlightCache_LookupFinished);

            // ChildProcessMonitor necessary, that in case of MobiFlight crash, all child processes are terminated
            scriptRunner = new ScriptRunner(joystickManager, simConnectCache, new ChildProcessMonitor());
            OnSimAircraftChanged += scriptRunner.OnSimAircraftChanged;
            OnSimAircraftPathChanged += scriptRunner.OnSimAircraftPathChanged;

            timer.Tick += new EventHandler(timer_Tick);
            timer.Stopped += new EventHandler(timer_Stopped);
            timer.Started += new EventHandler(timer_Started);
            SetPollInterval(Properties.Settings.Default.PollInterval);

            autoConnectTimer.Interval = 10000;
            autoConnectTimer.Tick += new EventHandler(AutoConnectTimer_TickAsync);

            testModeTimer.Tick += new EventHandler(testModeTimer_Tick);
            SetTestModeInterval(Properties.Settings.Default.TestTimerInterval);

#if MOBIFLIGHT
            mobiFlightCache.OnButtonPressed += new ButtonEventHandler(mobiFlightCache_OnButtonPressed);
#endif
            joystickManager.SetHandle(handle);
            joystickManager.OnButtonPressed += new ButtonEventHandler(mobiFlightCache_OnButtonPressed);
            joystickManager.Connected += (sender, e) =>
            {
                joystickManager.Startup();
                OnJoystickConnectedFinished?.Invoke(sender, e);
            };

            midiBoardManager.OnButtonPressed += new ButtonEventHandler(mobiFlightCache_OnButtonPressed);
            midiBoardManager.Connected += (sender, e) =>
            {
                midiBoardManager.Startup();
                OnMidiBoardConnectedFinished?.Invoke(sender, e);
            };

            OnProjectChanged += (s, p) =>
            {
                ActiveConfigIndex = 0;
                InitInputEventExecutor();
                MessageExchange.Instance.Publish(p);
            };

            frontendUpdateTimer.Interval = 200;
            frontendUpdateTimer.Tick += (s, e) =>
            {
                if (!updateFrontend) return;

                UpdateInputPreconditions();

                if (updatedValues.Count > 0)
                {
                    var list = updatedValues.Values.Select(cfg => new ConfigValueOnlyItem(cfg)).Cast<IConfigValueOnlyItem>().ToList();
                    // Replace the line causing the error with the following line
                    var update = new ConfigValueRawAndFinalUpdate(list);
                    MessageExchange.Instance.Publish(update);
                }

                lock (updatedValues)
                {
                    updatedValues.Clear();
                }
            };

            mobiFlightCache.Start();
            InitializeFrontendSubscriptions();
        }

        public void StartJoystickManager()
        {
            if (Properties.Settings.Default.EnableJoystickSupport)
            {
                joystickManager.Connect();
            }
        }

        public void StartMidiBoardManager()
        {
            if (Properties.Settings.Default.EnableMidiSupport)
            {
                midiBoardManager.Connect();
            }
        }

        private void InitializeFrontendSubscriptions()
        {
            MessageExchange.Instance.Subscribe<CommandUpdateConfigItem>((message) =>
            {
                HandleCommandUpdateConfigItem(message.Item);
            });

            MessageExchange.Instance.Subscribe<CommandAddConfigItem>((message) =>
            {
                IConfigItem item = new OutputConfigItem();
                if (message.Type == "InputConfig")
                {
                    item = new InputConfigItem();
                }

                item.GUID = Guid.NewGuid().ToString();
                item.Name = message.Name;
                item.Active = true;
                ConfigItems.Add(item);

                MessageExchange.Instance.Publish(new ConfigValueFullUpdate(ActiveConfigIndex, ConfigItems));
                OnConfigHasChanged?.Invoke(item, null);
            });

            MessageExchange.Instance.Subscribe<CommandConfigBulkAction>((message) =>
            {
                if (message.Action == "delete")
                {
                    message.Items.ForEach(item =>
                    {
                        var cfg = ConfigItems.Find(i => i.GUID == item.GUID);
                        ConfigItems.Remove(cfg);
                    });
                }
                else if (message.Action == "toggle")
                {
                    var toggleValue = !message.Items.First().Active;
                    message.Items.ForEach(item =>
                    {
                        var cfg = ConfigItems.Find(i => i.GUID == item.GUID);
                        if (cfg == null) return;
                        cfg.Active = toggleValue;
                    });
                }
                MessageExchange.Instance.Publish(new ConfigValueFullUpdate(ActiveConfigIndex, ConfigItems));
                OnConfigHasChanged?.Invoke(ConfigItems, null);
            });


            MessageExchange.Instance.Subscribe<CommandConfigContextMenu>((message) =>
            {
                IConfigItem cfg;
                switch (message.Action)
                {
                    case "delete":
                        ConfigItems.RemoveAll(i => i.GUID == message.Item.GUID);
                        break;

                    case "toggle":
                        cfg = ConfigItems.Find(i => i.GUID == message.Item.GUID);
                        if (cfg == null) break;
                        cfg.Active = !cfg.Active;
                        break;

                    case "duplicate":
                        var index = ConfigItems.FindIndex(i => i.GUID == message.Item.GUID);
                        if (index == -1) break;
                        var dup = ConfigItems[index].Duplicate();
                        ConfigItems.Insert(index, dup);
                        break;

                    case "test":
                        cfg = ConfigItems.Find(i => i.GUID == message.Item.GUID);
                        if (cfg == null || !(cfg is OutputConfigItem)) break;
                        var toggleTest = cfg.AreEqual(ConfigItemInTestMode);

                        try
                        {
                            ExecuteTestOff(ConfigItemInTestMode, true);
                        }
                        catch (Exception e)
                        {
                            Log.Instance.log(e.Message, LogSeverity.Error);
                        }

                        if (toggleTest)
                        {
                            return;
                        }

                        try
                        {
                            ExecuteTestOn(cfg as OutputConfigItem);
                        }
                        catch (Exception e)
                        {
                            Log.Instance.log($"Error during test mode execution: {cfg.Name}. {e.Message}", LogSeverity.Error);
                        }

                        return;

                    case "settings":
                        cfg = ConfigItems.Find(i => i.GUID == message.Item.GUID);
                        if (cfg == null) return;

                        var serial = SerialNumber.ExtractSerial(cfg.ModuleSerial);

                        if (SerialNumber.IsMobiFlightSerial(serial))
                        {
                            var module = mobiFlightCache.GetModuleBySerial(serial)?.ToMobiFlightModuleInfo();
                            if (module == null)
                            {
                                // the device is currently not connected.
                                return;
                            }
                            SettingsDialogRequested?.Invoke(module, null);
                            // we don't have to publish an update at this point.
                            return;
                        }
                        if (SerialNumber.IsMidiBoardSerial(serial) || SerialNumber.IsJoystickSerial(serial))
                        {
                            // at this point we don't need to pass in anything specific
                            SettingsDialogRequested?.Invoke(null, null);
                        }
                        break;

                    default:
                        return;
                }

                MessageExchange.Instance.Publish(new ConfigValueFullUpdate(ActiveConfigIndex, ConfigItems));
                OnConfigHasChanged?.Invoke(ConfigItems, null);
            });

            MessageExchange.Instance.Subscribe<CommandResortConfigItem>((message) =>
            {
                // find all items
                var resortedItems = new List<IConfigItem>();
                message.Items.ToList().ForEach(item =>
                {
                    IConfigItem cfg = ConfigItems.Find(i => i.GUID == item.GUID);
                    if (cfg == null) return;
                    resortedItems.Add(cfg);
                    ConfigItems.Remove(cfg);
                });

                var currentIndex = message.NewIndex;

                resortedItems.ForEach(item =>
                {
                    ConfigItems.Insert(currentIndex, item);
                    currentIndex++;
                });

                MessageExchange.Instance.Publish(new ConfigValueFullUpdate(ActiveConfigIndex, ConfigItems));
                OnConfigHasChanged?.Invoke(ConfigItems, null);
            });

            MessageExchange.Instance.Subscribe<CommandActiveConfigFile>((message) =>
            {
                ActiveConfigIndex = message.index;
                ClearConfigItemStatus();
            });

            MessageExchange.Instance.Subscribe<CommandFileContextMenu>((message) =>
            {
                if (message.Index>=Project.ConfigFiles.Count)
                {
                    Log.Instance.log($"Command {message.Action} - Index not found: {message.Index}", LogSeverity.Error);
                    return;
                }

                var file = Project.ConfigFiles[message.Index];

                if (file == null)
                {
                    Log.Instance.log($"Command {message.Action} - File not found: {message.File.FileName}", LogSeverity.Error);
                    return;
                }

                switch (message.Action)
                {
                    case CommandFileContextMenuAction.remove:
                        Project.ConfigFiles.RemoveAt(message.Index);
                        break;
                    case CommandFileContextMenuAction.rename:
                        file.Label = message.File.Label;
                        break;
                    case CommandFileContextMenuAction.export:
                        // this is not subscribed here
                        // see MainForm.cs for the subscription there.
                        break;
                    default:
                        return;
                }

                MessageExchange.Instance.Publish(Project);
                OnConfigHasChanged?.Invoke(this, null);
            });
        }

        private void HandleCommandUpdateConfigItem(ConfigItem item)
        {
            var configItem = ConfigItems.Find(i => i.GUID == item.GUID);
            if (configItem == null) return;

            configItem.Active = item.Active;
            configItem.Name = item.Name;
            MessageExchange.Instance.Publish(new ConfigValuePartialUpdate(configItem));
            OnConfigHasChanged?.Invoke(new IConfigItem[] { configItem }, null);
        }

        private void ModuleCache_ModuleConnected(object sender, EventArgs e)
        {
            TestModeStop();
            Stop();
            this.OnModuleConnected?.Invoke(sender, e);
        }

        private void sim_AirCraftChanged(object sender, string e)
        {
            Log.Instance.log($"Aircraft change detected: [{e}] ({sender.ToString()})", LogSeverity.Info);
            OnSimAircraftChanged?.Invoke(sender, e);
        }

        private void simConnect_AirCraftPathChanged(object sender, string e)
        {
            Log.Instance.log($"Aircraft path changed: [{e}]", LogSeverity.Info);
            OnSimAircraftPathChanged?.Invoke(sender, e);
        }

        public Dictionary<String, MobiFlightVariable> GetAvailableVariables()
        {
            Dictionary<String, MobiFlightVariable> variables = new Dictionary<string, MobiFlightVariable>();
            if (Project.ConfigFiles.Count == 0 || ActiveConfigIndex >= Project.ConfigFiles.Count)
            {
                return variables;
            }

            return Project.ConfigFiles[ActiveConfigIndex].GetAvailableVariables();
        }

        public void HandleWndProc(ref Message m)
        {
#if SIMCONNECT
            if (m.Msg == SimConnectMSFS.SimConnectCache.WM_USER_SIMCONNECT)
            {
                simConnectCache.ReceiveSimConnectMessage();
            }
#endif
        }

        private void simConnect_Closed(object sender, EventArgs e)
        {
            this.OnSimCacheClosed(sender, e);
        }

        private void simConnect_Connected(object sender, EventArgs e)
        {
            this.OnSimCacheConnected(sender, e);
        }

        private void simConnect_ConnectionLost(object sender, EventArgs e)
        {
            this.OnSimCacheConnectionLost(sender, e);
        }

        void mobiFlightCache_LookupFinished(object sender, EventArgs e)
        {
            OnInitialModuleLookupFinished?.Invoke(sender, e);
        }

        public void SetPollInterval(int value)
        {
            timer.Interval = value;
            xplaneCache.UpdateFrequencyPerSecond = (int)Math.Round(1000f / value);
        }

        public void SetTestModeInterval(int value)
        {
            testModeTimer.Interval = value;
        }

        public bool SimConnected()
        {
            return fsuipcCache.IsConnected()
#if SIMCONNECT
                || simConnectCache.IsConnected()
#endif
                || xplaneCache.IsConnected()
                || proSimCache.IsConnected()
                ;
        }

        public bool ModulesAvailable()
        {
#if MOBIFLIGHT
            return
#if ARCAZE
                arcazeCache.Available() ||
#endif
                mobiFlightCache.Available();
#endif
        }

        public void Start()
        {
            if (timer.Enabled) return;

            InitInputEventExecutor();
            simConnectCache.Start();
            xplaneCache.Start();

            scriptRunner.Start();

            // the timer has to be enabled before the 
            // on start actions are executed
            // otherwise the input events will not be executed.
            timer.Enabled = true;
            frontendUpdateTimer.Enabled = true;

            // Now we can execute the on start actions
            OnStartActions();

            // Force all the modules awake whenver run is activated
            mobiFlightCache.KeepConnectedModulesAwake(true);
        }

        private void InitInputEventExecutor()
        {
            _inputEventExecutors.Clear();

            foreach (var configFile in Project.ConfigFiles)
            {
                _inputEventExecutors[configFile] = new InputEventExecutor(
                    configFile.ConfigItems,
                    inputActionExecutionCache,
                    fsuipcCache,
                    simConnectCache,
                    xplaneCache,
                    mobiFlightCache,
                    joystickManager,
                    arcazeCache
                );
            }
        }

        public void Stop()
        {
            timer.Enabled = false;
            frontendUpdateTimer.Enabled = false;
            isExecuting = false;
#if ARCAZE
            arcazeCache.Clear();
#endif
            scriptRunner.Stop();
            mobiFlightCache.Stop();
            simConnectCache.Stop();
            xplaneCache.Stop();
            joystickManager.Stop();
            midiBoardManager.Stop();
            inputActionExecutionCache.Clear();
            mobiFlightCache.ActivateConnectedModulePowerSave();
            ClearConfigItemStatus();

            ClearErrorMessages();
        }

        private void ClearConfigItemStatus()
        {
            ConfigItems.ForEach(cfg =>
            {
                cfg?.Status?.Clear();
                cfg.Value = "";
                cfg.RawValue = "";
            });
        }

        public void AutoConnectStart()
        {
            autoConnectTimer.Start();
            AutoConnectTimer_TickAsync(null, null);
            Log.Instance.log("Started auto connect timer.", LogSeverity.Debug);
        }

        public void AutoConnectStop()
        {
            Log.Instance.log("Stopped auto connect timer.", LogSeverity.Debug);
            autoConnectTimer.Stop();
        }

        internal void OnInputConfigSettingsChanged(object sender, EventArgs e)
        {
            _inputEventExecutors.ToList().ForEach(executor => executor.Value.ClearCache());
        }

        public bool IsStarted()
        {
            return timer.Enabled;
        }

        public void TestModeStart()
        {
            testModeTimer.Enabled = true;

            OnTestModeStarted?.Invoke(this, null);
            Log.Instance.log("Started test timer.", LogSeverity.Debug);
        }

        public void TestModeStop()
        {
            testModeTimer.Enabled = false;

            // make sure every device is turned off
            mobiFlightCache.Stop();
            ConfigItems.ForEach(cfg => cfg?.Status?.Clear());
            ConfigItemInTestMode = null;
            ClearErrorMessages();

            OnTestModeStopped?.Invoke(this, null);
            Log.Instance.log("Stopped test timer.", LogSeverity.Debug);

        }

        public bool TestModeIsStarted()
        {
            return testModeTimer.Enabled;
        }

#if ARCAZE
        public ArcazeCache getModuleCache()
        {
            return arcazeCache;
        }
#endif

#if MOBIFLIGHT
        public MobiFlightCache getMobiFlightModuleCache()
        {
            return mobiFlightCache;
        }
#endif

#if ARCAZE
        public ArcazeCache getModules()
        {
            return arcazeCache;
        }
#endif

        public JoystickManager GetJoystickManager()
        {
            return joystickManager;
        }

        public MidiBoardManager GetMidiBoardManager()
        {
            return midiBoardManager;
        }

        public List<IModuleInfo> GetAllConnectedModulesInfo()
        {
            List<IModuleInfo> result = new List<IModuleInfo>();
#if ARCAZE
            result.AddRange(arcazeCache.getModuleInfo());
#endif
            result.AddRange(mobiFlightCache.GetModuleInfo());
            return result;
        }

        /// <summary>
        /// Shuts down all managers and caches and stops all threads for safe shutdown.
        /// </summary>
        public void Shutdown()
        {
            scriptRunner.Shutdown();
            autoConnectTimer.Stop();
#if ARCAZE
            arcazeCache.Shutdown();
#endif

#if MOBIFLIGHT
            mobiFlightCache.Shutdown();
#endif
            fsuipcCache.Disconnect();

#if SIMCONNECT
            simConnectCache.Disconnect();
#endif            
            if (Properties.Settings.Default.EnableJoystickSupport)
            {
                joystickManager.Shutdown();
            }
            if (Properties.Settings.Default.EnableMidiSupport)
            {
                midiBoardManager.Shutdown();
            }

            OnSimAircraftChanged -= scriptRunner.OnSimAircraftChanged;
            OnSimAircraftPathChanged -= scriptRunner.OnSimAircraftPathChanged;

            this.OnShutdown?.Invoke(this, new EventArgs());
        }

#if ARCAZE
        public void updateModuleSettings(Dictionary<string, ArcazeModuleSettings> arcazeSettings)
        {

            arcazeCache.updateModuleSettings(arcazeSettings);
            arcazeCache.Shutdown();
        }
#endif

        /// <summary>
        /// the main method where the configuration is parsed and executed
        /// </summary>
        private void ExecuteConfig()
        {
            if (
#if ARCAZE
                !arcazeCache.Available() &&
#endif
#if MOBIFLIGHT
                !mobiFlightCache.Available() &&

#endif
                !joystickManager.JoysticksConnected() &&

                !midiBoardManager.AreMidiBoardsConnected()
            ) return;



            // this is kind of sempahore to prevent multiple execution
            // in fact I don't know if this needs to be done in C# 
            if (isExecuting)
            {
                Log.Instance.log("Config execution taking too long, skipping execution.", LogSeverity.Warn);
                AppTelemetry.Instance.TrackSimpleEvent("SkippingExecuteConfig");
                return;
            }
            // now set semaphore to true
            isExecuting = true;
            fsuipcCache.Clear();

#if ARCAZE
            arcazeCache.clearGetValues();
#endif
            foreach (var configFile in _project.ConfigFiles)
            {
                var executor = new ConfigItemExecutor(
                                                  configFile.ConfigItems,
                                                  arcazeCache,
                                                  fsuipcCache,
                                                  simConnectCache,
                                                  xplaneCache,
                                                  mobiFlightCache,
                                                  joystickManager,
                                                  midiBoardManager,
                                                  inputActionExecutionCache,
                                                  ConfigItemInTestMode);

                foreach (var item in configFile.ConfigItems)
                {
                    var cfg = item as OutputConfigItem;
                    if (cfg == null || !cfg.Active) continue;

                    executor.Execute(cfg, updatedValues);
                }
            }

            isExecuting = false;
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void FsuipcCache_Closed(object sender, EventArgs e)
        {
            this.OnSimCacheClosed(sender, e);
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>        
        void FsuipcCache_Connected(object sender, EventArgs e)
        {
            this.OnSimCacheConnected(sender, e);
        }

        /// <summary>
        /// shows message to user and stops execution of timer
        /// </summary>
        void FsuipcCache_ConnectionLost(object sender, EventArgs e)
        {
            fsuipcCache.Disconnect();
            this.OnSimCacheConnectionLost(sender, e);
        }

        /// <summary>
        /// handler which sets the states of UI elements when timer gets started
        /// </summary>
        void timer_Started(object sender, EventArgs e)
        {
            OnStarted?.Invoke(this, new EventArgs());
        } //timer_Started()

        /// <summary>
        /// handler which sets the states of UI elements when timer gets stopped
        /// </summary>
        void timer_Stopped(object sender, EventArgs e)
        {
            OnStopped?.Invoke(this, new EventArgs());
        } //timer_Stopped

        /// <summary>
        /// Timer eventhandler
        /// </summary>        
        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                ExecuteConfig();
                mobiFlightCache.KeepConnectedModulesAwake();
                this.OnExecute?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error on config execution: {ex.Message}", LogSeverity.Error);
                Stop();
            }
        } //timer_Tick()

        void ModuleCache_ModuleRemoved(object sender, EventArgs e)
        {
            //_disconnectArcaze();
            this.OnModuleRemoved(sender, e);
            Stop();
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void ModuleCache_Closed(object sender, EventArgs e)
        {
            TestModeStop();
            this.OnShutdown?.Invoke(sender, e);
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void ModuleCache_Available(object sender, EventArgs e)
        {
            TestModeStop();
            Stop();
            this.OnModuleCacheAvailable?.Invoke(sender, e);
        }

        /// <summary>
        /// auto connect timer handler which tries to automagically connect to FSUIPC and Arcaze Modules        
        /// </summary>
        /// <remarks>
        /// auto connect is only done if current timer is not running since we suppose that an established
        /// connection was already available before the timer was started
        /// </remarks>
        void AutoConnectTimer_TickAsync(object sender, EventArgs e)
        {
            if (_autoConnectTimerRunning) return;
            _autoConnectTimerRunning = true;

#if ARCAZE
            if (arcazeCache.Enabled && !arcazeCache.Available())
            {
                arcazeCache.connect();
            }
#endif

            // Check only for available sims if not in Offline mode.
            if (true)
            {

                if (SimAvailable())
                {
                    if (LastDetectedSim != FlightSim.FlightSimType)
                    {
                        LastDetectedSim = FlightSim.FlightSimType;
                        OnSimAvailable?.Invoke(FlightSim.FlightSimType, null);
                    }

                    if (!fsuipcCache.IsConnected())
                    {
                        if (!simConnectCache.IsConnected() && !xplaneCache.IsConnected() && !proSimCache.IsConnected())
                        {
                            // we don't want to spam the log
                            // in case we have an active connection
                            // through a different type
                            Log.Instance.log("Trying auto connect to sim via FSUIPC", LogSeverity.Debug);
                        }

                        fsuipcCache.Connect();
                    }

                    if (!proSimCache.IsConnected())
                    {
                        if (!simConnectCache.IsConnected() && !xplaneCache.IsConnected() && !fsuipcCache.IsConnected())
                        {
                            // we don't want to spam the log
                            // in case we have an active connection
                            // through a different type
                            Log.Instance.log("Trying auto connect to sim via ProSim", LogSeverity.Debug);
                        }

                        proSimCache.Connect();
                    }
#if SIMCONNECT
                    if (FlightSim.FlightSimType == FlightSimType.MSFS2020 && !simConnectCache.IsConnected())
                    {
                        Log.Instance.log("Trying auto connect to sim via SimConnect (WASM)", LogSeverity.Debug);
                        simConnectCache.Connect();
                    }
#endif
                    if (FlightSim.FlightSimType == FlightSimType.XPLANE && !xplaneCache.IsConnected())
                    {
                        Log.Instance.log("Trying auto connect to sim via XPlane", LogSeverity.Debug);
                        xplaneCache.Connect();
                    }
                    // we return here to prevent the disabling of the timer
                    // so that autostart-feature can work properly
                    _autoConnectTimerRunning = false;
                    return;
                }
                else
                {
                    if (LastDetectedSim != FlightSimType.NONE)
                    {
                        OnSimUnavailable?.Invoke(LastDetectedSim, null);
                        LastDetectedSim = FlightSimType.NONE;
                    }
                }
            }

            _autoConnectTimerRunning = false;
        } //autoConnectTimer_Tick()

        public bool SimAvailable()
        {
            return FlightSim.IsAvailable();
        }

        /// <summary>
        /// this is the test mode routine where we simply toggle output pins and displays to provide a way for checking if correct settings for display are used
        /// </summary>        
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void testModeTimer_Tick(object sender, EventArgs args)
        {
            var OutputConfigItems = ConfigItems.Where(i => (i.GetType() == typeof(OutputConfigItem)) && i.Active && i.Device != null).ToList();
            if (OutputConfigItems.Count == 0) return;


            var lastTestedConfig = ConfigItemInTestMode;
            var lastTestedConfigIndex = OutputConfigItems.FindIndex(i => i.GUID == lastTestedConfig?.GUID);

            var currentIndex = (lastTestedConfigIndex + 1) % OutputConfigItems.Count;
            var currentConfig = OutputConfigItems[currentIndex];

            // Special case:
            // if we have only one config item and it is the same as the last tested one, we just toggle it off
            var toggleSingleConfig = (currentConfig.AreEqual(lastTestedConfig) && OutputConfigItems.Count == 1);

            if (lastTestedConfig?.Status?.ContainsKey(ConfigItemStatusType.Test) ?? false)
            {
                try
                {
                    ExecuteTestOff(lastTestedConfig as OutputConfigItem, true);
                    if (toggleSingleConfig)
                    {
                        return;
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    String RowDescription = lastTestedConfig.Name;
                    Log.Instance.log($"Error during test mode execution: module not connected > {RowDescription}. {ex.Message}", LogSeverity.Error);
                    OnTestModeException(new Exception(i18n._tr("uiMessageTestModeModuleNotConnected")), new EventArgs());
                }
                catch (Exception ex)
                {
                    // TODO: refactor - check if we can stop the execution and this way update the interface accordingly too
                    String RowDescription = lastTestedConfig.Name;
                    Log.Instance.log($"Error during test mode execution: {RowDescription}. {ex.Message}", LogSeverity.Error);
                    OnTestModeException(ex, new EventArgs());
                }
            }
            
            if (!currentConfig.Status.ContainsKey(ConfigItemStatusType.Test))
            {
                try
                {
                    ExecuteTestOn(currentConfig as OutputConfigItem, (currentConfig as OutputConfigItem).TestValue);
                }
                catch (IndexOutOfRangeException ex)
                {
                    String RowDescription = lastTestedConfig.Name;
                    Log.Instance.log($"Error during test mode execution: module not connected > {RowDescription}. {ex.Message}", LogSeverity.Error);
                    OnTestModeException(new Exception(i18n._tr("uiMessageTestModeModuleNotConnected")), new EventArgs());
                }
                catch (Exception ex)
                {
                    String RowDescription = currentConfig.Name;
                    Log.Instance.log($"Error during test mode execution: {RowDescription}. {ex.Message}", LogSeverity.Error);
                    OnTestModeException(ex, new EventArgs());
                }
            }
        }


        public void ExecuteTestOff(OutputConfigItem cfg, bool ResetConfigItemInTest)
        {
            if (ResetConfigItemInTest)
                ConfigItemInTestMode = null;

            var executor = new ConfigItemExecutor(ConfigItems,
                                                  arcazeCache,
                                                  fsuipcCache,
                                                  simConnectCache,
                                                  xplaneCache,
                                                  mobiFlightCache,
                                                  joystickManager,
                                                  midiBoardManager,
                                                  inputActionExecutionCache,
                                                  ConfigItemInTestMode);
            executor.ExecuteTestOff(cfg);
        }

        public void ExecuteTestOn(OutputConfigItem cfg, ConnectorValue value = null)
        {
            var executor = new ConfigItemExecutor(ConfigItems,
                                                  arcazeCache,
                                                  fsuipcCache,
                                                  simConnectCache,
                                                  xplaneCache,
                                                  mobiFlightCache,
                                                  joystickManager,
                                                  midiBoardManager,
                                                  inputActionExecutionCache,
                                                  ConfigItemInTestMode);
            executor.ExecuteTestOn(cfg, value);
            ConfigItemInTestMode = cfg;
        }


        private void ClearErrorMessages()
        {
            MessageExchange.Instance.Publish(new ConfigValueFullUpdate(ActiveConfigIndex, ConfigItems));
        }

#if MOBIFLIGHT
        void mobiFlightCache_OnButtonPressed(object sender, InputEventArgs e)
        {
            var msgEventLabel = $"{e.Name} => {e.DeviceLabel}";
            var updatedInputValues = new Dictionary<string, IConfigItem>();

            foreach (var executor in _inputEventExecutors.Values)
            {
                try
                {
                    var updatedValues = executor.Execute(e, IsStarted());
                    updatedValues.Keys.ToList().ForEach(k => updatedInputValues[k] = updatedValues[k]);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Error during input event execution: {ex.Message} - {e.DeviceId} => {e.DeviceLabel} ({e.Type})", LogSeverity.Error);
                }
            }

            updatedInputValues.Keys.ToList().ForEach(k => this.updatedValues[k] = updatedInputValues[k]);
        }

        private void UpdateInputPreconditions()
        {
            foreach (var cfgItem in ConfigItems)
            {
                try
                {
                    if (!cfgItem.Active) continue;

                    var cfg = cfgItem as InputConfigItem;
                    // this is not a input config item
                    if (cfg == null) continue;

                    var originalCfg = cfgItem.Clone() as InputConfigItem;

                    // if there are preconditions check and skip if necessary
                    if (cfg.Preconditions.Count > 0)
                    {
                        ConnectorValue currentValue = new ConnectorValue();

                        if (!PreconditionChecker.CheckPrecondition(cfg, currentValue, ConfigItems, arcazeCache, mobiFlightCache))
                        {
                            cfg.Status[ConfigItemStatusType.Precondition] = "not satisfied";
                        }
                        else
                        {
                            cfg.Status.Remove(ConfigItemStatusType.Precondition);
                        }

                        if (!cfg.Status.SequenceEqual(originalCfg.Status))
                        {
                            updatedValues[cfg.GUID] = cfg;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // probably the last row with no settings object 
                    continue;
                }
            }
        }
#endif

        public Dictionary<String, int> GetStatistics()
        {
            Dictionary<String, int> result = mobiFlightCache.GetStatistics();
            Dictionary<String, int> resultJoysticks = joystickManager.GetStatistics();
            Dictionary<String, int> resultMidiBoards = midiBoardManager.GetStatistics();

            foreach (String key in resultJoysticks.Keys)
            {
                result[key] = resultJoysticks[key];
            }
            foreach (String key in resultMidiBoards.Keys)
            {
                result[key] = resultMidiBoards[key];
            }

            result["arcazeCache.Enabled"] = 0;
#if ARCAZE
            if (arcazeCache.Enabled)
            {
                result["arcazeCache.Enabled"] = 1;
                result["arcazeCache.Count"] = arcazeCache.getModuleInfo().Count();
            }
#endif

            return result;
        }

        public SimConnectCache GetSimConnectCache()
        {
            return simConnectCache;
        }


        public Fsuipc2Cache GetFsuipcConnectCache()
        {
            return fsuipcCache;
        }

        public XplaneCache GetXlpaneConnectCache()
        {
            return xplaneCache;
        }

        public ProSim.ProSimCache GetProSimCache()
        {
            return proSimCache;
        }

        private void OnStartActions()
        {
            if (Properties.Settings.Default.AutoRetrigger)
            {
                var action = new RetriggerInputAction();
                action.execute(new CacheCollection()
                {
                    fsuipcCache = fsuipcCache,
                    simConnectCache = simConnectCache,
                    xplaneCache = xplaneCache,
                    proSimCache = proSimCache,
                    moduleCache = mobiFlightCache,
                    joystickManager = joystickManager,
                }, null, null);
            }
        }

        internal void OnMinimize(bool minimized)
        {
            updateFrontend = !minimized;
        }
    }
}
