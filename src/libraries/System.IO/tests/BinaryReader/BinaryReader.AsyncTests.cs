// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Tests
{
    public class BinaryReader_AsyncTests
    {
        protected virtual Stream CreateStream()
        {
            return new MemoryStream();
        }

        public class NegEncoding : UTF8Encoding
        {
            public override Decoder GetDecoder()
            {
                return new NegDecoder();
            }

            public class NegDecoder : Decoder
            {
                public override int GetCharCount(byte[] bytes, int index, int count)
                {
                    return 1;
                }

                public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
                {
                    return -10000000;
                }
            }
        }

        [Fact]
        public async Task ReadAsync_InvalidEncoding()
        {
            using var str = CreateStream();

            byte[] memb = new byte[100];
            new Random(345).NextBytes(memb);
            str.Write(memb, 0, 100);
            str.Position = 0;

            using var reader = new BinaryReader(str, new NegEncoding());
            await Assert.ThrowsAnyAsync<ArgumentException>(() => reader.ReadAsync(new char[10], 0, 10).AsTask());
        }

        [Theory]
        [InlineData(100, 0, 100, 100, 100)]
        [InlineData(100, 25, 50, 100, 50)]
        [InlineData(50, 0, 100, 100, 50)]
        [InlineData(0, 0, 10, 10, 0)]
        public async Task ReadAsync_CharArray(int sourceSize, int index, int count, int destinationSize, int expectedReadLength)
        {
            using var stream = CreateStream();

            var source = new char[sourceSize];
            var random = new Random(345);

            for (int i = 0; i < sourceSize; i++)
            {
                source[i] = (char)random.Next(0, 127);
            }

            stream.Write(Encoding.ASCII.GetBytes(source), 0, source.Length);
            stream.Position = 0;

            using var reader = new BinaryReader(stream, Encoding.ASCII);

            var destination = new char[destinationSize];

            int readCount = await reader.ReadAsync(destination, index, count);

            Assert.Equal(expectedReadLength, readCount);
            Assert.Equal(source.Take(readCount), destination.Skip(index).Take(readCount));

            // Make sure we didn't write past the end
            Assert.True(destination.Skip(readCount + index).All(b => b == default(char)));
        }

        [Theory]
        [InlineData(new[] { 'h', 'e', 'l', 'l', 'o' }, 5, new[] { 'h', 'e', 'l', 'l', 'o' })]
        [InlineData(new[] { 'h', 'e', 'l', 'l', 'o' }, 8, new[] { 'h', 'e', 'l', 'l', 'o' })]
        [InlineData(new[] { 'h', 'e', '\0', '\0', 'o' }, 5, new[] { 'h', 'e', '\0', '\0', 'o' })]
        [InlineData(new[] { 'h', 'e', 'l', 'l', 'o' }, 0, new char[0])]
        [InlineData(new char[0], 5, new char[0])]
        public async Task ReadCharsAsync(char[] source, int readLength, char[] expected)
        {
            using var stream = CreateStream();

            stream.Write(Encoding.ASCII.GetBytes(source), 0, source.Length);
            stream.Position = 0;

            using var reader = new BinaryReader(stream);
            var destination = await reader.ReadCharsAsync(readLength);

            Assert.Equal(expected, destination);
        }

        // ChunkingStream returns less than requested
        private sealed class ChunkingStream : MemoryStream
        {
            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                return base.ReadAsync(buffer, offset, count > 10 ? count - 3 : count, cancellationToken);
            }

            public override ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken)
            {
                return base.ReadAsync(destination.Length > 10 ? destination.Slice(0, destination.Length - 3) : destination, cancellationToken);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ReadCharsAsync_OverReads(bool unicodeEncoding)
        {
            Encoding encoding = unicodeEncoding ? Encoding.Unicode : Encoding.UTF8;

            char[] data1 = "hello world \ud83d\ude03!".ToCharArray(); // 14 code points, 15 chars in UTF-16, 17 bytes in UTF-8
            uint data2 = 0xABCDEF01;

            using Stream stream = new ChunkingStream();
            await using BinaryWriter writer = new BinaryWriter(stream, encoding, leaveOpen: true);

            await writer.WriteAsync(data1);
            await writer.WriteAsync(data2);

            stream.Seek(0, SeekOrigin.Begin);
            using BinaryReader reader = new BinaryReader(stream, encoding, leaveOpen: true);

            Assert.Equal(data1, await reader.ReadCharsAsync(data1.Length));
            Assert.Equal(data2, await reader.ReadUInt32Async());
        }

        [Theory]
        [InlineData(100, 100, 100)]
        [InlineData(100, 50, 50)]
        [InlineData(50, 100, 50)]
        [InlineData(10, 0, 0)]
        [InlineData(0, 10, 0)]
        public async Task ReadAsync_ByteSpan(int sourceSize, int destinationSize, int expectedReadLength)
        {
            using var stream = CreateStream();

            var source = new byte[sourceSize];
            new Random(345).NextBytes(source);
            stream.Write(source, 0, source.Length);
            stream.Position = 0;

            using var reader = new BinaryReader(stream);
            var destination = new byte[destinationSize];

            int readCount = await reader.ReadAsync(new Memory<byte>(destination));

            Assert.Equal(expectedReadLength, readCount);
            Assert.Equal(source.Take(expectedReadLength), destination.Take(expectedReadLength));

            // Make sure we didn't write past the end
            Assert.True(destination.Skip(expectedReadLength).All(b => b == default(byte)));
        }

        [Fact]
        public async Task ReadAsync_ByteSpan_ThrowIfDisposed()
        {
            using var memStream = CreateStream();
            var binaryReader = new BinaryReader(memStream);
            binaryReader.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryReader.ReadAsync(new Memory<byte>()).AsTask());
        }

        [Theory]
        [InlineData(100, 100, 100)]
        [InlineData(100, 50, 50)]
        [InlineData(50, 100, 50)]
        [InlineData(10, 0, 0)]
        [InlineData(0, 10, 0)]
        public async Task ReadAsync_CharSpan(int sourceSize, int destinationSize, int expectedReadLength)
        {
            using var stream = CreateStream();

            var source = new char[sourceSize];
            var random = new Random(345);

            for (int i = 0; i < sourceSize; i++)
            {
                source[i] = (char)random.Next(0, 127);
            }

            stream.Write(Encoding.ASCII.GetBytes(source), 0, source.Length);
            stream.Position = 0;

            using var reader = new BinaryReader(stream, Encoding.ASCII);
            var destination = new char[destinationSize];

            int readCount = await reader.ReadAsync(new Memory<char>(destination));

            Assert.Equal(expectedReadLength, readCount);
            Assert.Equal(source.Take(expectedReadLength), destination.Take(expectedReadLength));

            // Make sure we didn't write past the end
            Assert.True(destination.Skip(expectedReadLength).All(b => b == default(char)));
        }

        [Fact]
        public async Task ReadAsync_CharSpan_ThrowIfDisposed()
        {
            using var memStream = CreateStream();
            var binaryReader = new BinaryReader(memStream);
            binaryReader.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(() => binaryReader.ReadAsync(new Memory<char>()).AsTask());
        }
    }
}
