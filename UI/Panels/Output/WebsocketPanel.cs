using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class WebsocketPanel : UserControl
    {
        public WebsocketPanel()
        {
            InitializeComponent();
        }

        public void syncFromConfig(OutputConfigItem config)
        {
            payloadTextBox.Text = config.WebsocketOutput.Payload;
        }

        public OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            config.WebsocketOutput.Payload = payloadTextBox.Text;

            return config;
        }
    }
}
