using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    class ArcazeCommandExecutionException : Exception
    {        
        public ArcazeCommandExecutionException(string p, Exception e) : base (p,e)
        {            
        }
    }
}
