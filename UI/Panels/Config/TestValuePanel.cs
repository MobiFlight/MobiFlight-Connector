using MobiFlight.Modifier;
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
    public partial class TestValuePanel : UserControl
    {
        public event EventHandler<ConnectorValue> TestModeStart;
        public event EventHandler TestModeEnd;

        public TestValuePanel()
        {
            InitializeComponent();

            List<ListItem> list = new List<ListItem>() {
                new ListItem() { Value = FSUIPCOffsetType.Float.ToString(), Label = "Number" },
                new ListItem() { Value = FSUIPCOffsetType.String.ToString(), Label = "String" },
            };
            comboBoxTestValueType.DataSource = list;
            comboBoxTestValueType.ValueMember = "Value";
            comboBoxTestValueType.DisplayMember = "Label";
            comboBoxTestValueType.SelectedIndex = 0;
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            var result = new ConnectorValue();
            if (comboBoxTestValueType.SelectedItem == null) return;

            string type = (comboBoxTestValueType.SelectedItem as ListItem).Value;

            if (type == FSUIPCOffsetType.Float.ToString())
            {
                if (!double.TryParse(textBoxTestValue.Text, out result.Float64))
                {
                    return;
                };
                result.type = FSUIPCOffsetType.Float;
            }
            else if (type == FSUIPCOffsetType.String.ToString())
            {
                result.type = FSUIPCOffsetType.String;
                result.String = textBoxTestValue.Text;
            }

            TestModeStart?.Invoke(this, result);
        }
    }
}
