// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Regression test for https://github.com/dotnet/runtime/issues/127179
//
// A runtime-async method awaiting a non-runtime-async Task-returning generic method
// in a shared-generic context (T = __Canon) causes NativeAOT to throw
// System.InvalidOperationException at compile time WITHOUT the fix.
//
// Root cause: the ILC scanner incorrectly precomputes
//   MethodDictionary(asyncVariant(Callee<__Canon>))
// in the precomputed generic dictionary, while the JIT falls back to looking up
//   MethodDictionary(original Callee<__Canon>)
// because getAsyncOtherVariant detects that the async variant is a thunk.
//
// The callee must have RequiresInstMethodDescArg() == true so that a MethodDictionary
// lookup is emitted. Using "new T[]" triggers this, because array creation requires
// knowing the exact element type at runtime.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

class Program
{
    static int Main()
    {
        // Invoke with a reference type so T = __Canon is the shared canonical form
        string result1 = AsyncSharedGenericCaller("hello").GetAwaiter().GetResult();
        if (result1 != "hello-done")
        {
            Console.WriteLine($"FAIL: Expected 'hello-done', got '{result1}'");
            return 1;
        }

        object result2 = AsyncSharedGenericCaller(new object()).GetAwaiter().GetResult();
        if (result2 is null)
        {
            Console.WriteLine("FAIL: result2 should not be null");
            return 1;
        }

        Console.WriteLine("PASS");
        return 100;
    }

    // Non-runtime-async Task-returning generic method.
    // "new T[1]" forces RequiresInstMethodDescArg() = true so the JIT must
    // emit a MethodDictionary lookup for Callee<__Canon>.
    [MethodImpl(MethodImplOptions.NoInlining)]
    static Task<T> Callee<T>(T value)
    {
        T[] arr = new T[1];
        arr[0] = value;
        return Task.FromResult(arr[0]);
    }

    // Runtime-async generic caller — compiled as MethodImplAttributes.Async.
    // Awaiting Callee<T> in a shared generic context triggers the scanner/JIT mismatch
    // WITHOUT the method.IsAsync fix.
    [MethodImpl(MethodImplOptions.NoInlining)]
    static async Task<T> AsyncSharedGenericCaller<T>(T value)
        where T : class
    {
        T item = await Callee(value).ConfigureAwait(false);
        return item;
    }
}
