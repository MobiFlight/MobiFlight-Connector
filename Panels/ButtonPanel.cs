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
using ArcazeUSB.Panels.Group;

namespace ArcazeUSB.Panels
{
    public partial class ButtonPanel : UserControl
    {
        MobiFlight.InputConfig.ButtonInputConfig _config;

        public ButtonPanel()
        {
            InitializeComponent();
            onPressActionTypePanel.ActionTypeChanged += new ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);
        }

        // On Press Action
        private void onPressActionTypePanel_ActionTypeChanged(object sender, String value)
        {
            Control panel = null;

            actionConfigPanel.Controls.Clear();
            switch ((sender as ComboBox).SelectedItem.ToString())
            {
                case "FSUIPC Offset":
                    panel = new Panels.Group.FsuipcConfigPanel();
                    (panel as Panels.Group.FsuipcConfigPanel).setMode(false);

                    if (_config != null && _config.onPress != null)
                    {
                        (panel as Panels.Group.FsuipcConfigPanel).syncFromConfig(_config.onPress as FsuipcOffsetInputAction);
                    }

                    break;
            }

            if (panel != null)
            {
                panel.Padding = new Padding(0, 0, 0, 0);
                panel.Width = actionConfigPanel.Width;
                actionConfigPanel.Controls.Add(panel);
                panel.Height = actionConfigPanel.Height - 3;
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
                onPressActionTypePanel.syncFromConfig(_config.onRelease);
            }
        }

        public void ToConfig(MobiFlight.InputConfig.ButtonInputConfig config)
        {
            config.onPress = (actionConfigPanel.Controls[0] as FsuipcConfigPanel).ToConfig();
            // still missing on release
        }
    }
}
