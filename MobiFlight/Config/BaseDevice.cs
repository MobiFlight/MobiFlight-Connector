using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class BaseDevice : IConfigItem
    {
        protected DeviceType _type = DeviceType.NotSet;
        public const char Separator = '.';
        public const char End = ':';
        
        [XmlAttribute]
        public String Name { get; set; }

        virtual public String ToInternal() { return ((int)_type).ToString(); }
        virtual public bool FromInternal(String value) { throw new NotImplementedException(); }

        [XmlAttribute]
        public DeviceType Type
        {
            get { return _type; }
        }
    }
}
