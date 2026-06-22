using SharpDX.DirectInput;
using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.WingFlex
{
    internal class EfisCubeReport : ICubeReport
    {
        // -                Head                         Constant: 0xF2                          -       0       -       0xF2
        // -                Head                         Constant: 0xE1                          -       1       -       0xE1
        // -                Head                         Constant: 0x05                          -       2       -       0x05
        private readonly static byte[] InputHeader = new byte[] { 0xF2, 0xE1, 0x05 };
        // -                Data Type Total              Has 2 Data Type                         -       3       -       0x02
        // -                Data Type                    Bit Type                                -       4       -       0x01
        // -                Data Length                  Following data occupies 4 Bytes         -       5       -       0x04
        private readonly static byte[] InputBitSection = new byte[] { 0x02, 0x01, 0x04 };
        // -                Data Type                    Single Byte Type                        -       10      -       0x02
        // -                Data Length                  Following data occupies 1 Bytes         -       11      -       0x01
        private readonly static byte[] InputByteSection = new byte[] { 0x02, 0x01 };

        // -                Head                         Constant: 0xF2                          -       0       -       0xF2
        // -                Head                         Constant: 0xE1                          -       1       -       0xE1
        // -                Head                         Constant: 0x05                          -       2       -       0x05
        private readonly static byte[] OutputHeader = new byte[] { 0xF2, 0xE1, 0x05 };
        // -                Data Type Total              Has 3 Data Type                         -       3       -       0x03
        // -                Data Type                    Bit Type                                -       4       -       0x01
        // -                Data Length                  Following data occupies 3 Bytes         -       5       -       0x03
        private readonly static byte[] OutputBitSection = new byte[] { 0x03, 0x01, 0x03 };
        // -                Data Type                    Single Byte Type                        -       9       -       0x02
        // -                Data Length                  Following data occupies 2 Bytes         -       10      -       0x02
        // Output           Background Light Brightness  0x00(Minimum)~0xFF(Maximum)             -       11      -       0
        // Output           LCD Brightness               0x00(Minimum)~0xFF(Maximum)             -       12      -       0
        //                  Data Type                    Double Byte Type                        -       13      -       0x03
        //                  Data Length                  Following data occupies 2 Bytes         -       14      -       0x02
        private readonly static byte[] OutputByteSection = new byte[] { 0x02, 0x02, 0, 0, 0x03, 0x02 };

        private byte[] LastInputBufferState = new byte[64];
        private byte[] LastOutputBufferState = new byte[64];

        public EfisCubeReport()
        {
            InitLastInputBufferState();
            InitLastOutputBufferState();
        }
        private void InitLastInputBufferState()
        {
            Buffer.BlockCopy(InputHeader, 0, LastInputBufferState, 0, InputHeader.Length);
            Buffer.BlockCopy(InputBitSection, 0, LastInputBufferState, 3, InputBitSection.Length);
            Buffer.BlockCopy(InputByteSection, 0, LastInputBufferState, 10, InputByteSection.Length);
        }

        private void InitLastOutputBufferState()
        {
            Buffer.BlockCopy(OutputHeader, 0, LastOutputBufferState, 0, OutputHeader.Length);
            Buffer.BlockCopy(OutputBitSection, 0, LastOutputBufferState, 3, OutputBitSection.Length);
            Buffer.BlockCopy(OutputByteSection, 0, LastOutputBufferState, 9, OutputByteSection.Length);
        }

        public void CopyFromInputBuffer(byte[] inputBuffer)
        {
            if (inputBuffer == null || inputBuffer.Length < LastInputBufferState.Length)
            {
                throw new ArgumentException($"Invalid input buffer length. Expected {LastInputBufferState.Length}, got {inputBuffer?.Length ?? 0}");
            }
            LastInputBufferState = (byte[])inputBuffer?.Clone();
        }

        public ICubeReport Parse(byte[] inputBuffer)
        {
            var result = new EfisCubeReport();
            result.CopyFromInputBuffer(inputBuffer);

            return result;
        }

        public byte[] FromOutputDeviceState(List<JoystickOutputDevice> state)
        {
            // OUTPUT DATA STRUCTURE - EFIS Cube Output Report
            // Name	                        Note                                    Mask    Byte[]	Bit[]	Example
            // Head                         Constant: 0xF2                                  0       -       0xF2
            // Head                         Constant: 0xE1                                  1       -       0xE1
            // Head                         Constant: 0x05                                  2       -       0x05
            // Data Type Total              Has 3 Data Type                                 3       -       0x03
            // Data Type                    Bit Type                                        4       -       0x01
            // Data Length                  Following data occupies 3 Bytes                 5       -       0x03
            // MASTER WRAN LIGHT            On: 1, Off: 0                           0x01    6       0       1
            // MASTER CAUT LIGHT            On: 1, Off: 0                           0x02    6       1       1
            // CAPT Arrow                   On: 1, Off: 0                           0x04    6       2       1
            // CAPT                         On: 1, Off: 0                           0x08    6       3       1
            // FD LIGHT                     On: 1, Off: 0                           0x10    6       4       1
            // LS LIGHT                     On: 1, Off: 0                           0x20    6       5       1
            // CSTR LIGHT                   On: 1, Off: 0                           0x40    6       6       1
            // WPT LIGHT                    On: 1, Off: 0                           0x80    6       7       1
            // VOR.D LIGHT                  On: 1, Off: 0                           0x01    7       0       1
            // NDB LIGHT                    On: 1, Off: 0                           0x02    7       1       1
            // ARPT LIGHT                   On: 1, Off: 0                           0x04    7       2       1
            // Light Control Mode           By Host: 1(Default), By Slave:0         0x08    7       3       1
            // LCD QFE Tag                  On: 1, Off: 0                           0x10    7       4       1
            // LCD QNH Tag                  On: 1, Off: 0                           0x20    7       5       1
            // LCD Dot(.)                   On: 1, Off: 0                           0x40    7       6       1
            // LCD STD Mode                 On: 1, Off: 0                           0x80    7       7       0
            // Power                        On: 1, Off: 0                           0x01    8       0       1
            // (Reserve)                    -                                               8       1       0
            // (Reserve)                    -                                               8       2       0
            // (Reserve)                    -                                               8       3       0
            // (Reserve)                    -                                               8       4       0
            // (Reserve)                    -                                               8       5       0
            // (Reserve)                    -                                               8       6       0
            // (Reserve)                    -                                               8       7       0
            // Data Type                    Single Byte Type                        -       9       -       0x02
            // Data Length                  Following data occupies 2 Bytes         -       10      -       0x02
            // Background Light Brightness  0x00(Minimum) - 0xFF(Maximum)           -       11      -       0
            // LCD Brightness               0x00(Minimum) - 0xFF(Maximum)           -       12      -       0
            // Data Type                    Double Byte Type                        -       13      -       0x03
            // Data Length                  Following data occupies 2 Bytes         -       14      -       0x02
            // inHg - hPa Number            High 8 bit of Uint16                    -       15      -       0x00
            // inHg - hPa Number            Low 8 bit of Uint16                     -       16      -       0x00

            // Set default power state to ON before processing device states.
            // This may be overridden by subsequent device state logic below.
            // This ensures that if no device state explicitly sets power off,
            // the power will remain on by default.
            //
            // This is required to make test mode work correctly, as test mode
            // requires power to be on to light up the single LED and the display.
            LastOutputBufferState[8] |= 1;

            state.ForEach(item =>
            {
                if (item.Type == DeviceType.LcdDisplay)
                {
                    var lcdDisplay = item as JoystickOutputDisplay;
                    if (lcdDisplay == null) return;

                    var byteIndex = lcdDisplay.Byte;
                    UInt16 value = 0;
                    bool parsed;

                    if (lcdDisplay.Name != "VS.value")
                    {
                        parsed = UInt16.TryParse(lcdDisplay.Text, out value);
                    }
                    else
                    {
                        parsed = Int16.TryParse(lcdDisplay.Text, out var signedValue);
                        if (parsed) value = (UInt16)signedValue;
                    }

                    // Skip invalid text
                    if (!parsed && lcdDisplay.Text.Trim() != "") return;

                    // Copy High 8 bit from value
                    LastOutputBufferState[byteIndex] = (byte)(value >> 8);
                    // Copy Low 8 bit from value  
                    LastOutputBufferState[byteIndex + 1] = (byte)(value & 0xFF);

                    return;
                }

                var itemByte = item.Byte;

                if (itemByte >= 6 && itemByte <= 8)
                {
                    if (item.State == 1)
                    {
                        LastOutputBufferState[itemByte] |= (byte)(1 << item.Bit);
                    }
                    else
                    {
                        LastOutputBufferState[itemByte] &= (byte)~(1 << item.Bit);
                    }
                }
                else if (itemByte == 11 || itemByte == 12) // Brightness
                {
                    LastOutputBufferState[itemByte] = item.State;
                }
            });

            return LastOutputBufferState.Clone() as byte[];
        }

        public JoystickState ToJoystickState()
        {
            //  Name            Note                            Mask    Byte[]  Bit[]   Example
            //  Head            Constant:  0xF2                         0       -       0xF2
            //  Head            Constant:  0xE1                         1       -       0xE1
            //  Head            Constant:  0x05                         2       -       0x05
            //  Data Type Total Has 2 Data Type                         3       -       0x02
            //  Data Type       Bit Type                                4       -       0x01
            //  Data Length     Following data occupies 4 Bytes         5       -       0x04
            //  MASTER WRAM     Press: 1, Release: 0            0x01    6       0       1
            //  MASTER CAUT     Press: 1, Release: 0            0x02    6       1       1
            //  CHRONO          Press: 1, Release: 0            0x04    6       2       1
            //  STICK PRIORITY  Press: 1, Release: 0            0x08    6       3       1
            //  FD Press:       1, Release: 0                   0x10    6       4       1
            //  LS Press:       1, Release: 0                   0x20    6       5       1
            //  CSTR Press:     1, Release: 0                   0x40    6       6       1
            //  WPT Press:      1, Release: 0                   0x80    6       7       1
            //  VOR.D Push:     1, Release: 0                   0x01    7       0       1
            //  NDB Pull:       1, Release: 0                   0x02    7       1       1
            //  ARPT Push:      1, Release: 0                   0x04    7       2       1
            //  inHg Pull:      1, Release: 0                   0x08    7       3       1
            //  hPa Press:      1, Release: 0                   0x10    7       4       1
            //  Rotary LS       Pointing: 1, Non - pointing: 0  0x20    7       5       1
            //  Rotary VOR      Pointing: 1, Non - pointing: 0  0x40    7       6       1
            //  Rotary NAV      Pointing: 1, Non - pointing: 0  0x80    7       7       1
            //  Rotary ARC      Pointing: 1, Non - pointing: 0  0x01    8       0       1
            //  Rotary PLAN     Pointing: 1, Non - pointing: 0  0x02    8       1       1
            //  Rotary 10       Pointing: 1, Non - pointing: 0  0x04    8       2       1
            //  Rotary 20       Pointing: 1, Non - pointing: 0  0x08    8       3       0
            //  Rotary 40       Pointing: 1, Non - pointing: 0  0x10    8       4       0
            //  Rotary 80       Pointing: 1, Non - pointing: 0  0x20    8       5       0
            //  Rotary 160      Pointing: 1, Non - pointing: 0  0x40    8       6       0
            //  Rotary 320      Pointing: 1, Non - pointing: 0  0x80    8       7       0
            //  ADF1            Enable:1, Disable: 0            0x01    9       0       0
            //  OFF1            Enable:1, Disable: 0            0x02    9       1       1
            //  VOR1            Enable:1, Disable: 0            0x04    9       2       0
            //  ADF2            Enable:1, Disable: 0            0x08    9       3       0
            //  OFF2            Enable:1, Disable: 0            0x10    9       4       1
            //  VOR2            Enable:1, Disable: 0            0x20    9       5       0
            //  Baro Push       Enable: 1, Disable: 0           0x40    9       6       0
            //  Baro Pull       Enable: 1, Disable: 0           0x80    9       7       0

            JoystickState state = new JoystickState();

            // Buttons
            // copy the button states from the buffer to the Buttons bit by bit starting from byte 6 to byte 9
            for (int i = 0; i < 32; i++)
            {
                int byteIndex = 6 + (i / 8);
                int bitIndex = i % 8;
                bool isPressed = (LastInputBufferState[byteIndex] & (1 << bitIndex)) != 0;
                state.Buttons[i] = isPressed;
            }

            // Encoders
            // As long as we don't have proper Encoders, we will map them to buttons
            state.Buttons[32] = ((sbyte)LastInputBufferState[12]) < 0; // Baro Encoder Rotate Left
            state.Buttons[33] = ((sbyte)LastInputBufferState[12]) > 0; // Baro Encoder Rotate Right

            return state;
        }
    }
}