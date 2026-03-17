using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    class I2CPinInUseException : Exception
    {
        public String DeviceType;
        public MobiFlightPin Pin;
        public I2CPinInUseException(String DeviceType, MobiFlightPin Pin)
            : base(String.Format("{0} requires the use of pin {1} which is already assigned to another module. Remove the pin assignment from the other module then try adding {0} again.", DeviceType, Pin.Pin))
        {
            this.DeviceType = DeviceType;
            this.Pin = Pin;
        }
    }
}
