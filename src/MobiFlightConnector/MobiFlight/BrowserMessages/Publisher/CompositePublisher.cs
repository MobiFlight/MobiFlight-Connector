using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.BrowserMessages.Publisher
{
    public class CompositePublisher : IMessagePublisher
    {
        private readonly ConcurrentDictionary<string, IMessagePublisher> publishers = new ConcurrentDictionary<string, IMessagePublisher>();
        private readonly List<string> pausedPublishers = new List<string>();

        public void AddPublisher(string name, IMessagePublisher publisher)
        {
            publishers.AddOrUpdate(name, publisher, (key, existing) => publisher);
        }

        public IMessagePublisher RemovePublisher(string name)
        {
            publishers.TryRemove(name, out var removed);
            return removed;
        }

        public void Publish<TEvent>(TEvent eventToPublish)
        {
            publishers.Values.ToList().ForEach(publisher =>
            {
                try
                {
                    publisher?.Publish(eventToPublish);
                }
                catch (Exception ex)
                {
                    // Log but don't fail entire publish chain
                    Log.Instance.log($"Publish failed: {ex.Message}", LogSeverity.Warn);
                }
            });
        }

        public void OnMessageReceived(Action<string> callback)
        {
            var activePublishers = publishers
                                    .Where(p => !pausedPublishers.Contains(p.Key))
                                    .Select(p => p.Value)
                                    .ToList();

            activePublishers.ForEach(publisher =>
            {
                try
                {
                    publisher?.OnMessageReceived(callback);
                }
                catch (Exception ex)
                {
                    // Log but don't fail entire publish chain
                    Log.Instance.log($"OnMessageReceived failed: {ex.Message}", LogSeverity.Warn);
                }
            });
        }

        internal void PausePublisher(string publisher)
        {
            if (pausedPublishers.Contains(publisher)) return;

            pausedPublishers.Add(publisher);
        }

        internal void ResumePublisher(string publisher)
        {
            pausedPublishers.Remove(publisher);
        }
    }
}