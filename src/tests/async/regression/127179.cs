// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

// Regression test for https://github.com/dotnet/runtime/issues/127179:
// Awaiting a non-runtime-async Task-returning generic method from a runtime-async
// generic caller caused an InvalidOperationException at NativeAOT compile time.
// The scanner incorrectly used the async variant in the precomputed generic dictionary
// while the JIT reverted to the original method, creating a mismatch.
// This test also guards that the method.IsAsync check doesn't break non-NativeAOT scenarios.

public class Runtime_127179
{
    [Fact]
    public static void TestEntryPoint()
    {
        Assert.Equal(42, AsyncSharedGenericCaller(42).GetAwaiter().GetResult());
        Assert.Equal("hello", AsyncSharedGenericCaller("hello").GetAwaiter().GetResult());
    }

    // Non-runtime-async generic method; JIT reverts to calling this
    // directly rather than going through the async variant.
    [RuntimeAsyncMethodGeneration(false)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static async Task<TM> NonRuntimeAsyncCallee<TM>(TM value)
    {
        await Task.Yield();
        return value;
    }

    // Runtime-async generic caller in a shared generic context (T = __Canon).
    // Calling NonRuntimeAsyncCallee<T> requires a MethodDictionary lookup for T.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static async Task<T> AsyncSharedGenericCaller<T>(T value)
    {
        return await NonRuntimeAsyncCallee(value).ConfigureAwait(false);
    }
}
