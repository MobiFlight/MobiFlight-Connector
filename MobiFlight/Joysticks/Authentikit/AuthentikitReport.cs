using SharpDX.DirectInput;
using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.AuthentiKit
{
    public class AuthentiKitReport
    {
        private byte[] LastInputBufferState = new byte[5];

        public void CopyFromInputBuffer(byte[] inputBuffer)
        {
            if (inputBuffer == null || inputBuffer.Length < LastInputBufferState.Length)
            {
                throw new ArgumentException($"Invalid input buffer length. Expected {LastInputBufferState.Length}, got {inputBuffer?.Length ?? 0}");
            }
            LastInputBufferState = (byte[])inputBuffer.Clone();
        }

        public AuthentiKitReport Parse(byte[] inputBuffer)
        {
            var result = new AuthentiKitReport();
            result.CopyFromInputBuffer(inputBuffer);

            return result;
        }

        public JoystickState ToJoystickState(List<JoystickDevice> Axis, int ButtonCount = 12)
        {
            JoystickState state = new JoystickState();

            for (int i = 0; i < Axis.Count; i++)
            {
                int byteIndex = i * 2;

                if (byteIndex + 1 >= LastInputBufferState.Length)
                    break;

                // Combine: 16-bit little-endian value
                int axisValue = (LastInputBufferState[byteIndex] | (LastInputBufferState[byteIndex + 1] << 8)) << 4;

                var axisName = Axis[i].Name.ToLower();

                switch (axisName)
                {
                    case "axis x":
                        state.X = axisValue;
                        break;
                    case "axis y":
                        state.Y = axisValue;
                        break;
                    case "axis z":
                        state.Z = axisValue;
                        break;
                    case "axis rotationx":
                        state.RotationX = axisValue;
                        break;
                    case "axis rotationy":
                        state.RotationY = axisValue;
                        break;
                    case "axis rotationz":
                        state.RotationZ = axisValue;
                        break;
                    case "axis slider1":
                    case "slider1":
                        state.Sliders[0] = axisValue;
                        break;
                    case "axis slider2":
                    case "slider2":
                        state.Sliders[1] = axisValue;
                        break;
                }
            }

            int buttonByteOffset = Axis.Count * 2;
            // Buttons
            for (int i = 0; i < ButtonCount; i++)
            {
                int byteIndex = buttonByteOffset + (i / 8);
                int bitIndex = i % 8;

                if (byteIndex >= LastInputBufferState.Length)
                    break;

                bool isPressed = (LastInputBufferState[byteIndex] & (1 << bitIndex)) != 0;
                state.Buttons[i] = isPressed;
            }

            return state;
        }
    }
}
