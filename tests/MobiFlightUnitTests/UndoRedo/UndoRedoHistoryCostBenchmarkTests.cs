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
    public class UndoRedoHistoryCostBenchmarkTests
    {
        private const string FixturePath =
            @"assets\Base\ConfigFile\Json\OpenProjectTest.mfproj";

        private const int HistoryDepth = 100;

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CompareHistoryStrategiesWithRealConfigItems()
        {
            var seed = LoadSeed();

            Assert.IsTrue(
                seed.Count > 0,
                "Fixture contains no ConfigItems.");

            TestContext.WriteLine(
                $"Loaded {seed.Count} real ConfigItems.");

            TestContext.WriteLine(
                "Items | Strategy | Execute ms | Undo ms | " +
                "Redo ms | Execute allocated KB | Retained KB");

            var strategies = new[]
            {
                "Command",
                "ItemSnapshot",
                "FullSnapshot"
            };

            // Warm up code paths and JIT compilation.
            foreach (var strategy in strategies)
            {
                Measure(strategy, seed, 8, 4);
            }

            foreach (var itemCount in new[] { 25, 100, 500 })
            {
                foreach (var strategy in strategies)
                {
                    var result = Measure(
                        strategy,
                        seed,
                        itemCount,
                        HistoryDepth);

                    TestContext.WriteLine(
                        $"{itemCount,5} | " +
                        $"{strategy,-12} | " +
                        $"{result.ExecuteMs,10:F3} | " +
                        $"{result.UndoMs,7:F3} | " +
                        $"{result.RedoMs,7:F3} | " +
                        $"{result.AllocatedBytes / 1024.0,20:F1} | " +
                        $"{result.RetainedBytes / 1024.0,11:F1}");
                }
            }
        }

        private static List<IConfigItem> LoadSeed()
        {
            Assert.IsTrue(
                File.Exists(FixturePath),
                $"Missing fixture: {FixturePath}");

            var project = new Project
            {
                FilePath = FixturePath
            };

            project.OpenFile();

            return project.ConfigFiles
                .SelectMany(file => file.ConfigItems)
                .ToList();
        }

        private static List<IConfigItem> CreateDataset(
            IReadOnlyList<IConfigItem> seed,
            int count)
        {
            var items = new List<IConfigItem>(count);

            for (var i = 0; i < count; i++)
            {
                var item =
                    (IConfigItem)seed[i % seed.Count].Clone();

                // Expanded test projects need unique IDs.
                item.GUID = Guid.NewGuid().ToString();

                items.Add(item);
            }

            return items;
        }

        private static BenchmarkResult Measure(
            string strategy,
            IReadOnlyList<IConfigItem> seed,
            int itemCount,
            int historyDepth)
        {
            var items = CreateDataset(seed, itemCount);

            IHistory history = strategy switch
            {
                "Command" =>
                    new CommandHistory(items),

                "ItemSnapshot" =>
                    new ItemSnapshotHistory(items),

                "FullSnapshot" =>
                    new FullSnapshotHistory(items),

                _ => throw new ArgumentException(strategy)
            };

            var originalStates = history.Items
                .Select(item => item.Active)
                .ToArray();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var memoryBefore =
                GC.GetTotalMemory(true);

            var allocatedBefore =
                GC.GetAllocatedBytesForCurrentThread();

            var stopwatch = Stopwatch.StartNew();

            // Record multiple independent user actions.
            for (var i = 0; i < historyDepth; i++)
            {
                history.Execute(i % itemCount);
            }

            stopwatch.Stop();

            var executeMs =
                stopwatch.Elapsed.TotalMilliseconds;

            var allocatedBytes =
                GC.GetAllocatedBytesForCurrentThread()
                - allocatedBefore;

            // Approximate retained memory while history
            // and project state are still reachable.
            var retainedBytes =
                GC.GetTotalMemory(true) - memoryBefore;

            stopwatch.Restart();

            for (var i = 0; i < historyDepth; i++)
            {
                history.Undo();
            }

            stopwatch.Stop();

            var undoMs =
                stopwatch.Elapsed.TotalMilliseconds;

            // All original Active states must be restored.
            CollectionAssert.AreEqual(
                originalStates,
                history.Items
                    .Select(item => item.Active)
                    .ToArray());

            stopwatch.Restart();

            for (var i = 0; i < historyDepth; i++)
            {
                history.Redo();
            }

            stopwatch.Stop();

            var redoMs =
                stopwatch.Elapsed.TotalMilliseconds;

            // Verify the final state after redoing
            // the same number of toggle operations.
            var expectedStates =
                originalStates.ToArray();

            for (var i = 0; i < historyDepth; i++)
            {
                var index = i % itemCount;
                expectedStates[index] = !expectedStates[index];
            }

            CollectionAssert.AreEqual(
                expectedStates,
                history.Items
                    .Select(item => item.Active)
                    .ToArray());

            GC.KeepAlive(history);

            return new BenchmarkResult(
                executeMs,
                undoMs,
                redoMs,
                allocatedBytes,
                retainedBytes);
        }

        private sealed record BenchmarkResult(
            double ExecuteMs,
            double UndoMs,
            double RedoMs,
            long AllocatedBytes,
            long RetainedBytes);

        private interface IHistory
        {
            IReadOnlyList<IConfigItem> Items { get; }

            void Execute(int index);
            void Undo();
            void Redo();
        }

        private sealed class CommandHistory : IHistory
        {
            private readonly List<IConfigItem> _items;

            private readonly Stack<CommandEntry> _undo = new();
            private readonly Stack<CommandEntry> _redo = new();

            public CommandHistory(List<IConfigItem> items)
            {
                _items = items;
            }

            public IReadOnlyList<IConfigItem> Items => _items;

            public void Execute(int index)
            {
                var item = _items[index];

                var entry = new CommandEntry(
                    index,
                    item.GUID,
                    item.Active,
                    !item.Active);

                item.Active = entry.After;

                _undo.Push(entry);
                _redo.Clear();
            }

            public void Undo()
            {
                var entry = _undo.Pop();
                var item = _items[entry.Index];

                if (item.GUID != entry.Id)
                {
                    throw new InvalidOperationException(
                        "Command target changed.");
                }

                item.Active = entry.Before;
                _redo.Push(entry);
            }

            public void Redo()
            {
                var entry = _redo.Pop();
                var item = _items[entry.Index];

                if (item.GUID != entry.Id)
                {
                    throw new InvalidOperationException(
                        "Command target changed.");
                }

                item.Active = entry.After;
                _undo.Push(entry);
            }

            private sealed record CommandEntry(
                int Index,
                string Id,
                bool Before,
                bool After);
        }

        private sealed class ItemSnapshotHistory : IHistory
        {
            private readonly List<IConfigItem> _items;

            private readonly Stack<ItemEntry> _undo = new();
            private readonly Stack<ItemEntry> _redo = new();

            public ItemSnapshotHistory(List<IConfigItem> items)
            {
                _items = items;
            }

            public IReadOnlyList<IConfigItem> Items => _items;

            public void Execute(int index)
            {
                var item = _items[index];

                _undo.Push(new ItemEntry(
                    index,
                    (IConfigItem)item.Clone()));

                item.Active = !item.Active;
                _redo.Clear();
            }

            public void Undo()
            {
                Restore(_undo, _redo);
            }

            public void Redo()
            {
                Restore(_redo, _undo);
            }

            private void Restore(
                Stack<ItemEntry> source,
                Stack<ItemEntry> destination)
            {
                var entry = source.Pop();

                var current =
                    (IConfigItem)_items[entry.Index].Clone();

                // Replace with a fresh copy to avoid
                // modifying objects held in history.
                _items[entry.Index] =
                    (IConfigItem)entry.Snapshot.Clone();

                destination.Push(
                    new ItemEntry(entry.Index, current));
            }

            private sealed record ItemEntry(
                int Index,
                IConfigItem Snapshot);
        }

        private sealed class FullSnapshotHistory : IHistory
        {
            private List<IConfigItem> _items;

            private readonly Stack<List<IConfigItem>>
                _undo = new();

            private readonly Stack<List<IConfigItem>>
                _redo = new();

            public FullSnapshotHistory(List<IConfigItem> items)
            {
                _items = items;
            }

            public IReadOnlyList<IConfigItem> Items => _items;

            public void Execute(int index)
            {
                _undo.Push(CloneItems(_items));

                _items[index].Active =
                    !_items[index].Active;

                _redo.Clear();
            }

            public void Undo()
            {
                _redo.Push(CloneItems(_items));

                _items = CloneItems(_undo.Pop());
            }

            public void Redo()
            {
                _undo.Push(CloneItems(_items));

                _items = CloneItems(_redo.Pop());
            }

            private static List<IConfigItem> CloneItems(
                IEnumerable<IConfigItem> items)
            {
                return items
                    .Select(item => (IConfigItem)item.Clone())
                    .ToList();
            }
        }
    }
}
