using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.InputConfig;
using System.Collections.Generic;
using System.Xml;

namespace MobiFlight.InputConfig.Tests
{
    [TestClass]
    public class InputActionExecutionCacheTests
    {
        [TestMethod]
        public void Execute_ButtonInputConfig_SkipsDuplicateWhenStringValueUnchanged()
        {
            var cache = new InputActionExecutionCache();
            var action = new TrackingInputAction();
            var config = new ButtonInputConfig
            {
                onPress = action
            };

            var firstResult = cache.Execute(
                config,
                new CacheCollection(),
                new InputEventArgs { Value = 1, StrValue = "phase1" },
                new List<ConfigRefValue>());
            var secondResult = cache.Execute(
                config,
                new CacheCollection(),
                new InputEventArgs { Value = 1, StrValue = "phase1" },
                new List<ConfigRefValue>());

            Assert.IsTrue(firstResult);
            Assert.IsFalse(secondResult);
            Assert.AreEqual(1, action.ExecuteCount);
        }

        [TestMethod]
        public void Execute_ButtonInputConfig_ExecutesWhenStringValueChanges()
        {
            var cache = new InputActionExecutionCache();
            var action = new TrackingInputAction();
            var config = new ButtonInputConfig
            {
                onPress = action
            };

            var firstResult = cache.Execute(
                config,
                new CacheCollection(),
                new InputEventArgs { Value = 1, StrValue = "phase1" },
                new List<ConfigRefValue>());
            var secondResult = cache.Execute(
                config,
                new CacheCollection(),
                new InputEventArgs { Value = 1, StrValue = "phase2" },
                new List<ConfigRefValue>());

            Assert.IsTrue(firstResult);
            Assert.IsTrue(secondResult);
            Assert.AreEqual(2, action.ExecuteCount);
        }

        [TestMethod]
        public void Execute_AnalogInputConfig_SkipsDuplicateWhenNumericValueUnchangedAndStringMissing()
        {
            var cache = new InputActionExecutionCache();
            var action = new TrackingInputAction();
            var config = new AnalogInputConfig
            {
                onChange = action
            };

            var firstResult = cache.Execute(
                config,
                new CacheCollection(),
                new InputEventArgs { Value = 42, StrValue = null },
                new List<ConfigRefValue>());
            var secondResult = cache.Execute(
                config,
                new CacheCollection(),
                new InputEventArgs { Value = 42, StrValue = null },
                new List<ConfigRefValue>());

            Assert.IsTrue(firstResult);
            Assert.IsFalse(secondResult);
            Assert.AreEqual(1, action.ExecuteCount);
        }

        [TestMethod]
        public void Execute_AnalogInputConfig_ExecutesWhenNumericValueChangesAndStringMissing()
        {
            var cache = new InputActionExecutionCache();
            var action = new TrackingInputAction();
            var config = new AnalogInputConfig
            {
                onChange = action
            };

            var firstResult = cache.Execute(
                config,
                new CacheCollection(),
                new InputEventArgs { Value = 42, StrValue = null },
                new List<ConfigRefValue>());
            var secondResult = cache.Execute(
                config,
                new CacheCollection(),
                new InputEventArgs { Value = 43, StrValue = null },
                new List<ConfigRefValue>());

            Assert.IsTrue(firstResult);
            Assert.IsTrue(secondResult);
            Assert.AreEqual(2, action.ExecuteCount);
        }

        [TestMethod]
        public void Execute_AnalogInputConfig_ExecutesWhenStrValueTransitionsFromNullToText()
        {
            var cache = new InputActionExecutionCache();
            var action = new TrackingInputAction();
            var config = new AnalogInputConfig
            {
                onChange = action
            };

            var firstResult = cache.Execute(
                config,
                new CacheCollection(),
                new InputEventArgs { Value = 7, StrValue = null },
                new List<ConfigRefValue>());
            var secondResult = cache.Execute(
                config,
                new CacheCollection(),
                new InputEventArgs { Value = 7, StrValue = "phase7" },
                new List<ConfigRefValue>());

            Assert.IsTrue(firstResult);
            Assert.IsTrue(secondResult);
            Assert.AreEqual(2, action.ExecuteCount);
        }

        [TestMethod]
        public void Execute_ButtonInputConfig_DoesNotSkipReleaseAfterPress_WhenStringMissing()
        {
            var cache = new InputActionExecutionCache();
            var pressAction = new TrackingInputAction();
            var releaseAction = new TrackingInputAction();
            var config = new ButtonInputConfig
            {
                onPress = pressAction,
                onRelease = releaseAction
            };

            var pressResult = cache.Execute(
                config,
                new CacheCollection(),
                new InputEventArgs { Value = 1, StrValue = null },
                new List<ConfigRefValue>());
            var releaseResult = cache.Execute(
                config,
                new CacheCollection(),
                new InputEventArgs { Value = 0, StrValue = null },
                new List<ConfigRefValue>());

            Assert.IsTrue(pressResult);
            Assert.IsTrue(releaseResult);
            Assert.AreEqual(1, pressAction.ExecuteCount);
            Assert.AreEqual(1, releaseAction.ExecuteCount);
        }

        private class TrackingInputAction : InputAction
        {
            public int ExecuteCount { get; private set; }

            public override object Clone()
            {
                return new TrackingInputAction();
            }

            public override void ReadXml(XmlReader reader)
            {
            }

            public override void WriteXml(XmlWriter writer)
            {
            }

            public override void execute(CacheCollection cacheCollection, InputEventArgs e, List<ConfigRefValue> configRefs)
            {
                ExecuteCount++;
            }
        }
    }
}
