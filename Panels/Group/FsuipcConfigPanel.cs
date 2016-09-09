using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight;
using MobiFlight.InputConfig;

namespace MobiFlight.Panels.Group
{
    public partial class FsuipcConfigPanel : UserControl
    {
        public String PresetFile { get; set; }
        ErrorProvider errorProvider = new ErrorProvider();

        public FsuipcConfigPanel()
        {
            InitializeComponent();
        }

        private void FsuipcConfigPanel_Load(object sender, EventArgs e)
        {
            // if one opens the dialog for a new config
            // ensure that always the first tab is shown
            _initFsuipcOffsetTypeComboBox();
            PresetFile = Properties.Settings.Default.PresetFile;
            _loadPresets();
            fsuipcPresetComboBox.ResetText();
        }

        public void setMode(bool isOutputPanel)
        {
            multiplyPanel.Visible = isOutputPanel;
            valuePanel.Visible = !isOutputPanel;
            if (!isOutputPanel)
            {
                PresetFile = Properties.Settings.Default.InputsPresetFile;
                _loadPresets();
            }
        }

        private void _loadPresets()
        {
            bool isLoaded = true;

            if (!System.IO.File.Exists(PresetFile))
            {
                isLoaded = false;
                MessageBox.Show(MainForm._tr("uiMessageConfigWizard_PresetsNotFound"), MainForm._tr("Hint"));
                Log.Instance.log("Could not load file: " + PresetFile, LogSeverity.Debug);
            }
            else
            {

                try
                {
                    presetsDataSet.Clear();
                    presetsDataSet.ReadXml(PresetFile);
                    DataRow[] rows = presetDataTable.Select("", "description");
                    fsuipcPresetComboBox.Items.Clear();

                    foreach (DataRow row in rows)
                    {
                        fsuipcPresetComboBox.Items.Add(row["description"]);
                    }
                }
                catch (Exception e)
                {
                    isLoaded = false;
                    MessageBox.Show(MainForm._tr("uiMessageConfigWizard_ErrorLoadingPresets"), MainForm._tr("Hint"));
                }
            }

            fsuipcPresetComboBox.Enabled = isLoaded;
            fsuipcPresetUseButton.Enabled = isLoaded;
        }

        private void _initFsuipcOffsetTypeComboBox()
        {
            List<ListItem> offsetTypes = new List<ListItem>() {
                new ListItem() { Value = FSUIPCOffsetType.Integer.ToString(),       Label = "Int" },
                /*new ListItem() { Value = FSUIPCOffsetType.UnsignedInt.ToString(),   Label = "UInt" },*/
                new ListItem() { Value = FSUIPCOffsetType.Float.ToString(),         Label = "Float" },
                new ListItem() { Value = FSUIPCOffsetType.String.ToString(),        Label = "String" }
            };

            fsuipcOffsetTypeComboBox.DataSource = offsetTypes;
            fsuipcOffsetTypeComboBox.DisplayMember = "Label";
            fsuipcOffsetTypeComboBox.ValueMember = "Value";
            fsuipcOffsetTypeComboBox.SelectedIndex = 0;
        }

        private void fsuipcPresetUseButton_Click(object sender, EventArgs e)
        {
            if (fsuipcPresetComboBox.Text != "")
            {
                DataRow[] rows = presetDataTable.Select("description = '" + fsuipcPresetComboBox.Text + "'");
                if (rows.Length > 0)
                {
                    syncFromConfig(rows[0]["settings"] as IFsuipcConfigItem);
                }
            }
        }

        private void fsuipcOffsetTextBox_Validating(object sender, CancelEventArgs e)
        {
            _validatingHexFields(sender, e, 4);
        }

        private void fsuipcSizeComboBox_TextChanged(object sender, EventArgs e)
        {
            // we always set the mask according to the set bytes
            fsuipcMaskTextBox.Text = "0x" + (
                                        new String('F',
                                                    UInt16.Parse((sender as ComboBox).Text) * 2
                                                   ));
        }

        private void maskEditorButton_Click(object sender, EventArgs e)
        {
            BitMaskEditorForm bme = new BitMaskEditorForm(
                                        Byte.Parse(fsuipcSizeComboBox.Text),
                                        UInt64.Parse(fsuipcMaskTextBox.Text.Replace("0x", "").ToLower(),
                                                     System.Globalization.NumberStyles.HexNumber));
            bme.StartPosition = FormStartPosition.CenterParent;
            if (bme.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fsuipcMaskTextBox.Text = "0x" + bme.Result.ToString("X" + (Byte.Parse(fsuipcSizeComboBox.Text) * 2));
            }
        }

        private void _validatingHexFields(object sender, CancelEventArgs e, int length)
        {
            try
            {
                string tmp = (sender as TextBox).Text.Replace("0x", "").ToUpper();
                (sender as TextBox).Text = "0x" + Int64.Parse(tmp, System.Globalization.NumberStyles.HexNumber).ToString("X" + length.ToString());
                removeError(sender as Control);
            }
            catch (Exception exc)
            {
                e.Cancel = true;
                Log.Instance.log("_validatingHexFields : Parsing problem, " + exc.Message, LogSeverity.Debug);
                displayError(sender as Control, MainForm._tr("uiMessageConfigWizard_ValidHexFormat"));
            }
        }

        private void fsuipcValueTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void fsuipcOffsetTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateByteSizeComboBox();
        }
        
        private void updateByteSizeComboBox()
        {
            string selectedText = fsuipcSizeComboBox.Text;
            fsuipcSizeComboBox.Items.Clear();
            fsuipcSizeComboBox.Enabled = true;
            SubstringPanel.Visible = false;

            if ((fsuipcOffsetTypeComboBox.SelectedItem as ListItem).Value == FSUIPCOffsetType.Integer.ToString())
            {
                fsuipcSizeComboBox.Items.Add("1");
                fsuipcSizeComboBox.Items.Add("2");
                fsuipcSizeComboBox.Items.Add("4");
                fsuipcSizeComboBox.Items.Add("8");
                ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, selectedText);
            }
            else if ((fsuipcOffsetTypeComboBox.SelectedItem as ListItem).Value == FSUIPCOffsetType.Float.ToString())
            {
                fsuipcSizeComboBox.Items.Add("4");
                fsuipcSizeComboBox.Items.Add("8");
                if (!ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, selectedText))
                {
                    ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, "4");
                };
            }
            else if ((fsuipcOffsetTypeComboBox.SelectedItem as ListItem).Value == FSUIPCOffsetType.String.ToString())
            {
                fsuipcSizeComboBox.Enabled = false;
                SubstringPanel.Visible = false;
                // show the string stuff instead
            }
        }

        internal void syncFromConfig(IFsuipcConfigItem config)
        {
            if (config == null)
            {
                // this happens when casting badly
                return;
            }
            // first tab                        
            fsuipcOffsetTextBox.Text = "0x" + config.FSUIPCOffset.ToString("X4");

            // preselect fsuipc offset type
            try
            {
                fsuipcOffsetTypeComboBox.SelectedValue = config.FSUIPCOffsetType.ToString();
            }
            catch (Exception exc)
            {
                // TODO: provide error message
                Log.Instance.log("FsuipcConfigPanel::syncFromConfig : Exception on FSUIPCOffsetType.ToString", LogSeverity.Debug);
            }

            if (!ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, config.FSUIPCSize.ToString()))
            {
                // TODO: provide error message
                Log.Instance.log("FsuipcConfigPanel::syncFromConfig : Exception on selecting item in ComboBox", LogSeverity.Debug);
            }

            // mask
            fsuipcMaskTextBox.Text = "0x" + config.FSUIPCMask.ToString("X" + config.FSUIPCSize);

            // multiplier
            fsuipcMultiplyTextBox.Text = config.Transform.Expression;
            fsuipcBcdModeCheckBox.Checked = config.FSUIPCBcdMode;
            fsuipcValueTextBox.Text = config.Value;
        }

        internal void syncToConfig(IFsuipcConfigItem config)
        {
            config.FSUIPCMask = Int64.Parse(fsuipcMaskTextBox.Text.Replace("0x", "").ToLower(), System.Globalization.NumberStyles.HexNumber);
            config.FSUIPCOffset = Int32.Parse(fsuipcOffsetTextBox.Text.Replace("0x", "").ToLower(), System.Globalization.NumberStyles.HexNumber);
            config.FSUIPCOffsetType = (FSUIPCOffsetType)Enum.Parse(typeof(FSUIPCOffsetType), ((ListItem)(fsuipcOffsetTypeComboBox.SelectedItem)).Value);
            if (config.FSUIPCOffsetType != FSUIPCOffsetType.String)
            {
                config.FSUIPCSize = Byte.Parse(fsuipcSizeComboBox.Text);
            }
            else
            {
                config.FSUIPCSize = 255;
            }
            //config.FSUIPCMultiplier = Double.Parse(fsuipcMultiplyTextBox.Text);
            config.Transform.Expression = fsuipcMultiplyTextBox.Text;
            config.FSUIPCBcdMode = fsuipcBcdModeCheckBox.Checked;
            config.Value = fsuipcValueTextBox.Text;
        }

        internal MobiFlight.InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.FsuipcOffsetInputAction config = new FsuipcOffsetInputAction();

            config.FSUIPCMask = Int64.Parse(fsuipcMaskTextBox.Text.Replace("0x", "").ToLower(), System.Globalization.NumberStyles.HexNumber);
            config.FSUIPCOffset = Int32.Parse(fsuipcOffsetTextBox.Text.Replace("0x", "").ToLower(), System.Globalization.NumberStyles.HexNumber);
            config.FSUIPCSize = Byte.Parse(fsuipcSizeComboBox.Text);
            config.FSUIPCOffsetType = (FSUIPCOffsetType)Enum.Parse(typeof(FSUIPCOffsetType), ((ListItem)(fsuipcOffsetTypeComboBox.SelectedItem)).Value);
            //config.FSUIPCMultiplier = Double.Parse(fsuipcMultiplyTextBox.Text);
            config.Transform.Expression = fsuipcMultiplyTextBox.Text;
            config.FSUIPCBcdMode = fsuipcBcdModeCheckBox.Checked;
            config.Value = fsuipcValueTextBox.Text;
            return config;
        }

        private void fsuipcSizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // we always set the mask according to the set bytes
            fsuipcMaskTextBox.Text = "0x" + (
                                        new String ('F', 
                                                    UInt16.Parse((sender as ComboBox).Text)* 2
                                                   ));
        }

        private void fsuipcMaskTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                _validatingHexFields(sender, e, int.Parse(fsuipcSizeComboBox.Text) * 2);
            }
            catch (Exception ex)
            {
                Log.Instance.log("fsuipcMultiplyTextBox_Validating : Parsing problem, " + ex.Message, LogSeverity.Debug);
                displayError(sender as Control, ex.Message);
                e.Cancel = false;
            }
        }

        private void fsuipcMultiplyTextBox_Validating(object sender, CancelEventArgs e)
        {
            // do not validate when multiply panel is not visible
            if ((sender as TextBox).Name == fsuipcMultiplyTextBox.Name && !multiplyPanel.Visible) return;

            return;
            /*
            try
            {
                float.Parse((sender as TextBox).Text);
                removeError(sender as Control);
            }
            catch (Exception exc)
            {
                Log.Instance.log("fsuipcMultiplyTextBox_Validating : Parsing problem, " + exc.Message, LogSeverity.Debug);
                displayError(sender as Control, MainForm._tr("uiMessageFsuipcConfigPanelMultiplyWrongFormat"));
                e.Cancel = true;
            }
            */
        }

        private void fsuipcValueTextBox_Validating(object sender, CancelEventArgs e)
        {
            if ((sender as TextBox).Text.Trim() == "")
            {
                displayError(sender as Control, MainForm._tr("uiMessageFsuipcConfigPanelNoValue"));
                e.Cancel = true;
            }
            else
            {
                removeError(sender as Control);
            }
        }

        private void displayError(Control control, String message)
        {
            errorProvider.SetIconAlignment(control, ErrorIconAlignment.TopRight);
            errorProvider.SetError(
                    control,
                    message);
            MessageBox.Show(message, MainForm._tr("Hint"));
        }

        private void removeError(Control control)
        {
            errorProvider.SetError(
                    control,
                    "");
        }

    }
}
