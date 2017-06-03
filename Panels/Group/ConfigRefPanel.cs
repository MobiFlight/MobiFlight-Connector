using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.Base;

namespace MobiFlight.Panels.Group
{
    public partial class ConfigRefPanel : UserControl
    {
        DataView dv;

        public ConfigRefPanel()
        {
            InitializeComponent();
        }

        public void SetDataView(DataView dv)
        {
            this.dv = dv;

            comboBox1.DataSource = dv;
            comboBox1.ValueMember = "guid";
            comboBox1.DisplayMember = "description";
        }
        internal void syncFromConfig(ConfigRef config)
        {
            checkBox1.Checked = config.Active;
            try {
                comboBox1.SelectedValue = config.Ref;
            }
            catch (Exception exc)
            {
                // precondition could not be loaded, reference not valid anymore
                Log.Instance.log("ConfigRefPanel.syncFromConfig : Precondition could not be loaded, " + exc.Message, LogSeverity.Debug);
            }
            textBox1.Text = config.Placeholder;
        }

        internal ConfigRef syncToConfig(ConfigRef config)
        {
            config.Active = checkBox1.Checked;
            config.Ref = comboBox1.SelectedValue.ToString();
            config.Placeholder = textBox1.Text;
            return config;
        }
    }
}
