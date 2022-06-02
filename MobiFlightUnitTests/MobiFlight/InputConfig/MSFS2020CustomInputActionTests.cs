using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.InputConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.InputConfig.Tests
{
    [TestClass()]
    public class MSFS2020CustomInputActionTests
    {
        MSFS2020CustomInputAction generateTestObject()
        {
            MSFS2020CustomInputAction o = new MSFS2020CustomInputAction();
            o.Command = "(>K:TOGGLE_LIGHTS)";
            return o;
        }

        [TestMethod()]
        public void EqualsTest()
        {
            MSFS2020CustomInputAction o1 = new MSFS2020CustomInputAction();
            MSFS2020CustomInputAction o2 = new MSFS2020CustomInputAction();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void executeTest()
        {
            MSFS2020CustomInputAction o = generateTestObject();
            o.Command = "@ (>K:THROTTLE_SET)";

            List<ConfigRefValue> configrefs = new List<ConfigRefValue>();
            configrefs.Add(new ConfigRefValue() { ConfigRef = new Base.ConfigRef() { Active = true, Placeholder = "#" }, Value = "1" });


            MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock mock = new MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock();
            MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock simConnectMock = new MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock();
            MobiFlightUnitTests.mock.xplane.XplaneCacheMock xplaneCacheMock = new MobiFlightUnitTests.mock.xplane.XplaneCacheMock();

            CacheCollection cacheCollection = new CacheCollection()
            {
                fsuipcCache = mock,
                simConnectCache = simConnectMock,
                moduleCache = null,
                xplaneCache = xplaneCacheMock
            };

            o.execute(cacheCollection, new InputEventArgs() { Value = 359 }, configrefs);
            
            Assert.AreEqual(simConnectMock.Writes.Count, 1, "The message count is not as expected");
            Assert.AreEqual(simConnectMock.Writes[0], "359 (>K:THROTTLE_SET)", "The Write Value is wrong");
        }
    }
}