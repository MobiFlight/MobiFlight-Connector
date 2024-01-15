using MobiFlight.Config;
using System.Collections.Generic;
using System;

namespace MobiFlight
{
    public interface IHidDevice
    {
        string Name { get; }

        string Serial { get; }

        List<ListItem<IBaseDevice>> GetAvailableDevicesAsListItems();
        List<ListItem<IBaseDevice>> GetAvailableOutputDevicesAsListItems();
        string MapDeviceNameToLabel(string deviceName);
        void SetOutputDeviceState(string displayPin, byte state);
        void Shutdown();
        void Stop();
        void Update();
        void UpdateOutputDeviceStates();
    }
}