using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.BrowserMessages
{
    // Implement as singleton   
    public class MessageExchange : IMessageExchange
    {
        private readonly Dictionary<Type, List<object>> _subscribers = new Dictionary<Type, List<object>>();
        private readonly Dictionary<String, Type> _subscribedTypes = new Dictionary<string, Type>();
        private static readonly object _lock = new object();
        private static MessageExchange _instance;
        private IMessagePublisher _messagePublisher;

        public static MessageExchange Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new MessageExchange();
                        }
                    }
                }
                return _instance;
            }
        }

        private MessageExchange()
        {
        }

        public void SetPublisher(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
            _messagePublisher.OnMessageReceived(PublishReceivedMessage);
        }

        /// <summary>
        /// Publishes an event to the message publisher
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventToPublish"></param>
        public void Publish<TEvent>(TEvent eventToPublish)
        {
            _messagePublisher?.Publish(eventToPublish);
        }

        /// <summary>
        /// Publishes a received message to all subscribers
        /// </summary>
        private void PublishReceivedMessage(string jsonMessage)
        {
            var eventToPublish = JsonConvert.DeserializeObject<Message<object>>(jsonMessage);
            if (!_subscribedTypes.ContainsKey(eventToPublish.key))
            {
                Log.Instance.log("No subscribers for event: " + eventToPublish.key, LogSeverity.Warn);
                return;
            }

            Type eventType = _subscribedTypes[eventToPublish.key];

            List<object> subscribers;

            lock (_lock)
            {
                if (!_subscribers.ContainsKey(eventType)) return;
                subscribers = _subscribers[eventType].ToList();
            }

            try
            {

                var deserializedPayload = JsonConvert.DeserializeObject(eventToPublish.payload.ToString(), eventType);
                foreach (var subscriber in subscribers)
                {
                    subscriber.GetType().GetMethod("Invoke")?.Invoke(subscriber, new[] { deserializedPayload });
                }
            }
            catch (Exception e)
            {
                Log.Instance.log(e.Message, LogSeverity.Error);
            }
        }

        public void Subscribe<TMessagePayloadType>(Action<TMessagePayloadType> callback)
        {
            var eventType = typeof(TMessagePayloadType);

            lock (_lock)
            {
                if (!_subscribers.ContainsKey(eventType))
                {
                    _subscribedTypes.Add(eventType.Name, eventType);
                    _subscribers[eventType] = new List<object>();
                }

                _subscribers[eventType].Add(callback);
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> callback)
        {
            var eventType = typeof(TEvent);

            lock (_lock)
            {
                if (_subscribers.ContainsKey(eventType))
                {
                    _subscribers[eventType].Remove(callback);
                    if (_subscribers[eventType].Count == 0)
                    {
                        _subscribers.Remove(eventType);
                        _subscribedTypes.Remove(eventType.Name);
                    }
                }
            }
        }
    }
}
