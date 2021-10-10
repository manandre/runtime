// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// ------------------------------------------------------------------------------
// Changes to this file must follow the https://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.IO.Compression
{
    public enum ZSTD_dParameter
    {
        ZSTD_d_windowLogMax = 100
    }
    public partial struct ZstandardDecompressionOptions : System.IDisposable
    {
        private object _dummy;
        private int _dummyPrimitive;
        public ZstandardDecompressionOptions(byte[] dict) { throw null; }
        public ZstandardDecompressionOptions(byte[] dict, System.Collections.Generic.IReadOnlyDictionary<ZSTD_dParameter, int> advancedParams) { throw null; }
        public void Dispose() { }

        public readonly byte[] Dictionary;
        public readonly System.Collections.Generic.IReadOnlyDictionary<ZSTD_dParameter, int> AdvancedParams;
    }
    public partial struct ZstandardDecoder : System.IDisposable
    {
        private object _dummy;
        private int _dummyPrimitive;
        public ZstandardDecoder(ZstandardDecompressionOptions options) { throw null; }
        public System.Buffers.OperationStatus Decompress(System.ReadOnlySpan<byte> source, System.Span<byte> destination, out int bytesConsumed, out int bytesWritten) { throw null; }
        public void Dispose() { }
        public static bool TryDecompress(System.ReadOnlySpan<byte> source, System.Span<byte> destination, out int bytesWritten) { throw null; }
        public static bool TryDecompress(System.ReadOnlySpan<byte> source, System.Span<byte> destination, out int bytesWritten, ZstandardDecompressionOptions options) { throw null; }
    }
    public enum ZSTD_cParameter
    {
        // compression parameters
        ZSTD_c_compressionLevel = 100,
        ZSTD_c_windowLog = 101,
        ZSTD_c_hashLog = 102,
        ZSTD_c_chainLog = 103,
        ZSTD_c_searchLog = 104,
        ZSTD_c_minMatch = 105,
        ZSTD_c_targetLength = 106,
        ZSTD_c_strategy = 107,

        // long distance matching mode parameters
        ZSTD_c_enableLongDistanceMatching = 160,
        ZSTD_c_ldmHashLog = 161,
        ZSTD_c_ldmMinMatch = 162,
        ZSTD_c_ldmBucketSizeLog = 163,
        ZSTD_c_ldmHashRateLog = 164,

        // frame parameters
        ZSTD_c_contentSizeFlag = 200,
        ZSTD_c_checksumFlag = 201,
        ZSTD_c_dictIDFlag = 202,

        // multi-threading parameters
        ZSTD_c_nbWorkers = 400,
        ZSTD_c_jobSize = 401,
        ZSTD_c_overlapLog = 402
    }
    public partial struct ZstandardCompressionOptions : System.IDisposable
    {
        private object _dummy;
        private int _dummyPrimitive;
        public ZstandardCompressionOptions(int compressionLevel) { throw null; }
        public ZstandardCompressionOptions(byte[] dict, int compressionLevel) { throw null; }
        public ZstandardCompressionOptions(byte[] dict, System.Collections.Generic.IReadOnlyDictionary<ZSTD_dParameter, int> advancedParams, int compressionLevel) { throw null; }
        public void Dispose() { }

        public static int MinCompressionLevel { get; }
        public static int MaxCompressionLevel { get; }

        public const int DefaultCompressionLevel = 3;

        public static ZstandardCompressionOptions Default { get; }

        public readonly int CompressionLevel;
        public readonly byte[] Dictionary;
        public readonly System.Collections.Generic.IReadOnlyDictionary<ZSTD_cParameter, int> AdvancedParams;
    }
    public partial struct ZstandardEncoder : System.IDisposable
    {
        private object _dummy;
        private int _dummyPrimitive;
        public ZstandardEncoder(ZstandardCompressionOptions options) { throw null; }
        public System.Buffers.OperationStatus Compress(System.ReadOnlySpan<byte> source, System.Span<byte> destination, out int bytesConsumed, out int bytesWritten, bool isFinalBlock) { throw null; }
        public void Dispose() { }
        public System.Buffers.OperationStatus Flush(System.Span<byte> destination, out int bytesWritten) { throw null; }
        public static int GetMaxCompressedLength(int inputSize) { throw null; }
        public static long GetMaxCompressedLengthLong(long inputSize) { throw null; }
        public static bool TryCompress(System.ReadOnlySpan<byte> source, System.Span<byte> destination, out int bytesWritten) { throw null; }
        public static bool TryCompress(System.ReadOnlySpan<byte> source, System.Span<byte> destination, out int bytesWritten, ZstandardCompressionOptions options) { throw null; }
    }
    public sealed partial class ZstandardStream : System.IO.Stream
    {
        public ZstandardStream(System.IO.Stream stream, CompressionMode mode) { }
        public ZstandardStream(System.IO.Stream stream, CompressionMode mode, int bufferSize) { }
        public ZstandardStream(System.IO.Stream stream, CompressionMode mode, bool leaveOpen) { }
        public ZstandardStream(System.IO.Stream stream, CompressionMode mode, int bufferSize, bool leaveOpen) { }
        public ZstandardStream(System.IO.Stream stream, ZstandardCompressionOptions options) { }
        public ZstandardStream(System.IO.Stream stream, ZstandardCompressionOptions options, int bufferSize) { }
        public ZstandardStream(System.IO.Stream stream, ZstandardCompressionOptions options, bool leaveOpen) { }
        public ZstandardStream(System.IO.Stream stream, ZstandardCompressionOptions options, int bufferSize, bool leaveOpen) { }
        public ZstandardStream(System.IO.Stream stream, ZstandardDecompressionOptions options) { }
        public ZstandardStream(System.IO.Stream stream, ZstandardDecompressionOptions options, int bufferSize) { }
        public ZstandardStream(System.IO.Stream stream, ZstandardDecompressionOptions options, bool leaveOpen) { }
        public ZstandardStream(System.IO.Stream stream, ZstandardDecompressionOptions options, int bufferSize, bool leaveOpen) { }
        public System.IO.Stream BaseStream { get { throw null; } }
        public override bool CanRead { get { throw null; } }
        public override bool CanSeek { get { throw null; } }
        public override bool CanWrite { get { throw null; } }
        public override long Length { get { throw null; } }
        public override long Position { get { throw null; } set { } }
        public override System.IAsyncResult BeginRead(byte[] buffer, int offset, int count, System.AsyncCallback? asyncCallback, object? asyncState) { throw null; }
        public override System.IAsyncResult BeginWrite(byte[] buffer, int offset, int count, System.AsyncCallback? asyncCallback, object? asyncState) { throw null; }
        protected override void Dispose(bool disposing) { }
        public override System.Threading.Tasks.ValueTask DisposeAsync() { throw null; }
        public override int EndRead(System.IAsyncResult asyncResult) { throw null; }
        public override void EndWrite(System.IAsyncResult asyncResult) { }
        public override void Flush() { }
        public override System.Threading.Tasks.Task FlushAsync(System.Threading.CancellationToken cancellationToken) { throw null; }
        public override int Read(byte[] buffer, int offset, int count) { throw null; }
        public override int Read(System.Span<byte> buffer) { throw null; }
        public override System.Threading.Tasks.Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken) { throw null; }
        public override System.Threading.Tasks.ValueTask<int> ReadAsync(System.Memory<byte> buffer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public override int ReadByte() { throw null; }
        public override long Seek(long offset, System.IO.SeekOrigin origin) { throw null; }
        public override void SetLength(long value) { }
        public override void Write(byte[] buffer, int offset, int count) { }
        public override void Write(System.ReadOnlySpan<byte> buffer) { }
        public override System.Threading.Tasks.Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken) { throw null; }
        public override System.Threading.Tasks.ValueTask WriteAsync(System.ReadOnlyMemory<byte> buffer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public override void WriteByte(byte value) { }
    }

    public static partial class ZstandardDictBuilder
    {
        public static byte[] TrainFromBuffer(System.Collections.Generic.IEnumerable<byte[]> samples) { throw null; }
        public static byte[] TrainFromBuffer(System.Collections.Generic.IEnumerable<byte[]> samples, int dictCapacity) { throw null; }

        public const int DefaultDictCapacity = 112640;
    }
}
