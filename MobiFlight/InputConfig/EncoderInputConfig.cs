using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.InputConfig 
{
    public enum EncoderInputDirectionType
    {
        LEFT,
        RIGHT
    }

    public enum EncoderInputEventType
    {
        NORMAL,
        FAST,
        FASTEST
    }

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
            reader.Read(); // this should be the opening tag "onPress"
            if (reader.LocalName == "onLeft")
            {
                switch (reader["type"])
                {
                    case "FsuipcOffsetInputAction":
                        onLeft = new FsuipcOffsetInputAction();
                        onLeft.ReadXml(reader);
                        reader.ReadStartElement(); // this should be the closing tag "onPress"
                        break;

                    case "KeyInputAction":
                        onLeft = new KeyInputAction();
                        onLeft.ReadXml(reader);
                        break;

                    case "EventIdInputAction":
                        onLeft = new EventIdInputAction();
                        onLeft.ReadXml(reader);
                        break;
                }
            }

            reader.Read(); // this should be the opening tag "onPress"
            if (reader.LocalName == "onLeftFast")
            {
                switch (reader["type"])
                {
                    case "FsuipcOffsetInputAction":
                        onLeftFast = new FsuipcOffsetInputAction();
                        onLeftFast.ReadXml(reader);
                        reader.ReadStartElement(); // this should be the closing tag "onPress"
                        break;

                    case "KeyInputAction":
                        onLeftFast = new KeyInputAction();
                        onLeftFast.ReadXml(reader);
                        break;

                    case "EventIdInputAction":
                        onLeftFast = new EventIdInputAction();
                        onLeftFast.ReadXml(reader);
                        break;
                }
            }

            reader.Read();
            if (reader.LocalName == "onRight")
            {
                switch (reader["type"])
                {
                    case "FsuipcOffsetInputAction":
                        onRight = new FsuipcOffsetInputAction();
                        onRight.ReadXml(reader);
                        reader.ReadStartElement();
                        break;

                    case "KeyInputAction":
                        onRight = new KeyInputAction();
                        onRight.ReadXml(reader);
                        break;

                    case "EventIdInputAction":
                        onRight = new EventIdInputAction();
                        onRight.ReadXml(reader);
                        break;
                }
            }

            reader.Read();
            if (reader.LocalName == "onRightFast")
            {
                switch (reader["type"])
                {
                    case "FsuipcOffsetInputAction":
                        onRightFast = new FsuipcOffsetInputAction();
                        onRightFast.ReadXml(reader);
                        reader.ReadStartElement();
                        break;

                    case "KeyInputAction":
                        onRightFast = new KeyInputAction();
                        onRightFast.ReadXml(reader);
                        break;

                    case "EventIdInputAction":
                        onRightFast = new EventIdInputAction();
                        onRightFast.ReadXml(reader);
                        break;
                }
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

        internal void execute(MobiFlight.Fsuipc2Cache fsuipcCache, ButtonArgs e)
        {
            if ((e.Value == 0 && onLeft != null) || (e.Value == 1 && onLeftFast == null))
            {
                Log.Instance.log("Executing OnLeft: " + e.ButtonId + "@" + e.Serial, LogSeverity.Debug);
                onLeft.execute(fsuipcCache);
            }
            else if (e.Value == 1 && onLeftFast != null)
            {
                Log.Instance.log("Executing OnLeftFast: " + e.ButtonId + "@" + e.Serial, LogSeverity.Debug);
                onLeftFast.execute(fsuipcCache);
            }
            else if ((e.Value == 2 && onRight != null) || (e.Value == 3 && onRightFast == null))
            {
                Log.Instance.log("Executing OnRight: " + e.ButtonId + "@" + e.Serial, LogSeverity.Debug);
                onRight.execute(fsuipcCache);
            }
            else if (e.Value == 3 && onRightFast != null)
            {
                Log.Instance.log("Executing OnRightFast: " + e.ButtonId + "@" + e.Serial, LogSeverity.Debug);
                onRightFast.execute(fsuipcCache);
            }

        }
    }
}
