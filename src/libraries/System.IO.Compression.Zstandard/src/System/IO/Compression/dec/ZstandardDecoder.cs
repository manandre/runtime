// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Compression
{
    /// <summary>Provides non-allocating, performant Zstandard decompression methods. The methods decompress in a single pass without using a <see cref="System.IO.Compression.ZstandardStream" /> instance.</summary>
    public struct ZstandardDecoder : IDisposable
    {
        private SafeZstandardDecoderHandle? _state;
        private bool _finished;
        private bool _disposed;

        internal void InitializeDecoder()
        {
            _state = Interop.Zstandard.ZSTD_createDCtx();
            if (_state.IsInvalid)
                throw new IOException(SR.ZstandardDecoder_Create);
        }

        internal void EnsureInitialized()
        {
            EnsureNotDisposed();
            if (_state == null)
                InitializeDecoder();
        }

        /// <summary>Releases all resources used by the current Zstandard decoder instance.</summary>
        public void Dispose()
        {
            _disposed = true;
            _state?.Dispose();
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ZstandardDecoder), SR.ZstandardDecoder_Disposed);
        }

        /// <summary>Decompresses data that was compressed using the Zstandard algorithm.</summary>
        /// <param name="source">A buffer containing the compressed data.</param>
        /// <param name="destination">When this method returns, a byte span containing the decompressed data.</param>
        /// <param name="bytesConsumed">The total number of bytes that were read from <paramref name="source" />.</param>
        /// <param name="bytesWritten">The total number of bytes that were written in the <paramref name="destination" />.</param>
        /// <returns>One of the enumeration values that indicates the status of the decompression operation.</returns>
        /// <remarks>The return value can be as follows:
        /// - <see cref="System.Buffers.OperationStatus.Done" />: <paramref name="source" /> was successfully and completely decompressed into <paramref name="destination" />.
        /// - <see cref="System.Buffers.OperationStatus.DestinationTooSmall" />: There is not enough space in <paramref name="destination" /> to decompress <paramref name="source" />.
        /// - <see cref="System.Buffers.OperationStatus.NeedMoreData" />: The decompression action is partially done at least one more byte is required to complete the decompression task. This method should be called again with more input to decompress.
        /// - <see cref="System.Buffers.OperationStatus.InvalidData" />: The data in <paramref name="source" /> is invalid and could not be decompressed.</remarks>
        public OperationStatus Decompress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten)
        {
            EnsureInitialized();
            Debug.Assert(_state != null);

            bytesConsumed = 0;
            bytesWritten = 0;

            if (_finished)
                return OperationStatus.Done;

            var input = new Interop.Zstandard.ZSTD_Buffer(0, (nuint)source.Length);
            var output = new Interop.Zstandard.ZSTD_Buffer(0, (nuint)destination.Length);

            unsafe
            {
                fixed (byte* inBytes = &MemoryMarshal.GetReference(source))
                fixed (byte* outBytes = &MemoryMarshal.GetReference(destination))
                {
                    input.buffer = new IntPtr(inBytes);
                    output.buffer = new IntPtr(outBytes);

                    while (!output.IsFullyConsumed)
                    {
                        nuint result = Interop.Zstandard.ZSTD_decompressStream(_state, ref output, ref input);
                        if (!result.IsZstdSuccess())
                            return OperationStatus.InvalidData;

                        bytesConsumed = (int)input.pos;
                        bytesWritten = (int)output.pos;

                        _finished = result == 0;
                        if (_finished)
                            return OperationStatus.Done;

                        if (input.IsFullyConsumed)
                            return OperationStatus.NeedMoreData;
                    }
                }
                return OperationStatus.DestinationTooSmall;
            }
        }

        /// <summary>Attempts to decompress data that was compressed with the Zstandard algorithm.</summary>
        /// <param name="source">A buffer containing the compressed data.</param>
        /// <param name="destination">When this method returns, a byte span containing the decompressed data.</param>
        /// <param name="bytesWritten">The total number of bytes that were written in the <paramref name="destination" />.</param>
        /// <returns><see langword="true" /> on success; <see langword="false" /> otherwise.</returns>
        /// <remarks>If this method returns <see langword="false" />, <paramref name="destination" /> may be empty or contain partially decompressed data, with <paramref name="bytesWritten" /> being zero or greater than zero but less than the expected total.</remarks>
        public static unsafe bool TryDecompress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
        {
            bytesWritten = 0;

            if (source.IsEmpty) return false;

            fixed (byte* inBytes = &MemoryMarshal.GetReference(source))
            fixed (byte* outBytes = &MemoryMarshal.GetReference(destination))
            {
                nuint result = Interop.Zstandard.ZSTD_decompress(
                    outBytes, (nuint)destination.Length,
                    inBytes, (nuint)source.Length);

                if (!result.IsZstdSuccess())
                {
                    return false;
                }

                bytesWritten = (int)result;
                return true;
            }
        }
    }
}
