using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlightInstaller
{
    public static class i18n
    {
        /// <summary>
        /// the resource manager to access images 
        /// </summary>
        private static ResourceManager resourceManager = null;

        /// <summary>
        /// get a localized string
        /// </summary>
        /// <param name="s">the resource's string name</param>
        /// <returns>the translated string</returns>
        public static String tr(String s)
        {
            if (null == resourceManager)
            {
                resourceManager = new ResourceManager("MobiFlightInstaller.lang.i18n_Messages", typeof(MobiFlightUpdaterModel).Assembly);
            }
            String result = resourceManager.GetString(s);
            if(result == null) {
                result = s;
            }
            
            return result;
        }
    }
}
