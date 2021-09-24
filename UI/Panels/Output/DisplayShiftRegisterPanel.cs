﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class DisplayShiftRegisterPanel : UserControl
    {
        private int RegisterCount = 0;
        public bool WideStyle = false;

        public DisplayShiftRegisterPanel()
        {
            InitializeComponent();

            displayPinPanel.SetPorts(new List<ListItem>());
            displayPinPanel.WideStyle = true;
        }

        internal void SyncFromConfig(OutputConfigItem config)
        {
            // pre-select display stuff
            if (config.ShiftRegister != null)
            {
                if (!ComboBoxHelper.SetSelectedItem(shiftRegistersComboBox, config.ShiftRegister.ToString()))
                {
                    Log.Instance.log("_syncConfigToForm : Exception on selecting item in Shift Register ComboBox", LogSeverity.Debug);
                }
            }

            UpdatePinList();

            if (config.RegisterOutputPin != null) {
                OutputConfigItem cfg = config.Clone() as OutputConfigItem;
                cfg.Pin.DisplayPin = config.RegisterOutputPin;
                displayPinPanel.syncFromConfig(cfg);
            }
        }

        private void UpdatePinList()
        {

            List<ListItem> pinList = new List<ListItem>();
            for (int row = 1; row <= RegisterCount; row++)
            {

                for (int column = 1; column <= 8; column++)
                {
                    string itemNum = (8 * (row - 1) + column - 1).ToString();
                    pinList.Add(new ListItem() { Label = 
                        MobiFlightShiftRegister.LABEL_PREFIX + " " + itemNum, 
                        Value = MobiFlightShiftRegister.LABEL_PREFIX + " " + itemNum 
                    });
                }
            }

            
            displayPinPanel.SetPins(pinList);
        }

        public void SetAddresses(List<ListItem> ports)
        {
            shiftRegistersComboBox.DataSource = new List<ListItem>(ports);
            shiftRegistersComboBox.DisplayMember = "Label";
            shiftRegistersComboBox.ValueMember = "Value";
            if (ports.Count > 0)
                shiftRegistersComboBox.SelectedIndex = 0;

            shiftRegistersComboBox.Enabled = ports.Count > 0;
        }

        internal OutputConfigItem SyncToConfig(OutputConfigItem config)
        {
            config.ShiftRegister = shiftRegistersComboBox.SelectedValue as String;

            OutputConfigItem cfg = config.Clone() as OutputConfigItem;
            cfg = displayPinPanel.syncToConfig(cfg);

            config.RegisterOutputPin = cfg.Pin.DisplayPin;
            return config;
        }
        internal void SetNumModules(int num8bitRegisters)
        {
            this.RegisterCount = num8bitRegisters;
            UpdatePinList();
        }
    }
}
