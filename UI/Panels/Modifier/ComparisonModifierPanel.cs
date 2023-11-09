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
    public partial class ComparisonModifierPanel : UserControl, IModifierConfigPanel
    {
        public event EventHandler ModifierChanged;

        public ComparisonModifierPanel()
        {
            InitializeComponent();
            comparisonValueTextBox.TextChanged += value_Changed;
            comparisonValueTextBox.Leave += value_Changed;

            comparisonIfValueTextBox.TextChanged += value_Changed;
            comparisonIfValueTextBox.Leave += value_Changed;

            comparisonElseValueTextBox.TextChanged += value_Changed;
            comparisonElseValueTextBox.Leave += value_Changed;

            comparisonOperandComboBox.TextChanged += value_Changed;
            comparisonOperandComboBox.Leave += value_Changed;
            comparisonOperandComboBox.SelectedIndexChanged += value_Changed;
        }

        public void fromConfig(ModifierBase c)
        {
            var config = c as Comparison;
            if (config == null) return;

            comparisonValueTextBox.Text = (config as Comparison).Value;

            if (!ComboBoxHelper.SetSelectedItem(comparisonOperandComboBox, config.Operand))
            {
                // TODO: provide error message
                Log.Instance.log($"Exception on selecting item in Comparison ComboBox.", LogSeverity.Error);
            }
            comparisonIfValueTextBox.Text = config.IfValue;
            comparisonElseValueTextBox.Text = config.ElseValue;
        }

        public ModifierBase toConfig()
        {
            var config = new Comparison();
            // comparison panel
            // config.Active = comparisonActiveCheckBox.Checked;
            config.Value = comparisonValueTextBox.Text;
            config.Operand = comparisonOperandComboBox.Text;
            config.IfValue = comparisonIfValueTextBox.Text;
            config.ElseValue = comparisonElseValueTextBox.Text;

            return config;
        }

        private void value_Changed(object sender, EventArgs e)
        {
                ModifierChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
