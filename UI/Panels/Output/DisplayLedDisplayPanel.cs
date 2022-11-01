using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.Base;

namespace MobiFlight.UI.Panels
{
    public partial class DisplayLedDisplayPanel : UserControl
    {
        public bool WideStyle = false;
        private string filterReferenceGuid;
        public event EventHandler OnLedAddressChanged;
        
        public DisplayLedDisplayPanel()
        {
            InitializeComponent();
            InitPanelWithDefaultSettings();
            displayLedAddressComboBox.SelectedIndexChanged += (sender, e) =>
            {
                OnLedAddressChanged?.Invoke(displayLedAddressComboBox, new EventArgs());
            };
        }

        private void InitPanelWithDefaultSettings()
        {
            syncFromConfig(new OutputConfigItem());
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            // preselect display stuff
            if (config.LedModule.DisplayLedAddress != null)
            {
                if (!ComboBoxHelper.SetSelectedItem(displayLedAddressComboBox, config.LedModule.DisplayLedAddress.ToString()))
                {
                    // TODO: provide error message
                    Log.Instance.log("Exception on selecting item in LED address ComboBox.", LogSeverity.Error);
                } else
                {
                    OnLedAddressChanged?.Invoke(displayLedAddressComboBox, new EventArgs());
                }
            }

            if (config.LedModule.DisplayLedAddress != null)
            {
                if (!ComboBoxHelper.SetSelectedItem(displayLedConnectorComboBox, config.LedModule.DisplayLedConnector.ToString()))
                {
                    // TODO: provide error message
                    Log.Instance.log("Exception on selecting item in LED connector ComboBox.", LogSeverity.Error);
                }
            }

            if (!ComboBoxHelper.SetSelectedItem(displayLedModuleSizeComboBox, config.LedModule.DisplayLedModuleSize.ToString()))
            {
                // TODO: provide error message
                Log.Instance.log("Exception on selecting item in LED module size ComboBox.", LogSeverity.Error);
            }

            displayLedPaddingCheckBox.Checked = config.LedModule.DisplayLedPadding;
            displayLedReverseDigitsCheckBox.Checked = config.LedModule.DisplayLedReverseDigits;
            SetPaddingChar(config.LedModule.DisplayLedPaddingChar);

            foreach (string digit in config.LedModule.DisplayLedDigits)
            {
                (displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + digit + "Checkbox"] as CheckBox).Checked = true;
            }

            foreach (string digit in config.LedModule.DisplayLedDecimalPoints)
            {
                (displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + digit + "Checkbox"] as CheckBox).Checked = true;
            }

            if (!string.IsNullOrEmpty(config.LedModule.DisplayLedBrightnessReference))
            {
                brightnessDropDown.SelectedValue = config.LedModule.DisplayLedBrightnessReference;
            }
        }

        public void SetPaddingChar(String prefix)
        {
            if (prefix == " ") PaddingCharComboBox.SelectedIndex = 1;
            else PaddingCharComboBox.SelectedIndex = 0;
        }

        public void SetAddresses(List<ListItem> ports)
        {
            displayLedAddressComboBox.DataSource = new List<ListItem>(ports);
            displayLedAddressComboBox.DisplayMember = "Label";
            displayLedAddressComboBox.ValueMember = "Value";
            if (ports.Count > 0)
                displayLedAddressComboBox.SelectedIndex = 0;

            displayLedAddressComboBox.Enabled = ports.Count > 0;
            displayLedAddressComboBox.Width = WideStyle ? displayLedAddressComboBox.MaximumSize.Width : displayLedAddressComboBox.MinimumSize.Width;
        }

        public void SetConnectors(List<ListItem> pins)
        {
            displayLedConnectorComboBox.DataSource = new List<ListItem>(pins);
            displayLedConnectorComboBox.DisplayMember = "Label";
            displayLedConnectorComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                displayLedConnectorComboBox.SelectedIndex = 0;

            displayLedConnectorComboBox.Enabled = pins.Count > 0;
        }

        private void DisplayLedModuleSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int value = Int16.Parse((sender as ComboBox).Text);
            for (int i = 0; i < 8; i++)
            {
                displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + i + "CheckBox"].Visible = i < value;
                displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + i + "CheckBox"].Visible = i < value;
                displayLedGroupFlowLayoutPanel.Controls["displayLedDisplayLabel" + i].Visible = i < value;

                // uncheck all invisible checkboxes to ensure correct mask
                if (i >= value)
                {
                    (displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + i + "CheckBox"] as CheckBox).Checked = false;
                    (displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + i + "CheckBox"] as CheckBox).Checked = false;
                }
            }
        }

        public string GetPaddingChar()
        {
            String result = "0";

            switch (PaddingCharComboBox.SelectedIndex)
            {
                case 1:
                    result = " ";
                    break;
            }

            return result;
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            if (displayLedAddressComboBox.SelectedValue as String != null)
                config.LedModule.DisplayLedAddress = displayLedAddressComboBox.SelectedValue as String;

            if (brightnessDropDown.SelectedValue!=null)
                config.LedModule.DisplayLedBrightnessReference = brightnessDropDown.SelectedValue.ToString();

            config.LedModule.DisplayLedPadding = displayLedPaddingCheckBox.Checked;
            config.LedModule.DisplayLedReverseDigits = displayLedReverseDigitsCheckBox.Checked;
            config.LedModule.DisplayLedPaddingChar = GetPaddingChar();
            try
            {
                config.LedModule.DisplayLedConnector = byte.Parse(displayLedConnectorComboBox.Text);
                config.LedModule.DisplayLedModuleSize = byte.Parse(displayLedModuleSizeComboBox.Text);
            }
            catch (FormatException ex)
            {
                Log.Instance.log($"Exception parsing values: {ex.Message}", LogSeverity.Error);
            }
            config.LedModule.DisplayLedDigits.Clear();
            config.LedModule.DisplayLedDecimalPoints.Clear();
            for (int i = 0; i < 8; i++)
            {
                CheckBox cb = (displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + i + "Checkbox"] as CheckBox);
                if (cb.Checked)
                {
                    config.LedModule.DisplayLedDigits.Add(i.ToString());
                } //if

                cb = (displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + i + "Checkbox"] as CheckBox);
                if (cb.Checked)
                {
                    config.LedModule.DisplayLedDecimalPoints.Add(i.ToString());
                } //if
            }
            if (brightnessDropDown.SelectedIndex <= 0)
            {
                config.LedModule.DisplayLedBrightnessReference = string.Empty;
            } else
            {
                config.LedModule.DisplayLedBrightnessReference = brightnessDropDown.SelectedValue.ToString();
            }
            return config;
        }

        internal void SetConfigRefsDataView(DataView dv, string filterGuid)
        {            
            this.filterReferenceGuid = filterGuid==null?string.Empty:filterGuid;

            List<ListItem> configRefs = new List<ListItem>();
            configRefs.Add(new ListItem { Value = string.Empty, Label = "<None>" });
            foreach (DataRow refRow in dv.Table.Rows)
            {

                if (!filterReferenceGuid.Equals(refRow["guid"].ToString()))
                {
                    configRefs.Add(new ListItem { Value = ((Guid)refRow["guid"]).ToString(), Label = refRow["description"] as string });
                }
            }

            brightnessDropDown.DataSource = configRefs;
            brightnessDropDown.DisplayMember = "Label";
            brightnessDropDown.ValueMember = "Value";
        }
    }
}