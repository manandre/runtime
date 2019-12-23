// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Tests
{
    public partial class StreamMethods
    {
        protected virtual Stream CreateStream()
        {
            return new MemoryStream();
        }

        protected virtual Stream CreateStream(int bufferSize)
        {
            return new MemoryStream(new byte[bufferSize]);
        }

        [Fact]
        public void MemoryStreamSeekStress()
        {
            var ms1 = CreateStream();
            SeekTest(ms1, false);
        }
        [Fact]
        public void MemoryStreamSeekStressWithInitialBuffer()
        {
            var ms1 = CreateStream(1024);
            SeekTest(ms1, false);
        }

        [Fact]
        public void MemoryStreamStress()
        {
            var ms1 = CreateStream();
            StreamTest(ms1, false);
        }

        [Fact]
        public async Task MemoryStreamStressAsync()
        {
            var ms1 = CreateStream();
            await StreamTestAsync(ms1, false);
        }

        private static void SeekTest(Stream stream, bool fSuppres)
        {
            long lngPos;
            byte btValue;

            stream.Position = 0;

            Assert.Equal(0, stream.Position);

            int length = 1 << 10; //fancy way of writing 2 to the pow 10
            byte[] btArr = new byte[length];
            for (int i = 0; i < btArr.Length; i++)
                btArr[i] = unchecked((byte)i);

            if (stream.CanWrite)
                stream.Write(btArr, 0, btArr.Length);
            else
                stream.Position = btArr.Length;

            Assert.Equal(btArr.Length, stream.Position);

            lngPos = stream.Seek(0, SeekOrigin.Begin);
            Assert.Equal(0, lngPos);

            Assert.Equal(0, stream.Position);

            for (int i = 0; i < btArr.Length; i++)
            {
                if (stream.CanWrite)
                {
                    btValue = (byte)stream.ReadByte();
                    Assert.Equal(btArr[i], btValue);
                }
                else
                {
                    stream.Seek(1, SeekOrigin.Current);
                }
                Assert.Equal(i + 1, stream.Position);
            }

            Assert.Throws<IOException>(() => stream.Seek(-5, SeekOrigin.Begin));

            lngPos = stream.Seek(5, SeekOrigin.Begin);
            Assert.Equal(5, lngPos);
            Assert.Equal(5, stream.Position);

            lngPos = stream.Seek(5, SeekOrigin.End);
            Assert.Equal(length + 5, lngPos);
            Assert.Throws<IOException>(() => stream.Seek(-(btArr.Length + 1), SeekOrigin.End));

            lngPos = stream.Seek(-5, SeekOrigin.End);
            Assert.Equal(btArr.Length - 5, lngPos);
            Assert.Equal(btArr.Length - 5, stream.Position);

            lngPos = stream.Seek(0, SeekOrigin.End);
            Assert.Equal(btArr.Length, stream.Position);

            for (int i = btArr.Length; i > 0; i--)
            {
                stream.Seek(-1, SeekOrigin.Current);
                Assert.Equal(i - 1, stream.Position);
            }

            Assert.Throws<IOException>(() => stream.Seek(-1, SeekOrigin.Current));
        }

        private static void StreamTest(Stream stream, bool fSuppress)
        {
            string strValue;

            //[] We will first use the stream's 2 writing methods
            int iLength = 1 << 10;
            stream.Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < iLength; i++)
                stream.WriteByte(unchecked((byte)i));

            byte[] btArr = new byte[iLength];
            for (int i = 0; i < iLength; i++)
                btArr[i] = unchecked((byte)i);
            stream.Write(btArr, 0, iLength);

            //we will write many things here using a binary writer
            BinaryWriter bw1 = new BinaryWriter(stream);
            bw1.Write(false);
            bw1.Write(true);

            for (int i = 0; i < 10; i++)
            {
                bw1.Write((byte)i);
                bw1.Write((sbyte)i);
                bw1.Write((short)i);
                bw1.Write((char)i);
                bw1.Write((ushort)i);
                bw1.Write(i);
                bw1.Write((uint)i);
                bw1.Write((long)i);
                bw1.Write((ulong)i);
                bw1.Write((float)i);
                bw1.Write((double)i);
            }

            //Some strings, chars and Bytes
            char[] chArr = new char[iLength];
            for (int i = 0; i < iLength; i++)
                chArr[i] = (char)i;

            bw1.Write(chArr);
            bw1.Write(chArr, 512, 512);

            bw1.Write(new string(chArr));
            bw1.Write(new string(chArr));

            //[] we will now read
            stream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < iLength; i++)
            {
                Assert.Equal(i % 256, stream.ReadByte());
            }

            btArr = new byte[iLength];
            stream.Read(btArr, 0, iLength);
            for (int i = 0; i < iLength; i++)
            {
                Assert.Equal(unchecked((byte)i), btArr[i]);
            }

            //Now, for the binary reader
            BinaryReader br1 = new BinaryReader(stream);

            Assert.False(br1.ReadBoolean());
            Assert.True(br1.ReadBoolean());

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal((byte)i, br1.ReadByte());
                Assert.Equal((sbyte)i, br1.ReadSByte());
                Assert.Equal((short)i, br1.ReadInt16());
                Assert.Equal((char)i, br1.ReadChar());
                Assert.Equal((ushort)i, br1.ReadUInt16());
                Assert.Equal(i, br1.ReadInt32());
                Assert.Equal((uint)i, br1.ReadUInt32());
                Assert.Equal((long)i, br1.ReadInt64());
                Assert.Equal((ulong)i, br1.ReadUInt64());
                Assert.Equal((float)i, br1.ReadSingle());
                Assert.Equal((double)i, br1.ReadDouble());
            }

            chArr = br1.ReadChars(iLength);
            for (int i = 0; i < iLength; i++)
            {
                Assert.Equal((char)i, chArr[i]);
            }

            chArr = new char[512];
            chArr = br1.ReadChars(iLength / 2);
            for (int i = 0; i < iLength / 2; i++)
            {
                Assert.Equal((char)(iLength / 2 + i), chArr[i]);
            }

            chArr = new char[iLength];
            for (int i = 0; i < iLength; i++)
                chArr[i] = (char)i;
            strValue = br1.ReadString();
            Assert.Equal(new string(chArr), strValue);

            strValue = br1.ReadString();
            Assert.Equal(new string(chArr), strValue);

            stream.Seek(1, SeekOrigin.Current);
        }

        private static async Task StreamTestAsync(Stream stream, bool fSuppress)
        {
            string strValue;

            //[] We will first use the stream's 2 writing methods
            int iLength = 1 << 10;
            stream.Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < iLength; i++)
                await stream.WriteByteAsync(unchecked((byte)i));

            byte[] btArr = new byte[iLength];
            for (int i = 0; i < iLength; i++)
                btArr[i] = unchecked((byte)i);
            await stream.WriteAsync(btArr, 0, iLength);

            //we will write many things here using a binary writer
            BinaryWriter bw1 = new BinaryWriter(stream);
            await bw1.WriteAsync(false);
            await bw1.WriteAsync(true);

            for (int i = 0; i < 10; i++)
            {
                await bw1.WriteAsync((byte)i);
                await bw1.WriteAsync((sbyte)i);
                await bw1.WriteAsync((short)i);
                await bw1.WriteAsync((char)i);
                await bw1.WriteAsync((ushort)i);
                await bw1.WriteAsync(i);
                await bw1.WriteAsync((uint)i);
                await bw1.WriteAsync((long)i);
                await bw1.WriteAsync((ulong)i);
                await bw1.WriteAsync((float)i);
                await bw1.WriteAsync((double)i);
            }

            //Some strings, chars and Bytes
            char[] chArr = new char[iLength];
            for (int i = 0; i < iLength; i++)
                chArr[i] = (char)i;

            await bw1.WriteAsync(chArr);
            await bw1.WriteAsync(chArr, 512, 512);

            await bw1.WriteAsync(new string(chArr));
            await bw1.WriteAsync(new string(chArr));

            //[] we will now read
            stream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < iLength; i++)
            {
                Assert.Equal(i % 256, await stream.ReadByteAsync());
            }

            btArr = new byte[iLength];
            await stream.ReadAsync(btArr, 0, iLength);
            for (int i = 0; i < iLength; i++)
            {
                Assert.Equal(unchecked((byte)i), btArr[i]);
            }

            //Now, for the binary reader
            BinaryReader br1 = new BinaryReader(stream);

            Assert.False(br1.ReadBoolean());
            Assert.True(br1.ReadBoolean());

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal((byte)i, await br1.ReadByteAsync());
                Assert.Equal((sbyte)i, await br1.ReadSByteAsync());
                Assert.Equal((short)i, await br1.ReadInt16Async());
                Assert.Equal((char)i, await br1.ReadCharAsync());
                Assert.Equal((ushort)i, await br1.ReadUInt16Async());
                Assert.Equal(i, await br1.ReadInt32Async());
                Assert.Equal((uint)i, await br1.ReadUInt32Async());
                Assert.Equal((long)i, await br1.ReadInt64Async());
                Assert.Equal((ulong)i, await br1.ReadUInt64Async());
                Assert.Equal((float)i, await br1.ReadSingleAsync());
                Assert.Equal((double)i, await br1.ReadDoubleAsync());
            }

            chArr = await br1.ReadCharsAsync(iLength);
            for (int i = 0; i < iLength; i++)
            {
                Assert.Equal((char)i, chArr[i]);
            }

            chArr = new char[512];
            chArr = await br1.ReadCharsAsync(iLength / 2);
            for (int i = 0; i < iLength / 2; i++)
            {
                Assert.Equal((char)(iLength / 2 + i), chArr[i]);
            }

            chArr = new char[iLength];
            for (int i = 0; i < iLength; i++)
                chArr[i] = (char)i;
            strValue = await br1.ReadStringAsync();
            Assert.Equal(new string(chArr), strValue);

            strValue = await br1.ReadStringAsync();
            Assert.Equal(new string(chArr), strValue);

            stream.Seek(1, SeekOrigin.Current);
        }

        [Fact]
        public async Task FlushAsyncTest()
        {
            byte[] data = Enumerable.Range(0, 8000).Select(i => unchecked((byte)i)).ToArray();
            Stream stream = CreateStream();

            for (int i = 0; i < 4; i++)
            {
                await stream.WriteAsync(data, 2000 * i, 2000);
                await stream.FlushAsync();
            }

            stream.Position = 0;
            byte[] output = new byte[data.Length];
            int bytesRead, totalRead = 0;
            while ((bytesRead = await stream.ReadAsync(output, totalRead, data.Length - totalRead)) > 0)
                totalRead += bytesRead;
            Assert.Equal(data, output);
        }

        [Fact]
        public void ArgumentValidation()
        {
            Stream stream = CreateStream();
            Assert.Equal(TaskStatus.Canceled, stream.ReadAsync(new byte[1], 0, 1, new CancellationToken(canceled: true)).Status);
            Assert.Equal(TaskStatus.Canceled, stream.WriteAsync(new byte[1], 0, 1, new CancellationToken(canceled: true)).Status);
            Assert.Equal(TaskStatus.Canceled, stream.FlushAsync(new CancellationToken(canceled: true)).Status);

            AssertExtensions.Throws<ArgumentNullException>("buffer", () => { stream.ReadAsync(null, 0, 0); });
            AssertExtensions.Throws<ArgumentNullException>("buffer", () => { stream.WriteAsync(null, 0, 0); });

            AssertExtensions.Throws<ArgumentOutOfRangeException>("offset", () => { stream.ReadAsync(new byte[1], -1, 0); });
            AssertExtensions.Throws<ArgumentOutOfRangeException>("offset", () => { stream.WriteAsync(new byte[1], -1, 0); });

            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => { stream.ReadAsync(new byte[1], 0, -1); });
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => { stream.WriteAsync(new byte[1], 0, -1); });

            AssertExtensions.Throws<ArgumentException>(null, () => { stream.ReadAsync(new byte[1], 0, 2); });
            AssertExtensions.Throws<ArgumentException>(null, () => { stream.WriteAsync(new byte[1], 0, 2); });
        }
    }
}
