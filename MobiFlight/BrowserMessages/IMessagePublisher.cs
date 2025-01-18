using System;

namespace MobiFlight.BrowserMessages
{
    public interface IMessagePublisher
    {
        void Publish<TEvent>(TEvent eventToPublish);
        void OnMessageReceived<TEvent>(Action<TEvent> action);
    }
}
