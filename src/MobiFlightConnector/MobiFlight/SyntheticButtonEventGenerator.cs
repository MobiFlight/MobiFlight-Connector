using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary>
        /// The distinct Hold/Repeat/LongRelease delay settings among configs currently bound to a
        /// button. Resolved once at PRESS, held for that press's lifecycle. Null or empty means no
        /// config is bound - no synthetic events for that button at all. No config identity is
        /// carried - a config later decides an event is its own by matching its own delay (see
        /// ButtonInputConfig.MatchesSyntheticDelay), so two configs sharing a delay are
        /// indistinguishable here by design and share one raised event.
        /// </summary>
        public Func<InputEventArgs, List<ButtonTimings>> ResolveTimings { get; set; }

        /// <summary>Raised for HOLD/REPEAT. LONG_RELEASE is classified in-place by <see cref="Observe"/> instead.</summary>
        public event EventHandler<InputEventArgs> OnSyntheticEvent;

        private class BindingState
        {
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
        /// <remarks>Wire <see cref="ResolveTimings"/> before feeding events through - without it, nothing is ever bound and no synthetic events fire.</remarks>
        public SyntheticButtonEventGenerator(Func<DateTime> now = null, int tickIntervalMs = 20)
        {
            _now = now ?? (() => DateTime.Now);
            _timer = new Timer(tickIntervalMs) { AutoReset = true };
            _timer.Elapsed += (s, e) => Tick();
        }

        private static string KeyFor(InputEventArgs e) => $"{e.Controller?.Serial}:{e.Device?.Name}";

        /// <summary>
        /// Feed one real PRESS/RELEASE through. Returns the event(s) to forward: one untargeted
        /// event for PRESS; for RELEASE, one event per distinct LongReleaseDelay among bound configs
        /// (configs sharing a delay always agree on RELEASE vs LONG_RELEASE, so they share one event).
        /// A button with no bound config passes PRESS/RELEASE through untouched and is never tracked
        /// for HOLD/REPEAT/LONG_RELEASE.
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
                var timings = ResolveTimings?.Invoke(e);
                if (timings == null || timings.Count == 0)
                {
                    // No config bound to this button - no synthetic events at all.
                    return new List<InputEventArgs> { e };
                }

                lock (_lock)
                {
                    _pressed[key] = new TrackedButton
                    {
                        LastPress = e,
                        PressedAt = _now(),
                        Bindings = timings.ConvertAll(t => new BindingState { Timings = t })
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
                var result = new List<InputEventArgs>();
                // Grouped by LongReleaseDelay - configs sharing a delay always agree on RELEASE vs
                // LONG_RELEASE (same elapsed time, same threshold), so they share one raised event.
                foreach (var longReleaseDelay in tracked.Bindings.Select(b => b.Timings.LongReleaseDelay).Distinct())
                {
                    var classified = e.CloneWithLabel();
                    if ((now - tracked.PressedAt) > TimeSpan.FromMilliseconds(longReleaseDelay))
                    {
                        classified.Value = (int)MobiFlightButton.InputEvent.LONG_RELEASE;
                        classified.SyntheticDelayMs = longReleaseDelay;
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
                    var dueHoldDelays = new HashSet<int>();
                    var dueRepeatDelays = new HashSet<int>();

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
                                    dueHoldDelays.Add(binding.Timings.HoldDelay);
                                }
                            }
                            else if (binding.Timings.RepeatDelay > 0 && (now - binding.LastHoldFire) >= TimeSpan.FromMilliseconds(binding.Timings.RepeatDelay))
                            {
                                binding.LastHoldFire = now;
                                dueRepeatDelays.Add(binding.Timings.RepeatDelay);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Instance.log($"Error checking hold state for {kvp.Key}: {ex.Message}", LogSeverity.Error);
                        }
                    }

                    // One raised event per distinct delay value, not per binding - bindings sharing a
                    // delay become due on the same tick (same PressedAt) and share one event.
                    foreach (var holdDelay in dueHoldDelays)
                        dueHold.Add(Classify(tracked.LastPress, MobiFlightButton.InputEvent.HOLD, holdDelay));

                    foreach (var repeatDelay in dueRepeatDelays)
                        dueRepeat.Add(Classify(tracked.LastPress, MobiFlightButton.InputEvent.REPEAT, repeatDelay));
                }
            }

            // Raised outside the lock - subscribers must never run while we hold it.
            foreach (var due in dueHold) RaiseSynthetic(due);
            foreach (var due in dueRepeat) RaiseSynthetic(due);
        }

        private static InputEventArgs Classify(InputEventArgs lastPress, MobiFlightButton.InputEvent value, int delayMs)
        {
            var e = lastPress.CloneWithLabel();
            e.Value = (int)value;
            e.SyntheticDelayMs = delayMs;
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

    /// <summary>
    /// Hold/Repeat/LongRelease delays for one button press, one config. Intentionally unclamped here -
    /// RepeatDelay validation belongs at config authoring time (UI), not runtime evaluation, so a
    /// config's own value and what the generator schedules/matches against always agree. See
    /// <see cref="SyntheticButtonEventGenerator.ResolveTimings"/>.
    /// </summary>
    public struct ButtonTimings
    {
        /// <summary>Not enforced here - reserved for config-authoring-time (UI) validation.</summary>
        public const int MinRepeatDelay = 100; // ms

        /// <summary>HoldDelay sentinel meaning "never fires HOLD/REPEAT" - use when no onHold is defined, so a config that doesn't want it can't keep it alive for the whole button.</summary>
        public const int NoHold = int.MaxValue;

        /// <summary>LongReleaseDelay sentinel meaning "never reclassifies as LONG_RELEASE" - use when no onLongRelease is defined, since onRelease dispatches identically either way (see ButtonInputConfig.ResolveDispatchedEvent).</summary>
        public const int NoLongRelease = int.MaxValue;

        public int HoldDelay;
        public int RepeatDelay;
        public int LongReleaseDelay;

        public ButtonTimings(int holdDelay, int repeatDelay, int longReleaseDelay)
        {
            HoldDelay = holdDelay;
            RepeatDelay = repeatDelay;
            LongReleaseDelay = longReleaseDelay;
        }

        public static int ClampRepeatDelay(int repeatDelay) =>
            repeatDelay > 0 && repeatDelay < MinRepeatDelay ? MinRepeatDelay : repeatDelay;
    }
}
