using System;

namespace MobiFlight.BrowserMessages
{
    public interface IMessagePublisher : IDisposable
    {
        void Publish<TEvent>(TEvent eventToPublish);
        void OnMessageReceived(Action<string> callback);
    }
}
