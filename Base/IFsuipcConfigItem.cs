using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobiFlight;

namespace MobiFlight
{
    public interface IFsuipcConfigItem
    {
        int FSUIPCOffset { get; set; }
        byte FSUIPCSize { get; set; }
        FSUIPCOffsetType FSUIPCOffsetType { get; set; }
        long FSUIPCMask { get; set; }
        bool FSUIPCBcdMode { get; set; }
        string Value { get; set; }
        Transformation Transform { get; set; }
    }
}
