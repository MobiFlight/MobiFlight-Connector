using MobiFlight.Modifier;
using MobiFlight.UI.Panels.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Modifier
{
    public partial class ComparisonModifier : UserControl
    {
        public ComparisonModifier()
        {
            InitializeComponent();
        }

        public void fromConfig(Comparison config)
        {
            // second tab
            comparisonActiveCheckBox.Checked = config.Active;
            comparisonValueTextBox.Text = config.Value;

            if (!ComboBoxHelper.SetSelectedItem(comparisonOperandComboBox, config.Operand))
            {
                // TODO: provide error message
                Log.Instance.log($"Exception on selecting item in Comparison ComboBox.", LogSeverity.Error);
            }
            comparisonIfValueTextBox.Text = config.IfValue;
            comparisonElseValueTextBox.Text = config.ElseValue;
        }

        public void toConfig(Comparison config)
        {   
            // comparison panel
            config.Active = comparisonActiveCheckBox.Checked;
            config.Value = comparisonValueTextBox.Text;
            config.Operand = comparisonOperandComboBox.Text;
            config.IfValue = comparisonIfValueTextBox.Text;
            config.ElseValue = comparisonElseValueTextBox.Text;
        }
    }
}
