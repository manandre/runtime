// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Tests
{
    public class BinaryWriter_WriteAsyncTests
    {
        [Fact]
        public async Task BinaryWriter_WriteBoolAsyncTest()
        {
            // [] Write a series of booleans to a stream
            using(Stream mstr = CreateStream())
            await using(BinaryWriter dw2 = new BinaryWriter(mstr))
            using(BinaryReader dr2 = new BinaryReader(mstr))
            {
                await dw2.WriteAsync(false);
                await dw2.WriteAsync(false);
                await dw2.WriteAsync(true);
                await dw2.WriteAsync(false);
                await dw2.WriteAsync(true);
                await dw2.WriteAsync(5);
                await dw2.WriteAsync(0);

                await dw2.FlushAsync();
                mstr.Position = 0;

                Assert.False(dr2.ReadBoolean()); //false
                Assert.False(dr2.ReadBoolean()); //false
                Assert.True(dr2.ReadBoolean());  //true
                Assert.False(dr2.ReadBoolean()); //false
                Assert.True(dr2.ReadBoolean());  //true
                Assert.Equal(5, dr2.ReadInt32());  //5
                Assert.Equal(0, dr2.ReadInt32()); //0
            }
        }

        [Fact]
        public async Task BinaryWriter_WriteSingleAsyncTest()
        {
            float[] sglArr = new float[] {
                float.MinValue, float.MaxValue, float.Epsilon, float.PositiveInfinity, float.NegativeInfinity, new float(),
                0, (float)(-1E20), (float)(-3.5E-20), (float)(1.4E-10), (float)10000.2, (float)2.3E30
            };

            await WriteAsyncTest(sglArr, (bw, s) => bw.WriteAsync(s), (br) => br.ReadSingle());
        }

        [Fact]
        public async Task BinaryWriter_WriteDecimalAsyncTest()
        {
            decimal[] decArr = new decimal[] {
                decimal.One, decimal.Zero, decimal.MinusOne, decimal.MinValue, decimal.MaxValue,
                new decimal(-1000.5), new decimal(-10.0E-40), new decimal(3.4E-40898), new decimal(3.4E-28),
                new decimal(3.4E+28), new decimal(0.45), new decimal(5.55), new decimal(3.4899E23)
            };

            await WriteAsyncTest(decArr, (bw, s) => bw.WriteAsync(s), (br) => br.ReadDecimal());
        }

        [Fact]
        public async Task BinaryWriter_WriteDoubleAsyncTest()
        {
            double[] dblArr = new double[] {
                double.NegativeInfinity, double.PositiveInfinity, double.Epsilon, double.MinValue, double.MaxValue,
                -3E59, -1000.5, -1E-40, 3.4E-37, 0.45, 5.55, 3.4899E233
            };

            await WriteAsyncTest(dblArr, (bw, s) => bw.WriteAsync(s), (br) => br.ReadDouble());
        }

        [Fact]
        public async Task BinaryWriter_WriteInt16AsyncTest()
        {
            short[] i16Arr = new short[] { short.MinValue, short.MaxValue, 0, -10000, 10000, -50, 50 };

            await WriteAsyncTest(i16Arr, (bw, s) => bw.WriteAsync(s), (br) => br.ReadInt16());
        }

        [Fact]
        public async Task BinaryWriter_WriteInt32AsyncTest()
        {
            int[] i32arr = new int[] { int.MinValue, int.MaxValue, 0, -10000, 10000, -50, 50 };

            await WriteAsyncTest(i32arr, (bw, s) => bw.WriteAsync(s), (br) => br.ReadInt32());
        }

        [Fact]
        public async Task BinaryWriter_WriteInt64AsyncTest()
        {
            long[] i64arr = new long[] { long.MinValue, long.MaxValue, 0, -10000, 10000, -50, 50 };

            await WriteAsyncTest(i64arr, (bw, s) => bw.WriteAsync(s), (br) => br.ReadInt64());
        }

        [Fact]
        public async Task BinaryWriter_WriteUInt16AsyncTest()
        {
            ushort[] ui16Arr = new ushort[] { ushort.MinValue, ushort.MaxValue, 0, 100, 1000, 10000, ushort.MaxValue - 100 };

            await WriteAsyncTest(ui16Arr, (bw, s) => bw.WriteAsync(s), (br) => br.ReadUInt16());
        }

        [Fact]
        public async Task BinaryWriter_WriteUInt32AsyncTest()
        {
            uint[] ui32Arr = new uint[] { uint.MinValue, uint.MaxValue, 0, 100, 1000, 10000, uint.MaxValue - 100 };

            await WriteAsyncTest(ui32Arr, (bw, s) => bw.WriteAsync(s), (br) => br.ReadUInt32());
        }

        [Fact]
        public async Task BinaryWriter_WriteUInt64AsyncTest()
        {
            ulong[] ui64Arr = new ulong[] { ulong.MinValue, ulong.MaxValue, 0, 100, 1000, 10000, ulong.MaxValue - 100 };

            await WriteAsyncTest(ui64Arr, (bw, s) => bw.WriteAsync(s), (br) => br.ReadUInt64());
        }

        [Fact]
        public async Task BinaryWriter_WriteStringAsyncTest()
        {
            StringBuilder sb = new StringBuilder();
            string str1;
            for (int ii = 0; ii < 5; ii++)
                sb.Append("abc");
            str1 = sb.ToString();

            string[] strArr = new string[] {
                "ABC", "\t\t\n\n\n\0\r\r\v\v\t\0\rHello", "This is a normal string", "12345667789!@#$%^&&())_+_)@#",
                "ABSDAFJPIRUETROPEWTGRUOGHJDOLJHLDHWEROTYIETYWsdifhsiudyoweurscnkjhdfusiyugjlskdjfoiwueriye", "     ",
                "\0\0\0\t\t\tHey\"\"", "\u0022\u0011", str1, string.Empty };

            await WriteAsyncTest(strArr, (bw, s) => bw.WriteAsync(s), (br) => br.ReadString());
        }

        [Fact]
        public async Task BinaryWriter_WriteStringAsyncTest_Null()
        {
            using (Stream memStream = CreateStream())
            await using (BinaryWriter dw2 = new BinaryWriter(memStream))
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() => dw2.WriteAsync((string)null).AsTask());
            }
        }

        protected virtual Stream CreateStream()
        {
            return new MemoryStream();
        }

        private async Task WriteAsyncTest<T>(T[] testElements, Func<BinaryWriter, T, ValueTask> writeAsync, Func<BinaryReader, T> read)
        {
            using (Stream memStream = CreateStream())
            await using (BinaryWriter writer = new BinaryWriter(memStream))
            using (BinaryReader reader = new BinaryReader(memStream))
            {
                for (int i = 0; i < testElements.Length; i++)
                {
                    await writeAsync(writer, testElements[i]);
                }

                await writer.FlushAsync();
                memStream.Position = 0;

                for (int i = 0; i < testElements.Length; i++)
                {
                    Assert.Equal(testElements[i], read(reader));
                }

                // We've reached the end of the stream.  Check for expected EndOfStreamException
                Assert.Throws<EndOfStreamException>(() => read(reader));
            }
        }
    }
}
