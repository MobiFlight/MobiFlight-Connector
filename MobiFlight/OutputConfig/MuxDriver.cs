using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.OutputConfig
{
    public class MuxDriver
    {
        public String[] PinSx { get; set; }

        public MuxDriver()
        {
            PinSx[0] = "";
            PinSx[1] = "";
            PinSx[2] = "";
            PinSx[3] = "";
        }

        public override bool Equals(Object obj)
        {
            return (obj == this);
        }

        public void ReadXml(XmlReader reader)
        {
            for(var i=0; )
            
            if (reader["muxDriverPinS0"] != null && reader["muxDriverPinS0"] != "")
            {
                PinSx[0] = reader["muxDriverPinS0"];
            }

            if (reader["shiftRegister"] != null && reader["shiftRegister"] != "")
            {
                Address = reader["shiftRegister"];
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("shiftRegister", Address);
            writer.WriteAttributeString("registerOutputPin", Pin);
        }

        public object Clone()
        {
            return null;    // Singleton, can't be cloned
        }
    }
}
