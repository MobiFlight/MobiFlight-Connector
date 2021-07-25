using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using MobiFlight;
using MobiFlight.FSUIPC;
using MobiFlight.Base;
using MobiFlight.SimConnectMSFS;
using MobiFlight.Config;
using MobiFlight.OutputConfig;

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
        public event EventHandler OnSimCacheClosed;
        public event EventHandler OnSimCacheConnected;
        public event EventHandler OnSimCacheConnectionLost;

        public event EventHandler OnModulesConnected;
        public event EventHandler OnModulesDisconnected;
        /* public event EventHandler OnModuleRemoved; */
        public event EventHandler OnModuleConnectionLost;
        public event EventHandler OnModuleLookupFinished;

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

#if ARCAZE
        readonly ArcazeCache arcazeCache = new ArcazeCache();
#endif
        public bool OfflineMode { get { return fsuipcCache.OfflineMode; } set { fsuipcCache.OfflineMode = value; } }

#if MOBIFLIGHT
        readonly MobiFlightCache mobiFlightCache = new MobiFlightCache();
#endif
        DataGridView dataGridViewConfig = null;
        DataGridView inputsDataGridView = null;
        Dictionary<String, List<Tuple<InputConfigItem, DataGridViewRow>>> inputCache = new Dictionary<string, List<Tuple<InputConfigItem, DataGridViewRow>>>();

        private bool _autoConnectTimerRunning = false;

        public ExecutionManager(DataGridView dataGridViewConfig, DataGridView inputsDataGridView, IntPtr handle)
        {
            this.dataGridViewConfig = dataGridViewConfig;
            this.inputsDataGridView = inputsDataGridView;

            fsuipcCache.ConnectionLost += new EventHandler(FsuipcCache_ConnectionLost);
            fsuipcCache.Connected += new EventHandler(FsuipcCache_Connected);
            fsuipcCache.Closed += new EventHandler(FsuipcCache_Closed);

#if SIMCONNECT
            simConnectCache.SetHandle(handle);
            simConnectCache.ConnectionLost += new EventHandler(simConnect_ConnectionLost);
            simConnectCache.Connected += new EventHandler(simConnect_Connected);
            simConnectCache.Closed += new EventHandler(simConnect_Closed);
#endif

#if ARCAZE
            arcazeCache.Connected += new EventHandler(ArcazeCache_Connected);
            arcazeCache.Closed += new EventHandler(ArcazeCache_Closed);
            arcazeCache.ConnectionLost += new EventHandler(ArcazeCache_ConnectionLost);
            arcazeCache.Enabled = Properties.Settings.Default.ArcazeSupportEnabled;
#endif

            mobiFlightCache.Connected += new EventHandler(ArcazeCache_Connected);
            mobiFlightCache.Closed += new EventHandler(ArcazeCache_Closed);
            mobiFlightCache.ConnectionLost += new EventHandler(ArcazeCache_ConnectionLost);
            mobiFlightCache.LookupFinished += new EventHandler(mobiFlightCache_LookupFinished);

            timer.Interval = Properties.Settings.Default.PollInterval;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Stopped += new EventHandler(timer_Stopped);
            timer.Started += new EventHandler(timer_Started);

            autoConnectTimer.Interval = 10000;
            autoConnectTimer.Tick += new EventHandler(AutoConnectTimer_TickAsync);

            testModeTimer.Interval = Properties.Settings.Default.TestTimerInterval;
            testModeTimer.Tick += new EventHandler(testModeTimer_Tick);

#if MOBIFLIGHT
            mobiFlightCache.OnButtonPressed += new MobiFlightCache.ButtonEventHandler(mobiFlightCache_OnButtonPressed);
#endif
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
            OnModuleLookupFinished?.Invoke(sender, e);
        }

        public void SetFsuipcInterval(int value)
        {
            timer.Interval = value;
        }

        public void SetTestModeInterval(int value)
        {
            testModeTimer.Interval = value;
        }

        public bool SimConnected()
        {
            return fsuipcCache.isConnected()
#if SIMCONNECT
                || simConnectCache.IsConnected()
#endif
                ;
        }

        public bool ModulesConnected()
        {
#if MOBIFLIGHT
            return
#if ARCAZE
                arcazeCache.isConnected() ||
#endif
                mobiFlightCache.isConnected();
#else
            return arcazeCache.isConnected();
#endif
        }

        public void Start()
        {
            simConnectCache.Start();
            timer.Enabled = true;
        }

        public void Stop()
        {
            timer.Enabled = false;
            isExecuting = false;
            mobiFlightCache.Stop();
            simConnectCache.Stop();
            ClearErrorMessages();
        }

        public void AutoConnectStart()
        {
            autoConnectTimer.Start();
            AutoConnectTimer_TickAsync(null, null);
            Log.Instance.log("ExecutionManager.AutoConnectStart:" + "Started auto connect timer", LogSeverity.Debug);
        }

        public void ReconnectSim()
        {
            fsuipcCache.disconnect();
            fsuipcCache.connect();
#if SIMCONNECT
            simConnectCache.Disconnect();
            simConnectCache.Connect();
#endif
        }

        public void AutoConnectStop()
        {
            Log.Instance.log("ExecutionManager.AutoConnectStop:" + "Stopped auto connect timer", LogSeverity.Debug);
            autoConnectTimer.Stop();
        }

        public bool IsStarted()
        {
            return timer.Enabled;
        }

        public void TestModeStart()
        {
            testModeTimer.Enabled = true;
            Log.Instance.log("ExecutionManager.TestModeStart:" + "Started test timer", LogSeverity.Info);
        }

        public void TestModeStop()
        {
            testModeTimer.Enabled = false;
            Log.Instance.log("ExecutionManager.TestModeStop:" + "Stopped test timer", LogSeverity.Info);
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

        public List<IModuleInfo> GetAllConnectedModulesInfo()
        {
            List<IModuleInfo> result = new List<IModuleInfo>();
#if ARCAZE
            result.AddRange(arcazeCache.getModuleInfo());
#endif
            result.AddRange(mobiFlightCache.getModuleInfo());
            return result;
        }

        public void Shutdown()
        {
            autoConnectTimer.Stop();
#if ARCAZE
            arcazeCache.disconnect();
#endif

#if MOBIFLIGHT
            mobiFlightCache.disconnect();
#endif
            fsuipcCache.disconnect();

#if SIMCONNECT
            simConnectCache.Disconnect();
#endif

            this.OnModulesDisconnected?.Invoke(this, new EventArgs());
        }

#if ARCAZE
        public void updateModuleSettings(Dictionary<string, ArcazeModuleSettings> arcazeSettings)
        {

            arcazeCache.updateModuleSettings(arcazeSettings);
            arcazeCache.disconnect();
        }
#endif

        /// <summary>
        /// the main method where the configuration is parsed and executed
        /// </summary>
        private void ExecuteConfig()
        {
            if (
#if ARCAZE
                !arcazeCache.isConnected() &&
#endif
#if MOBIFLIGHT
                !mobiFlightCache.isConnected()
#endif
            ) return;

            // this is kind of sempahore to prevent multiple execution
            // in fact I don't know if this needs to be done in C# 
            if (isExecuting) return;
            // now set semaphore to true
            isExecuting = true;
            fsuipcCache.Clear();

#if ARCAZE
            arcazeCache.clearGetValues();
#endif

            // iterate over the config row by row
            foreach (DataGridViewRow row in dataGridViewConfig.Rows)
            {
                // ignore the rows that haven't been saved yet (new row, the last one in the grid)
                // and the ones that are not checked active
                if (row.IsNewRow || !(bool)row.Cells["active"].Value) continue;

                // initialisiere den adapter
                //// nimm type von col.type
                //// nimm config von col.config                

                //// if !all valid continue                
                OutputConfigItem cfg = ((row.DataBoundItem as DataRowView).Row["settings"] as OutputConfigItem);

                // If not connected to FSUIPC show an error message
                if (cfg.SourceType == SourceType.FSUIPC && !fsuipcCache.isConnected())
                {
                    row.ErrorText = i18n._tr("uiMessageNoFSUIPCConnection");
                    continue;
                }
#if SIMCONNECT
                // If not connected to SimConnect show an error message
                if (cfg.SourceType == SourceType.SIMCONNECT && !simConnectCache.IsConnected())
                {
                    row.ErrorText = i18n._tr("uiMessageNoSimConnectConnection");
                    continue;
                }
#endif
                // if (cfg.FSUIPCOffset == ArcazeConfigItem.FSUIPCOffsetNull) continue;

                ConnectorValue value = ExecuteRead(cfg);
                ConnectorValue processedValue = value;

                row.DefaultCellStyle.ForeColor = Color.Empty;

                row.Cells["fsuipcValueColumn"].Value = value.ToString();
                row.Cells["fsuipcValueColumn"].Tag = value;

                // only none string values get transformed
                String strValue = "";
                try
                {
                    processedValue = ExecuteTransform(value, cfg);

                    strValue = ExecuteComparison(processedValue, cfg);
                }
                catch (Exception e)
                {
                    Log.Instance.log("Problem with transform. " + e.Message, LogSeverity.Error);
                    row.ErrorText = i18n._tr("uiMessageTransformError") + "(" + e.Message + ")";
                    continue;
                }

                String strValueAfterComparison = (string)strValue.Clone();
                strValue = ExecuteInterpolation(strValue, cfg);


                row.Cells["arcazeValueColumn"].Value = strValue;
                if (strValueAfterComparison != strValue) row.Cells["arcazeValueColumn"].Value += " (" + strValueAfterComparison + ")";
                row.Cells["arcazeValueColumn"].Tag = processedValue + " / " + strValueAfterComparison;

                // check preconditions
                if (!CheckPrecondition(cfg, processedValue))
                {
                    if (!cfg.Preconditions.ExecuteOnFalse)
                    {
                        row.ErrorText = i18n._tr("uiMessagePreconditionNotSatisfied");
                        continue;
                    }
                    else
                    {
                        strValue = cfg.Preconditions.FalseCaseValue;
                    }
                }
                else
                {
                    row.ErrorText = "";
                }

                try
                {
                    ExecuteDisplay(strValue, cfg);
                }
                catch (Exception exc)
                {
                    String RowDescription = ((row.Cells["description"]).Value as String);
                    Exception resultExc = new ConfigErrorException(RowDescription + ". " + exc.Message, exc);
                    throw resultExc;
                }
            }

            // this update will trigger potential writes to the offsets
            // that came from the inputs and are waiting to be written
            // fsuipcCache.Write();

            isExecuting = false;
        }

        private string ExecuteInterpolation(string strValue, OutputConfigItem cfg)
        {
            if (cfg.Interpolation.Count > 0 && cfg.Interpolation.Active)
            {
                strValue = Math.Round(cfg.Interpolation.Value(float.Parse(strValue)), 0).ToString();
            }

            return strValue;
        }

        private bool CheckPrecondition(IBaseConfigItem cfg, ConnectorValue currentValue)
        {
            bool finalResult = true;
            bool result = true;
            bool logicOr = false; // false:and true:or
            ConnectorValue connectorValue = new ConnectorValue();

            foreach (Precondition p in cfg.Preconditions)
            {
                if (!p.PreconditionActive)
                {
                    //Log.Instance.log(p.PreconditionLabel + " inactive - skip!", LogSeverity.Debug);
                    continue;
                }

                OutputConfigItem tmp = new OutputConfigItem();

                switch (p.PreconditionType)
                {
#if ARCAZE
                    case "pin":
                        string serial = "";
                        if (p.PreconditionSerial.Contains("/"))
                        {
                            serial = p.PreconditionSerial.Split('/')[1].Trim();
                        };

                        string val = arcazeCache.getValue(
                                        serial,
                                        p.PreconditionPin,
                                        "repeat");

                        connectorValue.type = FSUIPCOffsetType.Integer;
                        connectorValue.Int64 = Int64.Parse(val);

                        tmp = new OutputConfigItem();
                        tmp.ComparisonActive = true;
                        tmp.ComparisonValue = p.PreconditionValue;
                        tmp.ComparisonOperand = "=";
                        tmp.ComparisonIfValue = "1";
                        tmp.ComparisonElseValue = "0";

                        try
                        {

                            String execResult = ExecuteComparison(connectorValue, tmp);
                            //Log.Instance.log(p.PreconditionLabel + " - Pin - val:"+val+" - " + execResult + "==" + tmp.ComparisonIfValue, LogSeverity.Debug);
                            result = (execResult == tmp.ComparisonIfValue);
                        }
                        catch (FormatException e)
                        {
                            Log.Instance.log("checkPrecondition : Exception on comparison execution, wrong format", LogSeverity.Error);
                            // maybe it is a text string
                            // @todo do something in the future here
                        }
                        break;
#endif
                    case "config":
                        // iterate over the config row by row
                        foreach (DataGridViewRow row in dataGridViewConfig.Rows)
                        {
                            // the last item is null and we hit that if we don't find the reference
                            // because we deleted it for example
                            if ((row.DataBoundItem as DataRowView) == null) continue;

                            // here we just don't have a match
                            if ((row.DataBoundItem as DataRowView).Row["guid"].ToString() != p.PreconditionRef) continue;

                            // if inactive ignore?
                            if (!(bool)row.Cells["active"].Value) break;

                            // was there an error on reading the value?
                            if (row.Cells["arcazeValueColumn"].Value == null) break;

                            // read the value
                            string value = row.Cells["arcazeValueColumn"].Value.ToString();

                            // if there hasn't been determined any value yet
                            // we cannot compare
                            if (value == "") break;

                            tmp = new OutputConfigItem();
                            tmp.ComparisonActive = true;
                            tmp.ComparisonValue = p.PreconditionValue.Replace("$", currentValue.ToString());
                            if (tmp.ComparisonValue != p.PreconditionValue)
                            {
                                var ce = new NCalc.Expression(tmp.ComparisonValue);
                                try
                                {
                                    tmp.ComparisonValue = (ce.Evaluate()).ToString();
                                }
                                catch (Exception exc)
                                {
                                    //argh!
                                    Log.Instance.log("checkPrecondition : Exception on eval of comparison value", LogSeverity.Error);
                                }
                            }

                            tmp.ComparisonOperand = p.PreconditionOperand;
                            tmp.ComparisonIfValue = "1";
                            tmp.ComparisonElseValue = "0";

                            connectorValue.type = FSUIPCOffsetType.Integer;
                            if (!Int64.TryParse(value, out connectorValue.Int64))
                            {
                                // likely to be a string
                                connectorValue.type = FSUIPCOffsetType.String;
                                connectorValue.String = value;
                            }

                            try
                            {
                                result = (ExecuteComparison(connectorValue, tmp) == "1");
                            }
                            catch (FormatException e)
                            {
                                // maybe it is a text string
                                // @todo do something in the future here
                                Log.Instance.log("checkPrecondition : Exception on comparison execution, wrong format", LogSeverity.Error);
                            }
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
                if (cfg.FSUIPC.OffsetType == FSUIPCOffsetType.String)
                {
                    result.type = FSUIPCOffsetType.String;
                    result.String = fsuipcCache.getStringValue(cfg.FSUIPC.Offset, cfg.FSUIPC.Size);
                }
                else if (cfg.FSUIPC.OffsetType == FSUIPCOffsetType.Integer)
                {
                    result = ExecuteReadInt(cfg);
                }
                else if (cfg.FSUIPC.OffsetType == FSUIPCOffsetType.Float)
                {
                    result = ExecuteReadFloat(cfg);
                }
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
            else
            {
                result.type = FSUIPCOffsetType.Float;
                result.Float64 = simConnectCache.GetSimVar(cfg.SimConnectValue.Value);
            }


            return result;
        }

        private ConnectorValue ExecuteReadInt(OutputConfigItem cfg)
        {
            ConnectorValue result = new ConnectorValue();
            switch (cfg.FSUIPC.Size)
            {
                case 1:
                    Byte value8 = (Byte)(cfg.FSUIPC.Mask & fsuipcCache.getValue(
                                                cfg.FSUIPC.Offset,
                                                cfg.FSUIPC.Size
                                              ));
                    if (cfg.FSUIPC.BcdMode)
                    {
                        FsuipcBCD val = new FsuipcBCD() { Value = value8 };
                        value8 = (Byte)val.asBCD;
                    }

                    result.type = FSUIPCOffsetType.Integer;
                    result.Int64 = value8;
                    break;
                case 2:
                    Int16 value16 = (Int16)(cfg.FSUIPC.Mask & fsuipcCache.getValue(
                                                cfg.FSUIPC.Offset,
                                                cfg.FSUIPC.Size
                                              ));
                    if (cfg.FSUIPC.BcdMode)
                    {
                        FsuipcBCD val = new FsuipcBCD() { Value = value16 };
                        value16 = (Int16)val.asBCD;
                    }

                    result.type = FSUIPCOffsetType.Integer;
                    result.Int64 = value16;
                    break;
                case 4:
                    Int64 value32 = ((int)cfg.FSUIPC.Mask & fsuipcCache.getValue(
                                                cfg.FSUIPC.Offset,
                                                cfg.FSUIPC.Size
                                              ));

                    // no bcd support anymore for 4 byte

                    result.type = FSUIPCOffsetType.Integer;
                    result.Int64 = value32;
                    break;
                case 8:
                    Double value64 = (Double)fsuipcCache.getDoubleValue(
                                                cfg.FSUIPC.Offset,
                                                cfg.FSUIPC.Size
                                                );

                    result.type = FSUIPCOffsetType.Float;
                    result.Float64 = (int)(Math.Round(value64, 0));

                    break;
            }
            return result;
        }

        private ConnectorValue ExecuteReadFloat(OutputConfigItem cfg)
        {
            ConnectorValue result = new ConnectorValue();
            result.type = FSUIPCOffsetType.Float;
            switch (cfg.FSUIPC.Size)
            {
                case 4:
                    Double value32 = fsuipcCache.getFloatValue(
                                                cfg.FSUIPC.Offset,
                                                cfg.FSUIPC.Size
                                              );

                    result.Float64 = value32;
                    break;
                case 8:
                    Double value64 = (Double)fsuipcCache.getDoubleValue(
                                                cfg.FSUIPC.Offset,
                                                cfg.FSUIPC.Size
                                                );

                    result.Float64 = value64;

                    break;
            }
            return result;
        }

        private ConnectorValue ExecuteTransform(ConnectorValue value, OutputConfigItem cfg)
        {
            double tmpValue;
            List<ConfigRefValue> configRefs = GetRefs(cfg.ConfigRefs);

            switch (value.type)
            {
                case FSUIPCOffsetType.Integer:
                    tmpValue = value.Int64;
                    tmpValue = cfg.Transform.Apply(tmpValue, configRefs);
                    value.Int64 = (Int64)Math.Floor(tmpValue);
                    break;

                /*case FSUIPCOffsetType.UnsignedInt:
                    tmpValue = value.Uint64;
                    tmpValue = tmpValue * cfg.FSUIPCMultiplier;
                    value.Uint64 = (UInt64)Math.Floor(tmpValue);
                    break;*/

                case FSUIPCOffsetType.Float:
                    value.Float64 = Math.Floor(cfg.Transform.Apply(value.Float64, configRefs));
                    break;

                case FSUIPCOffsetType.String:
                    value.String = cfg.Transform.Apply(value.String);
                    break;
            }
            return value;
        }

        private string ExecuteComparison(ConnectorValue connectorValue, OutputConfigItem cfg)
        {
            string result = null;
            List<ConfigRefValue> configRefs = GetRefs(cfg.ConfigRefs);

            if (connectorValue.type == FSUIPCOffsetType.String)
            {
                return ExecuteStringComparison(connectorValue, cfg);
            }

            Double value = connectorValue.Int64;
            /*if (connectorValue.type == FSUIPCOffsetType.UnsignedInt) value = connectorValue.Uint64;*/
            if (connectorValue.type == FSUIPCOffsetType.Float) value = connectorValue.Float64;

            if (!cfg.ComparisonActive)
            {
                return value.ToString();
            }

            if (cfg.ComparisonValue == "")
            {
                return value.ToString();
            }

            Double comparisonValue = Double.Parse(cfg.ComparisonValue);
            string comparisonIfValue = cfg.ComparisonIfValue != "" ? cfg.ComparisonIfValue : value.ToString();
            string comparisonElseValue = cfg.ComparisonElseValue != "" ? cfg.ComparisonElseValue : value.ToString();

            switch (cfg.ComparisonOperand)
            {
                case "!=":
                    result = (value != comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case ">":
                    result = (value > comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case ">=":
                    result = (value >= comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case "<=":
                    result = (value <= comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case "<":
                    result = (value < comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case "=":
                    result = (value == comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                default:
                    result = (value > 0) ? "1" : "0";
                    break;
            }

            // apply ncalc logic
            if (result.Contains("$"))
            {
                result = result.Replace("$", value.ToString());

                foreach (ConfigRefValue configRef in configRefs)
                {
                    result = result.Replace(configRef.ConfigRef.Placeholder, configRef.Value);
                }

                var ce = new NCalc.Expression(result);
                try
                {
                    result = (ce.Evaluate()).ToString();
                }
                catch
                {
                    Log.Instance.log("checkPrecondition : Exception on NCalc evaluate", LogSeverity.Warn);
                    throw new Exception(i18n._tr("uiMessageErrorOnParsingExpression"));
                }
            }

            return result;
        }

        private string ExecuteStringComparison(ConnectorValue connectorValue, OutputConfigItem cfg)
        {
            string result = connectorValue.String;
            string value = connectorValue.String;

            if (!cfg.ComparisonActive)
            {
                return connectorValue.String;
            }

            string comparisonValue = cfg.ComparisonValue;
            string comparisonIfValue = cfg.ComparisonIfValue;
            string comparisonElseValue = cfg.ComparisonElseValue;

            switch (cfg.ComparisonOperand)
            {
                case "!=":
                    result = (value != comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case "=":
                    result = (value == comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
            }

            return result;
        }

        private string ExecuteDisplay(string value, OutputConfigItem cfg)
        {
            string serial = "";
            if (cfg.DisplaySerial.Contains("/"))
            {
                serial = cfg.DisplaySerial.Split('/')[1].Trim();
            }

            if (serial == "") return value.ToString();

            if (serial.IndexOf("SN") != 0)
            {
#if ARCAZE
                switch (cfg.DisplayType)
                {
                    case ArcazeLedDigit.TYPE:
                        var val = value.PadRight(cfg.DisplayLedDigits.Count, cfg.DisplayLedPaddingChar[0]);
                        if (cfg.DisplayLedPadding) val = value.PadLeft(cfg.DisplayLedPadding ? cfg.DisplayLedDigits.Count : 0, cfg.DisplayLedPaddingChar[0]);
                        arcazeCache.setDisplay(
                            serial,
                            cfg.DisplayLedAddress,
                            cfg.DisplayLedConnector,
                            cfg.DisplayLedDigits,
                            cfg.DisplayLedDecimalPoints,
                            val);
                        break;

                    case ArcazeBcd4056.TYPE:
                        arcazeCache.setBcd4056(serial,
                            cfg.BcdPins,
                            value);
                        break;

                    default:
                        arcazeCache.setValue(serial,
                            cfg.DisplayPin,
                            (value != "0" ? cfg.DisplayPinBrightness.ToString() : "0"));
                        break;
                }
#endif
            }
            else
            {
                switch (cfg.DisplayType)
                {
                    case ArcazeLedDigit.TYPE:


                        var val = value.PadRight(cfg.DisplayLedDigits.Count, cfg.DisplayLedPaddingChar[0]);
                        var decimalPoints = new List<string>(cfg.DisplayLedDecimalPoints);

                        if (cfg.DisplayLedPadding) val = value.PadLeft(cfg.DisplayLedPadding ? cfg.DisplayLedDigits.Count : 0, cfg.DisplayLedPaddingChar[0]);

                        if (!string.IsNullOrEmpty(cfg.DisplayLedBrightnessReference))
                        {
                            string refValue = FindValueForRef(cfg.DisplayLedBrightnessReference);

                            mobiFlightCache.setDisplayBrightness(
                                serial,
                                cfg.DisplayLedAddress,
                                cfg.DisplayLedConnector,
                                refValue
                                );
                        }

                        if (cfg.DisplayLedReverseDigits)
                        {
                            val = new string(val.ToCharArray().Reverse().ToArray());
                            for (int i = 0; i != decimalPoints.Count; i++)
                            {
                                decimalPoints[i] = (cfg.DisplayLedDigits.Count - int.Parse(decimalPoints[i]) - 1).ToString();
                            };
                        }

                        mobiFlightCache.setDisplay(
                            serial,
                            cfg.DisplayLedAddress,
                            cfg.DisplayLedConnector,
                            cfg.DisplayLedDigits,
                            decimalPoints,
                            val);

                        break;

                    //case ArcazeBcd4056.TYPE:
                    //    mobiFlightCache.setBcd4056(serial,
                    //        cfg.BcdPins,
                    //        value);
                    //    break;

                    case MobiFlightStepper.TYPE:
                        mobiFlightCache.setStepper(
                            serial,
                            cfg.StepperAddress,
                            value,
                            int.Parse(cfg.StepperInputRev),
                            int.Parse(cfg.StepperOutputRev),
                            cfg.StepperCompassMode
                        );
                        break;

                    case MobiFlightServo.TYPE:
                        mobiFlightCache.setServo(
                            serial,
                            cfg.ServoAddress,
                            value,
                            int.Parse(cfg.ServoMin),
                            int.Parse(cfg.ServoMax),
                            Byte.Parse(cfg.ServoMaxRotationPercent)
                        );
                        break;

                    case OutputConfig.LcdDisplay.Type:
                        mobiFlightCache.setLcdDisplay(
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

                            if (outputValueShiftRegister != "0" && !cfg.DisplayPinPWM)
                                outputValueShiftRegister = cfg.DisplayPinBrightness.ToString();
                          
                            mobiFlightCache.setShiftRegisterOutput(
                                serial,
                                cfg.ShiftRegister,
                                cfg.RegisterOutputPin,
                                outputValueShiftRegister);
                        }
                        break;

                    default:
                        string outputValue = value;

                        // so in case the pin is not explicily treated as PWM pin and 
                        // we have a value other than 0 (which is output OFF) 
                        // we will set the full brightness.
                        // This ensures backward compatibility.
                        if (outputValue != "0" && !cfg.DisplayPinPWM)
                            outputValue = cfg.DisplayPinBrightness.ToString();

                        mobiFlightCache.setValue(serial,
                            cfg.DisplayPin,
                            outputValue);
                        break;
                }
            }

            return value.ToString();
        }

        private List<ConfigRefValue> GetRefs(List<ConfigRef> configRefs)
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
            foreach (DataGridViewRow row in dataGridViewConfig.Rows)
            {
                // the last item is null and we hit that if we don't find the reference
                // because we deleted it for example
                if ((row.DataBoundItem as DataRowView) == null) continue;

                // here we just don't have a match
                if ((row.DataBoundItem as DataRowView).Row["guid"].ToString() != refId) continue;

                // if inactive ignore?
                if (!(bool)row.Cells["active"].Value) break;

                // was there an error on reading the value?
                if (row.Cells["arcazeValueColumn"].Value == null) break;

                // read the value
                string value = row.Cells["arcazeValueColumn"].Value.ToString();

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
            fsuipcCache.disconnect();
            this.OnSimCacheConnectionLost(sender, e);
        }

        /// <summary>
        /// handler which sets the states of UI elements when timer gets started
        /// </summary>
        void timer_Started(object sender, EventArgs e)
        {
            this.OnStarted(this, new EventArgs());
        } //timer_Started()

        /// <summary>
        /// handler which sets the states of UI elements when timer gets stopped
        /// </summary>
        void timer_Stopped(object sender, EventArgs e)
        {
            // just forget about current states if timer gets stopped
#if ARCAZE
            arcazeCache.Clear();
#endif
            inputCache.Clear();
            this.OnStopped(this, new EventArgs());
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
                Log.Instance.log("Error on config execution. " + ex.Message, LogSeverity.Error);
                isExecuting = false;
                timer.Enabled = false;
            }
        } //timer_Tick()

        void ArcazeCache_ConnectionLost(object sender, EventArgs e)
        {
            //_disconnectArcaze();
            this.OnModuleConnectionLost(sender, e);
            Stop();
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void ArcazeCache_Closed(object sender, EventArgs e)
        {
            TestModeStop();
            this.OnModulesDisconnected?.Invoke(sender, e);
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void ArcazeCache_Connected(object sender, EventArgs e)
        {
            TestModeStop();
            Stop();
            this.OnModulesConnected?.Invoke(sender, e);
        }

        /// <summary>
        /// auto connect timer handler which tries to automagically connect to FSUIPC and Arcaze Modules        
        /// </summary>
        /// <remarks>
        /// auto connect is only done if current timer is not running since we suppose that an established
        /// connection was already available before the timer was started
        /// </remarks>
        async void AutoConnectTimer_TickAsync(object sender, EventArgs e)
        {
            if (_autoConnectTimerRunning) return;
            _autoConnectTimerRunning = true;
            // check if timer is running... 
            // do nothing if so, since everything else has been checked before...            
            if (timer.Enabled || testModeTimer.Enabled)
            {
                _autoConnectTimerRunning = false;
                return;
            }

            if (
#if ARCAZE
                !arcazeCache.isConnected() &&
#endif
#if MOBIFLIGHT
                !mobiFlightCache.isConnected()
#endif
                )
            {
                Log.Instance.log("ExecutionManager.autoConnectTimer_Tick(): AutoConnect Modules", LogSeverity.Debug);
#if ARCAZE
                if (Properties.Settings.Default.ArcazeSupportEnabled)
                    arcazeCache.connect(); //  _initializeArcaze();
#endif
#if MOBIFLIGHT
                await mobiFlightCache.connectAsync();
#endif
            }
            //if (!arcazeCache.isConnected()) arcazeCache.connect();
            //if (!mobiFlightCache.isConnected()) mobiFlightCache.connect();

            if (SimAvailable())
            {
                OnSimAvailable?.Invoke(FlightSim.FlightSimType, null);

                Log.Instance.log("ExecutionManager.autoConnectTimer_Tick(): AutoConnect Sim", LogSeverity.Debug);

                if (!fsuipcCache.isConnected())
                    fsuipcCache.connect();
#if SIMCONNECT
                if (FlightSim.FlightSimType == FlightSimType.MSFS2020 && !simConnectCache.IsConnected())
                    simConnectCache.Connect();
#endif
                // we return here to prevent the disabling of the timer
                // so that autostart-feature can work properly
                _autoConnectTimerRunning = false;
                return;
            }
            else
            {
                Log.Instance.log("ExecutionManager.autoConnectTimer_Tick(): No Sim running", LogSeverity.Debug);
            }

            // this line here provokes a timer stop event each time
            // and therefore the icon for starting the app will get enabled
            // @see timer_Stopped
            timer.Enabled = false;
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
        /// <param name="e"></param>
        void testModeTimer_Tick(object sender, EventArgs e)
        {
            DataGridViewRow lastRow = dataGridViewConfig.Rows[(testModeIndex - 1 + dataGridViewConfig.RowCount) % dataGridViewConfig.RowCount];

            string serial = "";
            string lastSerial = "";

            OutputConfigItem cfg = new OutputConfigItem();
            cfg.DisplaySerial = "";
            if (lastRow.DataBoundItem != null)
            {
                cfg = ((lastRow.DataBoundItem as DataRowView).Row["settings"] as OutputConfigItem);
            }

            if (cfg != null &&
                // (cfg.FSUIPC.Offset != FsuipcOffset.OffsetNull) &&

                lastRow.Cells["active"].Value != null && ((bool)lastRow.Cells["active"].Value) &&
                (cfg.DisplaySerial.Contains("/"))
            )
            {
                lastSerial = cfg.DisplaySerial.Split('/')[1].Trim();
                lastRow.Selected = false;
                try
                {
                    ExecuteTestOff(cfg);
                }
                catch (IndexOutOfRangeException ex)
                {
                    String RowDescription = ((lastRow.DataBoundItem as DataRowView).Row["description"] as String);
                    Log.Instance.log("Error Test Mode execution. Module not connected > " + RowDescription + ". " + ex.Message, LogSeverity.Error);
                    OnTestModeException(new Exception(i18n._tr("uiMessageTestModeModuleNotConnected")), new EventArgs());
                }
                catch (Exception ex)
                {
                    // TODO: refactor - check if we can stop the execution and this way update the interface accordingly too
                    String RowDescription = ((lastRow.DataBoundItem as DataRowView).Row["description"] as String);
                    Log.Instance.log("Error on Test Mode execution. " + RowDescription + ". " + ex.Message, LogSeverity.Error);
                    OnTestModeException(ex, new EventArgs());
                }
            }


            DataGridViewRow row = dataGridViewConfig.Rows[testModeIndex];

            while (
                row.Cells["active"].Value != null && // check for null since last row is empty and value is null
                !(bool)row.Cells["active"].Value &&
                row != lastRow)
            {
                testModeIndex = ++testModeIndex % dataGridViewConfig.RowCount;
                row = dataGridViewConfig.Rows[testModeIndex];
            } //while


            cfg = new OutputConfigItem();

            // iterate over the config row by row            
            if (row.DataBoundItem != null &&
                (row.DataBoundItem as DataRowView).Row["settings"] != null) // this is needed
            // since we immediately store all changes
            // and therefore there may be missing a 
            // valid cfg item
            {
                cfg = ((row.DataBoundItem as DataRowView).Row["settings"] as OutputConfigItem);
            }

            if (cfg != null && // this happens sometimes when a new line is added and still hasn't been configured
                (dataGridViewConfig.RowCount > 1 && row != lastRow) &&
                 //cfg.FSUIPC.Offset != FsuipcOffset.OffsetNull &&
                 cfg.DisplaySerial != null && cfg.DisplaySerial.Contains("/")
            )
            {
                serial = cfg.DisplaySerial.Split('/')[1].Trim();
                row.Selected = true;

                try
                {
                    ExecuteTestOn(cfg);
                }
                catch (IndexOutOfRangeException ex)
                {
                    String RowDescription = ((lastRow.DataBoundItem as DataRowView).Row["description"] as String);
                    Log.Instance.log("Error Test Mode execution. Module not connected > " + RowDescription + ". " + ex.Message, LogSeverity.Error);
                    OnTestModeException(new Exception(i18n._tr("uiMessageTestModeModuleNotConnected")), new EventArgs());
                }
                catch (Exception ex)
                {
                    String RowDescription = ((row.DataBoundItem as DataRowView).Row["description"] as String);
                    Log.Instance.log("Error on TestMode execution. " + RowDescription + ". " + ex.Message, LogSeverity.Error);
                    OnTestModeException(ex, new EventArgs());
                }
            }

            testModeIndex = ++testModeIndex % dataGridViewConfig.RowCount;
        }


        public void ExecuteTestOff(OutputConfigItem cfg)
        {
            OutputConfigItem offCfg = (OutputConfigItem)cfg.Clone();
            switch (offCfg.DisplayType)
            {
                case MobiFlightServo.TYPE:
                    ExecuteDisplay(offCfg.ServoMin, offCfg);
                    break;

                case OutputConfig.LcdDisplay.Type:
                    offCfg.LcdDisplay.Lines.Clear();
                    offCfg.LcdDisplay.Lines.Add(new string(' ', 20 * 4));
                    ExecuteDisplay(new string(' ', 20 * 4), offCfg);
                    break;

                case MobiFlightShiftRegister.TYPE:
                    // Needs to be called as othewise the default catches it which does not make sense. May be there should not be a default :-)
                    ExecuteDisplay("0", offCfg);
                    break;

                default:
                    offCfg.DisplayLedDecimalPoints = new List<string>();
                    ExecuteDisplay(offCfg.DisplayType == ArcazeLedDigit.TYPE ? "        " : "0", offCfg);
                    break;
            }
        }

        public void ExecuteTestOn(OutputConfigItem cfg)
        {
            switch (cfg.DisplayType)
            {
                case MobiFlightStepper.TYPE:
                    ExecuteDisplay((Int16.Parse(cfg.StepperTestValue)).ToString(), cfg);
                    break;

                case MobiFlightServo.TYPE:
                    ExecuteDisplay(cfg.ServoMax, cfg);
                    break;

                case OutputConfig.LcdDisplay.Type:
                    ExecuteDisplay("1234567890", cfg);
                    break;
                
                case MobiFlightShiftRegister.TYPE:
                    ExecuteDisplay("1", cfg);
                    break;

                default:
                    ExecuteDisplay(cfg.DisplayType == ArcazeLedDigit.TYPE ? "12345678" : "255", cfg);
                    break;
            }
        }


        private void ClearErrorMessages()
        {
            foreach (DataGridViewRow row in dataGridViewConfig.Rows)
            {
                row.ErrorText = "";
            }

            foreach (DataGridViewRow row in inputsDataGridView.Rows)
            {
                row.ErrorText = "";
            }
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
            else if (e.Type == DeviceType.AnalogInput)
            {
                eventAction = MobiFlightAnalogInput.InputEventIdToString(0) + "=>" +e.Value;
            }

            if (!inputCache.ContainsKey(inputKey))
            {
                inputCache[inputKey] = new List<Tuple<InputConfigItem, DataGridViewRow>>();
                // check if we have configs for this button
                // and store it      

                foreach (DataGridViewRow gridViewRow in inputsDataGridView.Rows)
                {
                    try
                    {
                        if (gridViewRow.DataBoundItem == null) continue;

                        InputConfigItem cfg = ((gridViewRow.DataBoundItem as DataRowView).Row["settings"] as InputConfigItem);
                        if (cfg.ModuleSerial.Contains("/ " + e.Serial) && cfg.Name == e.DeviceId)
                        {
                            inputCache[inputKey].Add(new Tuple<InputConfigItem, DataGridViewRow>(cfg, gridViewRow));
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

                Log.Instance.log("No config found for " + e.Type + ": " + e.DeviceId + " (" + eventAction + ")" + "@" + e.Serial, LogSeverity.Debug);
                return;
            }

            Log.Instance.log("Config found for " + e.Type + ": " + e.DeviceId + " (" + eventAction + ")" + "@" + e.Serial, LogSeverity.Debug);

            // Skip execution if not started
            if (!IsStarted()) return;

            ConnectorValue currentValue = new ConnectorValue();

            foreach (Tuple<InputConfigItem, DataGridViewRow> tuple in inputCache[inputKey])
            {
                if ((tuple.Item2.DataBoundItem as DataRowView) == null)
                {
                    Log.Instance.log("mobiFlightCache_OnButtonPressed: tuple.Item2.DataBoundItem is NULL", LogSeverity.Debug);
                    continue;
                }

                DataRow row = (tuple.Item2.DataBoundItem as DataRowView).Row;

                if (!(bool)row["active"]) continue;

                // if there are preconditions check and skip if necessary
                if (tuple.Item1.Preconditions.Count > 0)
                {
                    if (!CheckPrecondition(tuple.Item1, currentValue))
                    {
                        tuple.Item2.ErrorText = i18n._tr("uiMessagePreconditionNotSatisfied");
                        continue;
                    }
                    else
                    {
                        tuple.Item2.ErrorText = "";
                    }
                }
#if SIMCONNECT
                tuple.Item1.execute(
                    fsuipcCache,
                    simConnectCache,
                    mobiFlightCache,
                    e,
                    GetRefs(tuple.Item1.ConfigRefs))
                    ;
#endif

            }

            //fsuipcCache.ForceUpdate();
        }
#endif

        public Dictionary<String, int> GetStatistics()
        {
            Dictionary<String, int> result = mobiFlightCache.GetStatistics();
            result["arcazeCache.Enabled"] = 0;
            if(arcazeCache.Enabled)
            {
                result["arcazeCache.Enabled"] = 1;
                result["arcazeCache.Count"] = arcazeCache.getModuleInfo().Count();
            }

            return result;
        }

        public SimConnectCache GetSimConnectCache()
        {
            return simConnectCache;
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
