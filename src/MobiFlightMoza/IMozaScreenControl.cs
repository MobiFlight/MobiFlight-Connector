using System;
using MobiFlightMoza.Protocol;

namespace MobiFlightMoza
{
    /// <summary>
    /// The public seam MobiFlightConnector's MOZA joystick classes depend on instead of
    /// the concrete <see cref="MozaScreenControl"/>, so a controller can be exercised in
    /// tests without a real serial port.
    /// </summary>
    public interface IMozaScreenControl
    {
        event Action<byte> CabinPositionResolved;
        event Action<string> ErrorMessageCreated;
        event Action<string> TraceCreated;
        event Action ScreenReady;
        bool Connect();
        void SubmitScreenData(string json);
        void ForceResend();
        void SetLed(byte index, bool on);
        void SetLightGroupBrightness(MozaLightGroup group, int level);
        void SetLightGroupMode(MozaLightGroup group, MozaLightMode mode);
        void SetDisplayBrightness(int percent);
        void Stop();
        void Shutdown();
    }
}
