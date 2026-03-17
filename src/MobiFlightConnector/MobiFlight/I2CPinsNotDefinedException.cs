using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    class I2CPinsNotDefinedException : Exception
    {
        public String DeviceType;
        public I2CPinsNotDefinedException(String DeviceType)
            : base(String.Format("{0} requires I2C pins however none are defined for the selected board.", DeviceType))
        {
            this.DeviceType = DeviceType;
        }
    }
}
