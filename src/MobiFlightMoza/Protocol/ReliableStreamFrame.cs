using System;

namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// Pure pack/parse of Reliable Stream requests and ACKs. Only v3 (Version=2, full
    /// CRC32 trailer) is supported - the only version this connection ever negotiates
    /// to, even if the device itself declares a newer one in its SYN1.
    /// </summary>
    internal static class ReliableStreamFrame
    {
        public const byte NegotiatedVersion = 2; // v3

        public static byte[] PackSyn(ushort destinationPort, StreamMessageType messageType, ushort isn, ushort announcedPort, byte version)
        {
            if (messageType != StreamMessageType.Syn1 && messageType != StreamMessageType.Syn2)
            {
                throw new ArgumentException("Message type must be Syn1 or Syn2.", nameof(messageType));
            }

            return
            [
                (byte)(destinationPort >> 8), (byte)destinationPort,
                (byte)messageType,
                (byte)isn, (byte)(isn >> 8),
                (byte)announcedPort, (byte)(announcedPort >> 8),
                (byte)(~version & 0xFF), version,
            ];
        }

        public static byte[] PackAck(ushort destinationPort, ushort acknowledgedIsn)
        {
            return
            [
                (byte)(destinationPort >> 8), (byte)destinationPort,
                (byte)acknowledgedIsn, (byte)(acknowledgedIsn >> 8),
            ];
        }

        public static byte[] PackTrans(ushort destinationPort, ushort isn, byte[] applicationData)
        {
            applicationData ??= [];
            if (applicationData.Length > MozaConstants.MaxApplicationChunkLength)
            {
                throw new ArgumentException(
                    $"Application data exceeds the {MozaConstants.MaxApplicationChunkLength}-byte TRANS chunk limit.",
                    nameof(applicationData));
            }

            uint crc = Crc32.Compute(applicationData);
            return
            [
                (byte)(destinationPort >> 8), (byte)destinationPort,
                (byte)StreamMessageType.Trans,
                (byte)isn, (byte)(isn >> 8),
                .. applicationData,
                (byte)crc, (byte)(crc >> 8), (byte)(crc >> 16), (byte)(crc >> 24),
            ];
        }

        public static byte[] PackFin(ushort destinationPort, ushort isn)
        {
            return
            [
                (byte)(destinationPort >> 8), (byte)destinationPort,
                (byte)StreamMessageType.Fin,
                (byte)isn, (byte)(isn >> 8),
            ];
        }

        public static bool TryParseRequest(byte[] payload, out StreamRequest request, out string error)
        {
            request = null;
            error = null;

            if (payload == null || payload.Length < 5)
            {
                error = "Stream request payload is too short.";
                return false;
            }

            ushort destinationPort = (ushort)((payload[0] << 8) | payload[1]);
            byte messageType = payload[2];
            ushort isn = (ushort)(payload[3] | (payload[4] << 8));

            if (messageType == (byte)StreamMessageType.Syn1 || messageType == (byte)StreamMessageType.Syn2)
            {
                if (payload.Length != 9)
                {
                    error = "Stream SYN payload has the wrong length.";
                    return false;
                }
                ushort announcedPort = (ushort)(payload[5] | (payload[6] << 8));
                byte inverseVersion = payload[7];
                byte version = payload[8];
                if (inverseVersion != (byte)(~version & 0xFF))
                {
                    error = "Stream SYN inverse version does not match.";
                    return false;
                }
                request = new StreamRequest(destinationPort, (StreamMessageType)messageType, isn, announcedPort: announcedPort, version: version);
                return true;
            }

            if (messageType == (byte)StreamMessageType.Trans)
            {
                byte[] body = payload[5..];
                if (!TryUnpackTransBody(body, out byte[] applicationData, out error))
                {
                    return false;
                }
                request = new StreamRequest(destinationPort, StreamMessageType.Trans, isn, applicationData: applicationData);
                return true;
            }

            if (messageType == (byte)StreamMessageType.Fin)
            {
                if (payload.Length != 5)
                {
                    error = "Stream FIN payload has the wrong length.";
                    return false;
                }
                request = new StreamRequest(destinationPort, StreamMessageType.Fin, isn);
                return true;
            }

            error = $"Unknown Reliable Stream message type 0x{messageType:X2}.";
            return false;
        }

        private static bool TryUnpackTransBody(byte[] body, out byte[] applicationData, out string error)
        {
            applicationData = null;
            error = null;

            const int checksumSize = 4; // v3 only
            if (body.Length < checksumSize)
            {
                error = "TRANS body is missing its CRC trailer.";
                return false;
            }

            byte[] data = body[..^checksumSize];
            uint received = (uint)(body[^4] | (body[^3] << 8) | (body[^2] << 16) | (body[^1] << 24));
            uint expected = Crc32.Compute(data);
            if (received != expected)
            {
                error = $"TRANS CRC mismatch: received 0x{received:X8}, expected 0x{expected:X8}.";
                return false;
            }

            applicationData = data;
            return true;
        }

        public static bool TryParseAck(byte[] payload, out StreamAck ack)
        {
            ack = default;
            if (payload == null || payload.Length < 4)
            {
                return false;
            }

            ushort destinationPort = (ushort)((payload[0] << 8) | payload[1]);
            ushort acknowledgedIsn = (ushort)(payload[2] | (payload[3] << 8));
            ack = new StreamAck(destinationPort, acknowledgedIsn);
            return true;
        }
    }
}
