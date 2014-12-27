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

namespace ArcazeUSB.Panels.Group
{
    public partial class FsuipcConfigPanel : UserControl
    {
        public FsuipcConfigPanel()
        {
            InitializeComponent();
            // if one opens the dialog for a new config
            // ensure that always the first tab is shown
            _initFsuipcOffsetTypeComboBox();
            _loadPresets();
            fsuipcPresetComboBox.ResetText();
        }

        public void setMode(bool isOutputPanel)
        {
            multiplayPanel.Visible = isOutputPanel;
        }

        private void _loadPresets()
        {
            bool isLoaded = true;

            if (!System.IO.File.Exists(Properties.Settings.Default.PresetFile))
            {
                isLoaded = false;
                MessageBox.Show(MainForm._tr("uiMessageConfigWizard_PresetsNotFound"), MainForm._tr("Hint"));
            }
            else
            {

                try
                {
                    presetsDataSet.Clear();
                    presetsDataSet.ReadXml(Properties.Settings.Default.PresetFile);
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
                    _syncConfigToForm(rows[0]["settings"] as ArcazeConfigItem);
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
            }
            catch (Exception exc)
            {
                e.Cancel = true;
                Log.Instance.log("_validatingHexFields : Parsing problem, " + exc.Message, LogSeverity.Debug);
                MessageBox.Show(MainForm._tr("uiMessageConfigWizard_ValidHexFormat"), MainForm._tr("Hint"));
            }
        }

        /// <summary>
        /// sync the values from config with the config wizard form
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected bool _syncConfigToForm(ArcazeConfigItem config)
        {
            string serial = null;
            if (config == null) throw new Exception(MainForm._tr("uiException_ConfigItemNotFound"));
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
                Log.Instance.log("_syncConfigToForm : Exception on FSUIPCOffsetType.ToString", LogSeverity.Debug);
            }

            if (!ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, config.FSUIPCSize.ToString()))
            {
                // TODO: provide error message
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in ComboBox", LogSeverity.Debug);
            }

            // mask
            fsuipcMaskTextBox.Text = "0x" + config.FSUIPCMask.ToString("X" + config.FSUIPCSize);

            return true;
        }

        private void fsuipcValueTextBox_TextChanged(object sender, EventArgs e)
        {

        }


        internal void syncFromConfig(MobiFlight.InputConfig.FsuipcOffsetInputAction config)
        {
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
                Log.Instance.log("_syncConfigToForm : Exception on FSUIPCOffsetType.ToString", LogSeverity.Debug);
            }

            if (!ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, config.FSUIPCSize.ToString()))
            {
                // TODO: provide error message
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in ComboBox", LogSeverity.Debug);
            }

            // mask
            fsuipcMaskTextBox.Text = "0x" + config.FSUIPCMask.ToString("X" + config.FSUIPCSize);

            // multiplier
            fsuipcMultiplyTextBox.Text = config.FSUIPCMultiplier.ToString();
            fsuipcBcdModeCheckBox.Checked = config.FSUIPCBcdMode;
            fsuipcValueTextBox.Text = config.InputValue;
        }

        internal MobiFlight.InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.FsuipcOffsetInputAction config = new FsuipcOffsetInputAction();

            config.FSUIPCMask = Int64.Parse(fsuipcMaskTextBox.Text.Replace("0x", "").ToLower(), System.Globalization.NumberStyles.HexNumber);
            config.FSUIPCOffset = Int32.Parse(fsuipcOffsetTextBox.Text.Replace("0x", "").ToLower(), System.Globalization.NumberStyles.HexNumber);
            config.FSUIPCSize = Byte.Parse(fsuipcSizeComboBox.Text);
            config.FSUIPCOffsetType = (FSUIPCOffsetType)Enum.Parse(typeof(FSUIPCOffsetType), ((ListItem)(fsuipcOffsetTypeComboBox.SelectedItem)).Value);
            //config.FSUIPCMultiplier = Double.Parse(fsuipcMultiplyTextBox.Text);
            config.FSUIPCBcdMode = fsuipcBcdModeCheckBox.Checked;
            config.InputValue = fsuipcValueTextBox.Text;
            return config;
        }
    }
}
