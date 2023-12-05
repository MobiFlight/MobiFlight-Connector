using MQTTnet.Client;
using MQTTnet;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using static MobiFlight.MobiFlightButton;
using Newtonsoft.Json;
using System.IO;
using SharpDX.DirectInput;

namespace MobiFlight
{
    public class MQTTManager
    {
        public static readonly string Serial = "MQTTServer";
        public event Func<MqttClientConnectedEventArgs, Task> ConnectedAsync;
        public event ButtonEventHandler OnButtonPressed;

        private Dictionary<string, MQTTInput> Inputs = new Dictionary<string, MQTTInput>();

        private MqttFactory mqttFactory;
        private IMqttClient mqttClient;
        private readonly Dictionary<string, string> outputCache = new Dictionary<string, string>();

        public MQTTManager()
        {
            LoadInputs();
        }

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
        /// Provides the list of MQTTInput events defined in the corresponding inputs.mqtt.json file.
        /// </summary>
        /// <returns>A dictionary of the input events, where the keys are the MQTT topics and the values are MQTTInput objects.</returns>
        public Dictionary<string, MQTTInput> GetMqttInputs()
        {
            return Inputs;
        }

        /// <summary>
        /// Loads a list of MQTT inputs from the inputs.mqtt.json file.
        /// </summary>
        public void LoadInputs()
        {
            try
            {
                Inputs = JsonConvert.DeserializeObject<Dictionary<string, MQTTInput>>(File.ReadAllText("MQTT/inputs.mqtt.json"));
                Log.Instance.log($"Loaded {Inputs.Count} MQTT input events", LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Unable to load MQTT input events from MQTT/inputs.mqtt.json: {ex.Message}", LogSeverity.Error);

            }
        }

        /// <summary>
        /// Connects to an MQTT server using the settings saved in the app config.
        /// </summary>
        /// <returns>A task.</returns>
        public async Task Connect()
        {
            if (mqttClient?.IsConnected ?? false)
                return;

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

        /// <summary>
        /// Subscribes to the MQTT topics for each input event described in the inputs.mqtt.json file.
        /// </summary>
        /// <returns>A task that completes once all topics are subscribed to.</returns>
        private async Task SubscribeToInputs()
        {
            if (!mqttClient?.IsConnected ?? true)
                return;

            try
            {
                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder();

                foreach (var input in Inputs)
                {
                    Log.Instance.log($"MQTT: Subscribing to {input.Key}", LogSeverity.Debug);
                    mqttSubscribeOptions.WithTopicFilter(f => { f.WithTopic(input.Key); });
                }

                var response = await mqttClient.SubscribeAsync(mqttSubscribeOptions.Build(), CancellationToken.None);

                Log.Instance.log($"MQTT: Subscribing to all input topics complete.", LogSeverity.Debug);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"MQTT: Error subscribing to topics. {ex.Message}", LogSeverity.Error);
            }
        }

        /// <summary>
        /// Event handler for when the MQTT client completes its connection. Fires a ConnectedAsync
        /// event to any registered listeners, then subscribes to the MQTT input topics.
        /// </summary>
        /// <param name="arg">The connected event arguments</param>
        /// <returns>A task that completes after the topics are subscribed to.</returns>
        private async Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            ConnectedAsync?.Invoke(arg);
            await SubscribeToInputs();
        }

        /// <summary>
        /// Event handler when MQTT messages are received on subscribed topics. Processes the retrieved message,
        /// converts it to a MobiFlight InputEventArgs object, then fires the OnbuttonPressed event.
        /// </summary>
        /// <param name="arg">The received message information</param>
        /// <returns>A task that completes once the received message is processed.</returns>
        private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
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
                // All device types share these three properties so set them first.
                var eventArgs = new InputEventArgs
                {
                    DeviceLabel = input.Label,
                    Serial = Serial,
                    DeviceId = input.Label,
                };

                // Set specific type and value properties based on the input's type defined in the inputs.mqtt.json file.
                if (input.Type == DeviceType.Button)
                {
                    eventArgs.Type = DeviceType.Button;
                    eventArgs.Value = value == 0 ? (int)InputEvent.RELEASE : (int)InputEvent.PRESS;                       
                }
                else if (input.Type == DeviceType.AnalogInput)
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

        /// <summary>
        /// Publishes data to an MQTT topic, caching payload values to avoid sending the same
        /// message repeatedly when the data doesn't change.
        /// </summary>
        /// <param name="topic">The topic to publish to.</param>
        /// <param name="payload">The paylod to send.</param>
        /// <returns>A task that completes once the payload is published to the topic.</returns>
        public async Task Publish(string topic, string payload)
        {
            if (!mqttClient?.IsConnected ?? true)
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
                    .WithPayload(payload);

                await mqttClient.PublishAsync(applicationMessage.Build(), CancellationToken.None);
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
        public async Task Disconnect()
        {
            if (!mqttClient?.IsConnected ?? true)
                return;

            // Send a clean disconnect to the server by calling _DisconnectAsync_. Without this the TCP connection
            // gets dropped and the server will handle this as a non clean disconnect (see MQTT spec for details).
            var mqttClientDisconnectOptions = mqttFactory.CreateClientDisconnectOptionsBuilder();

            await mqttClient.DisconnectAsync(mqttClientDisconnectOptions.Build(), CancellationToken.None);

            Log.Instance.log($"MQTT: Disconnected from server.", LogSeverity.Debug);
        }
    }
}