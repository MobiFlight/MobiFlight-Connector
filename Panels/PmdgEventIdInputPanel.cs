using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.Panels
{
    public partial class PmdgEventIdInputPanel : UserControl
    {
        public String PresetFile { get; set; }
        private DataTable Data;
        ErrorProvider errorProvider = new ErrorProvider();
        List<ListItem> MouseParams = new List<ListItem>();

        public enum AircraftType { B737, B777 };

        public PmdgEventIdInputPanel(AircraftType type)
        {
            InitializeComponent();
            PresetFile = Properties.Settings.Default.Pmdg737EventIdPresetFile;
            if (type==AircraftType.B777)
                PresetFile = Properties.Settings.Default.Pmdg777EventIdPresetFile;

            Data = new DataTable();
            _loadPresets();
            _loadMouseParams();
        }

        private void _loadMouseParams()
        {
            MouseParams.Clear();
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

        private void EventIdInputPanel_Load(object sender, EventArgs e)
        {

        }

        private void _loadPresets()
        {
            bool isLoaded = true;

            if (!System.IO.File.Exists(PresetFile))
            {
                isLoaded = false;
                MessageBox.Show(MainForm._tr("uiMessageConfigWizard_PresetsNotFound"), MainForm._tr("Hint"));
            }
            else
            {

                try
                {
                    Data.Clear();
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
                    MessageBox.Show(MainForm._tr("uiMessageConfigWizard_ErrorLoadingPresets"), MainForm._tr("Hint"));
                }
            }

            fsuipcPresetComboBox.Enabled = isLoaded;
            fsuipcPresetUseButton.Enabled = isLoaded;
        }
        
        internal void syncFromConfig(InputConfig.PmdgEventIdInputAction eventIdInputAction)
        {
            if (eventIdInputAction == null) return;
            eventIdTextBox.Text = eventIdInputAction.EventId.ToString();
            MouseEventComboBox.SelectedValue = eventIdInputAction.Param.ToString();

            foreach (DataRow row in Data.Rows)
            {
                if (row["EventID"] as String == eventIdInputAction.EventId.ToString())
                {
                    fsuipcPresetComboBox.Text = row["Label"].ToString();
                    break;
                }
            }
        }

        internal InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.PmdgEventIdInputAction result = new InputConfig.PmdgEventIdInputAction();
            result.EventId = Int32.Parse(eventIdTextBox.Text);
            result.Param = UInt32.Parse(MouseEventComboBox.SelectedValue.ToString());
            return result;
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            MobiFlight.InputConfig.InputAction tmp = ToConfig();
            tmp.execute(null, null);
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
            } catch (FormatException fEx)
            {
                e.Cancel = true;
                errorMessage = fEx.Message;

            } catch (OverflowException oEx)
            {
                errorMessage = oEx.Message;
                e.Cancel = true;
            }

            if (e.Cancel)
            {
                Log.Instance.log("EventID/Param : Parsing problem, " + errorMessage, LogSeverity.Error);
                displayError(
                    sender as Control, 
                    String.Format(
                        MainForm._tr("uiMessageConfigWizard_ValidNumberInRange"),
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