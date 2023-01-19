using MobiFlight.Config.Compatibility;
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
        [XmlElement(typeof(InputShiftRegister))]
        [XmlElement(typeof(LedModule))]
        [XmlElement(typeof(Output))]
        [XmlElement(typeof(Servo))]
        [XmlElement(typeof(Stepper))]
        [XmlElement(typeof(LcdDisplay))]
        [XmlElement(typeof(AnalogInput))]
        [XmlElement(typeof(ShiftRegister))]
        [XmlElement(typeof(InputMultiplexer))]
        [XmlElement(typeof(MultiplexerDriver))]
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
                // MultiplexerDriver does not appear in this list as separate device config record
                // Its data is embedded (redundantly) in mux client devices

                String current = item.ToInternal();
                
                if ((message.Length + current.Length) > MaxMessageLength && message.Length > 0) {
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

            // Need to set aside the MultiplexerDriver reference (for subsequent devices) when we find it 
            MobiFlight.Config.MultiplexerDriver multiplexerDriver = null;

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
                            currentItem.FromInternal(item + BaseDevice.End);
                            break;

                        case DeviceType.EncoderSingleDetent:

                            // Single-detent encoder (previously just encoder) is retained
                            // for backwards compatibility. We parse as a single detent encoder
                            // and then convert into an encoder with configurable detents.
                            var encoderSingleDetent = new MobiFlight.Config.EncoderSingleDetent();
                            encoderSingleDetent.FromInternal(item + BaseDevice.End);

                            // Create an encoder with configurable detents
                            currentItem = new MobiFlight.Config.Encoder
                            {
                                PinLeft = encoderSingleDetent.PinLeft,
                                PinRight = encoderSingleDetent.PinRight,
                                Name = encoderSingleDetent.Name
                            };

                            break;

                        case DeviceType.Output:
                            currentItem = new MobiFlight.Config.Output();
                            currentItem.FromInternal(item + BaseDevice.End);
                            break;

                        case DeviceType.Servo:
                            currentItem = new MobiFlight.Config.Servo();
                            currentItem.FromInternal(item + BaseDevice.End);
                            break;

                        case DeviceType.Stepper:
                            currentItem = new MobiFlight.Config.Stepper();
                            currentItem.FromInternal(item + BaseDevice.End);
                            break;

                        case DeviceType.StepperDeprecatedV1:
                            currentItem = new MobiFlight.Config.Compatibility.StepperDeprecatedV2();
                            currentItem.FromInternal(item + BaseDevice.End);
                            (currentItem as StepperDeprecatedV2).BtnPin = "0"; // set this explicitly to 0 because the default used to be 5.
                            currentItem = new MobiFlight.Config.Stepper(currentItem as MobiFlight.Config.Compatibility.StepperDeprecatedV2);
                            break;

                        case DeviceType.StepperDeprecatedV2:
                            currentItem = new MobiFlight.Config.Compatibility.StepperDeprecatedV2();
                            currentItem.FromInternal(item + BaseDevice.End);
                            currentItem = new MobiFlight.Config.Stepper(currentItem as MobiFlight.Config.Compatibility.StepperDeprecatedV2);
                            break;

                        case DeviceType.LedModule:
                            currentItem = new MobiFlight.Config.LedModule();
                            currentItem.FromInternal(item + BaseDevice.End);
                            break;

                        case DeviceType.LcdDisplay:
                            currentItem = new MobiFlight.Config.LcdDisplay();
                            currentItem.FromInternal(item + BaseDevice.End);
                            break;

                        case DeviceType.Encoder:
                            currentItem = new MobiFlight.Config.Encoder();
                            currentItem.FromInternal(item + BaseDevice.End);
                            break;

                        case DeviceType.InputShiftRegister:
                            currentItem = new MobiFlight.Config.InputShiftRegister();
                            currentItem.FromInternal(item + BaseDevice.End);
                            break;

                        case DeviceType.InputMultiplexer:
                            // Build multiplexerDriver if none found yet 
                            if (multiplexerDriver == null) {
                                multiplexerDriver = new MobiFlight.Config.MultiplexerDriver();
                                // multiplexerDriver is not yet init'ed with pin numbers: the FromInternal() of the client
                                // (in this case InputMultiplexer) will provide them
                                // Treat the MultiplexerDriver as a regular device (add it to the items list), except it won't be shown in the GUI tree.
                                Items.Add(multiplexerDriver);
                            }
                            currentItem = new MobiFlight.Config.InputMultiplexer(multiplexerDriver);
                            currentItem.FromInternal(item + BaseDevice.End);
                            break;

                        case DeviceType.AnalogInput:
                            currentItem = new MobiFlight.Config.AnalogInput();
                            currentItem.FromInternal(item + BaseDevice.End);
                            break;

                        case DeviceType.ShiftRegister:
                            currentItem = new MobiFlight.Config.ShiftRegister();
                            currentItem.FromInternal(item + BaseDevice.End);
                            break;

                    }

                    if (currentItem != null)
                    {
                        Items.Add(currentItem);
                    }
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
