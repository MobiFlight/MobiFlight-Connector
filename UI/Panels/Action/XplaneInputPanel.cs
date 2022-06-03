using MobiFlight.InputConfig;
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
    public partial class XplaneInputPanel : UserControl
    {
        public XplaneInputPanel()
        {
            InitializeComponent();

            List<ListItem> listItems = new List<ListItem>();
            listItems.Add(new ListItem() { Value = XplaneInputAction.INPUT_TYPE_DATAREF, Label = XplaneInputAction.INPUT_TYPE_DATAREF });
            listItems.Add(new ListItem() { Value = XplaneInputAction.INPUT_TYPE_COMMAND, Label = XplaneInputAction.INPUT_TYPE_COMMAND });

            TypeComboBox.DataSource = listItems;
            TypeComboBox.ValueMember = "Value";
            TypeComboBox.DisplayMember = "Label";

            TypeComboBox.SelectedValueChanged += (sender, e) =>
            {
                PathLabel.Text = TypeComboBox.SelectedValue.ToString();
                HintLabel.Visible = ValueLabel.Visible = ValueTextBox.Visible = TypeComboBox.SelectedValue.ToString() == XplaneInputAction.INPUT_TYPE_DATAREF;
            };
        }
        internal void syncFromConfig(InputConfig.XplaneInputAction inputAction)
        {
            if (inputAction == null) inputAction = new InputConfig.XplaneInputAction();

            try
            {
                TypeComboBox.SelectedValue = inputAction.InputType;
            }
            catch (Exception)
            {
                TypeComboBox.SelectedValue = "DataRef";
            }

            NameTextBox.Text = inputAction.Path;
            ValueTextBox.Text = inputAction.Expression;
        }

        internal InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.XplaneInputAction result = new InputConfig.XplaneInputAction();
            result.InputType = TypeComboBox.SelectedValue.ToString();
            result.Path = NameTextBox.Text;
            result.Expression = ValueTextBox.Text;
            return result;
        }

    }
}
