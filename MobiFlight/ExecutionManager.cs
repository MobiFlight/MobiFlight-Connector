using MobiFlight.Base;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Incoming;
using MobiFlight.FSUIPC;
using MobiFlight.InputConfig;
using MobiFlight.SimConnectMSFS;
using MobiFlight.xplane;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MobiFlight
{
    public class ExecutionManager
    {
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

        public event EventHandler OnModuleConnected;
        public event EventHandler OnModuleRemoved;
        public event EventHandler OnModuleCacheAvailable;
        public event EventHandler OnShutdown;
        public event EventHandler OnInitialModuleLookupFinished;
        public event EventHandler OnJoystickConnectedFinished;
        public event EventHandler OnMidiBoardConnectedFinished;

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

#if ARCAZE
        readonly ArcazeCache arcazeCache = new ArcazeCache();
#endif

#if MOBIFLIGHT
        readonly MobiFlightCache mobiFlightCache = new MobiFlightCache();
#endif
        readonly JoystickManager joystickManager = new JoystickManager();
        readonly MidiBoardManager midiBoardManager = new MidiBoardManager();
        readonly InputActionExecutionCache inputActionExecutionCache = new InputActionExecutionCache();

        public List<OutputConfigItem> OutputConfigItems { get; set; } = new List<OutputConfigItem>();
        public List<InputConfigItem> InputConfigItems { get; set; } = new List<InputConfigItem>();

        Dictionary<String, List<InputConfigItem>> inputCache = new Dictionary<string, List<InputConfigItem>>();

        private bool _autoConnectTimerRunning = false;

        FlightSimType LastDetectedSim = FlightSimType.NONE;

        OutputConfigItem ConfigItemInTestMode = null;

        Dictionary<string, IConfigItem> lastUpdatedValues = new Dictionary<string, IConfigItem>();

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
#endif

            xplaneCache.ConnectionLost += new EventHandler(simConnect_ConnectionLost);
            xplaneCache.Connected += new EventHandler(simConnect_Connected);
            xplaneCache.Closed += new EventHandler(simConnect_Closed);
            xplaneCache.AircraftChanged += new EventHandler<string>(sim_AirCraftChanged);

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
            joystickManager.Connected += (sender, e) => { 
                joystickManager.Startup();
                OnJoystickConnectedFinished?.Invoke(sender, e);
            };            
            if (Properties.Settings.Default.EnableJoystickSupport)
            {
                joystickManager.Connect();
            }

            midiBoardManager.OnButtonPressed += new ButtonEventHandler(mobiFlightCache_OnButtonPressed);
            midiBoardManager.Connected += (sender, e) => { 
                midiBoardManager.Startup(); 
                OnMidiBoardConnectedFinished?.Invoke(sender, e);
            };
            if (Properties.Settings.Default.EnableMidiSupport)
            {
                midiBoardManager.Connect();
            }

            mobiFlightCache.Start();

            InitializeFrontendSubscriptions();
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
                if (message.Type == "InputAction")
                {
                    item = new InputConfigItem();
                }

                item.GUID = Guid.NewGuid().ToString();
                item.Name = message.Name;
                item.Active = true;
                if (item is OutputConfigItem)
                {
                    OutputConfigItems.Add((OutputConfigItem)item);
                }
                else
                {
                    InputConfigItems.Add((InputConfigItem)item);
                }
                MessageExchange.Instance.Publish(new ConfigValueUpdate() { ConfigItems = new List<IConfigItem>() { item } });
            });

            MessageExchange.Instance.Subscribe<CommandConfigContextMenu>((message) =>
            {
                IConfigItem cfg;
                switch (message.Action)
                {
                    case "delete":
                        if (message.Item.Type == "InputAction")
                        {
                            InputConfigItems.RemoveAll(i => i.GUID == message.Item.GUID);
                        }
                        else
                        {
                            OutputConfigItems.RemoveAll(i => i.GUID == message.Item.GUID);
                        }
                        break;

                    case "duplicate":
                        cfg = OutputConfigItems.Find(i => i.GUID == message.Item.GUID) as IConfigItem;
                        if (cfg != null)
                        {
                            var pos = OutputConfigItems.FindIndex(i => i.GUID == message.Item.GUID);
                            OutputConfigItems.Insert(pos, (cfg as OutputConfigItem).Duplicate() as OutputConfigItem);
                            break;
                        }
                        cfg = InputConfigItems.Find(i=> i.GUID == message.Item.GUID);

                        if (cfg != null)
                        {
                            var pos = InputConfigItems.FindIndex(i => i.GUID == message.Item.GUID);
                            InputConfigItems.Insert(pos, (cfg as InputConfigItem).Duplicate() as InputConfigItem);
                        }
                        break;
                    case "test":
                        cfg = OutputConfigItems.Find(i => i.GUID == message.Item.GUID);
                        if (cfg != null)
                        {
                            ExecuteTestOff(ConfigItemInTestMode, true);
                            ExecuteTestOn(cfg as OutputConfigItem);
                            break;
                        }
                        break;
                    
                    default:
                        return;
                }

                var cfgFile = new ConfigFile();
                cfgFile.OutputConfigItems = OutputConfigItems;
                cfgFile.InputConfigItems = InputConfigItems;

                MessageExchange.Instance.Publish(cfgFile);
            });
        }

        private void HandleCommandUpdateConfigItem(ConfigItem item)
        {
            IConfigItem configItem = OutputConfigItems.Find(i => i.GUID == item.GUID);
            if (configItem == null) { 
                configItem = InputConfigItems.Find(i => i.GUID == item.GUID);
            };

            if (configItem == null) return;
            configItem.Active = item.Active;
            configItem.Name = item.Name;
            MessageExchange.Instance.Publish(new ConfigValueUpdate() { ConfigItems = new List<IConfigItem>() { configItem } });
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

        internal Dictionary<String, MobiFlightVariable> GetAvailableVariables()
        {
            Dictionary<String, MobiFlightVariable> variables = new Dictionary<string, MobiFlightVariable>();

            // iterate over the config row by row
            foreach (var cfg in OutputConfigItems)
            {
                // cfg was not set yet, e.g. we created row and are still in edit mode and now we hit "Edit"
                if (cfg == null) continue;

                if (cfg.SourceType != SourceType.VARIABLE) continue;

                if (cfg.MobiFlightVariable == null) continue;

                if (variables.ContainsKey(cfg.MobiFlightVariable.Name)) continue;

                variables[cfg.MobiFlightVariable.Name] = cfg.MobiFlightVariable;
            }

            // iterate over the config row by row
            foreach (var cfg in InputConfigItems)
            {
                if (cfg == null) continue;

                List<InputAction> actions = cfg.GetInputActionsByType(typeof(VariableInputAction));

                if (actions == null) continue;

                actions.ForEach(action =>
                {
                    VariableInputAction a = (VariableInputAction)action;
                    if (variables.ContainsKey(a.Variable.Name)) return;

                    variables[a.Variable.Name] = a.Variable;
                });
            }

            return variables;
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

            simConnectCache.Start();
            xplaneCache.Start();

            // the timer has to be enabled before the 
            // on start actions are executed
            // otherwise the input events will not be executed.
            timer.Enabled = true;
            
            // Now we can execute the on start actions
            OnStartActions();

            // Force all the modules awake whenver run is activated
            mobiFlightCache.KeepConnectedModulesAwake(true);
        }

        public void Stop()
        {
            timer.Enabled = false;
            isExecuting = false;
#if ARCAZE
            arcazeCache.Clear();
#endif
            mobiFlightCache.Stop();
            simConnectCache.Stop();
            xplaneCache.Stop();
            joystickManager.Stop();
            midiBoardManager.Stop();
            inputCache.Clear();
            inputActionExecutionCache.Clear();
            mobiFlightCache.ActivateConnectedModulePowerSave();
            ClearErrorMessages();
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
            lock (inputCache)
            {
                inputCache.Clear();
            }
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

            var updatedValues = new Dictionary<string, IConfigItem>();

            // iterate over the config row by row
            foreach (var cfg in OutputConfigItems)
            {
                if (!cfg.Active) continue;

                if (cfg == null)
                {
                    // this can happen if a user activates (checkbox) a newly created config
                    continue;
                }

                // Don't execute a config that we are currently manually testing.
                var currentGuid = cfg.GUID;
                if (ConfigItemInTestMode != null && ConfigItemInTestMode.GUID == currentGuid)
                {
                    continue;
                }

                // If not connected to FSUIPC show an error message
                if (cfg.SourceType == SourceType.FSUIPC && !fsuipcCache.IsConnected())
                {
                    // TODO: REDESIGN: Review
                    // row.ErrorText = i18n._tr("uiMessageNoFSUIPCConnection");
                }
                else
#if SIMCONNECT
                // If not connected to SimConnect show an error message
                if (cfg.SourceType == SourceType.SIMCONNECT && !simConnectCache.IsConnected())
                {
                    // TODO: REDESIGN: Review
                    // row.ErrorText = i18n._tr("uiMessageNoSimConnectConnection");
                }
                else
#endif
                // If not connected to X-Plane show an error message
                if (cfg.SourceType == SourceType.XPLANE && !xplaneCache.IsConnected())
                {
                    // TODO: REDESIGN: Review
                    // row.ErrorText = i18n._tr("uiMessageNoXplaneConnection");
                }
                // In any other case remove the error message
                else
                {
                    // TODO: REDESIGN: Review
                    // row.ErrorText = "";
                }

                ConnectorValue value = ExecuteRead(cfg);
                ConnectorValue processedValue = value;

                // store the values for the input precondition checks
                updatedValues[currentGuid] = cfg.Clone() as ConfigItem;
                updatedValues[currentGuid].RawValue = value.ToString();
                updatedValues[currentGuid].Value = processedValue.ToString();
                
                List<ConfigRefValue> configRefs = GetRefs(cfg.ConfigRefs);

                try
                {
                    processedValue = value;
                    foreach (var modifier in cfg.Modifiers.Items.Where(m => m.Active))
                    {
                        processedValue = modifier.Apply(processedValue, configRefs);
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Transform error ({cfg.Name}): {ex.Message}", LogSeverity.Error);
                    // TODO: REDESIGN: Review
                    // row.ErrorText = $"{i18n._tr("uiMessageTransformError")}({ex.Message})";
                    continue;
                }

                // processedValue now has changed, so we need to update the value in the cache
                updatedValues[currentGuid].Value = processedValue.ToString();
                try
                {
                    // check preconditions
                    if (!CheckPrecondition(cfg, processedValue))
                    {
                        if (!cfg.Preconditions.ExecuteOnFalse)
                        {
                            // TODO: REDESIGN: Review
                            // row.ErrorText = i18n._tr("uiMessagePreconditionNotSatisfied");
                            continue;
                        }
                        else
                        {
                            processedValue.type = FSUIPCOffsetType.String;
                            processedValue.String = cfg.Preconditions.FalseCaseValue;
                        }
                    }
                    else
                    {
                        // TODO: REDESIGN: Review
                        //if (row.ErrorText == i18n._tr("uiMessagePreconditionNotSatisfied"))
                        //    row.ErrorText = "";
                    }

                    ExecuteDisplay(processedValue.ToString(), cfg);
                }
                catch (JoystickNotConnectedException jEx)
                {
                    // TODO: REDESIGN: Review
                    // row.ErrorText = jEx.Message;
                }
                catch (MidiBoardNotConnectedException mEx)
                {
                    // TODO: REDESIGN: Review
                    // row.ErrorText = mEx.Message;
                }
                catch (Exception exc)
                {
                    // TODO: REDESIGN: Review
                    String RowDescription = cfg.Name;
                    Exception resultExc = new ConfigErrorException(cfg.Name + ". " + exc.Message, exc);
                    // row.ErrorText = exc.Message;
                    throw resultExc;
                }
            }
            // this update will trigger potential writes to the offsets
            // that came from the inputs and are waiting to be written
            // fsuipcCache.Write();

            if (updatedValues.Count > 0)
            {
                // we have to compare updatedValues with lastUpdatedValues
                // and only publish the changes using the MessageExchange

                var changedValues = updatedValues
                                    .Where(
                                        kv => (
                                            !lastUpdatedValues.ContainsKey(kv.Key) ||
                                            kv.Value.Value != lastUpdatedValues[kv.Key].Value ||
                                            kv.Value.RawValue != lastUpdatedValues[kv.Key].RawValue
                                        )
                                    )
                                    .ToDictionary(kv => kv.Key, kv => kv.Value);
                if (changedValues.Count > 0)
                {
                    // TODO: EMIT Event
                    var update = new ConfigValueUpdate() { ConfigItems = changedValues.Values.ToList() };
                    MessageExchange.Instance.Publish(update);
                }
            }

            lastUpdatedValues = updatedValues;

            UpdateInputPreconditions();

            isExecuting = false;
        }

        private bool CheckPrecondition(ConfigItem cfg, ConnectorValue currentValue)
        {
            bool finalResult = true;
            bool result = true;
            bool logicOr = false; // false:and true:or
            ConnectorValue connectorValue = new ConnectorValue();

            foreach (Precondition p in cfg.Preconditions)
            {
                if (!p.PreconditionActive)
                {
                    continue;
                }

                OutputConfigItem tmp = new OutputConfigItem();

                switch (p.PreconditionType)
                {
#if ARCAZE
                    case "pin":
                        string serial = SerialNumber.ExtractSerial(p.PreconditionSerial);
                        string val = arcazeCache.getValue(serial, p.PreconditionPin, "repeat");

                        result = p.Evaluate(val, currentValue);
                        break;
#endif
                    case "variable":
                        var variableValue = mobiFlightCache.GetMobiFlightVariable(p.PreconditionRef);
                        if (variableValue == null) break;

                        result = p.Evaluate(variableValue);
                        break;
                    case "config":
                        // iterate over the config row by row
                        foreach (var outputConfig in OutputConfigItems)
                        {
                            // here we just don't have a match
                            if (outputConfig.GUID != p.PreconditionRef) continue;

                            // if inactive ignore?
                            if (!outputConfig.Active) break;

                            // was there an error on reading the value?
                            if (outputConfig.Value == null) break;

                            // read the value
                            string value = outputConfig.Value;

                            // if there hasn't been determined any value yet
                            // we cannot compare
                            if (value == "") break;

                            result = p.Evaluate(value, currentValue);
                            break;
                        }
                        break;
                } // switch

                if (logicOr)
                {
                    finalResult |= result;
                }
                else
                {
                    finalResult &= result;
                }

                logicOr = (p.PreconditionLogic == "or");
            } // foreach

            return finalResult;
        }

        private ConnectorValue ExecuteRead(OutputConfigItem cfg)
        {
            ConnectorValue result = new ConnectorValue();

            if (cfg.SourceType == SourceType.FSUIPC)
            {
                result = FsuipcHelper.executeRead(cfg, fsuipcCache);
            }
            else if (cfg.SourceType == SourceType.VARIABLE)
            {
                if (cfg.MobiFlightVariable.TYPE == MobiFlightVariable.TYPE_NUMBER) {
                    result.type = FSUIPCOffsetType.Float;
                    result.Float64 = mobiFlightCache.GetMobiFlightVariable(cfg.MobiFlightVariable.Name).Number;
                } else if (cfg.MobiFlightVariable.TYPE == MobiFlightVariable.TYPE_STRING)
                {
                    result.type = FSUIPCOffsetType.String;
                    result.String = mobiFlightCache.GetMobiFlightVariable(cfg.MobiFlightVariable.Name).Text;
                }
            }
            else if (cfg.SourceType == SourceType.XPLANE)
            {
                result.type = FSUIPCOffsetType.Float;
                result.Float64 = xplaneCache.readDataRef(cfg.XplaneDataRef.Path);
            }
            else
            {
                result.type = simConnectCache.GetSimVar(cfg.SimConnectValue.Value, out result.String, out result.Float64);
            }

            return result;
        }

        private string ExecuteDisplay(string value, OutputConfigItem cfg)
        {            
            string serial = SerialNumber.ExtractSerial(cfg.ModuleSerial);

            if (serial == "" && cfg.DeviceType != "InputAction") 
                return value.ToString();

            if (SerialNumber.IsJoystickSerial(serial) && cfg.DeviceType != "InputAction")
            {
                Joystick joystick = joystickManager.GetJoystickBySerial(serial);
                if(joystick != null)
                {
                    switch (cfg.DeviceType)
                    {
                        case OutputConfig.LcdDisplay.DeprecatedType:
                            joystick.SetLcdDisplay(cfg.LcdDisplay.Address, value);                            
                            break;

                        case "-":
                            // do nothing
                            break;

                        default: // LED Output                          
                            byte state = 0;
                            if (value != "0") state = 1;
                            joystick.SetOutputDeviceState(cfg.Pin.DisplayPin, state);
                            joystick.UpdateOutputDeviceStates();
                            joystick.Update();
                            break;
                    }
                } 
                else
                {
                    var joystickName = SerialNumber.ExtractDeviceName(cfg.ModuleSerial);
                    throw new JoystickNotConnectedException(i18n._tr($"{joystickName} not connected"));
                }
            }
            else if (SerialNumber.IsMidiBoardSerial(serial) && cfg.DeviceType != "InputAction")
            {
                MidiBoard midiBoard = midiBoardManager.GetMidiBoardBySerial(serial);
                if (midiBoard != null)
                {
                    byte state = 0;
                    if (value != "0") state = 1;
                    midiBoard.SetOutputDeviceState(cfg.Pin.DisplayPin, state);                             
                }
                else
                {
                    var midiBoardName = SerialNumber.ExtractDeviceName(cfg.ModuleSerial);
                    throw new MidiBoardNotConnectedException(i18n._tr($"{midiBoardName} not connected"));
                }
            }
            else if (serial.IndexOf("SN") != 0 && cfg.DeviceType != "InputAction")
            {
#if ARCAZE
                switch (cfg.DeviceType)
                {
                    case ArcazeLedDigit.TYPE:
                        var val = value.PadRight(cfg.LedModule.DisplayLedDigits.Count, cfg.LedModule.DisplayLedPaddingChar[0]);
                        if (cfg.LedModule.DisplayLedPadding) val = value.PadLeft(cfg.LedModule.DisplayLedPadding ? cfg.LedModule.DisplayLedDigits.Count : 0, cfg.LedModule.DisplayLedPaddingChar[0]);
                        arcazeCache.setDisplay(
                            serial,
                            cfg.LedModule.DisplayLedAddress,
                            cfg.LedModule.DisplayLedConnector,
                            cfg.LedModule.DisplayLedDigits,
                            cfg.LedModule.DisplayLedDecimalPoints,
                            val);
                        break;

                    case ArcazeBcd4056.TYPE:
                        arcazeCache.setBcd4056(serial,
                            cfg.BcdPins,
                            value);
                        break;

                    default:
                        arcazeCache.setValue(serial,
                            cfg.Pin.DisplayPin,
                            (value != "0" ? cfg.Pin.DisplayPinBrightness.ToString() : "0"));
                        break;
                }
#endif
            }
            else
            {
                switch (cfg.DeviceType)
                {
                    case ArcazeLedDigit.TYPE:
                        var decimalCount = value.Count(c => c=='.');

                        var val = value.PadRight(cfg.LedModule.DisplayLedDigits.Count + decimalCount, cfg.LedModule.DisplayLedPaddingChar[0]);
                        var decimalPoints = new List<string>(cfg.LedModule.DisplayLedDecimalPoints);

                        if (cfg.LedModule.DisplayLedPadding)
                        {
                            val = value.PadLeft(cfg.LedModule.DisplayLedPadding
                                    ? cfg.LedModule.DisplayLedDigits.Count + decimalCount
                                    : 0, cfg.LedModule.DisplayLedPaddingChar[0]);
                        }

                        if (!string.IsNullOrEmpty(cfg.LedModule.DisplayLedBrightnessReference))
                        {
                            string refValue = FindValueForRef(cfg.LedModule.DisplayLedBrightnessReference);

                            mobiFlightCache.SetDisplayBrightness(
                                serial,
                                cfg.LedModule.DisplayLedAddress,
                                cfg.LedModule.DisplayLedConnector,
                                refValue
                                );
                        }

                        var reverse = cfg.LedModule.DisplayLedReverseDigits;

                        mobiFlightCache.SetDisplay(
                            serial,
                            cfg.LedModule.DisplayLedAddress,
                            cfg.LedModule.DisplayLedConnector,
                            cfg.LedModule.DisplayLedDigits,
                            decimalPoints,
                            val,
                            reverse);

                        break;

                    //case ArcazeBcd4056.TYPE:
                    //    mobiFlightCache.setBcd4056(serial,
                    //        cfg.BcdPins,
                    //        value);
                    //    break;

                    case MobiFlightStepper.TYPE:
                        mobiFlightCache.SetStepper(
                            serial,
                            cfg.Stepper.Address,
                            value,
                            cfg.Stepper.InputRev,
                            cfg.Stepper.OutputRev,
                            cfg.Stepper.CompassMode,
                            cfg.Stepper.Speed,
                            cfg.Stepper.Acceleration
                        );
                        break;

                    case MobiFlightServo.TYPE:
                        mobiFlightCache.SetServo(
                            serial,
                            cfg.Servo.Address,
                            value,
                            int.Parse(cfg.Servo.Min),
                            int.Parse(cfg.Servo.Max),
                            Byte.Parse(cfg.Servo.MaxRotationPercent)
                        );
                        break;

                    case OutputConfig.LcdDisplay.DeprecatedType:
                        mobiFlightCache.SetLcdDisplay(
                            serial,
                            cfg.LcdDisplay,
                            value,
                            GetRefs(cfg.ConfigRefs)
                            );
                        break;

                    case MobiFlightShiftRegister.TYPE:
                        if (serial != null)
                        {
                            string outputValueShiftRegister = value;

                            if (outputValueShiftRegister != "0" && !cfg.Pin.DisplayPinPWM)
                                outputValueShiftRegister = cfg.Pin.DisplayPinBrightness.ToString();
                          
                            mobiFlightCache.SetShiftRegisterOutput(
                                serial,
                                cfg.ShiftRegister.Address,
                                cfg.ShiftRegister.Pin,
                                outputValueShiftRegister);
                        }
                        break;

                    case OutputConfig.CustomDevice.DeprecatedType:
                        mobiFlightCache.Set(serial, cfg.CustomDevice, value);
                        break;

                    case "InputAction":
                        int iValue = 0;
                        int.TryParse(value, out iValue);

                        List<ConfigRefValue> cfgRefs = GetRefs(cfg.ConfigRefs);
                        CacheCollection cacheCollection = new CacheCollection()
                        {
                            fsuipcCache = fsuipcCache,
                            simConnectCache = simConnectCache,
                            moduleCache = mobiFlightCache,
                            xplaneCache = xplaneCache,
                            joystickManager = joystickManager
                        };
                        
                        if (cfg.ButtonInputConfig != null)
                            inputActionExecutionCache.Execute(
                                cfg.ButtonInputConfig,
                                cacheCollection,
                                new InputEventArgs() { Value = iValue, StrValue = value },
                                cfgRefs
                            );
                        else
                        {
                            inputActionExecutionCache.Execute(
                                cfg.AnalogInputConfig,
                                cacheCollection,
                                new InputEventArgs() { Value = iValue, StrValue = value },
                                cfgRefs
                            );
                        }
                        break;

                    default:
                        string outputValue = value;

                        // so in case the pin is not explicily treated as PWM pin and 
                        // we have a value other than 0 (which is output OFF) 
                        // we will set the full brightness.
                        // This ensures backward compatibility.
                        if (outputValue != "0" && !cfg.Pin.DisplayPinPWM)
                            outputValue = cfg.Pin.DisplayPinBrightness.ToString();

                        mobiFlightCache.SetValue(serial,
                            cfg.Pin.DisplayPin,
                            outputValue);
                        break;
                }
            }

            return value.ToString();
        }

        private List<ConfigRefValue> GetRefs(ConfigRefList configRefs)
        {
            List<ConfigRefValue> result = new List<ConfigRefValue>();
            foreach (ConfigRef c in configRefs)
            {
                if (!c.Active) continue;
                String s = FindValueForRef(c.Ref);
                if (s == null) continue;
                result.Add(new ConfigRefValue(c, s));
            }
            return result;
        }

        private String FindValueForRef(String refId)
        {
            String result = null;
            // iterate over the config row by row
            foreach (var cfg in OutputConfigItems)
            {
                // here we just don't have a match
                if (cfg.GUID != refId) continue;

                // if inactive ignore?
                if (!cfg.Active) break;

                // was there an error on reading the value?
                if (cfg.Value == null) break;

                // read the value
                string value = cfg.Value;

                // if there hasn't been determined any value yet
                // we cannot compare
                if (value == "") break;
                result = value;
            }
            return result;
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
            if (true) { 

                if (SimAvailable())
                {
                    if (LastDetectedSim!= FlightSim.FlightSimType) {
                        LastDetectedSim = FlightSim.FlightSimType;
                        OnSimAvailable?.Invoke(FlightSim.FlightSimType, null);
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
                    if (LastDetectedSim != FlightSimType.NONE) {
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
            if (OutputConfigItems.Count == 0) return;

            var lastTestedConfig = OutputConfigItems[(testModeIndex - 1 + OutputConfigItems.Count) % OutputConfigItems.Count];

            string serial = "";
            string lastSerial = "";

            var tmpCfg = lastTestedConfig;

            if (lastTestedConfig.Active &&
                lastTestedConfig.ModuleSerial != null && (lastTestedConfig.ModuleSerial.Contains("/"))
            )
            {
                lastSerial = SerialNumber.ExtractSerial(tmpCfg.ModuleSerial);
                try
                {
                    ExecuteTestOff(tmpCfg, true);
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


            var row = OutputConfigItems[testModeIndex];

            while (
                !row.Active && // check for null since last row is empty and value is null
                row != lastTestedConfig)
            {
                testModeIndex = ++testModeIndex % OutputConfigItems.Count;
                row = OutputConfigItems[testModeIndex];
            } //while


            tmpCfg = row;

            if (tmpCfg != null && // this happens sometimes when a new line is added and still hasn't been configured
                (row != lastTestedConfig) &&
                 tmpCfg.ModuleSerial != null && tmpCfg.ModuleSerial.Contains("/")
            )
            {
                serial = SerialNumber.ExtractSerial(tmpCfg.ModuleSerial);

                // TODO:
                // REDESIGN: Send out a message that this is currently tested
                try
                {
                    ExecuteTestOn(tmpCfg, tmpCfg.TestValue);
                }
                catch (IndexOutOfRangeException ex)
                {
                    String RowDescription = lastTestedConfig.Name;
                    Log.Instance.log($"Error during test mode execution: module not connected > {RowDescription}. {ex.Message}", LogSeverity.Error);
                    OnTestModeException(new Exception(i18n._tr("uiMessageTestModeModuleNotConnected")), new EventArgs());
                }
                catch (Exception ex)
                {
                    String RowDescription = row.Name;
                    Log.Instance.log($"Error during test mode execution: {RowDescription}. {ex.Message}", LogSeverity.Error);
                    OnTestModeException(ex, new EventArgs());
                }
            }

            testModeIndex = ++testModeIndex % OutputConfigItems.Count;
        }


        public void ExecuteTestOff(OutputConfigItem cfg, bool ResetConfigItemInTest)
        {
            if (cfg == null) return;

            if (ResetConfigItemInTest)
                ConfigItemInTestMode = null;

            OutputConfigItem offCfg = (OutputConfigItem)cfg.Clone();
            
            if (offCfg.DeviceType == null) return;

            switch (offCfg.DeviceType)
            {
                case MobiFlightServo.TYPE:
                    ExecuteDisplay(offCfg.Servo.Min, offCfg);
                    break;

                case OutputConfig.LcdDisplay.DeprecatedType:
                    offCfg.LcdDisplay.Lines.Clear();
                    offCfg.LcdDisplay.Lines.Add(new string(' ', 20 * 4));
                    ExecuteDisplay(new string(' ', 20 * 4), offCfg);
                    break;

                case MobiFlightShiftRegister.TYPE:
                    // Needs to be called as othewise the default catches it which does not make sense. May be there should not be a default :-)
                    ExecuteDisplay("0", offCfg);
                    break;

                case "InputAction":
                    // Do nothing for the InputAction
                    break;

                default:
                    offCfg.LedModule.DisplayLedDecimalPoints = new List<string>();
                    ExecuteDisplay(offCfg.DeviceType == ArcazeLedDigit.TYPE ? "        " : "0", offCfg);
                    break;
            }
        }

        public void ExecuteTestOn(OutputConfigItem cfg, ConnectorValue value = null)
        {
            ConfigItemInTestMode = cfg;

            if (cfg.DeviceType == null) return;

            switch (cfg.DeviceType)
            {
                // the following execute displays assume that
                //
                // case 1) inside the config wizard, when we hit the test button
                // we will have an actual connector value, we use it - even if it is the empty string.
                //
                // case 2) when we trigger the global test mode
                // we won't have an actual connector value, and then
                // we will use a static test string, that is specific to the device.
                case MobiFlightStepper.TYPE:
                    ExecuteDisplay(value?.ToString() ?? cfg.Stepper.TestValue.ToString(), cfg);
                    break;

                case MobiFlightServo.TYPE:
                    ExecuteDisplay(value?.ToString() ?? cfg.Servo.Max, cfg);
                    break;

                case ArcazeLedDigit.TYPE:
                case OutputConfig.LcdDisplay.DeprecatedType:
                    ExecuteDisplay(value?.ToString() ?? "1234567890", cfg);
                    break;
                
                case MobiFlightShiftRegister.TYPE:
                    ExecuteDisplay(value?.ToString() ?? "1", cfg);
                    break;

                case MobiFlightCustomDevice.TYPE:
                    ExecuteDisplay(value?.ToString() ?? "1", cfg);
                    break;

                case "InputAction":
                    // Do nothing for the InputAction
                    break;

                default:
                    ExecuteDisplay(value?.ToString() ?? "255", cfg);
                    break;
            }
        }


        private void ClearErrorMessages()
        {
            // TODO: REDESIGN: Review
        }

#if MOBIFLIGHT
        void mobiFlightCache_OnButtonPressed(object sender, InputEventArgs e)
        {
            String inputKey = e.Serial + e.Type + e.DeviceId;
            String eventAction = "n/a";

            if (e.Type == DeviceType.Button)
            {
                eventAction = MobiFlightButton.InputEventIdToString(e.Value);
            }
            else if (e.Type == DeviceType.Encoder)
            {
                eventAction = MobiFlightEncoder.InputEventIdToString(e.Value);
            }
            else if (e.Type == DeviceType.InputShiftRegister)
            {
                eventAction = MobiFlightInputShiftRegister.InputEventIdToString(e.Value);
                // The inputKey gets the shifter external pin added to it if the input came from a shift register
                // This ensures caching works correctly when there are multiple channels on the same physical device
                inputKey = inputKey + e.ExtPin;
            }
            else if (e.Type == DeviceType.InputMultiplexer)
            {
                eventAction = MobiFlightInputMultiplexer.InputEventIdToString(e.Value);
//GCC CHECK
                // The inputKey gets the external pin no. added to it if the input came from a shift register
                // xThis ensures caching works correctly when there are multiple pins on the same physical device
                inputKey = inputKey + e.ExtPin;
            }
            else if (e.Type == DeviceType.AnalogInput)
            {
                eventAction = MobiFlightAnalogInput.InputEventIdToString(0) + " => " +e.Value;
            }

            var msgEventLabel = $"{e.Name} => {e.DeviceLabel} {(e.ExtPin.HasValue ? $":{e.ExtPin}" : "")} => {eventAction}";

            lock (inputCache)
			{
                if (!inputCache.ContainsKey(inputKey))
                {
                    inputCache[inputKey] = new List<InputConfigItem>();
                    // check if we have configs for this button
                    // and store it      

                    foreach (var cfg in InputConfigItems)
                    {
                        try
                        {
                            // item currently created and not saved yet.
                            if (cfg == null) continue;
                        
                        	if (cfg.ModuleSerial != null && 
                                cfg.ModuleSerial.Contains("/ " + e.Serial) && 
                               (cfg.DeviceName == e.DeviceId || 
                               // for backward compatibility we have to make this check
                               // because we used to have the label in the config
                               // but now we want to store the internal button identifier
                               // so that the label can change any time without breaking the config
                               (Joystick.IsJoystickSerial(cfg.ModuleSerial) && cfg.DeviceName == e.DeviceLabel)))
                        	{
                            	// Input shift registers have an additional check to see if the pin that changed matches the pin
                            	// assigned to the row. If not just skip this row. Without this every row that uses the input shift register
                            	// would get added to the input cache and fired even though the pins don't match.
//GCC CHECK
                            	if (e.Type == DeviceType.InputShiftRegister && cfg.inputShiftRegister != null && cfg.inputShiftRegister.ExtPin != e.ExtPin)
                            	{
                                	continue;
                            	}
                            	// similarly also for digital input Multiplexer
                            	if (e.Type == DeviceType.InputMultiplexer && cfg.inputMultiplexer != null && cfg.inputMultiplexer.DataPin != e.ExtPin)
                            	{
                                	continue;
                            	}
                                inputCache[inputKey].Add(cfg);
                            }
                    	}
                    	catch (Exception ex)
                    	{
                        	// probably the last row with no settings object 
                        	continue;
                    	}
                	}
            	}

            	// no config for this button found
            	if (inputCache[inputKey].Count == 0)
            	{
                	if (LogIfNotJoystickOrJoystickAxisEnabled(e.Serial, e.Type))
                    	    Log.Instance.log($"{msgEventLabel} =>  No config found.", LogSeverity.Warn);
                	return;
            	}
            }

            // Skip execution if not started
            if (!IsStarted())
            {
                Log.Instance.log($"{msgEventLabel} => Config not executed, MobiFlight not running", LogSeverity.Warn);
                return;
            }

            ConnectorValue currentValue = new ConnectorValue();
            CacheCollection cacheCollection = new CacheCollection()
            {
                fsuipcCache = fsuipcCache,
                simConnectCache = simConnectCache,
                xplaneCache = xplaneCache,
                moduleCache = mobiFlightCache,
                joystickManager = joystickManager
            };

            foreach (var cfg in inputCache[inputKey])
            {
                if (!cfg.Active)
                {
                    Log.Instance.log($"{msgEventLabel} => skipping \"{cfg.Name}\", config not active.", LogSeverity.Warn);
                    continue;
                }

                try
                {
                    // if there are preconditions check and skip if necessary
                    if (cfg.Preconditions.Count > 0)
                    {
                        if (!CheckPrecondition(cfg, currentValue))
                        {
                            // REDSIGN: Review if needed
                            // tuple.Item2.ErrorText = i18n._tr("uiMessagePreconditionNotSatisfied");
                            continue;
                        }
                        else
                        {
                            // REDSIGN: Review if needed
                            // tuple.Item2.ErrorText = "";
                        }
                    }

                    Log.Instance.log($"{msgEventLabel} => executing \"{cfg.Name}\"", LogSeverity.Info);

                    cfg.execute(
                        cacheCollection,
                        e,
                        GetRefs(cfg.ConfigRefs))
                        ;
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Error excuting \"{cfg.Name}\": {ex.Message}", LogSeverity.Error);
                }
            }

            //fsuipcCache.ForceUpdate();
        }

        private void UpdateInputPreconditions()
        {
            foreach (var cfg in InputConfigItems)
            {
                try
                {
                    if (!cfg.Active) continue;

                    // item currently created and not saved yet.
                    if (cfg == null) continue;

                    // if there are preconditions check and skip if necessary
                    if (cfg.Preconditions.Count > 0)
                    {
                        ConnectorValue currentValue = new ConnectorValue();
                        if (!CheckPrecondition(cfg, currentValue))
                        {
                            // REDSIGN: Review if needed
                            // gridViewRow.ErrorText = i18n._tr("uiMessagePreconditionNotSatisfied");
                            continue;
                        }
                        else
                        {
                            // REDSIGN: Review if needed
                            // gridViewRow.ErrorText = "";
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

        private bool LogIfNotJoystickOrJoystickAxisEnabled(String Serial, DeviceType type)
        {
            return !Joystick.IsJoystickSerial(Serial) ||
                    (Joystick.IsJoystickSerial(Serial) && (type != DeviceType.AnalogInput || Log.Instance.LogJoystickAxis));
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
            if(arcazeCache.Enabled)
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
                    moduleCache = mobiFlightCache,
                    joystickManager = joystickManager,
                }, null, null);
            }            
        }
    }

    public class ConfigRefValue
    {

        public ConfigRefValue() { }
        public ConfigRefValue(ConfigRef c, string value)
        {
            this.Value = value;
            this.ConfigRef = c.Clone() as ConfigRef;
        }

        public string Value { get; set; }
        public ConfigRef ConfigRef { get; set; }

    }
}
