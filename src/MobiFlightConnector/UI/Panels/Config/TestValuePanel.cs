using MobiFlight.Modifier;
using SharpDX;
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
        public event EventHandler<EventArgs> TestModeStop;
        public event EventHandler TestModeEnd;
        public event EventHandler TestValueChanged;
        public string Result { get { return labelTestResultValue.Text; } set { 
                toolTipResultLabel.SetToolTip(labelTestResultValue, value);
                labelTestResultValue.Text = $"'{value}'"; } 
        }

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
            textBoxTestValue.TextChanged += (s, e) => { TestValueChanged?.Invoke(s, e); };

            labelTestResult.Enabled = labelTestResultValue.Enabled = false;
        }

        private void displayPinTestButton_Click(object sender, EventArgs e)
        {
            
            if (comboBoxTestValueType.SelectedItem == null) return;

            var result = CreateConnectorValue();

            TestModeStart?.Invoke(this, result);
            displayPinTestStopButton.Enabled = true;
            displayPinTestButton.Enabled = false;
            labelTestResult.Enabled = labelTestResultValue.Enabled = true;
        }

        private ConnectorValue CreateConnectorValue()
        {
            var result = new ConnectorValue();
            string type = (comboBoxTestValueType.SelectedItem as ListItem).Value;

            if (type == FSUIPCOffsetType.Float.ToString())
            {
                if (!double.TryParse(textBoxTestValue.Text, out result.Float64))
                {
                    return result;
                };
                result.type = FSUIPCOffsetType.Float;
            }
            else if (type == FSUIPCOffsetType.String.ToString())
            {
                result.type = FSUIPCOffsetType.String;
                result.String = textBoxTestValue.Text;
            }

            return result;
        }

        private void displayPinTestStopButton_Click(object sender, EventArgs e)
        {
            // check if running in test mode otherwise simply return
            if (!displayPinTestStopButton.Enabled) return;

            displayPinTestStopButton.Enabled = false;
            displayPinTestButton.Enabled = true;
            labelTestResult.Enabled = labelTestResultValue.Enabled = false;
            TestModeStop?.Invoke(this, EventArgs.Empty);
        }

        public void FromConfig(OutputConfigItem config)
        {
            if (config == null) return;
            if (!ComboBoxHelper.SetSelectedItemByValue(comboBoxTestValueType, config.TestValue.type.ToString())) {

            }
            textBoxTestValue.Text = config.TestValue.ToString();
        }

        internal void ToConfig(OutputConfigItem config)
        {
            config.TestValue = CreateConnectorValue();
        }
    }
}
