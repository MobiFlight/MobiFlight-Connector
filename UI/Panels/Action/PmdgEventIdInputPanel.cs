using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.InputConfig;

namespace MobiFlight.UI.Panels.Action
{
    public partial class PmdgEventIdInputPanel : UserControl
    {
        static PmdgEventIdInputAction.PmdgAircraftType lastUsedType = PmdgEventIdInputAction.PmdgAircraftType.B737;
        public String PresetFile { get; set; }
        private DataTable Data;
        ErrorProvider errorProvider = new ErrorProvider();
        List<ListItem> MouseParams = new List<ListItem>();

        public PmdgEventIdInputPanel()
        {
            InitializeComponent();

            Data = new DataTable();
            _updateRadioButtons();
            //_loadPresets();
            _loadMouseParams();
        }

        private void _updateRadioButtons()
        {
            pmdg737radioButton.Checked = (lastUsedType == PmdgEventIdInputAction.PmdgAircraftType.B737);
            pmdg777radioButton.Checked = (lastUsedType == PmdgEventIdInputAction.PmdgAircraftType.B777);
            pmdg747radioButton.Checked = (lastUsedType == PmdgEventIdInputAction.PmdgAircraftType.B747);
        }

        private void _loadMouseParams()
        {
            MouseParams.Clear();
            MouseParams.Add(new ListItem() { Label = i18n._tr("uiPmdgEventIdInputPanelCustomParam"), Value = "0" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_RIGHTSINGLE", Value = "2147483648" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_MIDDLESINGLE", Value = "1073741824" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_LEFTSINGLE", Value = "536870912" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_RIGHTDOUBLE", Value = "268435456" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_MIDDLEDOUBLE", Value = "134217728" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_LEFTDOUBLE", Value = "67108864" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_RIGHTDRAG", Value = "33554432" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_MIDDLEDRAG", Value = "16777216" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_LEFTDRAG", Value = "8388608" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_MOVE", Value = "4194304" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_DOWN_REPEAT", Value = "2097152" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_RIGHTRELEASE", Value = "524288" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_MIDDLERELEASE", Value = "262144" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_LEFTRELEASE", Value = "131072" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_WHEEL_FLIP", Value = "65563" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_WHEEL_SKIP", Value = "32768" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_WHEEL_UP", Value = "16384" });
            MouseParams.Add(new ListItem() { Label = "MOUSE_FLAG_WHEEL_DOWN", Value = "8192" });

            MouseEventComboBox.DataSource = MouseParams;
            MouseEventComboBox.ValueMember = "Value";
            MouseEventComboBox.DisplayMember = "Label";
        }


        private void _loadPresets()
        {
            PresetFile = Properties.Settings.Default.PresetFilePmdg737EventId;
            if (lastUsedType == PmdgEventIdInputAction.PmdgAircraftType.B777)
                PresetFile = Properties.Settings.Default.PresetFilePmdg777EventId;
            else if (lastUsedType == PmdgEventIdInputAction.PmdgAircraftType.B747)
                PresetFile = Properties.Settings.Default.PresetFilePmdg747EventId;

            bool isLoaded = true;

            if (!System.IO.File.Exists(PresetFile))
            {
                isLoaded = false;
                MessageBox.Show(i18n._tr("uiMessageConfigWizard_PresetsNotFound"), i18n._tr("Hint"));
            }
            else
            {

                try
                {
                    Data.Clear();
                    Data.Columns.Clear();
                    Data.Columns.Add(new DataColumn("Label"));
                    Data.Columns.Add(new DataColumn("EventID"));
                    Data.Columns.Add(new DataColumn("Param"));

                    string[] lines = System.IO.File.ReadAllLines(PresetFile);

                    foreach (string line in lines)
                    {
                        var cols = line.Split(':');

                        DataRow dr = Data.NewRow();
                        dr[0] = cols[0];
                        dr[1] = cols[1];
                        if (cols.Count() == 3) dr[2] = cols[2];
                        else dr[2] = 0;

                        Data.Rows.Add(dr);
                    }

                    fsuipcPresetComboBox.Items.Clear();

                    foreach (DataRow row in Data.Rows)
                    {
                        if (row["EventID"].ToString() != "GROUP")
                            fsuipcPresetComboBox.Items.Add(row["Label"]);

                        // Maybe I add grouping later.
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
        
        internal void syncFromConfig(InputConfig.PmdgEventIdInputAction eventIdInputAction)
        {
            if (eventIdInputAction == null) return;
            eventIdTextBox.Text = eventIdInputAction.EventId.ToString();

            if (lastUsedType!=eventIdInputAction.AircraftType)
            {
                lastUsedType = eventIdInputAction.AircraftType;
                _updateRadioButtons();
                _loadPresets();
            }

            try { 
                MouseEventComboBox.SelectedValue = eventIdInputAction.Param.ToString();
            } catch (Exception e)
            {
                MouseEventComboBox.SelectedValue = "0"; // this is the custom param default value.
                // this will fail if we have a custom param that doesn't show in the list.
            }
            customParamTextBox.Text = eventIdInputAction.Param.ToString();

            foreach (DataRow row in Data.Rows)
            {
                if (row["EventID"] as String == eventIdInputAction.EventId.ToString())
                {
                    fsuipcPresetComboBox.Text = row["Label"].ToString();
                    break;
                }
            }
        }

        internal InputConfig.PmdgEventIdInputAction ToConfig()
        {
            MobiFlight.InputConfig.PmdgEventIdInputAction result = new InputConfig.PmdgEventIdInputAction();
            result.EventId = Int32.Parse(eventIdTextBox.Text);
            result.Param = customParamTextBox.Text;
            result.AircraftType = lastUsedType;
            return result;
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            MobiFlight.InputConfig.InputAction tmp = ToConfig();
            tmp.execute(null, null, null);
        }

        private void fsuipcPresetUseButton_Click(object sender, EventArgs e)
        {
            if (fsuipcPresetComboBox.Text != "")
            {
                DataRow[] rows = Data.Select("Label = '" + fsuipcPresetComboBox.Text + "'");
                if (rows.Length > 0)
                {
                    eventIdTextBox.Text = rows[0]["EventId"].ToString();
                    //paramTextBox.Text = rows[0]["Param"].ToString();
                }
            }
        }

        private void eventIdTextBox_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (!tb.Visible) return;

            String errorMessage = "";

            try
            {
                Int32.Parse(tb.Text);
            } catch (FormatException ex)
            {
                e.Cancel = true;
                errorMessage = ex.Message;
            }
            catch (OverflowException ex)
            {
                errorMessage = ex.Message;
                e.Cancel = true;
            }

            if (e.Cancel)
            {
                Log.Instance.log($"EventID/Param parsing problem: {errorMessage}", LogSeverity.Error);
                displayError(
                    sender as Control, 
                    String.Format(
                        i18n._tr("uiMessageValidNumberInRange"),
                        Int32.MinValue.ToString(), 
                        Int32.MaxValue.ToString()
                    )
                );
            } else
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

        private void MouseEventComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (sender as ComboBox);
            String selectedValue = cb.SelectedValue.ToString();
            customParamLabel.Visible = (selectedValue == "0");
            customParamTextBox.Visible = (selectedValue == "0");

            customParamTextBox.Text = selectedValue;
        }

        private void pmdg737radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton cb = (sender as RadioButton);
            if (!cb.Checked) return;

            lastUsedType = PmdgEventIdInputAction.PmdgAircraftType.B737;
            if (cb == pmdg777radioButton)
                lastUsedType = PmdgEventIdInputAction.PmdgAircraftType.B777;
            else if (cb == pmdg747radioButton)
                lastUsedType = PmdgEventIdInputAction.PmdgAircraftType.B747;

            _updateRadioButtons();
            _loadPresets();
        }
    }
}