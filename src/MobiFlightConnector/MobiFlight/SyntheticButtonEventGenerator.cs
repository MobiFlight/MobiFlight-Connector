using System;
using System.Collections.Generic;
using System.Timers;

namespace MobiFlight
{
    /// <summary>
    /// Produces HOLD/REPEAT/LONG_RELEASE from a controller's raw PRESS/RELEASE events. Owned by a
    /// controller manager (MobiFlightCache/JoystickManager/MidiBoardManager) and wired into its
    /// OnButtonPressed funnel, so downstream code sees virtual events exactly like real ones. See
    /// docs/architecture/virtual-button-events.md.
    /// </summary>
    public class SyntheticButtonEventGenerator : IDisposable
    {
        /// <summary>Fallback timing when <see cref="ResolveTimings"/> is unset or has no match.</summary>
        public int HoldDelay { get; set; } = 350; // ms

        /// <summary>Fallback, see <see cref="HoldDelay"/>.</summary>
        public int LongReleaseDelay { get; set; } = 350; // ms

        /// <summary>Fallback, see <see cref="HoldDelay"/>. 0 disables repeating.</summary>
        public int RepeatDelay { get; set; } = 0; // ms

        /// <summary>
        /// Resolves every config currently bound to a button and its delays. Resolved once at
        /// PRESS, held for that press's lifecycle. Empty/null uses the fallback values above.
        /// </summary>
        public Func<InputEventArgs, List<ButtonBinding>> ResolveTimings { get; set; }

        /// <summary>Raised for HOLD/REPEAT. LONG_RELEASE is classified in-place by <see cref="Observe"/> instead.</summary>
        public event EventHandler<InputEventArgs> OnSyntheticEvent;

        private class BindingState
        {
            public string ConfigGuid;
            public ButtonTimings Timings;
            public bool HoldFired;
            public DateTime LastHoldFire;
        }

        private class TrackedButton
        {
            public InputEventArgs LastPress;
            public DateTime PressedAt;
            public List<BindingState> Bindings;
        }

        private readonly Dictionary<string, TrackedButton> _pressed = new Dictionary<string, TrackedButton>();
        private readonly object _lock = new object();
        private readonly Func<DateTime> _now;
        private readonly Timer _timer;

        /// <param name="now">Clock for timing decisions; tests inject a fake one.</param>
        /// <param name="tickIntervalMs">Poll interval while at least one button is pressed. Idle otherwise.</param>
        public SyntheticButtonEventGenerator(Func<DateTime> now = null, int tickIntervalMs = 20)
        {
            _now = now ?? (() => DateTime.Now);
            _timer = new Timer(tickIntervalMs) { AutoReset = true };
            _timer.Elapsed += (s, e) => Tick();
        }

        private static string KeyFor(InputEventArgs e) => $"{e.Controller?.Serial}:{e.Device?.Name}";

        /// <summary>
        /// Feed one real PRESS/RELEASE through. Returns the event(s) to forward: one untargeted
        /// event for PRESS; one per bound config for RELEASE, each classified RELEASE/LONG_RELEASE
        /// against that config's own delay.
        /// </summary>
        public List<InputEventArgs> Observe(InputEventArgs e)
        {
            if (e == null || e.InputType != DeviceType.Button) return new List<InputEventArgs> { e };

            MobiFlightButton.InputEvent value;
            try
            {
                value = (MobiFlightButton.InputEvent)Convert.ToInt32(e.Value);
            }
            catch (Exception)
            {
                return new List<InputEventArgs> { e };
            }

            var key = KeyFor(e);

            if (value == MobiFlightButton.InputEvent.PRESS)
            {
                // Resolved once here, held for the whole press - not re-resolved per tick.
                var bindings = ResolveTimings?.Invoke(e);
                if (bindings == null || bindings.Count == 0)
                {
                    bindings = new List<ButtonBinding> { new ButtonBinding(null, new ButtonTimings(HoldDelay, RepeatDelay, LongReleaseDelay)) };
                }

                lock (_lock)
                {
                    _pressed[key] = new TrackedButton
                    {
                        LastPress = e,
                        PressedAt = _now(),
                        Bindings = bindings.ConvertAll(b => new BindingState { ConfigGuid = b.ConfigGuid, Timings = b.Timings })
                    };
                }
                EnsureTimerRunning();

                return new List<InputEventArgs> { e };
            }

            if (value == MobiFlightButton.InputEvent.RELEASE)
            {
                TrackedButton tracked;
                lock (_lock)
                {
                    _pressed.TryGetValue(key, out tracked);
                    _pressed.Remove(key);
                }
                StopTimerIfIdle();

                if (tracked == null)
                {
                    // No matching PRESS was tracked - forward as-is.
                    return new List<InputEventArgs> { e };
                }

                var now = _now();
                var result = new List<InputEventArgs>(tracked.Bindings.Count);
                foreach (var binding in tracked.Bindings)
                {
                    var classified = (InputEventArgs)e.Clone();
                    classified.TargetConfigGUID = binding.ConfigGuid;
                    if ((now - tracked.PressedAt) > TimeSpan.FromMilliseconds(binding.Timings.LongReleaseDelay))
                    {
                        classified.Value = (int)MobiFlightButton.InputEvent.LONG_RELEASE;
                    }
                    result.Add(classified);
                }
                return result;
            }

            return new List<InputEventArgs> { e };
        }

        /// <summary>Fires due HOLD/REPEAT per binding. Called by the internal timer or directly by tests.</summary>
        public void Tick()
        {
            var dueHold = new List<InputEventArgs>();
            var dueRepeat = new List<InputEventArgs>();
            var now = _now();

            lock (_lock)
            {
                foreach (var kvp in _pressed)
                {
                    var tracked = kvp.Value;
                    foreach (var binding in tracked.Bindings)
                    {
                        try
                        {
                            if (!binding.HoldFired)
                            {
                                if ((now - tracked.PressedAt) >= TimeSpan.FromMilliseconds(binding.Timings.HoldDelay))
                                {
                                    binding.HoldFired = true;
                                    binding.LastHoldFire = now;
                                    dueHold.Add(Classify(tracked.LastPress, binding.ConfigGuid, MobiFlightButton.InputEvent.HOLD));
                                }
                            }
                            else if (binding.Timings.RepeatDelay > 0 && (now - binding.LastHoldFire) >= TimeSpan.FromMilliseconds(binding.Timings.RepeatDelay))
                            {
                                binding.LastHoldFire = now;
                                dueRepeat.Add(Classify(tracked.LastPress, binding.ConfigGuid, MobiFlightButton.InputEvent.REPEAT));
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Instance.log($"Error checking hold state for {kvp.Key} ({binding.ConfigGuid}): {ex.Message}", LogSeverity.Error);
                        }
                    }
                }
            }

            // Raised outside the lock - subscribers must never run while we hold it.
            foreach (var due in dueHold) RaiseSynthetic(due);
            foreach (var due in dueRepeat) RaiseSynthetic(due);
        }

        private static InputEventArgs Classify(InputEventArgs lastPress, string targetConfigGuid, MobiFlightButton.InputEvent value)
        {
            var e = (InputEventArgs)lastPress.Clone();
            e.Value = (int)value;
            e.TargetConfigGUID = targetConfigGuid;
            return e;
        }

        private void RaiseSynthetic(InputEventArgs e)
        {
            try
            {
                OnSyntheticEvent?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error raising synthetic button event: {ex.Message}", LogSeverity.Error);
            }
        }

        private void EnsureTimerRunning()
        {
            lock (_lock)
            {
                if (!_timer.Enabled) _timer.Start();
            }
        }

        private void StopTimerIfIdle()
        {
            lock (_lock)
            {
                if (_pressed.Count == 0 && _timer.Enabled) _timer.Stop();
            }
        }

        /// <summary>Stops the timer and drops all tracked state. No Start() - it self-starts on the next PRESS.</summary>
        public void Stop()
        {
            lock (_lock) // same lock as start/stop elsewhere, to avoid racing a concurrent PRESS
            {
                _pressed.Clear();
                _timer.Stop();
            }
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }

    /// <summary>Hold/Repeat/LongRelease delays for one button press, one config. See <see cref="SyntheticButtonEventGenerator.ResolveTimings"/>.</summary>
    public struct ButtonTimings
    {
        /// <summary>Floor for a positive RepeatDelay - protects the sim API from a too-fast repeat. 0 (disabled) is exempt.</summary>
        public const int MinRepeatDelay = 200; // ms

        public int HoldDelay;
        public int RepeatDelay;
        public int LongReleaseDelay;

        public ButtonTimings(int holdDelay, int repeatDelay, int longReleaseDelay)
        {
            HoldDelay = holdDelay;
            RepeatDelay = ClampRepeatDelay(repeatDelay);
            LongReleaseDelay = longReleaseDelay;
        }

        public static int ClampRepeatDelay(int repeatDelay) =>
            repeatDelay > 0 && repeatDelay < MinRepeatDelay ? MinRepeatDelay : repeatDelay;
    }

    /// <summary>One config's claim on a button: its GUID and requested delays.</summary>
    public struct ButtonBinding
    {
        public string ConfigGuid;
        public ButtonTimings Timings;

        public ButtonBinding(string configGuid, ButtonTimings timings)
        {
            ConfigGuid = configGuid;
            Timings = timings;
        }
    }
}
