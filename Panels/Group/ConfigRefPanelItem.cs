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
    public partial class ConfigRefPanelItem : UserControl
    {
        DataView dv;

        public ConfigRefPanelItem()
        {
            InitializeComponent();
            comboBox1.Text = "select reference";
        }

        public void SetDataView(DataView dv)
        {
            this.dv = dv;

            comboBox1.DataSource = dv;
            comboBox1.ValueMember = "guid";
            comboBox1.DisplayMember = "description";
        }

        public void SetPlaceholder (String placeholder)
        {
            textBox1.Text = placeholder;
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
            if (comboBox1.SelectedValue != null)
                config.Ref = comboBox1.SelectedValue.ToString();
            config.Placeholder = textBox1.Text;
            return config;
        }
    }
}
