using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.BrowserMessages
{
    // Implement as singleton   
    public class MessageExchange : IMessageExchange
    {
        private readonly Dictionary<Type, List<object>> _subscribers = new Dictionary<Type, List<object>>();
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
            _messagePublisher.OnMessageReceived<object>(Publish);
        }

        public void Publish<TEvent>(TEvent eventToPublish)
        {
            _messagePublisher?.Publish(eventToPublish);
        }

        public void PublishReceivedMessage<TEvent>(TEvent eventToPublish)
        {
            var eventType = typeof(TEvent);
            List<object> subscribers;

            lock (_lock)
            {
                if (!_subscribers.ContainsKey(eventType)) return;
                subscribers = _subscribers[eventType].ToList();
            }

            foreach (var subscriber in subscribers)
            {
                ((Action<TEvent>)subscriber)(eventToPublish);
            }
        }

        public void Subscribe<TEvent>(Action<TEvent> action)
        {
            var eventType = typeof(TEvent);

            lock (_lock)
            {
                if (!_subscribers.ContainsKey(eventType))
                {
                    _subscribers[eventType] = new List<object>();
                }

                _subscribers[eventType].Add(action);
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> action)
        {
            var eventType = typeof(TEvent);

            lock (_lock)
            {
                if (_subscribers.ContainsKey(eventType))
                {
                    _subscribers[eventType].Remove(action);
                    if (_subscribers[eventType].Count == 0)
                    {
                        _subscribers.Remove(eventType);
                    }
                }
            }
        }
    }
}
