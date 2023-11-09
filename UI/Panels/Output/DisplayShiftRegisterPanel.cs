using System;
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
            displayPinPanel.EnablePWMSelect(false);
        }

        internal void SyncFromConfig(OutputConfigItem config)
        {
            // pre-select display stuff
            if (config.ShiftRegister != null && config.ShiftRegister.Address != null)
            {
                if (!ComboBoxHelper.SetSelectedItem(shiftRegistersComboBox, config.ShiftRegister.Address))
                {
                    Log.Instance.log($"Exception on selecting item {config.ShiftRegister.Address} in Shift Register ComboBox.", LogSeverity.Error);
                }
            }

            UpdatePinList();

            if (config.ShiftRegister.Pin != null) {
                OutputConfigItem cfg = config.Clone() as OutputConfigItem;
                cfg.Pin.DisplayPin = config.ShiftRegister.Pin;
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
            config.ShiftRegister.Address = shiftRegistersComboBox.SelectedValue as String;

            OutputConfigItem cfg = config.Clone() as OutputConfigItem;
            cfg = displayPinPanel.syncToConfig(cfg);

            config.ShiftRegister.Pin = cfg.Pin.DisplayPin;
            return config;
        }
        internal void SetNumModules(int num8bitRegisters)
        {
            this.RegisterCount = num8bitRegisters;
            UpdatePinList();
        }
    }
}
