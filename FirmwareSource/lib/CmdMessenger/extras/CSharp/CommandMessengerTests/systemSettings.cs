using System;
using CommandMessenger;
using CommandMessenger.Transport;

namespace CommandMessengerTests
{
    public class systemSettings
    {
        public String Description { get; set; }
        public ITransport Transport { get; set; }
        public float MinReceiveSpeed { get; set; }
        public float MinSendSpeed { get; set; }
        public float MinDirectSendSpeed { get; set; }
        public BoardType BoardType { get; set; }
        public int sendBufferMaxLength { get; set; }
    }
}
