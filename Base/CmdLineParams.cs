using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public class CmdLineParams
    {
        bool autoRun = false;
        string configFile = null;

        public bool AutoRun
        {
            get { return autoRun; }
        }

        public string ConfigFile
        {
            get { return configFile; }
        }

        public CmdLineParams()
        {
            string[] args = Environment.GetCommandLineArgs();
            autoRun = _hasCfgParam("autoRun", args);
            configFile = _getCfgParamValue("cfg", args, null);
        }

        /// <summary>
        /// check whether a config param is present or not
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool _hasCfgParam(string key, string[] args)
        {
            return ((args.Length > 1) && (Array.IndexOf(args, "/" + key) != -1));
        }

        /// <summary>
        /// get a value for a given parameter, use default value if not present
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        string _getCfgParamValue(string key, string[] args, string defValue)
        {
            string result = defValue;
            // The first commandline argument is always the executable path itself.
            if (args.Length > 1)
            {
                int pos = -1;
                if ((pos = Array.IndexOf(args, "/" + key)) != -1)
                {
                    try
                    {
                        if (args[pos + 1][0] != '/') result = args[pos + 1];
                    }
                    catch (Exception e)
                    {
                        // do nothing
                    }
                }
            }
            return result;
        }

    }
}
