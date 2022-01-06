using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Action
{
    public partial class BaseVariablePanel : UserControl
    {
        Dictionary<String, MobiFlightVariable> Variables = new Dictionary<String, MobiFlightVariable>();

        public BaseVariablePanel()
        {
            InitializeComponent();
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
    }
}
