using System.Collections.Generic;

namespace MobiFlight.BrowserMessages.Outgoing
{
    public class MobiFlightVariablesUpdate
    {
        public List<MobiFlightVariable> Variables { get; set; } = new List<MobiFlightVariable>();
    }
}