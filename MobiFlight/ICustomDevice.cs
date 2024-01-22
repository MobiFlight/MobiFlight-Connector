using MobiFlight.CustomDevices;
using System.Collections.Generic;

namespace MobiFlight
{
    public interface ICustomDevice : IConnectedDevice
    {
        void Display(int MessageType, string value);
        List<MessageType> MessageTypes { get; }
    }
}