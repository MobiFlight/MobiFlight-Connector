using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
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
                onLeft = InputActionFactory.CreateByType(reader["type"]);
                onLeft?.ReadXml(reader);
                reader.Read(); // advance to the next
            }

            if (reader.LocalName == "onLeftFast")
            {
                onLeftFast = InputActionFactory.CreateByType(reader["type"]);
                onLeftFast?.ReadXml(reader);
                reader.Read(); // advance to the next
            }

            if (reader.LocalName == "onRight")
            {
                onRight = InputActionFactory.CreateByType(reader["type"]);
                onRight?.ReadXml(reader);
                reader.Read(); // advance to the next
            }

            if (reader.LocalName == "onRightFast")
            {
                onRightFast = InputActionFactory.CreateByType(reader["type"]);
                onRightFast?.ReadXml(reader);
                reader.Read(); // advance to the next
            }

            if (reader.NodeType == XmlNodeType.EndElement) 
                reader.Read(); // this should be the corresponding "end" node            
        }

        public List<InputAction> GetInputActionsByType(Type type)
        {
            List<InputAction> result = new List<InputAction>();
            if (onRight != null && onRight.GetType() == type)
                result.Add(onRight);
            if (onRightFast != null && onRightFast.GetType() == type)
                result.Add(onRightFast);
            if (onLeft != null && onLeft.GetType() == type)
                result.Add(onLeft);
            if (onLeftFast != null && onLeftFast.GetType() == type)
                result.Add(onLeftFast);
            return result;
        }

        public Dictionary<String, int> GetStatistics()
        {
            Dictionary<String, int> result = new Dictionary<string, int>();

            result["Input.Encoder"] = 1;

            if (onLeft != null)
            {
                result["Input.OnLeft"] = 1;
                result["Input." + onLeft.GetType().Name] = 1;
            }

            if (onLeftFast != null)
            {
                result["Input.OnLeftFast"] = 1;
                result["Input." + onLeftFast.GetType().Name] = 1;
            }

            if (onRight != null)
            {
                result["Input.OnRight"] = 1;
                result["Input." + onRight.GetType().Name] = 1;
            }

            if (onRightFast != null)
            {
                result["Input.OnRightFast"] = 1;
                result["Input." + onRightFast.GetType().Name] = 1;
            }

            return result;
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

        internal void execute(CacheCollection cacheCollection, InputEventArgs args, List<ConfigRefValue> configRefs)
        {
            if ((args.Value == 0 && onLeft != null) || (args.Value == 1 && onLeftFast == null && onLeft != null))
            {
                onLeft.execute(cacheCollection, args, configRefs);
            }
            else if (args.Value == 1 && onLeftFast != null)
            {
                onLeftFast.execute(cacheCollection, args, configRefs);
            }
            else if ((args.Value == 2 && onRight != null) || (args.Value == 3 && onRightFast == null && onRight != null))
            {
                onRight.execute(cacheCollection, args, configRefs);
            }
            else if (args.Value == 3 && onRightFast != null)
            {
                onRightFast.execute(cacheCollection, args, configRefs);
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
