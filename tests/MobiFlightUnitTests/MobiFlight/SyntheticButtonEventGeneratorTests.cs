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
                HoldDelay = 300,
                LongReleaseDelay = 300,
                RepeatDelay = 0
            };
            _synthetic = new List<InputEventArgs>();
            _generator.OnSyntheticEvent += (s, e) => _synthetic.Add(e);
        }

        [TestCleanup]
        public void Teardown()
        {
            _generator.Dispose();
        }

        private static InputEventArgs ButtonEvent(string serial, string device, MobiFlightButton.InputEvent value)
        {
            return new InputEventArgs
            {
                Controller = new Controller { Serial = serial },
                Device = new DeviceReference { Name = device },
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
            Assert.IsNull(_synthetic[0].TargetConfigGUID, "No resolver wired, so this is the untargeted fallback - broadcasts like a real event.");
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
            _generator.RepeatDelay = ButtonTimings.MinRepeatDelay;
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(300);
            _generator.Tick(); // first HOLD

            _now = _now.AddMilliseconds(ButtonTimings.MinRepeatDelay);
            _generator.Tick(); // repeat

            Assert.HasCount(2, _synthetic);
            Assert.AreEqual((double)MobiFlightButton.InputEvent.HOLD, _synthetic[0].Value, "The first firing must be HOLD.");
            Assert.AreEqual((double)MobiFlightButton.InputEvent.REPEAT, _synthetic[1].Value, "Every firing after the first must be REPEAT, not HOLD again.");
        }

        [TestMethod]
        public void Tick_WithRepeatDelayElapsedTwice_FiresRepeatEachTime()
        {
            _generator.RepeatDelay = ButtonTimings.MinRepeatDelay;
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(300);
            _generator.Tick(); // HOLD

            _now = _now.AddMilliseconds(ButtonTimings.MinRepeatDelay);
            _generator.Tick(); // REPEAT #1

            _now = _now.AddMilliseconds(ButtonTimings.MinRepeatDelay);
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
        }

        [TestMethod]
        public void Observe_ReleaseAfterLongReleaseDelay_IsReclassifiedAsLongRelease()
        {
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(350); // > LongReleaseDelay (300)
            var result = ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.RELEASE));

            Assert.AreEqual((double)MobiFlightButton.InputEvent.LONG_RELEASE, result.Value);
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
            _generator.ResolveTimings = e => new List<ButtonBinding> { new ButtonBinding("cfgA", new ButtonTimings(50, 0, 300)) };
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(50); // well under the generator's own 300ms default
            _generator.Tick();

            Assert.HasCount(1, _synthetic, "The resolved 50ms HoldDelay should govern, not the generator's 300ms default.");
            Assert.AreEqual("cfgA", _synthetic[0].TargetConfigGUID);
        }

        [TestMethod]
        public void ResolveTimings_ReturningEmpty_FallsBackToGeneratorDefaults()
        {
            _generator.ResolveTimings = e => new List<ButtonBinding>(); // no active/precondition-satisfied config for this button
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(100); // < the generator's 300ms default HoldDelay
            _generator.Tick();
            Assert.HasCount(0, _synthetic);

            _now = _now.AddMilliseconds(250); // now past 300ms total
            _generator.Tick();
            Assert.HasCount(1, _synthetic);
            Assert.IsNull(_synthetic[0].TargetConfigGUID);
        }

        [TestMethod]
        public void ResolveTimings_IsResolvedOnceAtPress_NotReResolvedEveryTick()
        {
            int callCount = 0;
            _generator.ResolveTimings = e =>
            {
                callCount++;
                return new List<ButtonBinding> { new ButtonBinding("cfgA", new ButtonTimings(300, 0, 300)) };
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
            _generator.ResolveTimings = e => new List<ButtonBinding>
            {
                e.Device.Name == "Btn1"
                    ? new ButtonBinding("cfgBtn1", new ButtonTimings(50, 0, 300))
                    : new ButtonBinding("cfgBtn2", new ButtonTimings(500, 0, 300))
            };

            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));
            ObserveOne(ButtonEvent("S1", "Btn2", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(50);
            _generator.Tick();

            Assert.HasCount(1, _synthetic, "Only Btn1's shorter, resolved HoldDelay should have elapsed.");
            Assert.AreEqual("Btn1", _synthetic[0].Device.Name);
        }

        // Two configs on the same button - the case that needs targeting, not just resolving.

        [TestMethod]
        public void ResolveTimings_TwoConfigsOnSameButton_OnlyTheOneWhoseDelayElapsedFires()
        {
            _generator.ResolveTimings = e => new List<ButtonBinding>
            {
                new ButtonBinding("fastConfig", new ButtonTimings(holdDelay: 50, repeatDelay: 0, longReleaseDelay: 300)),
                new ButtonBinding("slowConfig", new ButtonTimings(holdDelay: 900, repeatDelay: 0, longReleaseDelay: 300))
            };

            _generator.Observe(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(50);
            _generator.Tick();

            Assert.HasCount(1, _synthetic, "slowConfig's 900ms HoldDelay has not elapsed - it must not fire yet.");
            Assert.AreEqual("fastConfig", _synthetic[0].TargetConfigGUID);
            Assert.AreEqual((double)MobiFlightButton.InputEvent.HOLD, _synthetic[0].Value);

            _now = _now.AddMilliseconds(850); // total 900ms
            _generator.Tick();

            Assert.HasCount(2, _synthetic, "slowConfig's own HoldDelay has now elapsed.");
            Assert.AreEqual("slowConfig", _synthetic[1].TargetConfigGUID);
        }

        [TestMethod]
        public void ResolveTimings_TwoConfigsOnSameButton_ReleaseFansOutOneEventPerConfig()
        {
            _generator.ResolveTimings = e => new List<ButtonBinding>
            {
                new ButtonBinding("shortLongRelease", new ButtonTimings(holdDelay: 300, repeatDelay: 0, longReleaseDelay: 200)),
                new ButtonBinding("longLongRelease", new ButtonTimings(holdDelay: 300, repeatDelay: 0, longReleaseDelay: 800))
            };

            _generator.Observe(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(300); // > shortLongRelease's 200ms, < longLongRelease's 800ms
            var released = _generator.Observe(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.RELEASE));

            Assert.HasCount(2, released, "One release event must be produced per config bound to the button.");

            var forShort = released.Single(e => e.TargetConfigGUID == "shortLongRelease");
            var forLong = released.Single(e => e.TargetConfigGUID == "longLongRelease");

            Assert.AreEqual((double)MobiFlightButton.InputEvent.LONG_RELEASE, forShort.Value,
                "300ms exceeds shortLongRelease's own 200ms threshold - this config must see LONG_RELEASE.");
            Assert.AreEqual((double)MobiFlightButton.InputEvent.RELEASE, forLong.Value,
                "300ms is under longLongRelease's own 800ms threshold - this config must see a plain RELEASE, not LONG_RELEASE.");
        }

        // RepeatDelay floor - protects the sim API from a too-fast repeat.

        [TestMethod]
        [DataRow(0, 0, DisplayName = "0 (disabled) is exempt from the floor")]
        [DataRow(1, ButtonTimings.MinRepeatDelay, DisplayName = "a tiny positive value is raised to the floor")]
        [DataRow(ButtonTimings.MinRepeatDelay - 1, ButtonTimings.MinRepeatDelay, DisplayName = "just under the floor is raised to it")]
        [DataRow(ButtonTimings.MinRepeatDelay, ButtonTimings.MinRepeatDelay, DisplayName = "exactly the floor is unchanged")]
        [DataRow(900, 900, DisplayName = "comfortably above the floor is unchanged")]
        public void ButtonTimings_ClampsRepeatDelayToTheFloor(int configured, int expected)
        {
            var timings = new ButtonTimings(holdDelay: 300, repeatDelay: configured, longReleaseDelay: 300);

            Assert.AreEqual(expected, timings.RepeatDelay);
        }

        [TestMethod]
        public void ResolveTimings_RepeatDelayBelowFloor_ActualRepeatCadenceUsesFloorNotConfiguredValue()
        {
            _generator.ResolveTimings = e => new List<ButtonBinding>
            {
                new ButtonBinding("cfgA", new ButtonTimings(holdDelay: 300, repeatDelay: 10, longReleaseDelay: 300))
            };
            ObserveOne(ButtonEvent("S1", "Btn1", MobiFlightButton.InputEvent.PRESS));

            _now = _now.AddMilliseconds(300);
            _generator.Tick(); // HOLD

            _now = _now.AddMilliseconds(10); // the configured (but floored) 10ms
            _generator.Tick();
            Assert.HasCount(1, _synthetic, "10ms is below the floor - no repeat yet.");

            _now = _now.AddMilliseconds(ButtonTimings.MinRepeatDelay - 10); // total: the floor
            _generator.Tick();
            Assert.HasCount(2, _synthetic, "The floor has now elapsed since the HOLD - repeat fires.");
            Assert.AreEqual((double)MobiFlightButton.InputEvent.REPEAT, _synthetic[1].Value);
        }
    }
}
