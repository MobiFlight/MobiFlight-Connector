using MobiFlightMoza.Protocol;
namespace MobiFlightMoza.Session.Tests
{
    // Drives a multiplexer through a full SYN1->SYN2->ACK handshake, for tests that need
    // an established connection to exercise TrySend against.
    internal static class MultiplexerTestHelper
    {
        public static ReliableStreamConnection Establish(ReliableStreamMultiplexer multiplexer, ushort servicePort, ushort announcedPort, DateTime now)
        {
            byte[] syn1 = ReliableStreamFrame.PackSyn(servicePort, StreamMessageType.Syn1, 1, announcedPort, 3);
            multiplexer.HandleStreamMessage(syn1, isReply: false, now);
            multiplexer.TryGetConnection(servicePort, out var connection);
            byte[] ack = ReliableStreamFrame.PackAck(connection.LocalPort, connection.LocalPort);
            multiplexer.HandleStreamMessage(ack, isReply: true, now);
            return connection;
        }
    }
}