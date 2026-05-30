// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;
using XrmFramework.Sdk.Queries;

namespace XrmFramework.Tests.Sdk.Queries
{
    [TestFixture]
    public class PagingInfoTests
    {
        [Test]
        public void DefaultPageNumber_IsOne()
        {
            var paging = new PagingInfo();
            Assert.AreEqual(1, paging.PageNumber);
        }

        [Test]
        public void DefaultPageSize_IsNull()
        {
            var paging = new PagingInfo();
            Assert.IsNull(paging.PageSize);
        }

        [Test]
        public void DefaultPagingCookie_IsNull()
        {
            var paging = new PagingInfo();
            Assert.IsNull(paging.PagingCookie);
        }

        [Test]
        public void PageNumber_CanBeSet()
        {
            var paging = new PagingInfo { PageNumber = 5 };
            Assert.AreEqual(5, paging.PageNumber);
        }

        [Test]
        public void PageSize_CanBeSet()
        {
            var paging = new PagingInfo { PageSize = 100 };
            Assert.AreEqual(100, paging.PageSize);
        }

        [Test]
        public void PagingCookie_CanBeSet()
        {
            var paging = new PagingInfo { PagingCookie = "cookie123" };
            Assert.AreEqual("cookie123", paging.PagingCookie);
        }
    }
}
