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
using AutoUpdaterDotNET;
using System.Runtime.InteropServices;

namespace MobiFlight
{
    public partial class MainForm : Form
    {
        public static String Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        public static String Build = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime.ToString("yyyymmdd");

        /// <summary>
        /// the currently used filename of the loaded config file
        /// </summary>
        private string currentFileName = null;

        /// <summary>
        /// the resource manager to access images 
        /// </summary>
        private static ResourceManager resourceManager = null;

        private int lastClickedRow = -1;

        private CmdLineParams cmdLineParams;

        private ExecutionManager execManager;

        private bool _onClosing = false;

        private delegate DialogResult MessageBoxDelegate(string msg, string title, MessageBoxButtons buttons, MessageBoxIcon icon);
        
        /// <summary>
        /// get a localized string
        /// </summary>
        /// <param name="s">the resource's string name</param>
        /// <returns>the translated string</returns>
        public static String _tr (String s) {
            if (null == resourceManager) {
                resourceManager = new ResourceManager("MobiFlight.ProjectMessages", typeof(MainForm).Assembly);
            }
            return resourceManager.GetString(s);
        }

        public MainForm()
        {
            InitializeComponent();
            UpgradeSettingsFromPreviousInstallation();

            inputsTabControl.DrawItem += new DrawItemEventHandler(tabControl1_DrawItem);

            // init logging
            LogAppenderTextBox logAppenderTextBox = new LogAppenderTextBox (logTextBox);
            Log.Instance.AddAppender(logAppenderTextBox);
            Log.Instance.Enabled = Properties.Settings.Default.LogEnabled;
            logTextBox.Visible = Log.Instance.Enabled;
            
            try
            {
                Log.Instance.Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), Properties.Settings.Default.LogLevel, true);
            }
            catch (Exception e)
            {
                Log.Instance.log("MainForm() : Unknown log level", LogSeverity.Error);
            }
            Log.Instance.log("MainForm() : Logger initialized " + Log.Instance.Severity.ToString(), LogSeverity.Info);


            execManager = new ExecutionManager(dataGridViewConfig, inputsDataGridView);
            cmdLineParams = new CmdLineParams();
            Properties.Settings.Default.SettingChanging += new System.Configuration.SettingChangingEventHandler(Default_SettingChanging);

            execManager.OnExecute += new EventHandler(timer_Tick);
            execManager.OnStopped += new EventHandler(timer_Stopped);
            execManager.OnStarted += new EventHandler(timer_Started);
            // we only load the autorun value stored in settings
            // and do not use possibly passed in autoRun from cmdline
            // because latter shall only have an temporary influence
            // on the program
            setAutoRunValue(Properties.Settings.Default.AutoRun);

            runToolStripButton.Enabled = true && execManager.SimConnected() && execManager.ModulesConnected();
            runTestToolStripButton.Enabled = execManager.ModulesConnected();
            updateNotifyContextMenu(false);
            
            arcazeSerial.Items.Clear();
            arcazeSerial.Items.Add( _tr("none") );

            execManager.OnSimCacheConnectionLost += new EventHandler(fsuipcCache_ConnectionLost);
            execManager.OnSimCacheConnected += new EventHandler(fsuipcCache_Connected);
            execManager.OnSimCacheConnected += new EventHandler(checkAutoRun);
            execManager.OnSimCacheClosed += new EventHandler(fsuipcCache_Closed);

            execManager.OnModulesConnected += new EventHandler(arcazeCache_Connected);
            execManager.OnModulesDisconnected += new EventHandler(arcazeCache_Closed);
            execManager.OnModuleConnectionLost += new EventHandler(arcazeCache_ConnectionLost);
            _initializeModuleSettings();

            execManager.OnTestModeException += new EventHandler(execManager_OnTestModeException);     
                       
            _updateRecentFilesMenuItems();
            _autoloadConfig();
            //_autoloadLastConfig();

            configDataTable.RowChanged += new DataRowChangeEventHandler(configDataTable_RowChanged);
            configDataTable.RowDeleted += new DataRowChangeEventHandler(configDataTable_RowChanged);
            inputsDataTable.RowChanged += new DataRowChangeEventHandler(configDataTable_RowChanged);
            inputsDataTable.RowDeleted += new DataRowChangeEventHandler(configDataTable_RowChanged);
            dataGridViewConfig.RowsAdded += new DataGridViewRowsAddedEventHandler(dataGridViewConfig_RowsAdded);


            // the debug output for selected offsets
            fsuipcOffsetValueLabel.Visible = false;

            dataGridViewConfig.Columns["Description"].DefaultCellStyle.NullValue = MainForm._tr("uiLabelDoubleClickToAddConfig");
            dataGridViewConfig.Columns["EditButtonColumn"].DefaultCellStyle.NullValue = "...";
            inputsDataGridView.Columns["inputDescription"].DefaultCellStyle.NullValue = MainForm._tr("uiLabelDoubleClickToAddConfig");
            inputsDataGridView.Columns["inputEditButtonColumn"].DefaultCellStyle.NullValue = "...";
            
            if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName != "de")
            {
                // change ui icon to english
                donateToolStripButton.Image = MobiFlight.Properties.Resources.btn_donate_uk_SM;
            }
        }

        private void UpgradeSettingsFromPreviousInstallation()
        {
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }
        }

        void AutoUpdater_CheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (!args.IsUpdateAvailable && !args.DoSilent)
            {
                this.Invoke(new MessageBoxDelegate(ShowMessageThreadSafe), String.Format(_tr("uiMessageNoUpdateNecessary"), Version), MainForm._tr("Hint"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private DialogResult ShowMessageThreadSafe(string msg, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(this, msg, title, buttons);
        }

        private void CheckForUpdate(bool force, bool silent = false)
        {
            AutoUpdater.LetUserSelectRemindLater = true;
            AutoUpdater.Start("http://www.mobiflight.de/tl_files/download/releases/mobiflightconnector.xml", force, silent);
        }

        void execManager_OnTestModeException(object sender, EventArgs e)
        {
            stopTestToolStripButton_Click(null, null);
            _showError((sender as Exception).Message);
        }

        void Default_SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            if (e.SettingName == "TestTimerInterval")
            {
                execManager.SetTestModeInterval((int)e.NewValue);
            }

            if (e.SettingName == "PollInterval")
            {
                // set FSUIPC update interval
                execManager.SetFsuipcInterval((int)e.NewValue);
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

            // get all currently connected devices
            // add 'em to the list
            foreach (IModuleInfo arcaze in execManager.getModuleCache().getModuleInfo())
            {
                serials.Add(arcaze.Serial);
            }

            // and now verify that all modules that are connected
            // really are configured
            // show message box if not
            if (settings.Keys.Intersect(serials).ToArray().Count() != serials.Count)
            {
                if (MessageBox.Show(
                                _tr("uiMessageModulesNotConfiguredYet"),                                
                                _tr("Hint"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.OK)
                {
                    SettingsDialog dlg = new SettingsDialog(execManager);
                    dlg.StartPosition = FormStartPosition.CenterParent;
                    (dlg.Controls["tabControl1"] as TabControl).SelectedTab = (dlg.Controls["tabControl1"] as TabControl).Controls[1] as TabPage;
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Properties.Settings.Default.Save();
                        logTextBox.Visible = Log.Instance.Enabled;
                    }
                }
            }           
 
            execManager.updateModuleSettings(getArcazeModuleSettings());
        }

        /// <summary>
        /// rebuilt Arcaze module settings from the stored configuration
        /// </summary>
        /// <returns></returns>
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
                Log.Instance.log("MainForm.getArcazeModuleSettings() : Deserialize problem.", LogSeverity.Warn);
            }

            foreach (ArcazeModuleSettings setting in moduleSettings)
            {
                result[setting.serial] = setting;
            }
               
            return result;
        }

        void arcazeCache_ConnectionLost(object sender, EventArgs e)
        {
            //_disconnectArcaze();
            _showError(_tr("uiMessageArcazeConnectionLost"));            
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void arcazeCache_Closed(object sender, EventArgs e)
        {
            arcazeUsbStatusToolStripStatusLabel.Image = MobiFlight.Properties.Resources.warning;
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void arcazeCache_Connected(object sender, EventArgs e)
        {
            arcazeUsbStatusToolStripStatusLabel.Image = MobiFlight.Properties.Resources.check;
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
                execManager.Start();
                minimizeMainForm(true);
            }
        }

        /// <summary>
        /// shows message to user and stops execution of timer
        /// </summary>
        void fsuipcCache_ConnectionLost(object sender, EventArgs e)
        {
            if (!_fsRunning())
            {
                _showError(_tr("uiMessageFsHasBeenStopped"));
            } else {
                _showError(_tr("uiMessageFsuipcConnectionLost"));                
            } //if
            execManager.Stop();
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
            updateNotifyContextMenu(execManager.IsStarted());
        } //timer_Started()

        /// <summary>
        /// handler which sets the states of UI elements when timer gets stopped
        /// </summary>
        void timer_Stopped(object sender, EventArgs e)
        {
            runToolStripButton.Enabled = true && execManager.SimConnected() && execManager.ModulesConnected() && !execManager.TestModeIsStarted();
            runTestToolStripButton.Enabled = !execManager.TestModeIsStarted();
            stopToolStripButton.Enabled = false;
            updateNotifyContextMenu(execManager.IsStarted());
        } //timer_Stopped

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
        } //timer_Tick()

        // TODO: refactor!!!
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

            proc = "wideclient";
            // check for FSUIPC wide client
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

            // TODO: refactor!!!
            foreach (IModuleInfo module in execManager.getModuleCache().getModuleInfo())
            {
                arcazeSerial.Items.Add(module.Name + "/ " + module.Serial);
                arcazeUsbToolStripDropDownButton.DropDownItems.Add(module.Name + "/ " + module.Serial);
            }
#if MOBIFLIGHT
            foreach (IModuleInfo module in execManager.getMobiFlightModuleCache().getModuleInfo())
            {
                arcazeSerial.Items.Add(module.Name + "/ " + module.Serial);
                arcazeUsbToolStripDropDownButton.DropDownItems.Add(module.Name + "/ " + module.Serial);
            }
#endif

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
            execManager.Shutdown();
            Properties.Settings.Default.Save();
        } //Form1_FormClosed

        /// <summary>
        /// toggles the current timer when user clicks on respective run/stop buttons
        /// </summary>
        private void buttonToggleStart_Click(object sender, EventArgs e)
        {
            if (execManager.IsStarted()) execManager.Stop();
            else execManager.Start();
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
                Log.Instance.log("MainForm.updateNotifyContextMenu() : " + e.Message, LogSeverity.Warn);
            }
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
                notifyIcon.BalloonTipTitle = _tr("uiMessageMFConnectorInterfaceActive");
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
            fd.Filter = "MobiFlight Connector Config (*.mcc)|*.mcc|ArcazeUSB Interface Config (*.aic) |*.aic";

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
            if (fileName.IndexOf(".aic") != -1)
            {
                if (MessageBox.Show(_tr("uiMessageMigrateConfigFileYesNo"), _tr("Hint"), MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }

                SaveFileDialog fd = new SaveFileDialog();
                fd.FileName = fileName.Replace(".aic", ".mcc");
                fd.Filter = "MobiFlight Connector Config (*.mcc)|*.mcc";
                if (DialogResult.OK != fd.ShowDialog())
                {
                    return;
                }

                String file = System.IO.File.ReadAllText(fileName);
                String newFile = file.Replace("ArcazeFsuipcConnector", "MFConnector");
                System.IO.File.WriteAllText(fd.FileName, newFile);
                fileName = fd.FileName;
            }
            else
            {
                String file = System.IO.File.ReadAllText(fileName);
                if (file.IndexOf("ArcazeUSB.ArcazeConfigItem") != -1)
                {
                    SaveFileDialog fd = new SaveFileDialog();
                    fd.FileName = fileName.Replace(".mcc", "_v6.0.mcc");

                    if (MessageBox.Show(_tr("uiMessageMigrateConfigFileV60YesNo"), _tr("Hint"), MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        fd.Filter = "MobiFlight Connector Config (*.mcc)|*.mcc";
                        if (DialogResult.OK != fd.ShowDialog())
                        {
                            return;
                        }
                    }

                    String newFile = file.Replace("ArcazeUSB.ArcazeConfigItem", "MobiFlight.OutputConfigItem");
                    System.IO.File.WriteAllText(fd.FileName, newFile);
                    fileName = fd.FileName;
                }
            }

            execManager.Stop();
            dataSetConfig.Clear();
            dataSetInputs.Clear();
            ConfigFile configFile = new ConfigFile(fileName);
            dataSetConfig.ReadXml(configFile.getOutputConfig());
            try
            {
                dataSetInputs.ReadXml(configFile.getInputConfig());
            }
            catch (InvalidExpressionException ex)
            {
                // no inputs configured... old format... just ignore
            }
            

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
            List<string> serials = new List<string>();
            foreach (IModuleInfo moduleInfo in execManager.getConnectedModulesInfo())
            {
                serials.Add(moduleInfo.Name + "/ " + moduleInfo.Serial);
            }

            OrphanedSerialsDialog opd = new OrphanedSerialsDialog(serials, configDataTable);
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
            fileName = fileName.Substring(fileName.LastIndexOf('\\')+1);
            Text = fileName + " - MobiFlight Connector";            
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
                    OutputConfigItem cfgItem = new OutputConfigItem();

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

            ConfigFile configFile = new ConfigFile(fileName);
            configFile.SaveFile(dataSetConfig, dataSetInputs);
            currentFileName=fileName;
            //dataSetConfig.WriteXml(fileName);
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
                OutputConfigItem cfgItem = row["settings"] as OutputConfigItem;
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
            fd.Filter = "MobiFlight Connector Config (*.mcc)|*.mcc";
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
        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ( MessageBox.Show(
                       _tr("uiMessageConfirmNewConfig"),
                       _tr("uiMessageConfirmNewConfigTitle"), 
                       MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                execManager.Stop();
                currentFileName = null;
                _setFilenameInTitle(_tr("DefaultFileName"));
                configDataTable.Clear();
                inputsDataTable.Clear();
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
            execManager.TestModeStart();
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
            execManager.TestModeStop();
            stopToolStripButton.Visible = true;
            stopTestToolStripButton.Visible = false;
            stopTestToolStripButton.Enabled = false;
            runTestToolStripButton.Enabled = true;
            runToolStripButton.Enabled = true && execManager.ModulesConnected() && execManager.SimConnected() && !execManager.TestModeIsStarted();
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

                    OutputConfigItem cfg;
                    DataRow row = null;
                    bool create = false;
                    if (isNew) {
                        cfg = new OutputConfigItem();                        
                    } else {
                        row = (dataGridViewConfig.Rows[e.RowIndex].DataBoundItem as DataRowView).Row;

                        // the row had been saved but no config object has been created
                        // TODO: move this logic to an appropriate event, e.g. when leaving the gridrow focus of the new row
                        if ((dataGridViewConfig.Rows[e.RowIndex].DataBoundItem as DataRowView).Row["settings"].GetType() == typeof(System.DBNull))
                        {
                            (dataGridViewConfig.Rows[e.RowIndex].DataBoundItem as DataRowView).Row["settings"] = new OutputConfigItem();
                        }

                        cfg = ((dataGridViewConfig.Rows[e.RowIndex].DataBoundItem as DataRowView).Row["settings"] as OutputConfigItem);                        
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
        /// click event when button in gridview gets clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inputsDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // handle clicks on header cells or row-header cells
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            switch (inputsDataGridView[e.ColumnIndex, e.RowIndex].OwningColumn.Name)
            {
                case "inputEditButtonColumn":
                    bool isNew = inputsDataGridView.Rows[e.RowIndex].IsNewRow;
                    if (isNew)
                    {
                        MessageBox.Show(MainForm._tr("uiMessageConfigLineNotSavedYet"),
                                        _tr("Hint"));
                        return;
                    } //if

                    InputConfigItem cfg;
                    DataRow row = null;
                    bool create = false;
                    if (isNew)
                    {
                        cfg = new InputConfigItem();
                    }
                    else
                    {
                        row = (inputsDataGridView.Rows[e.RowIndex].DataBoundItem as DataRowView).Row;

                        // the row had been saved but no config object has been created
                        // TODO: move this logic to an appropriate event, e.g. when leaving the gridrow focus of the new row
                        if (row["settings"].GetType() == typeof(System.DBNull))
                        {
                            row["settings"] = new InputConfigItem();
                        }

                        cfg = row["settings"] as InputConfigItem;
                    }
                    _editConfigWithInputWizard(
                             row,
                             cfg,
                             create);
                    
                    inputsDataGridView.Rows[e.RowIndex].Cells["inputName"].Value  =  cfg.Name + "/" + cfg.ModuleSerial;
                    inputsDataGridView.Rows[e.RowIndex].Cells["inputType"].Value = cfg.button != null ? "Button" : "Encoder";
                    inputsDataGridView.EndEdit();
                    break;

                case "inputActive":
                    // always end editing to store changes
                    inputsDataGridView.EndEdit();
                    break;
            }
        }

        private void _editConfigWithInputWizard(DataRow dataRow, InputConfigItem cfg, bool create)
        {
            // refactor!!! dependency to arcaze cache etc not nice
            InputConfigWizard wizard = new InputConfigWizard ( 
                                execManager, 
                                cfg, 
                                execManager.getModuleCache(), 
                                getArcazeModuleSettings(),
                                dataSetConfig, 
                                dataRow["guid"].ToString()
                                );
            wizard.StartPosition = FormStartPosition.CenterParent;
            wizard.SettingsDialogRequested += new EventHandler(wizard_SettingsDialogRequested);
            if (wizard.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                saveToolStripButton.Enabled = true;
                if (dataRow == null) return;
                // do something special
                // Show used Button
                // Show Type of Output
                // Show last set value
            };            
        }

        void wizard_SettingsDialogRequested(object sender, EventArgs e)
        {
            //(sender as InputConfigWizard).Close();
            settingsToolStripMenuItem_Click(sender, null);
        }

        /// <summary>
        /// initializes the config wizard and shows the modal dialog.
        /// afterwards stores some values in the data set so that the visible grid columns are filled with data.
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="cfg"></param>
        /// <param name="create"></param>
        private void _editConfigWithWizard(DataRow dataRow, OutputConfigItem cfg, bool create)
        {
            // refactor!!! dependency to arcaze cache etc not nice
            Form wizard = new ConfigWizard( execManager, 
                                            cfg, 
                                            execManager.getModuleCache(), 
                                            getArcazeModuleSettings(), 
                                            dataSetConfig, 
                                            dataRow["guid"].ToString()
                                          );
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

        private void inputsDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (inputsDataGridView[e.ColumnIndex, e.RowIndex].ReadOnly)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: refactor dependency to module cache
            SettingsDialog dialog = new SettingsDialog(execManager);
            dialog.StartPosition = FormStartPosition.CenterParent;
            if (sender is InputConfigWizard)
            {
                // show the mobiflight tab page
                dialog.tabControl1.SelectedTab = dialog.mobiFlightTabPage;
            }

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.Save();
                // TODO: refactor
                logTextBox.Visible = Log.Instance.Enabled;
                execManager.updateModuleSettings(getArcazeModuleSettings());                
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
                autoRunToolStripButton.Image = MobiFlight.Properties.Resources.lightbulb_on;
            }
            else
            {
                autoRunToolStripButton.Image = MobiFlight.Properties.Resources.lightbulb;
            }
        }

        private void dataGridViewConfig_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewConfig_CellContentClick(sender, e);
        }

        private void inputsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            inputsDataGridView_CellContentClick(sender, e);
        }

        private void dataGridViewConfig_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["guid"].Value = Guid.NewGuid();
        }

        private void inputsDataGridViewConfig_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["inputsGuid"].Value = Guid.NewGuid();
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

        private void deleteInputsRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // do somehting here
            foreach (DataGridViewRow row in inputsDataGridView.SelectedRows)
            {
                // we cannot delete a row which hasn't been saved yet
                if (row.IsNewRow) continue;
                inputsDataGridView.Rows.Remove(row);
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

        private void inputsDataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // handle clicks on header cells or row-header cells
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            switch (inputsDataGridView[e.ColumnIndex, e.RowIndex].OwningColumn.Name)
            {
                case "inputDescription":
                    inputsDataGridView.CurrentCell = inputsDataGridView[e.ColumnIndex, e.RowIndex];
                    inputsDataGridView.BeginEdit(true);
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
                
                OutputConfigItem cfg = ((row.DataBoundItem as DataRowView).Row["settings"] as OutputConfigItem);
                if (cfg != null) {
                    cfg = (cfg.Clone() as OutputConfigItem);
                } else {
                    cfg = new OutputConfigItem();
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

        private void duplicateInputsRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // this is called to ensure 
            // that all data has been stored in
            // the data table
            // otherwise there can occur strange inserts of new rows
            // at the first position instead of the expected one
            this.Validate();

            // do somehting here
            foreach (DataGridViewRow row in inputsDataGridView.SelectedRows)
            {
                // ignore new rows since they cannot be copied nor deleted
                if (row.IsNewRow) continue;

                // get current config item
                // duplicate it
                // duplicate row 
                // link to new config item 
                DataRow currentRow = (row.DataBoundItem as DataRowView).Row;
                DataRow newRow = inputsDataTable.NewRow();

                foreach (DataColumn col in inputsDataTable.Columns)
                {
                    newRow[col.ColumnName] = currentRow[col.ColumnName];
                }

                InputConfigItem cfg = ((row.DataBoundItem as DataRowView).Row["settings"] as InputConfigItem);
                if (cfg != null)
                {
                    cfg = (cfg.Clone() as InputConfigItem);
                }
                else
                {
                    cfg = new InputConfigItem();
                }

                newRow["description"] += " " + _tr("suffixCopy");
                newRow["settings"] = cfg;
                newRow["guid"] = Guid.NewGuid();

                int currentPosition = inputsDataTable.Rows.IndexOf(currentRow);
                if (currentPosition == -1)
                {
                    currentPosition = 1;
                }

                inputsDataTable.Rows.InsertAt(newRow, currentPosition + 1);

                row.Selected = false;
            }
        }

        private void dataGridViewContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // do not show up context menu
            // id there is only the new line visible
            e.Cancel = (dataGridViewConfig.Rows.Count == 1 || (lastClickedRow == dataGridViewConfig.Rows.Count-1));
        }

        private void inputsDataGridViewContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // do not show up context menu
            // id there is only the new line visible
            e.Cancel = (inputsDataGridView.Rows.Count == 1 || (lastClickedRow == inputsDataGridView.Rows.Count - 1));
        }

        private void orphanedSerialsFinderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _checkForOrphanedSerials(true);
        }

        private void donateToolStripButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=7GV3DCC7BXWLY");               
        }

        /// taken from
        /// http://msdn.microsoft.com/en-us/library/ms404305.aspx
        private void tabControl1_DrawItem(Object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush _textBrush = new System.Drawing.SolidBrush(e.ForeColor);

            // Get the item from the collection.
            TabPage _tabPage = inputsTabControl.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle _tabBounds = inputsTabControl.GetTabRect(e.Index);

            if (e.State == DrawItemState.Selected)
            {

                // Draw a different background color, and don't paint a focus rectangle.
                //_textBrush = new SolidBrush(Color.Red);
                //g.FillRectangle(Brushes.Gray, e.Bounds);
            }
            else
            {
                _textBrush = new System.Drawing.SolidBrush(e.ForeColor);
                e.DrawBackground();
            }

            // Use our own font.
            Font _tabFont = new Font("Arial", (float)10.0, FontStyle.Bold, GraphicsUnit.Pixel);

            // Draw string. Center the text.
            StringFormat _stringFlags = new StringFormat();
            _stringFlags.Alignment = StringAlignment.Center;
            _stringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(_tabPage.Text, _tabFont, _textBrush, _tabBounds, new StringFormat(_stringFlags));
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)       // Ctrl-S Save
            {
                // Do what you want here
                e.SuppressKeyPress = true;  // Stops bing! Also sets handled which stop event bubbling
                if (saveToolStripButton.Enabled)
                saveToolStripButton_Click(null, null);
            }
        }

        private void inputsDataGridView_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void inputsDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow gridRow in inputsDataGridView.Rows)
            {
                if (gridRow.DataBoundItem  == null) continue;
                DataRow dataRow = ((gridRow.DataBoundItem as DataRowView).Row as DataRow);
                if (dataRow["settings"] is InputConfigItem)
                {
                    InputConfigItem cfg = (dataRow["settings"] as InputConfigItem);

                    gridRow.Cells["inputName"].Value = cfg.Name + "/" + cfg.ModuleSerial;
                    gridRow.Cells["inputType"].Value = cfg.button != null ? "Button" : "Encoder";
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            CheckForUpdate(false, true);
            AutoUpdater.CheckForUpdateEvent += new AutoUpdater.CheckForUpdateEventHandler(AutoUpdater_CheckForUpdateEvent);
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForUpdate(true);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Started == 0)
            {
                int i = Properties.Settings.Default.Started;
                WelcomeDialog wd = new WelcomeDialog();
                wd.StartPosition = FormStartPosition.CenterParent;
                wd.Text = String.Format(wd.Text, Version);
                wd.ShowDialog();
                this.BringToFront();
            }

            Properties.Settings.Default.Started = Properties.Settings.Default.Started + 1;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_SHOWME)
            {
                ShowMe();
            }
            base.WndProc(ref m);
        }

        private void ShowMe()
        {
            minimizeMainForm(false);
        }
    }
    
    // this class just wraps some Win32 stuff that we're going to use
    internal class NativeMethods
    {
        public const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
    }
}
