using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobiFlight.Monitors.Tests
{
    [TestClass]
    public class UsbDeviceMonitorTests
    {
        private class TestableUsbDeviceMonitor : UsbDeviceMonitor
        {
            public Func<Task<List<PortDetails>>> OnGetUsbPorts { get; set; }
            public int ScanCount { get; private set; }
            public bool IsScanning => isScanning;

            public void RunScan() => Scan();

            protected override Task<List<PortDetails>> GetUsbPorts()
            {
                ScanCount++;
                return OnGetUsbPorts();
            }
        }

        #region Scan

        [TestMethod]
        public void Scan_WhenScanFails_ShouldNotThrowAndAllowNextScan()
        {
            // Arrange
            var monitor = new TestableUsbDeviceMonitor
            {
                OnGetUsbPorts = () => throw new InvalidOperationException("drive enumeration failed")
            };

            // Act
            monitor.RunScan();

            // Assert
            Assert.IsFalse(monitor.IsScanning, "A failed scan must not block further scans");
        }

        [TestMethod]
        public void Scan_AfterFailedScan_ShouldScanAgain()
        {
            // Arrange
            var shouldFail = true;
            var monitor = new TestableUsbDeviceMonitor
            {
                OnGetUsbPorts = () =>
                {
                    if (shouldFail) throw new InvalidOperationException("drive enumeration failed");
                    return Task.FromResult(new List<PortDetails>());
                }
            };
            monitor.RunScan();
            shouldFail = false;

            // Act
            monitor.RunScan();

            // Assert
            Assert.AreEqual(2, monitor.ScanCount, "The second scan should not be skipped");
        }

        [TestMethod]
        public void Scan_WhenScanFails_ShouldKeepDetectedPorts()
        {
            // Arrange
            var port = new PortDetails { Name = "E:\\", HardwareId = "RPI-RP2" };
            var unavailableRaised = false;
            var shouldFail = false;
            var monitor = new TestableUsbDeviceMonitor
            {
                OnGetUsbPorts = () =>
                {
                    if (shouldFail) throw new InvalidOperationException("drive enumeration failed");
                    return Task.FromResult(new List<PortDetails> { port });
                }
            };
            monitor.PortUnavailable += (sender, p) => unavailableRaised = true;
            monitor.RunScan();
            shouldFail = true;

            // Act
            monitor.RunScan();

            // Assert
            Assert.IsFalse(unavailableRaised, "A failed scan must not report ports as removed");
            Assert.AreEqual(1, monitor.DetectedPorts.Count);
        }

        [TestMethod]
        public void Scan_WhenScanSucceeds_ShouldReportNewPortAndAllowNextScan()
        {
            // Arrange
            var port = new PortDetails { Name = "E:\\", HardwareId = "RPI-RP2" };
            PortDetails reportedPort = null;
            var monitor = new TestableUsbDeviceMonitor
            {
                OnGetUsbPorts = () => Task.FromResult(new List<PortDetails> { port })
            };
            monitor.PortAvailable += (sender, p) => reportedPort = p;

            // Act
            monitor.RunScan();

            // Assert
            Assert.AreSame(port, reportedPort);
            Assert.IsFalse(monitor.IsScanning);
        }

        #endregion
    }
}
