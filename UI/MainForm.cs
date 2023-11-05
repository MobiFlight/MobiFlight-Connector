using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
#if ARCAZE
#endif
using System.Runtime.InteropServices;
using MobiFlight.FSUIPC;
using System.Reflection;
using MobiFlight.UI.Dialogs;
using MobiFlight.UI.Forms;
using MobiFlight.SimConnectMSFS;
using MobiFlight.UpdateChecker;
using MobiFlight.Base;
using MobiFlight.xplane;
using MobiFlight.HubHop;
using System.Threading.Tasks;
using MobiFlight.InputConfig;
using Newtonsoft.Json;

namespace MobiFlight.UI
{
    public partial class MainForm : Form
    {
        private delegate void UpdateAircraftCallback(string aircraftName);
        private delegate DialogResult MessageBoxDelegate(string msg, string title, MessageBoxButtons buttons, MessageBoxIcon icon);
        private delegate void VoidDelegate();

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
        private Dictionary<string, string> AutoLoadConfigs = new Dictionary<string, string>();
        public event EventHandler<string> CurrentFilenameChanged;

        public string CurrentFileName {
            get { return currentFileName; }
            set {
                if (currentFileName == value) return;
                currentFileName = value;
                CurrentFilenameChanged?.Invoke(this, value);
            }
        }

        private void InitializeUILanguage()
        {
            if (Properties.Settings.Default.Language != "")
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.Language);
            }
        }

        public ConfigFile ConfigFile { get { return configFile; } }

        public bool InitialLookupFinished { get; private set; } = false;
        public bool SettingsDialogActive { get; private set; }

        public event EventHandler<ConfigFile> ConfigLoaded;

        private void InitializeLogging()
        {
            LogAppenderLogPanel logAppenderTextBox = new LogAppenderLogPanel(logPanel1);
            LogAppenderFile logAppenderFile = new LogAppenderFile();

            Log.Instance.AddAppender(logAppenderTextBox);
            Log.Instance.AddAppender(logAppenderFile);
            Log.Instance.LogJoystickAxis = Properties.Settings.Default.LogJoystickAxis;
            Log.Instance.Enabled = Properties.Settings.Default.LogEnabled;
            logPanel1.Visible = Log.Instance.Enabled;
            logSplitter.Visible = Log.Instance.Enabled;

            try
            {
                Log.Instance.Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), Properties.Settings.Default.LogLevel, true);
            }
            catch (Exception e)
            {
                Log.Instance.log("Unknown log level.", LogSeverity.Error);
            }
            Log.Instance.log($"Logger initialized {Log.Instance.Severity}", LogSeverity.Info);
        }

        private void InitializeSettings()
        {
            UpgradeSettingsFromPreviousInstallation();
            Properties.Settings.Default.SettingChanging += new System.Configuration.SettingChangingEventHandler(Default_SettingChanging);
            UpdateAutoLoadConfig();
            RestoreAutoLoadConfig();
            CurrentFilenameChanged += (s, e) => { UpdateAutoLoadMenu(); };

            // we trigger this once:
            // because on a full fresh start
            // there are no recent files which
            // could lead to a filename change
            UpdateAutoLoadMenu();
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

            // Initialize the custom device configurations
            CustomDevices.CustomDeviceDefinitions.Load();

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
            startupPanel.Dock = DockStyle.Fill;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Check for updates before loading anything else
#if (!DEBUG)
            AutoUpdateChecker.CheckForUpdate(true);
#endif

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
            execManager.OnExecute += new EventHandler(ExecManager_Executed);
            execManager.OnStopped += new EventHandler(ExecManager_Stopped);
            execManager.OnStarted += new EventHandler(ExecManager_Started);
            execManager.OnShutdown += new EventHandler(ExecManager_OnShutdown);

            execManager.OnSimAvailable += ExecManager_OnSimAvailable;
            execManager.OnSimUnavailable += ExecManager_OnSimUnavailable;
            execManager.OnSimCacheConnectionLost += new EventHandler(fsuipcCache_ConnectionLost);
            execManager.OnSimCacheConnected += new EventHandler(fsuipcCache_Connected);
            execManager.OnSimCacheConnected += new EventHandler(checkAutoRun);
            execManager.OnSimCacheClosed += new EventHandler(fsuipcCache_Closed);
            execManager.OnSimAircraftChanged += ExecManager_OnSimAircraftChanged;

            // working hypothesis: we don't need this at all.
            // execManager.OnModuleCacheAvailable += new EventHandler(ModuleCache_Available);

            execManager.OnModuleConnected += new EventHandler(Module_Connected);
            execManager.OnModuleRemoved += new EventHandler(Module_Removed);
            execManager.OnInitialModuleLookupFinished += new EventHandler(ExecManager_OnInitialModuleLookupFinished);
            execManager.OnTestModeException += new EventHandler(execManager_OnTestModeException);

            moduleToolStripDropDownButton.DropDownDirection = ToolStripDropDownDirection.AboveRight;
            toolStripDropDownButton1.DropDownDirection = ToolStripDropDownDirection.AboveRight;
            toolStripAircraftDropDownButton.DropDownDirection = ToolStripDropDownDirection.AboveRight;

            SimConnectionIconStatusToolStripStatusLabel.Image = Properties.Resources.warning;
            SimProcessDetectedToolStripMenuItem.Image = Properties.Resources.warning;
            FsuipcToolStripMenuItem.Image = Properties.Resources.warning;
            simConnectToolStripMenuItem.Image = Properties.Resources.warning;
            xPlaneDirectToolStripMenuItem.Image = Properties.Resources.warning;

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
        private void ExecManager_OnSimAircraftChanged(object sender, string aircraftName)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateAircraftCallback(UpdateAircraft), new object[] { aircraftName });
            }
            else
            {
                UpdateAircraft(aircraftName);
            }
        }

        private void UpdateAircraft(String aircraftName)
        {
            if (aircraftName == "")
            {
                aircraftName = i18n._tr("uiLabelNoAircraftDetected.");
            }

            toolStripAircraftDropDownButton.Text = aircraftName;
            toolStripAircraftDropDownButton.DropDown.Enabled = true;

            var key = $"{FlightSim.FlightSimType}:{aircraftName}";

            if (!Properties.Settings.Default.AutoLoadLinkedConfig ||
                !AutoLoadConfigs.ContainsKey(key))
            {
                UpdateAutoLoadMenu();
                return;
            }

            var filename = AutoLoadConfigs[key];

            // we only really load the config if it is different from 
            // the current one.
            // the orphaned serials dialog would pop up multiple times
            // especially because we get two events sometimes:
            //      one coming from FSUIPC and
            //      one coming from SimConnect
            if (CurrentFileName == filename)
            {
                // we still have to update the menu correctly.
                UpdateAutoLoadMenu();
                return;
            }

            if (saveToolStripButton.Enabled && MessageBox.Show(
                       i18n._tr("uiMessageConfirmDiscardUnsaved"),
                       i18n._tr("uiMessageConfirmDiscardUnsavedTitle"),
                       MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                saveToolStripButton_Click(saveToolStripButton, new EventArgs());
            }

            Log.Instance.log($"Auto loading config for {aircraftName}", LogSeverity.Info);
            LoadConfig(filename);
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
            wd.WebsiteUrl = $"https://github.com/MobiFlight/MobiFlight-Connector/releases/tag/{CurrentVersion()}";
            wd.ReleaseNotesClicked += (sender, e) =>
            {
                Process.Start($"https://github.com/MobiFlight/MobiFlight-Connector/releases/tag/{CurrentVersion()}");
            };

            wd.StartPosition = FormStartPosition.CenterParent;
            wd.Text = String.Format(wd.Text, DisplayVersion());
            wd.ShowDialog();
            this.BringToFront();

            // MSFS2020
            WasmModuleUpdater updater = new WasmModuleUpdater();
            if (updater.AutoDetectCommunityFolder())
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
            UpdateAllConnectionIcons();
        }

        private void UpdateAllConnectionIcons()
        {
            UpdateSimStatusIcon();
            UpdateSimConnectStatusIcon();
            UpdateXplaneDirectConnectStatusIcon();
            UpdateFsuipcStatusIcon();
            UpdateSeparatorInStatusMenu();
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
            UpdateAllConnectionIcons();
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

        void ExecManager_OnInitialModuleLookupFinished(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new EventHandler(ExecManager_OnInitialModuleLookupFinished), new object[] { sender, e });
                return;
            }

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

            CheckForWasmModuleUpdate();

            UpdateAllConnectionIcons();

            UpdateStatusBarModuleInformation();

            // Track config loaded event
            AppTelemetry.Instance.TrackStart();

            InitialLookupFinished = true;
        }

        private void CheckForWasmModuleUpdate()
        {
            WasmModuleUpdater updater = new WasmModuleUpdater();
            
        }

        void CheckForFirmwareUpdates ()
        {
            MobiFlightCache mfCache = execManager.getMobiFlightModuleCache();

            List<MobiFlightModuleInfo> modules = mfCache.GetDetectedCompatibleModules();
            List<MobiFlightModule> modulesForUpdate = new List<MobiFlightModule>();
            List<MobiFlightModuleInfo> modulesForFlashing = new List<MobiFlightModuleInfo>();

            foreach (MobiFlightModule module in mfCache.GetModules())
            {
                if (module.Board.Info.CanInstallFirmware)
                {
                    if (module.FirmwareRequiresUpdate())
                    {
                        // Update needed!!!
                        modulesForUpdate.Add(module);
                    }
                }
            }

            foreach (MobiFlightModuleInfo moduleInfo in modules)
            {
                if (moduleInfo.Type == "Ignored") continue;

                if (moduleInfo.FirmwareInstallPossible())
                {
                    modulesForFlashing.Add(moduleInfo);
                }
            }

            // Connected USB devices that are in bootsel mode get added to the firmware flashing list
            modulesForFlashing.AddRange(MobiFlightCache.FindConnectedUsbDevices());

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
                PerformFirmwareUpdateProcess(modulesForUpdate);
            }

            // this is only for non mobiflight boards
            if (Properties.Settings.Default.FwAutoUpdateCheck && modulesForFlashing.Count > 0)
            {
                PerformFirmwareInstallProcess(modulesForFlashing);
            }
        }

        private void PerformFirmwareInstallProcess(MobiFlightModuleInfo module)
        {
            PerformFirmwareInstallProcess(new List<MobiFlightModuleInfo>() { module });
        }
        private void PerformFirmwareInstallProcess(List<MobiFlightModuleInfo> modulesForFlashing)
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

        private void PerformFirmwareUpdateProcess(MobiFlightModule module)
        {
            PerformFirmwareUpdateProcess(new List<MobiFlightModule>() { module });
        }

        private void PerformFirmwareUpdateProcess(List<MobiFlightModule> modulesForUpdate)
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

        private DialogResult ShowSettingsDialog(String SelectedTab, MobiFlightModuleInfo SelectedBoard, List<MobiFlightModuleInfo> BoardsForFlashing, List<MobiFlightModule> BoardsForUpdate)
        {
            SettingsDialog dlg = new SettingsDialog(execManager);
            dlg.StartPosition = FormStartPosition.CenterParent;
            execManager.OnModuleConnected += dlg.UpdateConnectedModule;
            execManager.OnModuleRemoved += dlg.UpdateRemovedModule;

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

            SettingsDialogActive = true;
            var dialogResult = dlg.ShowDialog();
            execManager.OnModuleConnected -= dlg.UpdateConnectedModule;
            execManager.OnModuleRemoved -= dlg.UpdateRemovedModule;
            SettingsDialogActive = false;
            return dialogResult;
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
            AutoUpdateChecker.CheckForUpdate();
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
                execManager.SetPollInterval((int)e.NewValue);
            }

            if (e.SettingName == "CommunityFeedback")
            {
                AppTelemetry.Instance.Enabled = Properties.Settings.Default.CommunityFeedback;
            }

            if (e.SettingName == "LogEnabled")
            {
                logPanel1.Visible = (bool) e.NewValue;
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
                    LoadConfig(cmdLineParams.ConfigFile);
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
                    LoadConfig(file);
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

        private void Module_Connected(object sender, EventArgs e)
        {   
            if (InvokeRequired)
            {
                this.Invoke(new EventHandler(Module_Connected), new object[] { sender, e });
                return;
            }
            UpdateStatusBarModuleInformation();
            runTestToolStripButton.Enabled = TestRunIsAvailable();

            // During initial lookup we are showing the panel
            // and we would like to display some progress information
            if (!InitialLookupFinished)
            {
                startupPanel.UpdateStatusText("Scanning for boards.");
                var progress = startupPanel.GetProgressBar();
                var progressIncrement = (75 - progress) / 2;
                startupPanel.UpdateProgressBar(progress + progressIncrement);

                return;
            }

            var module = (sender as MobiFlightModule);
            if (module == null) return;

            // When we open the settings dialog
            // many of these module connected events
            // are on purpose because we are 
            // flashing & resetting modules
            // in such cases we don't want the auto-detect feature
            if (SettingsDialogActive) return;

            // This board is not flashed yet
            if (module.ToMobiFlightModuleInfo()?.FirmwareInstallPossible() ?? false)
            {
                PerformFirmwareInstallProcess(module.ToMobiFlightModuleInfo());
                return;
            } 

            // The board already has MF firmware
            if (!module.FirmwareRequiresUpdate()) return;


            PerformFirmwareUpdateProcess(module);
        }

        void Module_Removed(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new EventHandler(Module_Removed), new object[] { sender, e });
                return;
            }
            // _disconnectArcaze();
            UpdateStatusBarModuleInformation();

            // Todo: Show this error outside of the context of firmware update
            // _showError(string.Format(i18n._tr("uiMessageModuleRemoved"), (sender as MobiFlightModuleInfo)?.Name ?? "Unknown", (sender as MobiFlightModuleInfo)?.Port ?? "???"));
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void ExecManager_OnShutdown(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new EventHandler(ExecManager_OnShutdown), new object[] { sender, e });
                return;
            }
            UpdateStatusBarModuleInformation();
            ModuleStatusIconToolStripLabel.Image = Properties.Resources.warning;
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>
        void ModuleCache_Available(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new EventHandler(ModuleCache_Available), new object[] { sender, e });
                return;
            }
            UpdateStatusBarModuleInformation();
            runTestToolStripButton.Enabled = TestRunIsAvailable();
        }

        /// <summary>
        /// Returns true if the run button should be enabled based on various MobiFlight states.
        /// </summary>
        private bool RunIsAvailable()
        {
            return 
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
                UpdateSimConnectStatusIcon();
            }
            else if (sender.GetType() == typeof(XplaneCache))
            {
                UpdateXplaneDirectConnectStatusIcon();
            }
            else if (sender.GetType() == typeof(Fsuipc2Cache))
            {
                UpdateFsuipcStatusIcon();
            }

            UpdateSeparatorInStatusMenu();

            SimConnectionIconStatusToolStripStatusLabel.Image = Properties.Resources.warning;

            runToolStripButton.Enabled = RunIsAvailable();
        }

        private void ExecManager_OnSimAvailable(object sender, EventArgs e)
        {
            FlightSimType flightSim = (FlightSimType) sender;

            switch (flightSim)
            {
                case FlightSimType.MSFS2020:
                    SimProcessDetectedToolStripMenuItem.Text = "MSFS2020 Detected";
                    SimProcessDetectedToolStripMenuItem.Image = Properties.Resources.check;
                    break;

                case FlightSimType.FS9:
                    SimProcessDetectedToolStripMenuItem.Text = "FS2004 Detected";
                    SimProcessDetectedToolStripMenuItem.Image = Properties.Resources.check;
                    break;

                case FlightSimType.FSX:
                    SimProcessDetectedToolStripMenuItem.Text = "FSX Detected";
                    SimProcessDetectedToolStripMenuItem.Image = Properties.Resources.check;
                    break;

                case FlightSimType.P3D:
                    SimProcessDetectedToolStripMenuItem.Text = "P3D Detected";
                    SimProcessDetectedToolStripMenuItem.Image = Properties.Resources.check;
                    break;

                case FlightSimType.XPLANE:
                    SimProcessDetectedToolStripMenuItem.Text = "X-Plane Detected";
                    SimProcessDetectedToolStripMenuItem.Image = Properties.Resources.check;
                    break;

                case FlightSimType.UNKNOWN:
                    SimProcessDetectedToolStripMenuItem.Text = "Unkown Detected";
                    SimProcessDetectedToolStripMenuItem.Image = Properties.Resources.module_unknown;
                    break;

                default:
                    SimProcessDetectedToolStripMenuItem.Text = "Undefined";
                    break;
            }
            SimProcessDetectedToolStripMenuItem.Image = Properties.Resources.check;
        }

        private void ExecManager_OnSimUnavailable(object sender, EventArgs e)
        {
            FlightSimType flightSim = (FlightSimType)sender;

            SimProcessDetectedToolStripMenuItem.Text = "No sim running.";
            SimProcessDetectedToolStripMenuItem.Image = Properties.Resources.warning;

            UpdateAllConnectionIcons();
        }

        /// <summary>
        /// updates the UI with appropriate icon states
        /// </summary>        
        void fsuipcCache_Connected(object sender, EventArgs e)
        {
            // Typically the information in this static object is correct
            // only in the case of FSUIPC it might actually be not correct
            // because we can have a native connection and a fsuipc connection at the same time
            FlightSimConnectionMethod CurrentConnectionMethod = FlightSim.FlightSimConnectionMethod;
            FlightSimType CurrentFlightSimType = FlightSim.FlightSimType;

            if ((sender as CacheInterface).IsConnected())
            {
                SimConnectionIconStatusToolStripStatusLabel.Image = Properties.Resources.check;
                Log.Instance.log($"Connected to {FlightSim.SimNames[CurrentFlightSimType]}. [{FlightSim.SimConnectionNames[CurrentConnectionMethod]}].", LogSeverity.Info);
            }

            runToolStripButton.Enabled = RunIsAvailable();

            if (sender.GetType() == typeof(SimConnectCache) && FlightSim.FlightSimType == FlightSimType.MSFS2020)
            {
                SimProcessDetectedToolStripMenuItem.Text = "MSFS2020 Detected";

                if ((sender as SimConnectCache).IsSimConnectConnected())
                {
                    simConnectToolStripMenuItem.Text = "SimConnect OK. Waiting for WASM Module. (MSFS2020)";
                    Log.Instance.log("Connected to SimConnect (MSFS2020).", LogSeverity.Info);
                }

                if ((sender as SimConnectCache).IsConnected()) { 
                    simConnectToolStripMenuItem.Text = "WASM Module (MSFS2020)";
                    simConnectToolStripMenuItem.Image = Properties.Resources.check;
                    simConnectToolStripMenuItem.Enabled = true;
                    Log.Instance.log("Connected to WASM Module (MSFS2020).", LogSeverity.Info);

                    if (!execManager.GetFsuipcConnectCache().IsConnected())
                    {
                        UpdateFsuipcStatusIcon();
                    }
                }

                UpdateSimConnectStatusIcon();

                AppTelemetry.Instance.TrackFlightSimConnected(FlightSim.FlightSimType.ToString(), FlightSimConnectionMethod.SIMCONNECT.ToString());
                Log.Instance.log($"{FlightSim.SimNames[FlightSim.FlightSimType]} detected. [{FlightSim.SimConnectionNames[FlightSim.FlightSimConnectionMethod]}].", LogSeverity.Info);
            }
            else if (sender.GetType() == typeof(XplaneCache) && FlightSim.FlightSimType == FlightSimType.XPLANE)
            {
                SimProcessDetectedToolStripMenuItem.Text = "X-Plane Detected";
                if ((sender as XplaneCache).IsConnected())
                {
                    UpdateXplaneDirectConnectStatusIcon();
                    xPlaneDirectToolStripMenuItem.Text = FlightSim.SimConnectionNames[FlightSim.FlightSimConnectionMethod].ToString();
                    xPlaneDirectToolStripMenuItem.Image = Properties.Resources.check;
                    xPlaneDirectToolStripMenuItem.Enabled = true;
                }

                AppTelemetry.Instance.TrackFlightSimConnected(FlightSim.FlightSimType.ToString(), FlightSimConnectionMethod.XPLANE.ToString());
                Log.Instance.log($"{FlightSim.SimNames[FlightSim.FlightSimType]} detected. [{FlightSim.SimConnectionNames[FlightSim.FlightSimConnectionMethod]}].", LogSeverity.Info);
            }
            else if (sender.GetType() == typeof(Fsuipc2Cache)) { 

                Fsuipc2Cache c = sender as Fsuipc2Cache;
                switch (FlightSim.FlightSimConnectionMethod)
                {
                    case FlightSimConnectionMethod.FSUIPC:
                        CurrentConnectionMethod = FlightSimConnectionMethod.FSUIPC;
                        FsuipcToolStripMenuItem.Text = i18n._tr("fsuipcStatus") + " ("+ FlightSim.FlightSimType.ToString() +")";
                        break;

                    case FlightSimConnectionMethod.XPLANE:
                    case FlightSimConnectionMethod.XPUIPC:
                        CurrentConnectionMethod = FlightSimConnectionMethod.XPUIPC;
                        FsuipcToolStripMenuItem.Text = "XPUIPC Status";
                        break;

                    case FlightSimConnectionMethod.WIDECLIENT:
                        CurrentConnectionMethod = FlightSimConnectionMethod.WIDECLIENT;
                        FsuipcToolStripMenuItem.Text = "WideClient Status";
                        break;
                }
                FsuipcToolStripMenuItem.Image = Properties.Resources.check;
                FsuipcToolStripMenuItem.Image.Tag = "check";
                FsuipcToolStripMenuItem.Enabled = true;
                AppTelemetry.Instance.TrackFlightSimConnected(FlightSim.FlightSimType.ToString(), c.FlightSimConnectionMethod.ToString());
                Log.Instance.log($"{FlightSim.SimNames[FlightSim.FlightSimType]} detected. [{FlightSim.SimConnectionNames[CurrentConnectionMethod]}].", LogSeverity.Info
                );
            }

            UpdateSeparatorInStatusMenu();
        }

        /// <summary>
        /// gets triggered as soon as the fsuipc is connected
        /// </summary>        
        void checkAutoRun (object sender, EventArgs e)
        {            
            if (Properties.Settings.Default.AutoRun || cmdLineParams.AutoRun)
            {
                execManager.Start();
                if (Properties.Settings.Default.MinimizeOnAutoRun)
                {
                    minimizeMainForm(true);
                }
            }
        }

        /// <summary>
        /// shows message to user and stops execution of timer
        /// </summary>
        void fsuipcCache_ConnectionLost(object sender, EventArgs e)
        {
            execManager.Stop();

            if (!execManager.SimAvailable())
            {
                _showError(i18n._tr("uiMessageFsHasBeenStopped"));
                UpdateAllConnectionIcons();
                return;
            }

            if (sender.GetType() == typeof(SimConnectCache))
            {
                _showError(i18n._tr("uiMessageSimConnectConnectionLost"));
                UpdateSimConnectStatusIcon();
            }
            else if(sender.GetType() == typeof(XplaneCache))
            {
                _showError(i18n._tr("uiMessageXplaneConnectionLost"));
                UpdateXplaneDirectConnectStatusIcon();
            }
            else
            {
                _showError(i18n._tr("uiMessageFsuipcConnectionLost"));
                if (execManager.GetSimConnectCache().IsConnected())
                UpdateFsuipcStatusIcon();
            }

            UpdateSeparatorInStatusMenu();
        }

        /// <summary>
        /// handler which sets the states of UI elements when timer gets started
        /// </summary>
        void ExecManager_Started(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(ExecManager_Started), new object[] { sender, e });
                return;
            }

            runToolStripButton.Enabled  = RunIsAvailable();
            runTestToolStripButton.Enabled = TestRunIsAvailable();
            stopToolStripButton.Enabled = true;
            updateNotifyContextMenu(execManager.IsStarted());
        } //timer_Started()

        /// <summary>
        /// handler which sets the states of UI elements when timer gets stopped
        /// </summary>
        void ExecManager_Stopped(object sender, EventArgs e)
        {
            if(InvokeRequired)
            {
                Invoke(new EventHandler(ExecManager_Stopped), new object[] { sender, e});
                return;
            }

            runToolStripButton.Enabled = RunIsAvailable();
            runTestToolStripButton.Enabled = TestRunIsAvailable();
            stopToolStripButton.Enabled = false;
            updateNotifyContextMenu(execManager.IsStarted());
        } //timer_Stopped

        private bool TestRunIsAvailable()
        {
            return execManager.ModulesAvailable() && !execManager.TestModeIsStarted() && !execManager.IsStarted();
        }

        /// <summary>
        /// Timer eventhandler
        /// </summary>        
        void ExecManager_Executed(object sender, EventArgs e)
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
        private bool UpdateStatusBarModuleInformation()
        {
            // remove the items from all comboboxes
            // and set default items
            bool modulesFound = false;
            ModuleStatusIconToolStripLabel.Image = Properties.Resources.warning;
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
                ModuleStatusIconToolStripLabel.Image = Properties.Resources.check;
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
            catch (Exception ex)
            {
                // do nothing
                Log.Instance.log(ex.Message, LogSeverity.Info);
            }
        }

        /// <summary>
        /// present errors to user via message dialog or when minimized via balloon
        /// </summary>        
        private void _showError (string msg)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                TimeoutMessageDialog.Show(msg, i18n._tr("Hint"), MessageBoxButtons.OK, MessageBoxIcon.Warning); 
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

            if (saveToolStripButton.Enabled && MessageBox.Show(
                       i18n._tr("uiMessageConfirmDiscardUnsaved"),
                       i18n._tr("uiMessageConfirmDiscardUnsavedTitle"),
                       MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                saveToolStripButton_Click(saveToolStripButton, new EventArgs());
            }

            if (DialogResult.OK == fd.ShowDialog())
            {
                LoadConfig(fd.FileName);
            }
        }

        private void mergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "MobiFlight Connector Config (*.mcc)|*.mcc|ArcazeUSB Interface Config (*.aic) |*.aic";

            if (DialogResult.OK == fd.ShowDialog())
            {
                LoadConfig(fd.FileName, true);
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
            if (saveToolStripButton.Enabled && MessageBox.Show(
                       i18n._tr("uiMessageConfirmDiscardUnsaved"),
                       i18n._tr("uiMessageConfirmDiscardUnsavedTitle"),
                       MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                saveToolStripButton_Click(saveToolStripButton, new EventArgs());
            };
            LoadConfig((sender as ToolStripMenuItem).Text);            
        } //recentMenuItem_Click()

        /// <summary>
        /// loads the according config given by filename
        /// </summary>        
        private void LoadConfig(string fileName, bool merge = false)
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

                SaveFileDialog fd = new SaveFileDialog
                {
                    FileName = fileName.Replace(".aic", ".mcc"),
                    Filter = "MobiFlight Connector Config (*.mcc)|*.mcc"
                };
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
                    SaveFileDialog fd = new SaveFileDialog
                    {
                        FileName = fileName.Replace(".mcc", "_v6.0.mcc")
                    };

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
                Log.Instance.log($"Unable to load configuration file: {ex.Message}", LogSeverity.Error);
                MessageBox.Show(i18n._tr("uiMessageProblemLoadingConfig"), i18n._tr("Hint"));
                return;
            }

            try
            {
                // refactor!!!
                inputConfigPanel.InputDataSetConfig.ReadXml(configFile.getInputConfig());
            }
            catch (InvalidExpressionException)
            {
                // no inputs configured... old format... just ignore
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Unable to load configuration file: {ex.Message}", LogSeverity.Error);
                MessageBox.Show(i18n._tr("uiMessageProblemLoadingConfig"), i18n._tr("Hint"));
                return;
            }


            // for backward compatibility 
            // we check if there are rows that need to
            // initialize our config item correctly
            _applyBackwardCompatibilityLoading();
            _restoreValuesInGridView();

            if (!merge)
            {
                CurrentFileName = fileName;
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
            _checkForOrphanedMidiBoards(false);

            // Track config loaded event
            AppTelemetry.Instance.ConfigLoaded(configFile);
            AppTelemetry.Instance.TrackBoardStatistics(execManager);
            AppTelemetry.Instance.TrackSettings();

            ConfigLoaded?.Invoke(this, configFile);
        }

        private void _checkForOrphanedJoysticks(bool showNotNecessaryMessage)
        {
            List<string> serials = new List<string>();
            List<string> NotConnectedJoysticks = new List<string>();

            foreach (Joystick j in execManager.GetJoystickManager().GetJoysticks())
            {
                serials.Add($"{j.Name} {SerialNumber.SerialSeparator}{j.Serial}");
            }

            if (configFile == null) return;

            foreach (OutputConfigItem item in configFile.GetOutputConfigItems())
            {
                if (item.DisplaySerial.Contains(Joystick.SerialPrefix) &&
                    !serials.Contains(item.DisplaySerial) &&
                    !NotConnectedJoysticks.Contains(item.DisplaySerial))
                {
                    NotConnectedJoysticks.Add(item.DisplaySerial);
                }
            }

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
                TimeoutMessageDialog.Show(i18n._tr("uiMessageNoNotConnectedJoysticksInConfigFound"), i18n._tr("Hint"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void _checkForOrphanedMidiBoards(bool showNotNecessaryMessage)
        {
            List<string> serials = new List<string>();
            List<string> NotConnectedMidiBoards = new List<string>();

            foreach (MidiBoard mb in execManager.GetMidiBoardManager().GetMidiBoards())
            {
                serials.Add($"{mb.Name} {SerialNumber.SerialSeparator}{mb.Serial}");        
            }

            if (configFile == null) return;

            foreach (OutputConfigItem item in configFile.GetOutputConfigItems())
            {
                if (item.DisplaySerial.Contains(MidiBoard.SerialPrefix) &&
                    !serials.Contains(item.DisplaySerial) &&
                    !NotConnectedMidiBoards.Contains(item.DisplaySerial))
                {
                    NotConnectedMidiBoards.Add(item.DisplaySerial);
                }
            }

            foreach (InputConfigItem item in configFile.GetInputConfigItems())
            {
                if (item.ModuleSerial.Contains(MidiBoard.SerialPrefix) &&
                    !serials.Contains(item.ModuleSerial) &&
                    !NotConnectedMidiBoards.Contains(item.ModuleSerial))
                {
                    NotConnectedMidiBoards.Add(item.ModuleSerial);
                }
            }

            if (NotConnectedMidiBoards.Count > 0)
            {
                TimeoutMessageDialog tmd = new TimeoutMessageDialog();
                tmd.HasCancelButton = false;
                tmd.StartPosition = FormStartPosition.CenterParent;
                tmd.Message = string.Format(
                                    i18n._tr("uiMessageNotConnectedMidiBoardsInConfigFound"),
                                    string.Join("\n", NotConnectedMidiBoards)
                                    );
                tmd.Text = i18n._tr("Hint");
                tmd.ShowDialog();
            }
            else if (showNotNecessaryMessage)
            {
                TimeoutMessageDialog.Show(i18n._tr("uiMessageNoNotConnectedMidiBoardsInConfigFound"), i18n._tr("Hint"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                serials.Add($"{moduleInfo.Name}{SerialNumber.SerialSeparator}{moduleInfo.Serial}");
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
                    TimeoutMessageDialog.Show(i18n._tr("uiMessageNoOrphanedSerialsFound"), i18n._tr("Hint"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) {
                // do nothing
                Log.Instance.log($"Orphaned serials exception: {ex.Message}", LogSeverity.Error);
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

        public static String CurrentVersion()
        {
            if (VersionBeta.Split('.')[3] != "0")
                return VersionBeta;

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
                        cfgItem.Modifiers.Comparison.Active = true;
                        cfgItem.Modifiers.Comparison.Operand = row["comparison"].ToString();
                    }

                    if (row["comparisonValue"].GetType() != typeof(System.DBNull))
                    {
                        cfgItem.Modifiers.Comparison.Value = row["comparisonValue"].ToString();
                    }

                    if (row["converter"].GetType() != typeof(System.DBNull))
                    {
                        if (row["converter"].ToString() == "Boolean")
                        {
                            cfgItem.Modifiers.Comparison.IfValue = "1";
                            cfgItem.Modifiers.Comparison.ElseValue = "0";
                        }
                    }

                    if (row["trigger"].GetType() != typeof(System.DBNull))
                    {
                        cfgItem.DisplayTrigger = row["trigger"].ToString();
                    }

                    if (row["usbArcazePin"].GetType() != typeof(System.DBNull))
                    {
                        cfgItem.DisplayType = MobiFlightOutput.TYPE;
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
            CurrentFileName = fileName;
            _restoreValuesInGridView();
            _storeAsRecentFile(fileName);
            _setFilenameInTitle(fileName);
            saveToolStripButton.Enabled = false;
        }

        private void UpdateSimConnectStatusIcon()
        {
            simConnectToolStripMenuItem.Image = Properties.Resources.warning;
            simConnectToolStripMenuItem.Visible = true;
            simConnectToolStripMenuItem.Enabled = true;
            simConnectToolStripMenuItem.ToolTipText = "Some configs are using MSFS2020 presets -> WASM module required";

            if (!ContainsConfigOfSourceType(outputConfigPanel.GetConfigItems(), inputConfigPanel.GetConfigItems(), SourceType.SIMCONNECT))
            {
                simConnectToolStripMenuItem.Image = Properties.Resources.disabled;
                simConnectToolStripMenuItem.Visible = false;
                simConnectToolStripMenuItem.Enabled = false;
                UpdateSeparatorInStatusMenu();
                return;
            }

            if (execManager.GetSimConnectCache().IsConnected())
                simConnectToolStripMenuItem.Image = Properties.Resources.check;
            else 
                SimConnectionIconStatusToolStripStatusLabel.Image = Properties.Resources.warning;

            UpdateSeparatorInStatusMenu();
        }

        private void UpdateSeparatorInStatusMenu()
        {
            separatorToolStripMenuItem.Visible = simConnectToolStripMenuItem.Enabled || xPlaneDirectToolStripMenuItem.Enabled|| FsuipcToolStripMenuItem.Enabled;
        }

        private void UpdateXplaneDirectConnectStatusIcon()
        {
            xPlaneDirectToolStripMenuItem.Image = Properties.Resources.warning;
            xPlaneDirectToolStripMenuItem.Visible = true;
            xPlaneDirectToolStripMenuItem.Enabled = true;
            xPlaneDirectToolStripMenuItem.ToolTipText = "Some configs are using XPlane DataRefs/Commands -> XPlane direct required";

            if (!ContainsConfigOfSourceType(outputConfigPanel.GetConfigItems(), inputConfigPanel.GetConfigItems(), SourceType.XPLANE))
            {
                xPlaneDirectToolStripMenuItem.Image = Properties.Resources.disabled;
                xPlaneDirectToolStripMenuItem.Visible = false;
                xPlaneDirectToolStripMenuItem.Enabled = false;
                UpdateSeparatorInStatusMenu();
                return;
            }

            if (execManager.GetXlpaneConnectCache().IsConnected())
                xPlaneDirectToolStripMenuItem.Image = Properties.Resources.check;
            else 
                SimConnectionIconStatusToolStripStatusLabel.Image = Properties.Resources.warning;

            UpdateSeparatorInStatusMenu();
        }

        private void UpdateFsuipcStatusIcon()
        {
            FsuipcToolStripMenuItem.Image = Properties.Resources.warning;
            FsuipcToolStripMenuItem.Visible = true;
            FsuipcToolStripMenuItem.Enabled = true;
            FsuipcToolStripMenuItem.ToolTipText = "Some configs are using FSUIPC -> FSUIPC required";

            if (!ContainsConfigOfSourceType(outputConfigPanel.GetConfigItems(), inputConfigPanel.GetConfigItems(), SourceType.FSUIPC))
            {
                FsuipcToolStripMenuItem.Image = Properties.Resources.disabled;
                FsuipcToolStripMenuItem.Visible = false;
                FsuipcToolStripMenuItem.Enabled = false;
                UpdateSeparatorInStatusMenu();
                return;
            }

            if (execManager.GetFsuipcConnectCache().IsConnected())
                FsuipcToolStripMenuItem.Image = Properties.Resources.check;
            else
                SimConnectionIconStatusToolStripStatusLabel.Image = Properties.Resources.warning;

            UpdateSeparatorInStatusMenu();
        }
        private void UpdateSimStatusIcon()
        {
            if (execManager.SimConnected())
            {
                SimConnectionIconStatusToolStripStatusLabel.Image = Properties.Resources.check;
            }
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
                CurrentFileName = null;
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
            if (CurrentFileName != null)
            {
                _saveConfig(CurrentFileName);
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
                // which is indicated by empty CurrentFilename
                e.Cancel = (CurrentFileName == null);
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

            } catch (Exception ex) {
                Log.Instance.log(ex.Message, LogSeverity.Error);
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
            ProgressForm progressForm = new ProgressForm();
            Control MainForm = this;

            updater.DownloadAndInstallProgress += progressForm.OnProgressUpdated;
            var t = new Task(() => {
                    if (!updater.AutoDetectCommunityFolder())
                    {
                        Log.Instance.log(i18n._tr("uiMessageWasmUpdateCommunityFolderNotFound"), LogSeverity.Error);
                        return;
                    }

                    if (updater.InstallWasmEvents())
                    {
                        progressForm.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        progressForm.DialogResult = DialogResult.No;
                        Log.Instance.log(i18n._tr("uiMessageWasmEventsInstallationError"), LogSeverity.Error);
                    }
                }
            );

            t.Start();
            if (progressForm.ShowDialog() == DialogResult.OK)
            {
                TimeoutMessageDialog.Show(
                   i18n._tr("uiMessageWasmEventsInstallationSuccessful"),
                   i18n._tr("uiMessageWasmUpdater"),
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else
            {
                TimeoutMessageDialog.Show(
                    i18n._tr("uiMessageWasmEventsInstallationError"),
                    i18n._tr("uiMessageWasmUpdater"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            progressForm.Dispose();
        }

        private void downloadHubHopPresetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WasmModuleUpdater updater = new WasmModuleUpdater();
            ProgressForm progressForm = new ProgressForm();
            Control MainForm = this;

            updater.DownloadAndInstallProgress += progressForm.OnProgressUpdated;
            var t = new Task(() => {
                if (updater.DownloadHubHopPresets())
                {
                    Msfs2020HubhopPresetListSingleton.Instance.Clear();
                    XplaneHubhopPresetListSingleton.Instance.Clear();
                    progressForm.DialogResult = DialogResult.OK;
                }
                else
                {
                    progressForm.DialogResult = DialogResult.No;
                    Log.Instance.log(i18n._tr("uiMessageHubHopUpdateError"), LogSeverity.Error);
                }
            }
            );

            t.Start();
            if (progressForm.ShowDialog() == DialogResult.OK)
            {
                TimeoutMessageDialog.Show(
                   i18n._tr("uiMessageHubHopUpdateSuccessful"),
                   i18n._tr("uiMessageWasmUpdater"),
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                TimeoutMessageDialog.Show(
                    i18n._tr("uiMessageWasmEventsInstallationError"),
                    i18n._tr("uiMessageWasmUpdater"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            progressForm.Dispose();
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

        private void HubHopToolStripButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://hubhop.mobiflight.com/");
        }

        private void releaseNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start($"https://github.com/MobiFlight/MobiFlight-Connector/releases/tag/{CurrentVersion()}");
        }

        private void inputsTabControl_TabIndexChanged(object sender, EventArgs e)
        {
            if (inputsTabControl.SelectedIndex == 0)
            {
                OutputTabPage.ImageKey = "mf-output.png";
                InputTabPage.ImageKey = "mf-input-inactive.png";
            } else
            {
                OutputTabPage.ImageKey = "mf-output-inactive.png";
                InputTabPage.ImageKey = "mf-input.png";
            }
        }

        public static bool ContainsConfigOfSourceType(List<OutputConfigItem> outputConfigItems, List<InputConfigItem> inputConfigItems, SourceType type)
        {
            var result = false;
            if (type == SourceType.SIMCONNECT)
            {
                result = outputConfigItems
                        .Any(x => x?.SourceType == type) ||
                         inputConfigItems
                        .Any(x => x?.GetInputActionsByType(typeof(MSFS2020CustomInputAction)).Count > 0);
            }
            else if (type == SourceType.FSUIPC)
            {
                result = outputConfigItems
                        .Any(x => x?.SourceType == type) ||
                         inputConfigItems
                        .Any(x => x?.GetInputActionsByType(typeof(FsuipcOffsetInputAction)).Count > 0 ||
                                  x?.GetInputActionsByType(typeof(EventIdInputAction)).Count > 0 ||
                                  x?.GetInputActionsByType(typeof(PmdgEventIdInputAction)).Count > 0 ||
                                  x?.GetInputActionsByType(typeof(JeehellInputAction)).Count > 0 ||
                                  x?.GetInputActionsByType(typeof(LuaMacroInputAction)).Count > 0);
            }
            else if (type == SourceType.XPLANE)
            {
                result = outputConfigItems
                        .Any(x => x?.SourceType == type) ||
                         inputConfigItems
                        .Any(x => x?.GetInputActionsByType(typeof(XplaneInputAction)).Count > 0);
            }
            else if (type == SourceType.VARIABLE)
            {
                result = outputConfigItems
                        .Any(x => x?.SourceType == type) ||
                         inputConfigItems
                        .Any(x => x?.GetInputActionsByType(typeof(VariableInputAction)).Count > 0);
            }
            return result;
        }


        private void RestoreAutoLoadConfig()
        {
            AutoLoadConfigs = JsonConvert.DeserializeObject<Dictionary<string, string>>(Properties.Settings.Default.AutoLoadLinkedConfigList);
            if (AutoLoadConfigs == null)
                AutoLoadConfigs = new Dictionary<string, string>();
            ;
        }

        private void SaveAutoLoadConfig()
        {
            Properties.Settings.Default.AutoLoadLinkedConfigList = JsonConvert.SerializeObject(AutoLoadConfigs);
            Properties.Settings.Default.Save();
            UpdateAutoLoadMenu();
        }

        private void UpdateAutoLoadConfig()
        {
            autoloadToggleToolStripMenuItem.Checked = Properties.Settings.Default.AutoLoadLinkedConfig;
        }

        private void autoloadToggleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoLoadLinkedConfig = !Properties.Settings.Default.AutoLoadLinkedConfig;
            UpdateAutoLoadConfig();
        }

        private void linkCurrentConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aircraftName = toolStripAircraftDropDownButton.Text ?? string.Empty;
            var key = $"{FlightSim.FlightSimType}:{aircraftName}";

            AutoLoadConfigs[key] = CurrentFileName;

            SaveAutoLoadConfig();
        }

        private void unlinkConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aircraftName = toolStripAircraftDropDownButton.Text ?? string.Empty;
            var key = $"{FlightSim.FlightSimType}:{aircraftName}";
            toolStripAircraftDropDownButton.Image = null;

            if (!AutoLoadConfigs.Remove(key)) return;
            
            SaveAutoLoadConfig();
        }

        private void UpdateAutoLoadMenu()
        {
            var aircraftName = toolStripAircraftDropDownButton.Text;
            var key = $"{FlightSim.FlightSimType}:{aircraftName}";

            toolStripAircraftDropDownButton.Image = null;

            linkCurrentConfigToolStripMenuItem.Enabled = (CurrentFileName != null);
            openLinkedConfigToolStripMenuItem.Enabled = false;
            removeLinkConfigToolStripMenuItem.Enabled = false;

            if (!AutoLoadConfigs.ContainsKey(key)) return;

            var linkedFile = AutoLoadConfigs[key];

            removeLinkConfigToolStripMenuItem.Enabled = true;
            openLinkedConfigToolStripMenuItem.Enabled = true;
            openLinkFilenameToolStripMenuItem.Text = linkedFile;
            toolStripAircraftDropDownButton.Image = Properties.Resources.warning;

            if (linkedFile != CurrentFileName) return;

            linkCurrentConfigToolStripMenuItem.Enabled = false;
            openLinkedConfigToolStripMenuItem.Enabled = false;
            toolStripAircraftDropDownButton.Image = Properties.Resources.check;
        }

        private void openLinkedConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aircraftName = toolStripAircraftDropDownButton.Text;
            var key = $"{FlightSim.FlightSimType}:{aircraftName}";
            if (!AutoLoadConfigs.ContainsKey(key)) return;

            var linkedFile = AutoLoadConfigs[key];

            if (saveToolStripButton.Enabled && MessageBox.Show(
                       i18n._tr("uiMessageConfirmDiscardUnsaved"),
                       i18n._tr("uiMessageConfirmDiscardUnsavedTitle"),
                       MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                saveToolStripButton_Click(saveToolStripButton, new EventArgs());
            };

            LoadConfig(linkedFile);
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
