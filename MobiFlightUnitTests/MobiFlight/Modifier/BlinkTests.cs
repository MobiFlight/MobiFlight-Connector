using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Modifier.Tests
{
    [TestClass()]
    public class BlinkTests
    {
        [TestMethod()]
        public void EqualsTest()
        {
            // Check for equality between two Blink objects
            var blink1 = new Blink();
            var blink2 = new Blink();
            Assert.AreEqual(blink1, blink2);

            // Check for inequality between two Blink objects
            blink1.BlinkValue = "1";
            Assert.AreNotEqual(blink1, blink2);

            blink2.BlinkValue = "1";
            Assert.AreEqual(blink1, blink2);

            blink1.OnOffSequence.Add(1);
            Assert.AreNotEqual(blink1, blink2);

            blink2.OnOffSequence.Add(1);
            Assert.AreEqual(blink1, blink2);

            blink1.FirstExecutionTime = 1;
            Assert.AreNotEqual(blink1, blink2);

            blink2.FirstExecutionTime = 1;
            Assert.AreEqual(blink1, blink2);

            blink1.OffDurationInMs = 1;
            Assert.AreNotEqual(blink1, blink2);

            blink2.OffDurationInMs = 1;
            Assert.AreEqual(blink1, blink2);
        }
    }
}