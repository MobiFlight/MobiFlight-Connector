using System.Collections.Generic;
using MobiFlightMoza.Session;

namespace MobiFlightMoza.Tests.Mocks
{
    // Records every wire frame handed to it, instead of writing to a real port.
    internal sealed class RecordingFrameSink : IMozaFrameSink
    {
        public List<byte[]> SentWires { get; } = [];

        public void Send(byte[] wire) => SentWires.Add(wire);
    }
}
