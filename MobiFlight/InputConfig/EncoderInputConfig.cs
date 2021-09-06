using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.InputConfig 
{
    public class EncoderInputConfig : IXmlSerializable, ICloneable
    {
        public InputAction onLeft;
        public InputAction onLeftFast;
        public InputAction onRight;
        public InputAction onRightFast;
        
        public object Clone()
        {
            EncoderInputConfig clone = new EncoderInputConfig();
            if (onLeft != null) clone.onLeft = (InputAction)onLeft.Clone();
            if (onLeftFast != null) clone.onLeftFast = (InputAction)onLeftFast.Clone();
            if (onRight != null) clone.onRight = (InputAction)onRight.Clone();
            if (onRightFast != null) clone.onRightFast = (InputAction)onRightFast.Clone();
            return clone;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return (null);
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.Read(); // this should be the opening tag "onLeft"
            if (reader.LocalName == "onLeft")
            {
                switch (reader["type"])
                {
                    case FsuipcOffsetInputAction.TYPE:
                        onLeft = new FsuipcOffsetInputAction();
                        onLeft.ReadXml(reader);
                        break;

                    case KeyInputAction.TYPE:
                        onLeft = new KeyInputAction();
                        onLeft.ReadXml(reader);
                        break;

                    case EventIdInputAction.TYPE:
                        onLeft = new EventIdInputAction();
                        onLeft.ReadXml(reader);
                        break;

                    case PmdgEventIdInputAction.TYPE:
                        onLeft = new PmdgEventIdInputAction();
                        onLeft.ReadXml(reader);
                        break;

                    case JeehellInputAction.TYPE:
                        onLeft = new JeehellInputAction();
                        onLeft.ReadXml(reader);
                        break;

                    case LuaMacroInputAction.TYPE:
                        onLeft = new LuaMacroInputAction();
                        onLeft.ReadXml(reader);
                        break;

                    case MSFS2020EventIdInputAction.TYPE:
                        onLeft = new MSFS2020EventIdInputAction();
                        onLeft.ReadXml(reader);
                        break;

                    case VariableInputAction.TYPE:
                        onLeft = new VariableInputAction();
                        onLeft.ReadXml(reader);
                        break;

                    case MSFS2020CustomInputAction.TYPE:
                        onLeft = new MSFS2020CustomInputAction();
                        onLeft.ReadXml(reader);
                        break;
                }

                reader.Read(); // advance to the next
            }

            if (reader.LocalName == "onLeftFast")
            {
                switch (reader["type"])
                {
                    case FsuipcOffsetInputAction.TYPE:
                        onLeftFast = new FsuipcOffsetInputAction();
                        onLeftFast.ReadXml(reader);
                        break;

                    case KeyInputAction.TYPE:
                        onLeftFast = new KeyInputAction();
                        onLeftFast.ReadXml(reader);
                        break;

                    case EventIdInputAction.TYPE:
                        onLeftFast = new EventIdInputAction();
                        onLeftFast.ReadXml(reader);
                        break;

                    case PmdgEventIdInputAction.TYPE:
                        onLeftFast = new PmdgEventIdInputAction();
                        onLeftFast.ReadXml(reader);
                        break;

                    case JeehellInputAction.TYPE:
                        onLeftFast = new JeehellInputAction();
                        onLeftFast.ReadXml(reader);
                        break;

                    case LuaMacroInputAction.TYPE:
                        onLeftFast = new LuaMacroInputAction();
                        onLeftFast.ReadXml(reader);
                        break;

                    case MSFS2020EventIdInputAction.TYPE:
                        onLeftFast = new MSFS2020EventIdInputAction();
                        onLeftFast.ReadXml(reader);
                        break;

                    case VariableInputAction.TYPE:
                        onLeftFast = new VariableInputAction();
                        onLeftFast.ReadXml(reader);
                        break;

                    case MSFS2020CustomInputAction.TYPE:
                        onLeftFast = new MSFS2020CustomInputAction();
                        onLeftFast.ReadXml(reader);
                        break;
                }

                reader.Read(); // advance to the next
            }

            if (reader.LocalName == "onRight")
            {
                switch (reader["type"])
                {
                    case FsuipcOffsetInputAction.TYPE:
                        onRight = new FsuipcOffsetInputAction();
                        onRight.ReadXml(reader);
                        break;

                    case KeyInputAction.TYPE:
                        onRight = new KeyInputAction();
                        onRight.ReadXml(reader);
                        break;

                    case EventIdInputAction.TYPE:
                        onRight = new EventIdInputAction();
                        onRight.ReadXml(reader);
                        break;

                    case PmdgEventIdInputAction.TYPE:
                        onRight = new PmdgEventIdInputAction();
                        onRight.ReadXml(reader);
                        break;

                    case JeehellInputAction.TYPE:
                        onRight = new JeehellInputAction();
                        onRight.ReadXml(reader);
                        break;

                    case LuaMacroInputAction.TYPE:
                        onRight = new LuaMacroInputAction();
                        onRight.ReadXml(reader);
                        break;

                    case MSFS2020EventIdInputAction.TYPE:
                        onRight = new MSFS2020EventIdInputAction();
                        onRight.ReadXml(reader);
                        break;

                    case VariableInputAction.TYPE:
                        onRight = new VariableInputAction();
                        onRight.ReadXml(reader);
                        break;

                    case MSFS2020CustomInputAction.TYPE:
                        onRight = new MSFS2020CustomInputAction();
                        onRight.ReadXml(reader);
                        break;
                }

                reader.Read(); // advance to the next
            }

            if (reader.LocalName == "onRightFast")
            {
                switch (reader["type"])
                {
                    case FsuipcOffsetInputAction.TYPE:
                        onRightFast = new FsuipcOffsetInputAction();
                        onRightFast.ReadXml(reader);
                        break;

                    case KeyInputAction.TYPE:
                        onRightFast = new KeyInputAction();
                        onRightFast.ReadXml(reader);
                        break;

                    case EventIdInputAction.TYPE:
                        onRightFast = new EventIdInputAction();
                        onRightFast.ReadXml(reader);
                        break;

                    case PmdgEventIdInputAction.TYPE:
                        onRightFast = new PmdgEventIdInputAction();
                        onRightFast.ReadXml(reader);
                        break;

                    case JeehellInputAction.TYPE:
                        onRightFast = new JeehellInputAction();
                        onRightFast.ReadXml(reader);
                        break;

                    case LuaMacroInputAction.TYPE:
                        onRightFast = new LuaMacroInputAction();
                        onRightFast.ReadXml(reader);
                        break;

                    case MSFS2020EventIdInputAction.TYPE:
                        onRightFast = new MSFS2020EventIdInputAction();
                        onRightFast.ReadXml(reader);
                        break;

                    case VariableInputAction.TYPE:
                        onRightFast = new VariableInputAction();
                        onRightFast.ReadXml(reader);
                        break;

                    case MSFS2020CustomInputAction.TYPE:
                        onRightFast = new MSFS2020CustomInputAction();
                        onRightFast.ReadXml(reader);
                        break;
                }

                reader.Read(); // advance to the next
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("onLeft");
            if (onLeft != null) onLeft.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("onLeftFast");
            if (onLeftFast != null) onLeftFast.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("onRight");
            if (onRight != null) onRight.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("onRightFast");
            if (onRightFast != null) onRightFast.WriteXml(writer);
            writer.WriteEndElement();
        }

        internal void execute(FSUIPC.Fsuipc2Cache fsuipcCache, SimConnectMSFS.SimConnectCache simConnectCache, MobiFlightCache moduleCache, InputEventArgs args, List<ConfigRefValue> configRefs)
        {
            if ((args.Value == 0 && onLeft != null) || (args.Value == 1 && onLeftFast == null))
            {
                Log.Instance.log("Executing OnLeft: " + args.DeviceId + "@" + args.Serial, LogSeverity.Debug);
                onLeft.execute(fsuipcCache, simConnectCache, moduleCache, args, configRefs);
            }
            else if (args.Value == 1 && onLeftFast != null)
            {
                Log.Instance.log("Executing OnLeftFast: " + args.DeviceId + "@" + args.Serial, LogSeverity.Debug);
                onLeftFast.execute(fsuipcCache, simConnectCache, moduleCache, args, configRefs);
            }
            else if ((args.Value == 2 && onRight != null) || (args.Value == 3 && onRightFast == null))
            {
                Log.Instance.log("Executing OnRight: " + args.DeviceId + "@" + args.Serial, LogSeverity.Debug);
                onRight.execute(fsuipcCache, simConnectCache, moduleCache, args, configRefs);
            }
            else if (args.Value == 3 && onRightFast != null)
            {
                Log.Instance.log("Executing OnRightFast: " + args.DeviceId + "@" + args.Serial, LogSeverity.Debug);
                onRightFast.execute(fsuipcCache, simConnectCache, moduleCache, args, configRefs);
            }

        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is EncoderInputConfig &&
                (
                    (onLeft == null && ((obj as EncoderInputConfig).onLeft == null)) ||
                    (onLeft != null && onLeft.Equals((obj as EncoderInputConfig).onLeft))
                ) &&
                (
                    (onLeftFast == null && ((obj as EncoderInputConfig).onLeftFast == null)) ||
                    (onLeftFast != null && onLeftFast.Equals((obj as EncoderInputConfig).onLeftFast))
                ) &&
                (
                    (onRight == null && ((obj as EncoderInputConfig).onRight == null)) ||
                    (onRight != null && onRight.Equals((obj as EncoderInputConfig).onRight))
                ) &&
                (
                    (onRightFast == null && ((obj as EncoderInputConfig).onRightFast == null)) ||
                    (onRightFast != null && onRightFast.Equals((obj as EncoderInputConfig).onRightFast))
                );
        }
    }
}
