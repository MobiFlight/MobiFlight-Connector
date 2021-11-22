﻿using System;
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
#if ARCAZE
using SimpleSolutions.Usb;
#endif
using System.Runtime.InteropServices;
using MobiFlight.FSUIPC;
using System.Reflection;
using MobiFlight.UI.Dialogs;
using MobiFlight.UI.Forms;
using MobiFlight.SimConnectMSFS;
using MobiFlight.UpdateChecker;
using MobiFlight.Base;
using Microsoft.ApplicationInsights.DataContracts;

namespace MobiFlight.UI
{
    public partial class MainForm : Form
    {
        public static String Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        public static String VersionBeta = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
        public static String Build = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime.ToString("yyyyMMdd");

        /// <summary>
        /// the currently used filename of the loaded config file
        /// </summary>
        private string currentFileName = null;
        private ConfigFile configFile = null;

        private CmdLineParams cmdLineParams;

        private ExecutionManager execManager;

        private delegate DialogResult MessageBoxDelegate(string msg, string title, MessageBoxButtons buttons, MessageBoxIcon icon);
        private delegate void VoidDelegate();

        private void InitializeUILanguage()
        {
            if (Properties.Settings.Default.Language != "")
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.Language);
            }
        }

        private void InitializeLogging()
        {
            LogAppenderTextBox logAppenderTextBox = new LogAppenderTextBox(logTextBox);
            LogAppenderFile logAppenderFile = new LogAppenderFile();

            Log.Instance.AddAppender(logAppenderTextBox);
            Log.Instance.AddAppender(logAppenderFile);
            Log.Instance.LogJoystickAxis = Properties.Settings.Default.LogJoystickAxis;
            Log.Instance.Enabled = Properties.Settings.Default.LogEnabled;
            logTextBox.Visible = Log.Instance.Enabled;
            logSplitter.Visible = Log.Instance.Enabled;

            try
            {
                Log.Instance.Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), Properties.Settings.Default.LogLevel, true);
            }
            catch (Exception e)
            {
                Log.Instance.log("MainForm() : Unknown log level", LogSeverity.Error);
            }
            Log.Instance.log("MainForm() : Logger initialized " + Log.Instance.Severity.ToString(), LogSeverity.Info);
        }

        private void InitializeSettings()
        {
            UpgradeSettingsFromPreviousInstallation();
            Properties.Settings.Default.SettingChanging += new System.Configuration.SettingChangingEventHandler(Default_SettingChanging);
        }

        public MainForm()
        {
            // this shall happen before anything else
            InitializeUILanguage();

            // then initialize components
            InitializeComponent();

            // then restore settings
            InitializeSettings();

            // finally set up logging (based on settings)
            InitializeLogging();

            // Initialize the board configurations
            BoardDefinitions.Load();

            // configure tracking correctly
            InitializeTracking();
        }

        private void InitializeTracking()
        {
            AppTelemetry.Instance.Enabled = Properties.Settings.Default.CommunityFeedback;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            panelMain.Visible = false;
            startupPanel.Visible = true;
            menuStrip.Enabled = false;
            toolStrip1.Enabled = false;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Started == 0)
            {
                OnFirstStart();
            }

            if (Properties.Settings.Default.Started > 0 && (Properties.Settings.Default.Started % 30 == 0))
            {
                OnRepeatedStart();
            }

            Properties.Settings.Default.Started = Properties.Settings.Default.Started + 1;

            // this is everything that used to be directly in the constructor
            inputsTabControl.DrawItem += new DrawItemEventHandler(tabControl1_DrawItem);

            cmdLineParams = new CmdLineParams(Environment.GetCommandLineArgs());

            execManager = new ExecutionManager(outputConfigPanel.DataGridViewConfig, inputConfigPanel.InputsDataGridView, this.Handle);
            execManager.OnExecute += new EventHandler(timer_Tick);
            execManager.OnStopped += new EventHandler(timer_Stopped);
            execManager.OnStarted += new EventHandler(timer_Started);

            execManager.OnSimAvailable += ExecManager_OnSimAvailable;
            execManager.OnSimCacheConnectionLost += new EventHandler(fsuipcCache_ConnectionLost);
            execManager.OnSimCacheConnected += new EventHandler(fsuipcCache_Connected);
            execManager.OnSimCacheConnected += new EventHandler(checkAutoRun);
            execManager.OnSimCacheClosed += new EventHandler(fsuipcCache_Closed);
//#if ARCAZE
            execManager.OnModulesConnected += new EventHandler(ArcazeCache_Connected);
            execManager.OnModulesDisconnected += new EventHandler(ArcazeCache_Closed);
            execManager.OnModuleConnectionLost += new EventHandler(ArcazeCache_ConnectionLost);
//#endif
            execManager.OnModuleLookupFinished += new EventHandler(ExecManager_OnModuleLookupFinished);

            execManager.OnTestModeException += new EventHandler(execManager_OnTestModeException);

            execManager.getMobiFlightModuleCache().ModuleConnecting += MainForm_ModuleConnected;

            execManager.OfflineMode = Properties.Settings.Default.OfflineMode;

            if (execManager.OfflineMode) OfflineModeIconToolStripStatusLabel.Image = Properties.Resources.lightbulb_on;
            FsuipcToolStripMenuItem.Image = Properties.Resources.warning;
            simConnectToolStripMenuItem.Image = Properties.Resources.warning;

            // we only load the autorun value stored in settings
            // and do not use possibly passed in autoRun from cmdline
            // because latter shall only have an temporary influence
            // on the program
            setAutoRunValue(Properties.Settings.Default.AutoRun);

            runToolStripButton.Enabled = false;
            runTestToolStripButton.Enabled = false;
            settingsToolStripButton.Enabled = false;
            updateNotifyContextMenu(false);

            // Reset the Title of the Main Window so that it displays the Version too.
            SetTitle("");

            _updateRecentFilesMenuItems();

            // TODO: REFACTOR THIS DEPENDENCY
            outputConfigPanel.ExecutionManager = execManager;
            outputConfigPanel.SettingsChanged += OutputConfigPanel_SettingsChanged;
            outputConfigPanel.SettingsDialogRequested += ConfigPanel_SettingsDialogRequested;

            inputConfigPanel.ExecutionManager = execManager;
            inputConfigPanel.SettingsChanged += InputConfigPanel_SettingsChanged;
            inputConfigPanel.SettingsDialogRequested += ConfigPanel_SettingsDialogRequested;
            inputConfigPanel.OutputDataSetConfig = outputConfigPanel.DataSetConfig;
            inputConfigPanel.SettingsChanged += execManager.OnInputConfigSettingsChanged;

            if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName != "de")
            {
                // change ui icon to english
                donateToolStripButton.Image = Properties.Resources.btn_donate_uk_SM;
            }

            startupPanel.UpdateStatusText("Start Connecting");
#if ARCAZE
            _initializeArcazeModuleSettings();
#endif
            Update();
            Refresh();
            execManager.AutoConnectStart();

            moduleToolStripDropDownButton.DropDownItems.Clear();
            moduleToolStripDropDownButton.ToolTipText = i18n._tr("uiMessageNoModuleFound");
        }

        private void OnRepeatedStart()
        {
            DonateDialog cfpForm = new DonateDialog();
            cfpForm.StartPosition = FormStartPosition.CenterParent;
            if (cfpForm.ShowDialog() == DialogResult.OK)
            {
                // we can track the click.
            }
            this.BringToFront();
        }

        private void OnFirstStart()
        {
            int i = Properties.Settings.Default.Started;
            WelcomeDialog wd = new WelcomeDialog();
            wd.StartPosition = FormStartPosition.CenterParent;
            wd.Text = String.Format(wd.Text, DisplayVersion());
            wd.ShowDialog();
            this.BringToFront();

            // MSFS2020
            WasmModuleUpdater udpater = new WasmModuleUpdater();
            if (udpater.AutoDetectCommunityFolder())
            {
                // MSFS2020 installed
                Msfs2020StartupForm msfsForm = new Msfs2020StartupForm();
                msfsForm.StartPosition = FormStartPosition.CenterParent;
                if (msfsForm.ShowDialog()==DialogResult.OK)
                {
                    InstallWasmModule();
                }
                this.BringToFront();
            }

            // if the user is not participating yet, ask for permission
            if (!Properties.Settings.Default.CommunityFeedback) { 
                CommunityFeedbackStartupForm cfpForm = new CommunityFeedbackStartupForm();
                cfpForm.StartPosition = FormStartPosition.CenterParent;
                if (cfpForm.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.CommunityFeedback = true;
                }
                this.BringToFront();
            }
        }

        private void OutputConfigPanel_SettingsChanged(object sender, EventArgs e)
        {
            saveToolStripButton.Enabled = true;
        }

        private void ConfigPanel_SettingsDialogRequested(object sender, EventArgs e)
        {
            MobiFlightModule module = (sender as MobiFlightModule);
            MobiFlightModuleInfo moduleInfo = null;

            if (module != null) moduleInfo = module.ToMobiFlightModuleInfo();

            ShowSettingsDialog("mobiFlightTabPage", moduleInfo, null, null);
        }

        private void InputConfigPanel_SettingsChanged(object sender, EventArgs e)
        {
            saveToolStripButton.Enabled = true;
        }

        private void MainForm_ModuleConnected(object sender, String text, int progress)
        {
            startupPanel.UpdateStatusText(text);
            if (startupPanel.GetProgressBar() < progress + 10)
                startupPanel.UpdateProgressBar(progress + 10);
        }

        /// <summary>
        /// properly disconnects all connections to FSUIPC and Arcaze
        /// </summary>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            AppTelemetry.Instance.TrackShutdown();
            execManager.Shutdown();
            Properties.Settings.Default.Save();
        } //Form1_FormClosed

        void ExecManager_OnModuleLookupFinished(object sender, EventArgs e)
        {
            startupPanel.UpdateStatusText("Checking for Firmware Updates...");
            startupPanel.UpdateProgressBar(70);
            CheckForFirmwareUpdates();

            startupPanel.UpdateStatusText("Loading last config...");
            startupPanel.UpdateProgressBar(90);
            _autoloadConfig();

            startupPanel.UpdateProgressBar(100);
            panelMain.Visible = true;
            startupPanel.Visible = false;

            menuStrip.Enabled = true;
            toolStrip1.Enabled = true;

            settingsToolStripButton.Enabled = true;
            runToolStripButton.Enabled = RunIsAvailable();
            runTestToolStripButton.Enabled = TestRunIsAvailable();

            AutoUpdateChecker.CheckForUpdate(false, true);

            CheckForWasmModuleUpdate();

            // Track config loaded event
            AppTelemetry.Instance.TrackStart(); 
        }

        private void CheckForWasmModuleUpdate()
        {
            WasmModuleUpdater udpater = new WasmModuleUpdater();
            
        }

        void CheckForFirmwareUpdates ()
        {
            MobiFlightCache mfCache = execManager.getMobiFlightModuleCache();

            List<MobiFlightModuleInfo> modules = mfCache.GetDetectedArduinoModules();
            List<MobiFlightModule> modulesForUpdate = new List<MobiFlightModule>();
            List<MobiFlightModuleInfo> modulesForFlashing = new List<MobiFlightModuleInfo>();

            foreach (MobiFlightModule module in mfCache.GetModules())
            {
                if (module.Board.Info.CanInstallFirmware)
                {
                    Version latestVersion = new Version(module.Board.Info.LatestFirmwareVersion);
                    Version currentVersion = new Version(module.Version != null ? module.Version : "0.0.0");
                    if (currentVersion.CompareTo(latestVersion) < 0)
                    {
                        // Update needed!!!
                        modulesForUpdate.Add(module);
                    }
                }
            }

            foreach (MobiFlightModuleInfo moduleInfo in modules)
            {
                if (moduleInfo.Board.Info.CanInstallFirmware && !moduleInfo.HasMfFirmware())
                {
                    modulesForFlashing.Add(moduleInfo);
                }
            }

            if (Properties.Settings.Default.FwAutoUpdateCheck && (modulesForFlashing.Count > 0 || modulesForUpdate.Count > 0))
            {
                if (!MobiFlightFirmwareUpdater.IsValidArduinoIdePath(Properties.Settings.Default.ArduinoIdePathDefault))
                {
                    ArduinoIdePathForm idePath = new ArduinoIdePathForm();
                    idePath.StartPosition = FormStartPosition.CenterParent;
                    if (idePath.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                }
            }

            if (modulesForUpdate.Count > 0)
            {
                TimeoutMessageDialog tmd = new TimeoutMessageDialog();
                tmd.StartPosition = FormStartPosition.CenterParent;
                tmd.DefaultDialogResult = DialogResult.Cancel;
                tmd.Message = i18n._tr("uiMessageUpdateOldFirmwareOkCancel");
                tmd.Text = i18n._tr("uiMessageUpdateOldFirmwareTitle");
                
                if (tmd.ShowDialog() == DialogResult.OK)
                {
                    if (ShowSettingsDialog("mobiFlightTabPage", null, null, modulesForUpdate) == System.Windows.Forms.DialogResult.OK)
                    {
                    }
                };
            }

            // this is only for non mobiflight boards
            if (Properties.Settings.Default.FwAutoUpdateCheck && modulesForFlashing.Count > 0)
            {
                TimeoutMessageDialog tmd = new TimeoutMessageDialog();
                tmd.StartPosition = FormStartPosition.CenterParent;
                tmd.DefaultDialogResult = DialogResult.Cancel;
                tmd.Message = i18n._tr("uiMessageUpdateArduinoOkCancel");
                tmd.Text = i18n._tr("uiMessageUpdateOldFirmwareTitle");

                if (tmd.ShowDialog() == DialogResult.OK)
                {
                    if (ShowSettingsDialog("mobiFlightTabPage", null, modulesForFlashing, null) == System.Windows.Forms.DialogResult.OK)
                    {
                    }
                }
                else
                {
                    tmd.StartPosition = FormStartPosition.CenterParent;
                    tmd.DefaultDialogResult = DialogResult.Cancel;
                    tmd.Message = i18n._tr("uiMessageUpdateArduinoFwAutoDisableYesNo");
                    tmd.Text = i18n._tr("Hint");

                    if (tmd.ShowDialog() == DialogResult.OK)
                    {
                        Properties.Settings.Default.FwAutoUpdateCheck = false;
                    };
                }
            }
        }

        private DialogResult ShowSettingsDialog(String SelectedTab, MobiFlightModuleInfo SelectedBoard, List<MobiFlightModuleInfo> BoardsForFlashing, List<MobiFlightModule> BoardsForUpdate)
        {
            SettingsDialog dlg = new SettingsDialog(execManager);
            dlg.StartPosition = FormStartPosition.CenterParent;
            switch(SelectedTab)
            {
                case "mobiFlightTabPage":
                    dlg.tabControl1.SelectedTab = dlg.mobiFlightTabPage;
                    break;
                case "ArcazeTabPage":
                    dlg.tabControl1.SelectedTab = dlg.ArcazeTabPage;
                    break;
            }
            if (SelectedBoard != null)
                dlg.PreselectedBoard = SelectedBoard;

            if (BoardsForFlashing != null)
                dlg.MobiFlightModulesForFlashing = BoardsForFlashing;

            if (BoardsForUpdate != null)
                dlg.MobiFlightModulesForUpdate = BoardsForUpdate;
            return dlg.ShowDialog();
        }

        // this performs the update of the existing user settings 
        // when updating to a new MobiFlight Version
        private void UpgradeSettingsFromPreviousInstallation()
        {
            if (Properties.Settings.Default.UpgradeRequired)
            {
                try
                {
                    Properties.Settings.Default.Upgrade();
                }
                catch
                {
                    // If the properties file is corrupted for some reason catch the exception and
                    // reset back to a default version.

                    Properties.Settings.Default.Reset();
                }
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.StartedTotal += Properties.Settings.Default.Started;
                Properties.Settings.Default.Started = 0;
                Properties.Settings.Default.Save();
            }
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoUpdateChecker.CheckForUpdate(true);
        }

        private void startAutoConnectThreadSafe()
        {
            execManager.AutoConnectStart();
        }

        private DialogResult ShowMessageThreadSafe(string msg, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(this, msg, title, buttons);
        }

        

        /*
        private void CheckForUpdate(bool force, bool silent = false)
        {
            AutoUpdater.Mandatory = force;
            AutoUpdater.ShowSkipButton = true;
            AutoUpdater.ShowRemindLaterButton = true;
            AutoUpdater.UpdateFormSize = new System.Drawing.Size(600, 500);

            if (Properties.Settings.Default.CacheId == "0") Properties.Settings.Default.CacheId = Guid.NewGuid().ToString();

            String trackingParams = "?cache=" + Properties.Settings.Default.CacheId + "-" + Properties.Settings.Default.Started;

            Log.Instance.log("Checking for updates", LogSeverity.Info);
            AutoUpdater.RunUpdateAsAdmin = true;
            SilentUpdateCheck = silent;

            String updateUrl = MobiFlightUpdateUrl;
            if (Properties.Settings.Default.BetaUpdates) updateUrl = MobiFlightUpdateBetasUrl;
#if DEBUG
            if (Properties.Settings.Default.BetaUpdates) updateUrl = MobiFlightUpdateDebugUrl;
#endif
            AutoUpdater.DownloadPath = Environment.CurrentDirectory;
            AutoUpdater.Start(updateUrl + trackingParams + "1");
        }
        */

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

            if (e.SettingName == "OfflineMode")
            {
                execManager.OfflineMode = (bool)e.NewValue;
                execManager.ReconnectSim();
                if (execManager.OfflineMode) OfflineModeIconToolStripStatusLabel.Image = Properties.Resources.lightbulb_on;
                else OfflineModeIconToolStripStatusLabel.Image = Properties.Resources.lightbulb;
            }

            if (e.SettingName == "CommunityFeedback")
            {
                AppTelemetry.Instance.Enabled = Properties.Settings.Default.CommunityFeedback;
            }

            if (e.SettingName == "LogEnabled")
            {
                logTextBox.Visible = (bool) e.NewValue;
                logSplitter.Visible = (bool) e.NewValue;
            }
        }

        private void _autoloadConfig()
        {            
            if (cmdLineParams.ConfigFile != null)
            {
                if (!System.IO.File.Exists(cmdLineParams.ConfigFile))
                {
                    MessageBox.Show(
                                i18n._tr("uiMessageCmdParamConfigFileDoesNotExist") + "\r" + cmdLineParams.ConfigFile,
                                i18n._tr("Hint"),
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

#if ARCAZE
        private void _initializeArcazeModuleSettings()
        {
            if (!Properties.Settings.Default.ArcazeSupportEnabled) return;

            Dictionary<string, ArcazeModuleSettings> settings = execManager.getModuleCache().GetArcazeModuleSettings();
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
                                i18n._tr("uiMessageModulesNotConfiguredYet"),
                                i18n._tr("Hint"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.OK)
                {
                    if (ShowSettingsDialog("ArcazeTabPage", null, null, null) == System.Windows.Forms.DialogResult.OK)
                    {
                    }
                }
            }           
 
            execManager.updateModuleSettings(execManager.getModuleCache().GetArcazeModuleSettings());
        }
#endif
        void ArcazeCache_ConnectionLost(object sender, EventArgs e)
        {
            //_disconnectArcaze();
            _showError(i18n._tr("uiMessageArcazeConnectionLost"));            
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void ArcazeCache_Closed(object sender, EventArgs e)
        {
            ModuleStatusIconToolStripLabel.Image = Properties.Resources.warning;
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void ArcazeCache_Connected(object sender, EventArgs e)
        {
            ModuleStatusIconToolStripLabel.Image = Properties.Resources.check;
            fillComboBoxesWithArcazeModules();
            runTestToolStripButton.Enabled = TestRunIsAvailable();
        }

        /// <summary>
        /// Returns true if the run button should be enabled based on various MobiFlight states.
        /// </summary>
        private bool RunIsAvailable()
        {
            return 
                   // Offline Mode Or Sim available
                   (execManager.OfflineMode || execManager.SimConnected()) &&
                   // Hardware available
                   (execManager.ModulesConnected() || execManager.GetJoystickManager().JoysticksConnected()) && 
                   // We are not already running
                   !execManager.IsStarted() && !execManager.TestModeIsStarted();
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void fsuipcCache_Closed(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(SimConnectCache))
            {
                simConnectToolStripMenuItem.Image = Properties.Resources.warning;
            }
            else if (sender.GetType() == typeof(Fsuipc2Cache))
            {
                FsuipcToolStripMenuItem.Image = Properties.Resources.warning;
            }

            SimConnectionIconStatusToolStripStatusLabel.Image = Properties.Resources.warning;

            runToolStripButton.Enabled = RunIsAvailable();
        }

        private void ExecManager_OnSimAvailable(object sender, EventArgs e)
        {
            FlightSimType flightSim = (FlightSimType) sender;

            switch (flightSim)
            {
                case FlightSimType.MSFS2020:
                    noSimRunningToolStripMenuItem.Text = "MSFS2020 Detected";
                    noSimRunningToolStripMenuItem.Image = Properties.Resources.check;
                    break;

                case FlightSimType.FS9:
                    noSimRunningToolStripMenuItem.Text = "FS2004 Detected";
                    noSimRunningToolStripMenuItem.Image = Properties.Resources.check;
                    break;

                case FlightSimType.FSX:
                    noSimRunningToolStripMenuItem.Text = "FSX Detected";
                    noSimRunningToolStripMenuItem.Image = Properties.Resources.check;
                    break;

                case FlightSimType.P3D:
                    noSimRunningToolStripMenuItem.Text = "P3D Detected";
                    noSimRunningToolStripMenuItem.Image = Properties.Resources.check;
                    break;

                case FlightSimType.XPLANE:
                    noSimRunningToolStripMenuItem.Text = "X-Plane Detected";
                    noSimRunningToolStripMenuItem.Image = Properties.Resources.check;
                    break;

                case FlightSimType.UNKNOWN:
                    noSimRunningToolStripMenuItem.Text = "Unkown Detected";
                    noSimRunningToolStripMenuItem.Image = Properties.Resources.module_unknown;
                    break;

                default:
                    noSimRunningToolStripMenuItem.Text = "Undefined";
                    break;
            }
            noSimRunningToolStripMenuItem.Image = Properties.Resources.check;
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>        
        void fsuipcCache_Connected(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(SimConnectCache) && FlightSim.FlightSimType == FlightSimType.MSFS2020)
            {
                noSimRunningToolStripMenuItem.Text = "MSFS2020 Detected";
                simConnectToolStripMenuItem.Text = "WASM Module (MSFS2020)";
                simConnectToolStripMenuItem.Image = Properties.Resources.check;
                AppTelemetry.Instance.TrackFlightSimConnected(FlightSim.FlightSimType.ToString(), "SimConnect");
            }

            else if (sender.GetType() == typeof(Fsuipc2Cache)) { 

                Fsuipc2Cache c = sender as Fsuipc2Cache;
                switch (FlightSim.FlightSimConnectionMethod)
                {
                    case FlightSimConnectionMethod.FSUIPC:
                        FsuipcToolStripMenuItem.Text = i18n._tr("fsuipcStatus") + " ("+ FlightSim.FlightSimType.ToString() +")";
                        break;

                    case FlightSimConnectionMethod.XPUIPC:
                        FsuipcToolStripMenuItem.Text = "XPUIPC Status";
                        break;

                    case FlightSimConnectionMethod.WIDECLIENT:
                        FsuipcToolStripMenuItem.Text = "WideClient Status";
                        break;
                }
                FsuipcToolStripMenuItem.Image = Properties.Resources.check;
                AppTelemetry.Instance.TrackFlightSimConnected(FlightSim.FlightSimType.ToString(), c.FlightSimConnectionMethod.ToString());
            }

            if (execManager.SimConnected())
            {
                SimConnectionIconStatusToolStripStatusLabel.Image = Properties.Resources.check;
            }

            runToolStripButton.Enabled = RunIsAvailable();
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
            if (!execManager.SimAvailable())
            {
                _showError(i18n._tr("uiMessageFsHasBeenStopped"));
            } else {
                if (sender.GetType() == typeof(SimConnectCache))
                {
                    _showError(i18n._tr("uiMessageSimConnectConnectionLost"));
                } else
                {
                    _showError(i18n._tr("uiMessageFsuipcConnectionLost"));
                }
            } //if
            execManager.Stop();
        }

        /// <summary>
        /// handler which sets the states of UI elements when timer gets started
        /// </summary>
        void timer_Started(object sender, EventArgs e)
        {
            runToolStripButton.Enabled  = RunIsAvailable();
            runTestToolStripButton.Enabled = TestRunIsAvailable();
            stopToolStripButton.Enabled = true;
            updateNotifyContextMenu(execManager.IsStarted());
        } //timer_Started()

        /// <summary>
        /// handler which sets the states of UI elements when timer gets stopped
        /// </summary>
        void timer_Stopped(object sender, EventArgs e)
        {
            runToolStripButton.Enabled = RunIsAvailable();
            runTestToolStripButton.Enabled = TestRunIsAvailable();
            stopToolStripButton.Enabled = false;
            updateNotifyContextMenu(execManager.IsStarted());
        } //timer_Stopped

        private bool TestRunIsAvailable()
        {
            return execManager.ModulesConnected() && !execManager.TestModeIsStarted() && !execManager.IsStarted();
        }

        /// <summary>
        /// Timer eventhandler
        /// </summary>        
        void timer_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel.Text += ".";
            if (toolStripStatusLabel.Text.Length > (10 + i18n._tr("Running").Length))
            {
                toolStripStatusLabel.Text = i18n._tr("Running");
            }
        } //timer_Tick()

        /// <summary>
        /// gathers infos about the connected modules and stores information in different objects
        /// </summary>
        /// <returns>returns true if there are modules present</returns>
        private bool fillComboBoxesWithArcazeModules()
        {
            // remove the items from all comboboxes
            // and set default items
            bool modulesFound = false;
            moduleToolStripDropDownButton.DropDownItems.Clear();
            moduleToolStripDropDownButton.ToolTipText = i18n._tr("uiMessageNoModuleFound");
#if ARCAZE
            // TODO: refactor!!!
            foreach (IModuleInfo module in execManager.getModuleCache().getModuleInfo())
            {
                moduleToolStripDropDownButton.DropDownItems.Add(module.Name + "/ " + module.Serial);
                modulesFound = true;
            }
#endif

#if MOBIFLIGHT
            foreach (IModuleInfo module in execManager.getMobiFlightModuleCache().getModuleInfo())
            {
                ToolStripDropDownItem item = new ToolStripMenuItem($"{module.Name} ({module.Port})");
                item.Tag = module;
                item.Click += statusToolStripMenuItemClick;
                moduleToolStripDropDownButton.DropDownItems.Add(item);
                modulesFound = true;
            }
#endif
            if (modulesFound)
            {
                moduleToolStripDropDownButton.ToolTipText = i18n._tr("uiMessageModuleFound");
            }
            // only enable button if modules are available            
            return (modulesFound);
        } //fillComboBoxesWithArcazeModules()

        private void statusToolStripMenuItemClick(object sender, EventArgs e)
        {
            MobiFlightModuleInfo moduleInfo = (sender as ToolStripMenuItem).Tag as MobiFlightModuleInfo;

            ShowSettingsDialog("mobiFlightTabPage", moduleInfo, null, null);
        }

        /// <summary>
        /// toggles the current timer when user clicks on respective run/stop buttons
        /// </summary>
        private void ButtonToggleStart_Click(object sender, EventArgs e)
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
                MessageBox.Show(msg, i18n._tr("Hint"), MessageBoxButtons.OK, MessageBoxIcon.Warning); 
            }
            else
            {
                notifyIcon.ShowBalloonTip(1000, i18n._tr("Hint"), msg, ToolTipIcon.Warning);
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
                notifyIcon.BalloonTipTitle = i18n._tr("uiMessageMFConnectorInterfaceActive");
                notifyIcon.BalloonTipText = i18n._tr("uiMessageApplicationIsRunningInBackgroundMode");
                notifyIcon.ShowBalloonTip(1000);               
                this.Hide();
            }
            else
            {
                notifyIcon.Visible = false;                
                this.Show();
                if (this.WindowState!=FormWindowState.Normal)
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
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) return;
            minimizeMainForm(false);
        }

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

        private void mergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "MobiFlight Connector Config (*.mcc)|*.mcc|ArcazeUSB Interface Config (*.aic) |*.aic";

            if (DialogResult.OK == fd.ShowDialog())
            {
                _loadConfig(fd.FileName, true);
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
        private void _loadConfig(string fileName, bool merge = false)
        {
            if (!System.IO.File.Exists(fileName))
            {
                MessageBox.Show(i18n._tr("uiMessageConfigNotFound"), i18n._tr("Hint"));
                return;
            }

            if (fileName.IndexOf(".aic") != -1)
            {
                if (MessageBox.Show(i18n._tr("uiMessageMigrateConfigFileYesNo"), i18n._tr("Hint"), MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
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

                    if (MessageBox.Show(i18n._tr("uiMessageMigrateConfigFileV60YesNo"), i18n._tr("Hint"), MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
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

            if (!merge) { 
                outputConfigPanel.DataSetConfig.Clear();
                inputConfigPanel.InputDataSetConfig.Clear();
            }

            configFile = new ConfigFile(fileName);
            try
            {
                // refactor!!!
                outputConfigPanel.DataSetConfig.ReadXml(configFile.getOutputConfig());
            }
            catch (Exception ex)
            {
                MessageBox.Show(i18n._tr("uiMessageProblemLoadingConfig"), i18n._tr("Hint"));
                return;
            }

            try
            {
                // refactor!!!
                inputConfigPanel.InputDataSetConfig.ReadXml(configFile.getInputConfig());
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

            if (!merge)
            {
                currentFileName = fileName;
                _setFilenameInTitle(fileName);
                _storeAsRecentFile(fileName);

                // set the button back to "disabled"
                // since due to initiliazing the dataSet
                // it will automatically gets enabled
                saveToolStripButton.Enabled = false;
            } else
            {
                // indicate that the merge changed
                // the current config and that the user
                saveToolStripButton.Enabled = true;
            }

            // always put this after "normal" initialization
            // savetoolstripbutton may be set to "enabled"
            // if user has changed something
            _checkForOrphanedSerials( false );
            _checkForOrphanedJoysticks( false );

            // Track config loaded event
            AppTelemetry.Instance.ConfigLoaded(configFile);
            AppTelemetry.Instance.TrackBoardStatistics(execManager);
            AppTelemetry.Instance.TrackSettings();
        }

        private void _checkForOrphanedJoysticks(bool showNotNecessaryMessage)
        {
            List<string> serials = new List<string>();
            List<string> NotConnectedJoysticks = new List<string>();

            foreach (Joystick j in execManager.GetJoystickManager().GetJoysticks())
            {
                serials.Add(j.Name + " / " + j.Serial);
            }

            if (serials.Count == 0) return;
            if (configFile == null) return;

            foreach (InputConfigItem item in configFile.GetInputConfigItems())
            {
                if (item.ModuleSerial.Contains(Joystick.SerialPrefix) &&
                    !serials.Contains(item.ModuleSerial) && 
                    !NotConnectedJoysticks.Contains(item.ModuleSerial)) { 
                    NotConnectedJoysticks.Add(item.ModuleSerial);
                }
            }

            if (NotConnectedJoysticks.Count>0)
            {
                TimeoutMessageDialog tmd = new TimeoutMessageDialog();
                tmd.HasCancelButton = false;
                tmd.StartPosition = FormStartPosition.CenterParent;
                tmd.Message = string.Format(
                                    i18n._tr("uiMessageNotConnectedJoysticksInConfigFound"),
                                    string.Join("\n", NotConnectedJoysticks)
                                    );
                tmd.Text = i18n._tr("Hint");
                tmd.ShowDialog();
            }
            else if (showNotNecessaryMessage)
            {
                MessageBox.Show(i18n._tr("uiMessageNoNotConnectedJoysticksInConfigFound"), i18n._tr("Hint"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void _restoreValuesInGridView()
        {
            outputConfigPanel.RestoreValuesInGridView();
            inputConfigPanel.RestoreValuesInGridView();
        }

        private void _checkForOrphanedSerials(bool showNotNecessaryMessage)
        {
            List<string> serials = new List<string>();
            
            foreach (IModuleInfo moduleInfo in execManager.GetAllConnectedModulesInfo())
            {
                serials.Add(moduleInfo.Name + "/ " + moduleInfo.Serial);
            }

            if (serials.Count == 0) return;

            try
            {
                OrphanedSerialsDialog opd = new OrphanedSerialsDialog(serials, outputConfigPanel.ConfigDataTable, inputConfigPanel.ConfigDataTable);
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
                    MessageBox.Show(i18n._tr("uiMessageNoOrphanedSerialsFound"), i18n._tr("Hint"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception e) {
                // do nothing
                Log.Instance.log("Orphaned Serials Exception. " + e.Message, LogSeverity.Error);
            }
        }

        private void SetTitle(string title)
        {
            string NewTitle = "MobiFlight Connector ("+ Version +")";
            if (VersionBeta.Split('.')[3]!="0") {
                NewTitle = "MobiFlight Connector BETA (" + VersionBeta + ")";
            }
            if (title!=null && title!="")
            {
                NewTitle = title + " - " + NewTitle;
            }

            Text = NewTitle;
        }

        public static String DisplayVersion ()
        {
            if (VersionBeta.Split('.')[3] != "0")
            {
                return VersionBeta + " (BETA)";
            }

            return Version;
        }

        private void _setFilenameInTitle(string fileName)
        {
            SetTitle(fileName.Substring(fileName.LastIndexOf('\\')+1));
        }

        /// <summary>
        /// due to the new settings-node there must be some routine to load 
        /// data from legacy config files
        /// </summary>
        private void _applyBackwardCompatibilityLoading()
        {            
            foreach (DataRow row in outputConfigPanel.ConfigDataTable.Rows) {
                if (row["settings"].GetType() == typeof(System.DBNull))
                {
                    OutputConfigItem cfgItem = new OutputConfigItem();

                    if (row["fsuipcOffset"].GetType() != typeof(System.DBNull))
                        cfgItem.FSUIPC.Offset = Int32.Parse(row["fsuipcOffset"].ToString().Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);

                    if (row["fsuipcSize"].GetType() != typeof(System.DBNull))
                        cfgItem.FSUIPC.Size = Byte.Parse(row["fsuipcSize"].ToString());

                    if (row["mask"].GetType() != typeof(System.DBNull))
                        cfgItem.FSUIPC.Mask = (row["mask"].ToString() != "") ? Int32.Parse(row["mask"].ToString()) : Int32.MaxValue;

                    // comparison
                    if (row["comparison"].GetType() != typeof(System.DBNull))
                    {
                        cfgItem.Comparison.Active = true;
                        cfgItem.Comparison.Operand = row["comparison"].ToString();
                    }

                    if (row["comparisonValue"].GetType() != typeof(System.DBNull))
                    {
                        cfgItem.Comparison.Value = row["comparisonValue"].ToString();
                    }

                    if (row["converter"].GetType() != typeof(System.DBNull))
                    {
                        if (row["converter"].ToString() == "Boolean")
                        {
                            cfgItem.Comparison.IfValue = "1";
                            cfgItem.Comparison.ElseValue = "0";
                        }
                    }

                    if (row["trigger"].GetType() != typeof(System.DBNull))
                    {
                        cfgItem.DisplayTrigger = row["trigger"].ToString();
                    }

                    if (row["usbArcazePin"].GetType() != typeof(System.DBNull))
                    {
                        cfgItem.DisplayType = "Pin";
                        cfgItem.Pin.DisplayPin = row["usbArcazePin"].ToString();
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
            outputConfigPanel.ApplyBackwardCompatibilitySaving();

            ConfigFile configFile = new ConfigFile(fileName);
            configFile.SaveFile(outputConfigPanel.DataSetConfig, inputConfigPanel.InputDataSetConfig);
            currentFileName=fileName;
            _restoreValuesInGridView();
            _storeAsRecentFile(fileName);
            _setFilenameInTitle(fileName);
            saveToolStripButton.Enabled = false;
        }

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
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
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
                       i18n._tr("uiMessageConfirmNewConfig"),
                       i18n._tr("uiMessageConfirmNewConfigTitle"), 
                       MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                execManager.Stop();
                currentFileName = null;
                _setFilenameInTitle(i18n._tr("DefaultFileName"));
                outputConfigPanel.ConfigDataTable.Clear();
                inputConfigPanel.ConfigDataTable.Clear();
            };
        } //toolStripMenuItem3_Click()

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
            runTestToolStripButton.Enabled = TestRunIsAvailable();
            runToolStripButton.Enabled = RunIsAvailable();
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
            runTestToolStripButton.Enabled = TestRunIsAvailable();
            runToolStripButton.Enabled = RunIsAvailable();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ShowSettingsDialog("GeneralTabPage", null, null, null) == System.Windows.Forms.DialogResult.OK)
            {
#if ARCAZE
                execManager.updateModuleSettings(execManager.getModuleCache().GetArcazeModuleSettings());
#endif
            }
        }

        private void AutoRunToolStripButton_Click(object sender, EventArgs e)
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            execManager.Stop();
            if (saveToolStripButton.Enabled && MessageBox.Show(
                       i18n._tr("uiMessageConfirmDiscardUnsaved"),
                       i18n._tr("uiMessageConfirmDiscardUnsavedTitle"),
                       MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // only cancel closing if not saved before
                // which is indicated by empty currentFileName
                e.Cancel = (currentFileName == null);
                saveToolStripButton_Click(saveToolStripButton, new EventArgs());                
            };
        }



        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(i18n._tr("WebsiteUrlHelp"));
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

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_SHOWME)
            {
                ShowMe();
            }
            if (m.Msg == SimConnectMSFS.SimConnectCache.WM_USER_SIMCONNECT) execManager?.HandleWndProc(ref m);

            base.WndProc(ref m);
        }

        private void ShowMe()
        {
            minimizeMainForm(false);
        }

        private void installWasmModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InstallWasmModule();
        }

        private static void InstallWasmModule()
        {
            WasmModuleUpdater updater = new WasmModuleUpdater();

            try {

                if (!updater.AutoDetectCommunityFolder())
                {
                    TimeoutMessageDialog.Show(
                       i18n._tr("uiMessageWasmUpdateCommunityFolderNotFound"),
                       i18n._tr("uiMessageWasmUpdater"),
                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!updater.WasmModulesAreDifferent())
                {
                    TimeoutMessageDialog.Show(
                       i18n._tr("uiMessageWasmUpdateAlreadyInstalled"),
                       i18n._tr("uiMessageWasmUpdater"),
                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (updater.InstallWasmModule())
                {
                    TimeoutMessageDialog.Show(
                       i18n._tr("uiMessageWasmUpdateInstallationSuccessful"),
                       i18n._tr("uiMessageWasmUpdater"),
                       MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }

            } catch (Exception e) {
                Log.Instance.log(e.Message, LogSeverity.Error);
            }

            // We only get here in case of an error.
            TimeoutMessageDialog.Show(
                i18n._tr("uiMessageWasmUpdateInstallationError"),
                i18n._tr("uiMessageWasmUpdater"),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void downloadLatestEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WasmModuleUpdater updater = new WasmModuleUpdater();

            if (!updater.AutoDetectCommunityFolder())
            {
                TimeoutMessageDialog.Show(
                   i18n._tr("uiMessageWasmUpdateCommunityFolderNotFound"),
                   i18n._tr("uiMessageWasmUpdater"),
                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (updater.InstallWasmEvents())
            {
                TimeoutMessageDialog.Show(
                   i18n._tr("uiMessageWasmEventsInstallationSuccessful"),
                   i18n._tr("uiMessageWasmUpdater"),
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                TimeoutMessageDialog.Show(
                   i18n._tr("uiMessageWasmEventsInstallationError"),
                   i18n._tr("uiMessageWasmUpdater"),
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openDiscordServer_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/U28QeEJpBV");
        }

        private void StatusBarToolStripButton_Click(object sender, EventArgs e)
        {
            ShowSettingsDialog("mobiFlightTabPage", null, null, null);
        }

        private void YouTubeToolStripButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.youtube.com/channel/UCxsoCWDKRyu3MpQKNZEXUYA");
        }
    }

    internal static class Helper
    {
        public static void DoubleBufferedDGV(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
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
