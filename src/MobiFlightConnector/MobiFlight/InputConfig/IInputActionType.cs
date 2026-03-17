using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.InputConfig
{
    public enum ActionType
    {
        NotSet,
        FsuipcOffset,
        FsuipcMacro,
        Key        
    }

    interface IInputActionType
    {
        ActionType Type { get; }
    }
}
