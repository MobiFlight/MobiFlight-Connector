using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Action
{
    public partial class JeehellInputPanel : UserControl
    {
        public String PresetFile { get; set; }
        private DataTable Data;
        private DataRow currentRow;

        public JeehellInputPanel()
        {
            InitializeComponent();
            PresetFile = Properties.Settings.Default.PresetFileJeehell;
            Data = new DataTable();
            hintLabel.Visible = false;

            _loadPresets();
        }


        private void JeehellInputPanel_Load(object sender, EventArgs e)
        {

        }

        private void _loadPresets()
        {
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
                    Data.Columns.Add(new DataColumn("Label"));
                    Data.Columns.Add(new DataColumn("EventId"));
                    Data.Columns.Add(new DataColumn("Tooltip"));

                    string[] lines = System.IO.File.ReadAllLines(PresetFile);

                    foreach (string line in lines)
                    {
                        var cols = line.Split(':');
                        DataRow dr = Data.NewRow();
                        dr[0] = cols[0];
                        dr[1] = cols[1];
                        if (cols.Count() == 3) dr[2] = cols[2];
                        else dr[2] = "";

                        Data.Rows.Add(dr);
                    }

                    fsuipcPresetComboBox.Items.Clear();

                    foreach (DataRow row in Data.Rows)
                    {
                        fsuipcPresetComboBox.Items.Add(row["Label"]);
                    }

                    fsuipcPresetComboBox.SelectedIndex = 0;
                }
                catch (Exception e)
                {
                    isLoaded = false;
                    MessageBox.Show(i18n._tr("uiMessageConfigWizard_ErrorLoadingPresets"), i18n._tr("Hint"));
                }
            }

            fsuipcPresetComboBox.Enabled = isLoaded;
        }

        private void fsuipcPresetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fsuipcPresetComboBox.Text != "")
            {
                DataRow[] rows = Data.Select("Label = '" + fsuipcPresetComboBox.Text + "'");
                if (rows.Length > 0)
                {
                    currentRow = rows[0];

                    hintLabel.Visible = false;
                    if (rows[0]["Tooltip"].ToString() != "")
                    {
                        hintLabel.Text = rows[0]["Tooltip"].ToString();
                        hintLabel.Visible = true;
                    }
                }
            }
        }

        internal void syncFromConfig(InputConfig.JeehellInputAction jeehellInputAction)
        {
            if (jeehellInputAction == null) return;
            DataRow[] rows = Data.Select("EventId = '" + jeehellInputAction.EventId.ToString() + "'");
            if (rows.Length > 0)
            {
                ComboBoxHelper.SetSelectedItem(fsuipcPresetComboBox, rows[0]["Label"].ToString());

                if (jeehellInputAction!=null)
                ValueTextBox.Text = jeehellInputAction.Param;
            }
        }

        internal InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.JeehellInputAction result = new InputConfig.JeehellInputAction();
            if (currentRow != null)
            {
                result.EventId = Byte.Parse(currentRow["EventId"].ToString());
                result.Param = ValueTextBox.Text;
            }
            return result;
        }

    }


}
