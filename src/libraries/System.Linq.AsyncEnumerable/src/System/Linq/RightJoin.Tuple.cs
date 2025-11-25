// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq
{
    public static partial class AsyncEnumerable
    {
        /// <summary>Correlates the elements of two sequences based on matching keys.</summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to use to hash and compare keys.</param>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <returns>An <see cref="IAsyncEnumerable{T}" /> that has elements of type <c>(<typeparamref name="TOuter"/>, <typeparamref name="TInner"/>)</c> that are obtained by performing a right outer join on two sequences.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="outer" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="inner" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="outerKeySelector" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="innerKeySelector" /> is <see langword="null" />.</exception>
        public static IAsyncEnumerable<(TOuter? Outer, TInner Inner)> RightJoin<TOuter, TInner, TKey>(
            this IAsyncEnumerable<TOuter> outer,
            IAsyncEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            IEqualityComparer<TKey>? comparer = null)
        {
            ArgumentNullException.ThrowIfNull(outer);
            ArgumentNullException.ThrowIfNull(inner);
            ArgumentNullException.ThrowIfNull(outerKeySelector);
            ArgumentNullException.ThrowIfNull(innerKeySelector);

            return
                inner.IsKnownEmpty() ? Empty<(TOuter?, TInner)>() :
                Impl(outer, inner, outerKeySelector, innerKeySelector, comparer, default);

            static async IAsyncEnumerable<(TOuter?, TInner)> Impl(
                IAsyncEnumerable<TOuter> outer,
                IAsyncEnumerable<TInner> inner,
                Func<TOuter, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                IEqualityComparer<TKey>? comparer,
                [EnumeratorCancellation] CancellationToken cancellationToken)
            {
                AsyncLookup<TKey, TOuter> lookup = await AsyncLookup<TKey, TOuter>.CreateForJoinAsync(outer, outerKeySelector, comparer, cancellationToken);

                await foreach (var innerElement in inner.WithCancellation(cancellationToken))
                {
                    TKey key = innerKeySelector(innerElement);
                    Grouping<TKey, TOuter>? g = lookup.GetGrouping(key, create: false);
                    if (g is not null)
                    {
                        int count = g._count;
                        TOuter[] elements = g._elements;
                        for (int i = 0; i != count; ++i)
                        {
                            yield return (elements[i], innerElement);
                        }
                    }
                    else
                    {
                        yield return (default, innerElement);
                    }
                }
            }
        }

        /// <summary>Correlates the elements of two sequences based on matching keys.</summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to use to hash and compare keys.</param>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <returns>An <see cref="IAsyncEnumerable{T}" /> that has elements of type <c>(<typeparamref name="TOuter"/>, <typeparamref name="TInner"/>)</c> that are obtained by performing a right outer join on two sequences.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="outer" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="inner" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="outerKeySelector" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="innerKeySelector" /> is <see langword="null" />.</exception>
        public static IAsyncEnumerable<(TOuter? Outer, TInner Inner)> RightJoin<TOuter, TInner, TKey>(
            this IAsyncEnumerable<TOuter> outer,
            IAsyncEnumerable<TInner> inner,
            Func<TOuter, CancellationToken, ValueTask<TKey>> outerKeySelector,
            Func<TInner, CancellationToken, ValueTask<TKey>> innerKeySelector,
            IEqualityComparer<TKey>? comparer = null)
        {
            ArgumentNullException.ThrowIfNull(outer);
            ArgumentNullException.ThrowIfNull(inner);
            ArgumentNullException.ThrowIfNull(outerKeySelector);
            ArgumentNullException.ThrowIfNull(innerKeySelector);

            return
                inner.IsKnownEmpty() ? Empty<(TOuter?, TInner)>() :
                Impl(outer, inner, outerKeySelector, innerKeySelector, comparer, default);

            static async IAsyncEnumerable<(TOuter?, TInner)> Impl(
                IAsyncEnumerable<TOuter> outer,
                IAsyncEnumerable<TInner> inner,
                Func<TOuter, CancellationToken, ValueTask<TKey>> outerKeySelector,
                Func<TInner, CancellationToken, ValueTask<TKey>> innerKeySelector,
                IEqualityComparer<TKey>? comparer,
                [EnumeratorCancellation] CancellationToken cancellationToken)
            {
                AsyncLookup<TKey, TOuter> lookup = await AsyncLookup<TKey, TOuter>.CreateForJoinAsync(outer, outerKeySelector, comparer, cancellationToken);

                await foreach (var innerElement in inner.WithCancellation(cancellationToken))
                {
                    TKey key = await innerKeySelector(innerElement, cancellationToken);
                    Grouping<TKey, TOuter>? g = lookup.GetGrouping(key, create: false);
                    if (g is not null)
                    {
                        int count = g._count;
                        TOuter[] elements = g._elements;
                        for (int i = 0; i != count; ++i)
                        {
                            yield return (elements[i], innerElement);
                        }
                    }
                    else
                    {
                        yield return (default, innerElement);
                    }
                }
            }
        }
    }
}
