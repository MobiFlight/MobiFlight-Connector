using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight;

namespace MobiFlight.UI.Dialogs
{
    public partial class OrphanedSerialsDialog : Form
    {
        List<string> moduleSerials = null;
        DataTable configDataTable = null;
        DataTable realConfigDataTable = null;
        DataTable inputDataTable = null;
        DataTable realInputDataTable = null;
        bool changed = false;

        public OrphanedSerialsDialog(List<string> serials, DataTable dataTable, DataTable inputDataTable)
        {
            this.moduleSerials = serials;
            this.configDataTable = dataTable.Copy();
            this.realConfigDataTable = dataTable;
            this.inputDataTable = inputDataTable.Copy();
            this.realInputDataTable = inputDataTable;
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

            foreach (DataRow row in configDataTable.Rows) {
                OutputConfigItem cfg = row["settings"] as OutputConfigItem;
                if (cfg.DisplaySerial != "" && 
                    cfg.DisplaySerial  != "-" && 
                    !Joystick.IsJoystickSerial(cfg.DisplaySerial) &&
                    !configSerials.Contains(cfg.DisplaySerial) && 
                    !moduleSerials.Contains(cfg.DisplaySerial))
                {
                    configSerials.Add(cfg.DisplaySerial);
                    orphanedSerialsListBox.Items.Add(cfg.DisplaySerial);
                }
            }

            foreach (DataRow row in inputDataTable.Rows)
            {
                InputConfigItem cfg = row["settings"] as InputConfigItem;
                if (cfg != null &&
                    cfg.ModuleSerial != "" &&
                    cfg.ModuleSerial != "-" &&
                    !Joystick.IsJoystickSerial(cfg.ModuleSerial) &&
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
            foreach (DataRow row in configDataTable.Rows)
            {
                OutputConfigItem cfg = row["settings"] as OutputConfigItem;
                if (cfg?.DisplaySerial == oldSerial)
                {
                    cfg.DisplaySerial = newSerial;
                    row["settings"] = cfg;
                }
            }

            foreach (DataRow row in inputDataTable.Rows)
            {
                InputConfigItem cfg = row["settings"] as InputConfigItem;
                if (cfg?.ModuleSerial == oldSerial)
                {
                    cfg.ModuleSerial = newSerial;
                    row["settings"] = cfg;
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
            for (int i=0; i!=realConfigDataTable.Rows.Count;++i)
            {
                realConfigDataTable.Rows[i]["settings"] = configDataTable.Rows[i]["settings"];                
            }

            for (int i = 0; i != realInputDataTable.Rows.Count; ++i)
            {
                realInputDataTable.Rows[i]["settings"] = inputDataTable.Rows[i]["settings"];
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
