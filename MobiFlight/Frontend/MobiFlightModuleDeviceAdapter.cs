using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Frontend
{
    public class MobiFlightModuleDeviceAdapter : IDeviceItem
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> MetaData { get; set; }
        public IDeviceElement[] Elements { get; set; }

        public MobiFlightModuleDeviceAdapter(MobiFlightModule device)
        {
            Type = device.Type;
            Id = device.Serial;
            Name = device.Name;
            MetaData = ConvertCommunityInfoToDictionary(device.Board.Info);
            Elements = device.GetConnectedDevices().Select(d => new DeviceElement() { Id = d.Name, Name = d.Name, Type = d.Type }).ToArray();
        }

        public Dictionary<string, string> ConvertCommunityInfoToDictionary(Info info)
        {
            if (info == null) return new Dictionary<string, string>();

            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                { "BoardIcon", GetDataImage(info.BoardIcon.Tag as string)},
                { "BoardPicture", info.BoardPicture != null ? GetDataImage(info.BoardPicture.Tag as string) : null},
                { "FriendlyName", info.FriendlyName },
                { "CanInstallFirmware", info.CanInstallFirmware.ToString() },
                { "CanResetBoard", info.CanResetBoard.ToString() },
                { "MobiFlightTypeLabel", info.MobiFlightTypeLabel?.ToString() },
                { "CustomDeviceTypes", string.Join(";", info.CustomDeviceTypes) },
                { "Project", info.Community?.Project },
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
