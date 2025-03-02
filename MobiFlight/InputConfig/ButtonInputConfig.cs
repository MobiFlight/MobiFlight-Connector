using System;
using System.Timers;
using System.Collections.Generic;
using System.Xml.Serialization;
using MobiFlight.Base;

namespace MobiFlight.InputConfig
{
    public class ButtonInputConfig : IXmlSerializable, ICloneable
    {
        public InputAction onPress;        
        public InputAction onRelease;        
        public InputAction onLongRelease;
        public InputAction onHold;

        private InputEventArgs LastOnPressEvent;
        private CacheCollection LastOnPressCacheCollection;        
        private List<ConfigRefValue> LastOnPressConfigRefs;
        private object LastOnPressLock = new object();
        
        public int LongReleaseDelay = 350; //ms
        public int HoldDelay = 350;
        public int RepeatDelay = 0;

        private Timer HoldTimer = new Timer();
        private Timer RepeatTimer = new Timer();

        public ButtonInputConfig()
        {
            RepeatTimer.AutoReset = true;
            HoldTimer.Elapsed += HoldTimer_Elapsed;            
            RepeatTimer.Elapsed += RepeatTimer_Elapsed;
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

        private void ExecuteOnHoldAction()
        {
            lock (LastOnPressLock)
            {
                InputEventArgs args = (InputEventArgs)LastOnPressEvent.Clone();
                args.Value = (int)MobiFlightButton.InputEvent.HOLD;
                Log.Instance.log($"{args.Name} => {args.DeviceLabel}  => Execute HOLD", LogSeverity.Info);
                onHold.execute(LastOnPressCacheCollection, args, LastOnPressConfigRefs);
            }            
        }

        private void RepeatTimer_Elapsed(object sender, EventArgs e)
        {
            ExecuteOnHoldAction();
        }

        private void ExecuteRepeatOnHold()
        {
            if (RepeatDelay > 0)
            {
                RepeatTimer.Interval = RepeatDelay;
                RepeatTimer.Start();
            }
        }

        private void HoldTimer_Elapsed(object sender, EventArgs e)
        {
            ExecuteOnHoldAction();
            HoldTimer.Stop();
            ExecuteRepeatOnHold();
        }

        private void ExecuteOnHoldWithTimer()
        {
            if (HoldDelay > 0)
            {                
                HoldTimer.Interval = HoldDelay;
                HoldTimer.Start();
            }
            else
            {
                ExecuteOnHoldAction();
                ExecuteRepeatOnHold();
            }
        }

        private void CheckAndStopTimer()
        {
            if (HoldTimer.Enabled)
                HoldTimer.Stop();
            if (RepeatTimer.Enabled)
                RepeatTimer.Stop();
        }

        private void CheckAndAdaptForLongButtonRelease(InputEventArgs current, InputEventArgs previous)
        {
            var inputEvent = (MobiFlightButton.InputEvent)current.Value;
            

            if (inputEvent == MobiFlightButton.InputEvent.RELEASE &&
                onLongRelease != null &&
                previous != null)
            {
                TimeSpan timeSpanToPreviousInput = current.Time - previous.Time;
                if (timeSpanToPreviousInput > TimeSpan.FromMilliseconds(LongReleaseDelay))
                {
                    current.Value = (int)MobiFlightButton.InputEvent.LONG_RELEASE;
                    Log.Instance.log($"{current.Name} => {current.DeviceLabel}  => Execute as LONG_RELEASE", LogSeverity.Info);
                }
            }
        }

        private void ExecuteOnPressAction(CacheCollection cacheCollection,
                                          InputEventArgs args,
                                          List<ConfigRefValue> configRefs)
        {
            lock (LastOnPressLock)
            {
                LastOnPressEvent = args;
                LastOnPressCacheCollection = cacheCollection;
                LastOnPressConfigRefs = configRefs;
                if (onPress != null)
                {
                    onPress.execute(cacheCollection, args, configRefs);
                }
            }
            if (onHold != null)
            {               
                ExecuteOnHoldWithTimer();
            }
        }

        private void ExecuteOnReleaseAction(CacheCollection cacheCollection,
                                            InputEventArgs args,
                                            List<ConfigRefValue> configRefs)
        {
            CheckAndStopTimer();
            if (onRelease != null)
            {
                onRelease.execute(cacheCollection, args, configRefs);
            }
        }

        private void ExecuteOnLongReleaseAction(CacheCollection cacheCollection,
                                                InputEventArgs args,
                                                List<ConfigRefValue> configRefs)
        {
            CheckAndStopTimer();
            if (onLongRelease != null)
            {
                onLongRelease.execute(cacheCollection, args, configRefs);
            }
        }

        internal void execute(CacheCollection cacheCollection, 
                              InputEventArgs args, 
                              List<ConfigRefValue> configRefs)
        {
            CheckAndAdaptForLongButtonRelease(args, LastOnPressEvent);

            switch ((MobiFlightButton.InputEvent)args.Value)
            {
                case MobiFlightButton.InputEvent.PRESS:
                    ExecuteOnPressAction(cacheCollection, args, configRefs);
                    break;
                case MobiFlightButton.InputEvent.RELEASE:
                    ExecuteOnReleaseAction(cacheCollection, args, configRefs);
                    break;
                case MobiFlightButton.InputEvent.LONG_RELEASE:
                    ExecuteOnLongReleaseAction(cacheCollection, args, configRefs);
                    break;
                default:
                    break;
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
