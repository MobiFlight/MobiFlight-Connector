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
        private MuxDriver muxDriver;
        private bool initialized;
        public event EventHandler Changed;

        public MFMuxDriverPanel()
        {
            InitializeComponent();
            mfPinS0ComboBox.Items.Clear();
            mfPinS1ComboBox.Items.Clear();
            mfPinS2ComboBox.Items.Clear();
            mfPinS3ComboBox.Items.Clear();
        }

        public MFMuxDriverPanel(InputShiftRegister inputShiftRegister, List<MobiFlightPin> Pins) : this()
        {
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS0ComboBox, Pins, muxDriver.PinSx[0]);
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS1ComboBox, Pins, muxDriver.PinSx[1]);
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS2ComboBox, Pins, muxDriver.PinSx[2]);
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS3ComboBox, Pins, muxDriver.PinSx[3]);

            if (mfPinS0ComboBox.Items.Count > 3)
            {
                mfPinS0ComboBox.SelectedIndex = 0;
                mfPinS1ComboBox.SelectedIndex = 1;
                mfPinS2ComboBox.SelectedIndex = 2;
                mfPinS3ComboBox.SelectedIndex = 3;
            }

            this.muxDriver = muxDriver;

            ComboBoxHelper.SetSelectedItem(mfPinS0ComboBox, muxDriver.PinSx[0]);
            ComboBoxHelper.SetSelectedItem(mfPinS1ComboBox, muxDriver.PinSx[1]);
            ComboBoxHelper.SetSelectedItem(mfPinS2ComboBox, muxDriver.PinSx[2]);
            ComboBoxHelper.SetSelectedItem(mfPinS3ComboBox, muxDriver.PinSx[3]);

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            setValues();

            if (Changed != null)
                Changed(muxDriver, new EventArgs());
        }

        private void setValues()
        {
            muxDriver.PinSx[0] = mfPinS0ComboBox.Text;
            muxDriver.PinSx[1] = mfPinS1ComboBox.Text;
            muxDriver.PinSx[2] = mfPinS2ComboBox.Text;
            muxDriver.PinSx[3] = mfPinS3ComboBox.Text;
        }

        static public bool BindMobiFlightFreePins(ComboBox comboBox, List<MobiFlightPin> Pins, String CurrentPin)
        {
            List<MobiFlightPin> UsablePins = Pins.ConvertAll(pin => new MobiFlightPin(pin));
            if (UsablePins.Exists(x => x.Pin == byte.Parse(CurrentPin)))
                UsablePins.Find(x => x.Pin == byte.Parse(CurrentPin)).Used = false;

            comboBox.DataSource = UsablePins.FindAll(x => x.Used == false);
            comboBox.DisplayMember = "Name";
            comboBox.ValueMember = "Pin";

            return false;
        }
    }
}