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
        }

        internal void syncToConfig(OutputConfigItem config)
        {
            config.MobiFlightVariable.TYPE = TypeComboBox.Text;
            config.MobiFlightVariable.Name = NameTextBox.Text;
            transformOptionsGroup1.syncToConfig(config);
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            ComboBoxHelper.SetSelectedItem(TypeComboBox, config.MobiFlightVariable.TYPE);
            NameTextBox.Text = config.MobiFlightVariable.Name;
            transformOptionsGroup1.syncFromConfig(config);
        }
    }
}
