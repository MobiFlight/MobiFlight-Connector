using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MobiFlight.UI.Dialogs
{
    public partial class OrphanedSerialsDialog : Form
    {
        List<string> moduleSerials = null;
        List<OutputConfigItem> configDataTable = null;
        List<OutputConfigItem> realConfigDataTable = null;
        List<InputConfigItem> inputDataTable = null;
        List<InputConfigItem> realInputDataTable = null;
        bool changed = false;

        public OrphanedSerialsDialog(List<string> serials, List<OutputConfigItem> outputConfigItems, List<InputConfigItem> inputConfigItems)
        {
            this.moduleSerials = serials;
            this.configDataTable = outputConfigItems.ToArray().ToList();
            this.realConfigDataTable = outputConfigItems;
            this.inputDataTable = inputConfigItems.ToArray().ToList();
            this.realInputDataTable = inputConfigItems;
            InitializeComponent();
            updateOrphanedList();
        }

        protected void updateOrphanedList()
        {
            List<String> configSerials = new List<string>();

            connectedModulesComboBox.Items.Clear();
            orphanedSerialsListBox.Items.Clear();

            foreach (String serial in moduleSerials)
            {
                connectedModulesComboBox.Items.Add(serial);
            }

            foreach (var cfg in configDataTable)
            {
                if (cfg.DisplaySerial != "" &&
                    cfg.DisplaySerial != "-" &&
                    !Joystick.IsJoystickSerial(cfg.DisplaySerial) &&
                    !MidiBoard.IsMidiBoardSerial(cfg.DisplaySerial) &&
                    !configSerials.Contains(cfg.DisplaySerial) &&
                    !moduleSerials.Contains(cfg.DisplaySerial))
                {
                    configSerials.Add(cfg.DisplaySerial);
                    orphanedSerialsListBox.Items.Add(cfg.DisplaySerial);
                }
            }

            foreach (var cfg in inputDataTable)
            {
                if (cfg != null &&
                    cfg.ModuleSerial != "" &&
                    cfg.ModuleSerial != "-" &&
                    !Joystick.IsJoystickSerial(cfg.ModuleSerial) &&
                    !MidiBoard.IsMidiBoardSerial(cfg.ModuleSerial) &&
                    !configSerials.Contains(cfg.ModuleSerial) &&
                    !moduleSerials.Contains(cfg.ModuleSerial))
                {
                    configSerials.Add(cfg.ModuleSerial);
                    orphanedSerialsListBox.Items.Add(cfg.ModuleSerial);
                }
            }

            if (connectedModulesComboBox.Items.Count > 0) connectedModulesComboBox.SelectedIndex = 0;
        }

        protected void replaceSerialBySerial(String oldSerial, String newSerial)
        {
            foreach (var cfg in configDataTable)
            {
                if (cfg?.DisplaySerial == oldSerial)
                {
                    cfg.DisplaySerial = newSerial;
                }
            }

            foreach (var cfg in inputDataTable)
            {
                if (cfg?.ModuleSerial == oldSerial)
                {
                    cfg.ModuleSerial = newSerial;
                }
            }
        }

        public bool HasOrphanedSerials()
        {
            return (orphanedSerialsListBox.Items.Count > 0);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i != realConfigDataTable.Count; ++i)
            {
                realConfigDataTable[i] = configDataTable[i];
            }

            for (int i = 0; i != realInputDataTable.Count; ++i)
            {
                realInputDataTable[i] = inputDataTable[i];
            }
            //realConfigDataTable = configDataTable;
            DialogResult = DialogResult.OK;
        }

        private void orphanedSerialsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            connectedModulesAssignButton.Enabled = (orphanedSerialsListBox.Items.Count > 0);
        }

        private void connectedModulesAssignButton_Click(object sender, EventArgs e)
        {
            if (orphanedSerialsListBox.SelectedItem == null || connectedModulesComboBox.SelectedItem == null) return;

            replaceSerialBySerial(orphanedSerialsListBox.SelectedItem.ToString(),
                                  connectedModulesComboBox.SelectedItem.ToString());
            changed = true;
            updateOrphanedList();
            (sender as Button).Enabled = false;
        }

        public bool HasChanged()
        {
            return changed;
        }
    }
}
