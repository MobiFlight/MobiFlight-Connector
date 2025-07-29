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
        private List<List<IConfigItem>> allConfigItems = null;
        private List<List<IConfigItem>> originalAllConfigItems = null;
        private bool changed = false;

        public OrphanedSerialsDialog(List<string> serials, List<List<IConfigItem>> configItems)
        {
            this.allConnectedSerials = serials;
            this.allConfigItems = configItems.Select(list => list.Select(item => (IConfigItem)item.Clone()).ToList()).ToList();
            this.originalAllConfigItems = configItems;
        
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
            UpdateOrphanedList();
        }

        private void UpdateOrphanedList()
        {
            List<string> configSerials = new List<string>();
            
            connectedModulesComboBox.Items.Clear();
            orphanedSerialsListBox.Items.Clear();

            foreach (string serial in allConnectedSerials)
            {
                connectedModulesComboBox.Items.Add(serial);
            }

            var configItems = allConfigItems.SelectMany(c => c).Where(c => c != null).ToList();
            var deviceNames = configItems.Select(c => c.Name).ToList();
            
            // Auto-assign orphaned serials when all device names in the profile are unique
            var autoAssign = deviceNames.Count == deviceNames.Distinct().Count();
            
            foreach (var cfg in configItems)
            {
                CheckAndAddConfigSerial(cfg.ModuleSerial, configSerials, autoAssign);
            }

            if (connectedModulesComboBox.Items.Count > 0) connectedModulesComboBox.SelectedIndex = 0;
        }

        private void CheckAndAddConfigSerial(string configSerial, List<string> configSerials, bool autoAssign)
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
                    string orphanedDeviceName = SerialNumber.ExtractDeviceName(configSerial);
                    if (!string.IsNullOrEmpty(orphanedDeviceName))
                    {
                        string matchingConnectedSerial = FindConnectedSerialByDeviceName(orphanedDeviceName, serialNumber);
                        
                        if (!string.IsNullOrEmpty(matchingConnectedSerial))
                        {
                            ReplaceSerialBySerial(configSerial, matchingConnectedSerial);
                            changed = true;
                            return;
                        }
                    }
                    
                    configSerials.Add(configSerial);
                    orphanedSerialsListBox.Items.Add(configSerial);
                }
            }
        }

        /// <summary>
        /// Finds a connected serial that matches the device name and serial type
        /// </summary>
        private string FindConnectedSerialByDeviceName(string deviceName, string orphanedSerial)
        {
            List<string> candidateSerials;
            if (SerialNumber.IsMobiFlightSerial(orphanedSerial))
            {
                candidateSerials = connectedModuleSerials;
            }
            else if (SerialNumber.IsJoystickSerial(orphanedSerial))
            {
                candidateSerials = connectedJoystickSerials;
            }
            else
            {
                candidateSerials = connectedArcazeSerials;
            }

            // Find matching device name
            foreach (var connectedSerial in candidateSerials)
            {
                var connectedDeviceName = SerialNumber.ExtractDeviceName(connectedSerial);
                if (string.Equals(deviceName, connectedDeviceName, StringComparison.OrdinalIgnoreCase))
                {
                    return connectedSerial;
                }
            }

            return null;
        }

        private void ReplaceSerialBySerial(string oldSerial, string newSerial)
        {
            allConfigItems.ForEach(cfgItems =>
            {
                cfgItems.ForEach(item =>
                {
                    if (item?.ModuleSerial == oldSerial)
                    {
                        item.ModuleSerial = newSerial;
                    }
                });
            });
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

            ReplaceSerialBySerial(orphanedSerialsListBox.SelectedItem.ToString(), 
                                  connectedModulesComboBox.SelectedItem.ToString());
            changed = true;
            UpdateOrphanedList();
            (sender as Button).Enabled = false;
        }

        public bool HasChanged()
        {
            return changed;
        }

        public List<List<IConfigItem>> GetUpdatedConfigs()
        {
            return allConfigItems;
        }
    }
}
