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
        public bool BcdMode
        {
            get;
            set;
        }

        public long Mask
        {
            get;
            set;
        }

        public double FSUIPCMultiplier
        {
            get;
            set;
        }

        public int Offset
        {
            get;
            set;
        }

        public FSUIPCOffsetType OffsetType
        {
            get;
            set;
        }

        public byte Size
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
