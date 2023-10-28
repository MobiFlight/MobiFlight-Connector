using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
//using SimpleSolutions.Usb;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MobiFlight.UI.Forms;
using MobiFlight.UI.Panels.Settings;
using MobiFlight.Base;
using Microsoft.ApplicationInsights.DataContracts;

namespace MobiFlight.UI.Dialogs
{
    public partial class SettingsDialog : Form
    {

        ExecutionManager execManager;
        int lastSelectedIndex = -1;

        public List<MobiFlightModule> MobiFlightModulesForUpdate {
            get { return mobiFlightPanel.modulesForUpdate; } 
            set { mobiFlightPanel.modulesForUpdate = value; }
        }
        public List<MobiFlightModuleInfo> MobiFlightModulesForFlashing
        {
            get { return mobiFlightPanel.modulesForFlashing; }
            set { mobiFlightPanel.modulesForFlashing = value; }
        }

        public MobiFlightModuleInfo PreselectedBoard
        {
            get { return mobiFlightPanel.PreselectedMobiFlightBoard; }
            set { mobiFlightPanel.PreselectedMobiFlightBoard = value; }
        }

        public SettingsDialog()
        {
            Init();
        }

        public SettingsDialog(ExecutionManager execManager)
        {
            this.execManager = execManager;
            Init();
        }

        private void Init()
        {
            InitializeComponent();
            // init Arcaze Tab Panel
#if ARCAZE
            arcazePanel.Init(execManager.getModuleCache());
#endif

#if !ARCAZE
            tabControl1.TabPages.Remove(ArcazeTabPage);
#endif

#if MOBIFLIGHT
            mobiFlightPanel.Init(execManager.getMobiFlightModuleCache());
            
            mobiFlightPanel.OnBeforeFirmwareUpdate += (object sender, EventArgs e) => { 
                execManager.AutoConnectStop();
                execManager.getMobiFlightModuleCache().PauseModuleScan();
            };

            mobiFlightPanel.OnAfterFirmwareUpdate += (object sender, EventArgs e) => {
                execManager.getMobiFlightModuleCache().ResumeModuleScan();
                execManager.AutoConnectStart(); 
            };

            mobiFlightPanel.OnModuleConfigChanged += (object sender, EventArgs e) => { ; };
#endif

#if !MOBIFLIGHT
            tabControl1.TabPages.Remove(mobiFlightTabPage);
#endif
            loadSettings();
        }

        /// <summary>
        /// Load all settings for each tab
        /// </summary>
        private void loadSettings ()
        {
            //
            // TAB General
            //
            generalPanel.loadSettings();

            //
            // TAB Arcaze
            //
#if ARCAZE
            arcazePanel.LoadSettings();
#endif
            //
            // TAB MobiFlight
            //
            mobiFlightPanel.LoadSettings();
        }

        /// <summary>
        /// Save the settings from tabs in Properties.Settings
        /// This does not apply to MF modules
        /// </summary>
        private void saveSettings()
        {
            // General Tab
            generalPanel.saveSettings();
#if ARCAZE
            // Arcaze Tab
            arcazePanel.SaveSettings();
#endif
            // MobiFlight Tab
            mobiFlightPanel.SaveSettings();

            // Save all Settings
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Callback for OK Button, used to close the form and save changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
            {
                return;
            }

            if (mobiFlightPanel.MFModuleConfigChanged)
            {
                if (MessageBox.Show(i18n._tr("MFModuleConfigChanged"),
                                    i18n._tr("Hint"),
                                    MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
                {
                    tabControl1.SelectedTab = mobiFlightTabPage;
                    return;
                }
            }

            DialogResult = DialogResult.OK;

            saveSettings();
        }

        

        /// <summary>
        /// Validate settings, e.g. ensure that every Arcaze has been configured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ledDisplaysTabPage_Validating(object sender, CancelEventArgs e)
        {
            // check that for all available arcaze serials there is an entry in module settings
        }

        /// <summary>
        /// Callback for cancel button - discard changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (mobiFlightPanel.MFModuleConfigChanged)
            {
                if (MessageBox.Show(i18n._tr("MFModuleConfigChanged"),
                                    i18n._tr("Hint"),
                                    MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
                {
                    DialogResult = DialogResult.None;
                    tabControl1.SelectedTab = mobiFlightTabPage;
                    return;
                }
            }

            DialogResult = DialogResult.Cancel;
        }

        private void SettingsDialog_Shown(object sender, EventArgs e)
        {
            TabPage current = tabControl1.SelectedTab;
        }

        private void SettingsDialog_Load(object sender, EventArgs e)
        {
            TabPage current = tabControl1.SelectedTab;
        }

        internal void UpdateConnectedModule(object sender, EventArgs e)
        {
            if (!(sender is MobiFlightModule)) return;

            var info = (sender as MobiFlightModule).ToMobiFlightModuleInfo();
            mobiFlightPanel.UpdateNewlyConnectedModule(info);
        }

        internal void UpdateRemovedModule(object sender, EventArgs e)
        {
            mobiFlightPanel.UpdateRemovedModule(sender as MobiFlightModuleInfo);
        }
    }
}
