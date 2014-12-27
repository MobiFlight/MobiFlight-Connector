using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.InputConfig
{
    class BaseInputAction : IInputActionType
    {
        protected ActionType _type = ActionType.NotSet;
        public ActionType Type
        {
            get { return _type; }
        }
    }
}
