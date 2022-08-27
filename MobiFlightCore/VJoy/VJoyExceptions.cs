using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.VJoy
{
    public class VJoyException : Exception
    {
    }

    public class VJoyNotEnabledException : VJoyException
    {
    }

    public class VJoyNotAccessible : VJoyException
    {
    }
}
