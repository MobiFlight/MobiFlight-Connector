using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public interface IModule
    {
        bool Connected { get; }
        void Connect();
        void Disconnect();
        IModuleInfo GetInfo();
    }
}
