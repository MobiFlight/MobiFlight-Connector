using MobiFlight.CustomDevices;
using System.Collections.Generic;

namespace MobiFlight
{
    public interface ICustomDevice : IConnectedDevice, IMessageTypeProvider
    {
        void Display(int MessageType, string value);
        List<MessageType> MessageType { get; }
    }
}