using System.Collections.Generic;
using MobiFlightMoza.Session;

namespace MobiFlightMoza.Tests.Mocks
{
    // Records every wire frame handed to it, instead of writing to a real port.
    internal sealed class RecordingFrameSink : IMozaFrameSink
    {
        public List<byte[]> SentWires { get; } = [];
        // Parallel to SentWires, by index - which sends were flagged as an ACK reply.
        public List<bool> SentIsReply { get; } = [];

        public void Send(byte[] wire, bool isReply = false)
        {
            SentWires.Add(wire);
            SentIsReply.Add(isReply);
        }
    }
}
