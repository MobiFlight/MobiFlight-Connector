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
            Transform = new Transformation();
        }
        public bool FSUIPCBcdMode
        {
            get;
            set;
        }

        public long FSUIPCMask
        {
            get;
            set;
        }

        public double FSUIPCMultiplier
        {
            get;
            set;
        }

        public int FSUIPCOffset
        {
            get;
            set;
        }

        public FSUIPCOffsetType FSUIPCOffsetType
        {
            get;
            set;
        }

        public byte FSUIPCSize
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
