using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.HubHop
{
    public enum HubHopType
    {
        [EnumMember(Value = "Output")]
        Output = 1,
        [EnumMember(Value = "Input")]
        Input = 2,
        [EnumMember(Value = "Input (Potentiometer)")]
        InputPotentiometer = 4,
        [EnumMember(Value = "AllInputs")]
        AllInputs = 6
    }
}
