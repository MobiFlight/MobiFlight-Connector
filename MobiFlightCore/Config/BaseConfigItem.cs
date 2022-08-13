using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public interface IBaseConfigItem
    {
        PreconditionList Preconditions { get; set; }
    }
}
