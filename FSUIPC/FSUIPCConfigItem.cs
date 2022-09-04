using MobiFlight.Modifier;
using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.FSUIPC
{
    class FSUIPCConfigItem : IFsuipcConfigItem
    {
        public FSUIPCConfigItem()
        {
            FSUIPC = new FsuipcOffset();
            Transform = new Transformation();
        }
        public FsuipcOffset FSUIPC
        {
            get;
            set;
        }

        public Transformation Transform
        {
            get;
            set;
        }

        public String Value
        {
            get;
            set;
        }
    }
}
