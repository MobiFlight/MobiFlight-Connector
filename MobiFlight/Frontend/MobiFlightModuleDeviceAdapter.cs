using MobiFlight.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Frontend
{
    public class MobiFlightModuleDeviceAdapter : IDeviceItem
    {
        public string DeviceType { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> MetaData { get; set; }

        public IDeviceElement[] Elements { get; set; }

        public MobiFlightPin[] Pins { get; set; }

        public MobiFlightModuleDeviceAdapter()
        {
            Elements = new List<DeviceElement>().ToArray();
        }
        public MobiFlightModuleDeviceAdapter(MobiFlightModule device)
        {
            this.DeviceType = device.Type.ToString();
            Type = "MobiFlight";
            Id = device.Serial;
            Name = device.Name;
            MetaData = ConvertCommunityInfoToDictionary(device.Board.Info);
            Elements = CreateElementsList(device);
            Pins = device.GetPins().ToArray();
        }

        private IDeviceElement[] CreateElementsList(MobiFlightModule device)
        {
            var result = new List<IDeviceElement>();
            result.AddRange(device.Config.Items.Select(
                d =>
                {
                    var element = new DeviceElement()
                    {
                        Id = d.Name,
                        Name = d.Name,
                        Type = d.Type.ToString()
                    };

                    element.ConfigData = ConvertConfigToDictionary(d);

                    return element;
                }).ToArray());
            return result.ToArray();
        }

        private Dictionary<string, string> ConvertConfigToDictionary(BaseDevice d)
        {
            switch (d.Type)
            {
                case MobiFlight.DeviceType.AnalogInput:
                    return new Dictionary<string, string>
                    {
                        { "Pin", (d as AnalogInput).Pin.ToString() }
                    };
                case MobiFlight.DeviceType.Button:
                    return new Dictionary<string, string>
                    {
                        { "Pin", (d as Button).Pin.ToString() }
                    };
                case MobiFlight.DeviceType.CustomDevice:
                    return new Dictionary<string, string>
                    {
                        { "Pins", (d as CustomDevice).Pins.ToString() }
                    };
                case MobiFlight.DeviceType.Encoder:
                    return new Dictionary<string, string>
                    {
                        { "PinLeft", (d as Encoder).PinLeft.ToString() },
                        { "PinRight", (d as Encoder).PinRight.ToString() }
                    };
                case MobiFlight.DeviceType.InputMultiplexer:
                    return new Dictionary<string, string>
                    {
                        { "PinS0", (d as InputMultiplexer).Selector.PinSx[0].ToString() },
                        { "PinS1", (d as InputMultiplexer).Selector.PinSx[1].ToString() },
                        { "PinS2", (d as InputMultiplexer).Selector.PinSx[2].ToString() },
                        { "PinS3", (d as InputMultiplexer).Selector.PinSx[3].ToString() },
                        { "DataPin", (d as InputMultiplexer).DataPin.ToString() }
                    };

                case MobiFlight.DeviceType.InputShiftRegister:
                    return new Dictionary<string, string>
                    {
                        { "DataPin", (d as InputShiftRegister).DataPin.ToString() },
                        { "ClockPin", (d as InputShiftRegister).ClockPin.ToString() },
                        { "LatchPin", (d as InputShiftRegister).LatchPin.ToString() }
                    };
                case MobiFlight.DeviceType.LcdDisplay:
                    return new Dictionary<string, string>
                    {
                        { "Address", (d as LcdDisplay).Address.ToString() },
                        { "Lines", (d as LcdDisplay).Lines.ToString() },
                        { "Cols", (d as LcdDisplay).Cols.ToString() }
                    };

                case MobiFlight.DeviceType.LedModule:
                    return new Dictionary<string, string>
                    {
                        { "DataPin", (d as LedModule).DinPin.ToString() },
                        { "ClockPin", (d as LedModule).ClkPin.ToString() },
                        { "LatchPin", (d as LedModule).ClsPin.ToString() }
                    };
                case MobiFlight.DeviceType.Output:
                    return new Dictionary<string, string>
                    {
                        { "Pin", (d as Output).Pin.ToString() }
                    };

                case MobiFlight.DeviceType.Servo:
                    return new Dictionary<string, string>
                    {
                        { "DataPin", (d as Servo).DataPin.ToString() }
                    };

                case MobiFlight.DeviceType.ShiftRegister:
                    return new Dictionary<string, string>
                    {
                        { "DataPin", (d as ShiftRegister).DataPin.ToString() },
                        { "ClockPin", (d as ShiftRegister).ClockPin.ToString() },
                        { "LatchPin", (d as ShiftRegister).LatchPin.ToString() }
                    };

                case MobiFlight.DeviceType.Stepper:
                    return new Dictionary<string, string>
                    {
                        { "Pin1", (d as Stepper).Pin1.ToString() },
                        { "Pin2", (d as Stepper).Pin2.ToString() },
                        { "Pin3", (d as Stepper).Pin3.ToString() },
                        { "Pin4", (d as Stepper).Pin4.ToString() }
                    };

                default:
                    return new Dictionary<string, string>();
            }
        }

        public Dictionary<string, string> ConvertCommunityInfoToDictionary(Info info)
        {
            if (info == null) return new Dictionary<string, string>();

            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                { "Icon", GetDataImage(info.BoardIcon.Tag as string)},
                { "Picture", info.BoardPicture != null ? GetDataImage(info.BoardPicture.Tag as string) : null},
                { "FriendlyName", info.FriendlyName },
                { "CanInstallFirmware", info.CanInstallFirmware.ToString() },
                { "CanResetBoard", info.CanResetBoard.ToString() },
                { "MobiFlightTypeLabel", info.MobiFlightTypeLabel?.ToString() },
                { "CustomDeviceTypes", string.Join(";", info.CustomDeviceTypes) },
                { "Project", info.Community?.Project ?? "MobiFlight" },
                { "Website", info.Community?.Website },
                { "Docs", info.Community?.Docs },
                { "Support", info.Community?.Support }
            };
            return dict;
        }

        public string GetDataImage(string file)
        {
            var extension = System.IO.Path.GetExtension(file).Substring(1);
            var base64EncodedImage = Convert.ToBase64String(System.IO.File.ReadAllBytes(file));

            return $"data:image/{extension};base64,{base64EncodedImage}";
        }
    }
}
