using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class GeneralPanel : UserControl
    {
        public GeneralPanel()
        {
            InitializeComponent();
        }

        public void loadSettings()
        {
            languageComboBox.Items.Clear();

            List<ListItem> languageOptions = new List<ListItem>();
            languageOptions.Add(new ListItem() { Value = "", Label = "System Default" });
            languageOptions.Add(new ListItem() { Value = "en-US", Label = "English" });
            languageOptions.Add(new ListItem() { Value = "de-DE", Label = "Deutsch" });

            languageComboBox.DataSource = languageOptions;
            languageComboBox.DisplayMember = "Label";
            languageComboBox.ValueMember = "Value";
            languageComboBox.SelectedIndex = 0;

            //
            // TAB General
            //
            // Recent Files max count
            recentFilesNumericUpDown.Value = Properties.Settings.Default.RecentFilesMaxCount;

            // TestMode speed
            // (1s) 0 - 4 (50ms)
            testModeSpeedTrackBar.Value = 0;
            if (Properties.Settings.Default.TestTimerInterval == 500) testModeSpeedTrackBar.Value = 1;
            else if (Properties.Settings.Default.TestTimerInterval == 250) testModeSpeedTrackBar.Value = 2;
            else if (Properties.Settings.Default.TestTimerInterval == 125) testModeSpeedTrackBar.Value = 3;
            else if (Properties.Settings.Default.TestTimerInterval == 50) testModeSpeedTrackBar.Value = 4;

            // Debug Mode
            logLevelCheckBox.Checked = Properties.Settings.Default.LogEnabled;
            ComboBoxHelper.SetSelectedItem(logLevelComboBox, Properties.Settings.Default.LogLevel);

            // Offline Mode
            offlineModeCheckBox.Checked = Properties.Settings.Default.OfflineMode;

            // System Language
            languageComboBox.SelectedValue = Properties.Settings.Default.Language;

            // Beta Versions
            BetaUpdateCheckBox.Checked = Properties.Settings.Default.BetaUpdates;

            // Community Feedback Program
            CommunityFeedbackCheckBox.Checked = Properties.Settings.Default.CommunityFeedback;
        }

        public void saveSettings()
        {
            if (testModeSpeedTrackBar.Value == 0) Properties.Settings.Default.TestTimerInterval = 1000;
            else if (testModeSpeedTrackBar.Value == 1) Properties.Settings.Default.TestTimerInterval = 500;
            else if (testModeSpeedTrackBar.Value == 2) Properties.Settings.Default.TestTimerInterval = 250;
            else if (testModeSpeedTrackBar.Value == 3) Properties.Settings.Default.TestTimerInterval = 125;
            else Properties.Settings.Default.TestTimerInterval = 50;

            // Recent Files max count
            Properties.Settings.Default.RecentFilesMaxCount = (int)recentFilesNumericUpDown.Value;

            // log settings
            Properties.Settings.Default.LogEnabled = logLevelCheckBox.Checked;
            Properties.Settings.Default.LogLevel = logLevelComboBox.SelectedItem as String;
            Log.Instance.Enabled = logLevelCheckBox.Checked;
            Log.Instance.Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), Properties.Settings.Default.LogLevel);

            // Offline Mode
            Properties.Settings.Default.OfflineMode = offlineModeCheckBox.Checked;

            // System Language
            Properties.Settings.Default.Language = languageComboBox.SelectedValue.ToString();

            // Beta Versions
            Properties.Settings.Default.BetaUpdates = BetaUpdateCheckBox.Checked;

            // Community Feedback Program
            Properties.Settings.Default.CommunityFeedback = CommunityFeedbackCheckBox.Checked;
        }
    }
}
