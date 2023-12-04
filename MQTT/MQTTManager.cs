using MQTTnet.Client;
using MQTTnet;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace MobiFlight.MQTT
{
    internal static class MQTTManager
    {
        public static string Serial = "MQTTServer";
        public static event Func<MqttClientConnectedEventArgs, Task> ConnectedAsync;

        private static MqttFactory mqttFactory;
        private static IMqttClient mqttClient;
        private static readonly Dictionary<string, string> outputCache = new Dictionary<string, string>();

        /// <summary>
        /// Compares the specified serial to the serial used to identify MQTT Server configurations.
        /// </summary>
        /// <param name="serial">The serial to verify.</param>
        /// <returns>True if the serial is an MQTT Server configuration.</returns>
        public static bool IsMQTTSerial(string serial)
        {
            return serial == Serial;
        }

        /// <summary>
        /// Connects to an MQTT server using the settings saved in the app config.
        /// </summary>
        /// <returns>A task.</returns>
        public static async Task Connect()
        {
            var settings = MQTTServerSettings.Load();

            mqttFactory = new MqttFactory();
            mqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(settings.Address, settings.Port);

            // Only use username/password authentication if the username setting is set to something.
            if (!string.IsNullOrWhiteSpace(settings.Username))
            {
                var unsecurePassword = settings.GetUnsecurePassword();
                mqttClientOptions.WithCredentials(settings.Username, unsecurePassword);
                unsecurePassword = "";
            }

            // Only use TLS and validate the certificate if requested.
            if (settings.EncryptConnection)
            {
                if (settings.ValidateCertificate)
                {
                    mqttClientOptions.WithTlsOptions(
                        o =>
                        {
                            o.UseTls();
                        });
                }
                else
                {
                    // From https://github.com/dotnet/MQTTnet/blob/master/Samples/Client/Client_Connection_Samples.cs
                    mqttClientOptions.WithTlsOptions(
                    o =>
                    {
                        o.UseTls();
                        // The used public broker sometimes has invalid certificates. This sample accepts all
                        // certificates. This should not be used in live environments.
                        o.WithCertificateValidationHandler(_ => true);
                    });
                }
            }

            // Add incoming message handler prior to connecting so queued events are processed.
            mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
            mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;

            // This will throw an exception if the server is not available.
            // The result from this message returns additional data which was sent 
            // from the server. Please refer to the MQTT protocol specification for details.
            await mqttClient.ConnectAsync(mqttClientOptions.Build(), CancellationToken.None);

            Log.Instance.log($"MQTT: Connected to {settings.Address}:{settings.Port}.", LogSeverity.Info);
        }

        private static Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            ConnectedAsync?.Invoke(arg);
            return Task.CompletedTask;
        }

        private static Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            Log.Instance.log($"MQTT: Received incoming message.", LogSeverity.Debug);
            return Task.CompletedTask;
        }

        public static async Task Publish(string topic, string payload)
        {
            if (!mqttClient.IsConnected)
                return;

            // Don't spam MQTT server if the payload is the same as last time for the topic.
            if (outputCache.ContainsKey(topic) && outputCache[topic] == payload)
                return;

            try
            {
                var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .Build();

                await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
                outputCache[topic] = payload;

                Log.Instance.log($"MQTT: Published {payload} to {topic}.", LogSeverity.Debug);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Unable to publish {payload} to {topic}: {ex.Message}", LogSeverity.Error);
            }
        }

        /// <summary>
        /// Subscribes to an MQTT topic. This method shouldn't be called until after the Connect() task completes.
        /// Use the MQTTServer.ConnectedAsync event to determine when the connection process is complete.
        /// </summary>
        /// <param name="topic">The MQTT topic to subscribe to.</param>
        /// <returns>A task.</returns>
        public static async Task Subscribe(string topic)
        {
            if (!mqttClient.IsConnected)
            {
                Log.Instance.log("MQTT: Unable to subscribe to topic, client isnt' connected.", LogSeverity.Error);
                return;
            }

            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic(topic);
                    })
                .Build();

            var response = await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            Log.Instance.log($"MQTT: Subscribed to {topic}.", LogSeverity.Debug);
        }

        /// <summary>
        /// Disconnects from the MQTT server.
        /// </summary>
        /// <returns>A task.</returns>
        public static async Task Disconnect()
        {
            // Send a clean disconnect to the server by calling _DisconnectAsync_. Without this the TCP connection
            // gets dropped and the server will handle this as a non clean disconnect (see MQTT spec for details).
            var mqttClientDisconnectOptions = mqttFactory.CreateClientDisconnectOptionsBuilder().Build();

            await mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);

            Log.Instance.log($"MQTT: Disconnected from server.", LogSeverity.Info);
        }
    }
}