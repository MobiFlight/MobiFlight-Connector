using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MobiFlight.UI.Dialogs
{
    public partial class OrphanedSerialsDialog : Form
    {
        private List<string> allConnectedSerials = new List<string>();
        private List<string> connectedModuleSerials = new List<string>();
        private List<string> connectedArcazeSerials = new List<string>();
        private List<string> connectedJoystickSerials = new List<string>();
        private List<IConfigItem> allConfigItems = null;
        private List<IConfigItem> originalAllConfigItems = null;
        private bool changed = false;

        public OrphanedSerialsDialog(List<string> serials, List<IConfigItem> dataTable)
        {
            this.allConnectedSerials = serials;
            this.allConfigItems = dataTable.Select(item => (IConfigItem)item.Clone()).ToList();
            this.originalAllConfigItems = dataTable;
        
            foreach (string serial in allConnectedSerials)
            {
                string serialNumber = SerialNumber.ExtractSerial(serial);
                if (SerialNumber.IsJoystickSerial(serialNumber))
                {
                    connectedJoystickSerials.Add(serial);
                }
                else if (SerialNumber.IsMobiFlightSerial(serialNumber))
                {
                    connectedModuleSerials.Add(serial);
                }
                else
                {
                    connectedArcazeSerials.Add(serial);
                }
            }

            InitializeComponent();
            updateOrphanedList();
        }

        protected void updateOrphanedList()
        {
            List<string> configSerials = new List<string>();
            
            connectedModulesComboBox.Items.Clear();
            orphanedSerialsListBox.Items.Clear();

            foreach (string serial in allConnectedSerials)
            {
                connectedModulesComboBox.Items.Add(serial);
            }

            foreach (IConfigItem cfg in allConfigItems) 
            {
                if (cfg== null) continue;
                CheckAndAddConfigSerial(cfg.ModuleSerial, configSerials);
            }

            if (connectedModulesComboBox.Items.Count > 0) connectedModulesComboBox.SelectedIndex = 0;
        }

        private void CheckAndAddConfigSerial(string configSerial, List<string> configSerials)
        {
            if (configSerial != "" &&
                configSerial != "-" &&
                !configSerials.Contains(configSerial) &&
                !allConnectedSerials.Contains(configSerial))
            {
                string serialNumber = SerialNumber.ExtractSerial(configSerial);
                bool showOrphanedJoystick = SerialNumber.IsJoystickSerial(serialNumber) && connectedJoystickSerials.Count > 0;
                bool showOrphanedModule = SerialNumber.IsMobiFlightSerial(serialNumber) && connectedModuleSerials.Count > 0;        
                bool showOrphanedArcaze = SerialNumber.IsArcazeSerial(serialNumber) && connectedArcazeSerials.Count > 0;

                if (showOrphanedJoystick || showOrphanedModule || showOrphanedArcaze)
                { 
                    configSerials.Add(configSerial);
                    orphanedSerialsListBox.Items.Add(configSerial);
                }
            }
        }

        protected void replaceSerialBySerial(string oldSerial, string newSerial)
        {
            foreach (IConfigItem cfg in allConfigItems)
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
            for (int i=0; i!=originalAllConfigItems.Count;++i)
            {
                originalAllConfigItems[i] = allConfigItems[i];                
            }

            //realConfigDataTable = configDataTable;
            DialogResult = DialogResult.OK;
        }

        private void orphanedSerialsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {   
            if (orphanedSerialsListBox.Items.Count > 0 || orphanedSerialsListBox.SelectedItem == null)
            {
                connectedModulesAssignButton.Enabled = true;

                // Filter for selected serial type          
                connectedModulesComboBox.Items.Clear();
                string selectedSerial = SerialNumber.ExtractSerial((string)orphanedSerialsListBox.SelectedItem);
                if (SerialNumber.IsMobiFlightSerial(selectedSerial))
                {
                    connectedModulesComboBox.Items.AddRange(connectedModuleSerials.ToArray());
                }
                else if (SerialNumber.IsJoystickSerial(selectedSerial))
                {
                    connectedModulesComboBox.Items.AddRange(connectedJoystickSerials.ToArray());
                }
                else
                {
                    connectedModulesComboBox.Items.AddRange(connectedArcazeSerials.ToArray());
                }
                if (connectedModulesComboBox.Items.Count > 0) connectedModulesComboBox.SelectedIndex = 0;
            }
            else
            {
                connectedModulesAssignButton.Enabled = false;
            }
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
