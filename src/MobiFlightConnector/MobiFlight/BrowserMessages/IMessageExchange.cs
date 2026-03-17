using System;

namespace MobiFlight.BrowserMessages
{
    public interface IMessageExchange
    {
        void Publish<TPayload>(TPayload message);
        void Subscribe<TPayload>(Action<TPayload> action);
    }
}
