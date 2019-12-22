// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Tests
{
    public class BinaryWriter_AsyncTests
    {
        protected virtual Stream CreateStream()
        {
            return new MemoryStream();
        }

        [Fact]
        public async ValueTask BinaryWriter_CtorAndWriteAsyncTests1()
        {
            // [] Smoke test to ensure that we can write with the constructed writer
            using Stream mstr = CreateStream();
            await using BinaryWriter dw2 = new BinaryWriter(mstr);
            using BinaryReader dr2 = new BinaryReader(mstr);

            await dw2.WriteAsync(true);
            await dw2.FlushAsync();
            mstr.Position = 0;

            Assert.True(dr2.ReadBoolean());
        }

        [Theory]
        [MemberData(nameof(EncodingAndEncodingStrings))]
        public async ValueTask BinaryWriter_EncodingCtorAndWriteAsyncTests(Encoding encoding, string testString)
        {
            using Stream memStream = CreateStream();
            await using BinaryWriter writer = new BinaryWriter(memStream, encoding);
            using BinaryReader reader = new BinaryReader(memStream, encoding);

            await writer.WriteAsync(testString);
            await writer.FlushAsync();
            memStream.Position = 0;

            Assert.Equal(testString, reader.ReadString());
        }

        public static IEnumerable<object[]> EncodingAndEncodingStrings
        {
            get
            {
                yield return new object[] { Encoding.UTF8, "This is UTF8\u00FF" };
                yield return new object[] { Encoding.BigEndianUnicode, "This is BigEndianUnicode\u00FF" };
                yield return new object[] { Encoding.Unicode, "This is Unicode\u00FF" };
            }
        }

        [Fact]
        public async ValueTask BinaryWriter_BaseStreamAsyncTests()
        {
            // [] Get the base stream for MemoryStream
            using Stream ms2 = CreateStream();
            await using BinaryWriter sr2 = new BinaryWriter(ms2);

            Assert.Same(ms2, await sr2.GetBaseStreamAsync());
        }

        [Fact]
        public virtual async ValueTask BinaryWriter_FlushAsyncTests()
        {
            // [] Check that flush updates the underlying stream
            using (Stream memstr2 = CreateStream())
            await using (BinaryWriter bw2 = new BinaryWriter(memstr2))
            {
                string str = "HelloWorld";
                int expectedLength = str.Length + 1; // 1 for 7-bit encoded length
                await bw2.WriteAsync(str);
                Assert.Equal(expectedLength, memstr2.Length);
                await bw2.FlushAsync();
                Assert.Equal(expectedLength, memstr2.Length);
            }

            // [] Flushing a closed writer may throw an exception depending on the underlying stream
            using (Stream memstr2 = CreateStream())
            {
                BinaryWriter bw2 = new BinaryWriter(memstr2);
                await bw2.DisposeAsync();
                await bw2.FlushAsync();
            }
        }

        [Fact]
        public async ValueTask BinaryWriter_DisposeAsyncTests_Negative()
        {
            using Stream memStream = CreateStream();
            BinaryWriter binaryWriter = new BinaryWriter(memStream);
            await binaryWriter.DisposeAsync();
            await ValidateDisposedExceptionsAsync(binaryWriter);
        }

        [Fact]
        public async ValueTask BinaryWriter_CloseAsyncTests_Negative()
        {
            using Stream memStream = CreateStream();
            BinaryWriter binaryWriter = new BinaryWriter(memStream);
            binaryWriter.Close();
            await ValidateDisposedExceptionsAsync(binaryWriter);
        }

        private async ValueTask ValidateDisposedExceptionsAsync(BinaryWriter binaryWriter)
        {
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Seek(1, SeekOrigin.Begin));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync(new byte[2], 0, 2).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync(new char[2], 0, 2).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync(true).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync((byte)4).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync(new byte[] { 1, 2 }).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync('a').AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync(new char[] { 'a', 'b' }).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync(5.3).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync((short)3).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync(33).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync((long)42).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync((sbyte)4).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync("Hello There").AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync((float)4.3).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync((ushort)3).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync((uint)4).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync((ulong)5).AsTask());
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryWriter.WriteAsync("Bah").AsTask());
        }
    }
}
