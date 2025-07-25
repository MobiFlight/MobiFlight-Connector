
using System.Collections.Generic;

namespace MobiFlight
{
    internal class UserMessageCodes
    {
        internal const int STARTING_SCRIPT = 1;
        internal const int PROCESS_TERMINATED = 2;
        internal const int PYTHON_NOT_READY = 3;

        internal static readonly Dictionary<int, string> CodeToMessageMap = new Dictionary<int, string>()
        {            
            { STARTING_SCRIPT, "Info: Starting {0}" },          
            { PROCESS_TERMINATED, "Error: Process {0} terminated. Please activate logging via Extras -> Settings" },
            { PYTHON_NOT_READY, "Error: Python not ready" },
        };

      
    }
}
