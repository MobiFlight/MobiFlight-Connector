using FSUIPC;
using MobiFlight.Base;
using MobiFlight.InputConfig;
using MobiFlight.OutputConfig;
using MobiFlight.UI.Forms;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace MobiFlight.UI.Panels.Config
{
    public partial class FsuipcConfigPanel : UserControl, IPanelConfigSync
    {
        public event EventHandler ModifyTabLink;
        public event EventHandler<IFsuipcConfigItem> PresetChanged;
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
            panelModifierHint.Visible = false;
        }

        public void setMode(bool isOutputPanel)
        {
            OutputPanelMode = isOutputPanel;
            transformOptionsGroup1.setMode(OutputPanelMode);
            
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
            panelModifierHint.Visible = false;
            if (fsuipcPresetComboBox.Text != "")
            {
                DataRow[] rows = presetDataTable.Select("description = '" + fsuipcPresetComboBox.Text + "'");
                if (rows.Length > 0)
                {
                    var config = rows[0]["settings"] as IFsuipcConfigItem;
                    syncFromConfig(config);                 
                    panelModifierHint.Visible = (config?.Modifiers.Items.Count > 0) && OutputPanelMode;
                    PresetChanged?.Invoke(this, config);
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
            }
        }

        public void syncFromConfig(object config)
        {
            var conf = config as IFsuipcConfigItem;
            
            if (conf == null)
            {
                // this happens when casting badly
                return;
            }
            
            // first tab                        
            fsuipcOffsetTextBox.Text = "0x" + conf.FSUIPC.Offset.ToString("X4");

            // preselect fsuipc offset type
            try
            {
                fsuipcOffsetTypeComboBox.SelectedValue = conf.FSUIPC.OffsetType.ToString();
            }
            catch (Exception ex)
            {
                // TODO: provide error message
                Log.Instance.log($"Exception on FSUIPCOffsetType.ToString: {ex.Message}", LogSeverity.Error);
            }

            if (!ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, conf.FSUIPC.Size.ToString()))
            {
                // TODO: provide error message
                Log.Instance.log("Exception on selecting item in ComboBox.", LogSeverity.Error);
            }

            // mask
            fsuipcMaskTextBox.Text = "0xFF";
            if (conf.FSUIPC.OffsetType != FSUIPCOffsetType.String)
                fsuipcMaskTextBox.Text = "0x" + conf.FSUIPC.Mask.ToString("X" + conf.FSUIPC.Size.ToString());
            
            fsuipcBcdModeCheckBox.Checked = conf.FSUIPC.BcdMode;
            transformOptionsGroup1.syncFromConfig(conf);

            foreach (DataRow row in presetDataTable.Rows)
            {
                var preset = row["settings"] as IFsuipcConfigItem;
                if (preset == null) continue;

                if (preset.FSUIPC.Equals(conf.FSUIPC)) 
                {
                    if (!preset.Modifiers.Items.FindAll(m=>m.Active).TrueForAll(m => conf.Modifiers.ContainsModifier(m))) continue;
                    // we found the preset
                    fsuipcPresetComboBox.Text = row["description"].ToString();
                    panelModifierHint.Visible = (row["settings"] as IFsuipcConfigItem).Modifiers.Items.Count > 0;
                    break;
                }
            }
        }

        internal void syncToConfig(OutputConfigItem config)
        {
            config.Source = new FsuipcSource();
            SyncOffsetToConfig((config.Source as FsuipcSource).FSUIPC);
            transformOptionsGroup1.syncToConfig(config);
        }

        internal void SyncOffsetToConfig(FsuipcOffset offset)
        {

            offset.Offset = Int32.Parse(fsuipcOffsetTextBox.Text.Replace("0x", "").ToLower(), System.Globalization.NumberStyles.HexNumber);
            offset.OffsetType = (FSUIPCOffsetType)Enum.Parse(typeof(FSUIPCOffsetType), ((ListItem)(fsuipcOffsetTypeComboBox.SelectedItem)).Value);

            if (offset.OffsetType != FSUIPCOffsetType.String)
            {
                // the mask has only meaning for values other than strings
                offset.Mask = Int64.Parse(fsuipcMaskTextBox.Text.Replace("0x", "").ToLower(), System.Globalization.NumberStyles.HexNumber);
                offset.Size = Byte.Parse(fsuipcSizeComboBox.Text);
            }   
            else
            {
                // by default we set the string length to 255
                // because we don't offer an option for the string length yet
                offset.Size = 255;
            }

            offset.BcdMode = fsuipcBcdModeCheckBox.Checked;
        }

        public InputConfig.InputAction ToConfig()
        {
            FsuipcOffsetInputAction config = new FsuipcOffsetInputAction();
            SyncOffsetToConfig(config.FSUIPC);
            transformOptionsGroup1.syncToConfig(config);
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

        private void ButtonModifyTab_Click(object sender, EventArgs e)
        {
            ModifyTabLink?.Invoke(this, EventArgs.Empty);
        }
    }
}
