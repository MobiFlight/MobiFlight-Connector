using System;
using System.Collections.Generic;
using System.Text;

namespace ArcazeUSB
{
    interface InputConnectorInterface
    {
        bool connect();
        bool disconnect();
        bool isConnected();
        Int32 readInt32(Int32 offset);
    }
}
