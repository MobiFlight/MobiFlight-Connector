using System;

namespace MobiFlight.BrowserMessages
{
    public interface IMessageExchange
    {
        void Publish<TPayload>(TPayload message);

        /// <summary>Runs the callback on whatever thread the transport delivered on.</summary>
        void Subscribe<TPayload>(Action<TPayload> action);

        /// <summary>Marshals the callback onto the captured UI thread before invoking it.</summary>
        void SubscribeOnUiThread<TPayload>(Action<TPayload> action);
    }
}
