using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MobiFlight.InputConfig
{
    public class ButtonInputConfig : IXmlSerializable, ICloneable
    {
        public InputAction onPress;
        public InputAction onRelease;
        public InputAction onLongRelease;


        public object Clone()
        {
            ButtonInputConfig clone = new ButtonInputConfig();
            if (onPress != null) clone.onPress = (InputAction)onPress.Clone();
            if (onRelease != null) clone.onRelease = (InputAction)onRelease.Clone();
            if (onLongRelease != null) clone.onLongRelease = (InputAction)onLongRelease.Clone();
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
                onPress = InputActionFactory.CreateByType(reader["type"]);
                onPress?.ReadXml(reader);
                reader.Read(); // Closing onPress
            }

            if (reader.LocalName == "") reader.Read();
            if (reader.LocalName == "onRelease")
            {                
                onRelease = InputActionFactory.CreateByType(reader["type"]);
                onRelease?.ReadXml(reader);
                reader.Read(); // closing onRelease
            }

            if (reader.LocalName == "") reader.Read();
            if (reader.LocalName == "onLongRelease")
            {
                onLongRelease = InputActionFactory.CreateByType(reader["type"]);
                onLongRelease?.ReadXml(reader);
                reader.Read(); // closing onLongRelease
            }

            if(reader.NodeType==System.Xml.XmlNodeType.EndElement)
                reader.Read();
        }

        public List<InputAction> GetInputActionsByType(Type type)
        {
            List<InputAction> result = new List<InputAction>();
            if (onPress != null && onPress.GetType()==type)
                result.Add(onPress);
            if (onRelease != null && onRelease.GetType() == type)
                result.Add(onRelease);
            if (onLongRelease != null && onLongRelease.GetType() == type)
                result.Add(onLongRelease);
            return result;
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("onPress");
            if (onPress != null) onPress.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("onRelease");
            if (onRelease != null) onRelease.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("onLongRelease");
            if (onLongRelease != null) onLongRelease.WriteXml(writer);
            writer.WriteEndElement();
        }

        internal void execute(CacheCollection cacheCollection, 
                              InputEventArgs args, 
                              List<ConfigRefValue> configRefs)
        {
            var inputEvent = (MobiFlightButton.InputEvent)args.Value;
            if (inputEvent == MobiFlightButton.InputEvent.PRESS && onPress != null)
            {
                onPress.execute(cacheCollection, args, configRefs);
            }
            else if (inputEvent == MobiFlightButton.InputEvent.RELEASE && onRelease != null)
            {
                onRelease.execute(cacheCollection, args, configRefs);
            }
            else if (inputEvent == MobiFlightButton.InputEvent.LONG_RELEASE)                
            {
                if (onLongRelease != null)
                {
                    onLongRelease.execute(cacheCollection, args, configRefs);
                }
                else if (onRelease != null)
                {
                    onRelease.execute(cacheCollection, args, configRefs);
                }                
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
                result["Input.OnRelease"] = 1;
                result["Input." + onRelease.GetType().Name] = 1;
            }

            if (onLongRelease != null)
            {
                result["Input.OnLongRelease"] = 1;
                result["Input." + onLongRelease.GetType().Name] = 1;
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
                ) &&
                (
                    (onLongRelease == null && ((obj as ButtonInputConfig).onLongRelease == null)) ||
                    (onLongRelease != null && onLongRelease.Equals((obj as ButtonInputConfig).onLongRelease))
                );
        }
    }
}
