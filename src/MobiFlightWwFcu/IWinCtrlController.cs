using System.Collections.Generic;

namespace MobiFlightWwFcu
{
    internal interface IWinCtrlController
    {
        string Name { get; }

        void Connect();

        void Shutdown();

        void Stop();

        List<string> GetLedNames();

        List<string> GetDisplayNames();

        List<string> GetInternalDisplayNames();

        void SetLed(string led, byte state);

        void SetDisplay(string name, string value);
    }
}
