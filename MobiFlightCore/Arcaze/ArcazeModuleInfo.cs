using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobiFlight;
using SimpleSolutions.Usb;

namespace MobiFlight
{
    public class ArcazeModuleInfo : IModuleInfo
    {
        DeviceInfo _info;
        public const String TYPE = "ArcazeUSB";

        public ArcazeModuleInfo(DeviceInfo info)
        {
            _info = info;
        }

        public String Version
        {
            get { return _info.DeviceAttributes.VersionNumber.ToString(); }
            set { throw new NotImplementedException(); }
        }

        public string Type
        {
            get
            {
                return TYPE;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Serial
        {
            get
            {
                return _info.Serial;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Port
        {
            get
            {
                return _info.Path;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                return _info.DeviceName;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
