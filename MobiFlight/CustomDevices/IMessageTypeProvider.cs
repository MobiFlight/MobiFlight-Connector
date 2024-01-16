using System.Collections.Generic;

namespace MobiFlight.CustomDevices
{
    public interface IMessageTypeProvider
    {
        List<MessageType> MessageTypes { get; }
    }
}