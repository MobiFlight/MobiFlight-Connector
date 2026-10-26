using MobiFlightMoza.Protocol;
using System;
using System.IO;

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

        private readonly MozaCdcPort CdcPort;

        public MozaMxg1000(JoystickDefinition definition)
            : base(definition)
        {
            CdcPort = new MozaCdcPort(Name, definition.VendorId, definition.ProductId);
        }

        // Testing seam: lets tests substitute the serial stream and the retry throttle.
        internal MozaMxg1000(JoystickDefinition definition, Func<Stream> openCdcStream, TimeSpan? connectRetryInterval = null)
            : base(definition)
        {
            CdcPort = new MozaCdcPort(Name, openCdcStream, connectRetryInterval ?? TimeSpan.FromSeconds(2));
        }

        /// <summary>Maps an output state (0..255) onto the panel's 0..15 brightness range.</summary>
        internal static byte ToBrightnessLevel(byte state)
        {
            return (byte)((state * MozaPanelCommand.MaxBrightness + 127) / 255);
        }

        public override void UpdateOutputDeviceStates()
        {
            if (!RequiresOutputUpdate) return;
            RequiresOutputUpdate = false;

            var backlight = Lights.Find(light => light.Name == BacklightId);
            if (backlight is null) return;

            var level = ToBrightnessLevel(backlight.State);
            var frame = MozaPanelCommand.Encode(MozaPanelCommand.Brightness(MozaLightGroup.KeyBacklight, level));

            // Stays pending until the CDC port accepts the frame.
            RequiresOutputUpdate = !CdcPort.TryWrite(frame);
        }

        public override void Stop()
        {
            // Don't open the port (and darken the panel) for an output that was never used.
            if (!CdcPort.IsOpen) return;

            base.Stop();
        }

        public override void Shutdown()
        {
            CdcPort.Dispose();
            base.Shutdown();
        }
    }
}
