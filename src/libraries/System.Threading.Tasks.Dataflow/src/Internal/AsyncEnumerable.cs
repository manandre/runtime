// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// AsyncEnumerable.cs
//
//
//
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System.Collections.Generic;

namespace System.Threading.Tasks.Dataflow
{
    internal static class AsyncEnumerable
    {
        // REVIEW: This type of blocking is an anti-pattern. We may want to move it to System.Interactive.Async
        //         and remove it from System.Linq.Async API surface.

        internal static IEnumerable<TSource> ToEnumerable<TSource>(this IAsyncEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return Core(source);

            static IEnumerable<TSource> Core(IAsyncEnumerable<TSource> source)
            {
                var e = source.GetAsyncEnumerator(default);

                try
                {
                    while (true)
                    {
                        if (!Wait(e.MoveNextAsync()))
                            break;

                        yield return e.Current;
                    }
                }
                finally
                {
                    Wait(e.DisposeAsync());
                }
            }
        }

        // NB: ValueTask and ValueTask<T> do not have to support blocking on a call to GetResult when backed by
        //     an IValueTaskSource or IValueTaskSource<T> implementation. Convert to a Task or Task<T> to do so
        //     in case the task hasn't completed yet.

        private static void Wait(ValueTask task)
        {
            var awaiter = task.GetAwaiter();

            if (!awaiter.IsCompleted)
            {
                task.AsTask().GetAwaiter().GetResult();
                return;
            }

            awaiter.GetResult();
        }

        private static T Wait<T>(ValueTask<T> task)
        {
            var awaiter = task.GetAwaiter();

            if (!awaiter.IsCompleted)
            {
                return task.AsTask().GetAwaiter().GetResult();
            }

            return awaiter.GetResult();
        }
    }
}