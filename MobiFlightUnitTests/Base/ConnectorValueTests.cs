using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class ConnectorValueTests
    {
        [TestMethod()]
        public void ToStringTest()
        {
            ConnectorValue o = new ConnectorValue();
            o.Float64 = Double.MaxValue;
            o.String = "TestString";

            o.type = FSUIPCOffsetType.Float;
            Assert.AreEqual(Double.MaxValue.ToString(), o.ToString());

            o.type = FSUIPCOffsetType.Integer;
            Assert.AreEqual(Double.MaxValue.ToString(), o.ToString());

            o.type = FSUIPCOffsetType.String;
            Assert.AreEqual("TestString", o.ToString());

        }
    }
}