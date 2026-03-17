using System;

namespace MobiFlight.BrowserMessages
{
    public interface IMessagePublisher
    {
        void Publish<TEvent>(TEvent eventToPublish);
        void OnMessageReceived(Action<string> callback);
    }
}
