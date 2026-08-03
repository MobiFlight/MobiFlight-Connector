using System;

namespace MobiFlight
{
    public class MobiFlightModuleInfo : IModuleInfo
    {
        String _version = null;
        public String Type { get; set; }
        public String Serial { get; set; }
        public String Port { get; set; }
        public String Name { get; set; }
        public String Config { get; set; }
        public String HardwareId { get; set; }
        public Board Board { get; set; }
        public String Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public bool HasMfFirmware()
        {
            return !String.IsNullOrEmpty(Version);
        }

        public bool FirmwareInstallPossible()
        {
            return (Board?.Info?.CanInstallFirmware ?? false) && !HasMfFirmware();
        }
        internal bool FirmwareRequiresUpdate()
        {
            Version latestVersion = new Version(Board.Info.LatestFirmwareVersion);
            Version currentVersion;

            try
            {
                currentVersion = new Version(Version != null ? Version : "0.0.0");
            }
            catch (Exception ex)
            {
                currentVersion = new Version("0.0.0");
            }
            bool isDevOrPrFirmware =
                currentVersion.Major == 0 &&
                currentVersion.Minor == 0;
            // PR firmware uses version 0.0.<PR_NUMBER>, so don't require an update.
            if (isDevOrPrFirmware)
            {
                return false;
            }
            // and update when version lower than latest
            return true ;
                
                
               
        }

    }
}
