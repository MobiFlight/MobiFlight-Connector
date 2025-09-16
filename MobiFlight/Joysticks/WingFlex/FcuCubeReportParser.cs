using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

namespace MobiFlight.Joysticks.WingFlex
{
    internal class FcuCubeReport
    {
        // -                Head                         Constant: 0xF2                          -       0       -       0xF2
        // -                Head                         Constant: 0xE1                          -       1       -       0xE1
        // -                Head                         Constant: 0x03                          -       2       -       0x03
        private static byte[] InputHeader = new byte[] { 0xF2, 0xE1, 0x03 };
        // -                Data Type Total              Has 2 Data Type                         -       3       -       0x02
        // -                Data Type                    Bit Type                                -       4       -       0x01
        // -                Data Length                  Following data occupies 2 Bytes         -       5       -       0x02
        private static byte[] InputBitSection = new byte[] { 0x02, 0x01, 0x02 };
        // -                Data Type                    Single Byte Type                        -       9       -       0x02
        // -                Data Length                  Following data occupies 6 Bytes         -       10      -       0x06
        private static byte[] InputByteSection = new byte[] { 0x02, 0x06 };
        
        // -                Head                         Constant: 0xF2                          -       0       -       0xF2
        // -                Head                         Constant: 0xE1                          -       1       -       0xE1
        // -                Head                         Constant: 0x03                          -       2       -       0x03
        private static byte[] OutputHeader = new byte[] { 0xF2, 0xE1, 0x03 };
        // -                Data Type Total              Has 3 Data Type                         -       3       -       0x02
        // -                Data Type                    Bit Type                                -       4       -       0x01
        // -                Data Length                  Following data occupies 3 Bytes         -       5       -       0x02
        private static byte[] OutputBitSection = new byte[] { 0x02, 0x01, 0x02 };
        // -                Data Type                    Single Byte Type                        -       9       -       0x02
        // -                Data Length                  Following data occupies 2 Bytes         -       10      -       0x02
        // Output           Background Light Brightness  0x00(Minimum)~0xFF(Maximum)             -       11      -       0
        // Output           LCD Brightness               0x00(Minimum)~0xFF(Maximum)             -       12      -       0
        //                  Data Type                    Double Byte Type                        -       13      -       0
        //                  Data Length                  Following data occupies 8 Bytes         -       14      -       0x08
        private static byte[] OutputByteSection = new byte[] { 0x02, 0x02, 0, 0, 0, 0x08 };


        private byte[] LastInputBufferState = new byte[16];
        private byte[] LastOutputBufferState = new byte[22];

        public FcuCubeReport() {
            InitLastInputBufferState();
            InitLastOutputBufferState();
        }
        private void InitLastInputBufferState()
        {
            Buffer.BlockCopy(InputHeader, 0, LastOutputBufferState, 0, InputHeader.Length);
            Buffer.BlockCopy(OutputBitSection, 0, LastOutputBufferState, 3, OutputBitSection.Length);
            Buffer.BlockCopy(OutputByteSection, 0, LastOutputBufferState, 9, OutputByteSection.Length);
        }

        private void InitLastOutputBufferState()
        {
            // Copy using Buffer.BlockCopy - fastest method
            Buffer.BlockCopy(OutputHeader, 0, LastOutputBufferState, 0, OutputHeader.Length);
            Buffer.BlockCopy(OutputBitSection, 0, LastOutputBufferState, 3, OutputBitSection.Length);
            Buffer.BlockCopy(OutputByteSection, 0, LastOutputBufferState, 9, OutputByteSection.Length);
        }

        public void CopyFromInputBuffer(byte[] inputBuffer)
        {
            LastInputBufferState = (byte[])inputBuffer.Clone();
        }

        public FcuCubeReport Parse(byte[] inputBuffer)
        {
            var result = new FcuCubeReport();
            result.CopyFromInputBuffer(inputBuffer);
            
            return result;
        }

        public byte[] FromOutputDeviceState(List<JoystickOutputDevice> state)
        {
            // OUTPUT DATA STRUCTURE - FCU Cube Output Report
            // Type             Name                         Note                                    Mask    Byte[]  Bit[]   Example
            // -                Head                         Constant: 0xF2                          -       0       -       0xF2
            // -                Head                         Constant: 0xE1                          -       1       -       0xE1
            // -                Head                         Constant: 0x03                          -       2       -       0x03
            // -                Data Type Total              Has 3 Data Type                         -       3       -       0x02
            // -                Data Type                    Bit Type                                -       4       -       0x01
            // -                Data Length                  Following data occupies 3 Bytes         -       5       -       0x02
            // Output           LOC Signal                   On:1, Off: 0                            0x01    6       0       1
            // Output           AP1 Signal                   On:1, Off: 0                            0x02    6       1       1
            // Output           AP2 Signal                   On:1, Off: 0                            0x04    6       2       1
            // Output           A/THR Signal                 On:1, Off: 0                            0x08    6       3       1
            // Output           EXPED Signal                 On:1, Off: 0                            0x10    6       4       1
            // Output           APPR Signal                  On:1, Off: 0                            0x20    6       5       1
            // Output           SPD Managed                  Circle Point, Active:1, Inactive:0      0x40    6       6       1
            // Output           SPD Dashed                   Active:1, Inactive:0                    0x80    6       7       1
            // Output           HDG Managed                  Circle Point, Active:1, Inactive:0      0x01    7       0       1
            // Output           HDG Dashed                   Active:1, Inactive:0                    0x02    7       1       1
            // Output           ALT Managed                  Active:1, Inactive:0                    0x04    7       2       1
            // Output           VS Dashed                    Active:1, Inactive:0                    0x08    7       3       1
            // Output           SPD MACH Mode                SPD:0, MACH:1                           0x10    7       4       1
            // Output           HDG V/S Mode                 HDG V/S:0, TRK FPA:1                    0x20    7       5       1
            // Output           Test Mode                    Turn On All Light And LCD               0x40    7       6       1
            // -                (Reserve)                    -                                       0x80    7       7       0
            // Output           Power                        On:1,Off:0                              0x01    8       0       1
            // -                (Reserve)                    -                                       0x02    8       1       1
            // -                (Reserve)                    -                                       0x04    8       2       1
            // -                (Reserve)                    -                                       -       8       3       0
            // -                (Reserve)                    -                                       -       8       4       0
            // -                (Reserve)                    -                                       -       8       5       0
            // -                (Reserve)                    -                                       -       8       6       0
            // -                (Reserve)                    -                                       -       8       7       0
            // -                Data Type                    Single Byte Type                        -       9       -       0x02
            // -                Data Length                  Following data occupies 2 Bytes         -       10      -       0x02
            // Output           Background Light Brightness  0x00(Minimum)~0xFF(Maximum)             -       11      -       0
            // Output           LCD Brightness               0x00(Minimum)~0xFF(Maximum)             -       12      -       0
            //                  Data Type                    Double Byte Type                        -       13      -       0
            //                  Data Length                  Following data occupies 8 Bytes         -       14      -       0x08
            // LcdDisplay       SPD Number                   High 8 bit of Uint16                    -       15      -       0x00
            // LcdDisplay       SPD Number                   Low 8 bit of Uint16                     -       16      -       0x00
            // LcdDisplay       HDG Number                   High 8 bit of Uint16                    -       17      -       0x00
            // LcdDisplay       HDG Number                   Low 8 bit of Uint16                     -       18      -       0x00
            // LcdDisplay       ALT Number                   High 8 bit of Uint16                    -       19      -       0x00
            // LcdDisplay       ALT Number                   Low 8 bit of Uint16                     -       20      -       0x00
            // LcdDisplay       V/S Number                   High 8 bit of Uint16                    -       21      -       0x00
            // LcdDisplay       V/S Number                   Low 8 bit of Uint16                     -       22      -       0x00

            state.ForEach(item => { 
                if (item.Byte >= 6 && item.Byte <= 8)
                {
                    if (item.State == 1)
                    {
                        LastOutputBufferState[item.Byte] |= (byte)(1 << item.Bit);
                    }
                    else
                    {
                        LastOutputBufferState[item.Byte] &= (byte)~(1 << item.Bit);
                    }
                }
                else if (item.Byte == 11 || item.Byte == 12) // Brightness
                {
                    LastOutputBufferState[item.Byte] = item.State;
                }
            });

            return LastOutputBufferState;
        }

        public JoystickState ToJoystickState()
        {
            // Device       Name                         Note                                    Mask    Byte[]  Bit[]   Example
            // -            Head                         Constant: 0xF2                          -       0       -       0xF2
            // -            Head                         Constant: 0xE1                          -       1       -       0xE1
            // -            Head                         Constant: 0x03                          -       2       -       0x03
            // -            Data Type Total              Has 2 Data Type                         -       3       -       0x02
            // -            Data Type                    Bit Type                                -       4       -       0x01
            // -            Data Length                  Following data occupies 2 Bytes         -       5       -       0x02
            // Button       SPD MACH Switch              Press:1, Release: 0                     0x01    6       0       1
            // Button       SPD Knob Push                Push: 1, Release: 0                     0x02    6       1       1
            // Button       SPD Knob Pull                Pull: 1, Release: 0                     0x04    6       2       1
            // Button       HDG TRK Switch               Press: 1, Release: 0                    0x08    6       3       1
            // Button       HDG Knob Push                Push: 1, Release: 0                     0x10    6       4       1
            // Button       HDG Knob Pull                Pull: 1, Release: 0                     0x20    6       5       1
            // Button       ALT 100                      Pointing: 1, Non-pointing: 0            0x40    6       6       1
            // Button       ALT 1000                     Pointing: 1, Non-pointing: 0            0x80    6       7       1
            // Button       ALT Knob Push                Push: 1, Release: 0                     0x01    7       0       1
            // Button       ALT Knob Pull                Pull: 1, Release: 0                     0x02    7       1       1
            // Button       VS Knob Push                 Push: 1, Release: 0                     0x04    7       2       1
            // Button       VS Knob Pull                 Pull: 1, Release: 0                     0x08    7       3       1
            // Button       METRIC ALT                   Press: 1, Release: 0                    0x10    7       4       1
            // Button       AP1                          Press: 1, Release: 0                    0x20    7       5       1
            // Button       AP2                          Press: 1, Release: 0                    0x40    7       6       1
            // Button       A/THR                        Press: 1, Release: 0                    0x80    7       7       1
            // Button       LOC                          Press: 1, Release: 0                    0x01    8       0       1
            // Button       EXPED                        Press: 1, Release: 0                    0x02    8       1       1
            // Button       APPR                         Press: 1, Release: 0                    0x04    8       2       1
            // -            (Reserve)                    -                                       -       8       3       0
            // -            (Reserve)                    -                                       -       8       4       0
            // -            (Reserve)                    -                                       -       8       5       0
            // -            (Reserve)                    -                                       -       8       6       0
            // -            (Reserve)                    -                                       -       8       7       0
            // -            Data Type                    Single Byte Type                        -       9       -       0x02
            // -            Data Length                  Following data occupies 6 Bytes         -       10      -       0x06
            // Encoder      SPD Knob Rotate              0x00~0xFF in Two's Complement           -       11      -       0
            // Encoder      HDG Knob Rotate              0x00~0xFF in Two's Complement           -       12      -       0
            // Encoder      ALT Knob Rotate              0x00~0xFF in Two's Complement           -       13      -       0
            // Encoder      VS Knob Rotate               0x00~0xFF in Two's Complement           -       14      -       0
            // Axis         Background Light Brightness  0x00(Minimum)~0xFF(Maximum)             -       15      -       0xFF
            // Axis         LCD Light Brightness         0x00(Minimum)~0xFF(Maximum)             -       16      -       0xFF

            JoystickState state = new JoystickState();

            // Buttons
            // copy the button states from the buffer to the Buttons bit by bit starting from byte 6 to byte 8
            for (int i = 0; i < 19; i++)
            {
                int byteIndex = 6 + (i / 8);
                int bitIndex = i % 8;
                bool isPressed = (LastInputBufferState[byteIndex] & (1 << bitIndex)) != 0;
                state.Buttons[i] = isPressed;
            }

            // Encoders
            // As long as we don't have proper Encoders, we will map them to buttons
            state.Buttons[20] = ((sbyte)LastInputBufferState[11]) < 0; // SPD Knob Rotate Left
            state.Buttons[21] = ((sbyte)LastInputBufferState[11]) > 0; // SPD Knob Rotate Right
            state.Buttons[22] = ((sbyte)LastInputBufferState[12]) < 0; // HDG Knob Rotate Left
            state.Buttons[23] = ((sbyte)LastInputBufferState[12]) > 0; // HDG Knob Rotate Right
            state.Buttons[24] = ((sbyte)LastInputBufferState[13]) < 0; // ALT Knob Rotate Left
            state.Buttons[25] = ((sbyte)LastInputBufferState[13]) > 0; // ALT Knob Rotate Right
            state.Buttons[26] = ((sbyte)LastInputBufferState[14]) < 0; // VS Knob Rotate Left
            state.Buttons[27] = ((sbyte)LastInputBufferState[14]) > 0; // VS Knob Rotate Right

            // Axes
            // Background Light Brightness
            state.X = LastInputBufferState[15]; // Background Light Brightness
            state.Y = LastInputBufferState[16]; // LCD Light Brightness

            return state;
        }
    }
}
