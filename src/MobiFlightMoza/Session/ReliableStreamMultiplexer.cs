using System;
using System.Collections.Generic;
using MobiFlightMoza.Protocol;

namespace MobiFlightMoza.Session
{
    /// <summary>
    /// Owns one <see cref="ReliableStreamConnection"/> per accepted service port.
    /// Only accepts device-initiated connections (SYN1 from the device) - MOZA never
    /// initiates 9010/9020/9050 itself. Outgoing application data is chunked to
    /// <see cref="MozaConstants.MaxApplicationChunkLength"/> bytes and drained one chunk at
    /// a time from <see cref="Tick"/>, rather than blocking the caller on each ACK - this
    /// runs on a background thread a UI-adjacent session owns, so it must never block.
    /// </summary>
    internal sealed class ReliableStreamMultiplexer
    {
        private static readonly HashSet<ushort> ReservedPorts =
        [
            MozaConstants.ServicePortTelemetry,
            MozaConstants.ServicePortSettings,
            MozaConstants.ServicePortFileTransfer,
            MozaConstants.ServicePortMcduTcp,
            MozaConstants.ServicePortMcduUdp,
        ];

        private readonly IMozaFrameSink Sink;
        private readonly HashSet<ushort> AcceptedServicePorts;
        private readonly Dictionary<ushort, ReliableStreamConnection> ConnectionsByLocalPort = [];
        private readonly Dictionary<ushort, ReliableStreamConnection> ConnectionsByServicePort = [];
        private readonly Dictionary<ushort, Queue<byte[]>> SendQueueByServicePort = [];
        private bool Closing;

        public event Action<string> Error;
        public event Action<ushort> ConnectionEstablished;

        public ReliableStreamMultiplexer(IMozaFrameSink sink, IEnumerable<ushort> acceptedServicePorts)
        {
            Sink = sink;
            AcceptedServicePorts = [.. acceptedServicePorts];
        }

        public bool TryGetConnection(ushort servicePort, out ReliableStreamConnection connection)
            => ConnectionsByServicePort.TryGetValue(servicePort, out connection);

        public void HandleStreamMessage(byte[] tunnelPayload, bool isReply, DateTime now)
        {
            if (isReply)
            {
                HandleAck(tunnelPayload, now);
                return;
            }

            if (!ReliableStreamFrame.TryParseRequest(tunnelPayload, out var request, out string parseError))
            {
                Error?.Invoke(parseError);
                return;
            }

            if (request.MessageType == StreamMessageType.Syn1)
            {
                HandleSyn1(request, now);
                return;
            }

            HandleRequest(request, now);
        }

        private void HandleAck(byte[] tunnelPayload, DateTime now)
        {
            if (!ReliableStreamFrame.TryParseAck(tunnelPayload, out var ack)) return;
            if (!ConnectionsByLocalPort.TryGetValue(ack.DestinationPort, out var connection)) return;

            bool wasEstablished = connection.Established;
            bool accepted = connection.AcceptAck(ack, now);

            if (accepted && !wasEstablished && connection.Established)
            {
                ConnectionEstablished?.Invoke(connection.ServicePort);
            }
            else if (accepted && connection.IsFullyClosed)
            {
                RemoveConnection(connection);
            }
            else if (accepted && connection.PeerClosed && !connection.LocalFinAcked && !connection.HasPending)
            {
                if (connection.TryBeginFin(now, out byte[] finWire)) Sink.Send(finWire);
            }
        }

        private void HandleSyn1(StreamRequest syn1, DateTime now)
        {
            if (Closing) return;
            if (!AcceptedServicePorts.Contains(syn1.DestinationPort)) return;

            if (ConnectionsByServicePort.TryGetValue(syn1.DestinationPort, out var current))
            {
                if (!current.Established && current.PeerPort == syn1.AnnouncedPort)
                {
                    // Duplicate SYN1 while the handshake is outstanding: resend SYN2 now,
                    // ignoring the retransmit-interval gate since the device asked again.
                    Sink.Send(current.BeginSyn2Handshake(now));
                    return;
                }
                RemoveConnection(current); // stale connection - the device is starting over
            }

            ushort localPort = AllocateLocalPort(syn1.AnnouncedPort ?? 0);
            var connection = ReliableStreamConnection.FromSyn1(syn1.DestinationPort, localPort, syn1, now);
            ConnectionsByLocalPort[localPort] = connection;
            ConnectionsByServicePort[syn1.DestinationPort] = connection;

            Sink.Send(connection.BeginSyn2Handshake(now));
        }

        private void HandleRequest(StreamRequest request, DateTime now)
        {
            if (!ConnectionsByLocalPort.TryGetValue(request.DestinationPort, out var connection)) return;

            bool peerWasClosed = connection.PeerClosed;
            ushort acknowledgedIsn = connection.AcceptRequest(request, now);
            Sink.Send(ReliableStreamFrame.PackAck(connection.PeerPort, acknowledgedIsn));

            bool peerJustClosed = request.MessageType == StreamMessageType.Fin && !peerWasClosed && connection.PeerClosed;
            if (!peerJustClosed) return;

            if (connection.IsFullyClosed)
            {
                RemoveConnection(connection);
            }
            else if (!connection.HasPending && connection.TryBeginFin(now, out byte[] finWire))
            {
                Sink.Send(finWire);
            }
        }

        private ushort AllocateLocalPort(ushort preferred)
        {
            ushort port = preferred;
            while (ReservedPorts.Contains(port) || ConnectionsByLocalPort.ContainsKey(port))
            {
                ushort next = (ushort)((port + 1) & 0xFFFF);
                if (next == preferred)
                {
                    throw new InvalidOperationException("No Reliable Stream local port is available.");
                }
                port = next;
            }
            return port;
        }

        private void RemoveConnection(ReliableStreamConnection connection)
        {
            ConnectionsByLocalPort.Remove(connection.LocalPort);
            ConnectionsByServicePort.Remove(connection.ServicePort);
            SendQueueByServicePort.Remove(connection.ServicePort);
        }

        // Chunks and enqueues; actual sends happen one at a time from Tick, once each
        // previous chunk is ACKed (only one TRANS may be outstanding per connection).
        public bool TrySend(ushort servicePort, byte[] applicationData)
        {
            if (!ConnectionsByServicePort.ContainsKey(servicePort)) return false;

            if (!SendQueueByServicePort.TryGetValue(servicePort, out var queue))
            {
                queue = new Queue<byte[]>();
                SendQueueByServicePort[servicePort] = queue;
            }

            applicationData ??= [];
            for (int offset = 0; offset < applicationData.Length; offset += MozaConstants.MaxApplicationChunkLength)
            {
                int length = Math.Min(MozaConstants.MaxApplicationChunkLength, applicationData.Length - offset);
                queue.Enqueue(applicationData[offset..(offset + length)]);
            }
            return true;
        }

        public void Tick(DateTime now)
        {
            List<ReliableStreamConnection> snapshot = [.. ConnectionsByLocalPort.Values];
            foreach (var connection in snapshot)
            {
                if (connection.TryGetRetransmit(now, out byte[] retryWire))
                {
                    Sink.Send(retryWire);
                    continue;
                }
                if (connection.IsRetransmitExhausted)
                {
                    Error?.Invoke($"Reliable Stream retransmit exhausted on service port {connection.ServicePort}.");
                    RemoveConnection(connection);
                    continue;
                }
                if (!connection.Established || connection.HasPending) continue;

                if (SendQueueByServicePort.TryGetValue(connection.ServicePort, out var queue) && queue.Count > 0)
                {
                    byte[] chunk = queue.Dequeue();
                    if (connection.TryBeginTransmission(chunk, now, out byte[] wire)) Sink.Send(wire);
                    continue;
                }

                if (connection.NeedsHeartbeat(now) && connection.TryBeginTransmission([], now, out byte[] heartbeatWire))
                {
                    Sink.Send(heartbeatWire);
                }
            }
        }

        public void BeginClose(DateTime now)
        {
            Closing = true;
            foreach (var connection in ConnectionsByLocalPort.Values)
            {
                if (connection.Established && !connection.LocalFinAcked && !connection.HasPending
                    && connection.TryBeginFin(now, out byte[] wire))
                {
                    Sink.Send(wire);
                }
            }
        }

        public bool IsCloseComplete => ConnectionsByLocalPort.Count == 0;
    }
}
