using System;
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
        public event EventHandler<EventArgs> ConfigRemoved;
        public ConfigRefPanelItem()
        {
            InitializeComponent();
            configRefComboBox.Text = "Select reference";
            checkBoxUse.Checked = true;
        }

        public void SetDataView(List<OutputConfigItem> outputConfigs)
        {
            configRefComboBox.DataSource = outputConfigs;
            configRefComboBox.ValueMember = "GUID";
            configRefComboBox.DisplayMember = "Name";
        }

        public void SetPlaceholder (String placeholder)
        {
            textBoxPlaceholder.Text = placeholder;
        }

        internal void syncFromConfig(ConfigRef config)
        {
            checkBoxUse.Checked = config.Active;
            
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
            catch (Exception ex)
            {
                // precondition could not be loaded, reference not valid anymore
                Log.Instance.log($"ConfigRef could not be loaded: {ex.Message}", LogSeverity.Error);
            }
            
            textBoxPlaceholder.Text = config.Placeholder;
            textBoxTestValue.Text = config.TestValue;
        }

        internal ConfigRef syncToConfig(ConfigRef config)
        {
            config.Active = checkBoxUse.Checked;
            if (configRefComboBox.SelectedValue != null)
                config.Ref = configRefComboBox.SelectedValue.ToString();
            config.Placeholder = textBoxPlaceholder.Text;
            config.TestValue = textBoxTestValue.Text.Trim();
            return config;
        }

        private void removeConfigReferenceButton_Click(object sender, EventArgs e)
        {
            ConfigRemoved?.Invoke(this, EventArgs.Empty);
        }
    }
}
