using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class TransformationTests
    {
        [TestMethod()]
        public void ApplyTest()
        {
            Transformation t = new Transformation();
            t.Expression = "$*0.5";
            Assert.AreEqual(0.5,t.Apply(1));
            t.Expression = "$*2";
            Assert.AreEqual(2.0, t.Apply(1));
            t.Expression = "$*2";
            Assert.AreEqual(2.0, t.Apply(1.0));
            t.Expression = "$*2.0";
            Assert.AreEqual(2.0, t.Apply(1));

            // test the substring stuff
            t.SubStrStart = 1;
            t.SubStrEnd = 5;
            string test = "UnitTest";
            Assert.AreEqual("nitT", t.Apply(test));
        }
    }
}