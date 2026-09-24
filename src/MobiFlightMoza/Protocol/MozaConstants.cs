namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// Fixed wire-level constants for the FCD Display CDC Serial Protocol.
    /// </summary>
    internal static class MozaConstants
    {
        // SerialLink framing.
        public const byte StartOfFrame = 0x7E;
        public const int ChecksumSeed = 0x0D + 0x7E;
        public const int MaxSerialPayloadLength = 64;

        // 0x43 tunnel. Outer command's lower 7 bits identify the tunnel; bit7 is the outer
        // reply flag and must not be confused with the inner one.
        public const byte TunnelCommand = 0x43;
        public const byte DevicePairToDevice = 0x12;
        public const byte DevicePairFromDevice = 0x21;
        public const int MaxTunnelInnerPayloadLength = 62; // 63 - 1 byte for the inner command

        // Reliable Stream. A TRANS frame's own 9-byte framing (port + type + ISN + CRC
        // trailer) has to fit inside MaxTunnelInnerPayloadLength alongside the application
        // chunk itself, so the real ceiling is 62 - 9 = 53, not 62 (see implementation-state
        // doc: a larger chunk tripped the tunnel's bounds check on real hardware).
        public const byte StreamInnerCommand = 0x7C;
        public const byte StreamAckInnerCommand = 0xFC;
        public const int MaxApplicationChunkLength = 53;

        // Logical service ports.
        public const ushort ServicePortTelemetry = 9010;
        public const ushort ServicePortSettings = 9020;
        public const ushort ServicePortFileTransfer = 9030;
        public const ushort ServicePortMcduTcp = 9050;
        public const ushort ServicePortMcduUdp = 9051;

        // MCDU geometry.
        public const int McduRows = 14;
        public const int McduColumns = 24;

        // CDC serial port configuration.
        public const int BaudRate = 2_000_000;
        public const int ReadBufferSize = 512;

        // Fixed startup frames, sent as-is - never re-derived at runtime.
        public static readonly byte[] RootHandshakeRequest = [0x7E, 0x00, 0x00, 0x12, 0x9D];
        public static readonly byte[] RootHandshakeResponse = [0x7E, 0x00, 0x80, 0x21, 0x2C];
        public static readonly byte[] DeviceInitRequest = [0x7E, 0x01, 0x43, 0x12, 0x00, 0xE1];
        public static readonly byte[] DeviceInitResponse = [0x7E, 0x01, 0xC3, 0x21, 0x80, 0xF0];
    }
}
