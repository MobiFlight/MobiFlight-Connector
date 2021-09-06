using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ConfigRefListTests
    {
        [TestMethod()]
        public void AddTest()
        {
            ConfigRefList o = new ConfigRefList();
            Assert.AreEqual(0, o.Count);
            o.Add(new ConfigRef());
            Assert.AreEqual(1, o.Count);
        }

        [TestMethod()]
        public void ClearTest()
        {
            ConfigRefList o = new ConfigRefList();
            o.Add(new ConfigRef());
            Assert.AreEqual(1, o.Count);
            o.Clear();
            Assert.AreEqual(0, o.Count);
        }

        [TestMethod()]
        public void CloneTest()
        {
            ConfigRefList o = new ConfigRefList();
            ConfigRefList clone = o.Clone() as ConfigRefList;

            Assert.IsNotNull(clone);
            Assert.AreEqual(o.Count, clone.Count);
            o.Add(new ConfigRef());
            Assert.AreNotEqual(o.Count, clone.Count);
            clone = o.Clone() as ConfigRefList;
            Assert.AreEqual(o.Count, clone.Count);
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {
            ConfigRefList o = new ConfigRefList();
            Assert.IsNotNull(o.GetEnumerator());
            Assert.IsTrue(o.GetEnumerator() is List<ConfigRef>.Enumerator);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            ConfigRefList o1 = new ConfigRefList();
            ConfigRefList o2 = new ConfigRefList();

            Assert.IsTrue(o1.Equals(o2));
            o1.Add(new ConfigRef() { Active = true, Placeholder = "#", Ref = "Ref" });
            Assert.IsFalse(o1.Equals(o2));
            o2.Add(new ConfigRef() { Active = true, Placeholder = "#", Ref = "Ref" });
            Assert.IsTrue(o1.Equals(o2));
        }
    }
}