using System;

namespace MobiFlight.BrowserMessages
{
    public interface IMessageExchange
    {
        void Publish<TEvent> (TEvent eventToPublish);
        void Subscribe<TEvent>(Action<TEvent> action);
    }
}
