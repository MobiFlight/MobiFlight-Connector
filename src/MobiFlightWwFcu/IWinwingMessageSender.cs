using System.Collections.Generic;

namespace MobiFlightWwFcu
{
    internal interface IWinwingMessageSender
    {
        bool IsConnected();

        void Connect();

        void Shutdown();

        void SendDisplayCommands(IList<byte[]> commands);

        void SendCduDisplayBytes(byte[] byteList);

        void SendLightControlMessage(byte[] destination, byte type, byte value);

        void SetBrightness(byte[] destinationAddress, byte type, string brightness);

        void SetBrightness(byte[] destinationAddress, byte type, int brightness);

        void SetVibration(byte[] destinationAddress, byte type, byte level);

        void SetPulseLight(byte[] destinationAddress, bool isOn);

        void SendHeartBeatMessage();

        void SendRequestFirmwareMessage();
    }
}
