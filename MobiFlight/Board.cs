using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public class Board
    {
        public int MaxOutputs = 0;
        public int MaxButtons = 0;
        public int MaxLedSegments = 0;
        public int MaxEncoders = 0;
        public int MaxSteppers = 0;
        public int MaxServos = 0;
        public int MaxLcdI2C = 0;
        public int MaxAnalogInputs = 0;

        public String LatestFirmwareVersion;
        public String Name;
        public List<String> HardwareIds;

        public int MessageSize;
        public int EEPROMSize;

        public List<MobiFlightPin> Pins;
    }
}
