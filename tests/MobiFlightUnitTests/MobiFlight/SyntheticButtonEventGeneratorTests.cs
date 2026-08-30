using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class SyntheticButtonEventGeneratorTests
    {
        // huge so the internal timer never fires during a test - time is driven via _now/Tick()
        private const int NeverFiresIntervalMs = 60000;

        private DateTime _now;
        private SyntheticButtonEventGenerator _generator;
        private List<InputEventArgs> _synthetic;

        [TestInitialize]
        public void Setup()
        {
            _now = new DateTime(2026, 1, 1, 12, 0, 0);
            _generator = new SyntheticButtonEventGenerator(() => _now, NeverFiresIntervalMs)
            {
                ResolveTimings = e => new List<ButtonTimings> { new ButtonTimings(holdDelay: 300, repeatDelay: 0, longReleaseDelay: 300) }
            };
            _synthetic = new List<InputEventArgs>();
            _generator.OnSyntheticEvent += (s, e) => _synthetic.Add(e);
        }

        [TestCleanup]
        public void Teardown()
        {
            _generator.Dispose();
        }

        private static InputEventArgs ButtonEvent(string serial, string device, MobiFlightButton.InputEvent value, string label = null)
        {
            return new InputEventArgs
            {
                Controller = new Controller { Serial = serial },
                Device = new DeviceReference { Name = device, Label = label },
                InputType = DeviceType.Button,
                Value = (int)value
            };
        }

        /// <summary>Convenience for tests where exactly one config (or none) is expected to match.</summary>
        private InputEventArgs ObserveOne(InputEventArgs e) => _generator.Observe(e).Single();

        [TestMethod]
        public void Tick_BeforeHoldDelayElapsed_DoesNotFireHold()
        {
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(100); // < HoldDelay (300)
            _generator.Tick();

            Assert.HasCount(0, _synthetic);
        }

        [TestMethod]
        public void Tick_AfterHoldDelayElapsed_FiresHoldOnce()
        {
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(300);
            _generator.Tick();

            Assert.HasCount(1, _synthetic);
            Assert.AreEqual((double)MobiFlightButton.InputEvent.HOLD, _synthetic[0].Value);
            Assert.AreEqual("S1", _synthetic[0].Controller.Serial);
            Assert.AreEqual("Btn1", _synthetic[0].Device.Name);
            Assert.AreEqual(300, _synthetic[0].SyntheticDelayMs, "Carries the HoldDelay that fired it, for display and for a config to match itself against.");
        }

        [TestMethod]
        public void Tick_CalledRepeatedlyWithRepeatDisabled_FiresHoldOnlyOnce()
        {
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(300);
            _generator.Tick();
            _generator.Tick();
            _now = _now.AddMilliseconds(1000);
            _generator.Tick();

            Assert.HasCount(1, _synthetic, "RepeatDelay is 0 (disabled), HOLD should not repeat.");
        }

        [TestMethod]
        public void Tick_WithRepeatDelayElapsed_FiresRepeatAfterInitialHold()
        {
            _generator.ResolveTimings = e => new List<ButtonTimings> { new ButtonTimings(holdDelay: 300, repeatDelay: 100, longReleaseDelay: 300) };
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(300);
            _generator.Tick(); // first HOLD

            _now = _now.AddMilliseconds(100);
            _generator.Tick(); // repeat

            Assert.HasCount(2, _synthetic);
            Assert.AreEqual((double)MobiFlightButton.InputEvent.HOLD, _synthetic[0].Value, "The first firing must be HOLD.");
            Assert.AreEqual((double)MobiFlightButton.InputEvent.REPEAT, _synthetic[1].Value, "Every firing after the first must be REPEAT, not HOLD again.");
        }

        [TestMethod]
        public void Tick_WithRepeatDelayElapsedTwice_FiresRepeatEachTime()
        {
            _generator.ResolveTimings = e => new List<ButtonTimings> { new ButtonTimings(holdDelay: 300, repeatDelay: 100, longReleaseDelay: 300) };
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(300);
            _generator.Tick(); // HOLD

            _now = _now.AddMilliseconds(100);
            _generator.Tick(); // REPEAT #1

            _now = _now.AddMilliseconds(100);
            _generator.Tick(); // REPEAT #2

            Assert.HasCount(3, _synthetic);
            Assert.AreEqual((double)MobiFlightButton.InputEvent.HOLD, _synthetic[0].Value);
            Assert.AreEqual((double)MobiFlightButton.InputEvent.REPEAT, _synthetic[1].Value);
            Assert.AreEqual((double)MobiFlightButton.InputEvent.REPEAT, _synthetic[2].Value);
        }

        [TestMethod]
        public void Observe_ReleaseBeforeLongReleaseDelay_StaysRelease()
        {
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(100); // < LongReleaseDelay (300)
            var result = ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.RELEASE));

            Assert.AreEqual((double)MobiFlightButton.InputEvent.RELEASE, result.Value);
            Assert.IsNull(result.SyntheticDelayMs, "Plain RELEASE has no delay of its own to show.");
        }

        [TestMethod]
        public void Observe_ReleaseAfterLongReleaseDelay_IsReclassifiedAsLongRelease()
        {
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(350); // > LongReleaseDelay (300)
            var result = ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.RELEASE));

            Assert.AreEqual((double)MobiFlightButton.InputEvent.LONG_RELEASE, result.Value);
            Assert.AreEqual(300, result.SyntheticDelayMs);
        }

        [TestMethod]
        public void Observe_Release_StopsTrackingSoHoldNeverFires()
        {
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(100);
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.RELEASE));

            _now = _now.AddMilliseconds(1000); // well past HoldDelay
            _generator.Tick();

            Assert.HasCount(0, _synthetic, "A released button must never fire HOLD.");
        }

        [TestMethod]
        public void Observe_TwoButtonsAreTrackedIndependently()
        {
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(200);
            ObserveOne(ButtonEvent("S1", "Btn2", MobiFlightButton.InputEvent.PRESS)); // pressed 200ms later

            _now = _now.AddMilliseconds(150); // Btn1: 350ms since press (fires), Btn2: 150ms (does not)
            _generator.Tick();

            Assert.HasCount(1, _synthetic);
            Assert.AreEqual("Btn1", _synthetic[0].Device.Name);
        }

        [TestMethod]
        public void Observe_NonButtonEvent_IsIgnored()
        {
            var encoderEvent = new InputEventArgs
            {
                Controller = new Controller { Serial = "S1" },
                Device = new DeviceReference { Name = "Enc1" },
                InputType = DeviceType.Encoder,
                Value = (int)MobiFlightEncoder.InputEvent.LEFT
            };

            var result = ObserveOne(encoderEvent);

            Assert.AreSame(encoderEvent, result);
            Assert.AreEqual((double)MobiFlightEncoder.InputEvent.LEFT, result.Value, "Non-button events must pass through untouched.");

            _now = _now.AddMilliseconds(1000);
            _generator.Tick();
            Assert.HasCount(0, _synthetic, "A non-button event must never be tracked for HOLD.");
        }

        [TestMethod]
        public void Stop_StopsTrackingEveryPressedButton()
        {
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _generator.Stop();

            _now = _now.AddMilliseconds(1000);
            _generator.Tick();

            Assert.HasCount(0, _synthetic, "Stop() must stop tracking so no HOLD fires afterwards.");
        }

        // ResolveTimings: delay is per config, not per physical button.

        [TestMethod]
        public void ResolveTimings_OverridesDefaultHoldDelayForThatPress()
        {
            _generator.ResolveTimings = e => new List<ButtonTimings> { new ButtonTimings(50, 0, 300) };
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(50); // well under the generator's own 300ms default
            _generator.Tick();

            Assert.HasCount(1, _synthetic, "The resolved 50ms HoldDelay should govern, not the generator's 300ms default.");
            Assert.AreEqual(50, _synthetic[0].SyntheticDelayMs);
        }

        [TestMethod]
        public void ResolveTimings_ReturningEmpty_ProducesNoSyntheticEvents()
        {
            // Resolver ran and found no config bound to this button - never track it, never fire
            // HOLD/REPEAT/LONG_RELEASE for it, no matter how long it's held.
            _generator.ResolveTimings = e => new List<ButtonTimings>();
            var pressResult = ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));
            Assert.AreEqual((double)MobiFlightButton.InputEvent.PRESS, pressResult.Value, "PRESS must still pass through untouched.");

            _now = _now.AddMilliseconds(1000); // well past any default HoldDelay
            _generator.Tick();
            Assert.HasCount(0, _synthetic, "No bound config means no HOLD, ever.");

            var releaseResult = ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.RELEASE));
            Assert.AreEqual((double)MobiFlightButton.InputEvent.RELEASE, releaseResult.Value, "Never reclassified as LONG_RELEASE - it was never tracked.");
        }

        [TestMethod]
        public void ResolveTimings_IsResolvedOnceAtPress_NotReResolvedEveryTick()
        {
            int callCount = 0;
            _generator.ResolveTimings = e =>
            {
                callCount++;
                return new List<ButtonTimings> { new ButtonTimings(300, 0, 300) };
            };

            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));
            Assert.AreEqual(1, callCount, "Resolving should happen exactly once, at PRESS.");

            _now = _now.AddMilliseconds(100);
            _generator.Tick();
            _now = _now.AddMilliseconds(100);
            _generator.Tick();
            _now = _now.AddMilliseconds(300);
            _generator.Tick();

            Assert.AreEqual(1, callCount, "Tick() must not re-resolve timings for an already-tracked press.");
        }

        [TestMethod]
        public void ResolveTimings_DifferentButtonsCanHaveDifferentDelays()
        {
            _generator.ResolveTimings = e => new List<ButtonTimings>
            {
                e.Device.Name == "Btn1"
                    ? new ButtonTimings(50, 0, 300)
                    : new ButtonTimings(500, 0, 300)
            };

            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));
            ObserveOne(ButtonEvent("S1", "Btn2", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(50);
            _generator.Tick();

            Assert.HasCount(1, _synthetic, "Only Btn1's shorter, resolved HoldDelay should have elapsed.");
            Assert.AreEqual("Btn1", _synthetic[0].Device.Name);
        }

        // Two configs sharing a button, different delays - each still gets its own event.

        [TestMethod]
        public void ResolveTimings_TwoConfigsWithDifferentHoldDelay_OnlyTheOneWhoseDelayElapsedFires()
        {
            _generator.ResolveTimings = e => new List<ButtonTimings>
            {
                new ButtonTimings(holdDelay: 50, repeatDelay: 0, longReleaseDelay: 300),
                new ButtonTimings(holdDelay: 900, repeatDelay: 0, longReleaseDelay: 300)
            };

            _generator.Observe(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(50);
            _generator.Tick();

            Assert.HasCount(1, _synthetic, "The 900ms delay has not elapsed - it must not fire yet.");
            Assert.AreEqual(50, _synthetic[0].SyntheticDelayMs);
            Assert.AreEqual((double)MobiFlightButton.InputEvent.HOLD, _synthetic[0].Value);

            _now = _now.AddMilliseconds(850); // total 900ms
            _generator.Tick();

            Assert.HasCount(2, _synthetic, "The 900ms delay has now elapsed too.");
            Assert.AreEqual(900, _synthetic[1].SyntheticDelayMs);
        }

        [TestMethod]
        public void ResolveTimings_TwoConfigsWithDifferentLongReleaseDelay_ReleaseFansOutOneEventPerDelay()
        {
            _generator.ResolveTimings = e => new List<ButtonTimings>
            {
                new ButtonTimings(holdDelay: 300, repeatDelay: 0, longReleaseDelay: 200),
                new ButtonTimings(holdDelay: 300, repeatDelay: 0, longReleaseDelay: 800)
            };

            _generator.Observe(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(300); // > 200ms, < 800ms
            var released = _generator.Observe(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.RELEASE));

            Assert.HasCount(2, released, "One release event must be produced per distinct LongReleaseDelay.");

            var longRelease = released.Single(e => e.Value == (double)MobiFlightButton.InputEvent.LONG_RELEASE);
            var plainRelease = released.Single(e => e.Value == (double)MobiFlightButton.InputEvent.RELEASE);

            Assert.AreEqual(200, longRelease.SyntheticDelayMs, "300ms exceeds the 200ms threshold - that delay's group must see LONG_RELEASE.");
            Assert.IsNull(plainRelease.SyntheticDelayMs, "300ms is under the 800ms threshold - that delay's group must see a plain RELEASE.");
        }

        // Two configs sharing the SAME delay - the actual grouping guarantee: one physical HOLD/
        // REPEAT/LONG_RELEASE must not be logged/raised once per config bound to the button.

        [TestMethod]
        public void Tick_TwoConfigsWithSameHoldDelay_FireOneGroupedHoldEvent()
        {
            _generator.ResolveTimings = e => new List<ButtonTimings>
            {
                new ButtonTimings(holdDelay: 300, repeatDelay: 0, longReleaseDelay: 200),
                new ButtonTimings(holdDelay: 300, repeatDelay: 0, longReleaseDelay: 800) // differs only in LongReleaseDelay
            };
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(300);
            _generator.Tick();

            Assert.HasCount(1, _synthetic, "Both configs share HoldDelay=300 - one HOLD, not two.");
            Assert.AreEqual(300, _synthetic[0].SyntheticDelayMs);
        }

        [TestMethod]
        public void Tick_TwoConfigsWithSameRepeatDelay_FireOneGroupedRepeatEvent()
        {
            _generator.ResolveTimings = e => new List<ButtonTimings>
            {
                new ButtonTimings(holdDelay: 300, repeatDelay: 100, longReleaseDelay: 200),
                new ButtonTimings(holdDelay: 300, repeatDelay: 100, longReleaseDelay: 800) // differs only in LongReleaseDelay
            };
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(300);
            _generator.Tick(); // HOLD (grouped - one event)

            _now = _now.AddMilliseconds(100);
            _generator.Tick(); // REPEAT

            Assert.HasCount(2, _synthetic, "Both configs share RepeatDelay=100 - one REPEAT, not two.");
            Assert.AreEqual((double)MobiFlightButton.InputEvent.REPEAT, _synthetic[1].Value);
            Assert.AreEqual(100, _synthetic[1].SyntheticDelayMs);
        }

        [TestMethod]
        public void Observe_TwoConfigsWithSameLongReleaseDelay_FireOneGroupedReleaseEvent()
        {
            _generator.ResolveTimings = e => new List<ButtonTimings>
            {
                new ButtonTimings(holdDelay: 50, repeatDelay: 0, longReleaseDelay: 300), // differs only in HoldDelay
                new ButtonTimings(holdDelay: 900, repeatDelay: 0, longReleaseDelay: 300)
            };
            _generator.Observe(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(350); // > 300ms
            var released = _generator.Observe(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.RELEASE));

            Assert.HasCount(1, released, "Both configs share LongReleaseDelay=300 - one LONG_RELEASE, not two.");
            Assert.AreEqual((double)MobiFlightButton.InputEvent.LONG_RELEASE, released[0].Value);
            Assert.AreEqual(300, released[0].SyntheticDelayMs);
        }

        // RepeatDelay is used exactly as configured - no runtime floor. Enforcing a minimum is a
        // config-authoring-time (UI) concern, not something evaluated on every tick.

        [TestMethod]
        public void ButtonTimingsConstructor_DoesNotClampRepeatDelay()
        {
            var timings = new ButtonTimings(holdDelay: 300, repeatDelay: 10, longReleaseDelay: 300);

            Assert.AreEqual(10, timings.RepeatDelay, "Clamping belongs at config-save time (UI), not runtime evaluation.");
        }

        [TestMethod]
        [DataRow(0, 0, DisplayName = "0 (disabled) is exempt from the floor")]
        [DataRow(1, ButtonTimings.MinRepeatDelay, DisplayName = "a tiny positive value is raised to the floor")]
        [DataRow(ButtonTimings.MinRepeatDelay - 1, ButtonTimings.MinRepeatDelay, DisplayName = "just under the floor is raised to it")]
        [DataRow(ButtonTimings.MinRepeatDelay, ButtonTimings.MinRepeatDelay, DisplayName = "exactly the floor is unchanged")]
        [DataRow(900, 900, DisplayName = "comfortably above the floor is unchanged")]
        public void ClampRepeatDelay_EnforcesTheFloor(int configured, int expected)
        {
            // The utility itself still exists, ready for config-authoring-time (UI) validation -
            // just not called automatically anywhere in this runtime evaluation path anymore.
            Assert.AreEqual(expected, ButtonTimings.ClampRepeatDelay(configured));
        }

        [TestMethod]
        public void Tick_RepeatDelayBelowTraditionalFloor_FiresAtTheConfiguredCadenceAsIs()
        {
            _generator.ResolveTimings = e => new List<ButtonTimings>
            {
                new ButtonTimings(holdDelay: 300, repeatDelay: 10, longReleaseDelay: 300)
            };
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(300);
            _generator.Tick(); // HOLD

            _now = _now.AddMilliseconds(10); // the configured 10ms, honored as-is
            _generator.Tick();

            Assert.HasCount(2, _synthetic, "10ms has elapsed since HOLD - REPEAT fires at the configured cadence, unclamped.");
            Assert.AreEqual((double)MobiFlightButton.InputEvent.REPEAT, _synthetic[1].Value);
            Assert.AreEqual(10, _synthetic[1].SyntheticDelayMs);
        }

        // Device.Label - the value the log line displays. All of these are built by cloning
        // internally, and DeviceReference.Clone() drops Label - CloneWithLabel() is what's supposed
        // to prevent it going blank here. This is the exact regression that shipped unnoticed until
        // manual testing caught it: PRESS kept its label because it's never cloned, everything else
        // silently lost it.

        [TestMethod]
        public void Tick_Hold_PreservesDeviceLabelFromPress()
        {
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS, "Button 1"));

            _now = _now.AddMilliseconds(300);
            _generator.Tick();

            Assert.AreEqual("Button 1", _synthetic[0].Device.Label);
        }

        [TestMethod]
        public void Tick_Repeat_PreservesDeviceLabelFromPress()
        {
            _generator.ResolveTimings = e => new List<ButtonTimings> { new ButtonTimings(holdDelay: 300, repeatDelay: 100, longReleaseDelay: 300) };
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS, "Button 1"));

            _now = _now.AddMilliseconds(300);
            _generator.Tick(); // HOLD

            _now = _now.AddMilliseconds(100);
            _generator.Tick(); // REPEAT

            Assert.AreEqual("Button 1", _synthetic[1].Device.Label);
        }

        [TestMethod]
        public void Observe_Release_PreservesDeviceLabel()
        {
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS, "Button 1"));

            var released = ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.RELEASE, "Button 1"));

            Assert.AreEqual("Button 1", released.Device.Label);
        }

        [TestMethod]
        public void Observe_LongRelease_PreservesDeviceLabel()
        {
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS, "Button 1"));

            _now = _now.AddMilliseconds(350); // > LongReleaseDelay (300)
            var result = ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.RELEASE, "Button 1"));

            Assert.AreEqual((double)MobiFlightButton.InputEvent.LONG_RELEASE, result.Value);
            Assert.AreEqual("Button 1", result.Device.Label);
        }
    }
}
