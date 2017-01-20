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
    public partial class EventIdInputPanel : UserControl
    {
        public String PresetFile { get; set; }
        private DataTable Data;
        ErrorProvider errorProvider = new ErrorProvider();

        public EventIdInputPanel()
        {
            InitializeComponent();
            PresetFile = Properties.Settings.Default.EventIdPresetFile;
            Data = new DataTable();
            _loadPresets();
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
                        fsuipcPresetComboBox.Items.Add(row["Label"]);
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
        
        internal void syncFromConfig(InputConfig.EventIdInputAction eventIdInputAction)
        {
            if (eventIdInputAction == null) return;
            eventIdTextBox.Text = eventIdInputAction.EventId.ToString();
            paramTextBox.Text = eventIdInputAction.Param.ToString();
        }

        internal InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.EventIdInputAction result = new InputConfig.EventIdInputAction();
            result.EventId = Int32.Parse(eventIdTextBox.Text);
            result.Param = Int32.Parse(paramTextBox.Text);
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
                    paramTextBox.Text = rows[0]["Param"].ToString();
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