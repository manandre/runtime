// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Tests
{
    public class RightJoinTupleTests : EnumerableBasedTests
    {
        public record CustomerRec
        {
            public string name;
            public int custID;
        }

        public record OrderRec
        {
            public int orderID;
            public int custID;
            public int total;
        }

        public record AnagramRec
        {
            public string name;
            public int orderID;
            public int total;
        }

        [Fact]
        public void FirstOuterMatchesLastInnerLastOuterMatchesFirstInner()
        {
            CustomerRec[] outer = {
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            };
            OrderRec[] inner = {
                new OrderRec{ orderID = 45321, custID = 99022, total = 50 },
                new OrderRec{ orderID = 43421, custID = 29022, total = 20 },
                new OrderRec{ orderID = 95421, custID = 98022, total = 9 }
            };

            var result = outer.AsQueryable().RightJoin(inner.AsQueryable(), e => e.custID, e => e.custID).ToList();
            Assert.Equal(3, result.Count);
            Assert.Equal(("Robert", 45321), (result[0].Outer?.name, result[0].Inner.orderID));
            Assert.Equal((null, 43421), (result[1].Outer?.name, result[1].Inner.orderID));
            Assert.Equal(("Prakash", 95421), (result[2].Outer?.name, result[2].Inner.orderID));
        }

        [Fact]
        public void NullComparer()
        {
            CustomerRec[] outer = {
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            };
            AnagramRec[] inner = {
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            };

            var result = outer.AsQueryable().RightJoin(inner.AsQueryable(), e => e.name, e => e.name, null).ToList();
            Assert.Equal(2, result.Count);
            Assert.Equal((null, 43455), (result[0].Outer?.name, result[0].Inner.orderID));
            Assert.Equal(("Prakash", 323232), (result[1].Outer?.name, result[1].Inner.orderID));
        }

        [Fact]
        public void CustomComparer()
        {
            CustomerRec[] outer = {
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            };
            AnagramRec[] inner = {
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            };

            var result = outer.AsQueryable().RightJoin(inner.AsQueryable(), e => e.name, e => e.name, new AnagramEqualityComparer()).ToList();
            Assert.Equal(2, result.Count);
            Assert.Equal(("Tim", 43455), (result[0].Outer?.name, result[0].Inner.orderID));
            Assert.Equal(("Prakash", 323232), (result[1].Outer?.name, result[1].Inner.orderID));
        }

        [Fact]
        public void OuterNull()
        {
            IQueryable<CustomerRec> outer = null;
            AnagramRec[] inner = {
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            };

            AssertExtensions.Throws<ArgumentNullException>("outer", () => outer.RightJoin(inner.AsQueryable(), e => e.name, e => e.name));
        }

        [Fact]
        public void InnerNull()
        {
            CustomerRec[] outer = {
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            };
            IQueryable<AnagramRec> inner = null;

            AssertExtensions.Throws<ArgumentNullException>("inner", () => outer.AsQueryable().RightJoin(inner, e => e.name, e => e.name));
        }

        [Fact]
        public void OuterKeySelectorNull()
        {
            CustomerRec[] outer = {
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            };
            AnagramRec[] inner = {
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            };

            AssertExtensions.Throws<ArgumentNullException>("outerKeySelector", () => outer.AsQueryable().RightJoin(inner.AsQueryable(), null, e => e.name));
        }

        [Fact]
        public void InnerKeySelectorNull()
        {
            CustomerRec[] outer = {
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            };
            AnagramRec[] inner = {
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            };

            AssertExtensions.Throws<ArgumentNullException>("innerKeySelector", () => outer.AsQueryable().RightJoin(inner.AsQueryable(), e => e.name, null));
        }

        [Fact]
        public void SelectorsReturnNull()
        {
            int?[] outer = { null, null };
            int?[] inner = { null, null, null };

            var result = outer.AsQueryable().RightJoin(inner.AsQueryable(), e => e, e => e).ToList();
            Assert.Equal(3, result.Count);
            Assert.All(result, item => Assert.Null(item.Outer));
        }

        [Fact]
        public void Join1()
        {
            var result = new[] { 0, 1, 2 }.AsQueryable().RightJoin(new[] { 1, 2, 3 }, n1 => n1, n2 => n2).ToList();
            Assert.Equal(3, result.Count);
            Assert.Equal((1, 1), (result[0].Outer, result[0].Inner));
            Assert.Equal((2, 2), (result[1].Outer, result[1].Inner));
            Assert.Equal((0, 3), (result[2].Outer, result[2].Inner));
        }

        [Fact]
        public void Join2()
        {
            var result = new[] { 0, 1, 2 }.AsQueryable().RightJoin(new[] { 1, 2, 3 }, n1 => n1, n2 => n2, EqualityComparer<int>.Default).ToList();
            Assert.Equal(3, result.Count);
            Assert.Equal((1, 1), (result[0].Outer, result[0].Inner));
            Assert.Equal((2, 2), (result[1].Outer, result[1].Inner));
            Assert.Equal((0, 3), (result[2].Outer, result[2].Inner));
        }

        [Fact]
        public void OuterNullNoComparer()
        {
            IQueryable<CustomerRec> outer = null;
            AnagramRec[] inner = {
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            };

            AssertExtensions.Throws<ArgumentNullException>("outer", () => outer.RightJoin(inner.AsQueryable(), e => e.name, e => e.name));
        }

        [Fact]
        public void InnerNullNoComparer()
        {
            CustomerRec[] outer = {
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            };
            IQueryable<AnagramRec> inner = null;

            AssertExtensions.Throws<ArgumentNullException>("inner", () => outer.AsQueryable().RightJoin(inner, e => e.name, e => e.name));
        }

        [Fact]
        public void OuterKeySelectorNullNoComparer()
        {
            CustomerRec[] outer = {
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            };
            AnagramRec[] inner = {
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            };

            AssertExtensions.Throws<ArgumentNullException>("outerKeySelector", () => outer.AsQueryable().RightJoin(inner.AsQueryable(), null, e => e.name));
        }

        [Fact]
        public void InnerKeySelectorNullNoComparer()
        {
            CustomerRec[] outer = {
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            };
            AnagramRec[] inner = {
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            };

            AssertExtensions.Throws<ArgumentNullException>("innerKeySelector", () => outer.AsQueryable().RightJoin(inner.AsQueryable(), e => e.name, null));
        }
    }
}
