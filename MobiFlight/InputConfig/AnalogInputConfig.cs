using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.InputConfig
{

    public class AnalogInputConfig : IXmlSerializable, ICloneable
    {
        public InputAction onChange;        


        public object Clone()
        {
            AnalogInputConfig clone = new AnalogInputConfig();
            if (onChange != null) clone.onChange = (InputAction) onChange.Clone();
            return clone;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return (null);
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {

            reader.Read(); // this should be the opening tag "onChange"
            if (reader.LocalName == "") reader.Read();
            if (reader.LocalName == "onChange")
            {
                switch (reader["type"])
                {
                    case FsuipcOffsetInputAction.TYPE:
                        onChange = new FsuipcOffsetInputAction();
                        onChange.ReadXml(reader);
                        reader.Read(); // this should be the closing tag "onChange"
                        break;

                    case KeyInputAction.TYPE:
                        onChange = new KeyInputAction();
                        onChange.ReadXml(reader);
                        break;

                    case EventIdInputAction.TYPE:
                        onChange = new EventIdInputAction();
                        onChange.ReadXml(reader);
                        break;

                    case PmdgEventIdInputAction.TYPE:
                        onChange = new PmdgEventIdInputAction();
                        onChange.ReadXml(reader);
                        break;

                    case JeehellInputAction.TYPE:
                        onChange = new JeehellInputAction();
                        onChange.ReadXml(reader);
                        break;

                    case LuaMacroInputAction.TYPE:
                        onChange = new LuaMacroInputAction();
                        onChange.ReadXml(reader);
                        break;

                    case RetriggerInputAction.TYPE:
                        onChange = new RetriggerInputAction();
                        onChange.ReadXml(reader);
                        break;

                    case VJoyInputAction.TYPE:
                        onChange = new VJoyInputAction();
                        onChange.ReadXml(reader);
                        break;

                    case MSFS2020EventIdInputAction.TYPE:
                        onChange = new MSFS2020EventIdInputAction();
                        onChange.ReadXml(reader);
                        break;

                    case VariableInputAction.TYPE:
                        onChange = new VariableInputAction();
                        onChange.ReadXml(reader);
                        break;

                    case MSFS2020CustomInputAction.TYPE:
                        onChange = new MSFS2020CustomInputAction();
                        onChange.ReadXml(reader);
                        break;
                }
            }         
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("onChange");
            if (onChange != null) onChange.WriteXml(writer);
            writer.WriteEndElement();
        }

        internal void execute(FSUIPC.Fsuipc2Cache fsuipcCache, 
                                SimConnectMSFS.SimConnectCache simConnectCache, 
                                MobiFlightCache moduleCache, 
                                InputEventArgs args, 
                                List<ConfigRefValue> configRefs)
        {
            if (onChange != null)
            {
                Log.Instance.log("Executing Change: " + args.DeviceId + "@" + args.Serial, LogSeverity.Debug);
                onChange.execute(fsuipcCache, simConnectCache, moduleCache, args, configRefs);
            }
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is AnalogInputConfig &&
                (
                    (onChange == null && ((obj as AnalogInputConfig).onChange == null)) ||
                    (onChange != null && onChange.Equals((obj as AnalogInputConfig).onChange))
                );
        }
    }

}
