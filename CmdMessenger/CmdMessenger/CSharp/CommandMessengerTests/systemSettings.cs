using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandMessenger;
using CommandMessenger.TransportLayer;

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
