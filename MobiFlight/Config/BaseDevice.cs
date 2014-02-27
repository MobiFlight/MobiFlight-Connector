using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class BaseDevice : IConfigItem
    {
        protected MobiFlightModule.DeviceType _type = MobiFlightModule.DeviceType.NotSet;
        protected const char separator = '.';
        
        [XmlAttribute]
        public String Name { get; set; }

        virtual public String ToInternal() { return ((int)_type).ToString(); }
        virtual public bool FromInternal(String value) { throw new NotImplementedException(); }

        [XmlAttribute]
        public MobiFlightModule.DeviceType Type
        {
            get { return _type; }
        }
    }
}
