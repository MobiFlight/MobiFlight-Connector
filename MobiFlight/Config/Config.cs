using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class Config
    {
        [XmlElement(typeof(Button))]
        [XmlElement(typeof(Encoder))]
        [XmlElement(typeof(LedModule))]
        [XmlElement(typeof(Output))]
        [XmlElement(typeof(Servo))]
        [XmlElement(typeof(Stepper))]
        public List<BaseDevice> Items = new List<BaseDevice>();

        public Config() { }

        public Config(String rawConfig) : this()
        {
            FromInternal(rawConfig);
        }

        public String ToInternal()
        {
            string result = "";
            foreach (BaseDevice item in Items)
            {
                result += item.ToInternal() + ":";
            }            
            return result;
        }

        public Config FromInternal(String value)
        {
            String[] items = value.Split(':');
            foreach (String item in items)
            {
                BaseDevice currentItem = null;
                try
                {
                    if (item == "") continue;
                    int limit = item.IndexOf(',');
                    if (-1 == limit) limit = item.Length;

                    MobiFlightModule.DeviceType type = (MobiFlightModule.DeviceType)int.Parse(item.Substring(0, item.IndexOf(',')));
                    switch (type)
                    {
                        case MobiFlightModule.DeviceType.Button:
                            currentItem = new MobiFlight.Config.Button();
                            break;

                        case MobiFlightModule.DeviceType.Encoder:
                            currentItem = new MobiFlight.Config.Encoder();
                            break;

                        case MobiFlightModule.DeviceType.Output:
                            currentItem = new MobiFlight.Config.Output();
                            break;

                        case MobiFlightModule.DeviceType.Servo:
                            currentItem = new MobiFlight.Config.Servo();
                            break;

                        case MobiFlightModule.DeviceType.Stepper:
                            currentItem = new MobiFlight.Config.Stepper();
                            break;

                        case MobiFlightModule.DeviceType.LedModule:
                            currentItem = new MobiFlight.Config.LedModule();
                            break;
                    }

                    currentItem.FromInternal(item);
                    Items.Add(currentItem);

                }
                catch (FormatException ex)
                {
                    throw new FormatException("Config not valid. Type not valid", ex);
                }

            }

            return this;
        }
    }
}
