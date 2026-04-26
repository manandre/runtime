// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

using ILCompiler.DependencyAnalysis;
using Internal.TypeSystem;
using Internal.TypeSystem.Ecma;

using Xunit;

namespace ILCompiler.Compiler.Tests
{
    /// <summary>
    /// Regression test for https://github.com/dotnet/runtime/issues/127179.
    ///
    /// Without the fix, the ILC scanner scans a runtime-async generic caller and sees a call to a
    /// non-runtime-async Task-returning callee followed by AsyncHelpers.Await (the task-await pattern).
    /// It incorrectly places a MethodDictionary slot for the async variant of the callee in the
    /// precomputed generic dictionary. At code-generation time the JIT reverts to the original
    /// (non-async) callee, looks up a slot for the original method, does not find it, and throws
    /// <see cref="InvalidOperationException"/>.
    ///
    /// The fix adds <c>allowAsyncVariant = allowAsyncVariant &amp;&amp; method.IsAsync</c>: only
    /// promote the callee to its async variant when the callee itself is runtime-async.
    /// </summary>
    public class AsyncDictionaryLayoutTests
    {
        private const string AsyncAssetsModuleName = "ILCompiler.Compiler.Tests.AsyncAssets";
        private const string AssetsNamespace = "ILCompiler.Compiler.Tests.AsyncAssets";
        private const string AsyncRegressionTypeName = "AsyncRegression";
        private const string CalleeName = "Callee";
        private const string AsyncCallerName = "AsyncCaller";

        private static (CompilerTypeSystemContext context, MetadataType asyncRegressionType) CreateContext()
        {
            var target = new TargetDetails(TargetArchitecture.X64, TargetOS.Windows, TargetAbi.NativeAot);
            var context = new CompilerTypeSystemContext(target, SharedGenericsMode.CanonicalReferenceTypes, DelegateFeature.All);

            context.InputFilePaths = new Dictionary<string, string>
            {
                { "Test.CoreLib", "Test.CoreLib.dll" },
                { AsyncAssetsModuleName, $"{AsyncAssetsModuleName}.dll" },
            };
            context.ReferenceFilePaths = new Dictionary<string, string>();
            context.SetSystemModule(context.GetModuleForSimpleName("Test.CoreLib"));

            var asyncAssetsModule = context.GetModuleForSimpleName(AsyncAssetsModuleName);
            var asyncRegressionType = (MetadataType)asyncAssetsModule
                .GetType("ILCompiler.Compiler.Tests.AsyncAssets"u8, "AsyncRegression"u8);
            return (context, asyncRegressionType);
        }

        [Fact]
        public void NonAsyncCalleeSlotInAsyncCallerDictionary_DoesNotThrow()
        {
            // Arrange: set up context and load types
            var (context, asyncRegressionType) = CreateContext();

            var calleeDefinition = asyncRegressionType.GetMethod(CalleeName, null);
            var asyncCallerDefinition = asyncRegressionType.GetMethod(AsyncCallerName, null);
            Assert.NotNull(calleeDefinition);
            Assert.NotNull(asyncCallerDefinition);

            // The canonical form of AsyncCaller<T>: T = __Canon
            var asyncCallerCanon = (MethodDesc)asyncCallerDefinition.GetCanonMethodTarget(CanonicalFormKind.Specific);

            // The async variant of the canonical caller is what the scanner scans.
            var asyncCallerVariant = context.GetAsyncVariantMethod(asyncCallerCanon);

            // Scan starting from the variant (this is what the compiler does for runtime-async methods).
            ILScanResults results = RunScanner(context, asyncCallerVariant);

            // Get the precomputed dictionary layout for the caller's canonical variant.
            DictionaryLayoutNode layout = results.GetDictionaryLayoutInfo().GetLayout(asyncCallerVariant);

            // Construct the lookup entry that the JIT uses: a MethodDictionary slot for the
            // *original* (non-async) callee instantiated with the caller's formal type parameter.
            TypeDesc callerFormalT = asyncCallerDefinition.Instantiation[0];
            MethodDesc calleeWithFormal = context.GetInstantiatedMethod(calleeDefinition, new Instantiation(callerFormalT));
            var entry = new MethodDictionaryGenericLookupResult(calleeWithFormal);

            // Assert: TryGetSlotForEntry must succeed — the original callee's slot must be present.
            // Without the fix, the scanner placed the *async variant* of Callee<T> in the layout
            // instead of the original, and TryGetSlotForEntry would throw InvalidOperationException.
            layout.TryGetSlotForEntry(entry, out int slot);
            Assert.True(slot >= 0, "Expected a non-negative dictionary slot for the original callee.");
        }

        private static ILScanResults RunScanner(CompilerTypeSystemContext context, MethodDesc rootMethod)
        {
            CompilationModuleGroup compilationGroup = new SingleFileCompilationModuleGroup();

            NativeAotILProvider ilProvider = new NativeAotILProvider();
            Dataflow.CompilerGeneratedState compilerGeneratedState = new Dataflow.CompilerGeneratedState(
                ilProvider, Logger.Null, disableGeneratedCodeHeuristics: true);

            UsageBasedMetadataManager metadataManager = new UsageBasedMetadataManager(
                compilationGroup, context,
                new FullyBlockedMetadataBlockingPolicy(),
                new FullyBlockedManifestResourceBlockingPolicy(),
                null,
                new NoStackTraceEmissionPolicy(),
                new NoDynamicInvokeThunkGenerationPolicy(),
                new ILLink.Shared.TrimAnalysis.FlowAnnotations(Logger.Null, ilProvider, compilerGeneratedState),
                UsageBasedMetadataGenerationOptions.None,
                default, Logger.Null,
                new Dictionary<string, bool>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>());

            IILScanner scanner = new RyuJitCompilationBuilder(context, compilationGroup)
                .UseILProvider(ilProvider)
                .GetILScannerBuilder()
                .UseCompilationRoots(new ICompilationRootProvider[] { new SingleMethodRootProvider(rootMethod) })
                .UseMetadataManager(metadataManager)
                .ToILScanner();

            return scanner.Scan();
        }
    }
}
