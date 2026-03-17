using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    class MobiFlightIOBasic
    {
        public String Port { get; set; }
        public int Pin { get; set; }

        public MobiFlightIOBasic() { Port = "Default"; Pin = -1; }

        public MobiFlightIOBasic(string portPin)
        {
            Parse(portPin);
        }

        /// <summary>
        /// parse a given port and pin combo like "A01"
        /// </summary>
        /// <param name="portPin"></param>
        public void Parse(string portPin)
        {
            if (portPin.Length < 3) throw new ArgumentException("portPin has wrong format");

            Pin     = int.Parse(portPin.Substring(portPin.Length - 2));
            Port    = portPin.Substring(0, portPin.Length - 2);
        }

        public bool isValid()
        {
            return (Pin != -1);
        }
    }
}
