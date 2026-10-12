using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using MobiFlight.Base;
using MobiFlight.InputConfig;
using MobiFlight.Modifier;
using MobiFlight.OutputConfig;
using Newtonsoft.Json.Linq;

namespace MobiFlight.Tests
{
    [TestClass]
    public class ItemSnapshotRestorationTests
    {
        private const string FixturePath =
            @"assets\Base\ConfigFile\Json\OpenProjectTest.mfproj";

        [TestMethod]
        public void EveryFixtureItem_ClonePreservesPersistedData()
        {
            var items = LoadItems();
            Assert.IsNotEmpty(items);
            Assert.IsTrue(items.OfType<InputConfigItem>().Any());
            Assert.IsTrue(items.OfType<OutputConfigItem>().Any());

            foreach (var original in items)
            {
                var snapshot = Copy(original);

                Assert.AreNotSame(original, snapshot);
                Assert.AreEqual(original.GetType(), snapshot.GetType());
                Assert.AreEqual(original.GUID, snapshot.GUID);

                AssertSameSettings(
                    Persisted(original),
                    Persisted(snapshot),
                    $"Clone of {original.GetType().Name}: {original.GUID}");

                AssertSeparateNestedObjects(original, snapshot);
            }
        }

        [TestMethod]
        public void OutputConfigItem_NestedChanges_RoundTripThroughUndoRedo()
        {
            var original = LoadItems()
                .OfType<OutputConfigItem>()
                .First(x => x.Device is LedModule &&
                            x.Source is SimConnectSource);

            // Ensure nested structures are present even if the
            // test fixture is otherwise relatively simple.
            original.Preconditions.Add(new Precondition
            {
                Type = "config",
                Ref = Guid.NewGuid().ToString(),
                Value = "1",
                Operand = "=",
                Logic = "and",
                Active = true
            });

            original.ConfigRefs.Add(new ConfigRef
            {
                Active = true,
                Ref = Guid.NewGuid().ToString(),
                Placeholder = "#",
                TestValue = "1"
            });

            original.Modifiers.Items.Add(new Transformation
            {
                Active = true,
                Expression = "$*2"
            });

            CheckRoundTrip(original, draft =>
            {
                draft.Name = "Edited output";
                draft.Active = !draft.Active;

                ((LedModule)draft.Device).DisplayLedDigits.Add("7");

                ((SimConnectSource)draft.Source)
                    .SimConnectValue.Value = "(L:UNDO_TEST,Number)";

                draft.TestValue.Float64 = 123.0;

                draft.Preconditions
                    .Cast<Precondition>().First().Value = "99";

                draft.ConfigRefs
                    .Cast<ConfigRef>().First().TestValue = "99";

                draft.Modifiers.Items
                    .OfType<Transformation>().First()
                    .Expression = "$+123";
            });
        }

        [TestMethod]
        public void InputConfigItem_NestedChanges_RoundTripThroughUndoRedo()
        {
            var original = LoadItems()
                .OfType<InputConfigItem>()
                .First();

            // A representative input action graph.
            original.button = new ButtonInputConfig
            {
                LongReleaseDelay = 400,
                onPress = new MSFS2020CustomInputAction
                {
                    Command = "(>K:LIGHTS_TOGGLE)",
                    PresetId = "test-preset"
                }
            };

            original.Preconditions.Add(new Precondition
            {
                Type = "config",
                Ref = Guid.NewGuid().ToString(),
                Value = "5",
                Operand = "=",
                Logic = "and",
                Active = true
            });

            original.ConfigRefs.Add(new ConfigRef
            {
                Active = true,
                Ref = Guid.NewGuid().ToString(),
                Placeholder = "#",
                TestValue = "3"
            });

            CheckRoundTrip(original, draft =>
            {
                draft.Name = "Edited input";
                draft.Active = !draft.Active;

                draft.button.LongReleaseDelay = 900;

                ((MSFS2020CustomInputAction)draft.button.onPress)
                    .Command = "(>K:BEACON_LIGHTS_TOGGLE)";

                draft.Preconditions
                    .Cast<Precondition>().Last().Value = "777";

                draft.ConfigRefs
                    .Cast<ConfigRef>().Last().TestValue = "888";
            });
        }

        private static void CheckRoundTrip<T>(
            T original,
            Action<T> modify)
            where T : class, IConfigItem
        {
            var expectedBefore = Persisted(original);

            // Separate editor draft from the committed object.
            var draft = (T)original.Clone();

            // Snapshot before the change.
            var undoSnapshot = (T)draft.Clone();

            AssertSeparateNestedObjects(original, draft);
            AssertSeparateNestedObjects(draft, undoSnapshot);

            modify(draft);

            var expectedAfter = Persisted(draft);

            Assert.IsFalse(
                JToken.DeepEquals(expectedBefore, expectedAfter),
                "The test mutation must change persisted settings.");

            // A draft edit must not leak into the committed item
            // or the undo snapshot.
            AssertSameSettings(
                expectedBefore, Persisted(original),
                "Original changed during draft editing");

            AssertSameSettings(
                expectedBefore, Persisted(undoSnapshot),
                "Undo snapshot changed during draft editing");

            // Save the edited state for Redo, then perform Undo.
            var redoSnapshot = (T)draft.Clone();
            AssertSeparateNestedObjects(draft, redoSnapshot);
            draft = (T)undoSnapshot.Clone();

            AssertSameSettings(
                expectedBefore, Persisted(draft),
                "Undo did not restore all settings");

            // Redo must bring back the full edited state.
            draft = (T)redoSnapshot.Clone();

            AssertSameSettings(
                expectedAfter, Persisted(draft),
                "Redo did not restore all settings");

            // Repeated restoration must not mutate either snapshot.
            draft = (T)undoSnapshot.Clone();
            AssertSameSettings(
                expectedBefore, Persisted(draft),
                "Second Undo differs");

            AssertSameSettings(
                expectedBefore, Persisted(undoSnapshot),
                "Stored Undo snapshot was mutated");

            AssertSameSettings(
                expectedAfter, Persisted(redoSnapshot),
                "Stored Redo snapshot was mutated");
        }

        private static List<IConfigItem> LoadItems()
        {
            var project = new Project { FilePath = FixturePath };
            project.OpenFile();

            return project.ConfigFiles
                .SelectMany(f => f.ConfigItems)
                .ToList();
        }

        private static IConfigItem Copy(IConfigItem item) =>
            (IConfigItem)item.Clone();

        private static JToken Persisted(IConfigItem item)
        {
            var file = new ConfigFile
            {
                Label = "Snapshot comparison",
                EmbedContent = true,
                ConfigItems = new List<IConfigItem> { item }
            };

            // Uses the same persistence-specific contract resolver
            // as normal MobiFlight ConfigFile serialization.
            var json = JObject.Parse(file.ToJson());

            return json["ConfigItems"]?[0]?.DeepClone()
                ?? throw new InvalidOperationException(
                    "ConfigItem was not serialized.");
        }

        private static void AssertSameSettings(
            JToken expected,
            JToken actual,
            string context)
        {
            if (!JToken.DeepEquals(expected, actual))
            {
                Assert.Fail(
                    $"{context}\nExpected:\n{expected}\nActual:\n{actual}");
            }
        }

        private static void AssertSeparateNestedObjects(
            IConfigItem original,
            IConfigItem snapshot)
        {
            if (original.Preconditions != null)
            {
                Assert.AreNotSame(
                    original.Preconditions,
                    snapshot.Preconditions);

                foreach (var pair in original.Preconditions
                    .Cast<Precondition>()
                    .Zip(snapshot.Preconditions.Cast<Precondition>()))
                {
                    Assert.AreNotSame(pair.First, pair.Second);
                }
            }

            if (original.ConfigRefs != null)
            {
                Assert.AreNotSame(
                    original.ConfigRefs,
                    snapshot.ConfigRefs);

                foreach (var pair in original.ConfigRefs
                    .Cast<ConfigRef>()
                    .Zip(snapshot.ConfigRefs.Cast<ConfigRef>()))
                {
                    Assert.AreNotSame(pair.First, pair.Second);
                }
            }

            if (original.Modifiers != null)
            {
                Assert.AreNotSame(
                    original.Modifiers,
                    snapshot.Modifiers);

                foreach (var pair in original.Modifiers.Items
                    .Zip(snapshot.Modifiers.Items))
                {
                    Assert.AreNotSame(pair.First, pair.Second);
                }
            }

            if (original.Controller != null)
                Assert.AreNotSame(original.Controller, snapshot.Controller);

            if (original.Device != null)
                Assert.AreNotSame(original.Device, snapshot.Device);

            if (original.Device is LedModule oldLed &&
                snapshot.Device is LedModule newLed)
            {
                Assert.AreNotSame(
                    oldLed.DisplayLedDigits,
                    newLed.DisplayLedDigits);

                Assert.AreNotSame(
                    oldLed.DisplayLedDecimalPoints,
                    newLed.DisplayLedDecimalPoints);
            }

            if (original.Device is LcdDisplay oldLcd &&
                snapshot.Device is LcdDisplay newLcd)
            {
                Assert.AreNotSame(oldLcd.Lines, newLcd.Lines);
            }

            if (original is OutputConfigItem oldOutput &&
                snapshot is OutputConfigItem newOutput)
            {
                if (oldOutput.Source != null)
                    Assert.AreNotSame(oldOutput.Source, newOutput.Source);

                if (oldOutput.TestValue != null)
                    Assert.AreNotSame(oldOutput.TestValue, newOutput.TestValue);

                if (oldOutput.ButtonInputConfig != null)
                {
                    Assert.AreNotSame(
                        oldOutput.ButtonInputConfig,
                        newOutput.ButtonInputConfig);

                    if (oldOutput.ButtonInputConfig.onPress != null)
                        Assert.AreNotSame(
                            oldOutput.ButtonInputConfig.onPress,
                            newOutput.ButtonInputConfig.onPress);
                }

                if (oldOutput.AnalogInputConfig != null)
                    Assert.AreNotSame(
                        oldOutput.AnalogInputConfig,
                        newOutput.AnalogInputConfig);

                if (oldOutput.Source is SimConnectSource oldSource &&
                    newOutput.Source is SimConnectSource newSource)
                {
                    Assert.AreNotSame(
                        oldSource.SimConnectValue,
                        newSource.SimConnectValue);
                }
            }

            if (original is InputConfigItem oldInput &&
                snapshot is InputConfigItem newInput)
            {
                if (oldInput.button != null)
                {
                    Assert.AreNotSame(oldInput.button, newInput.button);

                    if (oldInput.button.onPress != null)
                        Assert.AreNotSame(
                            oldInput.button.onPress,
                            newInput.button.onPress);

                    if (oldInput.button.onRelease != null)
                        Assert.AreNotSame(
                            oldInput.button.onRelease,
                            newInput.button.onRelease);

                    if (oldInput.button.onHold != null)
                        Assert.AreNotSame(
                            oldInput.button.onHold,
                            newInput.button.onHold);

                    if (oldInput.button.onLongRelease != null)
                        Assert.AreNotSame(
                            oldInput.button.onLongRelease,
                            newInput.button.onLongRelease);
                }

                if (oldInput.encoder != null)
                {
                    Assert.AreNotSame(oldInput.encoder, newInput.encoder);

                    if (oldInput.encoder.onLeft != null)
                        Assert.AreNotSame(
                            oldInput.encoder.onLeft,
                            newInput.encoder.onLeft);

                    if (oldInput.encoder.onRight != null)
                        Assert.AreNotSame(
                            oldInput.encoder.onRight,
                            newInput.encoder.onRight);
                }

                if (oldInput.analog != null)
                {
                    Assert.AreNotSame(oldInput.analog, newInput.analog);
                    if (oldInput.analog.onChange != null)
                        Assert.AreNotSame(
                            oldInput.analog.onChange,
                            newInput.analog.onChange);
                }
            }
        }
    }
}