using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;

namespace MobiFlight.Tests
{
    [TestClass]
    public class ItemSnapshotHistoryDepthTests
    {
        private const string FixturePath =
            @"assets\Base\ConfigFile\Json\OpenProjectTest.mfproj";

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CompareBoundedItemSnapshotHistoryDepth()
        {
            var project = new Project
            {
                FilePath = FixturePath
            };

            Assert.IsTrue(File.Exists(FixturePath));

            project.OpenFile();

            var items = project.ConfigFiles
                .SelectMany(file => file.ConfigItems)
                .ToList();

            Assert.IsNotEmpty(items);

            TestContext.WriteLine(
                $"Real ConfigItems loaded: {items.Count}");

            TestContext.WriteLine(
                "Limit | Allocated KB | Retained KB | " +
                "Execute ms | Stored Snapshots");

            // Each item has its own temporary history.
            foreach (var limit in new[] { 10, 25, 50 })
            {
                var result = Measure(items, limit);

                TestContext.WriteLine(
                    $"{limit,5} | " +
                    $"{result.AllocatedBytes / 1024.0,12:F1} | " +
                    $"{result.RetainedBytes / 1024.0,11:F1} | " +
                    $"{result.ElapsedMs,10:F3} | " +
                    $"{result.StoredSnapshots}");
            }
        }

        [TestMethod]
        public void ClonePreservesNestedConfigDataIndependently()
        {
            var project = new Project
            {
                FilePath = FixturePath
            };

            project.OpenFile();

            var items = project.ConfigFiles
                .SelectMany(file => file.ConfigItems)
                .ToList();

            Assert.IsNotEmpty(items);

            foreach (var original in items)
            {
                var copy = (IConfigItem)original.Clone();

                Assert.AreNotSame(original, copy);
                Assert.AreEqual(original.GUID, copy.GUID);
                Assert.AreEqual(original.Name, copy.Name);
                Assert.AreEqual(original.Active, copy.Active);

                if (original.Preconditions != null)
                {
                    Assert.AreNotSame(
                        original.Preconditions,
                        copy.Preconditions);
                }

                if (original.Modifiers != null)
                {
                    Assert.AreNotSame(
                        original.Modifiers,
                        copy.Modifiers);
                }

                if (original.ConfigRefs != null)
                {
                    Assert.AreNotSame(
                        original.ConfigRefs,
                        copy.ConfigRefs);
                }

                if (original.Device != null)
                {
                    Assert.AreNotSame(
                        original.Device,
                        copy.Device);
                }
            }
        }

        private static Measurement Measure(
            IReadOnlyList<IConfigItem> originalItems,
            int maxHistory)
        {
            const int totalChanges = 100;

            var drafts = originalItems
                .Select(item => (IConfigItem)item.Clone())
                .ToList();

            var histories = drafts
                .Select(_ => new Queue<IConfigItem>())
                .ToList();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var beforeMemory = GC.GetTotalMemory(true);
            var beforeAllocated =
                GC.GetAllocatedBytesForCurrentThread();

            var stopwatch = Stopwatch.StartNew();

            for (var step = 0; step < totalChanges; step++)
            {
                for (var index = 0; index < drafts.Count; index++)
                {
                    var draft = drafts[index];

                    draft.Name =
                        $"{originalItems[index].Name} Edit {step}";

                    // Save only the affected ConfigItem.
                    histories[index].Enqueue(
                        (IConfigItem)draft.Clone());

                    // Discard oldest snapshots.
                    if (histories[index].Count > maxHistory)
                    {
                        histories[index].Dequeue();
                    }
                }
            }

            stopwatch.Stop();

            var allocatedBytes =
                GC.GetAllocatedBytesForCurrentThread()
                - beforeAllocated;

            // Approximate retained memory.
            var retainedBytes =
                GC.GetTotalMemory(true) - beforeMemory;

            foreach (var history in histories)
            {
                Assert.AreEqual(
                    maxHistory,
                    history.Count);
            }

            var storedSnapshots = histories
                .Sum(history => history.Count);

            GC.KeepAlive(drafts);
            GC.KeepAlive(histories);

            return new Measurement(
                allocatedBytes,
                retainedBytes,
                stopwatch.Elapsed.TotalMilliseconds,
                storedSnapshots);
        }

        private sealed record Measurement(
            long AllocatedBytes,
            long RetainedBytes,
            double ElapsedMs,
            int StoredSnapshots);
    }
}
