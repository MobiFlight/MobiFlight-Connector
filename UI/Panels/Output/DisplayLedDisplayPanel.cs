using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
            if (config.LedModule.Address != null)
            {
                if (!ComboBoxHelper.SetSelectedItem(displayLedAddressComboBox, config.LedModule.Address.ToString()))
                {
                    // TODO: provide error message
                    Log.Instance.log("Exception on selecting item in LED address ComboBox.", LogSeverity.Error);
                }
                else
                {
                    OnLedAddressChanged?.Invoke(displayLedAddressComboBox, new EventArgs());
                }
            }

            if (config.LedModule.Address != null)
            {
                if (!ComboBoxHelper.SetSelectedItem(displayLedConnectorComboBox, config.LedModule.Connector.ToString()))
                {
                    // TODO: provide error message
                    Log.Instance.log("Exception on selecting item in LED connector ComboBox.", LogSeverity.Error);
                }
            }

            if (!ComboBoxHelper.SetSelectedItem(displayLedModuleSizeComboBox, config.LedModule.ModuleSize.ToString()))
            {
                // TODO: provide error message
                Log.Instance.log("Exception on selecting item in LED module size ComboBox.", LogSeverity.Error);
            }

            displayLedPaddingCheckBox.Checked = config.LedModule.Padding;
            displayLedReverseDigitsCheckBox.Checked = config.LedModule.ReverseDigits;
            SetPaddingChar(config.LedModule.PaddingChar);

            foreach (string digit in config.LedModule.Digits)
            {
                (displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + digit + "Checkbox"] as CheckBox).Checked = true;
            }

            foreach (string digit in config.LedModule.DecimalPoints)
            {
                (displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + digit + "Checkbox"] as CheckBox).Checked = true;
            }

            if (!string.IsNullOrEmpty(config.LedModule.BrightnessReference))
            {
                brightnessDropDown.SelectedValue = config.LedModule.BrightnessReference;
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
            ComboBox cb = displayLedConnectorComboBox;
            cb.DataSource = new List<ListItem>(pins);
            cb.DisplayMember = "Label";
            cb.ValueMember = "Value";
            if (pins.Count > 0)
                cb.SelectedIndex = 0;
            cb.Enabled = pins.Count > 1;
        }

        public void SetSizeDigits(List<ListItem> entries)
        {
            ComboBox cb = displayLedModuleSizeComboBox;
            cb.DataSource = new List<ListItem>(entries);
            cb.DisplayMember = "Label";
            cb.ValueMember = "Value";
            if (entries.Count > 0)
                cb.SelectedIndex = entries.Count - 1;
            cb.Enabled = entries.Count > 1;
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
                config.LedModule.Address = displayLedAddressComboBox.SelectedValue as String;

            if (brightnessDropDown.SelectedValue != null)
                config.LedModule.BrightnessReference = brightnessDropDown.SelectedValue.ToString();

            config.LedModule.Padding = displayLedPaddingCheckBox.Checked;
            config.LedModule.ReverseDigits = displayLedReverseDigitsCheckBox.Checked;
            config.LedModule.PaddingChar = GetPaddingChar();
            try
            {
                config.LedModule.Connector = byte.Parse(displayLedConnectorComboBox.Text);
                config.LedModule.ModuleSize = byte.Parse(displayLedModuleSizeComboBox.Text);
            }
            catch (FormatException ex)
            {
                Log.Instance.log($"Exception parsing values: {ex.Message}", LogSeverity.Error);
            }
            config.LedModule.Digits.Clear();
            config.LedModule.DecimalPoints.Clear();
            for (int i = 0; i < 8; i++)
            {
                CheckBox cb = (displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + i + "Checkbox"] as CheckBox);
                if (cb.Checked)
                {
                    config.LedModule.Digits.Add(i.ToString());
                } //if

                cb = (displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + i + "Checkbox"] as CheckBox);
                if (cb.Checked)
                {
                    config.LedModule.DecimalPoints.Add(i.ToString());
                } //if
            }
            if (brightnessDropDown.SelectedIndex <= 0)
            {
                config.LedModule.BrightnessReference = string.Empty;
            }
            else
            {
                config.LedModule.BrightnessReference = brightnessDropDown.SelectedValue.ToString();
            }
            return config;
        }

        internal void SetConfigRefsDataView(List<OutputConfigItem> dv, string filterGuid)
        {
            this.filterReferenceGuid = filterGuid == null ? string.Empty : filterGuid;

            List<ListItem> configRefs = new List<ListItem>
            {
                new ListItem { Value = string.Empty, Label = "<None>" }
            };

            foreach (var refCfg in dv)
            {

                if (!filterReferenceGuid.Equals(refCfg.GUID))
                {
                    configRefs.Add(new ListItem { Value = refCfg.GUID, Label = refCfg.Description });
                }
            }

            brightnessDropDown.DataSource = configRefs;
            brightnessDropDown.DisplayMember = "Label";
            brightnessDropDown.ValueMember = "Value";
        }
    }
}