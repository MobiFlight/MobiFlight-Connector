using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
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
                onChange = InputActionFactory.CreateByType(reader["type"]);
                onChange.ReadXml(reader);
                reader.Read(); // Closing onChange
            }

            if (reader.NodeType == XmlNodeType.EndElement) 
                reader.Read(); // this should be the corresponding "end" node
        }

        public List<InputAction> GetInputActionsByType(Type type)
        {
            List<InputAction> result = new List<InputAction>();
            if (onChange != null && onChange.GetType() == type)
                result.Add(onChange);
            return result;
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("onChange");
            if (onChange != null) onChange.WriteXml(writer);
            writer.WriteEndElement();
        }

        internal void execute(CacheCollection cacheCollection, 
                                InputEventArgs args, 
                                List<ConfigRefValue> configRefs)
        {
            if (onChange != null)
            {
                onChange.execute(cacheCollection, args, configRefs);
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

        public Dictionary<String, int> GetStatistics()
        {
            Dictionary<String, int> result = new Dictionary<string, int>();

            result["Input.Analog"] = 1;

            if (onChange != null)
            {
                result["Input.OnChange"] = 1;
                result["Input." + onChange.GetType().Name] = 1;
            }

            return result;
        }
    }

}
