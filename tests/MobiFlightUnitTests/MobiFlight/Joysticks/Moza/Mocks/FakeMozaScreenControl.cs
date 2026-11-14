using MobiFlightMoza;
using MobiFlightMoza.Protocol;
using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.Moza.Tests.Mocks
{
    /// <summary>Records what was submitted/connected, without a real serial port.</summary>
    internal class FakeMozaScreenControl : IMozaScreenControl
    {
        public event Action<byte> CabinPositionResolved;
        public event Action<string> ErrorMessageCreated;
        public event Action<string> TraceCreated;
        public event Action ScreenReady;

        public bool ConnectCalled { get; private set; }
        public bool StopCalled { get; private set; }
        public bool ShutdownCalled { get; private set; }
        public int ForceResendCallCount { get; private set; }
        public List<string> SubmittedPages { get; } = new();
        public List<(byte Index, bool On)> LedCalls { get; } = new();
        public List<(MozaLightGroup Group, int Level)> LightGroupBrightnessCalls { get; } = new();
        public List<(MozaLightGroup Group, MozaLightMode Mode)> LightGroupModeCalls { get; } = new();
        public List<int> DisplayBrightnessCalls { get; } = new();

        public bool Connect()
        {
            ConnectCalled = true;
            return true;
        }

        public void SubmitScreenData(string json) => SubmittedPages.Add(json);

        public void ForceResend() => ForceResendCallCount++;

        public void SetLed(byte index, bool on) => LedCalls.Add((index, on));

        public void SetLightGroupBrightness(MozaLightGroup group, int level) => LightGroupBrightnessCalls.Add((group, level));

        public void SetLightGroupMode(MozaLightGroup group, MozaLightMode mode) => LightGroupModeCalls.Add((group, mode));

        public void SetDisplayBrightness(int percent) => DisplayBrightnessCalls.Add(percent);

        public void Stop() => StopCalled = true;

        public void Shutdown() => ShutdownCalled = true;

        public void RaiseCabinPositionResolved(byte cabinPosition) => CabinPositionResolved?.Invoke(cabinPosition);

        public void RaiseErrorMessageCreated(string message) => ErrorMessageCreated?.Invoke(message);

        public void RaiseScreenReady() => ScreenReady?.Invoke();
    }
}
