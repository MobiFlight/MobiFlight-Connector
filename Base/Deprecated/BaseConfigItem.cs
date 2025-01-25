using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public interface IBaseConfigItemDeprecated
    {
        string Name { get; set; }
        string ComponentType { get; set; }
        PreconditionList Preconditions { get; set; }
    }
}
