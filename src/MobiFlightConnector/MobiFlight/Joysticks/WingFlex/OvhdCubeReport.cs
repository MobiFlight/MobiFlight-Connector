using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.WingFlex
{
    internal class OvhdCubeReport : ICubeReport
    {
        // -                Head                         Constant: 0xF2                          -       0       -       0xF2
        // -                Head                         Constant: 0xE1                          -       1       -       0xE1
        // -                Head                         Constant: 0x07                          -       2       -       0x07
        private readonly static byte[] InputHeader = new byte[] { 0xF2, 0xE1, 0x07 };
        // -                Data Type Total              Has 2 Data Type                         -       3       -       0x02
        // -                Data Type                    Bit Type                                -       4       -       0x01
        // -                Data Length                  Following data occupies 20 Bytes        -       5       -       0x14
        private readonly static byte[] InputBitSection = new byte[] { 0x02, 0x01, 0x14 };
        // -                Data Type                    Single Byte Type                        -       10      -       0x02
        // -                Data Length                  Following data occupies 1 Bytes         -       11      -       0x01
        private readonly static byte[] InputByteSection = new byte[] { 0x02, 0x01 };

        // -                Head                         Constant: 0xF2                          -       0       -       0xF2
        // -                Head                         Constant: 0xE1                          -       1       -       0xE1
        // -                Head                         Constant: 0x07                          -       2       -       0x07
        private readonly static byte[] OutputHeader = new byte[] { 0xF2, 0xE1, 0x07 };
        // -                Data Type Total              Has 3 Data Type                         -       3       -       0x03
        // -                Data Type                    Bit Type                                -       4       -       0x01
        // -                Data Length                  Following data occupies 10 Bytes        -       5       -       0x0A
        private readonly static byte[] OutputBitSection = new byte[] { 0x03, 0x01, 0x0A };
        // -                Data Type                    Single Byte Type                        -       16       -      0x02
        // -                Data Length                  Following data occupies 3 Bytes         -       17      -       0x03
        // Output           Background Light Brightness  0x00(Minimum)~0xFF(Maximum)             -       18      -       0
        // Output           VOLT Light Left Brightness   0x00(Minimum)~0xFF(Maximum)             -       19      -       0
        // Output           VOLT Light Right Brightness  0x00(Minimum)~0xFF(Maximum)             -       20      -       0
        //                  Data Type                    Double Byte Type                        -       21      -       0x03
        //                  Data Length                  Following data occupies 8 Bytes         -       22      -       0x04
        private readonly static byte[] OutputByteSection = new byte[] { 0x02, 0x03, 0, 0, 0, 0x03, 0x04 };

        private byte[] LastInputBufferState = new byte[64];
        private byte[] LastOutputBufferState = new byte[64];

        const byte VOLT_DISPLAY_LEFT = 23;
        const byte VOLT_DISPLAY_RIGHT = 25;

        public OvhdCubeReport()
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
            Buffer.BlockCopy(OutputByteSection, 0, LastOutputBufferState, 16, OutputByteSection.Length);
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
            var result = new OvhdCubeReport();
            result.CopyFromInputBuffer(inputBuffer);

            return result;
        }

        public byte[] FromOutputDeviceState(List<JoystickOutputDevice> state)
        {
            //  OUTPUT DATA STRUCTURE - OVHD Cube Output Report
            //  Name                        Note                                    Mask    Byte[]  Bit[]  Example
            //  Head                        Constant: 0xF2                                  0       -      0xF2
            //  Head                        Constant: 0xE1                                  1       -      0xE1
            //  Head                        Constant: 0x07                                  2       -      0x07
            //  Data Type Total             Has 3 Data Type                                 3       -      0x03
            //  Data Type                   Bit Type                                        4       -      0x01
            //  Data Length                 Following data occupies 10 Bytes                5       -      0x0A
            //  ON BAT                      On: 1, Off: 0                           0x01    6       0      1
            //  IR1 U                       On: 1, Off: 0                           0x02    6       1      1
            //  IR1 L                       On: 1, Off: 0                           0x04    6       2      1
            //  IR2 U                       On: 1, Off: 0                           0x08    6       3      1
            //  IR2 L                       On: 1, Off: 0                           0x10    6       4      1
            //  IR3 U                       On: 1, Off: 0                           0x20    6       5      1
            //  IR3 L                       On: 1, Off: 0                           0x40    6       6      1
            //  ENG1 FIRE L                 On: 1, Off: 0                           0x80    6       7      1
            //  ENG1 FIRE R                 On: 1, Off: 0                           0x01    7       0      1
            //  APU FIRE L                  On: 1, Off: 0                           0x02    7       1      1
            //  APU FIRE R                  On: 1, Off: 0                           0x04    7       2      1
            //  ENG2 FIRE L                 On: 1, Off: 0                           0x08    7       3      1
            //  ENG2 FIRE R                 On: 1, Off: 0                           0x10    7       4      1
            //  ENG1 AGENT1 U               On: 1, Off: 0                           0x20    7       5      1
            //  ENG1 AGENT1 L               On: 1, Off: 0                           0x40    7       6      1
            //  ENG1 AGENT2 U               On: 1, Off: 0                           0x80    7       7      0
            //  ENG1 AGENT2 L               On: 1, Off: 0                           0x01    8       0      1
            //  APU AGENT U                 On: 1, Off: 0                           0x02    8       1      1
            //  APU AGENT L                 On: 1, Off: 0                           0x04    8       2      1
            //  ENG2 AGENT1 U               On: 1, Off: 0                           0x08    8       3      0
            //  ENG2 AGENT1 L               On: 1, Off: 0                           0x10    8       4      0
            //  ENG2 AGENT2 U               On: 1, Off: 0                           0x20    8       5      0
            //  ENG2 AGENT2 L               On: 1, Off: 0                           0x40    8       6      0
            //  XFEED U                     On: 1, Off: 0                           0x80    8       7      0
            //  XFEED L                     On: 1, Off: 0                           0x01    9       0      1
            //  L TK PUMP1 U                On: 1, Off: 0                           0x02    9       1      1
            //  L TK PUMP1 L                On: 1, Off: 0                           0x04    9       2      1
            //  L TK PUMP2 U                On: 1, Off: 0                           0x08    9       3      0
            //  L TK PUMP2 L                On: 1, Off: 0                           0x10    9       4      0
            //  C TK PUMP1 U                On: 1, Off: 0                           0x20    9       5      0
            //  C TK PUMP1 L                On: 1, Off: 0                           0x40    9       6      0
            //  MODE SEL U                  On: 1, Off: 0                           0x80    9       7      0
            //  MODE SEL L                  On: 1, Off: 0                           0x01    10      0      1
            //  C TK PUMP2 U                On: 1, Off: 0                           0x02    10      1      1
            //  C TK PUMP2 L                On: 1, Off: 0                           0x04    10      2      1
            //  R TK PUMP1 U                On: 1, Off: 0                           0x08    10      3      0
            //  R TK PUMP1 L                On: 1, Off: 0                           0x10    10      4      0
            //  R TK PUMP2 U                On: 1, Off: 0                           0x20    10      5      0
            //  R TK PUMP2 L                On: 1, Off: 0                           0x40    10      6      0
            //  COMMERCIAL L                On: 1, Off: 0                           0x80    10      7      0
            //  BAT1 U                      On: 1, Off: 0                           0x01    11      0      1
            //  BAT1 L                      On: 1, Off: 0                           0x02    11      1      1
            //  BAT2 U                      On: 1, Off: 0                           0x04    11      2      1
            //  BAT2 L                      On: 1, Off: 0                           0x08    11      3      0
            //  APU GEN U                   On: 1, Off: 0                           0x10    11      4      0
            //  APU GEN L                   On: 1, Off: 0                           0x20    11      5      0
            //  EXT PWR U                   On: 1, Off: 0                           0x40    11      6      0
            //  EXT PWR L                   On: 1, Off: 0                           0x80    11      7      0
            //  PACK1 U                     On: 1, Off: 0                           0x01    12      0      1
            //  PACK1 L                     On: 1, Off: 0                           0x02    12      1      1
            //  PACK2 U                     On: 1, Off: 0                           0x04    12      2      1
            //  PACK2 L                     On: 1, Off: 0                           0x08    12      3      0
            //  APU BLEED U                 On: 1, Off: 0                           0x10    12      4      0
            //  APU BLEED L                 On: 1, Off: 0                           0x20    12      5      0
            //  HOT AIR U                   On: 1, Off: 0                           0x40    12      6      0
            //  HOT AIR L                   On: 1, Off: 0                           0x80    12      7      0
            //  AFT ISOL VALVE U            On: 1, Off: 0                           0x01    13      0      1
            //  AFT ISOL VALVE L            On: 1, Off: 0                           0x02    13      1      1
            //  ANTI-ICE WING U             On: 1, Off: 0                           0x04    13      2      1
            //  ANTI-ICE WING L             On: 1, Off: 0                           0x08    13      3      0
            //  ANTI-ICE ENG1 U             On: 1, Off: 0                           0x10    13      4      0
            //  ANTI-ICE ENG1 L             On: 1, Off: 0                           0x20    13      5      0
            //  ANTI-ICE ENG2 U             On: 1, Off: 0                           0x40    13      6      0
            //  ANTI-ICE ENG2 L             On: 1, Off: 0                           0x80    13      7      0
            //  WINDOW HEAT L               On: 1, Off: 0                           0x01    14      0      1
            //  CREW SUPPLY L               On: 1, Off: 0                           0x02    14      1      1
            //  APU MASTER U                On: 1, Off: 0                           0x04    14      2      1
            //  APU MASTER L                On: 1, Off: 0                           0x08    14      3      0
            //  APU START U                 On: 1, Off: 0                           0x10    14      4      0
            //  APU START L                 On: 1, Off: 0                           0x20    14      5      0
            //  EMER EXIT LT L              On: 1, Off: 0                           0x40    14      6      0
            //  EXT_LT_STROBE_AUTO          On: 1, Off: 0                           0x80    14      7      0
            //  VOLT DISPLAY L              On: 1, Off: 0                           0x01    15      0      1
            //  VOLT DISPLAY R              On: 1, Off: 0                           0x02    15      1      1
            //  (Reserved)                  -                                       0x04    15      2      1
            //  (Reserved)                  -                                       0x08    15      3      0
            //  (Reserved)                  -                                       0x10    15      4      0
            //  (Reserved)                  -                                       0x20    15      5      0
            //  (Reserved)                  -                                       0x40    15      6      0
            //  (Reserved)                  -                                       0x80    15      7      0
            //  Data Type                   Single Byte Type                        -       16      -      0x02
            //  Data Length                 Following data occupies 3 Bytes         -       17      -      0x03
            //  Background Light Brightness 0x00(Minimum)-0xFF(Maximum)             -       18      -      0
            //  VOLT LIGHT Left Brightness  0x00(Minimum)-0xFF(Maximum)             -       19      -      0
            //  VOLT LIGHT Right Brightness 0x00(Minimum)-0xFF(Maximum)             -       20      -      0
            //  Data Type                   Double Byte Type                        -       21      -      0x03
            //  Data Length                 Following data occupies 8 Bytes         -       22      -      0x04
            //  VOLT DISPLAY VALUE L        High 8 bit of Uint16                    -       23      -      0x00
            //  VOLT DISPLAY VALUE L        Low 8 bit of Uint16                     -       24      -      0x00
            //  VOLT DISPLAY VALUE R        High 8 bit of Uint16                    -       25      -      0x00
            //  VOLT DISPLAY VALUE R        Low 8 bit of Uint16                     -       26      -      0x00

            state.ForEach(item =>
            {
                if (item.Type == DeviceType.LcdDisplay)
                {
                    UpdateLcdDisplayOutputState(item);
                    return;
                }

                var itemByte = item.Byte;

                if (itemByte >= 6 && itemByte <= 15)
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
                // Background Brightness = 18
                // Volt L Light Brightness = 19
                // Volt R Light Brightness = 20
                else if (itemByte == 18 || itemByte == 19 || itemByte == 20)
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

            var text = lcdDisplay.Text.Replace(".", "");
            var paddedText = text.PadLeft(lcdDisplay.Cols, '0');
            UpdateLcdDisplay(item.Byte, paddedText);

            if (lcdDisplay.Byte == VOLT_DISPLAY_LEFT)
            {
                // enable the volt display left
                LastOutputBufferState[15] |= (byte)(1);
            }
            else if (lcdDisplay.Byte == VOLT_DISPLAY_RIGHT)
            {
                // enable the volt display right
                LastOutputBufferState[15] |= (byte)(2);
            }

            // continue with next item,
            // as we have already processed the LCD display state
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
            // Name                 Note                                Mask    Byte[]  Bit[]  Example
            // Head                 Constant: 0xF2                              0       -      0xF2
            // Head                 Constant: 0xE1                              1       -      0xE1
            // Head                 Constant: 0x07                              2       -      0x07
            // Data Type Total      Has 2 Data Type                             3       -      0x02
            // Data Type            Bit Type                                    4       -      0x01
            // Data Length          Following data occupies 20 Bytes            5       -      0x14
            // IR1 NORM             Press: 1, Release: 0                0x01    6       0      1
            // IR1 PUSH             Press: 1, Release: 0                0x02    6       1      1
            // IR2 NORM             Press: 1, Release: 0                0x04    6       2      1
            // IR2 PUSH             Press: 1, Release: 0                0x08    6       3      1
            // IR3 NORM             Press: 1, Release: 0                0x10    6       4      1
            // IR3 PUSH             Press: 1, Release: 0                0x20    6       5      1
            // IR1 OFF              Press: 1, Release: 0                0x40    6       6      1
            // IR1 NAV              Press: 1, Release: 0                0x80    6       7      1
            // IR1 ATT              Press: 1, Release: 0                0x01    7       0      1
            // IR2 OFF              Press: 1, Release: 0                0x02    7       1      1
            // IR2 NAV              Press: 1, Release: 0                0x04    7       2      1
            // IR2 ATT              Press: 1, Release: 0                0x08    7       3      1
            // IR3 OFF              Press: 1, Release: 0                0x10    7       4      1
            // IR3 NAV              Press: 1, Release: 0                0x20    7       5      1
            // IR3 ATT              Press: 1, Release: 0                0x40    7       6      1
            // ENG1 AGENT1 NORM     Press: 1, Release: 0                0x80    7       7      1
            // ENG1 AGENT1 PUSH     Press: 1, Release: 0                0x01    8       0      1
            // ENG1 AGENT2 NORM     Press: 1, Release: 0                0x02    8       1      1
            // ENG1 AGENT2 PUSH     Press: 1, Release: 0                0x04    8       2      1
            // APU AGENT NORM       Press: 1, Release: 0                0x08    8       3      0
            // APU AGENT PUSH       Press: 1, Release: 0                0x10    8       4      0
            // ENG2 AGENT1 NORM     Press: 1, Release: 0                0x20    8       5      0
            // ENG2 AGENT1 PUSH     Press: 1, Release: 0                0x40    8       6      0
            // ENG2 AGENT2 NORM     Press: 1, Release: 0                0x80    8       7      0
            // ENG2 AGENT2 PUSH     Press: 1, Release: 0                0x01    9       0      1
            // ENG1 FIRE NORM       Press: 1, Release: 0                0x02    9       1      1
            // ENG1 FIRE POPOUT     Press: 1, Release: 0                0x04    9       2      1
            // APU FIRE NORM        Press: 1, Release: 0                0x08    9       3      0
            // APU FIRE POPOUT      Press: 1, Release: 0                0x10    9       4      0
            // ENG2 FIRE NORM       Press: 1, Release: 0                0x20    9       5      0
            // ENG2 FIRE POPOUT     Press: 1, Release: 0                0x40    9       6      0
            // ENG1 FIRE TEST       Press: 1, Release: 0                0x80    9       7      0
            // APU FIRE TEST        Press: 1, Release: 0                0x01    10      0      1
            // ENG2 FIRE TEST       Press: 1, Release: 0                0x02    10      1      1
            // XFEED NORM           Press: 1, Release: 0                0x04    10      2      1
            // XFEED PUSH           Press: 1, Release: 0                0x08    10      3      0
            // L TK PUMP1 NORM      Press: 1, Release: 0                0x10    10      4      0
            // L TK PUMP1 PUSH      Press: 1, Release: 0                0x20    10      5      0
            // L TK PUMP2 NORM      Press: 1, Release: 0                0x40    10      6      0
            // L TK PUMP2 PUSH      Press: 1, Release: 0                0x80    10      7      0
            // CTR TK PUMP1 NORM    Press: 1, Release: 0                0x01    11      0      1
            // CTR TK PUMP1 PUSH    Press: 1, Release: 0                0x02    11      1      1
            // MODE SEL NORM        Press: 1, Release: 0                0x04    11      2      1
            // MODE SEL PUSH        Press: 1, Release: 0                0x08    11      3      0
            // CTR TK PUMP2 NORM    Press: 1, Release: 0                0x10    11      4      0
            // CTR TK PUMP2 PUSH    Press: 1, Release: 0                0x20    11      5      0
            // R TK PUMP1 NORM      Press: 1, Release: 0                0x40    11      6      0
            // R TK PUMP1 PUSH      Press: 1, Release: 0                0x80    11      7      0
            // R TK PUMP2 NORM      Press: 1, Release: 0                0x01    12      0      1
            // R TK PUMP2 PUSH      Press: 1, Release: 0                0x02    12      1      1
            // COMMERCIAL NORM      Press: 1, Release: 0                0x04    12      2      1
            // COMMERCIAL PUSH      Press: 1, Release: 0                0x08    12      3      0
            // BAT1 NORM            Press: 1, Release: 0                0x10    12      4      0
            // BAT1 PUSH            Press: 1, Release: 0                0x20    12      5      0
            // BAT2 NORM            Press: 1, Release: 0                0x40    12      6      0
            // BAT2 PUSH            Press: 1, Release: 0                0x80    12      7      0
            // APU GEN NORM         Press: 1, Release: 0                0x01    13      0      1
            // APU GEN PUSH         Press: 1, Release: 0                0x02    13      1      1
            // EXT PWR              Press: 1, Release: 0                0x04    13      2      1
            // PACK FLOW LO         Press: 1, Release: 0                0x08    13      3      0
            // PACK FLOW NORM       Press: 1, Release: 0                0x10    13      4      0
            // PACK FLOW HI         Press: 1, Release: 0                0x20    13      5      0
            // COCKPIT PACK 1       Press: 1, Release: 0                0x40    13      6      0
            // COCKPIT PACK 2       Press: 1, Release: 0                0x80    13      7      0
            // COCKPIT PACK 3       Press: 1, Release: 0                0x01    14      0      1
            // COCKPIT PACK 4       Press: 1, Release: 0                0x02    14      1      1
            // COCKPIT PACK 5       Press: 1, Release: 0                0x04    14      2      1
            // COCKPIT PACK 6       Press: 1, Release: 0                0x08    14      3      0
            // COCKPIT PACK 7       Press: 1, Release: 0                0x10    14      4      0
            // FWD CABIN PACK 1     Press: 1, Release: 0                0x20    14      5      0
            // FWD CABIN PACK 2     Press: 1, Release: 0                0x40    14      6      0
            // FWD CABIN PACK 3     Press: 1, Release: 0                0x80    14      7      0
            // FWD CABIN PACK 4     Press: 1, Release: 0                0x01    15      0      1
            // FWD CABIN PACK 5     Press: 1, Release: 0                0x02    15      1      1
            // FWD CABIN PACK 6     Press: 1, Release: 0                0x04    15      2      1
            // FWD CABIN PACK 7     Press: 1, Release: 0                0x08    15      3      0
            // AFT CABIN PACK 1     Press: 1, Release: 0                0x10    15      4      0
            // AFT CABIN PACK 2     Press: 1, Release: 0                0x20    15      5      0
            // AFT CABIN PACK 3     Press: 1, Release: 0                0x40    15      6      0
            // AFT CABIN PACK 4     Press: 1, Release: 0                0x80    15      7      0
            // AFT CABIN PACK 5     Press: 1, Release: 0                0x01    16      0      1
            // AFT CABIN PACK 6     Press: 1, Release: 0                0x02    16      1      1
            // AFT CABIN PACK 7     Press: 1, Release: 0                0x04    16      2      1
            // PACK1 NORM           Press: 1, Release: 0                0x08    16      3      0
            // PACK1 PUSH           Press: 1, Release: 0                0x10    16      4      0
            // APU BLEED NORM       Press: 1, Release: 0                0x20    16      5      0
            // APU BLEED PUSH       Press: 1, Release: 0                0x40    16      6      0
            // PACK2 NORM           Press: 1, Release: 0                0x80    16      7      0
            // PACK2 PUSH           Press: 1, Release: 0                0x01    17      0      1
            // HOT AIR NORM         Press: 1, Release: 0                0x02    17      1      1
            // HOT AIR PUSH         Press: 1, Release: 0                0x04    17      2      1
            // AFT ISOL VALVE NORM  Press: 1, Release: 0                0x08    17      3      0
            // AFT ISOL VALVE PUSH  Press: 1, Release: 0                0x10    17      4      0
            // CARGO PACK 1         Press: 1, Release: 0                0x20    17      5      0
            // CARGO PACK 2         Press: 1, Release: 0                0x40    17      6      0
            // CARGO PACK 3         Press: 1, Release: 0                0x80    17      7      0
            // CARGO PACK 4         Press: 1, Release: 0                0x01    18      0      1
            // CARGO PACK 5         Press: 1, Release: 0                0x02    18      1      1
            // CARGO PACK 6         Press: 1, Release: 0                0x04    18      2      1
            // CARGO PACK 7         Press: 1, Release: 0                0x08    18      3      0
            // ANTI-ICE WING NORM   Press: 1, Release: 0                0x10    18      4      0
            // ANTI-ICE WING PUSH   Press: 1, Release: 0                0x20    18      5      0
            // ANTI-ICE ENG1 NORM   Press: 1, Release: 0                0x40    18      6      0
            // ANTI-ICE ENG1 PUSH   Press: 1, Release: 0                0x80    18      7      0
            // ANTI-ICE ENG2 NORM   Press: 1, Release: 0                0x01    19      0      1
            // ANTI-ICE ENG2 PUSH   Press: 1, Release: 0                0x02    19      1      1
            // WINDOW HEAT NORM     Press: 1, Release: 0                0x04    19      2      1
            // WINDOW HEAT PUSH     Press: 1, Release: 0                0x08    19      3      0
            // CREW SUPPLY NORM     Press: 1, Release: 0                0x10    19      4      0
            // CREW SUPPLY PUSH     Press: 1, Release: 0                0x20    19      5      0
            // GND CTL              Press: 1, Release: 0                0x40    19      6      0
            // CVR TEST             Press: 1, Release: 0                0x80    19      7      0
            // CALLS ALL            Press: 1, Release: 0                0x01    20      0      1
            // WIPER OFF            Press: 1, Release: 0                0x02    20      1      1
            // WIPER SLOW           Press: 1, Release: 0                0x04    20      2      1
            // WIPER FAST           Press: 1, Release: 0                0x08    20      3      0
            // STROBE OFF           Press: 1, Release: 0                0x10    20      4      0
            // STROBE AUTO          Press: 1, Release: 0                0x20    20      5      0
            // STROBE ON            Press: 1, Release: 0                0x40    20      6      0
            // BEACON OFF           Press: 1, Release: 0                0x80    20      7      0
            // BEACON ON            Press: 1, Release: 0                0x01    21      0      1
            // WING OFF             Press: 1, Release: 0                0x02    21      1      1
            // WING ON              Press: 1, Release: 0                0x04    21      2      1
            // NAV & LOGO OFF       Press: 1, Release: 0                0x08    21      3      0
            // NAV & LOGO 1         Press: 1, Release: 0                0x10    21      4      0
            // NAV & LOGO 2         Press: 1, Release: 0                0x20    21      5      0
            // RWY TURN-OFF OFF     Press: 1, Release: 0                0x40    21      6      0
            // RWY TURN-OFF ON      Press: 1, Release: 0                0x80    21      7      0
            // LAND L RETRACT       Press: 1, Release: 0                0x01    22      0      1
            // LAND L OFF           Press: 1, Release: 0                0x02    22      1      1
            // LAND L ON            Press: 1, Release: 0                0x04    22      2      1
            // LAND R RETRACT       Press: 1, Release: 0                0x08    22      3      0
            // LAND R OFF           Press: 1, Release: 0                0x10    22      4      0
            // LAND R ON            Press: 1, Release: 0                0x20    22      5      0
            // NOSE OFF             Press: 1, Release: 0                0x40    22      6      0
            // NOSE TAXI            Press: 1, Release: 0                0x80    22      7      0
            // NOSE T.O             Press: 1, Release: 0                0x01    23      0      1
            // APU MASTER NORM      Press: 1, Release: 0                0x02    23      1      1
            // APU MASTER PUSH      Press: 1, Release: 0                0x04    23      2      1
            // APU START            Press: 1, Release: 0                0x08    23      3      0
            // STBY COMPASS OFF     Press: 1, Release: 0                0x10    23      4      0
            // STBY COMPASS ON      Press: 1, Release: 0                0x20    23      5      0
            // DOME OFF             Press: 1, Release: 0                0x40    23      6      0
            // DOME DIM             Press: 1, Release: 0                0x80    23      7      0
            // DOME BRT             Press: 1, Release: 0                0x01    24      0      1
            // ANN LT DIM           Press: 1, Release: 0                0x02    24      1      1
            // ANN LT BRT           Press: 1, Release: 0                0x04    24      2      1
            // ANN LT TEST          Press: 1, Release: 0                0x08    24      3      0
            // SEAT BELTS OFF       Press: 1, Release: 0                0x10    24      4      0
            // SEAT BELTS ON        Press: 1, Release: 0                0x20    24      5      0
            // NO SMOKING OFF       Press: 1, Release: 0                0x40    24      6      0
            // NO SMOKING AUTO      Press: 1, Release: 0                0x80    24      7      0
            // NO SMOKING ON        Press: 1, Release: 0                0x01    25      0      1
            // EMER EXIT KEY NORN   Press: 1, Release: 0                0x02    25      1      1
            // EMER EXIT KEY PUSH   Press: 1, Release: 0                0x04    25      2      1
            // EMER EXIT LT OFF     Press: 1, Release: 0                0x08    25      3      0
            // EMER EXIT LT ARM     Press: 1, Release: 0                0x10    25      4      0
            // EMER EXIT LT ON      Press: 1, Release: 0                0x20    25      5      0
            // RAIN RPLNT           Press: 1, Release: 0                0x40    25      6      0
            // (Reserved)           -                                   0x80    25      7      0
            // Data Type            Single Byte Type                    -       26      -      0x02
            // Data Length          Following data occupies 6 Bytes     -       27      -      0x01
            // OVHD INTEG LT        0x00-0xFF(255)                      -       28      -      0

            JoystickState state = new JoystickState();

            // Buttons
            // copy the button states from the buffer to the Buttons bit by bit starting from byte 6 to byte 9
            for (int i = 0; i < 159; i++)
            {
                int byteIndex = 6 + (i / 8);
                int bitIndex = i % 8;
                bool isPressed = (LastInputBufferState[byteIndex] & (1 << bitIndex)) != 0;
                state.Buttons[i] = isPressed;
            }

            // Axes
            // Overhead Integ Light Brightness
            state.X = LastInputBufferState[28];

            return state;
        }
    }
}