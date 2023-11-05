using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return (
                // ignore the developer board that has 0.0.1
                currentVersion.CompareTo(new Version("0.0.1")) != 0 &&
                // and update when version lower than latest
                currentVersion.CompareTo(latestVersion) < 0);
        }

    }
}
