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
    public partial class ButtonPanel : UserControl
    {
        MobiFlight.InputConfig.ButtonInputConfig _config;

        public ButtonPanel()
        {
            InitializeComponent();
            onPressActionTypePanel.ActionTypeChanged += new ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);
            onReleaseActionTypePanel.ActionTypeChanged += new ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);
        }

        // On Press Action
        private void onPressActionTypePanel_ActionTypeChanged(object sender, String value)
        {
            Control panel = null;
            Panel owner = onPressActionConfigPanel;
            bool isOnPress = (sender as ActionTypePanel) == onPressActionTypePanel;

            if (!isOnPress) owner = onReleaseActionConfigPanel;

            owner.Controls.Clear();
            switch (value)
            {
                case "FSUIPC Offset":
                    panel = new Panels.Group.FsuipcConfigPanel();
                    (panel as Panels.Group.FsuipcConfigPanel).setMode(false);


                    if (isOnPress && _config != null && _config.onPress != null) 
                        (panel as Panels.Group.FsuipcConfigPanel).syncFromConfig(_config.onPress as FsuipcOffsetInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as Panels.Group.FsuipcConfigPanel).syncFromConfig(_config.onRelease as FsuipcOffsetInputAction);

                    break;

                case "Key":
                    panel = new KeyboardInputPanel();
                    if (isOnPress && _config != null && _config.onPress != null)
                        (panel as Panels.KeyboardInputPanel).syncFromConfig(_config.onPress as KeyInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as Panels.KeyboardInputPanel).syncFromConfig(_config.onRelease as KeyInputAction);

                    break;

                case "Event ID":
                    panel = new EventIdInputPanel();
                    if (isOnPress && _config != null && _config.onPress != null)
                        (panel as Panels.EventIdInputPanel).syncFromConfig(_config.onPress as EventIdInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as Panels.EventIdInputPanel).syncFromConfig(_config.onRelease as EventIdInputAction);
  
                    break;

                case "Jeehell DataPipe":
                    panel = new JeehellInputPanel();
                    if (isOnPress && _config != null && _config.onPress != null)
                        (panel as Panels.JeehellInputPanel).syncFromConfig(_config.onPress as JeehellInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as Panels.JeehellInputPanel).syncFromConfig(_config.onRelease as JeehellInputAction);

                    break;
            }

            if (panel != null)
            {
                panel.Padding = new Padding(0, 0, 0, 0);
                panel.Width = owner.Width;
                owner.Controls.Add(panel);
                panel.Height = owner.Height - 3;
            }
        }

        public void syncFromConfig(MobiFlight.InputConfig.ButtonInputConfig config)
        {
            if (config == null) return;

            _config = config;

            if (_config.onPress != null)
            {
                onPressActionTypePanel.syncFromConfig(_config.onPress);
            }
            
            if (_config.onRelease != null)
            {
                onReleaseActionTypePanel.syncFromConfig(_config.onRelease);
            }
        }

        public void ToConfig(MobiFlight.InputConfig.ButtonInputConfig config)
        {
            // for on press check the action type
            if (onPressActionTypePanel.ActionTypeComboBox.SelectedItem != null)
            {
                switch (onPressActionTypePanel.ActionTypeComboBox.SelectedItem.ToString())
                {
                    case "FSUIPC Offset":
                        config.onPress = (onPressActionConfigPanel.Controls[0] as FsuipcConfigPanel).ToConfig();
                        break;

                    case "Key":
                        config.onPress = (onPressActionConfigPanel.Controls[0] as KeyboardInputPanel).ToConfig();
                        break;

                    case "Event ID":
                        config.onPress = (onPressActionConfigPanel.Controls[0] as EventIdInputPanel).ToConfig();
                        break;

                    case "Jeehell DataPipe":
                        config.onPress = (onPressActionConfigPanel.Controls[0] as JeehellInputPanel).ToConfig();
                        break;

                    default:
                        config.onPress = null;
                        break;
                }
            }

            if (onReleaseActionTypePanel.ActionTypeComboBox.SelectedItem != null)
            {
                switch (onReleaseActionTypePanel.ActionTypeComboBox.SelectedItem.ToString())
                {
                    case "FSUIPC Offset":
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as FsuipcConfigPanel).ToConfig();
                        break;

                    case "Key":
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as KeyboardInputPanel).ToConfig();
                        break;

                    case "Event ID":
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as EventIdInputPanel).ToConfig();
                        break;

                    case "Jeehell DataPipe":
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as JeehellInputPanel).ToConfig();
                        break;

                    default:
                        config.onRelease = null;
                        break;
                }
            }
        }
    }
}
