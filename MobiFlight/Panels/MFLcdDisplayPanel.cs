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
    public partial class MFLcddDisplayPanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;

        private Config.LcdDisplay config;
        bool initialized = false;

        public MFLcddDisplayPanel()
        {
            InitializeComponent();
        }

        public MFLcddDisplayPanel(Config.LcdDisplay config, List<byte> FreePins)
            : this()
        {
            // TODO: Complete member initialization
            this.config = config;
            NameTextBox.Text = config.Name;
            AddressTextBox.Text = "0x" + config.Address.ToString("X2");
            ColTextBox.Text = config.Cols.ToString();
            LinesTextBox.Text = config.Lines.ToString();

            setValues();

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            setValues();

            if (Changed!=null)
                Changed(config, new EventArgs());
        }

        private void setValues()
        {
            config.Name     = NameTextBox.Text;
            config.Address  = Byte.Parse(AddressTextBox.Text.Substring(2), System.Globalization.NumberStyles.HexNumber);
            config.Cols     = Byte.Parse(ColTextBox.Text);
            config.Lines    = Byte.Parse(LinesTextBox.Text);
        }
    }
}
