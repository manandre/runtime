// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;
using Microsoft.DotNet.XUnitExtensions;

namespace System.IO.Compression.Tests
{
    public class zip_CreateTests : ZipFileTestBase
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static async Task CreateModeInvalidOperations(bool disposeAsync)
        {
            MemoryStream ms = new MemoryStream();
            ZipArchive z = new ZipArchive(ms, ZipArchiveMode.Create);
            Assert.Throws<NotSupportedException>(() => { var x = z.Entries; }); //"Entries not applicable on Create"
            Assert.Throws<NotSupportedException>(() => z.GetEntry("dirka")); //"GetEntry not applicable on Create"

            ZipArchiveEntry e = z.CreateEntry("hey");
            Assert.Throws<NotSupportedException>(() => e.Delete()); //"Can't delete new entry"

            Stream s = e.Open();
            Assert.Throws<NotSupportedException>(() => s.ReadByte()); //"Can't read on new entry"
            Assert.Throws<NotSupportedException>(() => s.Seek(0, SeekOrigin.Begin)); //"Can't seek on new entry"
            Assert.Throws<NotSupportedException>(() => s.Position = 0); //"Can't set position on new entry"
            Assert.Throws<NotSupportedException>(() => { var x = s.Length; }); //"Can't get length on new entry"

            Assert.Throws<IOException>(() => e.LastWriteTime = new DateTimeOffset()); //"Can't get LastWriteTime on new entry"
            Assert.Throws<InvalidOperationException>(() => { var x = e.Length; }); //"Can't get length on new entry"
            Assert.Throws<InvalidOperationException>(() => { var x = e.CompressedLength; }); //"can't get CompressedLength on new entry"

            Assert.Throws<IOException>(() => z.CreateEntry("bad"));
            s.Dispose();

            Assert.Throws<ObjectDisposedException>(() => s.WriteByte(25)); //"Can't write to disposed entry"

            Assert.Throws<IOException>(() => e.Open());
            Assert.Throws<IOException>(() => e.LastWriteTime = new DateTimeOffset());
            Assert.Throws<InvalidOperationException>(() => { var x = e.Length; });
            Assert.Throws<InvalidOperationException>(() => { var x = e.CompressedLength; });

            ZipArchiveEntry e1 = z.CreateEntry("e1");
            ZipArchiveEntry e2 = z.CreateEntry("e2");

            Assert.Throws<IOException>(() => e1.Open()); //"Can't open previous entry after new entry created"

            await z.DisposeAsync(disposeAsync);

            Assert.Throws<ObjectDisposedException>(() => z.CreateEntry("dirka")); //"Can't create after dispose"
        }

        [Theory]
        [InlineData("small", false, false, false)]
        [InlineData("normal", false, false, false)]
        [InlineData("empty", false, false, false)]
        [InlineData("emptydir", false, false, false)]
        [InlineData("small", true, false, false)]
        [InlineData("normal", true, false, false)]
        [InlineData("small", false, true, false)]
        [InlineData("normal", false, true, false)]
        [InlineData("small", false, false, true)]
        [InlineData("normal", false, false, true)]
        [InlineData("empty", false, false, true)]
        [InlineData("emptydir", false, false, true)]
        [InlineData("small", true, false, true)]
        [InlineData("normal", true, false, true)]
        [InlineData("small", false, true, true)]
        [InlineData("normal", false, true, true)]
        public static async Task CreateNormal_Seekable(string folder, bool useSpansForWriting, bool writeInChunks, bool disposeAsync)
        {
            using (var s = new MemoryStream())
            {
                var testStream = new WrappedStream(s, false, true, true, null);
                await CreateFromDir(zfolder(folder), testStream, ZipArchiveMode.Create, useSpansForWriting, writeInChunks, disposeAsync);

                IsZipSameAsDir(s, zfolder(folder), ZipArchiveMode.Read, requireExplicit: true, checkTimes: true);
            }
        }

        [Theory]
        [InlineData("small", false)]
        [InlineData("normal", false)]
        [InlineData("empty", false)]
        [InlineData("emptydir", false)]
        [InlineData("small", true)]
        [InlineData("normal", true)]
        [InlineData("empty", true)]
        [InlineData("emptydir", true)]
        public static async Task CreateNormal_Unseekable(string folder, bool disposeAsync)
        {
            using (var s = new MemoryStream())
            {
                var testStream = new WrappedStream(s, false, true, false, null);
                await CreateFromDir(zfolder(folder), testStream, ZipArchiveMode.Create, disposeAsync: disposeAsync);

                IsZipSameAsDir(s, zfolder(folder), ZipArchiveMode.Read, requireExplicit: true, checkTimes: true);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [Trait(XunitConstants.Category, XunitConstants.IgnoreForCI)] // Jenkins fails with unicode characters [JENKINS-12610]
        public static async Task CreateNormal_Unicode_Seekable(bool disposeAsync)
        {
            using (var s = new MemoryStream())
            {
                var testStream = new WrappedStream(s, false, true, true, null);
                await CreateFromDir(zfolder("unicode"), testStream, ZipArchiveMode.Create, disposeAsync: disposeAsync);

                IsZipSameAsDir(s, zfolder("unicode"), ZipArchiveMode.Read, requireExplicit: true, checkTimes: true);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [Trait(XunitConstants.Category, XunitConstants.IgnoreForCI)] // Jenkins fails with unicode characters [JENKINS-12610]
        public static async Task CreateNormal_Unicode_Unseekable(bool disposeAsync)
        {
            using (var s = new MemoryStream())
            {
                var testStream = new WrappedStream(s, false, true, false, null);
                await CreateFromDir(zfolder("unicode"), testStream, ZipArchiveMode.Create, disposeAsync: disposeAsync);

                IsZipSameAsDir(s, zfolder("unicode"), ZipArchiveMode.Read, requireExplicit: true, checkTimes: true);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static async Task CreateUncompressedArchive(bool disposeAsync)
        {
            using (var testStream = new MemoryStream())
            {
                var testfilename = "testfile";
                var testFileContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
                var zip = new ZipArchive(testStream, ZipArchiveMode.Create);

                var utf8WithoutBom = new Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
                ZipArchiveEntry newEntry = zip.CreateEntry(testfilename, CompressionLevel.NoCompression);
                using (var writer = new StreamWriter(newEntry.Open(), utf8WithoutBom))
                {
                    writer.Write(testFileContent);
                    writer.Flush();
                }
                byte[] fileContent = testStream.ToArray();
                // zip file header stores values as little-endian
                byte compressionMethod = fileContent[8];
                Assert.Equal(0, compressionMethod); // stored => 0, deflate => 8
                uint compressedSize = BitConverter.ToUInt32(fileContent, 18);
                uint uncompressedSize = BitConverter.ToUInt32(fileContent, 22);
                Assert.Equal(uncompressedSize, compressedSize);
                byte filenamelength = fileContent[26];
                Assert.Equal(testfilename.Length, filenamelength);
                string readFileName = ReadStringFromSpan(fileContent.AsSpan(30, filenamelength));
                Assert.Equal(testfilename, readFileName);
                string readFileContent = ReadStringFromSpan(fileContent.AsSpan(30 + filenamelength, testFileContent.Length));
                Assert.Equal(testFileContent, readFileContent);
                await zip.DisposeAsync(disposeAsync);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static async Task CreateNormal_VerifyDataDescriptor(bool disposeAsync)
        {
            using var memoryStream = new MemoryStream();
            // We need an non-seekable stream so the data descriptor bit is turned on when saving
            var wrappedStream = new WrappedStream(memoryStream, true, true, false, null);

            // Creation will go through the path that sets the data descriptor bit when the stream is unseekable
            var archive = new ZipArchive(wrappedStream, ZipArchiveMode.Create);
            CreateEntry(archive, "A", "xxx");
            CreateEntry(archive, "B", "yyy");
            await archive.DisposeAsync(disposeAsync);

            AssertDataDescriptor(memoryStream, true);

            // Update should flip the data descriptor bit to zero on save
            archive = new ZipArchive(memoryStream, ZipArchiveMode.Update);
            ZipArchiveEntry entry = archive.Entries[0];
            using Stream entryStream = entry.Open();
            StreamReader reader = new StreamReader(entryStream);
            string content = reader.ReadToEnd();

            // Append a string to this entry
            entryStream.Seek(0, SeekOrigin.End);
            StreamWriter writer = new StreamWriter(entryStream);
            writer.Write("zzz");
            writer.Flush();
            await archive.DisposeAsync(disposeAsync);

            AssertDataDescriptor(memoryStream, false);
        }

        private static string ReadStringFromSpan(Span<byte> input)
        {
            return Text.Encoding.UTF8.GetString(input.ToArray());
        }

        private static void CreateEntry(ZipArchive archive, string fileName, string fileContents)
        {
            ZipArchiveEntry entry = archive.CreateEntry(fileName);
            using StreamWriter writer = new StreamWriter(entry.Open());
            writer.Write(fileContents);
        }

        private static void AssertDataDescriptor(MemoryStream memoryStream, bool hasDataDescriptor)
        {
            byte[] fileBytes = memoryStream.ToArray();
            Assert.Equal(hasDataDescriptor ? 8 : 0, fileBytes[6]);
            Assert.Equal(0, fileBytes[7]);
        }
    }
}
