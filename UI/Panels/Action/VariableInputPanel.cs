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

            List<ListItem> options = new List<ListItem>();
            options.Add(new ListItem() { Value = MobiFlightVariable.TYPE_NUMBER, Label = "Number" });
            options.Add(new ListItem() { Value = MobiFlightVariable.TYPE_STRING, Label = "String" });

            TypeComboBox.DisplayMember = "Label";
            TypeComboBox.ValueMember = "Value";
            TypeComboBox.DataSource = options;
            TypeComboBox.SelectedIndex = 0;
        }

        private void InitWithVariable(MobiFlightVariable Variable)
        {
            try
            {
                TypeComboBox.SelectedValue = Variable.TYPE;
            }
            catch (Exception)
            {
                TypeComboBox.SelectedValue = MobiFlightVariable.TYPE_NUMBER;
            }
            NameTextBox.Text = Variable.Name;
            ValueTextBox.Text = Variable.Expression;
        }

        internal void syncFromConfig(InputConfig.VariableInputAction inputAction)
        {
            if (inputAction == null) inputAction = new InputConfig.VariableInputAction();

            try
            {
                TypeComboBox.SelectedValue = inputAction.Variable.TYPE;
            }
            catch (Exception)
            {
                TypeComboBox.SelectedValue = MobiFlightVariable.TYPE_NUMBER;
            }
            NameTextBox.Text = inputAction.Variable.Name;
            ValueTextBox.Text = inputAction.Variable.Expression;
        }

        internal InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.VariableInputAction result = new InputConfig.VariableInputAction();
            result.Variable.TYPE = TypeComboBox.SelectedValue.ToString();
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