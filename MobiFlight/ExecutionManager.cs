using MobiFlight.Base;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Incoming;
using MobiFlight.Execution;
using MobiFlight.FSUIPC;
using MobiFlight.InputConfig;
using MobiFlight.ProSim;
using MobiFlight.Scripts;
using MobiFlight.SimConnectMSFS;
using MobiFlight.xplane;
using System;
using System.Collections.Concurrent;
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
        ProSim.ProSimCacheInterface GetProSimCache();
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

        private readonly Timer frontendUpdateTimer = new Timer();

        /// <summary>
        /// This list contains preparsed informations and cached values for the supervised FSUIPC offsets
        /// </summary>
        readonly FSUIPCCacheInterface fsuipcCache;

        readonly SimConnectCacheInterface simConnectCache;
        readonly ProSim.ProSimCacheInterface proSimCache;

        readonly XplaneCacheInterface xplaneCache;

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

        public List<IConfigItem> ConfigItems
        {
            get
            {
                return _project.ConfigFiles.Count > ActiveConfigIndex
                    ? _project.ConfigFiles[ActiveConfigIndex].ConfigItems
                    : new List<IConfigItem>();
            }
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

        // ProSim connection retry tracking
        private int _proSimConnectionAttempts = 0;
        private bool _proSimConnectionDisabled = false;

        OutputConfigItem ConfigItemInTestMode = null;
        ConcurrentDictionary<string, IConfigItem> updatedValues = new ConcurrentDictionary<string, IConfigItem>();
        bool updateFrontend = true;

        public ExecutionManager(IntPtr handle)
            : this(handle, new XplaneCache(), new SimConnectCache(), new Fsuipc2Cache(), new ProSimCache())
        {
        }

        public ExecutionManager(
            IntPtr handle,
            XplaneCacheInterface xplaneCache,
            SimConnectCacheInterface simConnectCache,
            FSUIPCCacheInterface fsuipcCache,
            ProSimCacheInterface proSimCache)
        {
            this.xplaneCache = xplaneCache;
            this.simConnectCache = simConnectCache;
            this.fsuipcCache = fsuipcCache;
            this.proSimCache = proSimCache;

            this.fsuipcCache.ConnectionLost += new EventHandler(FsuipcCache_ConnectionLost);
            this.fsuipcCache.Connected += new EventHandler(FsuipcCache_Connected);
            this.fsuipcCache.Closed += new EventHandler(FsuipcCache_Closed);
            this.fsuipcCache.AircraftChanged += new EventHandler<string>(sim_AircraftChanged);

            this.simConnectCache.SetHandle(handle);
            this.simConnectCache.ConnectionLost += new EventHandler(simConnect_ConnectionLost);
            this.simConnectCache.Connected += new EventHandler(simConnect_Connected);
            this.simConnectCache.Closed += new EventHandler(simConnect_Closed);
            this.simConnectCache.AircraftChanged += new EventHandler<string>(sim_AircraftChanged);
            this.simConnectCache.AircraftPathChanged += new EventHandler<string>(simConnect_AircraftPathChanged);

            this.xplaneCache.ConnectionLost += new EventHandler(simConnect_ConnectionLost);
            this.xplaneCache.Connected += new EventHandler(simConnect_Connected);
            this.xplaneCache.Closed += new EventHandler(simConnect_Closed);
            this.xplaneCache.AircraftChanged += new EventHandler<string>(sim_AircraftChanged);

            this.proSimCache.ConnectionLost += new EventHandler(proSim_ConnectionLost);
            this.proSimCache.Connected += new EventHandler(proSim_Connected);
            this.proSimCache.Closed += new EventHandler(proSim_Closed);
            this.proSimCache.AircraftChanged += new EventHandler<string>(sim_AircraftChanged);

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
            
            scriptRunner = new ScriptRunner(joystickManager, this.simConnectCache);
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
            frontendUpdateTimer.Tick += FrontendUpdateTimer_Execute;

            mobiFlightCache.Start();
            InitializeFrontendSubscriptions();
        }

        private void FrontendUpdateTimer_Execute(object sender, EventArgs e)
        {
            if (!updateFrontend) return;

            UpdateInputPreconditions();

            if (updatedValues.Count == 0) return;

            var list = new List<IConfigValueOnlyItem>();

            updatedValues.Keys.ToList().ForEach(key =>
            {
                if(!updatedValues.TryRemove(key, out var value))
                    return;

                list.Add(new ConfigValueOnlyItem(value));
            });

            MessageExchange.Instance.Publish(new ConfigValueRawAndFinalUpdate(list));
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
                IConfigItem item = new OutputConfigItem() {
                    Source = SourceFactory.Create(Project.ToProjectInfo().Sim)
                };
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
                var ConfigItems = Project.ConfigFiles[message.SourceFileIndex].ConfigItems;

                message.Items.ToList().ForEach(item =>
                {
                    IConfigItem cfg = ConfigItems.Find(i => i.GUID == item.GUID);
                    if (cfg == null) return;
                    resortedItems.Add(cfg);
                    ConfigItems.Remove(cfg);
                });

                var targetFile = Project.ConfigFiles[message.TargetFileIndex];

                var currentIndex = message.NewIndex;

                resortedItems.ForEach(item =>
                {
                    targetFile.ConfigItems.Insert(currentIndex, item);
                    currentIndex++;
                });

                MessageExchange.Instance.Publish(new ConfigValueFullUpdate(message.SourceFileIndex, ConfigItems));
                MessageExchange.Instance.Publish(new ConfigValueFullUpdate(message.TargetFileIndex, targetFile.ConfigItems));

                OnConfigHasChanged?.Invoke(ConfigItems, null);
            });

            MessageExchange.Instance.Subscribe<CommandActiveConfigFile>((message) =>
            {
                ActiveConfigIndex = message.index;
                ClearConfigItemStatus();
            });

            MessageExchange.Instance.Subscribe<CommandFileContextMenu>((message) =>
            {
                if (message.Index >= Project.ConfigFiles.Count)
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

        private void sim_AircraftChanged(object sender, string e)
        {
            if (sender is FSUIPCCacheInterface && (xplaneCache.IsConnected() || simConnectCache.IsConnected()))
            {
                Log.Instance.log($"Aircraft change detected from {sender} but X-Plane or SimConnect are connected. Ignoring name change", LogSeverity.Info);
                return;
            }

            Log.Instance.log($"Aircraft change detected: [{e}] ({sender.ToString()})", LogSeverity.Info);
            OnSimAircraftChanged?.Invoke(sender, e);
        }

        private void simConnect_AircraftPathChanged(object sender, string e)
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
            if (m.Msg == SimConnectMSFS.SimConnectCache.WM_USER_SIMCONNECT)
            {
                simConnectCache.ReceiveSimConnectMessage();
            }
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

        private void proSim_Closed(object sender, EventArgs e)
        {
            this.OnSimCacheClosed(sender, e);
        }

        private void proSim_Connected(object sender, EventArgs e)
        {
            // Reset retry state on successful connection
            _proSimConnectionAttempts = 0;
            _proSimConnectionDisabled = false;
            this.OnSimCacheConnected(sender, e);
        }

        private void proSim_ConnectionLost(object sender, EventArgs e)
        {
            Log.Instance.log("ProSim connection lost.", LogSeverity.Warn);
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
                || simConnectCache.IsConnected()
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
            if (IsStarted()) return;

            InitInputEventExecutor();
            simConnectCache.Start();
            xplaneCache.Start();

            scriptRunner.StartUp();

            // the timer has to be enabled before the 
            // on start actions are executed
            // otherwise the input events will not be executed.
            timer.Start();
            frontendUpdateTimer.Start();

            // Now we can execute the on start actions
            OnStartActions();

            mobiFlightCache.StartKeepAwake();
        }

        private void InitInputEventExecutor()
        {
            _inputEventExecutors.Clear();

            if (Project == null || Project.ConfigFiles == null) return;

            foreach (var configFile in Project.ConfigFiles)
            {
                _inputEventExecutors[configFile] = new InputEventExecutor(
                    configFile.ConfigItems,
                    inputActionExecutionCache,
                    fsuipcCache,
                    simConnectCache,
                    xplaneCache,
                    mobiFlightCache,
                    proSimCache,
                    joystickManager,
                    arcazeCache
                );
            }
        }

        public void Stop()
        {
            timer.Stop();
            frontendUpdateTimer.Stop();
            mobiFlightCache.StopKeepAwake();

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
            if (TestModeIsStarted()) return;

            testModeTimer.Start();
            mobiFlightCache.StartKeepAwake();

            OnTestModeStarted?.Invoke(this, null);

            Log.Instance.log("Started test timer.", LogSeverity.Debug);
        }

        public void TestModeStop()
        {
            testModeTimer.Stop();

            mobiFlightCache.StopKeepAwake();

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

            simConnectCache.Disconnect();

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
                                                  proSimCache,
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
        /// Attempts to connect to ProSim with retry limits and user settings
        /// </summary>
        private void TryConnectToProSim()
        {
            // Skip if already connected, auto-connect disabled, or connection disabled for this session
            if (proSimCache.IsConnected() || !Properties.Settings.Default.ProSimAutoConnectEnabled || _proSimConnectionDisabled)
            {
                return;
            }

            var maxRetries = Properties.Settings.Default.ProSimMaxRetryAttempts;

            if (_proSimConnectionAttempts < maxRetries)
            {
                // Only log if no other sim connections are active to avoid spam
                if (!simConnectCache.IsConnected() && !xplaneCache.IsConnected() && !fsuipcCache.IsConnected())
                {
                    Log.Instance.log($"Trying auto connect to sim via ProSim (attempt {_proSimConnectionAttempts + 1}/{maxRetries})", LogSeverity.Debug);
                }

                _proSimConnectionAttempts++;
                proSimCache.Connect();
            }
            else if (_proSimConnectionAttempts == maxRetries)
            {
                Log.Instance.log($"ProSim connection failed after {maxRetries} attempts. Disabling auto-connect for this session.", LogSeverity.Warn);
                _proSimConnectionDisabled = true;
            }
        }

        /// <summary>
        /// Resets ProSim retry state when connection is successful
        /// </summary>
        private void ResetProSimRetryStateOnSuccess()
        {
            if (proSimCache.IsConnected() && _proSimConnectionAttempts > 0)
            {
                Log.Instance.log("ProSim connection successful. Resetting retry counter.", LogSeverity.Debug);
                _proSimConnectionAttempts = 0;
                _proSimConnectionDisabled = false;
            }
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

            // Try to connect to ProSim with retry limits and user settings
            TryConnectToProSim();

            // Reset retry counter if ProSim is connected
            ResetProSimRetryStateOnSuccess();

            // Check only for available sims if not in Offline mode.
            if (SimAvailable())
            {
                if (LastDetectedSim != FlightSim.FlightSimType)
                {
                    LastDetectedSim = FlightSim.FlightSimType;
                    OnSimAvailable?.Invoke(FlightSim.FlightSimType, null);
                }

                if (FlightSim.FlightSimType == FlightSimType.MSFS2020 && !simConnectCache.IsConnected())
                {
                    Log.Instance.log("Trying auto connect to sim via SimConnect (WASM)", LogSeverity.Debug);
                    simConnectCache.Connect();
                }

                if (FlightSim.FlightSimType == FlightSimType.XPLANE && !xplaneCache.IsConnected())
                {
                    Log.Instance.log("Trying auto connect to sim via XPlane", LogSeverity.Debug);
                    xplaneCache.Connect();
                }

                if (!fsuipcCache.IsConnected())
                {
                    if (!simConnectCache.IsConnected() && !xplaneCache.IsConnected())
                    {
                        // we don't want to spam the log
                        // in case we have an active connection
                        // through a different type
                        Log.Instance.log("Trying auto connect to sim via FSUIPC", LogSeverity.Debug);
                    }

                    fsuipcCache.Connect();
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
            if (!IsStarted() && !TestModeIsStarted())
            {
                // for the case that this is an individual test
                // without MobiFlight in Run-mode nor in test mode
                // only then we need to stop the keep awake messages
                mobiFlightCache.StopKeepAwake();
            }

            if (ResetConfigItemInTest)
                ConfigItemInTestMode = null;

            var executor = new ConfigItemExecutor(ConfigItems,
                                                  arcazeCache,
                                                  fsuipcCache,
                                                  simConnectCache,
                                                  xplaneCache,
                                                  mobiFlightCache,
                                                  proSimCache,
                                                  joystickManager,
                                                  midiBoardManager,
                                                  inputActionExecutionCache,
                                                  ConfigItemInTestMode);
            executor.ExecuteTestOff(cfg);
        }

        public void ExecuteTestOn(OutputConfigItem cfg, ConnectorValue value = null)
        {
            // for the case that this is an individual test
            // without MobiFlight in Run-mode nor in test mode
            // in all other cases it will already be running
            mobiFlightCache.StartKeepAwake();
            
            var executor = new ConfigItemExecutor(ConfigItems,
                                                  arcazeCache,
                                                  fsuipcCache,
                                                  simConnectCache,
                                                  xplaneCache,
                                                  mobiFlightCache,
                                                  proSimCache,
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

        private bool LogIfNotJoystickAxisOrJoystickAxisEnabled(String Serial, DeviceType type)
        {
            return (!Joystick.IsJoystickSerial(Serial) ||
                     (Joystick.IsJoystickSerial(Serial) && (type != DeviceType.AnalogInput || Log.Instance.LogJoystickAxis)));
        }

#if MOBIFLIGHT
        void mobiFlightCache_OnButtonPressed(object sender, InputEventArgs e)
        {
            var updatedInputValues = new Dictionary<string, IConfigItem>();
            var msgEventLabel = e.GetMsgEventLabel();

            if (LogIfNotJoystickAxisOrJoystickAxisEnabled(e.Serial, e.Type))
            {
                Log.Instance.log(msgEventLabel, LogSeverity.Info);
            }

            foreach (var executor in _inputEventExecutors.Values)
            {
                try
                {
                    var updatedValues = executor.Execute(e, IsStarted());
                    updatedValues.Keys.ToList().ForEach(k => updatedInputValues[k] = updatedValues[k]);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Error during input event execution: {ex.Message} - {msgEventLabel}", LogSeverity.Error);
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

        public SimConnectCacheInterface GetSimConnectCache()
        {
            return simConnectCache;
        }

        public FSUIPCCacheInterface GetFsuipcConnectCache()
        {
            return fsuipcCache;
        }

        public XplaneCacheInterface GetXPlaneConnectCache()
        {
            return xplaneCache;
        }

        public ProSim.ProSimCacheInterface GetProSimCache()
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

        public void ResetProSimConnectionState()
        {
            _proSimConnectionAttempts = 0;
            _proSimConnectionDisabled = false;
            Log.Instance.log("ProSim connection retry state reset.", LogSeverity.Debug);
        }

        public void ConnectToProSim()
        {
            ResetProSimConnectionState();
            Log.Instance.log("Manual ProSim connection attempt.", LogSeverity.Info);
            proSimCache.Connect();
        }
    }
}
