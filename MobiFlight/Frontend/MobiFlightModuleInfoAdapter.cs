using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Frontend
{
    public class MobiFlightModuleInfoAdapter : IDeviceItem
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> MetaData { get; set; }
        public IDeviceElement[] Elements { get; set; }

        public MobiFlightModuleInfoAdapter(IModuleInfo device)
        {
            Type = device.Type;
            Id = device.Serial;
            Name = device.Name;
        }

        public Dictionary<string, string> ConvertCommunityInfoToDictionary(Community info)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                { "Project", info.Project },
                { "Website", info.Website },
                { "Docs", info.Docs },
                { "Support", info.Support }
            };
            return dict;
        }
    }
}
