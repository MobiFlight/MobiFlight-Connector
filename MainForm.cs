using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Resources;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml.Serialization;
using SimpleSolutions.Usb;

namespace ArcazeUSB
{
    public partial class MainForm : Form
    {
        public class CmdLineParams
        {
            bool autoRun = false;
            string configFile = null;

            public bool AutoRun {
                get { return autoRun; }
            }

            public string ConfigFile
            {
                get { return configFile; }
            }

            public CmdLineParams()
            {
                string[] args = Environment.GetCommandLineArgs();
                autoRun     = _hasCfgParam("autoRun", args);
                configFile = _getCfgParamValue("cfg", args, null);
            }

            /// <summary>
            /// check whether a config param is present or not
            /// </summary>
            /// <param name="key"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            bool _hasCfgParam(string key, string[] args)
            {
                return ((args.Length > 1) && (Array.IndexOf(args, "/" + key) != -1));
            }

            /// <summary>
            /// get a value for a given parameter, use default value if not present
            /// </summary>
            /// <param name="key"></param>
            /// <param name="args"></param>
            /// <param name="defValue"></param>
            /// <returns></returns>
            string _getCfgParamValue(string key, string[] args, string defValue)
            {
                string result = defValue;
                // The first commandline argument is always the executable path itself.
                if (args.Length > 1)
                {
                    int pos = -1;
                    if ((pos = Array.IndexOf(args, "/" + key)) != -1)
                    {
                        try
                        {
                            if (args[pos + 1][0] != '/') result = args[pos + 1];
                        }
                        catch (Exception e)
                        {
                            // do nothing
                        }
                    }
                } 
                return result;
            }

        }

        public static String Version = "3.9.1";
        public static String Build = "20131220";

        /// <summary>
        /// a semaphore to prevent multiple execution of timer callback
        /// </summary>
        protected bool isExecuting          = false;
        
        /// <summary>
        /// the timer used for polling
        /// </summary>
        private EventTimer timer                = new EventTimer ();

        /// <summary>
        /// the timer used for auto connect of FSUIPC and Arcaze
        /// </summary>
        private Timer autoConnectTimer          = new Timer ();

        /// <summary>
        /// the timer used for execution of test mode
        /// </summary>
        private Timer testModeTimer = new Timer();
        int testModeIndex = 0;

        /// <summary>
        /// the currently used filename of the loaded config file
        /// </summary>
        private string currentFileName = null;

        /// <summary>
        /// the resource manager to access images 
        /// </summary>
        private static ResourceManager resourceManager = null;

        /// <summary>
        /// This list contains preparsed informations and cached values for the supervised FSUIPC offsets
        /// </summary>
        List<Adapter> activeAdapters = new List<Adapter>();

        Fsuipc2Cache fsuipcCache = new Fsuipc2Cache();
        ArcazeCache arcazeCache = new ArcazeCache();

        private int lastClickedRow = -1;

        private CmdLineParams cmdLineParams;
        
        /// <summary>
        /// get a localized string
        /// </summary>
        /// <param name="s">the resource's string name</param>
        /// <returns>the translated string</returns>
        public static String _tr (String s) {
            if (null == resourceManager) {
                resourceManager = new ResourceManager("ArcazeUSB.ProjectMessages", typeof(MainForm).Assembly);
            }
            return resourceManager.GetString(s);
        }

        public MainForm()
        {
            // System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-FR");
            
            InitializeComponent();

            cmdLineParams = new CmdLineParams();

            Properties.Settings.Default.SettingChanging += new System.Configuration.SettingChangingEventHandler(Default_SettingChanging);

            timer.Interval      = Properties.Settings.Default.PollInterval;
            timer.Tick          += new EventHandler(timer_Tick);
            timer.Stopped       += new EventHandler(timer_Stopped);
            timer.Started       += new EventHandler(timer_Started);

            // we only load the autorun value stored in settings
            // and do not use possibly passed in autoRun from cmdline
            // because latter shall only have an temporary influence
            // on the program
            setAutoRunValue(Properties.Settings.Default.AutoRun);

            runToolStripButton.Enabled = true && fsuipcCache.isConnected() && arcazeCache.isConnected();
            runTestToolStripButton.Enabled = arcazeCache.isConnected();
            updateNotifyContextMenu(false);
            
            arcazeSerial.Items.Clear();
            arcazeSerial.Items.Add( _tr("none") );

            fsuipcCache.ConnectionLost += new EventHandler(fsuipcCache_ConnectionLost);
            fsuipcCache.Connected += new EventHandler(fsuipcCache_Connected);
            fsuipcCache.Connected += new EventHandler(checkAutoRun);
            fsuipcCache.Closed += new EventHandler(fsuipcCache_Closed);

            arcazeCache.Connected += new EventHandler(arcazeCache_Connected);
            arcazeCache.Closed += new EventHandler(arcazeCache_Closed);
            arcazeCache.ConnectionLost += new EventHandler(arcazeCache_ConnectionLost);
            _initializeModuleSettings();

            // initialize auto connect here
            // to be sure that the module settings are already available
            autoConnectTimer.Interval = 1000;
            autoConnectTimer.Tick += new EventHandler(autoConnectTimer_Tick);
            autoConnectTimer.Start();

            testModeTimer.Interval  = Properties.Settings.Default.TestTimerInterval;
            testModeTimer.Tick += new EventHandler(testModeTimer_Tick);                        
                       
            _updateRecentFilesMenuItems();
            _autoloadConfig();
            //_autoloadLastConfig();

            configDataTable.RowChanged += new DataRowChangeEventHandler(configDataTable_RowChanged);
            configDataTable.RowDeleted += new DataRowChangeEventHandler(configDataTable_RowChanged);
            dataGridViewConfig.RowsAdded += new DataGridViewRowsAddedEventHandler(dataGridViewConfig_RowsAdded);

            // the debug output for selected offsets
            fsuipcOffsetValueLabel.Visible = false;

            dataGridViewConfig.Columns["Description"].DefaultCellStyle.NullValue = MainForm._tr("uiLabelDoubleClickToAddConfig");
            dataGridViewConfig.Columns["EditButtonColumn"].DefaultCellStyle.NullValue = "...";
            
            if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName != "de")
            {
                // change ui icon to english
                donateToolStripButton.Image = ArcazeUSB.Properties.Resources.btn_donate_uk_SM;
            }
        }

        void Default_SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            if (e.SettingName == "TestTimerInterval")
            {
                testModeTimer.Interval = (int) e.NewValue;
            }
        }

        private void _autoloadConfig()
        {            
            if (cmdLineParams.ConfigFile != null)
            {
                if (!System.IO.File.Exists(cmdLineParams.ConfigFile))
                {
                    MessageBox.Show(
                                _tr("uiMessageCmdParamConfigFileDoesNotExist") + "\r" + cmdLineParams.ConfigFile,
                                _tr("Hint"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1);
                    return;
                }
                else
                {
                    _loadConfig(cmdLineParams.ConfigFile);
                    return;
                }                
            }

            _autoloadLastConfig();
        }

        private void _autoloadLastConfig()
        {
            // the new autoload feature
            // step1 load config always... good feature ;)
            // step2 run automatically -> see fsuipc connected event
            if (Properties.Settings.Default.RecentFiles.Count > 0)
            {
                foreach (string file in Properties.Settings.Default.RecentFiles)
                {
                    if (!System.IO.File.Exists(file)) continue;
                    _loadConfig(file);
                    break;
                }
            } //if 
        }

        void dataGridViewConfig_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            // if datagridviewconfig.RowCount == 1 this means that only the "new line" is added yet
            /*
            if (e.RowIndex != -1 && dataGridViewConfig.RowCount != 1)
            {
                (sender as DataGridView).Rows[e.RowIndex].Cells["active"].Style.BackColor
                       = (sender as DataGridView).DefaultCellStyle.BackColor;             

                (sender as DataGridView).Rows[e.RowIndex].Cells["description"].Style.BackColor
                       = (sender as DataGridView).DefaultCellStyle.BackColor;             
            }
             * */
        }

        /**
        private const int WM_DEVICECHANGE = 0x0219;  // int = 537
        private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004; 

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                arcazeCache_ConnectionLost(null, null);                
            }
            base.WndProc(ref m);            
        } */

        private void _initializeModuleSettings()
        {
            Dictionary<string, ArcazeModuleSettings> settings = getArcazeModuleSettings();
            List<string> serials = new List<string>();

            foreach (DeviceInfo arcaze in arcazeCache.getDeviceInfo())
            {
                serials.Add(arcaze.Serial);
            }

            if (settings.Keys.Intersect(serials).ToArray().Count() != serials.Count)
            {
                if (MessageBox.Show(
                                _tr("uiMessageModulesNotConfiguredYet"),                                
                                _tr("Hint"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.OK)
                {
                    SettingsDialog dlg = new SettingsDialog(arcazeCache);
                    dlg.StartPosition = FormStartPosition.CenterParent;
                    (dlg.Controls["tabControl1"] as TabControl).SelectedTab = (dlg.Controls["tabControl1"] as TabControl).Controls[1] as TabPage;
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Properties.Settings.Default.Save();                        
                    }
                }
            }            
            arcazeCache.updateModuleSettings(getArcazeModuleSettings());
        }

        protected Dictionary<string, ArcazeModuleSettings> getArcazeModuleSettings()
        {
            List<ArcazeModuleSettings> moduleSettings = new List<ArcazeModuleSettings>();
            Dictionary<string, ArcazeModuleSettings> result = new Dictionary<string, ArcazeModuleSettings>();

            if ("" == Properties.Settings.Default.ModuleSettings) return result;

            try
            {
                XmlSerializer SerializerObj = new XmlSerializer(typeof(List<ArcazeModuleSettings>));
                System.IO.StringReader w = new System.IO.StringReader(Properties.Settings.Default.ModuleSettings);
                moduleSettings = (List<ArcazeModuleSettings>)SerializerObj.Deserialize(w);
            }
            catch (Exception e)
            {
            }

            foreach (ArcazeModuleSettings setting in moduleSettings)
            {
                result[setting.serial] = setting;
            }
               
            return result;
        }

        public void executeTestOff(ArcazeConfigItem cfg)
        {
            executeDisplay(cfg.DisplayType == ArcazeLedDigit.TYPE ? "        " : "0", cfg);
        }

        public void executeTestOn(ArcazeConfigItem cfg)        
        {
            executeDisplay(cfg.DisplayType == ArcazeLedDigit.TYPE ? "12345678" : "8", cfg);
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

            ArcazeConfigItem cfg = new ArcazeConfigItem() ;
            cfg.DisplaySerial = "";
            if (lastRow.DataBoundItem != null) {
                cfg = ((lastRow.DataBoundItem as DataRowView).Row["settings"] as ArcazeConfigItem);
            };

            if (
                 cfg != null &&
                (cfg.FSUIPCOffset != ArcazeConfigItem.FSUIPCOffsetNull) &&
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
                    stopTestToolStripButton_Click(null, null);
                    _showError(ex.Message);                    
                }
            };


            DataGridViewRow row = dataGridViewConfig.Rows[testModeIndex];
            
            while (
                row.Cells["active"].Value != null && // check for null since last row is empty and value is null
                !(bool)row.Cells["active"].Value && 
                row != lastRow )
            {
                testModeIndex = ++testModeIndex % dataGridViewConfig.RowCount;
                row = dataGridViewConfig.Rows[testModeIndex];
            } //while


            cfg = new ArcazeConfigItem();

            // iterate over the config row by row            
            if ( row.DataBoundItem != null && 
                (row.DataBoundItem as DataRowView).Row["settings"] != null) // this is needed
                                                                            // since we immediately store all changes
                                                                            // and therefore there may be missing a 
                                                                            // valid cfg item
            {                
                cfg = ((row.DataBoundItem as DataRowView).Row["settings"] as ArcazeConfigItem);
            }
            
            if ( cfg != null && // this happens sometimes when a new line is added and still hasn't been configured
                (dataGridViewConfig.RowCount > 1 && row != lastRow) && 
                 cfg.FSUIPCOffset != ArcazeConfigItem.FSUIPCOffsetNull && 
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
                    stopTestToolStripButton_Click(null, null);
                    _showError(ex.Message);                    
                } 
            }

            testModeIndex = ++testModeIndex % dataGridViewConfig.RowCount;
        }

        void arcazeCache_ConnectionLost(object sender, EventArgs e)
        {
            //_disconnectArcaze();
            timer.Enabled = false;
            testModeTimer_Stop();
            isExecuting = false;
            _showError(_tr("uiMessageArcazeConnectionLost"));            
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void arcazeCache_Closed(object sender, EventArgs e)
        {
            arcazeUsbStatusToolStripStatusLabel.Image = ArcazeUSB.Properties.Resources.warning;
            testModeTimer_Stop();
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void arcazeCache_Connected(object sender, EventArgs e)
        {
            arcazeUsbStatusToolStripStatusLabel.Image = ArcazeUSB.Properties.Resources.check;
            testModeTimer_Stop();
            timer.Stop();            
            fillComboBoxesWithArcazeModules();
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void fsuipcCache_Closed(object sender, EventArgs e)
        {
            fsuipcStatusToolStripStatusLabel.Image = Properties.Resources.warning;
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>        
        void fsuipcCache_Connected(object sender, EventArgs e)
        {
            fsuipcStatusToolStripStatusLabel.Image = Properties.Resources.check;            
        }

        /// <summary>
        /// gets triggered as soon as the fsuipc is connected
        /// </summary>        
        void checkAutoRun (object sender, EventArgs e)
        {            
            if (Properties.Settings.Default.AutoRun || cmdLineParams.AutoRun)
            {
                timer.Enabled = true;
                minimizeMainForm(true);
            }
        }

        /// <summary>
        /// shows message to user and stops execution of timer
        /// </summary>
        void fsuipcCache_ConnectionLost(object sender, EventArgs e)
        {
            fsuipcCache.disconnect();
            if (!_fsRunning())
            {
                _showError(_tr("uiMessageFsHasBeenStopped"));
            } else {
                _showError(_tr("uiMessageFsuipcConnectionLost"));                
            } //if
            //
            timer.Enabled = false;
            isExecuting = false;            
        }

        /// <summary>
        /// enables the save button in toolbar after the user has changed config data
        /// </summary>        
        void configDataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            saveToolStripButton.Enabled = true;
        } //configDataTable_RowChanged


        /// <summary>
        /// handler which sets the states of UI elements when timer gets started
        /// </summary>
        void timer_Started(object sender, EventArgs e)
        {
            runToolStripButton.Enabled  = false;
            runTestToolStripButton.Enabled = false;
            stopToolStripButton.Enabled = true;
            updateNotifyContextMenu(timer.Enabled);
        } //timer_Started()

        /// <summary>
        /// handler which sets the states of UI elements when timer gets stopped
        /// </summary>
        void timer_Stopped(object sender, EventArgs e)
        {
            runToolStripButton.Enabled = true && fsuipcCache.isConnected() && arcazeCache.isConnected() && !testModeTimer.Enabled;
            runTestToolStripButton.Enabled = !testModeTimer.Enabled;
            stopToolStripButton.Enabled = false;

            // just forget about current states if timer gets stopped
            arcazeCache.Clear();
            updateNotifyContextMenu(timer.Enabled);
        } //timer_Stopped

        /// <summary>
        /// auto connect timer handler which tries to automagically connect to FSUIPC and Arcaze Modules        
        /// </summary>
        /// <remarks>
        /// auto connect is only done if current timer is not running since we suppose that an established
        /// connection was already available before the timer was started
        /// </remarks>
        void autoConnectTimer_Tick(object sender, EventArgs e)
        {
            // check if timer is running... 
            // do nothing if so, since everything else has been checked before...            
            if (timer.Enabled) return;

            if (testModeTimer.Enabled) return;

            if (!arcazeCache.isConnected())
            {
                arcazeCache.connect(); //  _initializeArcaze();
            }

            if ( _fsRunning() && !fsuipcCache.isConnected() )
            {
                fsuipcCache.connect();
                // we return here to prevent the disabling of the timer
                // so that autostart-feature can work properly
                return;
            }

            // this line here provokes a timer stop event each time
            // and therefore the icon for starting the app will get enabled
            // @see timer_Stopped
            timer.Enabled = false;
        } //autoConnectTimer_Tick()

        /// <summary>
        /// Timer eventhandler
        /// </summary>        
        void timer_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel.Text += ".";
            if (toolStripStatusLabel.Text.Length > (10 + _tr("Running").Length))
            {
                toolStripStatusLabel.Text = _tr("Running");
            }
            try
            {
                executeConfig();
            }
            catch (Exception)
            {
                isExecuting = false;
                timer.Enabled = false;
            }
        } //timer_Tick()

        private bool _fsRunning()
        {
            string proc = "fs9";
            // check for fs2004 / fs9
            if (Process.GetProcessesByName(proc).Length > 0)
            {
                fsuipcToolStripStatusLabel.Text = _tr("fsuipcStatus") + ":";
                return true;
            }
            proc = "fsx";
            // check for fsx
            if (Process.GetProcessesByName(proc).Length > 0)
            {
                fsuipcToolStripStatusLabel.Text = _tr("fsuipcStatus") + ":";
                return true;
            }
            // check for prepar3d
            proc = "prepar3d";
            if (Process.GetProcessesByName(proc).Length > 0)
            {
                fsuipcToolStripStatusLabel.Text = _tr("fsuipcStatus") + ":";
                return true;
            }
            // check for x-plane and xuipc
            proc = "x-plane";
            if (Process.GetProcessesByName(proc).Length > 0)
            {
                fsuipcToolStripStatusLabel.Text = _tr("xpuipcStatus") + ":";

                return true;
            }

            proc = "x-plane-32bit";
            if (Process.GetProcessesByName(proc).Length > 0)
            {
                fsuipcToolStripStatusLabel.Text = _tr("xpuipcStatus") + ":";

                return true;
            }

            return false;
        }

        /// <summary>
        /// start fsuipc connection or disconnect by pressing the button
        /// </summary>
        private void buttonFSUIPCConnect_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (fsuipcCache.isConnected())
            {
                fsuipcCache.disconnect();
                btn.Text = _tr("Connect");
            } else {
                if (fsuipcCache.connect())
                {
                    btn.Text = _tr("Disconnect");
                }
                else
                {
                    btn.Text = _tr("Retry");
                }
            }            
        } //buttonFSUIPCConnect_Click()

        /// <summary>
        /// Initialize Arcaze Connections and set button states accordingly
        /// </summary>
        private void buttonArcazeUsbConnect_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button; 
            if (btn.Text == _tr("Find")) {
                fillComboBoxesWithArcazeModules();
            } else if (btn.Text == _tr("Connect")) {
                arcazeCache.connect();
            } else if (btn.Text == _tr("Disconnect")) {
                arcazeCache.disconnect();
            } //if                                              
        } //buttonArcazeUsbConnect_Click()

        /// <summary>
        /// the timer shall be stopped if arcaze modules get disconnected
        /// </summary>
        /// <remarks>Please check why this never gets called.</remarks>
        void arcazeHid_DeviceRemoved(object sender, HidEventArgs e)
        {
            _showError( _tr("uiMessageAraceUSBHasBeenRemoved"));
            timer.Enabled = false;
        } //arcazeHid_DeviceRemoved()

        /// <summary>
        /// gathers infos about the connected modules and stores information in different objects
        /// </summary>
        /// <returns>returns true if there are modules present</returns>
        private bool fillComboBoxesWithArcazeModules()
        {
            // remove the items from all comboboxes
            // and set default items
            arcazeSerial.Items.Clear();
            arcazeSerial.Items.Add("none");
            arcazeUsbToolStripDropDownButton.DropDownItems.Clear();
            arcazeUsbToolStripDropDownButton.ToolTipText = _tr("uiMessageNoArcazeModuleFound");

            foreach (DeviceInfo module in arcazeCache.getDeviceInfo())
            {
                arcazeSerial.Items.Add(module.DeviceName + "/ " + module.Serial);
                arcazeUsbToolStripDropDownButton.DropDownItems.Add(module.DeviceName + "/ " + module.Serial);
            }

            if (arcazeSerial.Items.Count > 0)
            {
                arcazeUsbToolStripDropDownButton.ToolTipText = _tr("uiMessageArcazeModuleFound");
            }
            // only enable button if modules are available            
            return (arcazeSerial.Items.Count > 0);
        } //fillComboBoxesWithArcazeModules()

        /// <summary>
        /// properly disconnects all connections to FSUIPC and Arcaze
        /// </summary>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {           
            arcazeCache.disconnect();
            fsuipcCache.disconnect();
            Properties.Settings.Default.Save();
        } //Form1_FormClosed

        /// <summary>
        /// toggles the current timer when user clicks on respective run/stop buttons
        /// </summary>
        private void buttonToggleStart_Click(object sender, EventArgs e)
        {
            timer.Enabled = !timer.Enabled;                        
        } //buttonToggleStart_Click()

        /// <summary>
        /// updates the context menu entries for start and stop depending
        /// on the current application state
        /// </summary>
        /// <param name="isRunning"></param>
        protected void updateNotifyContextMenu(bool isRunning)
        {
            try
            {
                // The Start entry
                contextMenuStripNotifyIcon.Items[0].Enabled = !isRunning;

                // The Stop entry
                contextMenuStripNotifyIcon.Items[1].Enabled = isRunning;
            }
            catch (Exception e)
            {
                // do nothing
                // MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// the main method where the configuration is parsed and executed
        /// </summary>
        private void executeConfig()
        {
            // prevent execution if not connected to either FSUIPC or Arcaze
            if (!fsuipcCache.isConnected()) return;
            if (!arcazeCache.isConnected()) return;

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
                ArcazeConfigItem cfg = ((row.DataBoundItem as DataRowView).Row["settings"] as ArcazeConfigItem);

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
                    row.ErrorText = _tr("uiMessagePreconditionNotSatisfied");
                    continue;
                }
                
                executeDisplay(strValue, cfg);                
            }

            isExecuting = false;
        }

        private bool checkPrecondition(ArcazeConfigItem cfg, ConnectorValue currentValue)
        {
            bool finalResult = true;
            bool result = true;
            bool logic = true; // false:and true:or
            ConnectorValue connectorValue = new ConnectorValue();

            foreach (Precondition p in cfg.Preconditions)
            {
                if (!p.PreconditionActive) continue;

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

                        ArcazeConfigItem tmp = new ArcazeConfigItem();
                        tmp.ComparisonActive = true;
                        tmp.ComparisonValue = p.PreconditionValue;
                        tmp.ComparisonOperand = "=";
                        tmp.ComparisonIfValue = "1";
                        tmp.ComparisonElseValue = "0";

                        try
                        {
                            result = (executeComparison(connectorValue, tmp) == tmp.ComparisonIfValue);
                        }
                        catch (FormatException e)
                        {
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

                            tmp = new ArcazeConfigItem();
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
                                }
                            }

                            tmp.ComparisonOperand = p.PreconditionOperand;
                            tmp.ComparisonIfValue = "1";
                            tmp.ComparisonElseValue = "0";
                        
                            connectorValue.type = FSUIPCOffsetType.Integer;
                            if (! Int64.TryParse (value, out connectorValue.Int64))
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
                            }
                            break;
                        }
                        break;                    
                } // switch

                if (logic)
                {
                    finalResult |= result;
                }
                else
                {
                    finalResult &= result;
                }

                logic = (p.PreconditionLogic == "or" ? true : false);
            } // foreach

            return result;
        }

        private ConnectorValue executeRead(ArcazeConfigItem cfg)
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

        private ConnectorValue _executeReadInt(ArcazeConfigItem cfg)
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
                        fsuipcBCD val = new fsuipcBCD() { Value = value8 };
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
                        fsuipcBCD val = new fsuipcBCD() { Value = value16 };
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

        private ConnectorValue _executeReadFloat(ArcazeConfigItem cfg)
        {
            ConnectorValue result = new ConnectorValue();
            result.type = FSUIPCOffsetType.Float;
            switch (cfg.FSUIPCSize)
            {                
                case 4:
                    Double value32 = fsuipcCache.getFloatValue (
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

        private ConnectorValue executeTransform(ConnectorValue value, ArcazeConfigItem cfg)
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

        private string executeComparison(ConnectorValue connectorValue, ArcazeConfigItem cfg)
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
                    throw new Exception(_tr("uiMessageErrorOnParsingExpression"));
                }
            }

            return result;
        }

        private string _executeStringComparison(ConnectorValue connectorValue, ArcazeConfigItem cfg)
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

        private string executeDisplay(string value, ArcazeConfigItem cfg)
        {
            string serial = "";
            if (cfg.DisplaySerial.Contains("/"))
            {
                serial = cfg.DisplaySerial.Split('/')[1].Trim();
            };

            switch (cfg.DisplayType)
            {
                case ArcazeLedDigit.TYPE:
                    arcazeCache.setDisplay(
                        serial,
                        cfg.DisplayLedAddress,                        
                        cfg.DisplayLedConnector,
                        cfg.DisplayLedDigits,
                        cfg.DisplayLedDecimalPoints,
                        value.PadLeft(cfg.DisplayLedPadding ? cfg.DisplayLedDigits.Count : 0,'0'));
                    break;

                case ArcazeBcd4056.TYPE:
                    arcazeCache.setBcd4056(serial,
                        cfg.BcdPins,
                        cfg.DisplayTrigger,
                        value);
                    break;
                
                default:
                    arcazeCache.setValue(serial,
                        cfg.DisplayPin,
                        cfg.DisplayTrigger,
                        (value != "0" ? cfg.DisplayPinBrightness.ToString() : "0"));
                    break;
            }            
                
            return value.ToString();
        }

        /// <summary>
        /// present errors to user via message dialog or when minimized via balloon
        /// </summary>        
        private void _showError (string msg)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                MessageBox.Show(msg, _tr("Hint"), MessageBoxButtons.OK, MessageBoxIcon.Warning); 
            }
            else
            {
                notifyIcon.ShowBalloonTip(1000, _tr("Hint"), msg, ToolTipIcon.Warning);
            }
        } //_showError()

        /// <summary>
        /// handles the resize event
        /// </summary>
        private void MainForm_Resize (object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized) return;

            minimizeMainForm (FormWindowState.Minimized == this.WindowState);
        } //MainForm_Resize()

        /// <summary>
        /// handles minimize event
        /// </summary>        
        protected void minimizeMainForm (bool minimized)
        {
            if (minimized)
            {
                notifyIcon.Visible = true;
                notifyIcon.BalloonTipTitle = _tr("uiMessageArcazeUSBInterfaceActive");
                notifyIcon.BalloonTipText = _tr("uiMessageApplicationIsRunningInBackgroundMode");
                notifyIcon.ShowBalloonTip(1000);               
                this.Hide();
            }
            else
            {
                notifyIcon.Visible = false;                
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
            }
        } //minimizeMainForm()

        /// <summary>
        /// restores the current main form when user clicks on "restore" menu item in notify icon context menu
        /// </summary>
        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            minimizeMainForm(false);
        } //wiederherstellenToolStripMenuItem_Click()

        /// <summary>
        /// restores the current main form when user double clicks notify icon
        /// </summary>        
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            minimizeMainForm(false);
        } //notifyIcon_DoubleClick()

        /// <summary>
        /// exits when user selects according menu item in notify icon's context menu
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        } //exitToolStripMenuItem_Click()

        /// <summary>
        /// opens file dialog when clicking on according button
        /// </summary>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "ArcazeUSB Interface Config (*.aic) |*.aic";

            if (DialogResult.OK == fd.ShowDialog())
            {
                _loadConfig(fd.FileName);
            }   
        }

        /// <summary>
        /// stores the provided filename in the list of recently used files
        /// </summary>
        /// <param name="fileName">the filename to be used</param>
        private void _storeAsRecentFile(string fileName)
        {
            if (Properties.Settings.Default.RecentFiles.Contains(fileName))
            {
                Properties.Settings.Default.RecentFiles.Remove(fileName);
            }
            Properties.Settings.Default.RecentFiles.Insert(0, fileName);
            Properties.Settings.Default.Save();

            _updateRecentFilesMenuItems();
        }

        /// <summary>
        /// updates the list of the recent used files in the menu list
        /// </summary>
        private void _updateRecentFilesMenuItems()
        {
            recentDocumentsToolStripMenuItem.DropDownItems.Clear();
            int limit = 0;
            // update Menu
            foreach (string filename in Properties.Settings.Default.RecentFiles)
            {
                if (limit++ == Properties.Settings.Default.RecentFilesMaxCount) break;

                ToolStripItem current = new ToolStripMenuItem(filename);
                current.Click += new EventHandler(recentMenuItem_Click);
                recentDocumentsToolStripMenuItem.DropDownItems.Add(current);
            }

            recentDocumentsToolStripMenuItem.Enabled = recentDocumentsToolStripMenuItem.DropDownItems.Count > 0;           
        } //_updateRecentFilesMenuItems()

        /// <summary>
        /// gets triggered when user clicks on recent used file entry
        /// loads the according config
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void recentMenuItem_Click(object sender, EventArgs e)
        {
            _loadConfig((sender as ToolStripMenuItem).Text);            
        } //recentMenuItem_Click()

        /// <summary>
        /// loads the according config given by filename
        /// </summary>        
        private void _loadConfig(string fileName)
        {
            timer.Enabled = false;
            dataSetConfig.Clear();
            dataSetConfig.ReadXml(fileName);

            // for backward compatibility 
            // we check if there are rows that need to
            // initialize our config item correctly
            _applyBackwardCompatibilityLoading();
            _restoreValuesInGridView();            

            currentFileName = fileName;
            _setFilenameInTitle(fileName);
            _storeAsRecentFile(fileName);

            // set the button back to "disabled"
            // since due to initiliazing the dataSet
            // it will automatically gets enabled
            saveToolStripButton.Enabled = false;

            // always put this after "normal" initialization
            // savetoolstripbutton may be set to "enabled"
            // if user has changed something
            _checkForOrphanedSerials( false );
        }

        private void _checkForOrphanedSerials(bool showNotNecessaryMessage)
        {
            OrphanedSerialsDialog opd = new OrphanedSerialsDialog(arcazeCache, configDataTable);
            opd.StartPosition = FormStartPosition.CenterParent;
            if (opd.HasOrphanedSerials())
            {
                if (opd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _restoreValuesInGridView();
                    saveToolStripButton.Enabled = opd.HasChanged();
                }
            }
            else if (showNotNecessaryMessage)
            {
                MessageBox.Show(_tr("uiMessageNoOrphanedSerialsFound"), _tr("Hint"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void _setFilenameInTitle(string fileName)
        {
            var posOfDash = Text.IndexOf(" - ");

            fileName = fileName.Substring(fileName.LastIndexOf('\\')+1);
            if (posOfDash>=0) {
                Text = Text.Substring(posOfDash+3);
            }
            Text = fileName + " - " + Text;            
        } //_loadConfig()

        /// <summary>
        /// due to the new settings-node there must be some routine to load 
        /// data from legacy config files
        /// </summary>
        private void _applyBackwardCompatibilityLoading()
        {            
            foreach (DataRow row in configDataTable.Rows) {
                if (row["settings"].GetType() == typeof(System.DBNull))
                {
                    ArcazeConfigItem cfgItem = new ArcazeConfigItem();

                    if (row["fsuipcOffset"].GetType() != typeof(System.DBNull))
                        cfgItem.FSUIPCOffset = Int32.Parse(row["fsuipcOffset"].ToString().Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);

                    if (row["fsuipcSize"].GetType() != typeof(System.DBNull))
                        cfgItem.FSUIPCSize = Byte.Parse(row["fsuipcSize"].ToString());

                    if (row["mask"].GetType() != typeof(System.DBNull))
                        cfgItem.FSUIPCMask = (row["mask"].ToString() != "") ? Int32.Parse(row["mask"].ToString()) : Int32.MaxValue;

                    // comparison
                    if (row["comparison"].GetType() != typeof(System.DBNull))
                    {
                        cfgItem.ComparisonActive = true;
                        cfgItem.ComparisonOperand = row["comparison"].ToString();
                    }

                    if (row["comparisonValue"].GetType() != typeof(System.DBNull))
                    {
                        cfgItem.ComparisonValue = row["comparisonValue"].ToString();
                    }

                    if (row["converter"].GetType() != typeof(System.DBNull))
                    {
                        if (row["converter"].ToString() == "Boolean")
                        {
                            cfgItem.ComparisonIfValue = "1";
                            cfgItem.ComparisonElseValue = "0";
                        }
                    }

                    if (row["trigger"].GetType() != typeof(System.DBNull))
                    {
                        cfgItem.DisplayTrigger = row["trigger"].ToString();
                    }

                    if (row["usbArcazePin"].GetType() != typeof(System.DBNull))
                    {
                        cfgItem.DisplayType = "Pin";
                        cfgItem.DisplayPin = row["usbArcazePin"].ToString();
                    }

                    if (row["arcazeSerial"].GetType() != typeof(System.DBNull))
                    {
                        cfgItem.DisplaySerial = row["arcazeSerial"].ToString();
                    }

                    row["settings"] = cfgItem;
                }
            }
        } 

        /// <summary>
        /// saves the current config to filename
        /// </summary>        
        private void _saveConfig(string fileName)
        {
            _applyBackwardCompatibilitySaving();            
            dataSetConfig.WriteXml(fileName);
            _restoreValuesInGridView();
            _storeAsRecentFile(fileName);
            _setFilenameInTitle(fileName);
            saveToolStripButton.Enabled = false;
        }

        /// <summary>
        /// use the settings from the config object and initialize the grid cells 
        /// this is needed after loading and saving configs
        /// </summary>
        private void _restoreValuesInGridView()
        {
            foreach (DataRow row in configDataTable.Rows)
            {
                ArcazeConfigItem cfgItem = row["settings"] as ArcazeConfigItem;
                if (cfgItem != null)
                {
                    row["fsuipcOffset"] = "0x" + cfgItem.FSUIPCOffset.ToString("X4");
                    if (cfgItem.DisplaySerial != null)
                    {
                        row["arcazeSerial"] = cfgItem.DisplaySerial.ToString();
                    }
                }
            }            
        } //_restoreValuesInGridView()

        /// <summary>
        /// removes unnecessary information that is now stored in the settings node itself
        /// </summary>
        /// <remarks>DEPRECATED</remarks>
        private void _applyBackwardCompatibilitySaving()
        {
            // delete old values from config that are part of the new settings-node now
            foreach (DataRow row in configDataTable.Rows)
            {
                if (row["settings"].GetType() != typeof(System.DBNull))
                {
                    row["converter"] = null;
                    row["trigger"] = null;
                    row["fsuipcOffset"] = null;
                    row["fsuipcSize"] = null;
                    row["mask"] = null;
                    row["comparison"] = null;
                    row["comparisonValue"] = null;
                    row["usbArcazePin"] = null;
                    row["arcazeSerial"] = null;
                }
            }
        } //_saveConfig()

        /// <summary>
        /// triggers the save dialog if user clicks on according buttons
        /// </summary>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            SaveFileDialog fd = new SaveFileDialog();            
            fd.Filter = "ArcazeUSB Interface Config (*.aic)|*.aic";
            if (DialogResult.OK == fd.ShowDialog())
            {
                _saveConfig(fd.FileName);
            }            
        } //saveToolStripMenuItem_Click()

        /// <summary>
        /// shows the about form
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm ab = new AboutForm();
            ab.StartPosition = FormStartPosition.CenterParent;
            ab.ShowDialog();
        } //aboutToolStripMenuItem_Click()

        /// <summary>
        /// resets the config after presenting a message box where user hast to confirm the reset first
        /// </summary>
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if ( MessageBox.Show(
                       _tr("uiMessageConfirmNewConfig"),
                       _tr("uiMessageConfirmNewConfigTitle"), 
                       MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                timer.Enabled = false;
                currentFileName = null;
                _setFilenameInTitle(_tr("DefaultFileName"));
                configDataTable.Clear();
            };
        } //toolStripMenuItem3_Click()

        /// <summary>
        /// gets triggered if user changes values in the data grid
        /// </summary>
        private void dataGridViewConfig_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {            
            if ( (e.FormattedValue as String) == "" ) return;

            switch (dataGridViewConfig[e.ColumnIndex, e.RowIndex].OwningColumn.Name)
            {
                case "FsuipcOffset":
                    try
                    {
                        Int32 tmp = Int32.Parse((e.FormattedValue as String).Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
                    }
                    catch
                    {
                        e.Cancel = true;
                        MessageBox.Show(
                                _tr("uiMessageInvalidValueHexOnly"),
                                _tr("InputValidation"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                    }
                    break;
            }
            
        } //dataGridViewConfig_CellValidating()

        /// <summary>
        /// handles errors when user submits data to the datagrid
        /// </summary>
        private void dataGridViewConfig_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            switch (dataGridViewConfig[e.ColumnIndex, e.RowIndex].OwningColumn.Name)
            {
                case "arcazeSerial":
                    // when loading config and not beeing connected to arcaze modules
                    // we actually do not know whether the serial infos are correct or not
                    // so in this case we add the new value to our items
                    //
                    // otherwise we would get an error due to validation issues
                    arcazeSerial.Items.Add(dataGridViewConfig[e.ColumnIndex, e.RowIndex].Value.ToString());
                    e.Cancel = false;
                    break;
            }
        } //dataGridViewConfig_DataError()

        /// <summary>
        /// gets triggered if user uses quick save button from toolbar
        /// </summary>
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            // if filename of loaded file is known use it
            if (currentFileName != null)
            {
                _saveConfig(currentFileName);
                return;
            }
            // otherwise trigger normal open file dialog
            saveToolStripMenuItem_Click(sender, e);
        } //saveToolStripButton_Click()

        /// <summary>
        /// gets triggered when test mode is started via button, all states
        /// are set for the other buttons accordingly.
        /// </summary>
        /// <remarks>
        /// Why does this differ from normal run-Button handling?
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void runTestToolStripLabel_Click(object sender, EventArgs e)
        {
            testModeTimer_Start();
        }

        private void testModeTimer_Start()
        {
            testModeTimer.Enabled = true;
            stopToolStripButton.Visible = false;
            stopTestToolStripButton.Visible = true;
            stopTestToolStripButton.Enabled = true;
            runTestToolStripButton.Enabled = false;
            runToolStripButton.Enabled = false;
        }

        /// <summary>
        /// gets triggered when test mode is ended via stop button, all states
        /// are set for the other buttons accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopTestToolStripButton_Click(object sender, EventArgs e)
        {
            testModeTimer_Stop();
        }

        /// <summary>
        /// synchronize toolbaritems and other components with current testmodetimer state
        /// </summary>
        private void testModeTimer_Stop()
        {
            testModeTimer.Enabled = false;
            testModeIndex = 0;
            stopToolStripButton.Visible = true;
            stopTestToolStripButton.Visible = false;
            stopTestToolStripButton.Enabled = false;
            runTestToolStripButton.Enabled = true;
            arcazeCache.Clear();
            runToolStripButton.Enabled = true && arcazeCache.isConnected() && fsuipcCache.isConnected() && !testModeTimer.Enabled;
        }


        /// <summary>
        /// click event when button in gridview gets clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewConfig_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // handle clicks on header cells or row-header cells
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;            

            switch (dataGridViewConfig[e.ColumnIndex, e.RowIndex].OwningColumn.Name)
            {                
                case "FsuipcOffset":
                case "fsuipcValueColumn":
                case "arcazeValueColumn":
                case "arcazeSerial":                    
                case "EditButtonColumn":
                    bool isNew = dataGridViewConfig.Rows[e.RowIndex].IsNewRow;
                    if (isNew)
                    {                        
                        MessageBox.Show(MainForm._tr("uiMessageConfigLineNotSavedYet"),
                                        _tr("Hint"));
                        return;
                    } //if

                    ArcazeConfigItem cfg;
                    DataRow row = null;
                    bool create = false;
                    if (isNew) {
                        cfg = new ArcazeConfigItem();                        
                    } else {
                        row = (dataGridViewConfig.Rows[e.RowIndex].DataBoundItem as DataRowView).Row;

                        // the row had been saved but no config object has been created
                        // TODO: move this logic to an appropriate event, e.g. when leaving the gridrow focus of the new row
                        if ((dataGridViewConfig.Rows[e.RowIndex].DataBoundItem as DataRowView).Row["settings"].GetType() == typeof(System.DBNull))
                        {
                            (dataGridViewConfig.Rows[e.RowIndex].DataBoundItem as DataRowView).Row["settings"] = new ArcazeConfigItem();
                        }

                        cfg = ((dataGridViewConfig.Rows[e.RowIndex].DataBoundItem as DataRowView).Row["settings"] as ArcazeConfigItem);                        
                    }
                    _editConfigWithWizard(
                             row,
                             cfg,
                             create);

                    dataGridViewConfig.EndEdit();
                    break;                    
                case "Active":
                    // always end editing to store changes
                    dataGridViewConfig.EndEdit();
                    break;
            }
        }

        /// <summary>
        /// initializes the config wizard and shows the modal dialog.
        /// afterwards stores some values in the data set so that the visible grid columns are filled with data.
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="cfg"></param>
        /// <param name="create"></param>
        private void _editConfigWithWizard(DataRow dataRow, ArcazeConfigItem cfg, bool create)
        {
            Form wizard = new ConfigWizard(this, cfg, arcazeCache, getArcazeModuleSettings(), dataSetConfig, dataRow["guid"].ToString());
            wizard.StartPosition = FormStartPosition.CenterParent;
            if (wizard.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (dataRow == null) return;
                // do something special
                dataRow["fsuipcOffset"] = "0x" + cfg.FSUIPCOffset.ToString("X4");
                dataRow["arcazeSerial"] =  cfg.DisplaySerial;
            };            
        }

        /// <summary>
        /// when using tab in the grid view, the focus ignores readonly cell and jumps ahead to the next cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewConfig_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewConfig[e.ColumnIndex, e.RowIndex].ReadOnly)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsDialog dialog = new SettingsDialog(arcazeCache);
            dialog.StartPosition = FormStartPosition.CenterParent;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.Save();
                arcazeCache.updateModuleSettings(getArcazeModuleSettings());
                arcazeCache.disconnect();                
            }
        }

        private void autoRunToolStripButton_Click(object sender, EventArgs e)
        {
            setAutoRunValue(!Properties.Settings.Default.AutoRun);
        }

        private void setAutoRunValue(bool value)
        {
            Properties.Settings.Default.AutoRun = value;
            if (value)
            {
                autoRunToolStripButton.Image = ArcazeUSB.Properties.Resources.lightbulb_on;
            }
            else
            {
                autoRunToolStripButton.Image = ArcazeUSB.Properties.Resources.lightbulb;
            }
        }

        private void dataGridViewConfig_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewConfig_CellContentClick(sender, e);
        }

        private void dataGridViewConfig_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["guid"].Value = Guid.NewGuid();
        }

        private void configDataTable_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            e.Row["guid"] = Guid.NewGuid();        
        }

        private void configDataTable_RowChanged_1(object sender, DataRowChangeEventArgs e)
        {
            if (e.Row["guid"] == DBNull.Value)
                e.Row["guid"] = Guid.NewGuid();
        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // do somehting here
            foreach (DataGridViewRow row in dataGridViewConfig.SelectedRows)
            {
                // we cannot delete a row which hasn't been saved yet
                if (row.IsNewRow) continue;
                dataGridViewConfig.Rows.Remove(row);
            }            
        }

        /// <summary>
        /// this method is used to select the current row so that the context menu events may detect the current row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewConfig_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dataGridViewConfig.EndEdit();
                lastClickedRow = e.RowIndex;

                if (e.RowIndex != -1 && e.RowIndex != (sender as DataGridView).Rows.Count-1)
                {
                    if (!(sender as DataGridView).Rows[e.RowIndex].Selected)
                    {
                        // reset all rows since we are not right clicking on a currently
                        // selected one
                        foreach (DataGridViewRow row in (sender as DataGridView).SelectedRows)
                        {
                            row.Selected = false;
                        }
                    }

                    // the current one becomes selected in any case
                    (sender as DataGridView).Rows[e.RowIndex].Selected = true;
                }
            }            
        }


        private void dataGridViewConfig_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewConfig.EndEdit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (saveToolStripButton.Enabled && MessageBox.Show(
                       _tr("uiMessageConfirmDiscardUnsaved"),
                       _tr("uiMessageConfirmDiscardUnsavedTitle"),
                       MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // only cancel closing if not saved before
                // which is indicated by empty currentFileName
                e.Cancel = (currentFileName == null);
                saveToolStripButton_Click(saveToolStripButton, new EventArgs());                
            };
        }

        private void dataGridViewConfig_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // handle clicks on header cells or row-header cells
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            switch (dataGridViewConfig[e.ColumnIndex, e.RowIndex].OwningColumn.Name)
            {
                case "Description":
                    dataGridViewConfig.CurrentCell = dataGridViewConfig[e.ColumnIndex, e.RowIndex];
                    dataGridViewConfig.BeginEdit(true);
                    break;
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(MainForm._tr("WebsiteUrlHelp"));
        }

        private void duplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // this is called to ensure 
            // that all data has been stored in
            // the data table
            // otherwise there can occur strange inserts of new rows
            // at the first position instead of the expected one
            this.Validate();
            
            // do somehting here
            foreach (DataGridViewRow row in dataGridViewConfig.SelectedRows)
            {
                // ignore new rows since they cannot be copied nor deleted
                if (row.IsNewRow) continue;                
                
                // get current config item
                // duplicate it
                // duplicate row 
                // link to new config item 
                DataRow currentRow = (row.DataBoundItem as DataRowView).Row;
                DataRow newRow = configDataTable.NewRow();
                
                foreach (DataColumn col in configDataTable.Columns) {
                    newRow[col.ColumnName] = currentRow[col.ColumnName];
                }
                
                ArcazeConfigItem cfg = ((row.DataBoundItem as DataRowView).Row["settings"] as ArcazeConfigItem);
                if (cfg != null) {
                    cfg = (cfg.Clone() as ArcazeConfigItem);
                } else {
                    cfg = new ArcazeConfigItem();
                }

                newRow["description"] += " " + _tr("suffixCopy");
                newRow["settings"] = cfg;
                newRow["guid"] = Guid.NewGuid();                

                int currentPosition = configDataTable.Rows.IndexOf(currentRow);
                if (currentPosition == -1)
                {
                    currentPosition = 1;
                }

                configDataTable.Rows.InsertAt(newRow, currentPosition + 1);

                row.Selected = false;
            }            
        }

        private void dataGridViewContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // do not show up context menu
            // id there is only the new line visible
            e.Cancel = (dataGridViewConfig.Rows.Count == 1 || (lastClickedRow == dataGridViewConfig.Rows.Count-1));
        }

        private void orphanedSerialsFinderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _checkForOrphanedSerials(true);
        }

        private void donateToolStripButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=92VXMWBEZWN92");               
        }
    }
}
