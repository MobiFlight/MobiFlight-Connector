using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace MobiFlightMoza.Session
{
    internal readonly struct MozaPortInfo
    {
        public string PortName { get; }
        public int VendorId { get; }
        public int ProductId { get; }

        public MozaPortInfo(string portName, int vendorId, int productId)
        {
            PortName = portName;
            VendorId = vendorId;
            ProductId = productId;
        }
    }

    /// <summary>
    /// Finds the CDC serial port for MOZA's display module by VID/PID - the same
    /// Win32_PnPEntity approach <c>MobiFlight.Monitors.SerialPortMonitor</c> already uses to
    /// find boards. Parsing one entity's fields is split out as a pure function so it's
    /// testable without WMI.
    /// </summary>
    internal static class MozaPortLocator
    {
        private static readonly Regex HardwareIdPattern = new(@"VID_(?<vid>[0-9A-Fa-f]{4})&PID_(?<pid>[0-9A-Fa-f]{4})");
        private static readonly Regex PortNamePattern = new(@"\((?<port>COM\d+)\)");

        /// <summary>
        /// Finds every currently-attached serial port whose VID/PID matches. Returns an
        /// empty list - never throws - if WMI itself is unavailable or misbehaves: this
        /// library has no logger to report that to, and an empty result already reads as
        /// "device not found" to every caller.
        /// </summary>
        public static IReadOnlyList<MozaPortInfo> FindPorts(int vendorId, IReadOnlyCollection<int> productIds)
        {
            List<MozaPortInfo> result = [];
            try
            {
                using var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");
                foreach (ManagementObject queryObj in searcher.Get().Cast<ManagementObject>())
                {
                    string hardwareId = (queryObj["HardwareID"] as string[])?.FirstOrDefault() ?? "";
                    string caption = queryObj["Caption"] as string ?? "";
                    if (TryParsePnpEntity(hardwareId, caption, out var info) && info.VendorId == vendorId && productIds.Contains(info.ProductId))
                    {
                        result.Add(info);
                    }
                }
            }
            catch
            {
                return [];
            }
            return result;
        }

        /// <summary>
        /// Pure parse of one Win32_PnPEntity's HardwareID/Caption into a port info, with no
        /// WMI involved - the seam that makes this testable.
        /// </summary>
        internal static bool TryParsePnpEntity(string hardwareId, string caption, out MozaPortInfo info)
        {
            info = default;
            if (string.IsNullOrEmpty(hardwareId) || string.IsNullOrEmpty(caption)) return false;

            var idMatch = HardwareIdPattern.Match(hardwareId);
            if (!idMatch.Success) return false;

            var portMatch = PortNamePattern.Match(caption);
            if (!portMatch.Success) return false;

            int vendorId = Convert.ToInt32(idMatch.Groups["vid"].Value, 16);
            int productId = Convert.ToInt32(idMatch.Groups["pid"].Value, 16);
            info = new MozaPortInfo(portMatch.Groups["port"].Value, vendorId, productId);
            return true;
        }
    }
}
