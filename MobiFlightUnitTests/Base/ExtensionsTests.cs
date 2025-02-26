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
    public class ExtensionsTests
    {
        [TestMethod()]
        public void ReverseTest()
        {
            byte value = 1;
            Assert.AreEqual(128, value.Reverse(), $"The value was not reversed properly.");

            value = 128;
            Assert.AreEqual(1, value.Reverse(), $"The value was not reversed properly.");

            value = 0b11110000;
            Assert.AreEqual(0b00001111, value.Reverse(), $"The value was not reversed properly.");

            value = 0b10110111;
            Assert.AreEqual(0b11101101, value.Reverse(), $"The value was not reversed properly.");
        }

        [TestMethod()]
        public void GenerateSimpleHashTest()
        {
            string xTouch = "X-TOUCH MINI";
            int xTouchHash = xTouch.GenerateSimpleHash();
            Assert.AreEqual(954907131, xTouchHash, "Failed to generate correct hash from string.");

            string empty = "";
            int emptyHash = empty.GenerateSimpleHash();
            Assert.AreEqual(0, emptyHash, "Failed to generate correct hash from string.");

            string shortString = "a";
            int shortStringHash = shortString.GenerateSimpleHash();
            Assert.AreEqual(97, shortStringHash, "Failed to generate correct hash from string.");

            string longString = "This is a rather long string to hash. Let's do some magic. The result will be fantastic.";
            int longStringHash = longString.GenerateSimpleHash();
            Assert.AreEqual(1057832466, longStringHash, "Failed to generate correct hash from string.");

            string japanese = "日本にほんでは";
            int japaneseHash = japanese.GenerateSimpleHash();
            Assert.AreEqual(1550985689, japaneseHash, "Failed to generate correct hash from string.");

            string german = "Müßiggang";
            int germanHash = german.GenerateSimpleHash();
            Assert.AreEqual(1480680476, germanHash, "Failed to generate correct hash from string.");

            string fail = "My fail";
            int failHash = fail.GenerateSimpleHash();
            Assert.AreNotEqual(0, failHash, "Hash of 0 should not be the result.");
        }

        [TestMethod()]
        public void AreEqualTest()
        {
            string s1 = null;
            string s2 = null;
            Assert.IsTrue(s1.AreEqual(s2));

            s1 = "Hello";
            Assert.IsFalse(s1.AreEqual(s2));
            s2 = "World!";
            Assert.IsFalse(s1.AreEqual(s2));
            s2 = "Hello";
            Assert.IsTrue(s1.AreEqual(s2));
        }
    }
}