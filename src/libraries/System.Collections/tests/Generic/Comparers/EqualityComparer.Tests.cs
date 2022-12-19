// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Tests;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace System.Collections.Generic.Tests
{
    public class EqualityComparerTests
    {
        public class EqualsData<T> : TheoryData<T, T, bool>
        {
            public IEnumerable<T> Items
            {
                get
                {
                    return this.Select(array => array[0])
                        .Concat(this.Select(array => array[1]))
                        .Cast<T>();
                }
            }
        }

        public class HashData<T> : TheoryData<T, int> { }

        [Theory]
        [MemberData(nameof(ByteData))]
        [MemberData(nameof(Int32Data))]
        [MemberData(nameof(StringData))]
        [MemberData(nameof(IEquatableData))]
        [MemberData(nameof(Int16EnumData))]
        [MemberData(nameof(SByteEnumData))]
        [MemberData(nameof(Int32EnumData))]
        [MemberData(nameof(NullableInt32EnumData))]
        [MemberData(nameof(Int64EnumData))]
        [MemberData(nameof(NonEquatableValueTypeData))]
        [MemberData(nameof(NullableNonEquatableValueTypeData))]
        [MemberData(nameof(ObjectData))]
        public void EqualsTest<T>(T left, T right, bool expected)
        {
            var comparer = EqualityComparer<T>.Default;
            AssertEqualsGeneric(comparer, left, right, expected);
            AssertEqualsNonGeneric(comparer, left, right, expected);
        }

        private void AssertEqualsGeneric<T>(IEqualityComparer<T> comparer, T left, T right, bool expected)
        {
            Assert.Equal(expected, comparer.Equals(left, right));
            Assert.Equal(expected, comparer.Equals(right, left)); // Should be commutative.

            Assert.True(comparer.Equals(left, left)); // Should be reflexive.
            Assert.True(comparer.Equals(right, right));

            // All comparers returned by EqualityComparer<T>.Default should be
            // able to handle nulls before dispatching to IEquatable<T>.Equals()
            if (default(T) == null)
            {
                T nil = default(T);

                Assert.True(comparer.Equals(nil, nil));

                Assert.Equal(left == null, comparer.Equals(left, nil));
                Assert.Equal(left == null, comparer.Equals(nil, left));

                Assert.Equal(right == null, comparer.Equals(right, nil));
                Assert.Equal(right == null, comparer.Equals(nil, right));
            }

            // GetHashCode: If 2 objects are equal, then their hash code should be the same.

            if (expected)
            {
                int hash = comparer.GetHashCode(left);

                Assert.Equal(hash, comparer.GetHashCode(left)); // Should return the same result across multiple invocations
                Assert.Equal(hash, comparer.GetHashCode(right));
            }
        }

        private void AssertEqualsNonGeneric<T>(IEqualityComparer nonGenericComparer, T left, T right, bool expected)
        {
            // If both sides are Ts then the explicit implementation of
            // IEqualityComparer.Equals should also succeed, with the same results
            Assert.Equal(expected, nonGenericComparer.Equals(left, right));
            Assert.Equal(expected, nonGenericComparer.Equals(right, left));

            Assert.True(nonGenericComparer.Equals(left, left));
            Assert.True(nonGenericComparer.Equals(right, right));

            // All comparers returned by EqualityComparer<T>.Default should be
            // able to handle nulls before dispatching to IEquatable<T>.Equals()
            if (default(T) == null)
            {
                T nil = default(T);

                Assert.True(nonGenericComparer.Equals(nil, nil));

                Assert.Equal(left == null, nonGenericComparer.Equals(left, nil));
                Assert.Equal(left == null, nonGenericComparer.Equals(nil, left));

                Assert.Equal(right == null, nonGenericComparer.Equals(right, nil));
                Assert.Equal(right == null, nonGenericComparer.Equals(nil, right));
            }

            // GetHashCode: If 2 objects are equal, then their hash code should be the same.

            if (expected)
            {
                int hash = nonGenericComparer.GetHashCode(left);

                Assert.Equal(hash, nonGenericComparer.GetHashCode(left)); // Should return the same result across multiple invocations
                Assert.Equal(hash, nonGenericComparer.GetHashCode(right));
            }
        }

        // xUnit has problems with nullable type inference since a T? is
        // boxed to just a plain T. So we have separate theories for nullables.

        [Theory]
        [MemberData(nameof(ByteData))]
        [MemberData(nameof(Int32Data))]
        [MemberData(nameof(Int16EnumData))]
        [MemberData(nameof(SByteEnumData))]
        [MemberData(nameof(Int32EnumData))]
        [MemberData(nameof(Int64EnumData))]
        [MemberData(nameof(NonEquatableValueTypeData))]
        public void NullableEquals<T>(T left, T right, bool expected) where T : struct
        {
            var comparer = EqualityComparer<T?>.Default;
            IEqualityComparer nonGenericComparer = comparer;

            // The following code may look similar to what we have in the other theory.
            // The difference is that we're using EqualityComparer<T?> instead of EqualityComparer<T>
            // and the inputs are being implicitly converted to nullables.

            Assert.Equal(expected, comparer.Equals(left, right));
            Assert.Equal(expected, comparer.Equals(right, left));

            Assert.Equal(expected, nonGenericComparer.Equals(left, right));
            Assert.Equal(expected, nonGenericComparer.Equals(right, left));

            Assert.True(comparer.Equals(left, left));
            Assert.True(comparer.Equals(right, right));

            Assert.True(nonGenericComparer.Equals(left, left));
            Assert.True(nonGenericComparer.Equals(right, right));

            Assert.True(comparer.Equals(default(T), default(T)));

            Assert.True(nonGenericComparer.Equals(default(T), default(T)));

            // EqualityComparer<T?> should check for HasValue before dispatching
            // to IEquatable<T>.Equals().
            Assert.True(comparer.Equals(null, null));

            // A non-null nullable should never be equal to a null one.
            Assert.False(comparer.Equals(left, null));
            Assert.False(comparer.Equals(null, left));

            Assert.False(comparer.Equals(right, null));
            Assert.False(comparer.Equals(null, right));

            // Even if the underlying value is a default value.
            Assert.False(comparer.Equals(default(T), null));
            Assert.False(comparer.Equals(null, default(T)));

            // These should hold true for the non-generic comparer as well.
            Assert.True(nonGenericComparer.Equals(null, null));

            Assert.False(nonGenericComparer.Equals(left, null));
            Assert.False(nonGenericComparer.Equals(null, left));

            Assert.False(nonGenericComparer.Equals(right, null));
            Assert.False(nonGenericComparer.Equals(null, right));

            Assert.False(nonGenericComparer.Equals(default(T), null));
            Assert.False(nonGenericComparer.Equals(null, default(T)));

            // GetHashCode: If 2 objects are equal, then their hash code should be the same.

            if (expected)
            {
                int hash = comparer.GetHashCode(left);

                Assert.Equal(hash, comparer.GetHashCode(left)); // Should return the same result across multiple invocations
                Assert.Equal(hash, comparer.GetHashCode(right));

                Assert.Equal(hash, nonGenericComparer.GetHashCode(left));
                Assert.Equal(hash, nonGenericComparer.GetHashCode(right));
            }
        }

        public static EqualsData<byte> ByteData()
        {
            return new EqualsData<byte>
            {
                { 3, 3, true },
                { 3, 4, false },
                { 0, 255, false },
                { 0, 128, false },
                { 255, 255, true }
            };
        }

        public static EqualsData<int> Int32Data()
        {
            return new EqualsData<int>
            {
                { 3, 3, true },
                { 3, 5, false },
                { int.MinValue + 1, 1, false },
                { int.MinValue, int.MinValue, true }
            };
        }

        public static EqualsData<string> StringData()
        {
            return new EqualsData<string>
            {
                { "foo", "foo", true },
                { string.Empty, null, false },
                { "bar", new string("bar".ToCharArray()), true },
                { "foo", "bar", false }
            };
        }

        public static EqualsData<string> StringData_IgnoreCase()
        {
            return new EqualsData<string>
            {
                { "foo", "foo", true },
                { string.Empty, null, false },
                { "bar", new string("bar".ToCharArray()), true },
                { "foo", "bar", false },
                { "foo", "Foo", true },
                { "foo", "FOO", true },
                { "foo", "Bar", false },
                { "foo", "BAR", false },
                { "bar", new string("BAR".ToCharArray()), true }
            };
        }

        public static EqualsData<Equatable> IEquatableData()
        {
            var one = new Equatable(1);

            return new EqualsData<Equatable>
            {
                { one, one, true },
                { one, new Equatable(1), true },
                { new Equatable(int.MinValue + 1), new Equatable(1), false },
                { new Equatable(-1), new Equatable(int.MaxValue), false }
            };
        }

        public static EqualsData<Int16Enum> Int16EnumData()
        {
            return new EqualsData<Int16Enum>
            {
                { (Int16Enum)(-2), (Int16Enum)(-4), false }, // Negative shorts hash specially.
                { Int16Enum.Two, Int16Enum.Two, true },
                { Int16Enum.Min, Int16Enum.Max, false },
                { Int16Enum.Min, Int16Enum.Min, true },
                { Int16Enum.One, Int16Enum.Min + 1, false }
            };
        }

        public static EqualsData<SByteEnum> SByteEnumData()
        {
            return new EqualsData<SByteEnum>
            {
                { (SByteEnum)(-2), (SByteEnum)(-4), false }, // Negative sbytes hash specially.
                { SByteEnum.Two, SByteEnum.Two, true },
                { SByteEnum.Min, SByteEnum.Max, false },
                { SByteEnum.Min, SByteEnum.Min, true },
                { SByteEnum.One, SByteEnum.Min + 1, false }
            };
        }

        public static EqualsData<Int32Enum> Int32EnumData()
        {
            return new EqualsData<Int32Enum>
            {
                { (Int32Enum)(-2), (Int32Enum)(-4), false },
                { Int32Enum.Two, Int32Enum.Two, true },
                { Int32Enum.Min, Int32Enum.Max, false },
                { Int32Enum.Min, Int32Enum.Min, true },
                { Int32Enum.One, Int32Enum.Min + 1, false }
            };
        }

        public static EqualsData<Int32Enum?> NullableInt32EnumData()
        {
            return new EqualsData<Int32Enum?>
            {
                { (Int32Enum)(-2), (Int32Enum)(-4), false },
                { Int32Enum.Two, Int32Enum.Two, true },
                { Int32Enum.Min, Int32Enum.Max, false },
                { Int32Enum.Min, Int32Enum.Min, true },
                { Int32Enum.One, Int32Enum.Min + 1, false },
                { (Int32Enum)(-2), null, false },
                { Int32Enum.Two, null, false },
                { null, Int32Enum.Max, false },
                { null, Int32Enum.Min + 1, false }
            };
        }

        public static EqualsData<Int64Enum> Int64EnumData()
        {
            return new EqualsData<Int64Enum>
            {
                { (Int64Enum)(-2), (Int64Enum)(-4), false },
                { Int64Enum.Two, Int64Enum.Two, true },
                { Int64Enum.Min, Int64Enum.Max, false },
                { Int64Enum.Min, Int64Enum.Min, true },
                { Int64Enum.One, Int64Enum.Min + 1, false }
            };
        }

        public static EqualsData<NonEquatableValueType> NonEquatableValueTypeData()
        {
            // Comparisons for structs that do not override ValueType.Equals or
            // ValueType.GetHashCode should still work as expected.

            var one = new NonEquatableValueType { Value = 1 };

            return new EqualsData<NonEquatableValueType>
            {
                { new NonEquatableValueType(), new NonEquatableValueType(), true },
                { one, one, true },
                { new NonEquatableValueType(-1), new NonEquatableValueType(), false },
                { new NonEquatableValueType(2), new NonEquatableValueType(2), true }
            };
        }

        public static EqualsData<NonEquatableValueType?> NullableNonEquatableValueTypeData()
        {
            // Comparisons for structs that do not override ValueType.Equals or
            // ValueType.GetHashCode should still work as expected.

            var one = new NonEquatableValueType { Value = 1 };

            return new EqualsData<NonEquatableValueType?>
            {
                { new NonEquatableValueType(), new NonEquatableValueType(), true },
                { one, one, true },
                { new NonEquatableValueType(-1), new NonEquatableValueType(), false },
                { new NonEquatableValueType(2), new NonEquatableValueType(2), true },
                { new NonEquatableValueType(-1), null, false },
                { null, new NonEquatableValueType(2), false }
            };
        }

        public static EqualsData<object> ObjectData()
        {
            var obj = new object();

            return new EqualsData<object>
            {
                { obj, obj, true },
                { obj, new object(), false },
                { obj, null, false }
            };
        }

        [Theory]
        [MemberData(nameof(ByteHashData))]
        [MemberData(nameof(Int32HashData))]
        [MemberData(nameof(StringHashData))]
        [MemberData(nameof(IEquatableHashData))]
        [MemberData(nameof(Int16EnumHashData))]
        [MemberData(nameof(SByteEnumHashData))]
        [MemberData(nameof(Int32EnumHashData))]
        [MemberData(nameof(Int64EnumHashData))]
        [MemberData(nameof(NonEquatableValueTypeHashData))]
        [MemberData(nameof(ObjectHashData))]
        public void GetHashCodeTest<T>(T value, int expected)
        {
            var comparer = EqualityComparer<T>.Default;
            IEqualityComparer nonGenericComparer = comparer;

            Assert.Equal(expected, comparer.GetHashCode(value));
            Assert.Equal(expected, comparer.GetHashCode(value)); // Should return the same result across multiple invocations

            Assert.Equal(expected, nonGenericComparer.GetHashCode(value));
            Assert.Equal(expected, nonGenericComparer.GetHashCode(value));

            // We should deal with nulls before dispatching to object.GetHashCode.
            if (default(T) == null)
            {
                T nil = default(T);

                Assert.Equal(0, comparer.GetHashCode(nil));

                Assert.Equal(0, nonGenericComparer.GetHashCode(nil));
            }
        }

        [Theory]
        [MemberData(nameof(ByteHashData))]
        [MemberData(nameof(Int32HashData))]
        [MemberData(nameof(Int16EnumHashData))]
        [MemberData(nameof(SByteEnumHashData))]
        [MemberData(nameof(Int32EnumHashData))]
        [MemberData(nameof(Int64EnumHashData))]
        [MemberData(nameof(NonEquatableValueTypeHashData))]
        public void NullableGetHashCode<T>(T value, int expected) where T : struct
        {
            var comparer = EqualityComparer<T?>.Default;
            IEqualityComparer nonGenericComparer = comparer;

            Assert.Equal(expected, comparer.GetHashCode(value));
            Assert.Equal(expected, comparer.GetHashCode(value));

            Assert.Equal(expected, nonGenericComparer.GetHashCode(value));
            Assert.Equal(expected, nonGenericComparer.GetHashCode(value));

            Assert.Equal(0, comparer.GetHashCode(null));

            Assert.Equal(0, nonGenericComparer.GetHashCode(null));
        }

        public static HashData<byte> ByteHashData() => GenerateHashData(ByteData());

        public static HashData<int> Int32HashData() => GenerateHashData(Int32Data());

        public static HashData<string> StringHashData() => GenerateHashData(StringData());

        public static HashData<Equatable> IEquatableHashData() => GenerateHashData(IEquatableData());

        public static HashData<Int16Enum> Int16EnumHashData() => GenerateHashData(Int16EnumData());

        public static HashData<SByteEnum> SByteEnumHashData() => GenerateHashData(SByteEnumData());

        public static HashData<Int32Enum> Int32EnumHashData() => GenerateHashData(Int32EnumData());

        public static HashData<Int64Enum> Int64EnumHashData() => GenerateHashData(Int64EnumData());

        public static HashData<NonEquatableValueType> NonEquatableValueTypeHashData() => GenerateHashData(NonEquatableValueTypeData());

        public static HashData<object> ObjectHashData() => GenerateHashData(ObjectData());

        [Fact]
        public void TryCallLeftHandEqualsFirst()
        {
            // Given two non-null inputs x and y, Comparer<T>.Equals should try
            // to call x.Equals() first. y.Equals() should only be called if x
            // is null and y is not.

            var comparer = EqualityComparer<DelegateEquatable>.Default;

            int state1 = 0, state2 = 0;

            var left = new DelegateEquatable { EqualsWorker = _ => { state1++; return false; } };
            var right = new DelegateEquatable { EqualsWorker = _ => { state2++; return true; } };

            Assert.False(comparer.Equals(left, right));
            Assert.Equal(1, state1);
            Assert.Equal(0, state2);

            Assert.True(comparer.Equals(right, left));
            Assert.Equal(1, state1);
            Assert.Equal(1, state2);
        }

        [Fact]
        public void NullableTryCallLeftHandEqualsFirst()
        {
            // Comparer<T>.Default is specialized when T is a nullable of a type that implements IEquatable.

            var comparer = EqualityComparer<ValueDelegateEquatable?>.Default;

            int state1 = 0, state2 = 0;

            var left = new ValueDelegateEquatable { EqualsWorker = _ => { state1++; return false; } };
            var right = new ValueDelegateEquatable { EqualsWorker = _ => { state2++; return true; } };

            Assert.False(comparer.Equals(left, right));
            Assert.Equal(1, state1);
            Assert.Equal(0, state2);

            Assert.True(comparer.Equals(right, left));
            Assert.Equal(1, state1);
            Assert.Equal(1, state2);
        }

        private static HashData<T> GenerateHashData<T>(EqualsData<T> input)
        {
            Debug.Assert(input != null);

            var result = new HashData<T>();

            foreach (T item in input.Items)
            {
                result.Add(item, item?.GetHashCode() ?? 0);
            }

            return result;
        }

        [Fact]
        public void EqualityComparerCreate_InvalidArguments_Throws()
        {
            AssertExtensions.Throws<ArgumentNullException>("equals", () => EqualityComparer<int>.Create(null));
            AssertExtensions.Throws<ArgumentNullException>("equals", () => EqualityComparer<string>.Create(null, null));
            EqualityComparer<int>.Create((x, y) => x == y); // no exception
            EqualityComparer<int>.Create((x, y) => x == y, null); // no exception
        }

        [Fact]
        public void EqualityComparerCreate_DelegatesUsed()
        {
            int equalsCalls = 0;
            EqualityComparer<int> ec;

            ec = EqualityComparer<int>.Create(
                (x, y) =>
                {
                    equalsCalls++;
                    return x == y * 2;
                });
            Assert.Throws<NotSupportedException>(() => ec.GetHashCode(42));
            Assert.True(ec.Equals(2, 1));
            Assert.False(ec.Equals(2, 2));
            Assert.False(ec.Equals(1, 2));
            Assert.True(ec.Equals(6, 3));
            Assert.Equal(4, equalsCalls);

            ec = EqualityComparer<int>.Create((x, y) => x == y, null);
            Assert.Throws<NotSupportedException>(() => ec.GetHashCode(42));

            int getHashCodeCalls = 0;
            equalsCalls = 0;
            ec = EqualityComparer<int>.Create(
                (x, y) =>
                {
                    equalsCalls++;
                    return x * 2 == y;
                },
                x =>
                {
                    getHashCodeCalls++;
                    return x * 2;
                });
            Assert.True(ec.Equals(1, 2));
            Assert.False(ec.Equals(2, 2));
            Assert.False(ec.Equals(2, 1));
            Assert.True(ec.Equals(3, 6));
            Assert.Equal(6, ec.GetHashCode(3));
            Assert.Equal(8, ec.GetHashCode(4));
            Assert.Equal(4, equalsCalls);
            Assert.Equal(2, getHashCodeCalls);
        }

        [Fact]
        public void EqualityComparerCreate_ArgsNotDereferenced()
        {
            EqualityComparer<string> ec = EqualityComparer<string>.Create((x, y) => true, x => 0);
            Assert.True(ec.Equals(null, null));
            Assert.Equal(0, ec.GetHashCode(null));
        }

        [Fact]
        public void EqualityComparerCreate_EqualsGetHashCodeOverridden()
        {
            Func<int, int, bool> equals1 = (x, y) => x == y;
            Func<int, int, bool> equals2 = (x, y) => x == y;
            Func<int, int> getHashCode1 = x => x;
            Func<int, int> getHashCode2 = x => x;

            EqualityComparer<int> ec = EqualityComparer<int>.Create(equals1, getHashCode1);
            Assert.True(ec.Equals(ec));
            Assert.Equal(ec.GetHashCode(), ec.GetHashCode());

            Assert.True(EqualityComparer<int>.Create(equals1).Equals(EqualityComparer<int>.Create(equals1)));
            Assert.True(EqualityComparer<int>.Create(equals1, getHashCode1).Equals(EqualityComparer<int>.Create(equals1, getHashCode1)));
            Assert.Equal(EqualityComparer<int>.Create(equals1).GetHashCode(), EqualityComparer<int>.Create(equals1).GetHashCode());
            Assert.Equal(EqualityComparer<int>.Create(equals1, getHashCode1).GetHashCode(), EqualityComparer<int>.Create(equals1, getHashCode1).GetHashCode());

            Assert.False(EqualityComparer<int>.Create(equals1).Equals(EqualityComparer<int>.Create(equals2)));
            Assert.False(EqualityComparer<int>.Create(equals1).Equals(EqualityComparer<int>.Create(equals1, getHashCode1)));
            Assert.False(EqualityComparer<int>.Create(equals1, getHashCode1).Equals(EqualityComparer<int>.Create(equals1, getHashCode2)));
            Assert.False(EqualityComparer<int>.Create(equals1, getHashCode1).Equals(EqualityComparer<int>.Create(equals2, getHashCode1)));
        }

        [Theory]
        [MemberData(nameof(StringData))]
        public void EqualityComparer_CreateEnumerableComparer_EqualsTest(string left, string right, bool expected)
        {
            var comparer = EqualityComparer.CreateEnumerableComparer<string, char>();
            AssertEqualsGeneric(comparer, left, right, expected);
        }

        [Theory]
        [MemberData(nameof(StringData_IgnoreCase))]
        public void EqualityComparer_CreateEnumerableComparer_EqualsIgnoreCaseTest(string left, string right, bool expected)
        {
            var comparer = EqualityComparer.CreateEnumerableComparer<string, char>(IgnoreCaseEqualityComparer());
            AssertEqualsGeneric(comparer, left, right, expected);
        }

        [Fact]
        public void EqualityComparer_CreateEnumerableComparer_EqualsGetHashCodeOverridden()
        {
            var comparer = EqualityComparer.CreateEnumerableComparer<string, char>();
            Assert.True(comparer.Equals(comparer));
            Assert.Equal(comparer.GetHashCode(), comparer.GetHashCode());

            var ec1 = EqualityComparer<char>.Create((x, y) => x == y);
            var ec2 = EqualityComparer<char>.Create((x, y) => x == y);

            Assert.True(EqualityComparer.CreateEnumerableComparer<string, char>(ec1).Equals(EqualityComparer.CreateEnumerableComparer<string, char>(ec1)));
            Assert.True(EqualityComparer.CreateEnumerableComparer<string, char>(ec1).GetHashCode().Equals(EqualityComparer.CreateEnumerableComparer<string, char>(ec1).GetHashCode()));

            Assert.False(EqualityComparer.CreateEnumerableComparer<string, char>(ec1).Equals(EqualityComparer.CreateEnumerableComparer<string, char>(ec2)));
        }

        [Theory]
        [MemberData(nameof(StringData))]
        public void EqualityComparer_CreateSetComparer_EqualsTest(string left, string right, bool expected)
        {
            var comparer = EqualityComparer.CreateSetComparer<HashSet<char>, char>();
            AssertEqualsGeneric(comparer, left?.ToHashSet(), right?.ToHashSet(), expected);
        }

        [Theory]
        [MemberData(nameof(StringData))]
        public void EqualityComparer_CreateSetComparer_EqualsIgnoreCaseTest(string left, string right, bool expected)
        {
            var comparer = EqualityComparer.CreateSetComparer<HashSet<char>, char>(IgnoreCaseEqualityComparer());
            AssertEqualsGeneric(comparer, left?.ToHashSet(), right?.ToHashSet(), expected);
        }

        [Theory]
        [MemberData(nameof(StringData))]
        public void EqualityComparer_CreateSetComparer_SortedSet_EqualsTest(string left, string right, bool expected)
        {
            var comparer = EqualityComparer.CreateSetComparer<SortedSet<char>, char>();
            AssertEqualsGeneric(comparer, ToSortedSet(left), ToSortedSet(right), expected);
        }

        [Theory]
        [MemberData(nameof(StringData))]
        public void EqualityComparer_CreateSetComparer_SortedSet_EqualsIgnoreCaseTest(string left, string right, bool expected)
        {
            var comparer = EqualityComparer.CreateSetComparer<SortedSet<char>, char>(IgnoreCaseEqualityComparer());
            AssertEqualsGeneric(comparer, ToSortedSet(left), ToSortedSet(right), expected);
        }

        [Fact]
        public void EqualityComparer_CreateSetComparer_EqualsGetHashCodeOverridden()
        {
            var comparer = EqualityComparer.CreateSetComparer<HashSet<char>, char>();
            Assert.True(comparer.Equals(comparer));
            Assert.Equal(comparer.GetHashCode(), comparer.GetHashCode());

            var ec1 = EqualityComparer<char>.Create((x, y) => x == y);
            var ec2 = EqualityComparer<char>.Create((x, y) => x == y);

            Assert.True(EqualityComparer.CreateSetComparer<HashSet<char>, char>(ec1).Equals(EqualityComparer.CreateSetComparer<HashSet<char>, char>(ec1)));
            Assert.True(EqualityComparer.CreateSetComparer<HashSet<char>, char>(ec1).GetHashCode().Equals(EqualityComparer.CreateSetComparer<HashSet<char>, char>(ec1).GetHashCode()));

            Assert.False(EqualityComparer.CreateSetComparer<HashSet<char>, char>(ec1).Equals(EqualityComparer.CreateSetComparer<HashSet<char>, char>(ec2)));
        }

        [Theory]
        [MemberData(nameof(StringData))]
        public void EqualityComparer_CreateDictionaryComparer_EqualsTest(string left, string right, bool expected)
        {
            var comparer = EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>();
            AssertEqualsGeneric(comparer, ToIndexedDictionary(left), ToIndexedDictionary(right), expected);
        }

        [Theory]
        [MemberData(nameof(StringData))]
        public void EqualityComparer_CreateDictionaryComparer_EqualsIgnoreCaseTest(string left, string right, bool expected)
        {
            var comparer = EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(valueComparer: IgnoreCaseEqualityComparer());
            AssertEqualsGeneric(comparer, ToIndexedDictionary(left), ToIndexedDictionary(right), expected);
        }

        [Fact]
        public void EqualityComparer_CreateDictionaryComparer_EqualsGetHashCodeOverridden()
        {
            var comparer = EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>();
            Assert.True(comparer.Equals(comparer));
            Assert.Equal(comparer.GetHashCode(), comparer.GetHashCode());

            var ec11 = EqualityComparer<int>.Create((x, y) => x == y);
            var ec12 = EqualityComparer<int>.Create((x, y) => x == y);
            var ec21 = EqualityComparer<char>.Create((x, y) => x == y);
            var ec22 = EqualityComparer<char>.Create((x, y) => x == y);

            Assert.True(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec11).Equals(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec11)));
            Assert.True(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(null, ec21).Equals(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(null, ec21)));
            Assert.True(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec11, ec21).Equals(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec11, ec21)));
            Assert.True(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec11).GetHashCode().Equals(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec11).GetHashCode()));
            Assert.True(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(null, ec21).GetHashCode().Equals(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(null, ec21).GetHashCode()));
            Assert.True(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec11, ec21).GetHashCode().Equals(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec11, ec21).GetHashCode()));

            Assert.False(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec11).Equals(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec12)));
            Assert.False(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(null, ec21).Equals(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(null, ec22)));
            Assert.False(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec11, ec21).Equals(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec11, ec22)));
            Assert.False(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec11, ec21).Equals(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec12, ec21)));
            Assert.False(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec11, ec21).Equals(EqualityComparer.CreateDictionaryComparer<Dictionary<int, char>, int, char>(ec12, ec22)));
        }

        private SortedSet<char> ToSortedSet(string left) => left is not null ? new SortedSet<char>(left) : null;

        private static Dictionary<int, char> ToIndexedDictionary(string value) => value?
            .Select((value, index) => (value, index))
            .ToDictionary(t => t.index, t => t.value);

        private static EqualityComparer<char> IgnoreCaseEqualityComparer() =>
            EqualityComparer<char>.Create((x, y) => char.ToUpperInvariant(x).Equals(char.ToUpperInvariant(y)));
    }
}
