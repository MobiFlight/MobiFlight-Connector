using MobiFlight;
using MobiFlight.SimConnectMSFS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MobiFlightUnitTests.SimConnectMSFS
{
    [TestClass()]
    public class SimConnectCacheTests
    {
        [TestMethod()]
        public void GetSimVar_ReturnsFloatAsDoubleCorrectly()
        {
            // Arrange
            var simConnectCache = new SimConnectCache();
            var simVar = new SimVar { ID = 1, Name = "TestVar", Data = 123.456f };

            // Mark the module as connected so Stop() enters the device loop.
            var connectedField = typeof(SimConnectCache).GetField(
                "_wasmConnected",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(connectedField, "connected field should exist.");
            connectedField.SetValue(simConnectCache, true);

            var simVarsField = typeof(SimConnectCache).GetField(
                "SimVars",
                BindingFlags.NonPublic | BindingFlags.Instance);
            (simVarsField.GetValue(simConnectCache) as List<SimVar>).Add(simVar);

            // Act
            var result = simConnectCache.GetSimVar("TestVar", out var stringValue, out var doubleValue);

            // Assert
            Assert.AreEqual(123.456, doubleValue, "The double value should match the float value.");
        }
    }
}