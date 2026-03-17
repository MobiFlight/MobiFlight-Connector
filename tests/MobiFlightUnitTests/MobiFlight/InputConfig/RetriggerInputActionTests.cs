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
    public class RetriggerInputActionTests
    {
        [TestMethod()]
        public void EqualsTest()
        {
            RetriggerInputAction o1 = new RetriggerInputAction();
            RetriggerInputAction o2 = new RetriggerInputAction();

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}