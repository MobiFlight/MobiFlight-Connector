using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace MobiFlight
{
    public static class KeyboardInput
    {
        #region Imports

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        #endregion

        public static void SendKeyAsInput(System.Windows.Forms.Keys Key, bool Control, bool Alt, bool Shift)
        {
            List<INPUT> keyPresses = new List<INPUT>();

            if (Control)
            {
                INPUT control = new INPUT();
                control.type = (int)InputType.INPUT_KEYBOARD;
                control.ki.wVk = (short)System.Windows.Forms.Keys.ControlKey;
                control.ki.dwFlags = (int)KEYEVENTF.KEYDOWN;
                control.ki.dwExtraInfo = GetMessageExtraInfo();
                keyPresses.Add(control);
            }

            if (Alt)
            {
                INPUT alt = new INPUT();
                alt.type = (int)InputType.INPUT_KEYBOARD;
                alt.ki.wVk = (short)System.Windows.Forms.Keys.LMenu;
                alt.ki.dwFlags = (int)KEYEVENTF.KEYDOWN;
                alt.ki.dwExtraInfo = GetMessageExtraInfo();
                keyPresses.Add(alt);
            }

            if (Shift)
            {
                INPUT shift = new INPUT();
                shift.type = (int)InputType.INPUT_KEYBOARD;
                shift.ki.wVk = (short)System.Windows.Forms.Keys.ShiftKey;
                shift.ki.dwFlags = (int)KEYEVENTF.KEYDOWN;
                shift.ki.dwExtraInfo = GetMessageExtraInfo();
                keyPresses.Add(shift);
            }

            /*
            foreach (System.Windows.Forms.Keys modifier in modifiers)
            {
                INPUT structure = new INPUT();
                structure.type = (int)InputType.INPUT_KEYBOARD;
                structure.ki.wVk = (short)modifier;
                structure.ki.dwFlags = (int)KEYEVENTF.KEYDOWN;
                structure.ki.dwExtraInfo = GetMessageExtraInfo();
            }
             * */

            INPUT structure = new INPUT();
            structure.type = (int)InputType.INPUT_KEYBOARD;
            structure.ki.wVk = (short)Key;
            structure.ki.dwFlags = (int)KEYEVENTF.KEYDOWN;
            structure.ki.dwExtraInfo = GetMessageExtraInfo();

            keyPresses.Add(structure);

            INPUT input2 = new INPUT();
            input2.type = (int)InputType.INPUT_KEYBOARD;
            input2.ki.wVk = (short)Key;
            input2.mi.dwFlags = (int)KEYEVENTF.KEYUP;
            input2.ki.dwExtraInfo = GetMessageExtraInfo();

            keyPresses.Add(input2);

            if (Control)
            {
                INPUT control = new INPUT();
                control.type = (int)InputType.INPUT_KEYBOARD;
                control.ki.wVk = (short)System.Windows.Forms.Keys.ControlKey;
                control.ki.dwFlags = (int)KEYEVENTF.KEYUP;
                control.ki.dwExtraInfo = GetMessageExtraInfo();
                keyPresses.Add(control);
            }

            if (Alt)
            {
                INPUT alt = new INPUT();
                alt.type = (int)InputType.INPUT_KEYBOARD;
                alt.ki.wVk = (short)System.Windows.Forms.Keys.LMenu;
                alt.ki.dwFlags = (int)KEYEVENTF.KEYUP;
                alt.ki.dwExtraInfo = GetMessageExtraInfo();
                keyPresses.Add(alt);
            }

            if (Shift)
            {
                INPUT shift = new INPUT();
                shift.type = (int)InputType.INPUT_KEYBOARD;
                shift.ki.wVk = (short)System.Windows.Forms.Keys.ShiftKey;
                shift.ki.dwFlags = (int)KEYEVENTF.KEYUP;
                shift.ki.dwExtraInfo = GetMessageExtraInfo();
                keyPresses.Add(shift);
            }

            SendInput((uint)keyPresses.Count, (INPUT[]) (keyPresses.ToArray()), Marshal.SizeOf(structure));
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT
        {
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public int type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [Flags]
        public enum InputType
        {
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 2
        }

        [Flags]
        public enum KEYEVENTF
        {
            KEYDOWN = 0,
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            UNICODE = 0x0004,
            SCANCODE = 0x0008,
        }
    }
}