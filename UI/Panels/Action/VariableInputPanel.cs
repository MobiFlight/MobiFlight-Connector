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
    public partial class VariableInputPanel : UserControl
    {
        ErrorProvider errorProvider = new ErrorProvider();

        public VariableInputPanel()
        {
            InitializeComponent();
            InitWithVariable(new MobiFlightVariable());
        }

        private void InitWithVariable(MobiFlightVariable Variable)
        {
            ComboBoxHelper.SetSelectedItem(TypeComboBox, Variable.TYPE);
            NameTextBox.Text = Variable.Name;
            ValueTextBox.Text = Variable.Expression;
        }

        internal void syncFromConfig(InputConfig.VariableInputAction inputAction)
        {
            if (inputAction == null) inputAction = new InputConfig.VariableInputAction();

            ComboBoxHelper.SetSelectedItem(TypeComboBox, inputAction.Variable.TYPE);
            NameTextBox.Text = inputAction.Variable.Name;
            ValueTextBox.Text = inputAction.Variable.Expression;
        }

        internal InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.VariableInputAction result = new InputConfig.VariableInputAction();
            result.Variable.TYPE = TypeComboBox.Text;
            result.Variable.Name = NameTextBox.Text;
            result.Variable.Expression = ValueTextBox.Text;
            return result;
        }

        private void displayError(Control control, String message)
        {
            errorProvider.SetIconAlignment(control, ErrorIconAlignment.TopRight);
            errorProvider.SetError(
                    control,
                    message);
            MessageBox.Show(message, i18n._tr("Hint"));
        }

        private void removeError(Control control)
        {
            errorProvider.SetError(
                    control,
                    "");
        }

    }
}