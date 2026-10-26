using MobiFlightMoza.Protocol;
using System;

namespace MobiFlight.Joysticks.Moza
{
    /// <summary>
    /// The MOZA MGX1000 instrument panel. Backlight brightness goes over the panel's CDC serial
    /// port as plain panel commands; the device accepts them without any handshake or session.
    /// </summary>
    internal class MozaMxg1000 : MozaBaseController
    {
        private const string BacklightId = "backlight";

        public override string Name => Definition?.InstanceName ?? "MOZA MGX1000 Instrument Panel";

        private readonly Func<MozaCdcPort> OpenCdcPort;
        private MozaCdcPort CdcPort;

        public MozaMxg1000(JoystickDefinition definition)
            : base(definition)
        {
            OpenCdcPort = () => MozaCdcPort.Open(Name, definition.VendorId, definition.ProductId);
        }

        // Testing seam: lets tests substitute the CDC port.
        internal MozaMxg1000(JoystickDefinition definition, Func<MozaCdcPort> openCdcPort)
            : base(definition)
        {
            OpenCdcPort = openCdcPort;
        }

        /// <summary>Maps an output state (0..255) onto the panel's 0..15 brightness range.</summary>
        internal static byte ToBrightnessLevel(byte state)
        {
            return (byte)((state * MozaPanelCommand.MaxBrightness + 127) / 255);
        }

        public override void Connect(IntPtr handle)
        {
            base.Connect(handle);
            CdcPort ??= OpenCdcPort();
        }

        public override void UpdateOutputDeviceStates()
        {
            if (!RequiresOutputUpdate) return;
            RequiresOutputUpdate = false;

            var port = CdcPort;
            var backlight = Lights.Find(light => light.Name == BacklightId);
            if (port is null || backlight is null) return;

            var level = ToBrightnessLevel(backlight.State);
            port.Write(MozaPanelCommand.Encode(MozaPanelCommand.Brightness(MozaLightGroup.KeyBacklight, level)));
        }

        public override void Shutdown()
        {
            CdcPort?.Dispose();
            CdcPort = null;
            base.Shutdown();
        }
    }
}
