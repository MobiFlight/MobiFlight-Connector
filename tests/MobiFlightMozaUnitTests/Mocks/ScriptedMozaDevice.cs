using MobiFlightMoza.Protocol;

namespace MobiFlightMoza.Tests.Mocks
{
    // Builds complete wire frames as the device itself would send them (DevicePair=0x21,
    // outer command 0xC3), for driving MozaScreenSession end to end in tests.
    internal static class ScriptedMozaDevice
    {
        public static byte[] Syn1(ushort destinationPort, ushort isn, ushort announcedPort, byte version)
            => Wrap(isReply: false, ReliableStreamFrame.PackSyn(destinationPort, StreamMessageType.Syn1, isn, announcedPort, version));

        public static byte[] Ack(ushort destinationPort, ushort acknowledgedIsn)
            => Wrap(isReply: true, ReliableStreamFrame.PackAck(destinationPort, acknowledgedIsn));

        public static byte[] Trans(ushort destinationPort, ushort isn, byte[] applicationData)
            => Wrap(isReply: false, ReliableStreamFrame.PackTrans(destinationPort, isn, applicationData));

        private static byte[] Wrap(bool isReply, byte[] streamPayload)
        {
            byte commandByte = (byte)(MozaConstants.StreamInnerCommand | (isReply ? 0x80 : 0x00));
            byte[] tunnelPayload = [commandByte, .. streamPayload];
            return SerialLinkFrame.EncodeRaw(0xC3, MozaConstants.DevicePairFromDevice, tunnelPayload);
        }
    }
}
