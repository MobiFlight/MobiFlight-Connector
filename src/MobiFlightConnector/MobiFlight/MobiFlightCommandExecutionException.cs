using System;

namespace MobiFlight
{
    class MobiFlightCommandExecutionException : Exception
    {
        public MobiFlightCommandExecutionException(string p, Exception e) : base(p, e)
        {
        }
    }
}
