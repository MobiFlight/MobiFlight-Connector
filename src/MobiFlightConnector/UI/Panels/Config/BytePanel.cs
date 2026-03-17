using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{
    public partial class BytePanel : UserControl
    {
        byte _byte = 1;

        [Description("Set the Byte"), Category("Default")] 
        public byte Byte
        {
            get { return _byte; }
            set
            {
                _byte = value;
                Label.Text = "Byte " + _byte;
                for (int i = 0; i != 8; i++)
                {
                    flowLayoutPanel.Controls["checkBox" + i].Text = ((_byte-1) * 8 + i).ToString();
                }
            }
        }

        public byte Value
        {
            get { 
                byte result = 0;
                for (int i = 0; i != 8; i++)
                {
                    result += (Byte)(
                                ((flowLayoutPanel.Controls["checkBox" + i] as CheckBox).Checked ? 1 : 0) << i
                              );
                }
                return result;
            }
            set
            {
                for (int i = 0; i != 8; i++)
                {
                    (flowLayoutPanel.Controls["checkBox" + i] as CheckBox).Checked = ((value & 1) == 1);
                    value = (byte) (value >> 1);
                }
            }
        }
        public BytePanel() {
            InitializeComponent();            
        }

        private void btnSetAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i != 8; i++) {
                (flowLayoutPanel.Controls["checkBox" + i] as CheckBox).Checked = true;
            }
        }

        private void btnClrAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i != 8; i++) {
                (flowLayoutPanel.Controls["checkBox" + i] as CheckBox).Checked = false;
            }
        }
    }
}
