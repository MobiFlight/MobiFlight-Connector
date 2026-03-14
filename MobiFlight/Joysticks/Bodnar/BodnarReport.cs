using SharpDX.DirectInput;
using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.Bodnar
{
    public class BodnarReport
    {
        private readonly int _buttonCount;
        private byte[] _lastInputBuffer;

        public BodnarReport(int buttonCount)
        {
            _buttonCount = buttonCount;
            _lastInputBuffer = Array.Empty<byte>();
        }

        public void CopyFromInputBuffer(byte[] inputBuffer)
        {
            if (inputBuffer == null)
                throw new ArgumentNullException(nameof(inputBuffer));

            _lastInputBuffer = (byte[])inputBuffer.Clone();
        }

        public BodnarReport Parse(byte[] inputBuffer)
        {
            var result = new BodnarReport(_buttonCount);
            result.CopyFromInputBuffer(inputBuffer);
            return result;
        }

        public JoystickState ToJoystickState(List<JoystickDevice> axis)
        {
            JoystickState state = new JoystickState();

            for (int i = 0; i < axis.Count; i++)
            {
                int byteIndex = i * 2;

                if (byteIndex + 1 >= _lastInputBuffer.Length)
                    break;

                int axisValue = (_lastInputBuffer[byteIndex] | (_lastInputBuffer[byteIndex + 1] << 8)) << 4;

                switch (axis[i].Name.ToLower())
                {
                    case "axis x":        state.X = axisValue; break;
                    case "axis y":        state.Y = axisValue; break;
                    case "axis z":        state.Z = axisValue; break;
                    case "axis rotationx": state.RotationX = axisValue; break;
                    case "axis rotationy": state.RotationY = axisValue; break;
                    case "axis rotationz": state.RotationZ = axisValue; break;
                    case "axis slider1":
                    case "slider1":       state.Sliders[0] = axisValue; break;
                    case "axis slider2":
                    case "slider2":       state.Sliders[1] = axisValue; break;
                }
            }

            int buttonByteOffset = axis.Count * 2;
            for (int i = 0; i < _buttonCount; i++)
            {
                int byteIndex = buttonByteOffset + (i / 8);
                int bitIndex  = i % 8;

                if (byteIndex >= _lastInputBuffer.Length)
                    break;

                state.Buttons[i] = (_lastInputBuffer[byteIndex] & (1 << bitIndex)) != 0;
            }

            return state;
        }
    }
}