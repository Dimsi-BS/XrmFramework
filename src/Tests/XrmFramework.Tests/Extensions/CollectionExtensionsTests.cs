// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace XrmFramework.Tests.Extensions
{
    [TestFixture]
    public class CollectionExtensionsTests
    {
        // ────────────────────────────────────────────────────────────
        //  SplitList
        // ────────────────────────────────────────────────────────────

        [Test]
        public void SplitList_EvenDivision_ProducesCorrectChunks()
        {
            var list = Enumerable.Range(1, 10).ToList();
            var chunks = list.SplitList(5).ToList();

            Assert.AreEqual(2, chunks.Count);
            Assert.AreEqual(5, chunks[0].Count);
            Assert.AreEqual(5, chunks[1].Count);
        }

        [Test]
        public void SplitList_UnevenDivision_LastChunkHasRemainder()
        {
            var list = Enumerable.Range(1, 7).ToList();
            var chunks = list.SplitList(3).ToList();

            Assert.AreEqual(3, chunks.Count);
            Assert.AreEqual(3, chunks[0].Count);
            Assert.AreEqual(3, chunks[1].Count);
            Assert.AreEqual(1, chunks[2].Count);
        }

        [Test]
        public void SplitList_SizeGreaterThanCollection_ReturnsSingleChunk()
        {
            var list = new List<int> { 1, 2, 3 };
            var chunks = list.SplitList(1000).ToList();

            Assert.AreEqual(1, chunks.Count);
            Assert.AreEqual(3, chunks[0].Count);
        }

        [Test]
        public void SplitList_EmptyCollection_ReturnsNoChunks()
        {
            var list = new List<int>();
            var chunks = list.SplitList(5).ToList();

            Assert.AreEqual(0, chunks.Count);
        }

        [Test]
        public void SplitList_SizeOfOne_EachItemInOwnChunk()
        {
            var list = new List<int> { 1, 2, 3 };
            var chunks = list.SplitList(1).ToList();

            Assert.AreEqual(3, chunks.Count);
            Assert.AreEqual(1, chunks[0][0]);
            Assert.AreEqual(2, chunks[1][0]);
            Assert.AreEqual(3, chunks[2][0]);
        }

        [Test]
        public void SplitList_DefaultSize_Uses1000()
        {
            var list = Enumerable.Range(1, 999).ToList();
            var chunks = list.SplitList().ToList();

            Assert.AreEqual(1, chunks.Count);
            Assert.AreEqual(999, chunks[0].Count);
        }

        [Test]
        public void SplitList_PreservesOrder()
        {
            var list = new List<int> { 10, 20, 30, 40, 50 };
            var all = list.SplitList(2).SelectMany(c => c).ToList();

            Assert.That(all, Is.EqualTo(list));
        }
    }
}
