using System;
using System.Collections.Generic;
using MobiFlightMoza.Protocol;

namespace MobiFlightMoza.Session
{
    /// <summary>
    /// Per-port Reliable Stream state machine. Pure: every timing decision is
    /// a function of an explicit <c>now</c> parameter, never the wall clock.
    /// <para>
    /// SYN2 and TRANS/FIN retransmission share one "pending" slot: SYN2 is sent via
    /// <see cref="BeginSyn2Handshake"/> with <c>PendingIsn = LocalPort</c>, which is
    /// exactly what <see cref="AcceptAck"/> already checks to confirm establishment -
    /// so one retransmit/timeout path covers both phases.
    /// </para>
    /// </summary>
    internal sealed class ReliableStreamConnection
    {
        public const double RetransmitSeconds = 0.6;
        public const double HeartbeatSeconds = 3.0;
        public const int MaxRetries = 8;
        public const double ConnectionTimeoutSeconds = 10.0;

        public ushort ServicePort { get; }
        public ushort LocalPort { get; }
        public ushort PeerPort { get; }
        public byte Version { get; }
        public ushort PeerLastIsn { get; private set; }
        public ushort SendIsn { get; private set; }
        public bool Established { get; private set; }
        public bool PeerClosed { get; private set; }
        public bool LocalFinAcked { get; private set; }

        // Computed, not stored: either side's FIN can arrive/get-acked first, and both
        // paths must agree once the second one lands - a single settable flag would need
        // both call sites to remember to set it.
        public bool IsFullyClosed => PeerClosed && LocalFinAcked;

        private ushort? PendingIsn;
        private bool PendingIsFin;
        private byte[] PendingWire;
        private DateTime PendingSentAt;
        private int PendingRetries;
        private DateTime LastActivityAt;
        private readonly List<byte> ApplicationBuffer = [];

        private ReliableStreamConnection(ushort servicePort, ushort localPort, ushort peerPort, byte version, ushort peerLastIsn, ushort sendIsn, DateTime now)
        {
            ServicePort = servicePort;
            LocalPort = localPort;
            PeerPort = peerPort;
            Version = version;
            PeerLastIsn = peerLastIsn;
            SendIsn = sendIsn;
            LastActivityAt = now;
        }

        public static ReliableStreamConnection FromSyn1(ushort servicePort, ushort localPort, StreamRequest syn1, DateTime now)
        {
            if (syn1 == null || syn1.MessageType != StreamMessageType.Syn1 || syn1.AnnouncedPort == null || syn1.Version == null)
            {
                throw new ArgumentException("FromSyn1 requires a complete SYN1 request.", nameof(syn1));
            }

            // Externally negotiate fixed to v3, even if the device declares a higher version.
            byte version = Math.Min(ReliableStreamFrame.NegotiatedVersion, syn1.Version.Value);
            ushort sendIsn = (ushort)((localPort + 1) & 0xFFFF);
            return new ReliableStreamConnection(servicePort, localPort, syn1.AnnouncedPort.Value, version, syn1.Isn, sendIsn, now);
        }

        // Sends SYN2 and arms the same retransmit/establishment tracking AcceptAck checks.
        public byte[] BeginSyn2Handshake(DateTime now)
        {
            byte[] wire = ReliableStreamFrame.PackSyn(PeerPort, StreamMessageType.Syn2, LocalPort, LocalPort, Version);
            ArmPending(LocalPort, isFin: false, wire, now);
            return wire;
        }

        public bool AcceptAck(StreamAck ack, DateTime now)
        {
            if (ack.DestinationPort != LocalPort) return false;

            if (!Established)
            {
                if (PendingIsn.HasValue && ack.AcknowledgedIsn == PendingIsn.Value && ack.AcknowledgedIsn == LocalPort)
                {
                    Established = true;
                    ClearPending();
                    LastActivityAt = now;
                    return true;
                }
                return false;
            }

            if (PendingIsn.HasValue && ack.AcknowledgedIsn == PendingIsn.Value)
            {
                bool completedFin = PendingIsFin;
                ClearPending();
                LastActivityAt = now;
                if (completedFin)
                {
                    LocalFinAcked = true;
                    Established = false;
                }
                return true;
            }
            return false;
        }

        // Always returns the current cumulative ACK value, whether or not this request
        // advanced it - a duplicate/out-of-order request is still ACKed, just not delivered.
        public ushort AcceptRequest(StreamRequest request, DateTime now)
        {
            if (request.DestinationPort != LocalPort)
            {
                throw new ArgumentException("Request targets a different local port.", nameof(request));
            }

            ushort expected = (ushort)((PeerLastIsn + 1) & 0xFFFF);
            if (request.Isn == expected)
            {
                PeerLastIsn = request.Isn;
                if (request.MessageType == StreamMessageType.Trans && request.ApplicationData.Length > 0)
                {
                    ApplicationBuffer.AddRange(request.ApplicationData);
                }
                else if (request.MessageType == StreamMessageType.Fin)
                {
                    PeerClosed = true;
                }
                LastActivityAt = now;
            }
            return PeerLastIsn;
        }

        public byte[] ReadApplicationBytes()
        {
            byte[] data = [.. ApplicationBuffer];
            ApplicationBuffer.Clear();
            return data;
        }

        public bool TryBeginTransmission(byte[] applicationData, DateTime now, out byte[] wire)
        {
            wire = null;
            if (!Established || PeerClosed || IsFullyClosed || PendingIsn.HasValue) return false;

            ushort isn = NextSendIsn();
            wire = ReliableStreamFrame.PackTrans(PeerPort, isn, applicationData);
            ArmPending(isn, isFin: false, wire, now);
            return true;
        }

        public bool TryBeginFin(DateTime now, out byte[] wire)
        {
            wire = null;
            if (!Established || LocalFinAcked || IsFullyClosed || PendingIsn.HasValue) return false;

            ushort isn = NextSendIsn();
            wire = ReliableStreamFrame.PackFin(PeerPort, isn);
            ArmPending(isn, isFin: true, wire, now);
            return true;
        }

        private ushort NextSendIsn()
        {
            ushort isn = SendIsn;
            SendIsn = (ushort)((SendIsn + 1) & 0xFFFF);
            return isn;
        }

        // Both TRANS and FIN wait for the same single "pending" ACK slot (see class remarks).
        private void ArmPending(ushort isn, bool isFin, byte[] wire, DateTime now)
        {
            PendingIsn = isn;
            PendingIsFin = isFin;
            PendingWire = wire;
            PendingSentAt = now;
            PendingRetries = 0;
        }

        public bool HasPending => PendingIsn.HasValue;

        // True once retries are exhausted, so the caller can fault instead of retrying forever.
        public bool IsRetransmitExhausted => PendingIsn.HasValue && PendingRetries >= MaxRetries;

        public bool TryGetRetransmit(DateTime now, out byte[] wire)
        {
            wire = null;
            if (!PendingIsn.HasValue) return false;
            if ((now - PendingSentAt).TotalSeconds < RetransmitSeconds) return false;
            if (PendingRetries >= MaxRetries) return false;

            PendingRetries++;
            PendingSentAt = now;
            wire = PendingWire;
            return true;
        }

        public bool NeedsHeartbeat(DateTime now)
        {
            return Established && !PendingIsn.HasValue && !PeerClosed
                && (now - LastActivityAt).TotalSeconds >= HeartbeatSeconds;
        }

        public bool IsTimedOut(DateTime now) => (now - LastActivityAt).TotalSeconds >= ConnectionTimeoutSeconds;

        private void ClearPending()
        {
            PendingIsn = null;
            PendingWire = null;
            PendingRetries = 0;
        }
    }
}
