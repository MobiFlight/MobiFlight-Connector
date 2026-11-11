using MobiFlightMoza.Protocol;

namespace MobiFlightMoza.Session
{
    /// <summary>
    /// Drives the 9010 telemetry channel just enough to keep it healthy: echoes the
    /// device's registration token unchanged, which the device requires before it will
    /// continue delivering telemetry at all. Doesn't yet register/consume any URLs or
    /// forward HID button state - not required for MCDU rendering.
    /// </summary>
    internal sealed class MozaTelemetryChannel
    {
        private readonly ReliableStreamMultiplexer Multiplexer;
        private readonly NetworkPackageExtractor Extractor = new();
        private bool SkippedLeadingByte;

        public MozaTelemetryChannel(ReliableStreamMultiplexer multiplexer)
        {
            Multiplexer = multiplexer;
        }

        // The device sends one raw 0xFF byte before its first NetworkPackage on this
        // channel - not part of the NetworkPackage framing itself, has to be dropped
        // before feeding the rest to the extractor.
        public void OnApplicationData(byte[] data)
        {
            if (!SkippedLeadingByte)
            {
                if (data.Length == 0) return;
                SkippedLeadingByte = true;
                data = data[1..];
                if (data.Length == 0) return;
            }

            foreach (var package in Extractor.Feed(data))
            {
                if (package.PackageId != 0x06) continue; // token
                if (package.Payload.Length != 4) continue;
                Multiplexer.TrySend(MozaConstants.ServicePortTelemetry, NetworkPackage.Pack(0x06, package.Payload));
            }
        }
    }
}
