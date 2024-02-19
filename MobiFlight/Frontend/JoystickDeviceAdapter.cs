using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MobiFlight.Frontend
{
    internal class JoystickDeviceAdapter : IDeviceItem
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> MetaData { get; set; }
        public IDeviceElement[] Elements { get; set; }

        public JoystickDeviceAdapter(Joystick device)
        {
            Type = "Joystick";
            Id = device.Serial;
            Name = device.Name;
            MetaData = CreateMetaData(device);
            Elements = CreateElementsList(device);
        }

        private Dictionary<string, string> CreateMetaData(Joystick device)
        {   
            if (device.Definition == null) return new Dictionary<string, string>();
            if (device.Definition?.Info == null) return new Dictionary<string, string>();
            var basePath = "./Joysticks";
            var iconData = device.Definition.Info.Icon != null ? 
                GetDataImage(Path.Combine(basePath, device.Definition.Info.Icon)) : null;
            var pictureData = device.Definition.Info.Picture != null ?
                GetDataImage(Path.Combine(basePath, device.Definition.Info.Picture)) : null;

            return new Dictionary<string, string>()
            {
                { "Icon", iconData },
                { "Picture", pictureData },
                { "Manufacturer", device.Definition.Info.Manufacturer },
                { "Website", device.Definition.Info.Website },
                { "VendorId", device.Definition.VendorId.ToString() },
                { "ProductId", device.Definition.ProductId.ToString()}
            };
        }

        public IDeviceElement[] CreateElementsList(Joystick device)
        {
           var inputs = device.GetAvailableDevicesAsListItems().Select(d => 
            new DeviceElement() { 
                Id = d.Value.Name, Name = d.Value.Label, Type = d.Value.Type }).ToArray();

           var outputs = device.GetAvailableDevicesAsListItems().Select(d => 
                      new DeviceElement()
                      {
                Id = d.Value.Name, Name = d.Value.Label, Type = d.Value.Type }).ToArray();

            return inputs.Concat(outputs).ToArray();
        }

        public string GetDataImage(string file)
        {
            var extension = System.IO.Path.GetExtension(file).Substring(1);
            var base64EncodedImage = Convert.ToBase64String(System.IO.File.ReadAllBytes(file));

            return $"data:image/{extension};base64,{base64EncodedImage}";
        }
    }
}