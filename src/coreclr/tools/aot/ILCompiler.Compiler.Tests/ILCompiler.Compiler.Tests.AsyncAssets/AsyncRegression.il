// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Test asset for https://github.com/dotnet/runtime/issues/127179
//
// Regression test proving that the ILC scanner must not place a MethodDictionary
// slot for the async variant of a non-runtime-async callee in the precomputed
// generic dictionary of a runtime-async caller.
//
// Without the fix, scanning AsyncCaller<T__Canon> results in a layout that
// contains MethodDictionaryGenericLookupResult(AsyncVariant(Callee<T>)) instead of
// MethodDictionaryGenericLookupResult(Callee<T>), and TryGetSlotForEntry throws
// InvalidOperationException when the JIT looks up the original slot.

// Namespace used by AsyncDictionaryLayoutTests.cs
.namespace ILCompiler.Compiler.Tests.AsyncAssets
{
    .class public auto ansi abstract sealed beforefieldinit 'AsyncRegression'
           extends [Test.CoreLib]System.Object
    {
        // Non-runtime-async generic Task-returning method (callee).
        // RequiresInstMethodDescArg() == true because the method has a
        // generic instantiation, forcing a MethodDictionary lookup.
        .method public static hidebysig
                class [Test.CoreLib]System.Threading.Tasks.Task`1<!!T>
                Callee<T> (!!T 'value') cil managed
        {
            .maxstack 1
            ldnull
            ret
        }

        // Runtime-async (MethodImplAttributes.Async) generic caller.
        // Awaits Callee<T> using AsyncHelpers.Await, which is the pattern
        // the scanner recognises as a task-await.
        .method public static hidebysig
                class [Test.CoreLib]System.Threading.Tasks.Task`1<!!T>
                AsyncCaller<T> (!!T 'value') async cil managed
        {
            .maxstack 2
            ldarg.0
            call class [Test.CoreLib]System.Threading.Tasks.Task`1<!!T>
                 ILCompiler.Compiler.Tests.AsyncAssets.AsyncRegression::Callee<!!T>(!!0)
            call !!T [Test.CoreLib]System.Runtime.CompilerServices.AsyncHelpers::Await<!!T>(
                     class [Test.CoreLib]System.Threading.Tasks.Task`1<!!0>)
            ret
        }
    }
}
