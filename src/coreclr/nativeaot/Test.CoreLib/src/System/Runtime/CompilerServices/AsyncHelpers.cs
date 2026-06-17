// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.CompilerServices
{
    public static class AsyncHelpers
    {
        [Intrinsic]
        public static TResult Await<TResult>(System.Threading.Tasks.Task<TResult> task) => default!;
    }
}
