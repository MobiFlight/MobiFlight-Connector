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
using MobiFlight.UI.Panels.Config;

namespace MobiFlight.UI.Panels.Input
{
    public partial class ButtonPanel : UserControl
    {
        public event EventHandler<EventArgs> OnPanelChanged;
        Dictionary<String, MobiFlightVariable> Variables = new Dictionary<String, MobiFlightVariable>();
        InputConfig.ButtonInputConfig _config;

        private void clipBoardActionChanged(InputAction action)
        {
            onPressActionTypePanel.OnClipBoardChanged(action);
            onReleaseActionTypePanel.OnClipBoardChanged(action);
        }
        public ButtonPanel()
        {
            InitializeComponent();
            onPressActionTypePanel.ActionTypeChanged += new MobiFlight.UI.Panels.Config.ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);
            onReleaseActionTypePanel.ActionTypeChanged += new MobiFlight.UI.Panels.Config.ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);


            List<ActionTypePanel> panels = new List<ActionTypePanel>() { 
                onPressActionTypePanel, onReleaseActionTypePanel
            };
            panels.ForEach(action =>
            {
                action.CopyButtonPressed += Action_CopyButtonPressed;
                action.PasteButtonPressed += Action_PasteButtonPressed;
            });

            Clipboard.Instance.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName != "InputAction") return;

                clipBoardActionChanged(Clipboard.Instance.InputAction);
            };

            // activates the paste button
            // in case that we have something in the clipboard
            if (Clipboard.Instance.InputAction != null)
            {
                clipBoardActionChanged(Clipboard.Instance.InputAction);
            }
        }

        private void Action_CopyButtonPressed(object sender, EventArgs e)
        {
            InputConfig.ButtonInputConfig config = new ButtonInputConfig();
            ToConfig(config);
            if ((sender as ActionTypePanel) == onPressActionTypePanel) { 
                Clipboard.Instance.InputAction = config.onPress;
            }
            if ((sender as ActionTypePanel) == onReleaseActionTypePanel)
            {
                Clipboard.Instance.InputAction = config.onRelease;
            }
        }

        private void Action_PasteButtonPressed(object sender, EventArgs e)
        {
            Panel owner = null;

            (sender as ActionTypePanel).syncFromConfig(Clipboard.Instance.InputAction);

            bool isPress = (sender as ActionTypePanel) == onPressActionTypePanel;

            InputConfig.ButtonInputConfig config = new ButtonInputConfig();
            if (isPress)
            {
                owner = onPressActionConfigPanel; 
                config.onPress = Clipboard.Instance.InputAction;
            }
            else
            {
                owner = onReleaseActionConfigPanel;
                config.onRelease = Clipboard.Instance.InputAction;
            }

            String value = null;
            String type = Clipboard.Instance.InputAction.GetType().ToString();

            if (type == "MobiFlight.InputConfig.FsuipcOffsetInputAction") value = MobiFlight.InputConfig.FsuipcOffsetInputAction.Label;
            else if (type == "MobiFlight.InputConfig.KeyInputAction") value = MobiFlight.InputConfig.KeyInputAction.Label;
            else if (type == "MobiFlight.InputConfig.EventIdInputAction") value = MobiFlight.InputConfig.EventIdInputAction.Label;
            else if (type == "MobiFlight.InputConfig.PmdgEventIdInputAction") value = MobiFlight.InputConfig.PmdgEventIdInputAction.Label;
            else if (type == "MobiFlight.InputConfig.JeehellInputAction") value = MobiFlight.InputConfig.JeehellInputAction.Label;
            else if (type == "MobiFlight.InputConfig.VJoyInputAction") value = MobiFlight.InputConfig.VJoyInputAction.Label;
            else if (type == "MobiFlight.InputConfig.LuaMacroInputAction") value = MobiFlight.InputConfig.LuaMacroInputAction.Label;
            else if (type == "MobiFlight.InputConfig.RetriggerInputAction") value = MobiFlight.InputConfig.RetriggerInputAction.Label;
            else if (type == "MobiFlight.InputConfig.MSFS2020EventIdInputAction") value = MobiFlight.InputConfig.MSFS2020CustomInputAction.Label;
            else if (type == "MobiFlight.InputConfig.MSFS2020CustomInputAction") value = MobiFlight.InputConfig.MSFS2020CustomInputAction.Label;
            else if (type == "MobiFlight.InputConfig.VariableInputAction") value = MobiFlight.InputConfig.VariableInputAction.Label;
            else if (type == "MobiFlight.InputConfig.XplaneInputAction") value = MobiFlight.InputConfig.XplaneInputAction.Label;

            UpdatePanelWithAction(owner, config, value, isPress);
        }

        private void UpdatePanelWithAction(Panel owner, ButtonInputConfig config, string value, bool isOnPress)
        {
            this.SuspendLayout();
            Control panel = null;
            owner.Controls.Clear();
            switch (value)
            {
                case MobiFlight.InputConfig.FsuipcOffsetInputAction.Label:
                    panel = new Panels.Config.FsuipcConfigPanel();
                    (panel as Panels.Config.FsuipcConfigPanel).setMode(false);


                    if (isOnPress && config != null && config.onPress != null)
                        (panel as Panels.Config.FsuipcConfigPanel).syncFromConfig(config.onPress as FsuipcOffsetInputAction);
                    else if (!isOnPress && config != null && config.onRelease != null)
                        (panel as Panels.Config.FsuipcConfigPanel).syncFromConfig(config.onRelease as FsuipcOffsetInputAction);

                    break;

                case InputConfig.KeyInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.KeyboardInputPanel();
                    if (isOnPress && config != null && config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.KeyboardInputPanel).syncFromConfig(config.onPress as KeyInputAction);
                    else if (!isOnPress && config != null && config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.KeyboardInputPanel).syncFromConfig(config.onRelease as KeyInputAction);

                    break;

                case MobiFlight.InputConfig.EventIdInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.EventIdInputPanel();
                    if (isOnPress && config != null && config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.EventIdInputPanel).syncFromConfig(config.onPress as EventIdInputAction);
                    else if (!isOnPress && config != null && config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.EventIdInputPanel).syncFromConfig(config.onRelease as EventIdInputAction);

                    break;


                case MobiFlight.InputConfig.PmdgEventIdInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.PmdgEventIdInputPanel();
                    if (isOnPress && config != null && config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.PmdgEventIdInputPanel).syncFromConfig(config.onPress as PmdgEventIdInputAction);
                    else if (!isOnPress && config != null && config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.PmdgEventIdInputPanel).syncFromConfig(config.onRelease as PmdgEventIdInputAction);

                    break;

                case InputConfig.JeehellInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.JeehellInputPanel();
                    if (isOnPress && config != null && config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.JeehellInputPanel).syncFromConfig(config.onPress as JeehellInputAction);
                    else if (!isOnPress && config != null && config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.JeehellInputPanel).syncFromConfig(config.onRelease as JeehellInputAction);

                    break;

                case InputConfig.VJoyInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.VJoyInputPanel();
                    if (isOnPress && config != null && config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.VJoyInputPanel).syncFromConfig(config.onPress as VJoyInputAction);
                    else if (!isOnPress && config != null && config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.VJoyInputPanel).syncFromConfig(config.onRelease as VJoyInputAction);
                    break;

                case MobiFlight.InputConfig.LuaMacroInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.LuaMacroInputPanel();
                    if (isOnPress && config != null && config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.LuaMacroInputPanel).syncFromConfig(config.onPress as LuaMacroInputAction);
                    else if (!isOnPress && config != null && config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.LuaMacroInputPanel).syncFromConfig(config.onRelease as LuaMacroInputAction);

                    break;

                case MobiFlight.InputConfig.RetriggerInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.RetriggerInputPanel();
                    if (isOnPress && config != null && config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.RetriggerInputPanel).syncFromConfig(config.onPress as RetriggerInputAction);
                    else if (!isOnPress && config != null && config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.RetriggerInputPanel).syncFromConfig(config.onRelease as RetriggerInputAction);

                    break;

                case MobiFlight.InputConfig.VariableInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.VariableInputPanel();
                    (panel as MobiFlight.UI.Panels.Action.VariableInputPanel).SetVariableReferences(Variables);
                    if (isOnPress && config != null && config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.VariableInputPanel).syncFromConfig(config.onPress as VariableInputAction);
                    else if (!isOnPress && config != null && config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.VariableInputPanel).syncFromConfig(config.onRelease as VariableInputAction);

                    break;

                // For backward compatibility this is now combined and MSFS2020EventIdInputAction was removed
                case MobiFlight.InputConfig.MSFS2020CustomInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.MSFS2020CustomInputPanel();
                    if (isOnPress && config != null && config.onPress != null)
                    {
                        if (config.onPress is MSFS2020CustomInputAction)
                            (panel as MobiFlight.UI.Panels.Action.MSFS2020CustomInputPanel).syncFromConfig(config.onPress as MSFS2020CustomInputAction);
                        else if (config.onPress is MSFS2020EventIdInputAction)
                            (panel as MobiFlight.UI.Panels.Action.MSFS2020CustomInputPanel).syncFromConfig(config.onPress as MSFS2020EventIdInputAction);
                    }
                    else if (!isOnPress && config != null && config.onRelease != null)
                    {
                        if (config.onRelease is MSFS2020CustomInputAction)
                            (panel as MobiFlight.UI.Panels.Action.MSFS2020CustomInputPanel).syncFromConfig(config.onRelease as MSFS2020CustomInputAction);
                        else if (config.onRelease is MSFS2020EventIdInputAction)
                            (panel as MobiFlight.UI.Panels.Action.MSFS2020CustomInputPanel).syncFromConfig(config.onRelease as MSFS2020EventIdInputAction);
                    }

                    break;

                case MobiFlight.InputConfig.XplaneInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.XplaneInputPanel();
                    if (isOnPress && config != null && config.onPress != null)
                        (panel as MobiFlight.UI.Panels.Action.XplaneInputPanel).syncFromConfig(config.onPress as XplaneInputAction);
                    else if (!isOnPress && config != null && config.onRelease != null)
                        (panel as MobiFlight.UI.Panels.Action.XplaneInputPanel).syncFromConfig(config.onRelease as XplaneInputAction);

                    break;
            }

            if (panel != null)
            {
                panel.Padding = new Padding(2, 0, 2, 0);
                panel.Dock = DockStyle.Top;
                owner.Controls.Add(panel);
                owner.Dock = DockStyle.Top;
                OnPanelChanged?.Invoke(panel, EventArgs.Empty);
            }
            this.ResumeLayout(true);
        }

        public void SetVariableReferences(Dictionary<String, MobiFlightVariable> variables)
        {
            Variables = variables;
        }

        // On Press Action
        private void onPressActionTypePanel_ActionTypeChanged(object sender, String value)
        {
            Panel owner = onPressActionConfigPanel;
            bool isOnPress = (sender as MobiFlight.UI.Panels.Config.ActionTypePanel) == onPressActionTypePanel;

            if (!isOnPress) owner = onReleaseActionConfigPanel;

            UpdatePanelWithAction(owner, _config, value, isOnPress);
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

                    case MobiFlight.InputConfig.XplaneInputAction.Label:
                        config.onPress = (onPressActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.XplaneInputPanel).ToConfig();
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

                    case MobiFlight.InputConfig.XplaneInputAction.Label:
                        config.onRelease = (onReleaseActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.XplaneInputPanel).ToConfig();
                        break;

                    default:
                        config.onRelease = null;
                        break;
                }
            }
        }
    }
}
