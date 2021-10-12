// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#include "../../AnyOS/entrypoints.h"

// Include System.IO.Compression.Native headers
#include "../zlib/pal_zlib.h"
#include "../brotli/include/brotli/decode.h"
#include "../brotli/include/brotli/encode.h"
#include "../brotli/include/brotli/port.h"
#include "../brotli/include/brotli/types.h"

#include "../zstandard/zstd.h"
#include "../zstandard/zstd_errors.h"
#include "../zstandard/zdict.h"

#include "../../AnyOS/entrypoints.h"

static const Entry s_compressionNative[] =
{
    DllImportEntry(BrotliDecoderCreateInstance)
    DllImportEntry(BrotliDecoderDecompress)
    DllImportEntry(BrotliDecoderDecompressStream)
    DllImportEntry(BrotliDecoderDestroyInstance)
    DllImportEntry(BrotliDecoderIsFinished)
    DllImportEntry(BrotliEncoderCompress)
    DllImportEntry(BrotliEncoderCompressStream)
    DllImportEntry(BrotliEncoderCreateInstance)
    DllImportEntry(BrotliEncoderDestroyInstance)
    DllImportEntry(BrotliEncoderHasMoreOutput)
    DllImportEntry(BrotliEncoderSetParameter)
    DllImportEntry(CompressionNative_Crc32)
    DllImportEntry(CompressionNative_Deflate)
    DllImportEntry(CompressionNative_DeflateEnd)
    DllImportEntry(CompressionNative_DeflateReset)
    DllImportEntry(CompressionNative_DeflateInit2_)
    DllImportEntry(CompressionNative_Inflate)
    DllImportEntry(CompressionNative_InflateEnd)
    DllImportEntry(CompressionNative_InflateReset)
    DllImportEntry(CompressionNative_InflateInit2_)
    DllImportEntry(ZDICT_trainFromBuffer)
    DllImportEntry(ZDICT_isError)
    DllImportEntry(ZDICT_getErrorName)
    DllImportEntry(ZSTD_compress)
    DllImportEntry(ZSTD_decompress)
    DllImportEntry(ZSTD_createCCtx)
    DllImportEntry(ZSTD_freeCCtx)
    DllImportEntry(ZSTD_createDCtx)
    DllImportEntry(ZSTD_freeDCtx)
    DllImportEntry(ZSTD_compressCCtx)
    DllImportEntry(ZSTD_decompressDCtx)
    DllImportEntry(ZSTD_compress2)
    DllImportEntry(ZSTD_createCDict)
    DllImportEntry(ZSTD_freeCDict)
    DllImportEntry(ZSTD_compress_usingCDict)
    DllImportEntry(ZSTD_createDDict)
    DllImportEntry(ZSTD_freeDDict)
    DllImportEntry(ZSTD_decompress_usingDDict)
    DllImportEntry(ZSTD_getDecompressedSize)
    DllImportEntry(ZSTD_getFrameContentSize)
    DllImportEntry(ZSTD_maxCLevel)
    DllImportEntry(ZSTD_minCLevel)
    DllImportEntry(ZSTD_defaultCLevel)
    DllImportEntry(ZSTD_compressBound)
    DllImportEntry(ZSTD_isError)
    DllImportEntry(ZSTD_getErrorName)
    // Advanced APIs
    DllImportEntry(ZSTD_CCtx_reset)
    DllImportEntry(ZSTD_cParam_getBounds)
    DllImportEntry(ZSTD_CCtx_setParameter)
    DllImportEntry(ZSTD_DCtx_reset)
    DllImportEntry(ZSTD_dParam_getBounds)
    DllImportEntry(ZSTD_DCtx_setParameter)
    // Streaming APIs
    DllImportEntry(ZSTD_createCStream)
    DllImportEntry(ZSTD_freeCStream)
    DllImportEntry(ZSTD_initCStream)
    DllImportEntry(ZSTD_compressStream)
    DllImportEntry(ZSTD_flushStream)
    DllImportEntry(ZSTD_endStream)
    DllImportEntry(ZSTD_CStreamInSize)
    DllImportEntry(ZSTD_CStreamOutSize)
    DllImportEntry(ZSTD_createDStream)
    DllImportEntry(ZSTD_freeDStream)
    DllImportEntry(ZSTD_initDStream)
    DllImportEntry(ZSTD_decompressStream)
    DllImportEntry(ZSTD_DStreamInSize)
    DllImportEntry(ZSTD_DStreamOutSize)
    DllImportEntry(ZSTD_compressStream2)
    DllImportEntry(ZSTD_CCtx_refCDict)
    DllImportEntry(ZSTD_DCtx_refDDict)
};

EXTERN_C const void* CompressionResolveDllImport(const char* name);

EXTERN_C const void* CompressionResolveDllImport(const char* name)
{
    return ResolveDllImport(s_compressionNative, lengthof(s_compressionNative), name);
}
