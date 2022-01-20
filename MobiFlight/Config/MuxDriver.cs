using System;
using System.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public sealed class MuxDriverS : BaseDevice
    {
        const ushort _paramCount = 4;
        private int _initCounter;
        [XmlAttribute]
        private String[] _pins = { "-1", "-2", "-3", "-4" };
        public String[] PinSx  { get => _pins; }

        public MuxDriverS()
        {
            Name = "MuxDriver"; 
            _type = DeviceType.MuxDriver;
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
                if (byte.Parse(pinNos[i]) <= 0) return false;
            }
            Array.Copy(pinNos, _pins, 4); 
            if(_initCounter == 0) _initCounter++;
            return true;
        }

        public void Reset()
        {
            _pins[0] = "-1";
            _pins[1] = "-2";
            _pins[2] = "-3";
            _pins[3] = "-4";
            _initCounter = 0;
        }


        override public String ToInternal()
        {
            // Differs from other devices' "ToInternal" because it only returns a partial string
            // to be embedded in the string returned by devices using the multiplexer 
            return _pins[0] + Separator
                 + _pins[1] + Separator
                 + _pins[2] + Separator
                 + _pins[3] + Separator;
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


    // !!!!!!!!  DEPRECATED  !!!!!!!!!!
    //
    //public class MuxDriver : BaseDevice
    //{
    //    const ushort _paramCount = 5;

    //    [XmlAttribute]
    //    public String[] PinSx = { "-1", "-2", "-3", "-4" };

    //    public MuxDriver() {  Name = "MuxDriver"; _type = DeviceType.MuxDriver; }

    //    override public String ToInternal()
    //    {
    //        return base.ToInternal() + Separator
    //             + PinSx[0] + Separator
    //             + PinSx[1] + Separator
    //             + PinSx[2] + Separator 
    //             + PinSx[3] + Separator
    //             + Name + End;
    //    }

    //    override public bool FromInternal(String value)
    //    {
    //        if (value.Length == value.IndexOf(End) + 1) value = value.Substring(0, value.Length - 1);
    //        String[] paramList = value.Split(Separator);
    //        if (paramList.Count() != _paramCount + 1)
    //        {
    //            throw new ArgumentException("Param count does not match. " + paramList.Count() + " given, " + _paramCount + " expected");
    //        }

    //        PinSx[0] = paramList[1];
    //        PinSx[1] = paramList[2];
    //        PinSx[2] = paramList[3];
    //        PinSx[3] = paramList[4];
    //        // Name = paramList[5];     // Single instance, internal use; no need to change name

    //        return true;
    //    }

    //    public override string ToString()
    //    {
    //        return $"{Type}:{Name}";
    //    }
    //}
}