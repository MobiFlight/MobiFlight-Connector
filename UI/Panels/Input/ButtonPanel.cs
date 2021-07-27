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

namespace MobiFlight.UI.Panels.Input
{
    public partial class ButtonPanel : UserControl
    {
        InputConfig.ButtonInputConfig _config;

        public ButtonPanel()
        {
            InitializeComponent();
            onPressActionTypePanel.ActionTypeChanged += new MobiFlight.UI.Panels.Config.ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);
            onReleaseActionTypePanel.ActionTypeChanged += new MobiFlight.UI.Panels.Config.ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);
        }

        // On Press Action
        private void onPressActionTypePanel_ActionTypeChanged(object sender, String value)
        {
            Control panel = null;
            Panel owner = onPressActionConfigPanel;
            bool isOnPress = (sender as MobiFlight.UI.Panels.Config.ActionTypePanel) == onPressActionTypePanel;

            if (!isOnPress) owner = onReleaseActionConfigPanel;

            owner.Controls.Clear();
            switch (value)
            {
                case MobiFlight.InputConfig.FsuipcOffsetInputAction.Label:
                    panel = new Panels.Config.FsuipcConfigPanel();
                    (panel as Panels.Config.FsuipcConfigPanel).setMode(false);


                    if (isOnPress && _config != null && _config.onPress != null) 
                        (panel as Panels.Config.FsuipcConfigPanel).syncFromConfig(_config.onPress as FsuipcOffsetInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as Panels.Config.FsuipcConfigPanel).syncFromConfig(_config.onRelease as FsuipcOffsetInputAction);

                    break;

                case InputConfig.KeyInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.KeyboardInputPanel();
                    if (isOnPress && _config != null && _config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.KeyboardInputPanel).syncFromConfig(_config.onPress as KeyInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.KeyboardInputPanel).syncFromConfig(_config.onRelease as KeyInputAction);

                    break;

                case MobiFlight.InputConfig.EventIdInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.EventIdInputPanel();
                    if (isOnPress && _config != null && _config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.EventIdInputPanel).syncFromConfig(_config.onPress as EventIdInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.EventIdInputPanel).syncFromConfig(_config.onRelease as EventIdInputAction);
  
                    break;


                case MobiFlight.InputConfig.PmdgEventIdInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.PmdgEventIdInputPanel();
                    if (isOnPress && _config != null && _config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.PmdgEventIdInputPanel).syncFromConfig(_config.onPress as PmdgEventIdInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.PmdgEventIdInputPanel).syncFromConfig(_config.onRelease as PmdgEventIdInputAction);

                    break;

                case InputConfig.JeehellInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.JeehellInputPanel();
                    if (isOnPress && _config != null && _config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.JeehellInputPanel).syncFromConfig(_config.onPress as JeehellInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.JeehellInputPanel).syncFromConfig(_config.onRelease as JeehellInputAction);

                    break;

                case InputConfig.VJoyInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.VJoyInputPanel();
                    if (isOnPress && _config != null && _config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.VJoyInputPanel).syncFromConfig(_config.onPress as VJoyInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.VJoyInputPanel).syncFromConfig(_config.onRelease as VJoyInputAction);
                    break;

                case MobiFlight.InputConfig.LuaMacroInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.LuaMacroInputPanel();
                    if (isOnPress && _config != null && _config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.LuaMacroInputPanel).syncFromConfig(_config.onPress as LuaMacroInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.LuaMacroInputPanel).syncFromConfig(_config.onRelease as LuaMacroInputAction);

                    break;

                case MobiFlight.InputConfig.RetriggerInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.RetriggerInputPanel();
                    if (isOnPress && _config != null && _config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.RetriggerInputPanel).syncFromConfig(_config.onPress as RetriggerInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.RetriggerInputPanel).syncFromConfig(_config.onRelease as RetriggerInputAction);

                    break;

                case MobiFlight.InputConfig.MSFS2020EventIdInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.MSFS2020InputPanel();
                    if (isOnPress && _config != null && _config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.MSFS2020InputPanel).syncFromConfig(_config.onPress as MSFS2020EventIdInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.MSFS2020InputPanel).syncFromConfig(_config.onRelease as MSFS2020EventIdInputAction);

                    break;

                case MobiFlight.InputConfig.VariableInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.VariableInputPanel();
                    if (isOnPress && _config != null && _config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.VariableInputPanel).syncFromConfig(_config.onPress as VariableInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.VariableInputPanel).syncFromConfig(_config.onRelease as VariableInputAction);

                    break;

                case MobiFlight.InputConfig.MSFS2020CustomInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.MSFS2020CustomInputPanel();
                    if (isOnPress && _config != null && _config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.MSFS2020CustomInputPanel).syncFromConfig(_config.onPress as MSFS2020CustomInputAction);
                    else if (!isOnPress && _config != null && _config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.MSFS2020CustomInputPanel).syncFromConfig(_config.onRelease as MSFS2020CustomInputAction);

                    break;
            }

            if (panel != null)
            {
                panel.Padding = new Padding(0, 0, 0, 0);
                panel.Width = owner.Width;
                panel.Dock = DockStyle.Fill;
                owner.Controls.Add(panel);
                panel.Height = owner.Height - 3;
            }
        }

        public void syncFromConfig(InputConfig.ButtonInputConfig config)
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

        public void ToConfig(InputConfig.ButtonInputConfig config)
        {
            // for on press check the action type
            if (onPressActionTypePanel.ActionTypeComboBox.SelectedItem != null)
            {
                switch (onPressActionTypePanel.ActionTypeComboBox.SelectedItem.ToString())
                {
                    case MobiFlight.InputConfig.FsuipcOffsetInputAction.Label:
                        config.onPress = (onPressActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Config.FsuipcConfigPanel).ToConfig();
                        break;

                    case InputConfig.KeyInputAction.Label:
                        config.onPress = (onPressActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.KeyboardInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.EventIdInputAction.Label:
                        config.onPress = (onPressActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.EventIdInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.PmdgEventIdInputAction.Label:
                        config.onPress = (onPressActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.PmdgEventIdInputPanel).ToConfig();
                        break;

                    case InputConfig.JeehellInputAction.Label:
                        config.onPress = (onPressActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.JeehellInputPanel).ToConfig();
                        break;

                    case InputConfig.VJoyInputAction.Label:
                        config.onPress = (onPressActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.VJoyInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.LuaMacroInputAction.Label:
                        config.onPress = (onPressActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.LuaMacroInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.RetriggerInputAction.Label:
                        config.onPress = (onPressActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.RetriggerInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.MSFS2020EventIdInputAction.Label:
                        config.onPress = (onPressActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.MSFS2020InputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.MSFS2020CustomInputAction.Label:
                        config.onPress = (onPressActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.MSFS2020CustomInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.VariableInputAction.Label:
                        config.onPress = (onPressActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.VariableInputPanel).ToConfig();
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
                    case MobiFlight.InputConfig.FsuipcOffsetInputAction.Label:
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Config.FsuipcConfigPanel).ToConfig();
                        break;

                    case InputConfig.KeyInputAction.Label:
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.KeyboardInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.EventIdInputAction.Label:
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.EventIdInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.PmdgEventIdInputAction.Label:
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.PmdgEventIdInputPanel).ToConfig();
                        break;

                    case InputConfig.JeehellInputAction.Label:
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.JeehellInputPanel).ToConfig();
                        break;

                    case InputConfig.VJoyInputAction.Label:
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.VJoyInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.LuaMacroInputAction.Label:
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.LuaMacroInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.RetriggerInputAction.Label:
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.RetriggerInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.MSFS2020EventIdInputAction.Label:
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.MSFS2020InputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.MSFS2020CustomInputAction.Label:
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.MSFS2020CustomInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.VariableInputAction.Label:
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.VariableInputPanel).ToConfig();
                        break;

                    default:
                        config.onRelease = null;
                        break;
                }
            }
        }
    }
}
