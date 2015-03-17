using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight;
using MobiFlight.InputConfig;
using MobiFlight.Panels.Group;

namespace MobiFlight.Panels
{
    public partial class EncoderPanel : UserControl
    {
        MobiFlight.InputConfig.EncoderInputConfig _config;

        public EncoderPanel()
        {
            InitializeComponent();
            onLeftActionTypePanel.ActionTypeChanged += new ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);
            onLeftFastActionTypePanel.ActionTypeChanged += new ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);
            onRightActionTypePanel.ActionTypeChanged += new ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);
            onRightFastActionTypePanel.ActionTypeChanged += new ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);
        }

        // On Press Action
        private void onPressActionTypePanel_ActionTypeChanged(object sender, String value)
        {
            Control panel = null;
            Panel owner = onLeftActionConfigPanel;
            bool isLeft = ((sender as ActionTypePanel) == onLeftActionTypePanel) || ((sender as ActionTypePanel) == onLeftFastActionTypePanel);
            bool isFast = ((sender as ActionTypePanel) == onLeftFastActionTypePanel) || ((sender as ActionTypePanel) == onRightFastActionTypePanel);

            if (isLeft && isFast) owner = onLeftFastActionConfigPanel;
            else if (!isLeft)
            {
                owner = onRightActionConfigPanel;
                if (isFast) owner = onRightFastActionConfigPanel;
            }

            owner.Controls.Clear();
            switch (value)
            {
                case "FSUIPC Offset":
                    panel = new Panels.Group.FsuipcConfigPanel();
                    (panel as Panels.Group.FsuipcConfigPanel).setMode(false);

                    if (isLeft && !isFast && _config != null && _config.onLeft != null)
                        (panel as Panels.Group.FsuipcConfigPanel).syncFromConfig(_config.onLeft as FsuipcOffsetInputAction);
                    else if (isLeft && isFast && _config != null && _config.onLeftFast != null)
                        (panel as Panels.Group.FsuipcConfigPanel).syncFromConfig(_config.onLeftFast as FsuipcOffsetInputAction);
                    else if (!isLeft && !isFast && _config != null && _config.onRight != null)
                        (panel as Panels.Group.FsuipcConfigPanel).syncFromConfig(_config.onRight as FsuipcOffsetInputAction);
                    else if (!isLeft && isFast && _config != null && _config.onRightFast != null)
                        (panel as Panels.Group.FsuipcConfigPanel).syncFromConfig(_config.onRightFast as FsuipcOffsetInputAction);
                    break;

                case "Key":
                    throw new NotImplementedException("The support for Sending Keys is not yet implemented!");
            }

            if (panel != null)
            {
                panel.Padding = new Padding(0, 0, 0, 0);
                panel.Width = owner.Width;
                owner.Controls.Add(panel);
                panel.Height = owner.Height - 3;
            }
        }

        public void syncFromConfig(MobiFlight.InputConfig.EncoderInputConfig config)
        {
            if (config == null) return;

            _config = config;

            if (_config.onLeft != null)
            {
                onLeftActionTypePanel.syncFromConfig(_config.onLeft);
            }

            if (_config.onLeftFast != null)
            {
                onLeftFastActionTypePanel.syncFromConfig(_config.onLeftFast);
            }

            if (_config.onRight != null)
            {
                onRightActionTypePanel.syncFromConfig(_config.onRight);
            }

            if (_config.onRightFast != null)
            {
                onRightFastActionTypePanel.syncFromConfig(_config.onRightFast);
            }
        }

        public void ToConfig(MobiFlight.InputConfig.EncoderInputConfig config)
        {
            // for on press check the action type
            if (onLeftActionTypePanel.ActionTypeComboBox.SelectedItem != null)
            {
                switch (onLeftActionTypePanel.ActionTypeComboBox.SelectedItem.ToString())
                {
                    case "FSUIPC Offset":
                        config.onLeft = (onLeftActionConfigPanel.Controls[0] as FsuipcConfigPanel).ToConfig();
                        break;
                    case "Key":
                        throw new NotImplementedException("The support for Sending Keys is not yet implemented!");
                        //break;
                    default:
                        config.onLeft = null;
                        break;
                }
            }

            // for on fast press check the action type
            if (onLeftFastActionTypePanel.ActionTypeComboBox.SelectedItem != null)
            {
                switch (onLeftFastActionTypePanel.ActionTypeComboBox.SelectedItem.ToString())
                {
                    case "FSUIPC Offset":
                        config.onLeftFast = (onLeftFastActionConfigPanel.Controls[0] as FsuipcConfigPanel).ToConfig();
                        break;
                    case "Key":
                        throw new NotImplementedException("The support for Sending Keys is not yet implemented!");
                        //break;
                    default:
                        config.onLeftFast = null;
                        break;
                }
            }

            if (onRightActionTypePanel.ActionTypeComboBox.SelectedItem != null)
            {
                switch (onRightActionTypePanel.ActionTypeComboBox.SelectedItem.ToString())
                {
                    case "FSUIPC Offset":
                        config.onRight = (onRightActionConfigPanel.Controls[0] as FsuipcConfigPanel).ToConfig();
                        break;
                    case "Key":
                        throw new NotImplementedException("The support for Sending Keys is not yet implemented!");
                        //break;
                    default:
                        config.onRight = null;
                        break;
                }
            }

            if (onRightFastActionTypePanel.ActionTypeComboBox.SelectedItem != null)
            {
                switch (onRightFastActionTypePanel.ActionTypeComboBox.SelectedItem.ToString())
                {
                    case "FSUIPC Offset":
                        config.onRightFast = (onRightFastActionConfigPanel.Controls[0] as FsuipcConfigPanel).ToConfig();
                        break;
                    case "Key":
                        throw new NotImplementedException("The support for Sending Keys is not yet implemented!");
                        //break;
                    default:
                        config.onRightFast = null;
                        break;
                }
            }
        }
    }
}
