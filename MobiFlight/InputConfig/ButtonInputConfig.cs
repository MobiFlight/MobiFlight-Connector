using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.InputConfig
{
    public class ButtonInputConfig : IXmlSerializable, ICloneable
    {
        public InputAction onPress;
        public InputAction onRelease;


        public object Clone()
        {
            ButtonInputConfig clone = new ButtonInputConfig();
            if (onPress != null) clone.onPress = (InputAction)onPress.Clone();
            if (onRelease != null) clone.onRelease = (InputAction)onRelease.Clone();
            return clone;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return (null);
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {

            reader.Read(); // this should be the opening tag "onPress"
            if (reader.LocalName == "") reader.Read();
            if (reader.LocalName == "onPress")
            {
                switch (reader["type"])
                {
                    case FsuipcOffsetInputAction.TYPE:
                        onPress = new FsuipcOffsetInputAction();
                        onPress.ReadXml(reader);
                        break;

                    case KeyInputAction.TYPE:
                        onPress = new KeyInputAction();
                        onPress.ReadXml(reader);
                        break;

                    case EventIdInputAction.TYPE:
                        onPress = new EventIdInputAction();
                        onPress.ReadXml(reader);
                        break;

                    case PmdgEventIdInputAction.TYPE:
                        onPress = new PmdgEventIdInputAction();
                        onPress.ReadXml(reader);
                        break;

                    case JeehellInputAction.TYPE:
                        onPress = new JeehellInputAction();
                        onPress.ReadXml(reader);
                        break;

                    case LuaMacroInputAction.TYPE:
                        onPress = new LuaMacroInputAction();
                        onPress.ReadXml(reader);
                        break;

                    case RetriggerInputAction.TYPE:
                        onPress = new RetriggerInputAction();
                        onPress.ReadXml(reader);
                        break;

                    case VJoyInputAction.TYPE:
                        onPress = new VJoyInputAction();
                        onPress.ReadXml(reader);
                        break;

                    case MSFS2020EventIdInputAction.TYPE:
                        onPress = new MSFS2020EventIdInputAction();
                        onPress.ReadXml(reader);
                        break;

                    case VariableInputAction.TYPE:
                        onPress = new VariableInputAction();
                        onPress.ReadXml(reader);
                        break;

                    case MSFS2020CustomInputAction.TYPE:
                        onPress = new MSFS2020CustomInputAction();
                        onPress.ReadXml(reader);
                        break;
                }
                reader.Read(); // Closing onPress
            }

            if (reader.LocalName == "") reader.Read();
            if (reader.LocalName == "onRelease")
            {
                switch (reader["type"])
                {
                    case FsuipcOffsetInputAction.TYPE:
                        onRelease = new FsuipcOffsetInputAction();
                        onRelease.ReadXml(reader);
                        break;

                    case KeyInputAction.TYPE:
                        onRelease = new KeyInputAction();
                        onRelease.ReadXml(reader);
                        break;

                    case EventIdInputAction.TYPE:
                        onRelease = new EventIdInputAction();
                        onRelease.ReadXml(reader);
                        break;

                    case PmdgEventIdInputAction.TYPE:
                        onRelease = new PmdgEventIdInputAction();
                        onRelease.ReadXml(reader);
                        break;

                    case JeehellInputAction.TYPE:
                        onRelease = new JeehellInputAction();
                        onRelease.ReadXml(reader);
                        break;

                    case LuaMacroInputAction.TYPE:
                        onRelease = new LuaMacroInputAction();
                        onRelease.ReadXml(reader);
                        break;

                    case RetriggerInputAction.TYPE:
                        onRelease = new RetriggerInputAction();
                        onRelease.ReadXml(reader);
                        break;

                    case VJoyInputAction.TYPE:
                        onRelease = new VJoyInputAction();
                        onRelease.ReadXml(reader);
                        break;
                    case MSFS2020EventIdInputAction.TYPE:
                        onRelease = new MSFS2020EventIdInputAction();
                        onRelease.ReadXml(reader);
                        break;

                    case VariableInputAction.TYPE:
                        onRelease = new VariableInputAction();
                        onRelease.ReadXml(reader);
                        break;

                    case MSFS2020CustomInputAction.TYPE:
                        onRelease = new MSFS2020CustomInputAction();
                        onRelease.ReadXml(reader);
                        break;
                }

                reader.Read(); // closing onRelease
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("onPress");
            if (onPress != null) onPress.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("onRelease");
            if (onRelease != null) onRelease.WriteXml(writer);
            writer.WriteEndElement();
        }

        internal void execute(FSUIPC.Fsuipc2Cache fsuipcCache, SimConnectMSFS.SimConnectCache simConnectCache, MobiFlightCache moduleCache, InputEventArgs args, List<ConfigRefValue> configRefs)
        {
            if (args.Value == 0 && onPress != null)
            {
                Log.Instance.log("Executing OnPress: " + args.DeviceId + "@" + args.Serial, LogSeverity.Debug);
                onPress.execute(fsuipcCache, simConnectCache, moduleCache, args, configRefs);
            }
            else if (args.Value == 1 && onRelease != null)
            {
                Log.Instance.log("Executing OnRelease: " + args.DeviceId + "@" + args.Serial, LogSeverity.Debug);
                onRelease.execute(fsuipcCache, simConnectCache, moduleCache, args, configRefs);
            }

        }

        public Dictionary<String, int> GetStatistics()
        {
            Dictionary<String, int> result = new Dictionary<string, int>();

            result["Input.Button"] = 1;

            if (onPress != null)
            {
                result["Input.OnPress"] = 1;
                result["Input." + onPress.GetType().Name] = 1;
            }

            if (onRelease != null)
            {
                result["Input.OnPress"] = 1;
                result["Input." + onRelease.GetType().Name] = 1;
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is ButtonInputConfig && 
                (
                    (onPress == null && ((obj as ButtonInputConfig).onPress == null)) || 
                    (onPress != null && onPress.Equals((obj as ButtonInputConfig).onPress))
                ) &&
                (
                    (onRelease == null && ((obj as ButtonInputConfig).onRelease == null)) ||
                    (onRelease != null && onRelease.Equals((obj as ButtonInputConfig).onRelease))
                );
        }
    }
}
