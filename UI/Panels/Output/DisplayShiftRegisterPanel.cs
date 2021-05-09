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
        const char POSITION_SEPERATOR = '|';
        private int RegisterCount = 0;
        private int MAX_8BIT_REGISTER_COUNT = 4;

        private List<CheckBox> pinCheckboxes = new List<CheckBox>();
        public bool WideStyle = false;

        public DisplayShiftRegisterPanel()
        {
            InitializeComponent();
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

            // It's not easy to hide rows. So we just resize them.
            for (int row = 1; row <= 4; row++)
            {
                tableLayoutPanel.RowStyles[row].SizeType = SizeType.Absolute;
                tableLayoutPanel.RowStyles[row].Height = 0;
            }

            List<ListItem> pinList = new List<ListItem>();
            for (int row = 1; row <= MAX_8BIT_REGISTER_COUNT; row++)
            {
                
                for (int column = 1; column <= 8; column++)
                {
                    string itemNum = (8 * (row-1) + column - 1).ToString(); ;
                    pinList.Add(new ListItem() { Label = itemNum, Value = itemNum });

                    CheckBox cb = new CheckBox() { Text = "", Checked = false };
                    pinCheckboxes.Add(cb);
                    tableLayoutPanel.Controls.Add(cb, column, row);
                }
            }

            // Simple list
            SetPins(pinList);

            if (config.RegisterOutputPin != null)
            {

                if (config.RegisterOutputPin.Contains('|'))
                {
                    // Select the checkboxes. "|" seperated list
                    string pins = config.RegisterOutputPin;
                    var splitPins = pins.Split(POSITION_SEPERATOR);
                    foreach (string pin in splitPins)
                    {
                        int pinValue;
                        if (int.TryParse(pin, out pinValue))
                        {
                            if (pinCheckboxes.Count > pinValue)
                            {
                                pinCheckboxes[pinValue].Checked = true;
                            }
                        }
                    }
                    tabControl1.SelectedIndex = 1;
                } else
                {
                    if (!ComboBoxHelper.SetSelectedItem(displayPinComboBox, config.RegisterOutputPin)) { /* TODO: provide error message */ }
                    tabControl1.SelectedIndex = 0;
                }
            }
          
            UpdatePinTable();
        }

        private void UpdatePinTable()
        {
            // Fill table with check boxes
            for (int row = 1; row <= RegisterCount; row++)
            {
                tableLayoutPanel.RowStyles[row].Height = 20;
            }

            // Clear checkboxes which are not visible and hide colums which are not used.
            for (int row = RegisterCount; row < MAX_8BIT_REGISTER_COUNT; row++)
            {
                for (int col = 0; col < 8; col++) {                    
                    
                    if (pinCheckboxes.Count >= row * 8 + col)
                    {
                        pinCheckboxes[row * 8 + col].Checked = false;
                    }
                }
                // First row is header
                tableLayoutPanel.RowStyles[row+1].Height = 0;                
            }
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

            if (tabControl1.SelectedIndex == 1) /* Advanced tab */
            {
                StringBuilder sb = new StringBuilder();
                int pinNum = 0;
                foreach (var pin in pinCheckboxes)
                {
                    if (pin.Checked)
                    {
                        sb.Append(pinNum + POSITION_SEPERATOR.ToString());
                    }
                    pinNum++;
                }

                config.RegisterOutputPin = sb.ToString();
            }
            else /* Simpled tab */
            {
                config.RegisterOutputPin = displayPinComboBox.SelectedItem.ToString();
            }

            return config;
        }

        public void SetPins(List<ListItem> pins)
        {
            displayPinComboBox.DataSource = new List<ListItem>(pins);
            displayPinComboBox.DisplayMember = "Label";
            displayPinComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                displayPinComboBox.SelectedIndex = 0;

            displayPinComboBox.Enabled = pins.Count > 0;
            displayPinComboBox.Width = WideStyle ? displayPinComboBox.MaximumSize.Width : displayPinComboBox.MinimumSize.Width;
        }

        internal void SetNumModules(int num8bitRegisters)
        {
            this.RegisterCount = num8bitRegisters;
            UpdatePinTable();
        }
    }
}
