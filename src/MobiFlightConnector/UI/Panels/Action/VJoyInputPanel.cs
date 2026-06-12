using MobiFlight.InputConfig;
using MobiFlight.UI.Panels.Config;
using MobiFlight.VJoy;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Action
{
    public partial class VJoyInputPanel : UserControl, IPanelConfigSync
    {
        public VJoyInputPanel()
        {
            InitializeComponent();
            hintLabel.Visible = false;
            radioButtonOn.Enabled = false;
            radioButtonOff.Enabled = false;
            textBoxValue.Enabled = false;

            _loadPresets();
        }

        private void _loadPresets()
        {
            try
            {
                List<uint> availableVJoys = VJoyHelper.getAvailableVJoys();
                ComboBoxID.Items.Clear();
                foreach (uint id in availableVJoys)
                {
                    ComboBoxID.Items.Add(id);
                }
            }
            catch (VJoyNotEnabledException)
            {
                setNotEnabled();
            }
            comboBoxAxis.Enabled = false;
            comboBoxButtonNr.Enabled = false;

        }

        private void setNotEnabled()
        {
            ComboBoxID.Enabled = false;
            hintLabel.Text = "vJoy not Enabled";
            hintLabel.Visible = true;
            comboBoxAxis.Enabled = false;
            comboBoxButtonNr.Enabled = false;
        }

        public void syncFromConfig(object config)
        {
            VJoyInputAction vJoyInputAction = config as VJoyInputAction;
            if (vJoyInputAction == null) return;
            
            ComboBoxID.SelectedItem = vJoyInputAction.vJoyID;
            try
            {
                getVJoyInformation(vJoyInputAction.vJoyID);
            }
            catch (VJoyNotEnabledException ex)
            {
                setNotEnabled();
                return;
            }
            if (vJoyInputAction.buttonNr > 0)
            {
                comboBoxButtonNr.SelectedItem = vJoyInputAction.buttonNr;
                comboBoxAxis.SelectedIndex = 0;
                radioButtonOff.Checked = !vJoyInputAction.buttonComand;
                radioButtonOn.Checked = vJoyInputAction.buttonComand;
                setInputEnabledState(1);
                return;
            }
            if (vJoyInputAction.axisString != null)
            {
                comboBoxButtonNr.SelectedIndex = 0;
                comboBoxAxis.SelectedItem = vJoyInputAction.axisString;
                textBoxValue.Text = vJoyInputAction.sendValue;
                setInputEnabledState(2);
                return;
            }
            comboBoxAxis.SelectedIndex = 0;
            comboBoxButtonNr.SelectedIndex = 0;
            setInputEnabledState(-1);
        }

        public InputAction ToConfig()
        {
            if (ComboBoxID.SelectedItem != null)
            {
                VJoyInputAction vJoyInputAction = new VJoyInputAction();
                vJoyInputAction.vJoyID = UInt16.Parse(ComboBoxID.SelectedItem.ToString());
                if (comboBoxButtonNr.SelectedItem.ToString() != "--")
                {
                    vJoyInputAction.buttonNr = Int16.Parse(comboBoxButtonNr.SelectedItem.ToString());
                    vJoyInputAction.axisString = null;
                    vJoyInputAction.buttonComand = radioButtonOff.Checked ? false : true;
                    vJoyInputAction.sendValue = null;
                    return vJoyInputAction;
                }
                if (comboBoxAxis.SelectedIndex.ToString() != "--")
                {
                    vJoyInputAction.buttonNr = -1;
                    vJoyInputAction.axisString = comboBoxAxis.SelectedItem.ToString();
                    vJoyInputAction.buttonComand = false;
                    vJoyInputAction.sendValue = textBoxValue.Text;
                    return vJoyInputAction;
                }
                vJoyInputAction.buttonNr = -1;
                vJoyInputAction.axisString = null;
                vJoyInputAction.buttonComand = false;
                vJoyInputAction.sendValue = null;
                return vJoyInputAction;
            }else
            {
                return null;
            }
        }

        private void ComboBoxID_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                uint id = UInt16.Parse(ComboBoxID.SelectedItem.ToString());
                getVJoyInformation(id);
            }
            catch (VJoyNotEnabledException ex)
            {
                setNotEnabled();
            }

        }

        private void getVJoyInformation(uint id)
        {
            int vJoyButtons = VJoyHelper.getAvailableButtons(id);
            comboBoxButtonNr.Items.Clear();
            comboBoxButtonNr.Items.Add("--");
            for (int i = 1; i <= vJoyButtons; i++)
            {
                comboBoxButtonNr.Items.Add(i);
            }
            comboBoxButtonNr.Enabled = true;
            comboBoxButtonNr.SelectedIndex = 0;

            AxisState axState = VJoyHelper.getAvailableAxis(id);
            comboBoxAxis.Items.Clear();
            comboBoxAxis.Items.Add("--");
            if (axState.X) comboBoxAxis.Items.Add("X");
            if (axState.Y) comboBoxAxis.Items.Add("Y");
            if (axState.Z) comboBoxAxis.Items.Add("Z");
            if (axState.RX) comboBoxAxis.Items.Add("RX");
            if (axState.RY) comboBoxAxis.Items.Add("RY");
            if (axState.RZ) comboBoxAxis.Items.Add("RZ");
            comboBoxAxis.Enabled = true;
            comboBoxAxis.SelectedIndex = 0;
        }

        private void comboBoxButtonNr_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isSet = comboBoxButtonNr.SelectedIndex != 0;
            comboBoxAxis.Enabled = !isSet;
            setInputEnabledState(isSet ? 1 : -1);
        }

        private void comboBoxAxis_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isSet = comboBoxAxis.SelectedIndex != 0;
            comboBoxButtonNr.Enabled = !isSet;
            setInputEnabledState(isSet ? 2 : -1);
        }

        private void setInputEnabledState(int state)
        {
            switch (state)
            {
                case 1:
                    textBoxValue.Enabled = false;
                    radioButtonOn.Enabled = true;
                    radioButtonOff.Enabled = true;
                    break;
                case 2:
                    textBoxValue.Enabled = true;
                    radioButtonOn.Enabled = false;
                    radioButtonOff.Enabled = false;
                    break;
                default:
                    textBoxValue.Enabled = false;
                    radioButtonOn.Enabled = false;
                    radioButtonOff.Enabled = false;
                    break;
            }
        }
    }

}