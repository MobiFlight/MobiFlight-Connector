using System;
using System.Collections.Generic;

namespace MobiFlight.Frontend
{
    public class MobiFlightModuleInfoAdapter : IDeviceItem
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> MetaData { get; set; }
        public IDeviceElement[] Elements { get; set; }

        public MobiFlightPin[] Pins { get; set; }

        public MobiFlightModuleInfoAdapter(IModuleInfo device)
        {
            Type = device.Type;
            Id = device.Serial;
            Name = device.Name;
            MetaData = ConvertCommunityInfoToDictionary(BoardDefinitions.GetBoardByFriendlyName(device.Name)?.Info);
            Pins = new MobiFlightPin[0];
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
