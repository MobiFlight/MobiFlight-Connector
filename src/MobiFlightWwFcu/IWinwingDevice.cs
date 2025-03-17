using System.Collections.Generic;

namespace MobiFlightWwFcu
{
    public interface IWinwingDevice
    {
        string Name { get; }

        void Connect();

        void Shutdown();

        List<string> GetLedNames();

        List<string> GetDisplayNames();

        void SetLed(string led, byte state);

        void SetDisplay(string name, string value);
    }
}
