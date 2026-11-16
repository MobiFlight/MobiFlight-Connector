using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.BrowserMessages
{
    // Implement as singleton
    public class MessageExchange : IMessageExchange
    {
        /// <summary>A callback plus whether it must run on the captured UI thread.</summary>
        private class Subscription
        {
            public object Callback;
            public bool OnUiThread;
        }

        private readonly Dictionary<Type, List<Subscription>> _subscribers = new Dictionary<Type, List<Subscription>>();
        private readonly Dictionary<String, Type> _subscribedTypes = new Dictionary<string, Type>();
        private static readonly object _lock = new object();
        private static MessageExchange _instance;
        private IMessagePublisher _messagePublisher;

        /// <summary>UI thread context, set via SetSynchronizationContext; used only by SubscribeOnUiThread. Null forces inline dispatch (e.g. in tests).</summary>
        private System.Threading.SynchronizationContext _uiSynchronizationContext;

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

        public void ClearSubscriptions()
        {
            lock (_lock)
            {
                _subscribers.Clear();
                _subscribedTypes.Clear();
            }
        }

        public void SetPublisher(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
            _messagePublisher?.OnMessageReceived(PublishReceivedMessage);
        }

        public IMessagePublisher GetPublisher()
        {
            return _messagePublisher;
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

        /// <summary>Public so non-IMessagePublisher transports (e.g. WebViewMessageReceiver) can wire to it directly.</summary>
        public void PublishReceivedMessage(string jsonMessage)
        {
            var eventToPublish = JsonConvert.DeserializeObject<Message<object>>(jsonMessage);
            if (!_subscribedTypes.ContainsKey(eventToPublish.key))
            {
                Log.Instance.log("No subscribers for event: " + eventToPublish.key, LogSeverity.Warn);
                return;
            }

            Type eventType = _subscribedTypes[eventToPublish.key];

            List<Subscription> subscriptions;

            lock (_lock)
            {
                if (!_subscribers.ContainsKey(eventType)) return;
                subscriptions = _subscribers[eventType].ToList();
            }

            object deserializedPayload = null;
            try
            {
                var rawPayload = eventToPublish.payload?.ToString();
                if (rawPayload != null)
                    deserializedPayload = JsonConvert.DeserializeObject(rawPayload, eventType);
            }
            catch (Exception e)
            {
                Log.Instance.log($"Failed to deserialize payload for {eventToPublish.key}: {e.Message}", LogSeverity.Error);
                return;
            }

            foreach (var subscription in subscriptions)
            {
                // Per-subscriber error handling: one failing subscriber must not prevent delivery to the others
                Action invokeSubscriber = () =>
                {
                    try
                    {
                        subscription.Callback.GetType().GetMethod("Invoke")?.Invoke(subscription.Callback, new[] { deserializedPayload });
                    }
                    catch (Exception e)
                    {
                        var cause = (e as System.Reflection.TargetInvocationException)?.InnerException ?? e;
                        Log.Instance.log($"Subscriber for {eventToPublish.key} failed: {cause.Message}", LogSeverity.Error);
                    }
                };

                var isInvokedOnUiThread = subscription.OnUiThread && _uiSynchronizationContext != null;

                if (isInvokedOnUiThread)
                {
                    _uiSynchronizationContext.Post((_) => invokeSubscriber(), null);
                    continue;
                }
                invokeSubscriber();
            }
        }

        /// <summary>Runs the callback on whatever thread delivered the message - never marshaled.</summary>
        public void Subscribe<TMessagePayloadType>(Action<TMessagePayloadType> callback)
        {
            AddSubscription(callback, onUiThread: false);
        }

        /// <summary>Marshals the callback onto the captured UI thread. Use only for handlers touching WinForms state.</summary>
        public void SubscribeOnUiThread<TMessagePayloadType>(Action<TMessagePayloadType> callback)
        {
            AddSubscription(callback, onUiThread: true);
        }

        private void AddSubscription<TMessagePayloadType>(Action<TMessagePayloadType> callback, bool onUiThread)
        {
            var eventType = typeof(TMessagePayloadType);

            lock (_lock)
            {
                if (!_subscribers.ContainsKey(eventType))
                {
                    _subscribedTypes.Add(eventType.Name, eventType);
                    _subscribers[eventType] = new List<Subscription>();
                }

                _subscribers[eventType].Add(new Subscription { Callback = callback, OnUiThread = onUiThread });
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> callback)
        {
            var eventType = typeof(TEvent);

            lock (_lock)
            {
                if (_subscribers.ContainsKey(eventType))
                {
                    _subscribers[eventType].RemoveAll(s => Equals(s.Callback, callback));
                    if (_subscribers[eventType].Count == 0)
                    {
                        _subscribers.Remove(eventType);
                        _subscribedTypes.Remove(eventType.Name);
                    }
                }
            }
        }
        /// <summary>Call once from the UI thread during startup, before any publisher is set. Pass null to force inline dispatch (tests).</summary>
        public void SetSynchronizationContext(System.Threading.SynchronizationContext context)
        {
            _uiSynchronizationContext = context;
        }
    }
}