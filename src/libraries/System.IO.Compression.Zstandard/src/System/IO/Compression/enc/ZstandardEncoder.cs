// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Compression
{
    /// <summary>Provides methods and static methods to encode and decode data in a streamless, non-allocating, and performant manner using the Zstandard data format specification.</summary>
    public partial struct ZstandardEncoder : IDisposable
    {
        internal SafeZstandardEncoderHandle? _state;
        private bool _disposed;

        /// <summary>Initializes a new instance of the <see cref="System.IO.Compression.ZstandardEncoder" /> structure using the specified quality and window.</summary>
        /// <param name="compressionLevel">A number representing quality of the Zstandard compression.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="compressionLevel" /> is not in the supported range.</exception>
        public ZstandardEncoder(int compressionLevel)
        {
            _disposed = false;
            _state = Interop.Zstandard.ZSTD_createCCtx();
            if (_state.IsInvalid)
                throw new IOException(SR.ZstandardEncoder_Create);
            SetCompressionLevel(compressionLevel);
        }

        /// <summary>
        /// Performs a lazy initialization of the native encoder using the default CompressionLevel.
        /// </summary>
        internal void InitializeEncoder()
        {
            EnsureNotDisposed();
            _state = Interop.Zstandard.ZSTD_createCCtx();
            if (_state.IsInvalid)
                throw new IOException(SR.ZstandardEncoder_Create);
        }

        internal void EnsureInitialized()
        {
            EnsureNotDisposed();
            if (_state == null)
            {
                InitializeEncoder();
            }
        }

        /// <summary>Frees and disposes unmanaged resources.</summary>
        public void Dispose()
        {
            _disposed = true;
            _state?.Dispose();
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ZstandardEncoder), SR.ZstandardEncoder_Disposed);
        }

        internal void SetCompressionLevel(int compressionLevel)
        {
            EnsureNotDisposed();
            if (_state == null || _state.IsInvalid || _state.IsClosed)
            {
                InitializeEncoder();
                Debug.Assert(_state != null && !_state.IsInvalid && !_state.IsClosed);
            }
            if (compressionLevel < ZstandardUtils.CompressionLevel_Min || compressionLevel > ZstandardUtils.CompressionLevel_Max)
            {
                throw new ArgumentOutOfRangeException(nameof(compressionLevel), SR.Format(SR.ZstandardEncoder_CompressionLevel, compressionLevel, ZstandardUtils.CompressionLevel_Min, ZstandardUtils.CompressionLevel_Max));
            }
            if (!Interop.Zstandard.ZSTD_CCtx_setParameter(_state, Interop.Zstandard.ZSTD_cParameter.ZSTD_c_compressionLevel, compressionLevel).IsZstdSuccess())
            {
                throw new InvalidOperationException(SR.Format(SR.ZstandardEncoder_InvalidSetParameter, "CompressionLevel"));
            }
        }

        /// <summary>Gets the maximum expected compressed length for the provided input size.</summary>
        /// <param name="inputSize">The input size to get the maximum expected compressed length from. Must be greater or equal than 0.</param>
        /// <returns>A number representing the maximum compressed length for the provided input size.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="inputSize" /> is less than 0, the minimum allowed input size.</exception>
        public static int GetMaxCompressedLength(int inputSize)
        {
            if (inputSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(inputSize));
            }

            return (int)Interop.Zstandard.ZSTD_compressBound((nuint)inputSize);
        }

        internal OperationStatus Flush(Memory<byte> destination, out int bytesWritten) => Flush(destination.Span, out bytesWritten);

        /// <summary>Compresses an empty read-only span of bytes into its destination, which ensures that output is produced for all the processed input. An actual flush is performed when the source is depleted and there is enough space in the destination for the remaining data.</summary>
        /// <param name="destination">When this method returns, a span of bytes where the compressed data will be stored.</param>
        /// <param name="bytesWritten">When this method returns, the total number of bytes that were written to <paramref name="destination" />.</param>
        /// <returns>One of the enumeration values that describes the status with which the operation finished.</returns>
        public OperationStatus Flush(Span<byte> destination, out int bytesWritten) => Compress(ReadOnlySpan<byte>.Empty, destination, out int bytesConsumed, out bytesWritten, Interop.Zstandard.ZSTD_EndDirective.ZSTD_e_flush);

        internal OperationStatus Compress(ReadOnlyMemory<byte> source, Memory<byte> destination, out int bytesConsumed, out int bytesWritten, bool isFinalBlock) => Compress(source.Span, destination.Span, out bytesConsumed, out bytesWritten, isFinalBlock);

        /// <summary>Compresses a read-only byte span into a destination span.</summary>
        /// <param name="source">A read-only span of bytes containing the source data to compress.</param>
        /// <param name="destination">When this method returns, a byte span where the compressed is stored.</param>
        /// <param name="bytesConsumed">When this method returns, the total number of bytes that were read from <paramref name="source" />.</param>
        /// <param name="bytesWritten">When this method returns, the total number of bytes that were written to <paramref name="destination" />.</param>
        /// <param name="isFinalBlock"><see langword="true" /> to finalize the internal stream, which prevents adding more input data when this method returns; <see langword="false" /> to allow the encoder to postpone the production of output until it has processed enough input.</param>
        /// <returns>One of the enumeration values that describes the status with which the span-based operation finished.</returns>
        public OperationStatus Compress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten, bool isFinalBlock) => Compress(source, destination, out bytesConsumed, out bytesWritten, isFinalBlock ? Interop.Zstandard.ZSTD_EndDirective.ZSTD_e_end : Interop.Zstandard.ZSTD_EndDirective.ZSTD_e_continue);

        internal OperationStatus Compress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten, Interop.Zstandard.ZSTD_EndDirective endDirective)
        {
            EnsureInitialized();
            Debug.Assert(_state != null);

            bytesWritten = 0;
            bytesConsumed = 0;

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
                        nuint result = Interop.Zstandard.ZSTD_compressStream2(_state, ref output, ref input, endDirective);
                        if (!result.IsZstdSuccess())
                            return OperationStatus.InvalidData;

                        bytesConsumed = (int)input.pos;
                        bytesWritten = (int)output.pos;

                        if (result == 0)
                            return OperationStatus.Done;
                    }
                }

                return OperationStatus.DestinationTooSmall;
            }
        }

        /// <summary>Tries to compress a source byte span into a destination span.</summary>
        /// <param name="source">A read-only span of bytes containing the source data to compress.</param>
        /// <param name="destination">When this method returns, a span of bytes where the compressed data is stored.</param>
        /// <param name="bytesWritten">When this method returns, the total number of bytes that were written to <paramref name="destination" />.</param>
        /// <returns><see langword="true" /> if the compression operation was successful; <see langword="false" /> otherwise.</returns>
        public static bool TryCompress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten) => TryCompress(source, destination, out bytesWritten, ZstandardUtils.CompressionLevel_Default);

        /// <summary>Tries to compress a source byte span into a destination byte span, using the provided compression quality leven and encoder window bits.</summary>
        /// <param name="source">A read-only span of bytes containing the source data to compress.</param>
        /// <param name="destination">When this method returns, a span of bytes where the compressed data is stored.</param>
        /// <param name="bytesWritten">When this method returns, the total number of bytes that were written to <paramref name="destination" />.</param>
        /// <param name="compressionLevel">A number representing quality of the Zstandard compression.</param>
        /// <returns><see langword="true" /> if the compression operation was successful; <see langword="false" /> otherwise.</returns>
        public static bool TryCompress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten, int compressionLevel)
        {
            if (compressionLevel < ZstandardUtils.CompressionLevel_Min || compressionLevel > ZstandardUtils.CompressionLevel_Max)
            {
                throw new ArgumentOutOfRangeException(nameof(compressionLevel), SR.Format(SR.ZstandardEncoder_CompressionLevel, compressionLevel, ZstandardUtils.CompressionLevel_Min, ZstandardUtils.CompressionLevel_Max));
            }

            bytesWritten = 0;

            unsafe
            {
                fixed (byte* inBytes = &MemoryMarshal.GetReference(source))
                fixed (byte* outBytes = &MemoryMarshal.GetReference(destination))
                {
                    nuint result = Interop.Zstandard.ZSTD_compress(
                        outBytes, (nuint)destination.Length,
                        inBytes, (nuint)source.Length,
                        compressionLevel);

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
}
