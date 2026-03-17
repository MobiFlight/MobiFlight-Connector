using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.UI.Panels.Config;
using MobiFlight.InputConfig;

namespace MobiFlight.UI.Panels.Action
{
    public partial class KeyboardInputPanel : UserControl, IPanelConfigSync
    {
        int isRecording = 0;
        bool CtrlPressed = false;
        bool AltPressed = false;
        bool ShiftPressed = false;
        Keys CurrentKey = Keys.None;

        public KeyboardInputPanel()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CtrlPressed = false;
            AltPressed = false;
            ShiftPressed = false;
            CurrentKey = Keys.None;
            UpdateTextBox();
        }

        private void KeyboardInputPanel_KeyDown(object sender, KeyEventArgs e)
        {
            CtrlPressed = e.Control;
            AltPressed = e.Alt;
            ShiftPressed = e.Shift;
            if (e.KeyCode != Keys.ControlKey &&
                e.KeyCode != Keys.ShiftKey &&
                e.KeyCode != Keys.Menu && 
                e.KeyCode != Keys.Capital &&
                e.KeyCode != Keys.CapsLock)
                CurrentKey = e.KeyCode;
            UpdateTextBox();
        }

        private void UpdateTextBox()
        {
            String text = "";
            if (CtrlPressed) text = "Ctrl + ";
            if (AltPressed) text += "Alt + ";
            if (ShiftPressed) text += "Shift + ";
            if (CurrentKey != Keys.None) text += CurrentKey.ToString();

            textBox1.Text = text;
        }

        private void KeyboardInputPanel_KeyUp(object sender, KeyEventArgs e)
        {
        }

        public void syncFromConfig(object config)
        {
            KeyInputAction keyInputAction = config as KeyInputAction;
            if (keyInputAction == null) return;
            
            KeysConverter kc = new KeysConverter();
            CurrentKey = keyInputAction.Key;
            CtrlPressed = keyInputAction.Control;
            AltPressed = keyInputAction.Alt;
            ShiftPressed = keyInputAction.Shift;
            UpdateTextBox();
        }

        public InputConfig.InputAction ToConfig()
        {
            KeyInputAction config = new KeyInputAction();
            config.Key = CurrentKey;
            config.Shift = ShiftPressed;
            config.Control = CtrlPressed;
            config.Alt = AltPressed;
            return config;
        }
    }
}
