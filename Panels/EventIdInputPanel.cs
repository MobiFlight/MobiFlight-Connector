using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.Panels
{
    public partial class EventIdInputPanel : UserControl
    {
        public EventIdInputPanel()
        {
            InitializeComponent();
        }
        
        internal void syncFromConfig(InputConfig.EventIdInputAction eventIdInputAction)
        {
            if (eventIdInputAction == null) return;
            eventIdTextBox.Text = eventIdInputAction.EventId.ToString();
            paramTextBox.Text = eventIdInputAction.Param.ToString();
        }

        internal InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.EventIdInputAction result = new InputConfig.EventIdInputAction();
            result.EventId = Int32.Parse(eventIdTextBox.Text);
            result.Param = Int32.Parse(paramTextBox.Text);
            return result;
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            MobiFlight.InputConfig.InputAction tmp = ToConfig();
            tmp.execute(null);
        }
    }
}