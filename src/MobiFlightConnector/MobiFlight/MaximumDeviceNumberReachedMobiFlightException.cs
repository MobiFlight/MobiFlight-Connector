using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    class MaximumDeviceNumberReachedMobiFlightException : Exception
    {
        public String DeviceType;
        public int MaxNumber;
        public MaximumDeviceNumberReachedMobiFlightException(String DeviceType, int MaxNumber)
            : base(String.Format("Max number of {0} is {1}", DeviceType, MaxNumber))
        {
            this.DeviceType = DeviceType;
            this.MaxNumber = MaxNumber;
        }
    }
}
