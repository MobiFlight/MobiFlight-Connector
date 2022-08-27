using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public interface IBaseConfigItem
    {
        bool Active { get; set; }
        string Description { get; set; }
        Guid Guid { get; set; }
        PreconditionList Preconditions { get; set; }
    }

    public class BaseConfigItem : IBaseConfigItem
    {
        public bool Active { get; set; }
        public string Description { get; set; }
        public Guid Guid { get; set; }
        public PreconditionList Preconditions { get; set; }
    }
}
