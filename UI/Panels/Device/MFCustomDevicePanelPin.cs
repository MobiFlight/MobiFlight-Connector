﻿using MobiFlight.CustomDevices;
using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings.Device
{
    public partial class MFCustomDevicePanelPin : UserControl
    {
        private bool initialized = false;

        public event EventHandler Changed;

        public MFCustomDevicePanelPin()
        {
            InitializeComponent();
            comboBox0.Items.Clear();
            comboBox0.SelectedIndexChanged += value_Changed;
        }

        public MFCustomDevicePanelPin(String Label, List<MobiFlightPin> Pins, string pin) : this()
        {
            if (pin == null) pin = Pins[0].Pin.ToString();

            ComboBoxHelper.BindMobiFlightFreePins(comboBox0, Pins, pin);

            pinLabel.Text = Label;
            initialized = true;
        }

        public uint SelectedPin()
        {
            if (uint.TryParse(comboBox0.SelectedValue?.ToString(), out uint result))
            {
                return result;
            }
            else
            {
                Log.Instance.log($"Unable to convert `{comboBox0.SelectedValue}` to a uint", LogSeverity.Error);
                return 0;
            }
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            if (Changed != null)
                Changed(comboBox0, new EventArgs());
        }
    }
}
