using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace MobiFlight
{
    /// <summary>
    /// Produces HOLD/REPEAT from a controller's raw PRESS/RELEASE events, and stamps RELEASE with how
    /// long the button was held. Owned by a controller manager (MobiFlightCache/JoystickManager/
    /// MidiBoardManager) and wired into its OnButtonPressed funnel, so downstream code sees virtual
    /// events exactly like real ones. See docs/architecture/virtual-button-events.md.
    /// </summary>
    public class SyntheticButtonEventGenerator : IDisposable
    {
        /// <summary>
        /// The distinct Hold/Repeat delay settings among configs currently bound to a button. Resolved
        /// once at PRESS, held for that press's lifecycle. Null or empty means no config is bound - no
        /// HOLD/REPEAT for that button at all. No config identity is carried - a config later decides
        /// a HOLD/REPEAT is its own by matching its own delay (see
        /// ButtonInputConfig.MatchesSyntheticDelay), so two configs sharing a delay are
        /// indistinguishable here by design and share one raised event.
        /// </summary>
        public Func<InputEventArgs, List<ButtonTimings>> ResolveTimings { get; set; }

        /// <summary>Raised for HOLD/REPEAT.</summary>
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
        /// Feed one real PRESS/RELEASE through. PRESS and RELEASE are each always forwarded exactly
        /// once, untouched except RELEASE also carries HeldDurationMs - LONG_RELEASE is not decided
        /// here at all; it's a per-config dispatch-time decision (see
        /// ButtonInputConfig.ResolveDispatchedEvent) based on that duration, so the raw "an event was
        /// raised" record for RELEASE can't disagree with itself depending on which configs happen to
        /// be bound. A button with no bound config passes PRESS/RELEASE through untouched and is never
        /// tracked for HOLD/REPEAT.
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
                    // No config bound to this button - no HOLD/REPEAT at all.
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

                var classified = e.CloneWithLabel();
                classified.HeldDurationMs = (int)(_now() - tracked.PressedAt).TotalMilliseconds;
                return new List<InputEventArgs> { classified };
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
                    var dueRepeatTimings = new HashSet<ButtonTimings>();

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
                                dueRepeatTimings.Add(binding.Timings);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Instance.log($"Error checking hold state for {kvp.Key}: {ex.Message}", LogSeverity.Error);
                        }
                    }

                    // One raised event per distinct HoldDelay (HOLD) or (HoldDelay, RepeatDelay) pair
                    // (REPEAT), not per binding - bindings sharing a key become due on the same tick
                    // (same PressedAt) and share one event. REPEAT keys on the pair, not RepeatDelay
                    // alone, so two bindings sharing a RepeatDelay but not a HoldDelay stay distinct -
                    // otherwise one could start repeating before the other's own HoldDelay elapsed.
                    foreach (var holdDelay in dueHoldDelays)
                        dueHold.Add(Classify(tracked.LastPress, MobiFlightButton.InputEvent.HOLD, holdDelay));

                    foreach (var timings in dueRepeatTimings)
                        dueRepeat.Add(ClassifyRepeat(tracked.LastPress, timings));
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

        private static InputEventArgs ClassifyRepeat(InputEventArgs lastPress, ButtonTimings timings)
        {
            var e = Classify(lastPress, MobiFlightButton.InputEvent.REPEAT, timings.RepeatDelay);
            e.SyntheticHoldDelayMs = timings.HoldDelay;
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
    /// Hold/Repeat delays for one button press, one config. Intentionally unclamped here - RepeatDelay
    /// validation belongs at config authoring time (UI), not runtime evaluation, so a config's own
    /// value and what the generator schedules/matches against always agree. See
    /// <see cref="SyntheticButtonEventGenerator.ResolveTimings"/>.
    /// </summary>
    public struct ButtonTimings
    {
        /// <summary>Not enforced here - reserved for config-authoring-time (UI) validation.</summary>
        public const int MinRepeatDelay = 100; // ms

        /// <summary>HoldDelay sentinel meaning "never fires HOLD/REPEAT" - use when no onHold is defined, so a config that doesn't want it can't keep it alive for the whole button.</summary>
        public const int NoHold = int.MaxValue;

        public int HoldDelay;
        public int RepeatDelay;

        public ButtonTimings(int holdDelay, int repeatDelay)
        {
            HoldDelay = holdDelay;
            RepeatDelay = repeatDelay;
        }

        public static int ClampRepeatDelay(int repeatDelay) =>
            repeatDelay > 0 && repeatDelay < MinRepeatDelay ? MinRepeatDelay : repeatDelay;
    }
}
