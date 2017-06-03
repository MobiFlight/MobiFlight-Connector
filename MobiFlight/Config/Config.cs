using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class Config
    {
        public String ModuleName = "";
        public int PowerSavingTime = 60 * 10; // => 10 Minutes Default

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
            FromInternal(rawConfig, false);
        }

        public List<String> ToInternal(int MaxMessageLength)
        {
            List<String> result = new List<string>();
            String message = "";
            
            foreach (BaseDevice item in Items)
            {
                String current = item.ToInternal();
                
                if ((message.Length + current.Length) > MaxMessageLength) {
                    result.Add(message);
                    message = "";
                }

                message += current;
            }
            result.Add(message);

            return result;
        }

        public Config FromInternal(String value, bool throwException = false)
        {
            String[] items = value.Split(BaseDevice.End);
            foreach (String item in items)
            {
                BaseDevice currentItem = null;
                try
                {
                    if (item == "") continue;
                    int limit = item.IndexOf(BaseDevice.Separator);
                    if (-1 == limit) limit = item.Length;

                    DeviceType type = (DeviceType)int.Parse(item.Substring(0, limit));
                    switch (type)
                    {
                        case DeviceType.Button:
                            currentItem = new MobiFlight.Config.Button();
                            break;

                        case DeviceType.Encoder:
                            currentItem = new MobiFlight.Config.Encoder();
                            break;

                        case DeviceType.Output:
                            currentItem = new MobiFlight.Config.Output();
                            break;

                        case DeviceType.Servo:
                            currentItem = new MobiFlight.Config.Servo();
                            break;

                        case DeviceType.Stepper:
                            currentItem = new MobiFlight.Config.Stepper();
                            break;

                        case DeviceType.LedModule:
                            currentItem = new MobiFlight.Config.LedModule();
                            break;

                        case DeviceType.LcdDisplay:
                            currentItem = new MobiFlight.Config.LCDDisplay();
                            break;
                    }

                    currentItem.FromInternal(item + BaseDevice.End);
                    
                    Items.Add(currentItem);

                }
                catch (ArgumentException ex)
                {
                    if (throwException)
                        throw new FormatException("Config not valid. Type not valid", ex);
                    else
                        return this;
                }
                catch (FormatException ex)
                {
                    if (throwException)
                        throw new FormatException("Config not valid. Type not valid", ex);
                    else
                        return this;
                }

            }

            return this;
        }
    }
}
