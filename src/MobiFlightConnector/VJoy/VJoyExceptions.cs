using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.VJoy
{
    class VJoyException : Exception
    {
    }

    class VJoyNotEnabledException : VJoyException
    {
    }

    class VJoyNotAccessible : VJoyException
    {
    }
}
