﻿using System;
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
            if (reader.LocalName == "onPress" && reader["type"] != null)
            {
                onPress = InputActionFactory.CreateByType(reader["type"]);
                onPress.ReadXml(reader);
                reader.Read(); // Closing onPress
            }

            if (reader.LocalName == "") reader.Read();
            if (reader.LocalName == "onRelease" && reader["type"] != null)
            {
                onRelease = InputActionFactory.CreateByType(reader["type"]);
                onRelease.ReadXml(reader);
                reader.Read(); // closing onRelease
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
        }

        internal void execute(FSUIPC.Fsuipc2Cache fsuipcCache, 
                              SimConnectMSFS.SimConnectCache simConnectCache, 
                              xplane.XplaneCache xplaneCache,
                              MobiFlightCache moduleCache, 
                              InputEventArgs args, 
                              List<ConfigRefValue> configRefs)
        {
            if (args.Value == 0 && onPress != null)
            {
                Log.Instance.log("Executing OnPress: " + args.DeviceId + "@" + args.Serial, LogSeverity.Debug);
                if(onPress.GetType().ToString()== "MobiFlight.InputConfig.XplaneInputAction")
                {
                    (onPress as XplaneInputAction).execute(xplaneCache, moduleCache, args, configRefs);
                }
                else 
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
