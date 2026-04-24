// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Regression test for https://github.com/dotnet/runtime/issues/127179
// Scanner/JIT mismatch when a runtime-async method awaits a non-runtime-async
// Task-returning generic method from a generic context.

using System;
using System.Threading.Tasks;

class Program
{
    static int Main()
    {
        var box1 = new Box<int>(42);
        int result1 = box1.GetValueAsync().GetAwaiter().GetResult();
        if (result1 != 42)
        {
            Console.WriteLine($"FAIL: Expected 42, got {result1}");
            return 1;
        }

        var box2 = new Box<string>("hello");
        string result2 = box2.GetValueAsync().GetAwaiter().GetResult();
        if (result2 != "hello")
        {
            Console.WriteLine($"FAIL: Expected 'hello', got '{result2}'");
            return 1;
        }

        Console.WriteLine("PASS");
        return 100;
    }
}

// Non-runtime-async static generic Task-returning method
static class Helper
{
    public static Task<T> IdentityAsync<T>(T value) => Task.FromResult(value);
}

// Generic class with a runtime-async method that awaits a non-runtime-async Task-returning method.
// The ILC scanner must not precompute a MethodDictionary slot for the async variant of IdentityAsync<T>
// while the JIT falls back to looking up the original, or vice versa.
class Box<T>
{
    private T _value;
    public Box(T value) { _value = value; }

    public async Task<T> GetValueAsync()
    {
        return await Helper.IdentityAsync(_value).ConfigureAwait(false);
    }
}
