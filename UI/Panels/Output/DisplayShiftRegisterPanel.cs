using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
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
            displayPinPanel.EnablePWMSelect(false);
        }

        internal void SyncFromConfig(OutputConfigItem config)
        {
            if (!(config.Device is ShiftRegister)) return;

            var shiftRegister = config.Device as ShiftRegister;
            // pre-select display stuff
            if (shiftRegister.Address != null)
            {
                if (!ComboBoxHelper.SetSelectedItem(shiftRegistersComboBox, shiftRegister.Address))
                {
                    Log.Instance.log($"Exception on selecting item {shiftRegister.Address} in Shift Register ComboBox.", LogSeverity.Error);
                }
            }

            UpdatePinList();

            if (shiftRegister.Pin != null) {
                OutputConfigItem cfg = config.Clone() as OutputConfigItem;
                cfg.Device = new Output()
                {
                    DisplayPin = shiftRegister.Pin,
                    DisplayPinBrightness = shiftRegister.Brightness,
                    DisplayPinPWM = shiftRegister.PWM
                };
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

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            config.Device = new ShiftRegister();
            var shiftRegister = config.Device as ShiftRegister;

            shiftRegister.Address = shiftRegistersComboBox.SelectedValue as String;

            OutputConfigItem cfg = config.Clone() as OutputConfigItem;
            cfg = displayPinPanel.syncToConfig(cfg);

            shiftRegister.Pin = (cfg.Device as Output).DisplayPin;
            shiftRegister.Brightness = (cfg.Device as Output).DisplayPinBrightness;
            shiftRegister.PWM = (cfg.Device as Output).DisplayPinPWM;
            return config;
        }
        internal void SetNumModules(int num8bitRegisters)
        {
            this.RegisterCount = num8bitRegisters;
            UpdatePinList();
        }
    }
}
