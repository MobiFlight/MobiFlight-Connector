﻿using System;
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
            mobiFlightPanel.OnBeforeFirmwareUpdate += (object sender, EventArgs e) => { execManager.AutoConnectStop(); };
            mobiFlightPanel.OnAfterFirmwareUpdate += (object sender, EventArgs e) => { execManager.AutoConnectStart(); };
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

            //
            // TAB FSUIPC
            //
            // FSUIPC poll interval
            fsuipcPollIntervalTrackBar.Value = (int)Math.Floor(Properties.Settings.Default.PollInterval / 50.0);
        }

        /// <summary>
        /// Save the settings from tabs in Properties.Settings
        /// This does not apply to MF modules
        /// </summary>
        private void saveSettings()
        {
            // General Tab
            generalPanel.saveSettings();

            // Arcaze Tab
#if ARCAZE
            arcazePanel.SaveSettings();
#endif
            // MobiFlight Tab
            mobiFlightPanel.SaveSettings();

            // FSUIPC poll interval
            Properties.Settings.Default.PollInterval = (int)(fsuipcPollIntervalTrackBar.Value * 50);
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
    }
}
