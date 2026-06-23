using SharpDX.DirectInput;
using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.WingFlex
{
    internal class RmpCubeReport : ICubeReport
    {
        // -                Head                         Constant: 0xF2                          -       0       -       0xF2
        // -                Head                         Constant: 0xE1                          -       1       -       0xE1
        // -                Head                         Constant: 0x06                          -       2       -       0x06
        private readonly static byte[] InputHeader = new byte[] { 0xF2, 0xE1, 0x06 };
        // -                Data Type Total              Has 2 Data Type                         -       3       -       0x02
        // -                Data Type                    Bit Type                                -       4       -       0x01
        // -                Data Length                  Following data occupies 4 Bytes         -       5       -       0x04
        private readonly static byte[] InputBitSection = new byte[] { 0x02, 0x01, 0x04 };
        // -                Data Type                    Single Byte Type                        -       10      -       0x02
        // -                Data Length                  Following data occupies 2 Bytes         -       11      -       0x02
        private readonly static byte[] InputByteSection = new byte[] { 0x02, 0x02 };

        // -                Head                         Constant: 0xF2                          -       0       -       0xF2
        // -                Head                         Constant: 0xE1                          -       1       -       0xE1
        // -                Head                         Constant: 0x06                          -       2       -       0x06
        private readonly static byte[] OutputHeader = new byte[] { 0xF2, 0xE1, 0x06 };
        // -                Data Type Total              Has 3 Data Type                         -       3       -       0x02
        // -                Data Type                    Bit Type                                -       4       -       0x01
        // -                Data Length                  Following data occupies 3 Bytes         -       5       -       0x03
        private readonly static byte[] OutputBitSection = new byte[] { 0x02, 0x01, 0x03 };
        // -                Data Type                    Single Byte Type                        -       9       -       0x02
        // -                Data Length                  Following data occupies 2 Bytes         -       10      -       0x02
        // Output           LCDBrightness                0x00(Minimum)~0xFF(Maximum)             -       11      -       0
        // Output           Background Light Brightness  0x00(Minimum)~0xFF(Maximum)             -       12      -       0
        // Output           Left Dot Position            0-5                                     -       13      -       0
        // Output           Right Dot Position           0-5                                     -       14      -       3
        // Output           Left Display Digit Switch    digits enabled                          -       15      -       3
        // Output           Right Display Digit Switch   digits enabled                          -       16      -       3
        //                  Data Type                    Double Byte Type                        -       17      -       0x00
        //                  Data Length                  Following data occupies 8 Bytes         -       18      -       0x08
        private readonly static byte[] OutputByteSection = new byte[] { 0x02, 0x02, 0, 0, 0, 0, 0, 0, 0x00, 0x08 };

        private byte[] LastInputBufferState = new byte[64];
        private byte[] LastOutputBufferState = new byte[64];

        private readonly static Dictionary<byte, byte> DotByteMapping = new Dictionary<byte, byte>
            {
                { 19, 13 }, // Left LCD
                { 21, 14 }  // Right LCD
            };

        public RmpCubeReport()
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
            var result = new RmpCubeReport();
            result.CopyFromInputBuffer(inputBuffer);

            return result;
        }

        public byte[] FromOutputDeviceState(List<JoystickOutputDevice> state)
        {
            //  OUTPUT DATA STRUCTURE - RMP Cube Output Report
            //  Name	                    Note                                    Mask    Byte[]	Bit[]	Example
            //  Head                        Constant: 0xF2                                  0       -       0xF2
            //  Head                        Constant: 0xE1                                  1       -       0xE1
            //  Head                        Constant: 0x06                                  2       -       0x06
            //  Data Type Total             Has 3 Data Type                                 3       -       0x03
            //  Data Type                   Bit Type                                        4       -       0x01
            //  Data Length                 Following data occupies 3 Bytes                 5       -       0x03
            //  Transfer Led                On: 1, Off: 0                           0x01    6       0       1
            //  VHF1 Signal                 On: 1, Off: 0                           0x02    6       1       1
            //  VHF2 Signal                 On: 1, Off: 0                           0x04    6       2       1
            //  VHF3 Signal                 On: 1, Off: 0                           0x08    6       3       1
            //  LOAD Signal                 On: 1, Off: 0                           0x10    6       4       1
            //  HF1 Signal                  On: 1, Off: 0                           0x20    6       5       1
            //  SEL Signal                  On: 1, Off: 0                           0x40    6       6       1
            //  HF2 Signal                  On: 1, Off: 0                           0x80    6       7       1
            //  ATC Signal                  On: 1, Off: 0                           0x01    7       0       1
            //  NAV Signal                  On: 1, Off: 0                           0x02    7       1       1
            //  VOR Signal                  On: 1, Off: 0                           0x04    7       2       1
            //  ILS Signal                  On: 1, Off: 0                           0x08    7       3       1
            //  GLS Signal                  On: 1, Off: 0                           0x10    7       4       1
            //  MLS Signal                  On: 1, Off: 0                           0x20    7       5       1
            //  ADF Signal                  On: 1, Off: 0                           0x40    7       6       1
            //  ATC MSG Upper Led           On: 1, Off: 0                           0x80    7       7       0
            //  AUTO LAND Led               On: 1, Off: 0                           0x01    8       0       1
            //  Left LCD Switch             On: 1, Off: 0                           0x02    8       1       1
            //  Right LCD Switch            On: 1, Off: 0                           0x04    8       2       1
            //  Left Display -"C" Mode      On: 1, Off: 0, priority second.         -       8       3       0
            //  Right Display -"C" Mode     On: 1, Off: 0, priority second.         -       8       4       0
            //  Left Display -"Data" Mode   On:1, Off: 0, priority first.           -       8       5       0
            //  Right Display -"Data" Mode  On:1, Off: 0 priority first.            -       8       6       0
            //  ATC MSG Lower Led           On: 1, Off: 0                           -       8       7       0
            //  Data Type                   Single Byte Type                        -       9       -       0x02
            //  Data Length                 Following data occupies 2 Bytes         -       10      -       0x02
            //  LCD Brightness              0x00(Minimum) - 0xFF(Maximum)           -       12      -       0
            //  Background Light Brightness 0x00(Minimum) - 0xFF(Maximum)           -       11      -       0
            //  Left Dot Position	        0-5                                     -       13      -       3
            //  Right Dot Position          0-5                                     -       14      -       3
            //  Left Display Digit Switch   See About Note                          -       15      -       3
            //  Right Display Digit Switch  See About Note                          -       16      -       3
            //  Data Type                   Double Byte Type                        -       17      -       0x00
            //  Data Length                 Following data occupies 2 Bytes         -       18      -       0x08
            //  ACTIVE LCD Left 3 Digit     High 8 bits of Uint16, 000 - 999        -       19      -       0x00
            //  ACTIVE LCD Left 3 Digit     Low 8 bits of Uint16, 000 - 999         -       20      -       0x00
            //  ACTIVE LCD Right 3 Digit    High 8 bits of Uint16, 000 - 999        -       21      -       0x00
            //  ACTIVE LCD Right 3 Digit    Low 8 bits of Uint16, 000 - 999         -       22      -       0x00
            //  STBY / CRS Left 3 Digit     High 8 bits of Uint16, 000 - 999        -       23      -       0x00
            //  STBY / CRS Left 3 Digit     Low 8 bits of Uint16, 000 - 999         -       24      -       0x00
            //  STBY / CRS Right 3 Digit    High 8 bits of Uint16, 000 - 999        -       25      -       0x00
            //  STBY / CRS Right 3 Digit    Low 8 bits of Uint16, 000 - 999         -       26      -       0x00

            state.ForEach(item =>
            {
                if (item.Type == DeviceType.LcdDisplay)
                {
                    UpdateLcdDisplayOutputState(item);
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

        private void UpdateLcdDisplayOutputState(JoystickOutputDevice item)
        {

            var lcdDisplay = item as JoystickOutputDisplay;
            if (lcdDisplay == null) return;

            var hasDot = lcdDisplay.Text.Contains(".");
            var textWithoutDot = lcdDisplay.Text.Replace(".", "");
            var paddedText = textWithoutDot.PadLeft(lcdDisplay.Cols, '0');
            var group1 = paddedText.Substring(0, (int)Math.Floor(lcdDisplay.Cols / 2.0));
            var group2 = paddedText.Substring((int)Math.Floor(lcdDisplay.Cols / 2.0));
            UpdateLcdDisplay(item.Byte, group1);
            UpdateLcdDisplay(item.Byte + 2, group2);

            // Update visible digits based on original text length without dots

            // update dot position based on first dot position in the original text
            // update active digits
            if (DotByteMapping.ContainsKey(lcdDisplay.Byte))
            {
                var dotByte = DotByteMapping[lcdDisplay.Byte];
                // Always clear the dot
                LastOutputBufferState[dotByte] = 0;
                if (hasDot)
                {
                    int dotPosition = lcdDisplay.Text.IndexOf(".");
                    // the dot position corresponds to the position of the dot in the original text
                    // example: 123.456 -> dot-position is 3 which is the 3rd digit from the left
                    // this seems to be fine for the resulting dot position too.
                    LastOutputBufferState[dotByte] = (byte)(dotPosition);
                }

                // enable digits based on length of the text without dots, but only if the text is not empty
                var digitByte = dotByte + 2;
                // Always clear the digit enable bits
                LastOutputBufferState[digitByte] = 0;
                if (!string.IsNullOrEmpty(textWithoutDot))
                {
                    int visibleDigits = textWithoutDot.Length;
                    LastOutputBufferState[digitByte] = (byte)((1<<visibleDigits) - 1);
                }
            }
            // continue with next item, as we have already processed the LCD display state
            return;
        }

        protected void UpdateLcdDisplay(int byteIndex, string text)
        {
            UInt16 value = 0;
            bool parsed;

            parsed = Int16.TryParse(text.Trim(), out var signedValue);
            if (parsed) value = (UInt16)signedValue;

            // Skip invalid text
            if (!parsed) return;

            // Copy High 8 bit from value
            LastOutputBufferState[byteIndex] = (byte)(value >> 8);
            // Copy Low 8 bit from value  
            LastOutputBufferState[byteIndex + 1] = (byte)(value & 0xFF);
        }

        public JoystickState ToJoystickState()
        {
            //  Name                                Note                                Mask    Byte[]  Bit[]   Example
            //  Head                                Constant: 0xF2                              0       -       0xF2
            //  Head                                Constant: 0xE1                              1       -       0xE1
            //  Head                                Constant: 0x06                              2       -       0x06
            //  Data Type Total                     Has 2 Data Type                             3       -       0x02
            //  Data Type                           Bit Type                                    4       -       0x01
            //  Data Length                         Following data occupies 4 Bytes             5       -       0x04
            //  Transfer Key                        Press: 1, Release: 0                0x01    6       0       0
            //  VHF1 Key                            Press: 1, Release: 0                0x02    6       1       0
            //  VHF2 Key                            Press: 1, Release: 0                0x04    6       2       0
            //  VHF3 Key                            Press: 1, Release: 0                0x08    6       3       0
            //  LOAD Key                            Press: 1, Release: 0                0x10    6       4       0
            //  HF1 Key                             Press: 1, Release: 0                0x20    6       5       0
            //  HF2 Key                             Press: 1, Release: 0                0x40    6       6       0
            //  ATC Key                             Press: 1, Release: 0                0x80    6       7       0
            //  NAV Key                             Push: 1, Release: 0                 0x01    7       0       0
            //  VOR Key                             Pull: 1, Release: 0                 0x02    7       1       0
            //  ILS Key                             Push: 1, Release: 0                 0x04    7       2       0
            //  GLS Key                             Pull: 1, Release: 0                 0x08    7       3       0
            //  MLS Key                             Press: 1, Release: 0                0x10    7       4       0
            //  ADF Key                             Press: 1, Release: 0                0x20    7       5       0
            //  Encoder Key -IDENT                  Press: 1, Release: 0                0x40    7       6       0
            //  Power On                            Press: 1, Release: 0                0x80    7       7       1
            //  Power Off                           Press: 1, Release: 0                0x01    8       0       0
            //  ATC MSG Key                         Press: 1, Release: 0                0x02    8       1       0
            //  AUTO LAND Key                       Press: 1, Release: 0                0x04    8       2       0
            //  Mode Selector -STBY                 Pointing: 1, Non - pointing: 0      0x08    8       3       1
            //  Mode Selector -AUTO                 Pointing: 1, Non - pointing: 0      0x10    8       4       0
            //  Mode Selector -ON                   Pointing: 1, Non - pointing: 0      0x20    8       5       0
            //  TCAS Mode -STBY                     Pointing: 1, Non - pointing: 0      0x40    8       6       1
            //  TCAS Mode -TA                       Pointing: 1, Non - pointing: 0      0x80    8       7       0
            //  TCAS Mode -TA / RA                  Pointing: 1, Non - pointing: 0      0x01    9       0       0
            //  Frequency Selector Knobs - Upper    0x00~0xFF in Two's Complement               12      -       0
            //  Frequency Selector Knobs - Lower    0x00~0xFF in Two's Complement               13      -       0

            JoystickState state = new JoystickState();

            // Buttons
            // copy the button states from the buffer to the Buttons bit by bit starting from byte 6 to byte 9
            for (int i = 0; i < 25; i++)
            {
                int byteIndex = 6 + (i / 8);
                int bitIndex = i % 8;
                bool isPressed = (LastInputBufferState[byteIndex] & (1 << bitIndex)) != 0;
                state.Buttons[i] = isPressed;
            }

            // Encoders
            // As long as we don't have proper Encoders, we will map them to buttons
            state.Buttons[25] = ((sbyte)LastInputBufferState[12]) < 0; // Small Knob Rotate Left
            state.Buttons[26] = ((sbyte)LastInputBufferState[12]) > 0; // Small Knob Rotate Right
            state.Buttons[27] = ((sbyte)LastInputBufferState[13]) < 0; // Big Knob Rotate Left
            state.Buttons[28] = ((sbyte)LastInputBufferState[13]) > 0; // Big Knob Rotate Right

            return state;
        }
    }
}