using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using System;
using System.Collections.Generic;

namespace MobiFlight.Tests
{
    [TestClass]
    public class LRUCacheTests
    {
        [TestMethod]
        public void Add_ShouldAddItemToCache()
        {
            var cache = new LRUCache<int, string>(2);
            cache.Add(1, "one");

            Assert.AreEqual("one", cache[1]);
        }

        [TestMethod]
        public void Add_ShouldEvictOldestItemWhenCapacityIsReached()
        {
            var cache = new LRUCache<int, string>(2);
            cache.Add(1, "one");
            cache.Add(2, "two");
            cache.Add(3, "three");

            bool exceptionThrown = false;
            try
            {
                var value = cache[1];
            }
            catch (KeyNotFoundException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
            Assert.AreEqual("two", cache[2]);
            Assert.AreEqual("three", cache[3]);
        }

        [TestMethod]
        public void Get_ShouldReturnItemAndUpdateLRUOrder()
        {
            var cache = new LRUCache<int, string>(2);
            cache.Add(1, "one");
            cache.Add(2, "two");

            // use the item to update its LRU order
            // 2 becomes oldest
            var value = cache[1];
            Assert.AreEqual("one", value);

            // add a new item to trigger eviction
            // which affects `2`
            cache.Add(3, "three");

            bool exceptionThrown = false;
            try
            {
                var value2 = cache[2];
            }
            catch (KeyNotFoundException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
            Assert.AreEqual("one", cache[1]);
            Assert.AreEqual("three", cache[3]);
        }

        [TestMethod]
        public void TryGetValue_ShouldReturnTrueAndValueIfKeyExists()
        {
            var cache = new LRUCache<int, string>(2);
            cache.Add(1, "one");

            var result = cache.TryGetValue(1, out var value);

            Assert.IsTrue(result);
            Assert.AreEqual("one", value);
        }

        [TestMethod]
        public void TryGetValue_ShouldReturnFalseIfKeyDoesNotExist()
        {
            var cache = new LRUCache<int, string>(2);

            var result = cache.TryGetValue(1, out var value);

            Assert.IsFalse(result);
            Assert.IsNull(value);
        }

        [TestMethod]
        public void Indexer_ShouldGetAndSetValue()
        {
            var cache = new LRUCache<int, string>(2);
            cache[1] = "one";

            Assert.AreEqual("one", cache[1]);

            cache[1] = "uno";
            Assert.AreEqual("uno", cache[1]);
        }
    }
}