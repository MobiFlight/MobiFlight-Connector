using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using MobiFlight;

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

        /// <summary>
        /// a semaphore to prevent multiple execution of timer callback
        /// </summary>
        protected bool isExecuting = false;

        /// <summary>
        /// the timer used for polling
        /// </summary>
        private EventTimer timer = new EventTimer();

        /// <summary>
        /// the timer used for auto connect of FSUIPC and Arcaze
        /// </summary>
        private Timer autoConnectTimer = new Timer();

        /// <summary>
        /// the timer used for execution of test mode
        /// </summary>
        private Timer testModeTimer = new Timer();
        int testModeIndex = 0;

        /// <summary>
        /// This list contains preparsed informations and cached values for the supervised FSUIPC offsets
        /// </summary>
        Fsuipc2Cache fsuipcCache = new Fsuipc2Cache();
        ArcazeCache arcazeCache = new ArcazeCache();

#if MOBIFLIGHT
        MobiFlightCache mobiFlightCache = new MobiFlightCache();
#endif
        DataGridView dataGridViewConfig = null;
        DataGridView inputsDataGridView = null;
        Dictionary<String, List<Tuple<InputConfigItem, DataGridViewRow>>> inputCache = new Dictionary<string, List<Tuple<InputConfigItem, DataGridViewRow>>>();

        private bool _autoConnectTimerRunning = false;

        public ExecutionManager(DataGridView dataGridViewConfig, DataGridView inputsDataGridView)
        {
            this.dataGridViewConfig = dataGridViewConfig;
            this.inputsDataGridView = inputsDataGridView;

            fsuipcCache.ConnectionLost += new EventHandler(fsuipcCache_ConnectionLost);
            fsuipcCache.Connected += new EventHandler(fsuipcCache_Connected);
            fsuipcCache.Closed += new EventHandler(fsuipcCache_Closed);

            arcazeCache.Connected += new EventHandler(arcazeCache_Connected);
            arcazeCache.Closed += new EventHandler(arcazeCache_Closed);
            arcazeCache.ConnectionLost += new EventHandler(arcazeCache_ConnectionLost);

            mobiFlightCache.Connected += new EventHandler(arcazeCache_Connected);
            mobiFlightCache.Closed += new EventHandler(arcazeCache_Closed);
            mobiFlightCache.ConnectionLost += new EventHandler(arcazeCache_ConnectionLost);
            
            timer.Interval = Properties.Settings.Default.PollInterval;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Stopped += new EventHandler(timer_Stopped);
            timer.Started += new EventHandler(timer_Started);

            autoConnectTimer.Interval = 5000;
            autoConnectTimer.Tick += new EventHandler(autoConnectTimer_Tick);
            autoConnectTimer.Start();            

            testModeTimer.Interval = Properties.Settings.Default.TestTimerInterval;
            testModeTimer.Tick += new EventHandler(testModeTimer_Tick);

#if MOBIFLIGHT
            mobiFlightCache.OnButtonPressed += new MobiFlightCache.ButtonEventHandler(mobiFlightCache_OnButtonPressed);
#endif
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
            return fsuipcCache.isConnected();
        }

        public bool ModulesConnected()
        {
#if MOBIFLIGHT
            return arcazeCache.isConnected() || mobiFlightCache.isConnected();
#else
            return arcazeCache.isConnected();
#endif
        }

        public void Start()
        {
            timer.Enabled = true;
        }

        public void Stop()
        {
            timer.Enabled = false;
            isExecuting = false;
            mobiFlightCache.Stop();
        }

        public void AutoConnectStart()
        {
            autoConnectTimer.Enabled = true;
        }

        public void AutoConnectStop()
        {
            autoConnectTimer.Enabled = false;
        }

        public bool IsStarted()
        {
            return timer.Enabled;
        }

        public void TestModeStart()
        {
            testModeTimer.Enabled = true;
        }

        public void TestModeStop()
        {
            testModeTimer.Enabled = false;
        }

        public bool TestModeIsStarted()
        {
            return testModeTimer.Enabled;
        }

        public ArcazeCache getModuleCache()
        {
            return arcazeCache;
        }
#if MOBIFLIGHT
        public MobiFlightCache getMobiFlightModuleCache()
        {
            return mobiFlightCache;
        }
#endif

        public ArcazeCache getModules()
        {
            return arcazeCache;
        }

        public List<IModuleInfo> getConnectedModulesInfo()
        {
            List<IModuleInfo> result = new List<IModuleInfo>();
            result.AddRange(arcazeCache.getModuleInfo());
            result.AddRange(mobiFlightCache.getModuleInfo());
            return result;
        }

        public void Shutdown()
        {
            autoConnectTimer.Stop();
            arcazeCache.disconnect();
#if MOBIFLIGHT
            mobiFlightCache.disconnect();
#endif
            fsuipcCache.disconnect(); 
            this.OnModulesDisconnected(this, new EventArgs());         
        }

        public void updateModuleSettings(Dictionary<string, ArcazeModuleSettings> arcazeSettings)
        {
            arcazeCache.updateModuleSettings(arcazeSettings);
            arcazeCache.disconnect();
        }

        /// <summary>
        /// the main method where the configuration is parsed and executed
        /// </summary>
        private void executeConfig()
        {
            // prevent execution if not connected to either FSUIPC or Arcaze
            if (!fsuipcCache.isConnected()) return;
            if (!arcazeCache.isConnected() && !mobiFlightCache.isConnected()) return;

            // this is kind of sempahore to prevent multiple execution
            // in fact I don't know if this needs to be done in C# 
            if (isExecuting) return;
            // now set semaphore to true
            isExecuting = true;
            fsuipcCache.Clear();
            arcazeCache.clearGetValues();

            // iterate over the config row by row
            foreach (DataGridViewRow row in dataGridViewConfig.Rows)
            {

                if (row.IsNewRow || !(bool)row.Cells["active"].Value) continue;

                // initialisiere den adapter
                //// nimm type von col.type
                //// nimm config von col.config                

                //// if !all valid continue                
                OutputConfigItem cfg = ((row.DataBoundItem as DataRowView).Row["settings"] as OutputConfigItem);

                // if (cfg.FSUIPCOffset == ArcazeConfigItem.FSUIPCOffsetNull) continue;


                ConnectorValue value = executeRead(cfg);
                ConnectorValue processedValue = value;

                row.DefaultCellStyle.ForeColor = Color.Empty;
                row.ErrorText = "";


                row.Cells["fsuipcValueColumn"].Value = value.ToString();
                row.Cells["fsuipcValueColumn"].Tag = value;

                // only none string values get transformed
                if (cfg.FSUIPCOffsetType != FSUIPCOffsetType.String)
                    processedValue = executeTransform(value, cfg);

                String strValue = executeComparison(processedValue, cfg);
                row.Cells["arcazeValueColumn"].Value = strValue;
                row.Cells["arcazeValueColumn"].Tag = processedValue;

                // check preconditions
                if (!checkPrecondition(cfg, processedValue))
                {
                    row.ErrorText = MainForm._tr("uiMessagePreconditionNotSatisfied");
                    continue;
                }

                executeDisplay(strValue, cfg);
            }
            isExecuting = false;
        }

        private bool checkPrecondition(IBaseConfigItem cfg, ConnectorValue currentValue)
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

                switch (p.PreconditionType)
                {
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

                        OutputConfigItem tmp = new OutputConfigItem();
                        tmp.ComparisonActive = true;
                        tmp.ComparisonValue = p.PreconditionValue;
                        tmp.ComparisonOperand = "=";
                        tmp.ComparisonIfValue = "1";
                        tmp.ComparisonElseValue = "0";

                        try
                        {
                            
                            String execResult = executeComparison(connectorValue, tmp);
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

                    case "config":
                        // iterate over the config row by row
                        foreach (DataGridViewRow row in dataGridViewConfig.Rows)
                        {
                            if ((row.DataBoundItem as DataRowView).Row["guid"].ToString() != p.PreconditionRef) continue;
                            if (row.Cells["arcazeValueColumn"].Value == null) break;
                            string value = row.Cells["arcazeValueColumn"].Value.ToString();

                            // if inactive ignore?
                            if (!(bool)row.Cells["active"].Value) break;

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
                                result = (executeComparison(connectorValue, tmp) == "1");
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

                logicOr = (p.PreconditionLogic == "or" ? true : false);
            } // foreach

            return finalResult;
        }

        private ConnectorValue executeRead(OutputConfigItem cfg)
        {
            ConnectorValue result = new ConnectorValue();

            if (cfg.FSUIPCOffsetType == FSUIPCOffsetType.String)
            {
                result.type = FSUIPCOffsetType.String;
                result.String = fsuipcCache.getStringValue(cfg.FSUIPCOffset, cfg.FSUIPCSize);
            }
            else if (cfg.FSUIPCOffsetType == FSUIPCOffsetType.Integer)
            {
                result = _executeReadInt(cfg);
            }
            else if (cfg.FSUIPCOffsetType == FSUIPCOffsetType.Float)
            {
                result = _executeReadFloat(cfg);
            }
            return result;
        }

        private ConnectorValue _executeReadInt(OutputConfigItem cfg)
        {
            ConnectorValue result = new ConnectorValue();
            switch (cfg.FSUIPCSize)
            {
                case 1:
                    Byte value8 = (Byte)(cfg.FSUIPCMask & fsuipcCache.getValue(
                                                cfg.FSUIPCOffset,
                                                cfg.FSUIPCSize
                                              ));
                    if (cfg.FSUIPCBcdMode)
                    {
                        FsuipcBCD val = new FsuipcBCD() { Value = value8 };
                        value8 = (Byte)val.asBCD;
                    }

                    result.type = FSUIPCOffsetType.Integer;
                    result.Int64 = value8;
                    break;
                case 2:
                    Int16 value16 = (Int16)(cfg.FSUIPCMask & fsuipcCache.getValue(
                                                cfg.FSUIPCOffset,
                                                cfg.FSUIPCSize
                                              ));
                    if (cfg.FSUIPCBcdMode)
                    {
                        FsuipcBCD val = new FsuipcBCD() { Value = value16 };
                        value16 = (Int16)val.asBCD;
                    }

                    result.type = FSUIPCOffsetType.Integer;
                    result.Int64 = value16;
                    break;
                case 4:
                    Int64 value32 = ((int)cfg.FSUIPCMask & fsuipcCache.getValue(
                                                cfg.FSUIPCOffset,
                                                cfg.FSUIPCSize
                                              ));

                    // no bcd support anymore for 4 byte

                    result.type = FSUIPCOffsetType.Integer;
                    result.Int64 = value32;
                    break;
                case 8:
                    Double value64 = (Double)fsuipcCache.getDoubleValue(
                                                cfg.FSUIPCOffset,
                                                cfg.FSUIPCSize
                                                );

                    result.type = FSUIPCOffsetType.Float;
                    result.Float64 = (int)(Math.Round(value64, 0));

                    break;
            }
            return result;
        }

        private ConnectorValue _executeReadFloat(OutputConfigItem cfg)
        {
            ConnectorValue result = new ConnectorValue();
            result.type = FSUIPCOffsetType.Float;
            switch (cfg.FSUIPCSize)
            {
                case 4:
                    Double value32 = fsuipcCache.getFloatValue(
                                                cfg.FSUIPCOffset,
                                                cfg.FSUIPCSize
                                              );

                    result.Float64 = value32;
                    break;
                case 8:
                    Double value64 = (Double)fsuipcCache.getDoubleValue(
                                                cfg.FSUIPCOffset,
                                                cfg.FSUIPCSize
                                                );

                    result.Float64 = (int)(Math.Round(value64, 0));

                    break;
            }
            return result;
        }

        private ConnectorValue executeTransform(ConnectorValue value, OutputConfigItem cfg)
        {
            double tmpValue;

            switch (value.type)
            {
                case FSUIPCOffsetType.Integer:
                    tmpValue = value.Int64;
                    tmpValue = tmpValue * cfg.FSUIPCMultiplier;
                    value.Int64 = (Int64)Math.Floor(tmpValue);
                    break;

                /*case FSUIPCOffsetType.UnsignedInt:
                    tmpValue = value.Uint64;
                    tmpValue = tmpValue * cfg.FSUIPCMultiplier;
                    value.Uint64 = (UInt64)Math.Floor(tmpValue);
                    break;*/

                case FSUIPCOffsetType.Float:
                    value.Float64 = Math.Floor(value.Float64 * cfg.FSUIPCMultiplier);
                    break;

                // nothing to do in case of string
            }
            return value;
        }

        private string executeComparison(ConnectorValue connectorValue, OutputConfigItem cfg)
        {
            string result = null;
            if (connectorValue.type == FSUIPCOffsetType.String)
            {
                return _executeStringComparison(connectorValue, cfg);
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
                var ce = new NCalc.Expression(result);
                try
                {
                    result = (ce.Evaluate()).ToString();
                }
                catch
                {
                    Log.Instance.log("checkPrecondition : Exception on NCalc evaluate", LogSeverity.Warn);
                    throw new Exception(MainForm._tr("uiMessageErrorOnParsingExpression"));
                }
            }

            return result;
        }

        private string _executeStringComparison(ConnectorValue connectorValue, OutputConfigItem cfg)
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

        private string executeDisplay(string value, OutputConfigItem cfg)
        {
            string serial = "";
            if (cfg.DisplaySerial.Contains("/"))
            {
                serial = cfg.DisplaySerial.Split('/')[1].Trim();
            }

            if (serial == "") return value.ToString();

            if (serial.IndexOf("SN") != 0)
            {

                switch (cfg.DisplayType)
                {
                    case ArcazeLedDigit.TYPE:
                        arcazeCache.setDisplay(
                            serial,
                            cfg.DisplayLedAddress,
                            cfg.DisplayLedConnector,
                            cfg.DisplayLedDigits,
                            cfg.DisplayLedDecimalPoints,
                            value.PadLeft(cfg.DisplayLedPadding ? cfg.DisplayLedDigits.Count : 0, cfg.DisplayLedPaddingChar[0]));
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
            }
            else
            {
                switch (cfg.DisplayType)
                {
                    case ArcazeLedDigit.TYPE:
                        mobiFlightCache.setDisplay(
                            serial,
                            cfg.DisplayLedAddress,
                            cfg.DisplayLedConnector,
                            cfg.DisplayLedDigits,
                            cfg.DisplayLedDecimalPoints,
                            value.PadLeft(cfg.DisplayLedPadding ? cfg.DisplayLedDigits.Count : 0, cfg.DisplayLedPaddingChar[0]));
                        break;

                    //case ArcazeBcd4056.TYPE:
                    //    mobiFlightCache.setBcd4056(serial,
                    //        cfg.BcdPins,
                    //        value);
                    //    break;

                    case MobiFlight.MobiFlightStepper.TYPE:
                        mobiFlightCache.setStepper(
                            serial,
                            cfg.StepperAddress,
                            value,
                            int.Parse(cfg.StepperInputRev),
                            int.Parse(cfg.StepperOutputRev)
                        );
                        break;

                    case MobiFlight.MobiFlightServo.TYPE:
                        mobiFlightCache.setServo(
                            serial,
                            cfg.ServoAddress,
                            value,
                            int.Parse(cfg.ServoMin),
                            int.Parse(cfg.ServoMax),
                            Byte.Parse(cfg.ServoMaxRotationPercent)
                        );
                        break;

                    default:
                        mobiFlightCache.setValue(serial,
                            cfg.DisplayPin,
                            (value != "0" ? cfg.DisplayPinBrightness.ToString() : "0"));
                        break;
                }
            }

            return value.ToString();
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void fsuipcCache_Closed(object sender, EventArgs e)
        {
            this.OnSimCacheClosed(sender, e);
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>        
        void fsuipcCache_Connected(object sender, EventArgs e)
        {
            this.OnSimCacheConnected(sender, e);
        }

        /// <summary>
        /// shows message to user and stops execution of timer
        /// </summary>
        void fsuipcCache_ConnectionLost(object sender, EventArgs e)
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
            arcazeCache.Clear();
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
                executeConfig();
                if (this.OnExecute != null)
                    this.OnExecute(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Log.Instance.log("Error on config execution. " + ex.Message, LogSeverity.Error);
                isExecuting = false;
                timer.Enabled = false;
            }
        } //timer_Tick()

        void arcazeCache_ConnectionLost(object sender, EventArgs e)
        {
            //_disconnectArcaze();
            this.OnModuleConnectionLost(sender, e);
            Stop();
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void arcazeCache_Closed(object sender, EventArgs e)
        {
            TestModeStop();
            this.OnModulesDisconnected(sender, e);
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void arcazeCache_Connected(object sender, EventArgs e)
        {
            TestModeStop();
            Stop();
            this.OnModulesConnected(sender, e);
        }

        /// <summary>
        /// auto connect timer handler which tries to automagically connect to FSUIPC and Arcaze Modules        
        /// </summary>
        /// <remarks>
        /// auto connect is only done if current timer is not running since we suppose that an established
        /// connection was already available before the timer was started
        /// </remarks>
        void autoConnectTimer_Tick(object sender, EventArgs e)
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

            if (!arcazeCache.isConnected() 
#if MOBIFLIGHT
                && !mobiFlightCache.isConnected())
#endif
            {
                arcazeCache.connect(); //  _initializeArcaze();
#if MOBIFLIGHT
                mobiFlightCache.connect();
#endif
            }
            //if (!arcazeCache.isConnected()) arcazeCache.connect();
            //if (!mobiFlightCache.isConnected()) mobiFlightCache.connect();

            if (SimAvailable() && !fsuipcCache.isConnected())
            {
                fsuipcCache.connect();
                // we return here to prevent the disabling of the timer
                // so that autostart-feature can work properly
                _autoConnectTimerRunning = false;
                return;
            }

            // this line here provokes a timer stop event each time
            // and therefore the icon for starting the app will get enabled
            // @see timer_Stopped
            timer.Enabled = false;
            _autoConnectTimerRunning = false;
        } //autoConnectTimer_Tick()

        private bool SimAvailable()
        {
            return fsuipcCache.IsAvailable();
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

            if (
                 cfg != null &&
                (cfg.FSUIPCOffset != OutputConfigItem.FSUIPCOffsetNull) &&
                ((bool)lastRow.Cells["active"].Value) &&
                (cfg.DisplaySerial.Contains("/"))
            )
            {
                lastSerial = cfg.DisplaySerial.Split('/')[1].Trim();
                lastRow.Selected = false;
                try
                {
                    executeTestOff(cfg);
                }
                catch (Exception ex)
                {
                    // TODO: refactor - check if we can stop the execution and this way update the interface accordingly too
                    Log.Instance.log("Error on Test Mode execution. " + ex.Message, LogSeverity.Error);
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
                 cfg.FSUIPCOffset != OutputConfigItem.FSUIPCOffsetNull &&
                 cfg.DisplaySerial.Contains("/"))
            {
                serial = cfg.DisplaySerial.Split('/')[1].Trim();
                row.Selected = true;

                try
                {
                    executeTestOn(cfg);
                }
                catch (ConfigErrorException ex)
                {
                    Log.Instance.log("Error on TestMode execution. " + ex.Message, LogSeverity.Error);
                    OnTestModeException(ex, new EventArgs());
                }
            }

            testModeIndex = ++testModeIndex % dataGridViewConfig.RowCount;
        }


        public void executeTestOff(OutputConfigItem cfg)
        {
            OutputConfigItem offCfg = (OutputConfigItem) cfg.Clone();
            switch (offCfg.DisplayType)
            {
                case MobiFlightServo.TYPE:
                    executeDisplay(offCfg.ServoMin, offCfg);
                    break;

                default:
                    offCfg.DisplayLedDecimalPoints = new List<string>();
                    executeDisplay(offCfg.DisplayType == ArcazeLedDigit.TYPE ? "        " : "0", offCfg);
                    break;
            }
        }

        public void executeTestOn(OutputConfigItem cfg)
        {
            switch (cfg.DisplayType)
            {
                case MobiFlightStepper.TYPE:
                    executeDisplay((Int16.Parse(cfg.StepperInputRev)).ToString(), cfg);
                    break;

                case MobiFlightServo.TYPE:
                    executeDisplay(cfg.ServoMax, cfg);
                    break;

                default:
                    executeDisplay(cfg.DisplayType == ArcazeLedDigit.TYPE ? "12345678" : "8", cfg);
                    break;
            }
        }

#if MOBIFLIGHT
        void mobiFlightCache_OnButtonPressed(object sender, ButtonArgs e)
        {
            if (!IsStarted()) return;

            String inputKey = e.Serial+e.Type+e.ButtonId;
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
                        if (cfg.ModuleSerial.Contains("/ " + e.Serial) && cfg.Name == e.ButtonId)
                        {
                            inputCache[inputKey].Add(new Tuple<InputConfigItem, DataGridViewRow> (cfg, gridViewRow));
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
                Log.Instance.log("No config found for button: " + e.ButtonId + "@" + e.Serial, LogSeverity.Debug);
                return;
            }
            
            ConnectorValue currentValue = new ConnectorValue();

            foreach (Tuple<InputConfigItem, DataGridViewRow> tuple in inputCache[inputKey])
            {
                DataRow row = (tuple.Item2.DataBoundItem as DataRowView).Row;

                if (!(bool) row["active"]) continue;

                // if there are preconditions check and skip if necessary
                if (tuple.Item1.Preconditions.Count > 0)
                {
                    if (!checkPrecondition(tuple.Item1, currentValue)) continue;
                }

                tuple.Item1.execute(fsuipcCache, e);
            }

            fsuipcCache.ForceUpdate();
        }
#endif
    }
}
