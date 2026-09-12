using System;

namespace MobiFlight.Joysticks.WingFlex
{
    internal class GmpReport
    {
        private byte[] LastInputBufferState = new byte[5];
        
        public GmpReport() {}

        public void CopyFromInputBuffer(byte[] inputBuffer)
        {
            if (inputBuffer == null || inputBuffer.Length < LastInputBufferState.Length)
            {
                throw new ArgumentException($"Invalid input buffer length. Expected {LastInputBufferState.Length}, got {inputBuffer?.Length ?? 0}");
            }
            LastInputBufferState = (byte[])inputBuffer?.Clone();
        }

        public GmpReport Parse(byte[] inputBuffer)
        {
            var result = new GmpReport();
            result.CopyFromInputBuffer(inputBuffer);

            return result;
        }

        public JoystickState ToJoystickState()
        {
            //  Byte    Bit     Bit length  Type    Name                        Description	Value           Range                   Default
            //  1       0       1	        bool    Brake Fan Key               Brake fan button            [0=Released, 1=Pressed] 0~1
            //  1       1       1           bool    AutoBrake Low Key           Autobrake LOW position      [0=Released, 1=Pressed]	0~1	false
            //  1       2       1           bool    AutoBrake Medium Key        Autobrake MEDIUM position   [0=Released, 1=Pressed]	0~1	false
            //  1       3       1           bool    AutoBrake Max Key           Autobrake MAX position      [0=Released, 1=Pressed]	0~1	false
            //  1       4       1           bool    Terrain On ND Key           Terrain on ND button        [0=Released, 1=Pressed]	0~1	false
            //  1       5       1           bool    Anti Skid Switch            Anti-skid switch            [0=OFF, 1=ON]	0~1	
            //  1       6       1           bool    RST Encoder Left            RST encoder rotated left    [0=OFF, 1=ON]	0~1	
            //  1       7       1           bool    RST Encoder Right           RST encoder rotated right   [0=OFF, 1=ON]	0~1	
            //  2       0       1           bool    RST Encoder Press           RST encoder pressed         [0=OFF, 1=ON]	0~1	
            //  2       1       1           bool    CHR Encoder Left            CHR encoder rotated left    [0=OFF, 1=ON]	0~1	
            //  2       2       1           bool    CHR Encoder Right           CHR encoder rotated right   [0=OFF, 1=ON]	0~1	
            //  2       3       1           bool    CHR Encoder Press           CHR encoder pressed         [0=OFF, 1=ON]	0~1	
            //  2       4       1           bool    SET Encoder Left            SET encoder rotated left    [0=OFF, 1=ON]	0~1	
            //  2       5       1           bool    SET Encoder Right           SET encoder rotated right   [0=OFF, 1=ON]	0~1	
            //  2       6       1           bool    SET Encoder Press           SET encoder pressed         [0=OFF, 1=ON]	0~1	
            //  2       7       1           bool    Reserved	                                            [0=OFF, 1=ON]	Reserved	
            //  3       0       1           bool    GPS1                        GPS selector position 1     [0=OFF, 1=ON]	0~1	
            //  3       1       1           bool    GPS2                        GPS selector position 2     [0=OFF, 1=ON]	0~1	
            //  3       2       1           bool    GPS3                        GPS selector position 3     [0=OFF, 1=ON]	0~1	
            //  3       3       1           bool    RUN1                        RUN selector position 1     [0=OFF, 1=ON]	0~1	
            //  3       4       1           bool    RUN2                        RUN selector position 2     [0=OFF, 1=ON]	0~1	
            //  3       5       1           bool    RUN3                        RUN selector position 3     [0=OFF, 1=ON]	0~1	
            //  3       6       1           bool    Landing Gear Handle Up      Landing gear handle up      [0=OFF, 1=ON]	0~1	
            //  3       7       1           bool    Landing Gear Handle Down    Landing gear handle down    [0=OFF, 1=ON]	0~1	
            //  4       0       16          uint16  Landing Gear Handle	        Landing gear handle	                        0~1023	
            //  6       0       16          uint16  Light Sensor                Ambient light sensor	    0~127	
            JoystickState state = new JoystickState();

            // Buttons
            // copy the button states from the buffer to the Buttons bit by bit starting from byte 6 to byte 8
            var startingByte = 1;
            for (int i = 0; i < 25; i++)
            {
                int byteIndex = startingByte + (i / 8);
                int bitIndex = i % 8;
                bool isPressed = (LastInputBufferState[byteIndex] & (1 << bitIndex)) != 0;
                state.Buttons[i] = isPressed;
            }

            // Axes
            // Landing Gear Handle
            state.X = (int) (LastInputBufferState[4] << 8 | LastInputBufferState[5]);

            // Light sensor value
            state.Y = LastInputBufferState[6];

            return state;
        }
    }
}