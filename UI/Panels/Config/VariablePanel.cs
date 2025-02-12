using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{
    public partial class VariablePanel : UserControl
    {
        public event EventHandler ModifyTabLink;

        Dictionary<String, MobiFlightVariable> Variables = new Dictionary<String, MobiFlightVariable>();

        public VariablePanel()
        {
            InitializeComponent();
            NameTextBox.AutoCompleteMode = AutoCompleteMode.Append;

            // hide the string options.
            transformOptionsGroup1.setMode(true);

            List<ListItem> options = new List<ListItem>();
            options.Add(new ListItem() { Value = MobiFlightVariable.TYPE_NUMBER, Label = "Number" });
            options.Add(new ListItem() { Value = MobiFlightVariable.TYPE_STRING, Label = "String" });

            TypeComboBox.DisplayMember = "Label";
            TypeComboBox.ValueMember = "Value";
            TypeComboBox.DataSource = options;
            TypeComboBox.SelectedIndex = 0;
        }

        internal void SetVariableReferences(Dictionary<String, MobiFlightVariable> variables)
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

        internal void syncToConfig(OutputConfigItem config)
        {
            config.Source = new VariableSource()
            {
                MobiFlightVariable = new MobiFlightVariable()
                {
                    Name = NameTextBox.Text,
                    TYPE = TypeComboBox.SelectedValue.ToString()
                }
            };
            transformOptionsGroup1.syncToConfig(config);
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            if (!(config.Source is VariableSource)) return;

            try { 
                TypeComboBox.SelectedValue = (config.Source as VariableSource).MobiFlightVariable.TYPE;
            } catch (Exception)
            {
                TypeComboBox.SelectedValue = MobiFlightVariable.TYPE_NUMBER;
            }

            NameTextBox.Text = (config.Source as VariableSource).MobiFlightVariable.Name;
            transformOptionsGroup1.syncFromConfig(config);
        }

        private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
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
