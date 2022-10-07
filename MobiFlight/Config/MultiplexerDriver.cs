using System;
using System.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    // MultiplexerDriver is not really used as a full-fledged BaseDevice (though it could really well be);
    // it is mainly embedded in other devices which are mux clients.
    // However, it inherits from BaseDevice because:
    // 1 - it uses many of the features of the base class in the very same capacity
    // 2 - this way it can easily be returned to the role of full Device, should it be required
    public class MultiplexerDriver: BaseDevice
    {
        const ushort _paramCount = 4;
        private int _initCounter;

        [XmlAttribute]
        public String[] PinSx = { "-1", "-2", "-3", "-4" };

        public MultiplexerDriver()
        {
            Name = "MultiplexerDriver"; 
            _type = DeviceType.MultiplexerDriver;
            Reset();
        }

        public bool isInitialized()
        {
            return (_initCounter>0);
        }

        public bool registerClient()
        {
            bool res = false;
            if (_initCounter > 0) {
                _initCounter++;
                res = true;
            }
            return res;
        }

        public bool unregisterClient()
        {
            bool res = false;
            if (isInitialized()) {
                if (--_initCounter == 0) Reset();
                res = true;
            }
            return res;
        }

        public bool Initialize(String[] pinNos)
        {
            if (pinNos.Length < 4) return false;
            for (var i = 0; i < 4; i++) {
                if (byte.Parse(pinNos[i]) < 0) return false;
            }
            Array.Copy(pinNos, PinSx, 4); 
            if(_initCounter == 0) _initCounter++;
            return true;
        }

        public void Reset()
        {
            PinSx[0] = "-1";
            PinSx[1] = "-2";
            PinSx[2] = "-3";
            PinSx[3] = "-4";
            _initCounter = 0;
        }

        public String ToInternalStripped()
        {
            // Differs from "standard" ToInternal() because it only returns a partial string
            // to be embedded in the string returned by devices using the multiplexer 
            return PinSx[0] + Separator
                 + PinSx[1] + Separator
                 + PinSx[2] + Separator
                 + PinSx[3] + Separator;
        }
        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                 + PinSx[0] + Separator
                 + PinSx[1] + Separator
                 + PinSx[2] + Separator
                 + PinSx[3] + Separator
                 + Name + End;
        }

        override public bool FromInternal(String value)
        {
            // Cut end character if present
            if (value.Length == value.IndexOf(End) + 1) value = value.Substring(0, value.Length - 1);
            String[] paramList = value.Split(Separator);
            if (paramList.Count() != _paramCount + 1) {
                throw new ArgumentException("Param count does not match. " + paramList.Count() + " given, " + _paramCount + " expected");
            }

            bool res = true;
            if (!isInitialized()) {
                var pins = new string[4];
                pins[0] = paramList[1];
                pins[1] = paramList[2];
                pins[2] = paramList[3];
                pins[3] = paramList[4];
                res = Initialize(pins);
            } else {
                _initCounter++;
            }

            return res;
        }

        public override string ToString()
        {
            return $"{Type}:{Name}";
        }
    }


}