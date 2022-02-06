﻿using MobiFlight.Config;
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
    public partial class MFInputShiftRegisterPanel : UserControl
    {
        private List<MobiFlightPin> pinList;    // COMPLETE list of pins (includes status)
        private InputShiftRegister inputShiftRegister;
        private bool initialized;
        private int MAX_MODULES = 4;

        public event EventHandler Changed;

        public MFInputShiftRegisterPanel()
        {
            InitializeComponent();
            mfPin1ComboBox.Items.Clear();
            mfPin2ComboBox.Items.Clear();
            mfPin3ComboBox.Items.Clear();
        }

        public MFInputShiftRegisterPanel(InputShiftRegister inputShiftRegister, List<MobiFlightPin> Pins) : this()
        {
            pinList = Pins; // Keep pin list stored

            this.inputShiftRegister = inputShiftRegister;
            update_lists();

            for (int i = 1; i <= MAX_MODULES; i++) {
                mfNumModulesComboBox.Items.Add(i);
            }

            ComboBoxHelper.SetSelectedItem(mfNumModulesComboBox, inputShiftRegister.NumModules);
            textBox1.Text = inputShiftRegister.Name;

            initialized = true;
        }

        private void setNonPinValues()
        {
            inputShiftRegister.Name = textBox1.Text;
            inputShiftRegister.NumModules = string.IsNullOrEmpty(mfNumModulesComboBox.Text) ? "1" : mfNumModulesComboBox.Text;
        }

        private void update_lists()
        {
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, pinList, inputShiftRegister.LatchPin);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin2ComboBox, pinList, inputShiftRegister.ClockPin);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin3ComboBox, pinList, inputShiftRegister.DataPin);
            initialized = exInitialized;
        }

        private void update_all(ComboBox comboBox)
        {
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events

            // First update the one that is changed
            // Here, the config data (shiftRegister.XXXPin) is updated with the new value read from the changed ComboBox;
            if (comboBox == mfPin1ComboBox) { ComboBoxHelper.reassignPin(mfPin1ComboBox, pinList, ref inputShiftRegister.LatchPin); } else
            if (comboBox == mfPin2ComboBox) { ComboBoxHelper.reassignPin(mfPin2ComboBox, pinList, ref inputShiftRegister.ClockPin); } else
            if (comboBox == mfPin3ComboBox) { ComboBoxHelper.reassignPin(mfPin3ComboBox, pinList, ref inputShiftRegister.DataPin); }
            // then the others are updated too 
            update_lists();

            initialized = exInitialized;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;
            update_all(sender as ComboBox);
            setNonPinValues();
            if (Changed != null)
                Changed(inputShiftRegister, new EventArgs());
        }

    }
}