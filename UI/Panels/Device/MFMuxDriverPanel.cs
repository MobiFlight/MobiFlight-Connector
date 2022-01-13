using MobiFlight.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class MFMuxDriverPanel : UserControl
    {
        private MuxDriverS _muxDriver;
        private bool _initialized;
        public event EventHandler Changed;

        public MFMuxDriverPanel()
        {
            InitializeComponent();
            mfPinS0ComboBox.Items.Clear();
            mfPinS1ComboBox.Items.Clear();
            mfPinS2ComboBox.Items.Clear();
            mfPinS3ComboBox.Items.Clear();
        }

        public MFMuxDriverPanel(MuxDriverS muxDriver, List<MobiFlightPin> Pins, bool isEnabled = false) : this()
        {
            _muxDriver = muxDriver;

            SuspendLayout();

            ComboBoxHelper.BindMobiFlightFreePins(mfPinS0ComboBox, Pins, _muxDriver.PinSx[0]);
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS1ComboBox, Pins, _muxDriver.PinSx[1]);
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS2ComboBox, Pins, _muxDriver.PinSx[2]);
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS3ComboBox, Pins, _muxDriver.PinSx[3]);

            if (mfPinS0ComboBox.Items.Count > 3)
            {
                mfPinS0ComboBox.SelectedIndex = 0;
                mfPinS1ComboBox.SelectedIndex = 1;
                mfPinS2ComboBox.SelectedIndex = 2;
                mfPinS3ComboBox.SelectedIndex = 3;
            }

            ComboBoxHelper.SetSelectedItem(mfPinS0ComboBox, _muxDriver.PinSx[0]);
            ComboBoxHelper.SetSelectedItem(mfPinS1ComboBox, _muxDriver.PinSx[1]);
            ComboBoxHelper.SetSelectedItem(mfPinS2ComboBox, _muxDriver.PinSx[2]);
            ComboBoxHelper.SetSelectedItem(mfPinS3ComboBox, _muxDriver.PinSx[3]);

            enable(isEnabled);

            ResumeLayout();

            _initialized = true;
        }

        public void enable(bool status)
        {
            mfPinS0ComboBox.Enabled = status;
            mfPinS1ComboBox.Enabled = status;
            mfPinS2ComboBox.Enabled = status;
            mfPinS3ComboBox.Enabled = status;
            btnGotoSetting.Visible  = !status;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!_initialized) return;

            setValues();

            if (Changed != null)
                Changed(_muxDriver, new EventArgs());
        }

        private void setValues()
        {
            _muxDriver.PinSx[0] = mfPinS0ComboBox.Text;
            _muxDriver.PinSx[1] = mfPinS1ComboBox.Text;
            _muxDriver.PinSx[2] = mfPinS2ComboBox.Text;
            _muxDriver.PinSx[3] = mfPinS3ComboBox.Text;
        }

        private void btnGotoSetting_Click(object sender, EventArgs e)
        {
            //TODO
        }
    }
}