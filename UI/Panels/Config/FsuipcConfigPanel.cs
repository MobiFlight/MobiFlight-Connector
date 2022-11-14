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
using MobiFlight.UI.Forms;

namespace MobiFlight.UI.Panels.Config
{
    public partial class FsuipcConfigPanel : UserControl
    {
        public String PresetFile { get; set; }
        ErrorProvider errorProvider = new ErrorProvider();
        protected Boolean OutputPanelMode = true;

        public FsuipcConfigPanel()
        {
            InitializeComponent();
            // if one opens the dialog for a new config
            // ensure that always the first tab is shown
            _initFsuipcOffsetTypeComboBox();
            PresetFile = Properties.Settings.Default.PresetFileOutputs;
            _loadPresets();
            fsuipcPresetComboBox.ResetText();
        }

        public void setMode(bool isOutputPanel)
        {
            OutputPanelMode = isOutputPanel;
            // the transform field only is visible
            // if we are dealing with outputs
            multiplyPanel.Visible = OutputPanelMode;

            // and the value panel vice versa
            // only if we deal with inputs
            valuePanel.Visible = !OutputPanelMode;

            AutoSize = isOutputPanel;

            if (!OutputPanelMode)
            {
                PresetFile = Properties.Settings.Default.PresetFileInputs;
                _loadPresets();
            }
        }

        private void _loadPresets()
        {
            bool isLoaded = true;

            if (!System.IO.File.Exists(PresetFile))
            {
                isLoaded = false;
                MessageBox.Show(i18n._tr("uiMessageConfigWizard_PresetsNotFound"), i18n._tr("Hint"));
                Log.Instance.log($"Could not load file {PresetFile}.", LogSeverity.Error);
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
                    MessageBox.Show(i18n._tr("uiMessageConfigWizard_ErrorLoadingPresets"), i18n._tr("Hint"));
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
            catch (Exception ex)
            {
                e.Cancel = true;
                Log.Instance.log($"Parsing problem : {ex.Message}", LogSeverity.Debug);
                displayError(sender as Control, i18n._tr("uiMessageConfigWizard_ValidHexFormat"));
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
            fsuipcSizeLabel.Visible = true;
            fsuipcSizeComboBox.Enabled = true;
            fsuipcSizeComboBox.Visible = true;
            maskAndBcdPanel.Visible = true;
            multiplyPanel.Visible = true && OutputPanelMode;
            SubstringPanel.Visible = false && OutputPanelMode;

            if ((fsuipcOffsetTypeComboBox.SelectedItem as ListItem).Value == FSUIPCOffsetType.Integer.ToString())
            {
                // INTs come with a min size of 1 byte
                // up to 8 bytes
                fsuipcSizeComboBox.Items.Add("1");
                fsuipcSizeComboBox.Items.Add("2");
                fsuipcSizeComboBox.Items.Add("4");
                fsuipcSizeComboBox.Items.Add("8");
                if (!ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, selectedText))
                {
                    ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, "1");
                };
            }
            else if ((fsuipcOffsetTypeComboBox.SelectedItem as ListItem).Value == FSUIPCOffsetType.Float.ToString())
            {
                // floats always come with a min size of 4 bytes
                // up to 8 bytes
                fsuipcSizeComboBox.Items.Add("4");
                fsuipcSizeComboBox.Items.Add("8");
                if (!ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, selectedText))
                {
                    ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, "4");
                };

                // mask doesn't make sense for floats
                maskAndBcdPanel.Visible = false;
            }
            else if ((fsuipcOffsetTypeComboBox.SelectedItem as ListItem).Value == FSUIPCOffsetType.String.ToString())
            {
                // the size is always 255 and then defined
                // by the zero-termination 
                // this makes it easier for the user
                // because s/he doesn't have to care about it.
                fsuipcSizeLabel.Visible = false;
                fsuipcSizeComboBox.Enabled = false;
                fsuipcSizeComboBox.Visible = false;

                // mask doesn't make sense for strings
                maskAndBcdPanel.Visible = false;

                // multiply doesn't make sense for strings
                multiplyPanel.Visible = false && OutputPanelMode;

                // show the string stuff instead
                SubstringPanel.Visible = true && OutputPanelMode;
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
            fsuipcOffsetTextBox.Text = "0x" + config.FSUIPC.Offset.ToString("X4");

            // preselect fsuipc offset type
            try
            {
                fsuipcOffsetTypeComboBox.SelectedValue = config.FSUIPC.OffsetType.ToString();
            }
            catch (Exception ex)
            {
                // TODO: provide error message
                Log.Instance.log($"Exception on FSUIPCOffsetType.ToString: {ex.Message}", LogSeverity.Error);
            }

            if (!ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, config.FSUIPC.Size.ToString()))
            {
                // TODO: provide error message
                Log.Instance.log("Exception on selecting item in ComboBox.", LogSeverity.Error);
            }

            // mask
            fsuipcMaskTextBox.Text = "0xFF";
            if (config.FSUIPC.OffsetType != FSUIPCOffsetType.String)
                fsuipcMaskTextBox.Text = "0x" + config.FSUIPC.Mask.ToString("X" + config.FSUIPC.Size.ToString());

            // multiplier
            if (config.FSUIPC.OffsetType != FSUIPCOffsetType.String) {
                TransformationCheckBox.Checked = config.Transform.Active;
                SubstringTransformationCheckBox.Checked = false;
            } else {
                TransformationCheckBox.Checked = false;
                SubstringTransformationCheckBox.Checked = config.Transform.Active;
            }
            fsuipcMultiplyTextBox.Text = config.Transform.Expression;
            fsuipcBcdModeCheckBox.Checked = config.FSUIPC.BcdMode;
            fsuipcValueTextBox.Text = config.Value;

            // substring panel
            SubStringFromTextBox.Text = config.Transform.SubStrStart.ToString();
            SubStringToTextBox.Text = config.Transform.SubStrEnd.ToString();

            foreach (DataRow row in presetDataTable.Rows)
            {
                if ((row["settings"] as IFsuipcConfigItem).FSUIPC.Offset == config.FSUIPC.Offset &&
                    (row["settings"] as IFsuipcConfigItem).FSUIPC.OffsetType == config.FSUIPC.OffsetType &&
                    (row["settings"] as IFsuipcConfigItem).FSUIPC.Size == config.FSUIPC.Size &&
                    (row["settings"] as IFsuipcConfigItem).FSUIPC.Mask == config.FSUIPC.Mask &&
                    (row["settings"] as IFsuipcConfigItem).FSUIPC.BcdMode == config.FSUIPC.BcdMode
                    ) {
                    fsuipcPresetComboBox.Text = row["description"].ToString();
                    break;
                }
            }
        }

        internal void syncToConfig(IFsuipcConfigItem config)
        {
            config.FSUIPC.Offset = Int32.Parse(fsuipcOffsetTextBox.Text.Replace("0x", "").ToLower(), System.Globalization.NumberStyles.HexNumber);
            config.FSUIPC.OffsetType = (FSUIPCOffsetType)Enum.Parse(typeof(FSUIPCOffsetType), ((ListItem)(fsuipcOffsetTypeComboBox.SelectedItem)).Value);
            if (config.FSUIPC.OffsetType != FSUIPCOffsetType.String)
            {
                // the mask has only meaning for values other than strings
                config.FSUIPC.Mask = Int64.Parse(fsuipcMaskTextBox.Text.Replace("0x", "").ToLower(), System.Globalization.NumberStyles.HexNumber);
                config.FSUIPC.Size = Byte.Parse(fsuipcSizeComboBox.Text);
                config.Transform.Active = TransformationCheckBox.Checked;
            }
            else
            {
                // by default we set the string length to 255
                // because we don't offer an option for the string length yet
                config.FSUIPC.Size = 255;
                config.Transform.Active = SubstringTransformationCheckBox.Checked;
            }

            config.Transform.Expression = fsuipcMultiplyTextBox.Text;
            
            if (SubStringFromTextBox.Text!="")
                config.Transform.SubStrStart = Byte.Parse(SubStringFromTextBox.Text);
            if (SubStringToTextBox.Text != "")
                config.Transform.SubStrEnd = Byte.Parse(SubStringToTextBox.Text);
            
            config.FSUIPC.BcdMode = fsuipcBcdModeCheckBox.Checked;
            config.Value = fsuipcValueTextBox.Text;
        }

        internal InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.FsuipcOffsetInputAction config = new FsuipcOffsetInputAction();
            syncToConfig(config);

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
            // skip this test if the panel is not visible
            // because we have a string type offset
            if (!maskAndBcdPanel.Visible) return;

            try
            {
                _validatingHexFields(sender, e, int.Parse(fsuipcSizeComboBox.Text) * 2);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Parsing problem: {ex.Message}", LogSeverity.Error);
                displayError(sender as Control, ex.Message);
                e.Cancel = false;
            }
        }

        private void fsuipcMultiplyTextBox_Validating(object sender, CancelEventArgs e)
        {
            // do not validate when multiply panel is not visible
            // or disabled when dealing with strings
            if ((sender as TextBox).Name == fsuipcMultiplyTextBox.Name && (!multiplyPanel.Visible||!multiplyPanel.Visible)) return;

            return;
            // we should add a parse error test here
            // for the expression that is used.

            /*
            // this code snippet was prior to allowing
            // expressions in the multiply field

            try
            {
                float.Parse((sender as TextBox).Text);
                removeError(sender as Control);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Parsing problem: {ex.Message}", LogSeverity.Debug);
                displayError(sender as Control, i18n._tr("uiMessageFsuipcConfigPanelMultiplyWrongFormat"));
                e.Cancel = true;
            }
            */
        }

        private void fsuipcValueTextBox_Validating(object sender, CancelEventArgs e)
        {
            // do not validate when multiply panel is not visible
            if (!valuePanel.Visible) return;
            
            if ((sender as TextBox).Text.Trim() == "")
            {
                displayError(sender as Control, i18n._tr("uiMessageFsuipcConfigPanelNoValue"));
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
            MessageBox.Show(message, i18n._tr("Hint"));
        }

        private void removeError(Control control)
        {
            errorProvider.SetError(
                    control,
                    "");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            fsuipcMultiplyTextBox.Enabled = (sender as CheckBox).Checked;
        }
    }
}
