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
            Modifiers = new ModifierList();
        }
        public FsuipcOffset FSUIPC
        {
            get;
            set;
        }

        public ModifierList Modifiers
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
