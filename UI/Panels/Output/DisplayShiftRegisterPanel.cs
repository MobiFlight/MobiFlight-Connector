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
        DataView RefsDataView = null;
        private string filterReferenceGuid;
        private int RegisterCount = 0;
        private int MAX_8BIT_REGISTER_COUNT = 4;

        //private HashSet<string> shiftRegisterPWMSupport;
        //private int num8bitRegisters;

        private List<CheckBox> pinCheckboxes = new List<CheckBox>();
        public bool WideStyle = false;



        public DisplayShiftRegisterPanel()
        {
            InitializeComponent();
        }

        internal void SyncFromConfig(OutputConfigItem config)
        {
            // preselect display stuff
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

            for (int row = 1; row <= MAX_8BIT_REGISTER_COUNT; row++)
            {
                for (int column = 1; column <= 8; column++)
                {
                    CheckBox cb = new CheckBox() { Text = "", Checked = false };
                    pinCheckboxes.Add(cb);
                    tableLayoutPanel.Controls.Add(cb, column, row);
                }
            }
            
            if (config.RegisterOutputPin != null)
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
            }

            List<ListItem> configRefs = new List<ListItem>();
            configRefs.Add(new ListItem { Value = string.Empty, Label = "<None>" });
            foreach (DataRow refRow in RefsDataView.Table.Rows)
            {

                if (!filterReferenceGuid.Equals(refRow["guid"].ToString()))
                {
                    configRefs.Add(new ListItem { Value = ((Guid)refRow["guid"]).ToString(), Label = refRow["description"] as string });
                }
            }

            brightnessDropDown.Enabled = true;
            brightnessDropDown.DataSource = configRefs;
            brightnessDropDown.DisplayMember = "Label";
            brightnessDropDown.ValueMember = "Value";

            if (!string.IsNullOrEmpty(config.ShiftRegisterPWMReference))
            {
                brightnessDropDown.SelectedValue = config.ShiftRegisterPWMReference;
            }
        }

        private void UpdatePinTable()
        {
            // Fill table with check boxes
            for (int row = 1; row <= RegisterCount; row++)
            {
                tableLayoutPanel.RowStyles[row].Height = 20;
            }

            // Clear checkboxes which are not visible and hide colums which are not used.
            for (int row = RegisterCount + 1; row <= MAX_8BIT_REGISTER_COUNT; row++)
            {
                for (int col = 0; col < 8; col++) {
                    if (pinCheckboxes.Count > row * col )
                    {
                        pinCheckboxes[row * col].Checked = true;
                    }                        
                }
                tableLayoutPanel.RowStyles[row].Height = 0;                
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


            if (brightnessDropDown.SelectedIndex <= 0 || brightnessDropDown.Enabled == false)
            {
                config.ShiftRegisterPWMReference = string.Empty;
            }
            else
            {
                config.ShiftRegisterPWMReference = brightnessDropDown.SelectedValue.ToString();
            }

            return config;
        }

        internal void SetConfigRefsDataView(DataView dv, string filterGuid)
        {
            this.filterReferenceGuid = filterGuid == null ? string.Empty : filterGuid;
            RefsDataView = dv;
        }

        internal void UpdatePWMSupport(bool shiftRegisterPWMSupport)
        {

            if (shiftRegisterPWMSupport)
            {
                brightnessDropDown.Enabled = true;
            }
            else
            {
                brightnessDropDown.Enabled = false;
            }
        }

        internal void SetNumModules(int num8bitRegisters)
        {
            this.RegisterCount = num8bitRegisters;
            UpdatePinTable();
        }
    }
}
