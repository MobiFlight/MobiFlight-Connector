using System;

namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// Wraps/unwraps the FCD Display 0x43 tunnel. The inner command and reply flag live
    /// in the tunnel payload's first byte, never in the outer command's reply bit.
    /// </summary>
    internal static class MozaTunnel
    {
        public static byte[] Wrap(byte innerCommand, byte[] innerPayload, bool isReply = false)
        {
            innerPayload ??= [];
            if (innerPayload.Length > MozaConstants.MaxTunnelInnerPayloadLength)
            {
                throw new ArgumentException(
                    $"Tunnel inner payload exceeds {MozaConstants.MaxTunnelInnerPayloadLength} bytes.",
                    nameof(innerPayload));
            }

            byte commandByte = (byte)(innerCommand | (isReply ? 0x80 : 0x00));
            byte[] payload = [commandByte, .. innerPayload];

            return SerialLinkFrame.EncodeRaw(MozaConstants.TunnelCommand, MozaConstants.DevicePairToDevice, payload);
        }

        public static bool TryUnwrap(SerialLinkMessage message, out MozaTunnelMessage tunnelMessage)
        {
            tunnelMessage = null;
            if (message?.Payload == null || message.Payload.Length == 0) return false;
            if ((message.Command & 0x7F) != MozaConstants.TunnelCommand) return false;

            byte commandByte = message.Payload[0];
            bool isReply = (commandByte & 0x80) != 0;
            byte innerCommand = (byte)(commandByte & 0x7F);

            tunnelMessage = new MozaTunnelMessage(innerCommand, isReply, message.Payload[1..]);
            return true;
        }
    }
}
