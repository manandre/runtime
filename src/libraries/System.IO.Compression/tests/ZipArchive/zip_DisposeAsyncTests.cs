// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO.Tests;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Compression.Tests
{
    public class zip_DisposeAsyncTests : ZipFileTestBase
    {
        [Fact]
        public static async Task DiposeAsyncCallsWriteAsyncOnly()
        {
            MemoryStream ms = new MemoryStream();
            CallTrackingStream trackingStream = new CallTrackingStream(ms);

            await using (ZipArchive archive = new ZipArchive(trackingStream, ZipArchiveMode.Create))
            {
                archive.CreateEntry("hey");
            }

            Assert.Equal(0, trackingStream.TimesCalled("Write"));
            Assert.NotEqual(0, trackingStream.TimesCalled("WriteAsync"));
        }
    }
}
