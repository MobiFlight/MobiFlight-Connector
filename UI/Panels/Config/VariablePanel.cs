using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{
    public partial class VariablePanel : UserControl
    {
        public VariablePanel()
        {
            InitializeComponent();

            // hide the string options.
            transformOptionsGroup1.setMode(true);
            transformOptionsGroup1.ShowSubStringPanel(false);

            List<ListItem> options = new List<ListItem>();
            options.Add(new ListItem() { Value = MobiFlightVariable.TYPE_NUMBER, Label = "Number" });
            options.Add(new ListItem() { Value = MobiFlightVariable.TYPE_STRING, Label = "String" });

            TypeComboBox.DisplayMember = "Label";
            TypeComboBox.ValueMember = "Value";
            TypeComboBox.DataSource = options;
            TypeComboBox.SelectedIndex = 0;
        }

        internal void syncToConfig(OutputConfigItem config)
        {
            config.MobiFlightVariable.TYPE = TypeComboBox.SelectedValue.ToString();
            config.MobiFlightVariable.Name = NameTextBox.Text;
            transformOptionsGroup1.syncToConfig(config);
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            try { 
                TypeComboBox.SelectedValue = config.MobiFlightVariable.TYPE;
            } catch (Exception)
            {
                TypeComboBox.SelectedValue = MobiFlightVariable.TYPE_NUMBER;
            }

            NameTextBox.Text = config.MobiFlightVariable.Name;
            transformOptionsGroup1.syncFromConfig(config);
        }

        private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            transformOptionsGroup1.ShowMultiplyPanel(MobiFlightVariable.TYPE_NUMBER == (sender as ComboBox).SelectedValue.ToString());
            transformOptionsGroup1.ShowSubStringPanel(MobiFlightVariable.TYPE_STRING == (sender as ComboBox).SelectedValue.ToString());
        }
    }
}
