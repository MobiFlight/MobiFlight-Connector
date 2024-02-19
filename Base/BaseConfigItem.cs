using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public interface IBaseConfigItem
    {
        string GUID { get; set; }
        bool Active { get; set; }
        string Description { get; set; }
        PreconditionList Preconditions { get; set; }
    }
}
