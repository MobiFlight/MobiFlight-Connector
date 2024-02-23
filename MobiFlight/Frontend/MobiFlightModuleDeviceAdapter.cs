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

        public MobiFlightModuleDeviceAdapter(MobiFlightModule device)
        {
            Type = "MobiFlight";
            Id = device.Serial;
            Name = device.Name;
            MetaData = ConvertCommunityInfoToDictionary(device.Board.Info);
            Elements = CreateElementsList(device);
            
        }

        private IDeviceElement[] CreateElementsList(MobiFlightModule device)
        {
            var result = new List<IDeviceElement>();
            result.AddRange(device.GetConnectedOutputDevices().Select(d => new DeviceElement() { Id = d.Name, Name = d.Name, Type = d.Type.ToString() }).ToArray());
            result.AddRange(device.GetConnectedInputDevices().Select(d => new DeviceElement() { Id = d.Name, Name = d.Name, Type = d.Type.ToString() }).ToArray());
            return result.ToArray();
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
