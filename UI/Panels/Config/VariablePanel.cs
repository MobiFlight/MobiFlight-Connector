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
        public event EventHandler ModifyTabLink;

        Dictionary<String, MobiFlightVariable> Variables = new Dictionary<String, MobiFlightVariable>();

        public VariablePanel()
        {
            InitializeComponent();
            NameTextBox.AutoCompleteMode = AutoCompleteMode.Append;

            // hide the string options.
            transformOptionsGroup1.setMode(true);
            transformOptionsGroup1.ModifyTabLink += (s, e) => {
                ModifyTabLink?.Invoke(this, e);
            };

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
