using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.BrowserMessages
{
    // implement as singleton   
    public class MessageExchange : IMessageExchange
    {
        private readonly Dictionary<Type, List<object>> _subscribers = new Dictionary<Type, List<object>>();
        private static MessageExchange _instance;
        public static MessageExchange Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MessageExchange();
                }
                return _instance;
            }
        }

        private MessageExchange()
        {
        }

        public void Publish<TEvent>(TEvent eventToPublish)
        {
            var eventType = typeof(TEvent);
            if (_subscribers.ContainsKey(eventType))
            {
                var subscribers = _subscribers[eventType];
                foreach (var subscriber in subscribers.ToList())
                {
                    ((Action<TEvent>)subscriber)(eventToPublish);
                }
            }
        }

        public void Subscribe<TEvent>(Action<TEvent> action)
        {
            var eventType = typeof(TEvent);
            if (!_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType] = new List<object>();
            }

            _subscribers[eventType].Add(action);
        }
    }
}
