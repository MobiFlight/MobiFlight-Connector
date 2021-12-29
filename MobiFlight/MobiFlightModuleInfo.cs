using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public class MobiFlightModuleInfo : IModuleInfo
    {
        String _version = null;
        public String Type { get; set; }
        public String Serial { get; set; }
        public String Port { get; set; }
        public String Name { get; set; }
        public String Config { get; set; }

        public List<Board> Boards { get; set; }

        public Board Board
        {
            get
            {
                return Boards?[0];
            }
        }

        public String Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public bool HasMfFirmware()
        {
            return !String.IsNullOrEmpty(Version);
        }
    }
}
