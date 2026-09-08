using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MobiFlight.BrowserMessages.Transport
{
    /// <summary>
    /// In-process WebSocket server for frontend messaging. Transport-only - routing by message
    /// key stays in MessageExchange. Built on TcpListener + WebSocket.CreateFromStream, no
    /// third-party dependency; only the HTTP Upgrade handshake is ours. Loopback-only.
    /// See docs/architecture/frontend-backend-messaging.md.
    /// </summary>
    public sealed class MessageServer : IDisposable
    {
        private const string WebSocketGuid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        /// <summary>Most recently started server; set before FrontendPanel needs its Url.</summary>
        public static MessageServer Current { get; private set; }

        private readonly ConcurrentDictionary<Guid, Connection> _connections = new ConcurrentDictionary<Guid, Connection>();
        private readonly HashSet<string> _allowedOrigins;
        private TcpListener _listener;
        private CancellationTokenSource _cts;

        public int Port { get; private set; }
        public bool IsRunning { get; private set; }
        public string Url => $"ws://127.0.0.1:{Port}/";

        /// <summary>Open connection count; mainly for tests to wait on.</summary>
        public int ConnectionCount => _connections.Count;

        /// <summary>Fires on a threadpool thread, never the UI thread.</summary>
        public event Action<string> MessageReceived;

        /// <param name="preferredPort">Falls back to an ephemeral port if taken.</param>
        /// <param name="allowedOrigins">Allowed browser Origins; no Origin header is also allowed (non-browser clients).</param>
        public MessageServer(int preferredPort, params string[] allowedOrigins)
        {
            Port = preferredPort;
            _allowedOrigins = new HashSet<string>(allowedOrigins, StringComparer.OrdinalIgnoreCase);
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();

            try
            {
                _listener = new TcpListener(IPAddress.Loopback, Port);
                _listener.Start();
            }
            catch (SocketException ex)
            {
                Log.Instance.log($"[Messaging] Port {Port} unavailable ({ex.Message}), falling back to an ephemeral port.", LogSeverity.Warn);
                _listener = new TcpListener(IPAddress.Loopback, 0);
                _listener.Start();
            }

            Port = ((IPEndPoint)_listener.LocalEndpoint).Port;
            IsRunning = true;
            Current = this;

            _ = AcceptLoopAsync(_cts.Token);

            Log.Instance.log($"[Messaging] Listening on {Url}", LogSeverity.Info);
        }

        private async Task AcceptLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                TcpClient client;
                try
                {
                    client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                }
                catch (Exception) when (token.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"[Messaging] Accept failed: {ex.Message}", LogSeverity.Error);
                    continue;
                }

                _ = HandleClientAsync(client, token);
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            var stream = client.GetStream();
            string key, origin;

            try
            {
                (key, origin) = await ReadHandshakeRequestAsync(stream, token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"[Messaging] Handshake failed: {ex.Message}", LogSeverity.Debug);
                client.Dispose();
                return;
            }

            // Check before the 101 response goes out, not after.
            if (!IsOriginAllowed(origin))
            {
                Log.Instance.log($"[Messaging] Rejected connection from disallowed origin: {origin}", LogSeverity.Warn);
                try
                {
                    var rejection = Encoding.ASCII.GetBytes("HTTP/1.1 403 Forbidden\r\nConnection: close\r\n\r\n");
                    await stream.WriteAsync(rejection, token).ConfigureAwait(false);
                }
                catch (Exception) { /* client may already be gone; nothing to do */ }
                client.Dispose();
                return;
            }

            try
            {
                await SendHandshakeAcceptAsync(stream, key, token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"[Messaging] Handshake accept failed: {ex.Message}", LogSeverity.Debug);
                client.Dispose();
                return;
            }

            var socket = WebSocket.CreateFromStream(stream, isServer: true, subProtocol: null, keepAliveInterval: TimeSpan.FromSeconds(30));
            var connection = new Connection(Guid.NewGuid(), client, socket);
            _connections[connection.Id] = connection;
            Log.Instance.log($"[Messaging] Client connected: {origin ?? "(no origin)"}", LogSeverity.Info);

            connection.Start(
                onMessage: message => MessageReceived?.Invoke(message),
                onClosed: () =>
                {
                    _connections.TryRemove(connection.Id, out _);
                    Log.Instance.log("[Messaging] Client disconnected.", LogSeverity.Info);
                },
                token);
        }

        private bool IsOriginAllowed(string origin)
        {
            if (string.IsNullOrEmpty(origin)) return true; // non-browser client
            return _allowedOrigins.Contains(origin);
        }

        private static async Task<(string Key, string Origin)> ReadHandshakeRequestAsync(NetworkStream stream, CancellationToken token)
        {
            var requestText = await ReadHttpRequestAsync(stream, token).ConfigureAwait(false);

            var key = ReadHeader(requestText, "Sec-WebSocket-Key")
                ?? throw new InvalidOperationException("Missing Sec-WebSocket-Key header.");
            var origin = ReadHeader(requestText, "Origin");

            return (key, origin);
        }

        private static Task SendHandshakeAcceptAsync(NetworkStream stream, string key, CancellationToken token)
        {
            var accept = Convert.ToBase64String(SHA1.HashData(Encoding.ASCII.GetBytes(key + WebSocketGuid)));

            var response =
                "HTTP/1.1 101 Switching Protocols\r\n" +
                "Upgrade: websocket\r\n" +
                "Connection: Upgrade\r\n" +
                $"Sec-WebSocket-Accept: {accept}\r\n\r\n";

            return stream.WriteAsync(Encoding.ASCII.GetBytes(response), token).AsTask();
        }

        private static async Task<string> ReadHttpRequestAsync(NetworkStream stream, CancellationToken token)
        {
            var buffer = new byte[8192];
            var builder = new StringBuilder();

            while (!builder.ToString().Contains("\r\n\r\n"))
            {
                var read = await stream.ReadAsync(buffer, token).ConfigureAwait(false);
                if (read == 0) throw new IOException("Connection closed during handshake.");
                builder.Append(Encoding.ASCII.GetString(buffer, 0, read));

                if (builder.Length > 32 * 1024) throw new InvalidOperationException("Handshake request too large.");
            }

            return builder.ToString();
        }

        private static string ReadHeader(string requestText, string headerName)
        {
            foreach (var line in requestText.Split("\r\n"))
            {
                var separatorIndex = line.IndexOf(':');
                if (separatorIndex < 0) continue;
                if (!string.Equals(line.Substring(0, separatorIndex).Trim(), headerName, StringComparison.OrdinalIgnoreCase)) continue;
                return line.Substring(separatorIndex + 1).Trim();
            }
            return null;
        }

        /// <summary>Non-blocking: enqueues onto each connection's send channel, no per-session addressing.</summary>
        public void Broadcast(string json)
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            foreach (var connection in _connections.Values)
            {
                connection.TrySend(bytes);
            }
        }

        public void Stop()
        {
            _cts?.Cancel();
            _listener?.Stop();

            foreach (var connection in _connections.Values)
            {
                connection.Dispose();
            }
            _connections.Clear();

            IsRunning = false;
            if (Current == this) Current = null;
        }
        public void Dispose()
        {
            Stop();
            _cts?.Dispose();
            _listener?.Dispose();
        }

        /// <summary>One WebSocket connection. Send channel + writer task, since WebSocket.SendAsync allows only one outstanding call at a time.</summary>
        private class Connection : IDisposable
        {
            private readonly TcpClient _client;
            private readonly WebSocket _socket;
            private readonly Channel<byte[]> _sendChannel = Channel.CreateUnbounded<byte[]>(
                new UnboundedChannelOptions { SingleReader = true });

            public Guid Id { get; }

            public Connection(Guid id, TcpClient client, WebSocket socket)
            {
                Id = id;
                _client = client;
                _socket = socket;
            }

            public void Start(Action<string> onMessage, Action onClosed, CancellationToken token)
            {
                _ = WriteLoopAsync(token);
                _ = ReceiveLoopAsync(onMessage, onClosed, token);
            }

            public bool TrySend(byte[] bytes) => _sendChannel.Writer.TryWrite(bytes);

            private async Task WriteLoopAsync(CancellationToken token)
            {
                try
                {
                    await foreach (var bytes in _sendChannel.Reader.ReadAllAsync(token).ConfigureAwait(false))
                    {
                        if (_socket.State != WebSocketState.Open) continue;
                        await _socket.SendAsync(bytes, WebSocketMessageType.Text, endOfMessage: true, token).ConfigureAwait(false);
                    }
                }
                catch (Exception ex) when (!(ex is OperationCanceledException))
                {
                    Log.Instance.log($"[Messaging] Send to client failed: {ex.Message}", LogSeverity.Debug);
                }
            }

            private async Task ReceiveLoopAsync(Action<string> onMessage, Action onClosed, CancellationToken token)
            {
                var buffer = new byte[8192];
                try
                {
                    while (_socket.State == WebSocketState.Open && !token.IsCancellationRequested)
                    {
                        using var messageStream = new MemoryStream();
                        WebSocketReceiveResult result;
                        do
                        {
                            result = await _socket.ReceiveAsync(buffer, token).ConfigureAwait(false);
                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token).ConfigureAwait(false);
                                return;
                            }
                            messageStream.Write(buffer, 0, result.Count);
                        } while (!result.EndOfMessage);

                        onMessage(Encoding.UTF8.GetString(messageStream.ToArray()));
                    }
                }
                catch (Exception ex) when (!(ex is OperationCanceledException))
                {
                    Log.Instance.log($"[Messaging] Receive failed: {ex.Message}", LogSeverity.Debug);
                }
                finally
                {
                    _sendChannel.Writer.TryComplete();
                    onClosed();
                    Dispose();
                }
            }

            public void Dispose()
            {
                _socket?.Dispose();
                _client?.Dispose();
            }
        }
    }
}
