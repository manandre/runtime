project(System.IO.Compression.Native)

if("${CMAKE_SOURCE_DIR}" STREQUAL "${CMAKE_BINARY_DIR}")
    message(FATAL_ERROR "Binary directory isn't being correctly set before calling Cmake. Tree must be built in separate directory from source.")
endif()

set(CMAKE_INCLUDE_CURRENT_DIR ON)

if (GEN_SHARED_LIB)
    include (GenerateExportHeader)
endif()

if(${CLR_CMAKE_HOST_ARCH} STREQUAL x86 OR ${CLR_CMAKE_HOST_ARCH} STREQUAL x64)
    set(NATIVECOMPRESSION_SOURCES
        zlib-intel/adler32.c
        zlib-intel/compress.c
        zlib-intel/crc_folding.c
        zlib-intel/crc32.c
        zlib-intel/deflate_medium.c
        zlib-intel/deflate_quick.c
        zlib-intel/deflate.c
        zlib-intel/inffast.c
        zlib-intel/inflate.c
        zlib-intel/inftrees.c
        zlib-intel/match.c
        zlib-intel/slide_sse.c
        zlib-intel/trees.c
        zlib-intel/x86.c
        zlib-intel/zutil.c
)
else()
    set(NATIVECOMPRESSION_SOURCES
        zlib/adler32.c
        zlib/compress.c
        zlib/crc32.c
        zlib/deflate.c
        zlib/inffast.c
        zlib/inflate.c
        zlib/inftrees.c
        zlib/trees.c
        zlib/zutil.c
    )
endif()

set (NATIVECOMPRESSION_SOURCES
    ${NATIVECOMPRESSION_SOURCES}
    ../../AnyOS/zlib/pal_zlib.c
    ../../AnyOS/brotli/common/constants.c
    ../../AnyOS/brotli/common/context.c
    ../../AnyOS/brotli/common/dictionary.c
    ../../AnyOS/brotli/common/platform.c
    ../../AnyOS/brotli/common/transform.c
    ../../AnyOS/brotli/dec/bit_reader.c
    ../../AnyOS/brotli/dec/decode.c
    ../../AnyOS/brotli/dec/huffman.c
    ../../AnyOS/brotli/dec/state.c
    ../../AnyOS/brotli/enc/backward_references.c
    ../../AnyOS/brotli/enc/backward_references_hq.c
    ../../AnyOS/brotli/enc/bit_cost.c
    ../../AnyOS/brotli/enc/block_splitter.c
    ../../AnyOS/brotli/enc/brotli_bit_stream.c
    ../../AnyOS/brotli/enc/cluster.c
    ../../AnyOS/brotli/enc/command.c
    ../../AnyOS/brotli/enc/compress_fragment.c
    ../../AnyOS/brotli/enc/compress_fragment_two_pass.c
    ../../AnyOS/brotli/enc/dictionary_hash.c
    ../../AnyOS/brotli/enc/encode.c
    ../../AnyOS/brotli/enc/encoder_dict.c
    ../../AnyOS/brotli/enc/entropy_encode.c
    ../../AnyOS/brotli/enc/fast_log.c
    ../../AnyOS/brotli/enc/histogram.c
    ../../AnyOS/brotli/enc/literal_cost.c
    ../../AnyOS/brotli/enc/memory.c
    ../../AnyOS/brotli/enc/metablock.c
    ../../AnyOS/brotli/enc/static_dict.c
    ../../AnyOS/brotli/enc/utf8_util.c
    ../../AnyOS/zstandard/common/debug.c
    ../../AnyOS/zstandard/common/entropy_common.c
    ../../AnyOS/zstandard/common/error_private.c
    ../../AnyOS/zstandard/common/fse_decompress.c
    ../../AnyOS/zstandard/common/pool.c
    ../../AnyOS/zstandard/common/threading.c
    ../../AnyOS/zstandard/common/xxhash.c
    ../../AnyOS/zstandard/common/zstd_common.c
    ../../AnyOS/zstandard/compress/fse_compress.c
    ../../AnyOS/zstandard/compress/hist.c
    ../../AnyOS/zstandard/compress/huf_compress.c
    ../../AnyOS/zstandard/compress/zstdmt_compress.c
    ../../AnyOS/zstandard/compress/zstd_compress.c
    ../../AnyOS/zstandard/compress/zstd_compress_literals.c
    ../../AnyOS/zstandard/compress/zstd_compress_sequences.c
    ../../AnyOS/zstandard/compress/zstd_compress_superblock.c
    ../../AnyOS/zstandard/compress/zstd_double_fast.c
    ../../AnyOS/zstandard/compress/zstd_fast.c
    ../../AnyOS/zstandard/compress/zstd_lazy.c
    ../../AnyOS/zstandard/compress/zstd_ldm.c
    ../../AnyOS/zstandard/compress/zstd_opt.c
    ../../AnyOS/zstandard/decompress/huf_decompress.c
    ../../AnyOS/zstandard/decompress/zstd_ddict.c
    ../../AnyOS/zstandard/decompress/zstd_decompress.c
    ../../AnyOS/zstandard/decompress/zstd_decompress_block.c
    ../../AnyOS/zstandard/dictBuilder/cover.c
    ../../AnyOS/zstandard/dictBuilder/divsufsort.c
    ../../AnyOS/zstandard/dictBuilder/fastcover.c
    ../../AnyOS/zstandard/dictBuilder/zdict.c
)

#Include Brotli include files
include_directories("../../AnyOS/brotli/include")

#Include Zstandard include files
include_directories("../../AnyOS/zstandard")
add_definitions(-DZSTD_MULTITHREAD)

if (GEN_SHARED_LIB)
    add_library(System.IO.Compression.Native
        SHARED
        ${NATIVECOMPRESSION_SOURCES}
        System.IO.Compression.Native.def
        # This will add versioning to the library
        ${CMAKE_REPO_ROOT}/artifacts/obj/NativeVersion.rc
    )
endif()

if (NOT GEN_SHARED_LIB AND NOT CLR_CMAKE_TARGET_MACCATALYST AND NOT CLR_CMAKE_TARGET_IOS AND NOT CLR_CMAKE_TARGET_TVOS AND NOT CLR_CMAKE_TARGET_ANDROID AND NOT CLR_CMAKE_TARGET_BROWSER)
    set(NATIVECOMPRESSION_SOURCES ${NATIVECOMPRESSION_SOURCES} ../../AnyOS/System.IO.Compression.Native/entrypoints.c)
endif()

add_library(System.IO.Compression.Native-Static
    STATIC
    ${NATIVECOMPRESSION_SOURCES}
)

# Allow specification of arguments that should be passed to the linker
if (GEN_SHARED_LIB)
    SET_TARGET_PROPERTIES(System.IO.Compression.Native PROPERTIES LINK_OPTIONS "${__LinkArgs};${__SharedLinkArgs}")
endif()
SET_TARGET_PROPERTIES(System.IO.Compression.Native-Static PROPERTIES STATIC_LIBRARY_OPTIONS "${__LinkArgs}")

# Allow specification of libraries that should be linked against
if (GEN_SHARED_LIB)
    target_link_libraries(System.IO.Compression.Native ${__LinkLibraries})
endif()
target_link_libraries(System.IO.Compression.Native-Static ${__LinkLibraries})

if (GEN_SHARED_LIB)
    GENERATE_EXPORT_HEADER( System.IO.Compression.Native
         BASE_NAME System.IO.Compression.Native
         EXPORT_MACRO_NAME System.IO.Compression.Native_EXPORT
         EXPORT_FILE_NAME System.IO.Compression.Native_Export.h
         STATIC_DEFINE System.IO.Compression.Native_BUILT_AS_STATIC
    )

    install (TARGETS System.IO.Compression.Native DESTINATION .)
    install (FILES $<TARGET_PDB_FILE:System.IO.Compression.Native> DESTINATION .)
endif()

install (TARGETS System.IO.Compression.Native-Static DESTINATION ${STATIC_LIB_DESTINATION} COMPONENT libs)
