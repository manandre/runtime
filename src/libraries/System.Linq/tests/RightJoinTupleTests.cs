// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class RightJoinTupleTests : EnumerableTests
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
        public void OuterEmptyInnerNonEmpty()
        {
            CustomerRec[] outer = [];
            OrderRec[] inner =
            [
                new OrderRec{ orderID = 45321, custID = 98022, total = 50 },
                new OrderRec{ orderID = 97865, custID = 32103, total = 25 }
            ];
            (CustomerRec?, OrderRec)[] expected =
            [
                (null, new OrderRec{ orderID = 45321, custID = 98022, total = 50 }),
                (null, new OrderRec{ orderID = 97865, custID = 32103, total = 25 })
            ];

            Assert.Equal(expected, outer.RightJoin(inner, e => e.custID, e => e.custID));
        }

        [Fact]
        public void FirstOuterMatchesLastInnerLastOuterMatchesFirstInnerSameNumberElements()
        {
            CustomerRec[] outer =
            [
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            ];
            OrderRec[] inner =
            [
                new OrderRec{ orderID = 45321, custID = 99022, total = 50 },
                new OrderRec{ orderID = 43421, custID = 29022, total = 20 },
                new OrderRec{ orderID = 95421, custID = 98022, total = 9 }
            ];
            (CustomerRec?, OrderRec)[] expected =
            [
                (new CustomerRec{ name = "Robert", custID = 99022 }, new OrderRec{ orderID = 45321, custID = 99022, total = 50 }),
                (null, new OrderRec{ orderID = 43421, custID = 29022, total = 20 }),
                (new CustomerRec{ name = "Prakash", custID = 98022 }, new OrderRec{ orderID = 95421, custID = 98022, total = 9 })
            ];

            Assert.Equal(expected, outer.RightJoin(inner, e => e.custID, e => e.custID));
        }

        [Fact]
        public void NullComparer()
        {
            CustomerRec[] outer =
            [
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            ];
            AnagramRec[] inner =
            [
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            ];
            (CustomerRec?, AnagramRec)[] expected =
            [
                (null, new AnagramRec{ name = "miT", orderID = 43455, total = 10 }),
                (new CustomerRec{ name = "Prakash", custID = 98022 }, new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 })
            ];

            Assert.Equal(expected, outer.RightJoin(inner, e => e.name, e => e.name, null));
        }

        [Fact]
        public void CustomComparer()
        {
            CustomerRec[] outer =
            [
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            ];
            AnagramRec[] inner =
            [
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            ];
            (CustomerRec?, AnagramRec)[] expected =
            [
                (new CustomerRec{ name = "Tim", custID = 99021 }, new AnagramRec{ name = "miT", orderID = 43455, total = 10 }),
                (new CustomerRec{ name = "Prakash", custID = 98022 }, new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 })
            ];

            Assert.Equal(expected, outer.RightJoin(inner, e => e.name, e => e.name, new AnagramEqualityComparer()));
        }

        [Fact]
        public void OuterNull()
        {
            CustomerRec[] outer = null;
            AnagramRec[] inner =
            [
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            ];

            AssertExtensions.Throws<ArgumentNullException>("outer", () => outer.RightJoin(inner, e => e.name, e => e.name, new AnagramEqualityComparer()));
        }

        [Fact]
        public void InnerNull()
        {
            CustomerRec[] outer =
            [
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            ];
            AnagramRec[] inner = null;

            AssertExtensions.Throws<ArgumentNullException>("inner", () => outer.RightJoin(inner, e => e.name, e => e.name, new AnagramEqualityComparer()));
        }

        [Fact]
        public void OuterKeySelectorNull()
        {
            CustomerRec[] outer =
            [
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            ];
            AnagramRec[] inner =
            [
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            ];

            AssertExtensions.Throws<ArgumentNullException>("outerKeySelector", () => outer.RightJoin(inner, null, e => e.name, new AnagramEqualityComparer()));
        }

        [Fact]
        public void InnerKeySelectorNull()
        {
            CustomerRec[] outer =
            [
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            ];
            AnagramRec[] inner =
            [
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            ];

            AssertExtensions.Throws<ArgumentNullException>("innerKeySelector", () => outer.RightJoin(inner, e => e.name, null, new AnagramEqualityComparer()));
        }

        [Fact]
        public void OuterNullNoComparer()
        {
            CustomerRec[] outer = null;
            AnagramRec[] inner =
            [
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            ];

            AssertExtensions.Throws<ArgumentNullException>("outer", () => outer.RightJoin(inner, e => e.name, e => e.name));
        }

        [Fact]
        public void InnerNullNoComparer()
        {
            CustomerRec[] outer =
            [
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            ];
            AnagramRec[] inner = null;

            AssertExtensions.Throws<ArgumentNullException>("inner", () => outer.RightJoin(inner, e => e.name, e => e.name));
        }

        [Fact]
        public void OuterKeySelectorNullNoComparer()
        {
            CustomerRec[] outer =
            [
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            ];
            AnagramRec[] inner =
            [
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            ];

            AssertExtensions.Throws<ArgumentNullException>("outerKeySelector", () => outer.RightJoin(inner, null, e => e.name));
        }

        [Fact]
        public void InnerKeySelectorNullNoComparer()
        {
            CustomerRec[] outer =
            [
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            ];
            AnagramRec[] inner =
            [
                new AnagramRec{ name = "miT", orderID = 43455, total = 10 },
                new AnagramRec{ name = "Prakash", orderID = 323232, total = 9 }
            ];

            AssertExtensions.Throws<ArgumentNullException>("innerKeySelector", () => outer.RightJoin(inner, e => e.name, null));
        }

        [Fact]
        public void NullElements()
        {
            string[] outer = [null, string.Empty];
            string[] inner = [null, string.Empty];
            (string?, string)[] expected = [(null, null), (string.Empty, string.Empty)];

            Assert.Equal(expected, outer.RightJoin(inner, e => e, e => e, EqualityComparer<string>.Default));
        }

        [Fact]
        public void OuterNonEmptyInnerEmpty()
        {
            CustomerRec[] outer =
            [
                new CustomerRec{ name = "Tim", custID = 43434 },
                new CustomerRec{ name = "Bob", custID = 34093 }
            ];
            OrderRec[] inner = [];
            Assert.Empty(outer.RightJoin(inner, e => e.custID, e => e.custID));
        }

        [Fact]
        public void SingleElementEachAndMatches()
        {
            CustomerRec[] outer = [new CustomerRec { name = "Prakash", custID = 98022 }];
            OrderRec[] inner = [new OrderRec { orderID = 45321, custID = 98022, total = 50 }];
            (CustomerRec?, OrderRec)[] expected =
            [
                (new CustomerRec { name = "Prakash", custID = 98022 }, new OrderRec { orderID = 45321, custID = 98022, total = 50 })
            ];

            Assert.Equal(expected, outer.RightJoin(inner, e => e.custID, e => e.custID));
        }

        [Fact]
        public void SingleElementEachAndDoesntMatch()
        {
            CustomerRec[] outer = [new CustomerRec { name = "Prakash", custID = 98922 }];
            OrderRec[] inner = [new OrderRec { orderID = 45321, custID = 98022, total = 50 }];
            (CustomerRec?, OrderRec)[] expected =
            [
                (null, new OrderRec{ orderID = 45321, custID = 98022, total = 50 })
            ];

            Assert.Equal(expected, outer.RightJoin(inner, e => e.custID, e => e.custID));
        }

        [Fact]
        public void SelectorsReturnNull()
        {
            int?[] outer = [null, null];
            int?[] inner = [null, null, null];
            (int?, int?)[] expected = [(null, null), (null, null), (null, null)];

            Assert.Equal(expected, outer.RightJoin(inner, e => e, e => e));
        }

        [Fact]
        public void InnerSameKeyMoreThanOneElementAndMatches()
        {
            CustomerRec[] outer =
            [
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            ];
            OrderRec[] inner =
            [
                new OrderRec{ orderID = 45321, custID = 98022, total = 50 },
                new OrderRec{ orderID = 45421, custID = 98022, total = 10 },
                new OrderRec{ orderID = 43421, custID = 99022, total = 20 },
                new OrderRec{ orderID = 85421, custID = 98022, total = 18 },
                new OrderRec{ orderID = 95421, custID = 99021, total = 9 }
            ];
            (CustomerRec?, OrderRec)[] expected =
            [
                (new CustomerRec{ name = "Prakash", custID = 98022 }, new OrderRec{ orderID = 45321, custID = 98022, total = 50 }),
                (new CustomerRec{ name = "Prakash", custID = 98022 }, new OrderRec{ orderID = 45421, custID = 98022, total = 10 }),
                (new CustomerRec{ name = "Robert", custID = 99022 }, new OrderRec{ orderID = 43421, custID = 99022, total = 20 }),
                (new CustomerRec{ name = "Prakash", custID = 98022 }, new OrderRec{ orderID = 85421, custID = 98022, total = 18 }),
                (new CustomerRec{ name = "Tim", custID = 99021 }, new OrderRec{ orderID = 95421, custID = 99021, total = 9 })
            ];

            Assert.Equal(expected, outer.RightJoin(inner, e => e.custID, e => e.custID));
        }

        [Fact]
        public void OuterSameKeyMoreThanOneElementAndMatches()
        {
            CustomerRec[] outer =
            [
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Bob", custID = 99022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            ];
            OrderRec[] inner =
            [
                new OrderRec{ orderID = 45321, custID = 98022, total = 50 },
                new OrderRec{ orderID = 43421, custID = 99022, total = 20 },
                new OrderRec{ orderID = 95421, custID = 99021, total = 9 }
            ];
            (CustomerRec?, OrderRec)[] expected =
            [
                (new CustomerRec{ name = "Prakash", custID = 98022 }, new OrderRec{ orderID = 45321, custID = 98022, total = 50 }),
                (new CustomerRec{ name = "Bob", custID = 99022 }, new OrderRec{ orderID = 43421, custID = 99022, total = 20 }),
                (new CustomerRec{ name = "Robert", custID = 99022 }, new OrderRec{ orderID = 43421, custID = 99022, total = 20 }),
                (new CustomerRec{ name = "Tim", custID = 99021 }, new OrderRec{ orderID = 95421, custID = 99021, total = 9 })
            ];

            Assert.Equal(expected, outer.RightJoin(inner, e => e.custID, e => e.custID));
        }

        [Fact]
        public void NoMatches()
        {
            CustomerRec[] outer =
            [
                new CustomerRec{ name = "Prakash", custID = 98022 },
                new CustomerRec{ name = "Bob", custID = 99022 },
                new CustomerRec{ name = "Tim", custID = 99021 },
                new CustomerRec{ name = "Robert", custID = 99022 }
            ];
            OrderRec[] inner =
            [
                new OrderRec{ orderID = 45321, custID = 18022, total = 50 },
                new OrderRec{ orderID = 43421, custID = 29022, total = 20 },
                new OrderRec{ orderID = 95421, custID = 39021, total = 9 }
            ];
            (CustomerRec?, OrderRec)[] expected =
            [
                (null, new OrderRec{ orderID = 45321, custID = 18022, total = 50 }),
                (null, new OrderRec{ orderID = 43421, custID = 29022, total = 20 }),
                (null, new OrderRec{ orderID = 95421, custID = 39021, total = 9 })
            ];

            Assert.Equal(expected, outer.RightJoin(inner, e => e.custID, e => e.custID));
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerate()
        {
            var iterator = NumberRangeGuaranteedNotCollectionType(0, 3).RightJoin(Enumerable.Empty<int>(), i => i, i => i);
            var en = iterator as IEnumerator<(int?, int)>;
            Assert.False(en is not null && en.MoveNext());
        }
    }
}
