// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    internal sealed class SafeZstandardEncoderHandle : SafeHandle
    {
        public SafeZstandardEncoderHandle() : base(IntPtr.Zero, true) { }

        protected override bool ReleaseHandle()
        {
            Interop.Zstandard.ZSTD_freeCCtx(handle);
            return true;
        }

        public override bool IsInvalid => handle == IntPtr.Zero;
    }

    internal sealed class SafeZstandardDecoderHandle : SafeHandle
    {
        public SafeZstandardDecoderHandle() : base(IntPtr.Zero, true) { }

        protected override bool ReleaseHandle()
        {
            Interop.Zstandard.ZSTD_freeDCtx(handle);
            return true;
        }

        public override bool IsInvalid => handle == IntPtr.Zero;
    }

    internal sealed class SafeZstandardCompressStreamHandle : SafeHandle
    {
        public SafeZstandardCompressStreamHandle() : base(IntPtr.Zero, true) { }

        protected override bool ReleaseHandle()
        {
            Interop.Zstandard.ZSTD_freeCStream(handle);
            return true;
        }

        public override bool IsInvalid => handle == IntPtr.Zero;
    }

    internal sealed class SafeZstandardDecompressStreamHandle : SafeHandle
    {
        public SafeZstandardDecompressStreamHandle() : base(IntPtr.Zero, true) { }

        protected override bool ReleaseHandle()
        {
            Interop.Zstandard.ZSTD_freeDStream(handle);
            return true;
        }

        public override bool IsInvalid => handle == IntPtr.Zero;
    }
}
