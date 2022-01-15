using MobiFlight.UI.Panels.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Forms
{
    public partial class BitMaskEditorForm : Form
    {
        byte ByteSize = 1;
        private UInt64 _result = 0;
        public UInt64 Result { get { return _result; } }

        public BitMaskEditorForm() : this(1, 0)
        {            
        }

        public BitMaskEditorForm(byte ByteSize, UInt64 value)
        {
            this.ByteSize = ByteSize;            
            InitializeComponent();
            
            int PanelBorderWidth = Width - flowLayoutPanel.Width;
            int PanelBorderHeight = Height - flowLayoutPanel.Height - flowLayoutPanel.Margin.Bottom - panel.Margin.Top - panel.Height;

            for (int i = 8; i != this.ByteSize; i--)
            {
                flowLayoutPanel.Controls["bytePanel" + i].Dispose();
            }

            for (int i = 1; i <= this.ByteSize; i++)
            {
                byte currentValue = (byte)(value & byte.MaxValue);
                value = value >> 8;
                (flowLayoutPanel.Controls["bytePanel" + i] as BytePanel).Value = (byte) currentValue;
            }

            if (ByteSize != 8)
            {
                Width = (flowLayoutPanel.Width + PanelBorderWidth);

                panel.Left = (int)(Math.Floor((decimal)((flowLayoutPanel.Width - panel.Width) / 2)));
                panel.Top = flowLayoutPanel.Top + flowLayoutPanel.Height + 10;

                Height = panel.Top + panel.Height + PanelBorderHeight;
            }
        }

        private void BitMaskEditorForm_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            _result = 0;
            for (byte currentByte = 1; currentByte <= ByteSize; currentByte++)
            {
                _result += (UInt64) ((flowLayoutPanel.Controls["bytePanel" + currentByte] as BytePanel).Value << ((currentByte - 1) * 8));
            }
        }
    }
}
