using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlightInstaller
{
    public class CmdLineParams
    {
        
        bool check = false;
        bool checkBeta = false;
        bool expert = false;

        string install = null;
        string configFile = null;
        string cacheId = "";
        bool installOnly = false;

        public string Install
        {
            get { return install; }
        }

        public bool Check
        {
            get { return check; }
        }
        public bool CheckBeta
        {
            get { return checkBeta; }
        }

        public bool ExpertMode
        {
            get { return expert; }
        }

        public bool InstallOnly
        {
            get { return installOnly; }
        }


        public string ConfigFile
        {
            get { return configFile; }
        }

        public string CacheId
        {
            get { return cacheId; }
        }

        public CmdLineParams(string[] args)
        {
            install = _getCfgParamValue("install", args, null);
            check = _hasCfgParam("check", args);
            checkBeta = _hasCfgParam("beta", args);
            expert = _hasCfgParam("expert", args);
            installOnly = _hasCfgParam("installOnly", args);

            configFile = _getCfgParamValue("cfg", args, null);
            cacheId = _getCfgParamValue("cacheId", args, null);
        }

        public bool HasParams()
        {
            return Check || CheckBeta || ExpertMode || ConfigFile != null || Install != null;
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
                    catch (Exception)
                    {
                        // do nothing
                    }
                }
            }
            return result;
        }
    }
}
