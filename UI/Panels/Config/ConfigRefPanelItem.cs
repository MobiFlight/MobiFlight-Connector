﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.Base;

namespace MobiFlight.UI.Panels.Config
{
    public partial class ConfigRefPanelItem : UserControl
    {
        public ConfigRefPanelItem()
        {
            InitializeComponent();
            configRefComboBox.Text = "Select reference";
        }

        public void SetDataView(DataView dv)
        {
            configRefComboBox.DataSource = dv;
            configRefComboBox.ValueMember = "guid";
            configRefComboBox.DisplayMember = "description";
        }

        public void SetPlaceholder (String placeholder)
        {
            textBox1.Text = placeholder;
        }

        internal void syncFromConfig(ConfigRef config)
        {
            checkBox1.Checked = config.Active;
            
            try {
                // if we have a null value, it might have happened
                // that we use to have a reference that got deleted
                // and the dialog was loaded and saved again.
                // then the Ref will be null.
                // in this case we don't preselect an item but we also
                // dont ignore the config entirely.
                if (config.Ref == null)
                {
                    configRefComboBox.SelectedIndex = -1;
                    configRefComboBox.Text = "Select reference";
                    errorProvider1.SetError(configRefComboBox, "No valid reference");
                }
                else {
                    configRefComboBox.SelectedValue = config.Ref;                    
                }

                if (configRefComboBox.SelectedValue == null)
                {
                    configRefComboBox.Text = "Select reference";
                };
            }
            catch (Exception exc)
            {
                // precondition could not be loaded, reference not valid anymore
                Log.Instance.log("ConfigRefPanel.syncFromConfig : ConfigRef could not be loaded, " + exc.Message, LogSeverity.Debug);
            }
            
            textBox1.Text = config.Placeholder;
        }

        internal ConfigRef syncToConfig(ConfigRef config)
        {
            config.Active = checkBox1.Checked;
            if (configRefComboBox.SelectedValue != null)
                config.Ref = configRefComboBox.SelectedValue.ToString();
            config.Placeholder = textBox1.Text;
            return config;
        }
    }
}
