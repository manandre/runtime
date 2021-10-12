// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.IO.Compression
{
    internal static partial class ZstandardUtils
    {
        public static int CompressionLevel_Min = Interop.Zstandard.ZSTD_minCLevel();
        public static int CompressionLevel_Max = Interop.Zstandard.ZSTD_maxCLevel();
        public static int CompressionLevel_Default = Interop.Zstandard.ZSTD_defaultCLevel();
        //public const int MaxInputSize = int.MaxValue - 515; // 515 is the max compressed extra bytes

        internal static int GetCompressionLevel(CompressionLevel level) =>
            level switch
            {
                CompressionLevel.Optimal => CompressionLevel_Default,
                CompressionLevel.NoCompression => CompressionLevel_Min,
                CompressionLevel.Fastest => CompressionLevel_Min,
                CompressionLevel.SmallestSize => CompressionLevel_Max,
                _ => (int)level,
            };

        public static bool IsZstdSuccess(this nuint returnValue) => Interop.Zstandard.ZSTD_isError(returnValue) == 0;
    }
}
