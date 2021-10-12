// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.IO.Compression;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static class Zstandard
    {
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZDICT_trainFromBuffer(byte[] dictBuffer, nuint dictBufferCapacity, byte[] samplesBuffer, nuint[] samplesSizes, uint nbSamples);
        [DllImport(Libraries.CompressionNative)]
        internal static extern uint ZDICT_isError(nuint code);
        [DllImport(Libraries.CompressionNative)]
        internal static extern IntPtr ZDICT_getErrorName(nuint code);

        [DllImport(Libraries.CompressionNative)]
        internal static extern unsafe nuint ZSTD_compress(byte* dst, nuint dstCapacity, byte* src, nuint srcSize, int compressionLevel);
        [DllImport(Libraries.CompressionNative)]
        internal static extern unsafe nuint ZSTD_decompress(byte* dst, nuint dstCapacity, byte* src, nuint srcSize);

        [DllImport(Libraries.CompressionNative)]
        internal static extern SafeZstandardEncoderHandle ZSTD_createCCtx();
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_freeCCtx(IntPtr cctx);

        [DllImport(Libraries.CompressionNative)]
        internal static extern SafeZstandardDecoderHandle ZSTD_createDCtx();
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_freeDCtx(IntPtr cctx);

        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_compressCCtx(SafeZstandardEncoderHandle ctx, IntPtr dst, nuint dstCapacity, IntPtr src, nuint srcSize, int compressionLevel);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_compressCCtx(SafeZstandardEncoderHandle ctx, ref byte dst, nuint dstCapacity, ref byte src, nuint srcSize, int compressionLevel);

        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_decompressDCtx(SafeZstandardDecoderHandle ctx, IntPtr dst, nuint dstCapacity, IntPtr src, nuint srcSize);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_decompressDCtx(SafeZstandardDecoderHandle ctx, ref byte dst, nuint dstCapacity, ref byte src, nuint srcSize);


        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_compress2(SafeZstandardEncoderHandle ctx, ref byte dst, nuint dstCapacity, ref byte src, nuint srcSize);


        [DllImport(Libraries.CompressionNative)]
        internal static extern IntPtr ZSTD_createCDict(byte[] dict, nuint dictSize, int compressionLevel);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_freeCDict(IntPtr cdict);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_compress_usingCDict(IntPtr cctx, IntPtr dst, nuint dstCapacity, IntPtr src, nuint srcSize, IntPtr cdict);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_compress_usingCDict(IntPtr cctx, ref byte dst, nuint dstCapacity, ref byte src, nuint srcSize, IntPtr cdict);


        [DllImport(Libraries.CompressionNative)]
        internal static extern IntPtr ZSTD_createDDict(byte[] dict, nuint dictSize);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_freeDDict(IntPtr ddict);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_decompress_usingDDict(IntPtr dctx, IntPtr dst, nuint dstCapacity, IntPtr src, nuint srcSize, IntPtr ddict);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_decompress_usingDDict(IntPtr dctx, ref byte dst, nuint dstCapacity, ref byte src, nuint srcSize, IntPtr ddict);


        [DllImport(Libraries.CompressionNative)]
        internal static extern ulong ZSTD_getDecompressedSize(IntPtr src, nuint srcSize);
        [DllImport(Libraries.CompressionNative)]
        internal static extern ulong ZSTD_getFrameContentSize(IntPtr src, nuint srcSize);
        [DllImport(Libraries.CompressionNative)]
        internal static extern ulong ZSTD_getFrameContentSize(ref byte src, nuint srcSize);


        internal const ulong ZSTD_CONTENTSIZE_UNKNOWN = unchecked(0UL - 1);
        internal const ulong ZSTD_CONTENTSIZE_ERROR = unchecked(0UL - 2);

        [DllImport(Libraries.CompressionNative)]
        internal static extern int ZSTD_maxCLevel();
        [DllImport(Libraries.CompressionNative)]
        internal static extern int ZSTD_minCLevel();
        [DllImport(Libraries.CompressionNative)]
        internal static extern int ZSTD_defaultCLevel();
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_compressBound(nuint srcSize);
        [DllImport(Libraries.CompressionNative)]
        internal static extern uint ZSTD_isError(nuint code);
        [DllImport(Libraries.CompressionNative)]
        internal static extern IntPtr ZSTD_getErrorName(nuint code);

        #region Advanced APIs

        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_CCtx_reset(SafeZstandardEncoderHandle cctx, ZSTD_ResetDirective reset);

        [DllImport(Libraries.CompressionNative)]
        internal static extern ZSTD_bounds ZSTD_cParam_getBounds(ZSTD_cParameter cParam);

        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_CCtx_setParameter(SafeZstandardEncoderHandle cctx, ZSTD_cParameter param, int value);

        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_DCtx_reset(SafeZstandardDecoderHandle dctx, ZSTD_ResetDirective reset);

        [DllImport(Libraries.CompressionNative)]
        internal static extern ZSTD_bounds ZSTD_dParam_getBounds(ZSTD_dParameter dParam);

        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_DCtx_setParameter(SafeZstandardDecoderHandle dctx, ZSTD_dParameter param, int value);


        [StructLayout(LayoutKind.Sequential)]
        internal struct ZSTD_bounds
        {
            internal nuint error;
            internal int lowerBound;
            internal int upperBound;
        }

        internal enum ZSTD_ResetDirective
        {
            ZSTD_reset_session_only = 1,
            ZSTD_reset_parameters = 2,
            ZSTD_reset_session_and_parameters = 3
        }

        #endregion

        #region Streaming APIs

        [DllImport(Libraries.CompressionNative)]
        internal static extern SafeZstandardCompressStreamHandle ZSTD_createCStream();
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_freeCStream(IntPtr zcs);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_initCStream(SafeZstandardCompressStreamHandle zcs, int compressionLevel);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_compressStream(SafeZstandardCompressStreamHandle zcs, ref ZSTD_Buffer output, ref ZSTD_Buffer input);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_flushStream(SafeZstandardCompressStreamHandle zcs, ref ZSTD_Buffer output);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_endStream(SafeZstandardCompressStreamHandle zcs, ref ZSTD_Buffer output);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_CStreamInSize();
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_CStreamOutSize();
        [DllImport(Libraries.CompressionNative)]
        internal static extern SafeZstandardDecompressStreamHandle ZSTD_createDStream();
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_freeDStream(IntPtr zds);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_initDStream(SafeZstandardDecompressStreamHandle zds);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_decompressStream(SafeZstandardDecoderHandle zds, ref ZSTD_Buffer output, ref ZSTD_Buffer input);
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_DStreamInSize();
        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_DStreamOutSize();

        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_compressStream2(SafeZstandardEncoderHandle cctx, ref ZSTD_Buffer output, ref ZSTD_Buffer input, ZSTD_EndDirective endOp);

        internal enum ZSTD_EndDirective
        {
            ZSTD_e_continue = 0,
            ZSTD_e_flush = 1,
            ZSTD_e_end = 2
        }

        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_initDStream_usingDDict(SafeZstandardDecompressStreamHandle zds, IntPtr dict);

        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_initCStream_usingCDict(SafeZstandardCompressStreamHandle zcs, IntPtr dict);

        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_CCtx_refCDict(IntPtr cctx, IntPtr cdict);

        [DllImport(Libraries.CompressionNative)]
        internal static extern nuint ZSTD_DCtx_refDDict(IntPtr cctx, IntPtr cdict);

        [StructLayout(LayoutKind.Sequential)]
        internal struct ZSTD_Buffer
        {
            internal ZSTD_Buffer(nuint pos, nuint size)
            {
                this.buffer = IntPtr.Zero;
                this.size = size;
                this.pos = pos;
            }

            internal IntPtr buffer;
            internal nuint size;
            internal nuint pos;

            internal bool IsFullyConsumed => (ulong)size <= (ulong)pos;
        }

        #endregion

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

        public enum ZSTD_dParameter
        {
            ZSTD_d_windowLogMax = 100
        }

        public enum ZSTD_ErrorCode
        {
            ZSTD_error_no_error = 0,
            ZSTD_error_GENERIC = 1,
            ZSTD_error_prefix_unknown = 10,
            ZSTD_error_version_unsupported = 12,
            ZSTD_error_frameParameter_unsupported = 14,
            ZSTD_error_frameParameter_windowTooLarge = 16,
            ZSTD_error_corruption_detected = 20,
            ZSTD_error_checksum_wrong = 22,
            ZSTD_error_dictionary_corrupted = 30,
            ZSTD_error_dictionary_wrong = 32,
            ZSTD_error_dictionaryCreation_failed = 34,
            ZSTD_error_parameter_unsupported = 40,
            ZSTD_error_parameter_outOfBound = 42,
            ZSTD_error_tableLog_tooLarge = 44,
            ZSTD_error_maxSymbolValue_tooLarge = 46,
            ZSTD_error_maxSymbolValue_tooSmall = 48,
            ZSTD_error_stage_wrong = 60,
            ZSTD_error_init_missing = 62,
            ZSTD_error_memory_allocation = 64,
            ZSTD_error_workSpace_tooSmall = 66,
            ZSTD_error_dstSize_tooSmall = 70,
            ZSTD_error_srcSize_wrong = 72,
            ZSTD_error_dstBuffer_null = 74
        }
    }
}
