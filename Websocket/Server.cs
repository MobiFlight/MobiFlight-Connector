using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebsocket;

namespace MobiFlight.Websocket
{
    public class Server
    {
        WatsonWsServer server = null;
        bool started = false;
        public static Server Instance { get; set; }

        Dictionary<int, string> cache = new Dictionary<int, string>();

        public Server()
        {
        }

        public void SetConfig(int port = 5000, string host = "127.0.0.1")
        {
            server = new WatsonWsServer(host, port, false);
            server.ClientConnected += ClientConnected;
            server.ClientDisconnected += ClientDisconnected;
            server.MessageReceived += MessageReceived;
        }

        public void Start()
        {
            if (!started)
            {
                cache.Clear();
                server.Start();
                started = true;
            }
        }

        public void Stop()
        {
            if (started)
            {
                server.Stop();
                started = false;
            }
        }

        public void SendMessage(int HashCode, string Message)
        {
            if (cache.ContainsKey(HashCode) && cache[HashCode] == Message) return;

            cache[HashCode] = Message;

            foreach (string client in server.ListClients())
                server.SendAsync(client, Message);
        }

        void ClientConnected(object sender, ClientConnectedEventArgs args)
        {
            Log.Instance.log("WSS: Client connected: " + args.IpPort, LogSeverity.Info);
        }

        void ClientDisconnected(object sender, ClientDisconnectedEventArgs args)
        {
            Log.Instance.log("WSS: Client disconnected: " + args.IpPort, LogSeverity.Info);
        }

        void MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Log.Instance.log(
                "WSS: Message received from " + args.IpPort + ": " + Encoding.UTF8.GetString(args.Data.ToArray()),
                LogSeverity.Debug
            );
        }
    }
}
