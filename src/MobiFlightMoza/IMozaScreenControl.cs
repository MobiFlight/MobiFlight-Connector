using System;

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

        bool Connect();
        void SubmitScreenData(string json);
        void ForceResend();
        void Stop();
        void Shutdown();
    }
}
