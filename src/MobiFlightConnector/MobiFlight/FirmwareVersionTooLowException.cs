using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    class FirmwareVersionTooLowException : Exception
    {
        Version Required;
        Version Current;
        public FirmwareVersionTooLowException(Version RequiredVersion, Version CurrentVersion)
        {
            Required = RequiredVersion;
            Current = CurrentVersion;
        }

    }
}
