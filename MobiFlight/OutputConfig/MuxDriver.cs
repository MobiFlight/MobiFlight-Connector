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
    public static class MuxDriver
    {
        private static String[] PinSx;
        private static bool initialized; 

        static MuxDriver()
        {
            PinSx[0] = "";
            PinSx[1] = "";
            PinSx[2] = "";
            PinSx[3] = "";
            initialized = false;
        }
        public static bool isInitialized()
        {
            return initialized;
        }
        public static void Clear()
        {
            initialized = false;
        }
        public static void ReadXml(XmlReader reader)
        {
            String pinName;
            for(var i=0; i<4; i++) {
                pinName = $"muxDriverPinS{i}";
                if (reader[pinName] != null && reader[pinName] != "") {
                    PinSx[i] = reader[pinName];
                }
            }
            initialized = true;
        }

        public static void WriteXml(XmlWriter writer)
        {
            String pinName;
            for (var i = 0; i < 4; i++) {
                pinName = $"muxDriverPinS{i}";
                writer.WriteAttributeString(pinName, PinSx[i]);
            }
        }
    }
}
