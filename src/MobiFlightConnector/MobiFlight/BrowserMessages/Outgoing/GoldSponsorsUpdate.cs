using MobiFlight.Sponsors;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages.Outgoing
{
    internal sealed class GoldSponsorsUpdate
    {
        public List<GoldSponsor> Sponsors { get; set; }
            = new List<GoldSponsor>();
    }
}