// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO.Compression;
using System.Threading.Tasks;

internal static class DisposeAsyncExtensions
{
    public static async ValueTask DisposeAsync(this ZipArchive archive, bool disposeAsync)
    {
        if (disposeAsync)
        {
            await archive.DisposeAsync();
        }
        else
        {
            archive.Dispose();
        }
    }
}
