using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace ArcazeUSB
{
    interface AdapterInterface
    {
        void setConfig (XElement config);
        String getConfig ();
    }
}
