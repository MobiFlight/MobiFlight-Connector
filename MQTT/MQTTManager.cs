using MQTTnet.Client;
using MQTTnet;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using static MobiFlight.MobiFlightButton;

namespace MobiFlight.MQTT
{
    internal static class MQTTManager
    {
        public static string Serial = "MQTTServer";
        public static event Func<MqttClientConnectedEventArgs, Task> ConnectedAsync;
        public static event ButtonEventHandler OnButtonPressed;

        private static readonly Dictionary<string, MQTTInput> Inputs = new Dictionary<string, MQTTInput>();

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

        private static void LoadInputs()
        {
            Inputs["mobiflight/input/parkingbrake"] = new MQTTInput
            {
                Type = MQTTInputType.Button,
                Label = "Parking brake",
                Topic = "mobiflight/input/parkingbrake"
            };

            Inputs["mobiflight/input/brightness"] = new MQTTInput
            {
                Type = MQTTInputType.AnalogInput,
                Label = "Brightness",
                Topic = "mobiflight/input/brightness"
            };

        }

        /// <summary>
        /// Connects to an MQTT server using the settings saved in the app config.
        /// </summary>
        /// <returns>A task.</returns>
        public static async Task Connect()
        {
            if (mqttClient?.IsConnected ?? false)
                return;

            var settings = MQTTServerSettings.Load();

            LoadInputs();

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
            try
            {
                await mqttClient.ConnectAsync(mqttClientOptions.Build(), CancellationToken.None);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"MQTT: Unable to connect to {settings.Address}:{settings.Port}: {ex.Message}", LogSeverity.Error);
                return;
            }

            Log.Instance.log($"MQTT: Connected to {settings.Address}:{settings.Port}.", LogSeverity.Info);
        }

        private static async Task RegisterInputs()
        {
            if (!mqttClient.IsConnected)
                return;

            try
            {
                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder();

                foreach (var input in Inputs)
                {
                    Log.Instance.log($"MQTT: Subscribing to {input.Value.Topic}", LogSeverity.Debug);
                    mqttSubscribeOptions.WithTopicFilter(f => { f.WithTopic(input.Value.Topic); });
                }

                var response = await mqttClient.SubscribeAsync(mqttSubscribeOptions.Build(), CancellationToken.None);

                Log.Instance.log($"MQTT: Subscribing to all input topics complete.", LogSeverity.Debug);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"MQTT: Error subscribing to topics. {ex.Message}", LogSeverity.Error);
            }
        }

        private static async Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            ConnectedAsync?.Invoke(arg);
            await RegisterInputs();
        }

        private static Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            var input = Inputs[arg.ApplicationMessage.Topic];

            if (input == null)
            {
                Log.Instance.log($"MQTT: Received an incoming message for {arg.ApplicationMessage.Topic} but it's not in the list of topics being watched. This should never happen.", LogSeverity.Error);
                return Task.CompletedTask;
            }

            // Absolute nonsense to parse the incoming message value as a number.
            var payloadString = System.Text.Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment.ToArray());
            if (int.TryParse(payloadString, out int value))
            {
                var eventArgs = new InputEventArgs
                {
                    DeviceLabel = input.Label,
                    Serial = Serial,
                    DeviceId = input.Topic,
                };

                if (input.Type == MQTTInputType.Button)
                {
                    eventArgs.Type = DeviceType.Button;
                    eventArgs.Value = value == 0 ? (int)InputEvent.RELEASE : (int)InputEvent.PRESS;                       
                }
                else if (input.Type == MQTTInputType.AnalogInput)
                {
                    eventArgs.Type = DeviceType.AnalogInput;
                    eventArgs.Value = value;
                }
                else
                {
                    Log.Instance.log($"MQTT: Received incoming message {arg.ApplicationMessage.Topic} for a type that isn't understhood. This should never happen.", LogSeverity.Error);
                    return Task.CompletedTask;
                }

                Log.Instance.log($"MQTT: Received incoming message: {arg.ApplicationMessage.Topic} {value}", LogSeverity.Debug);
                OnButtonPressed?.Invoke(null, eventArgs);
            }
            else
            {
                Log.Instance.log($"MQTT: Unable to parse {payloadString} from {arg.ApplicationMessage.Topic} as a number.", LogSeverity.Error);
            }

            return Task.CompletedTask;
        }

        public static async Task Publish(string topic, string payload)
        {
            if (!mqttClient.IsConnected)
                return;

            // Don't spam MQTT server if the payload is the same as last time for the topic.
            if (outputCache.ContainsKey(topic) && outputCache[topic] == payload)
                return;

            // Don't send anything if the topic is empty.
            if (String.IsNullOrEmpty(topic))
            {
                Log.Instance.log($"MQTT: Received a blank topic, not sending {payload}", LogSeverity.Warn);
                return;
            }

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
                Log.Instance.log($"MQTT: Unable to publish {payload} to {topic}: {ex.Message}", LogSeverity.Error);
            }
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

            Log.Instance.log($"MQTT: Disconnected from server.", LogSeverity.Debug);
        }
    }
}