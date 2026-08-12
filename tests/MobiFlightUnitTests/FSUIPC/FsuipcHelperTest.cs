using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using MobiFlight.FSUIPC;
using MobiFlight.OutputConfig;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlightInstaller.FSUIPC
{
    [TestClass]
    public class FsuipcHelperTest
    {


        [TestMethod]
        public void ExecuteRead_8ByteOffset_DoesNotRoundDoubleValue()
        {
            // Arrange
            const double expectedValue = 12345.6789;

            var offset = new FsuipcOffset
            {
                Offset = 0x1234,
                OffsetType = FSUIPCOffsetType.Integer,
                Size = 8
            };

            var cacheMock = new Mock<FSUIPCCacheInterface>();
            cacheMock.Setup(x => x.getDoubleValue(offset.Offset, offset.Size))
                     .Returns(expectedValue);

            // Act
            var result = FsuipcHelper.executeRead(offset, cacheMock.Object);

            // Assert
            Assert.AreEqual(FSUIPCOffsetType.Float, result.type);
            Assert.AreEqual(expectedValue, result.Float64);
            Assert.AreNotEqual(Math.Round(expectedValue), result.Float64);
        }
    }
}
