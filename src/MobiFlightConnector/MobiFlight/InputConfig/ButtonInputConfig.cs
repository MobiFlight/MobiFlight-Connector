using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MobiFlight.InputConfig
{
    public class ButtonInputConfig : IXmlSerializable, ICloneable
    {
        public InputAction onPress;
        public InputAction onRelease;
        public InputAction onLongRelease;
        public InputAction onHold;
        public int LongReleaseDelay = 350; //ms
        public int HoldDelay = 350;
        public int RepeatDelay = 0;

        public ButtonInputConfig()
        {
        }

        /// <summary>
        /// Copy constructor, this allows to reuse the clone method in derived classes
        /// </summary>
        /// <param name="copyFrom"></param>
        protected ButtonInputConfig(ButtonInputConfig copyFrom) : this()
        {
            this.onPress = (InputAction)copyFrom?.onPress?.Clone();
            this.onRelease = (InputAction)copyFrom?.onRelease?.Clone();
            this.onLongRelease = (InputAction)copyFrom?.onLongRelease?.Clone();
            this.onHold = (InputAction)copyFrom?.onHold?.Clone();
            this.RepeatDelay = copyFrom.RepeatDelay;
            this.HoldDelay = copyFrom.HoldDelay;
            this.LongReleaseDelay = copyFrom.LongReleaseDelay;
        }

        public object Clone()
        {
            return new ButtonInputConfig(this);
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
                if (reader["longReleaseDelay"] != null)
                {
                    LongReleaseDelay = int.Parse(reader["longReleaseDelay"]);
                }
                onLongRelease = InputActionFactory.CreateByType(reader["type"]);
                onLongRelease?.ReadXml(reader);
                reader.Read(); // closing onLongRelease
            }

            if (reader.LocalName == "") reader.Read();
            if (reader.LocalName == "onHold")
            {
                HoldDelay = int.Parse(reader["holdDelay"]);
                RepeatDelay = int.Parse(reader["repeatDelay"]);
                onHold = InputActionFactory.CreateByType(reader["type"]);
                onHold?.ReadXml(reader);
                reader.Read(); // closing onLongRelease
            }

            if (reader.NodeType==System.Xml.XmlNodeType.EndElement)
                reader.Read();
        }

        public void SetInputActionByName(string name, InputAction inputAction)
        {
            switch (name)
            {
                case "onPress":
                    onPress = inputAction;
                    break;
                case "onRelease":
                    onRelease = inputAction;
                    break;
                case "onLongRelease":
                    onLongRelease = inputAction;
                    break;
                case "onHold":
                    onHold = inputAction;
                    break;
            }
        }

        public InputAction GetInputActionByName(string name)
        {
            switch (name)
            {
                case "onPress":
                    return onPress;
                case "onRelease":
                    return onRelease;
                case "onLongRelease":
                    return onLongRelease;
                case "onHold":
                    return onHold;
                default:
                    return null;
            }
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
            if (onHold != null && onHold.GetType() == type)
                result.Add(onHold);
            return result;
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (onPress != null)
            {
                writer.WriteStartElement("onPress");
                onPress.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (onRelease != null)
            {
                writer.WriteStartElement("onRelease");
                onRelease.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (onLongRelease != null)
            {
                writer.WriteStartElement("onLongRelease");
                writer.WriteAttributeString("longReleaseDelay", LongReleaseDelay.ToString());
                onLongRelease.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (onHold != null)
            {
                writer.WriteStartElement("onHold");
                writer.WriteAttributeString("holdDelay", HoldDelay.ToString());
                writer.WriteAttributeString("repeatDelay", RepeatDelay.ToString());
                onHold.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        /// <summary>LONG_RELEASE without onLongRelease resolves to RELEASE. Shared with GetInputAction/InputEventExecutor.</summary>
        internal MobiFlightButton.InputEvent ResolveDispatchedEvent(MobiFlightButton.InputEvent value) =>
            value == MobiFlightButton.InputEvent.LONG_RELEASE && onLongRelease == null
                ? MobiFlightButton.InputEvent.RELEASE
                : value;

        /// <summary>Dispatches to the matching InputAction. REPEAT dispatches to onHold, same as HOLD.</summary>
        internal void execute(CacheCollection cacheCollection,
                              InputEventArgs args,
                              List<ConfigRefValue> configRefs)
        {
            var value = (MobiFlightButton.InputEvent)args.Value;
            var dispatchedValue = ResolveDispatchedEvent(value);

            InputAction action;
            switch (dispatchedValue)
            {
                case MobiFlightButton.InputEvent.PRESS:
                    action = onPress;
                    break;
                case MobiFlightButton.InputEvent.RELEASE:
                    action = onRelease;
                    break;
                case MobiFlightButton.InputEvent.LONG_RELEASE:
                    action = onLongRelease;
                    break;
                case MobiFlightButton.InputEvent.HOLD:
                case MobiFlightButton.InputEvent.REPEAT:
                    action = onHold;
                    break;
                default:
                    action = null;
                    break;
            }

            if (action == null) return;

            if (dispatchedValue == value)
            {
                action.execute(cacheCollection, args, configRefs);
                return;
            }

            var normalizedArgs = args.CloneWithLabel();
            normalizedArgs.Value = (int)dispatchedValue;
            action.execute(cacheCollection, normalizedArgs, configRefs);
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

            if (onHold != null)
            {
                result["Input.OnHold"] = 1;
                result["Input." + onHold.GetType().Name] = 1;
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
                    (onLongRelease != null && onLongRelease.Equals((obj as ButtonInputConfig).onLongRelease) &&
                    (LongReleaseDelay == (obj as ButtonInputConfig).LongReleaseDelay))
                ) &&
                (
                    (onHold == null && ((obj as ButtonInputConfig).onHold == null)) ||
                    (onHold != null && onHold.Equals((obj as ButtonInputConfig).onHold) &&
                    (HoldDelay == (obj as ButtonInputConfig).HoldDelay) &&
                    (RepeatDelay == (obj as ButtonInputConfig).RepeatDelay))
                );
        }
    }
}