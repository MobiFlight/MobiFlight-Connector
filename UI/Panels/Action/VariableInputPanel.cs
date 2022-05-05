using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.InputConfig;

namespace MobiFlight.UI.Panels.Action
{
    public partial class VariableInputPanel : UserControl
    {
        ErrorProvider errorProvider = new ErrorProvider();
        Dictionary<String, MobiFlightVariable> Variables = new Dictionary<String, MobiFlightVariable>();

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

            NameTextBox.AutoCompleteMode = AutoCompleteMode.Append;
        }

        public void SetVariableReferences(Dictionary<String, MobiFlightVariable> variables)
        {
            Variables = variables;
            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
            List<ListItem> options = new List<ListItem>();

            foreach (String key in variables.Keys)
            {
                collection.Add(variables[key].Name);
                options.Add(new ListItem() { Value = variables[key].Name, Label = variables[key].Name });
            }

            NameTextBox.DisplayMember = "Label";
            NameTextBox.ValueMember = "Value";
            NameTextBox.DataSource = options;

            NameTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            NameTextBox.AutoCompleteCustomSource = collection;

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

            // this can happen when we are 
            // copy & paste an input actions
            // where the var name was defined after
            // the panel was loaded.
            if (!Variables.ContainsKey(inputAction.Variable.Name))
            {
                Variables.Add(inputAction.Variable.Name, inputAction.Variable);
                SetVariableReferences(Variables);
            }

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

        private void NameTextBox_SelectedValueChanged(object sender, EventArgs e)
        {
            String key = (sender as ComboBox).SelectedValue as String;

            if (key == null) return;

            // lookup and check if the value is an existing preset
            if (!Variables.ContainsKey(key)) return;

            TypeComboBox.SelectedValue = Variables[key].TYPE;
        }
    }
}