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

            // DestinationPort:u16 BE | Magic:u8 | ISN:u16 LE | AnnouncedPort:u16 LE | ~Version:u8 | Version:u8
            return
            [
                .. Bytes.U16Be(destinationPort),
                (byte)messageType,
                .. Bytes.U16Le(isn),
                .. Bytes.U16Le(announcedPort),
                (byte)(~version & 0xFF), version,
            ];
        }

        public static byte[] PackAck(ushort destinationPort, ushort acknowledgedIsn)
        {
            // DestinationPort:u16 BE | AcknowledgedISN:u16 LE
            return [.. Bytes.U16Be(destinationPort), .. Bytes.U16Le(acknowledgedIsn)];
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
            // DestinationPort:u16 BE | Magic:u8 | ISN:u16 LE | ApplicationChunk | CRC32:u32 LE
            return [.. Bytes.U16Be(destinationPort), (byte)StreamMessageType.Trans, .. Bytes.U16Le(isn), .. applicationData, .. Bytes.U32Le(crc)];
        }

        public static byte[] PackFin(ushort destinationPort, ushort isn)
        {
            // DestinationPort:u16 BE | Magic:u8 | ISN:u16 LE
            return [.. Bytes.U16Be(destinationPort), (byte)StreamMessageType.Fin, .. Bytes.U16Le(isn)];
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

            ushort destinationPort = Bytes.ReadU16Be(payload, 0);
            byte messageType = payload[2];
            ushort isn = Bytes.ReadU16Le(payload, 3);

            if (messageType == (byte)StreamMessageType.Syn1 || messageType == (byte)StreamMessageType.Syn2)
            {
                if (payload.Length != 9)
                {
                    error = "Stream SYN payload has the wrong length.";
                    return false;
                }
                ushort announcedPort = Bytes.ReadU16Le(payload, 5);
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
            uint received = Bytes.ReadU32Le(body, body.Length - 4);
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

            ushort destinationPort = Bytes.ReadU16Be(payload, 0);
            ushort acknowledgedIsn = Bytes.ReadU16Le(payload, 2);
            ack = new StreamAck(destinationPort, acknowledgedIsn);
            return true;
        }
    }
}
