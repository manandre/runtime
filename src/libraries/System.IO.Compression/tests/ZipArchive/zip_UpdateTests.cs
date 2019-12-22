// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Compression.Tests
{
    public class zip_UpdateTests : ZipFileTestBase
    {
        [Theory]
        [InlineData("normal.zip", "normal")]
        [InlineData("fake64.zip", "small")]
        [InlineData("empty.zip", "empty")]
        [InlineData("appended.zip", "small")]
        [InlineData("prepended.zip", "small")]
        [InlineData("emptydir.zip", "emptydir")]
        [InlineData("small.zip", "small")]
        [InlineData("unicode.zip", "unicode")]
        public static async Task UpdateReadNormal(string zipFile, string zipFolder)
        {
            IsZipSameAsDir(await StreamHelpers.CreateTempCopyStream(zfile(zipFile)), zfolder(zipFolder), ZipArchiveMode.Update, requireExplicit: true, checkTimes: true);
        }

        [Fact]
        public static async Task UpdateReadTwice()
        {
            using (ZipArchive archive = new ZipArchive(await StreamHelpers.CreateTempCopyStream(zfile("small.zip")), ZipArchiveMode.Update))
            {
                ZipArchiveEntry entry = archive.Entries[0];
                string contents1, contents2;
                using (StreamReader s = new StreamReader(entry.Open()))
                {
                    contents1 = s.ReadToEnd();
                }
                using (StreamReader s = new StreamReader(entry.Open()))
                {
                    contents2 = s.ReadToEnd();
                }
                Assert.Equal(contents1, contents2);
            }
        }

        [Theory]
        [InlineData("normal", false)]
        [InlineData("empty", false)]
        [InlineData("unicode", false)]
        [InlineData("normal", true)]
        [InlineData("empty", true)]
        [InlineData("unicode", true)]
        public static async Task UpdateCreate(string zipFolder, bool disposeAsync)
        {
            var zs = new LocalMemoryStream();
            await CreateFromDir(zfolder(zipFolder), zs, ZipArchiveMode.Update, disposeAsync: disposeAsync);
            IsZipSameAsDir(zs.Clone(), zfolder(zipFolder), ZipArchiveMode.Read, requireExplicit: true, checkTimes: true);
        }

        [Theory]
        [InlineData(ZipArchiveMode.Create, false)]
        [InlineData(ZipArchiveMode.Update, true)]
        public static async Task EmptyEntryTest(ZipArchiveMode mode, bool disposeAsync)
        {
            string data1 = "test data written to file.";
            string data2 = "more test data written to file.";
            DateTimeOffset lastWrite = new DateTimeOffset(1992, 4, 5, 12, 00, 30, new TimeSpan(-5, 0, 0));

            var baseline = new LocalMemoryStream();
            ZipArchive archive = new ZipArchive(baseline, mode);
            AddEntry(archive, "data1.txt", data1, lastWrite);

            ZipArchiveEntry e = archive.CreateEntry("empty.txt");
            e.LastWriteTime = lastWrite;
            using (Stream s = e.Open()) { }

            AddEntry(archive, "data2.txt", data2, lastWrite);
            await archive.DisposeAsync(disposeAsync);

            var test = new LocalMemoryStream();
            archive = new ZipArchive(test, mode);
            AddEntry(archive, "data1.txt", data1, lastWrite);

            e = archive.CreateEntry("empty.txt");
            e.LastWriteTime = lastWrite;

            AddEntry(archive, "data2.txt", data2, lastWrite);
            await archive.DisposeAsync(disposeAsync);
            //compare
            Assert.True(ArraysEqual(baseline.ToArray(), test.ToArray()), "Arrays didn't match");

            //second test, this time empty file at end
            baseline = baseline.Clone();
            archive = new ZipArchive(baseline, mode);
            AddEntry(archive, "data1.txt", data1, lastWrite);

            e = archive.CreateEntry("empty.txt");
            e.LastWriteTime = lastWrite;
            using (Stream s = e.Open()) { }
            await archive.DisposeAsync(disposeAsync);

            test = test.Clone();
            archive = new ZipArchive(test, mode);
            AddEntry(archive, "data1.txt", data1, lastWrite);

            e = archive.CreateEntry("empty.txt");
            e.LastWriteTime = lastWrite;
            await archive.DisposeAsync(disposeAsync);
            //compare
            Assert.True(ArraysEqual(baseline.ToArray(), test.ToArray()), "Arrays didn't match after update");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static async Task DeleteAndMoveEntries(bool disposeAsync)
        {
            //delete and move
            var testArchive = await StreamHelpers.CreateTempCopyStream(zfile("normal.zip"));

            ZipArchive archive = new ZipArchive(testArchive, ZipArchiveMode.Update, true);
            ZipArchiveEntry toBeDeleted = archive.GetEntry("binary.wmv");
            toBeDeleted.Delete();
            toBeDeleted.Delete(); //delete twice should be okay
            ZipArchiveEntry moved = archive.CreateEntry("notempty/secondnewname.txt");
            ZipArchiveEntry orig = archive.GetEntry("notempty/second.txt");
            using (Stream origMoved = orig.Open(), movedStream = moved.Open())
            {
                origMoved.CopyTo(movedStream);
            }
            moved.LastWriteTime = orig.LastWriteTime;
            orig.Delete();
            await archive.DisposeAsync(disposeAsync);

            IsZipSameAsDir(testArchive, zmodified("deleteMove"), ZipArchiveMode.Read, requireExplicit: true, checkTimes: true);

        }
        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public static async Task AppendToEntry(bool writeWithSpans, bool disposeAsync)
        {
            //append
            Stream testArchive = await StreamHelpers.CreateTempCopyStream(zfile("normal.zip"));

            ZipArchive archive = new ZipArchive(testArchive, ZipArchiveMode.Update, true);
            ZipArchiveEntry e = archive.GetEntry("first.txt");
            using (Stream s = e.Open())
            {
                s.Seek(0, SeekOrigin.End);

                byte[] data = Encoding.ASCII.GetBytes("\r\n\r\nThe answer my friend, is blowin' in the wind.");
                if (writeWithSpans)
                {
                    s.Write(data, 0, data.Length);
                }
                else
                {
                    s.Write(new ReadOnlySpan<byte>(data));
                }
            }

            var file = FileData.GetFile(zmodified(Path.Combine("append", "first.txt")));
            e.LastWriteTime = file.LastModifiedDate;
            await archive.DisposeAsync(disposeAsync);

            IsZipSameAsDir(testArchive, zmodified("append"), ZipArchiveMode.Read, requireExplicit: true, checkTimes: true);

        }
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static async Task OverwriteEntry(bool disposeAsync)
        {
            //Overwrite file
            Stream testArchive = await StreamHelpers.CreateTempCopyStream(zfile("normal.zip"));

            ZipArchive archive = new ZipArchive(testArchive, ZipArchiveMode.Update, true);

            string fileName = zmodified(Path.Combine("overwrite", "first.txt"));
            ZipArchiveEntry e = archive.GetEntry("first.txt");

            var file = FileData.GetFile(fileName);
            e.LastWriteTime = file.LastModifiedDate;

            using (var stream = await StreamHelpers.CreateTempCopyStream(fileName))
            {
                using (Stream es = e.Open())
                {
                    es.SetLength(0);
                    stream.CopyTo(es);
                }
            }
            await archive.DisposeAsync(disposeAsync);

            IsZipSameAsDir(testArchive, zmodified("overwrite"), ZipArchiveMode.Read, requireExplicit: true, checkTimes: true);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static async Task AddFileToArchive(bool disposeAsync)
        {
            //add file
            var testArchive = await StreamHelpers.CreateTempCopyStream(zfile("normal.zip"));

            ZipArchive archive = new ZipArchive(testArchive, ZipArchiveMode.Update, true);
            await updateArchive(archive, zmodified(Path.Combine("addFile", "added.txt")), "added.txt");
            await archive.DisposeAsync(disposeAsync);

            IsZipSameAsDir(testArchive, zmodified("addFile"), ZipArchiveMode.Read, requireExplicit: true, checkTimes: true);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static async Task AddFileToArchive_AfterReading(bool disposeAsync)
        {
            //add file and read entries before
            Stream testArchive = await StreamHelpers.CreateTempCopyStream(zfile("normal.zip"));

            ZipArchive archive = new ZipArchive(testArchive, ZipArchiveMode.Update, true);
            var x = archive.Entries;

            await updateArchive(archive, zmodified(Path.Combine("addFile", "added.txt")), "added.txt");
            await archive.DisposeAsync(disposeAsync);

            IsZipSameAsDir(testArchive, zmodified("addFile"), ZipArchiveMode.Read, requireExplicit: true, checkTimes: true);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static async Task AddFileToArchive_ThenReadEntries(bool disposeAsync)
        {
            //add file and read entries after
            Stream testArchive = await StreamHelpers.CreateTempCopyStream(zfile("normal.zip"));

            ZipArchive archive = new ZipArchive(testArchive, ZipArchiveMode.Update, true);
            await updateArchive(archive, zmodified(Path.Combine("addFile", "added.txt")), "added.txt");

            var x = archive.Entries;
            await archive.DisposeAsync(disposeAsync);

            IsZipSameAsDir(testArchive, zmodified("addFile"), ZipArchiveMode.Read, requireExplicit: true, checkTimes: true);
        }

        private static async Task updateArchive(ZipArchive archive, string installFile, string entryName)
        {
            ZipArchiveEntry e = archive.CreateEntry(entryName);

            var file = FileData.GetFile(installFile);
            e.LastWriteTime = file.LastModifiedDate;
            Assert.Equal(e.LastWriteTime, file.LastModifiedDate);

            using (var stream = await StreamHelpers.CreateTempCopyStream(installFile))
            {
                using (Stream es = e.Open())
                {
                    es.SetLength(0);
                    stream.CopyTo(es);
                }
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static async Task UpdateModeInvalidOperations(bool disposeAsync)
        {
            using (LocalMemoryStream ms = await LocalMemoryStream.readAppFileAsync(zfile("normal.zip")))
            {
                ZipArchive target = new ZipArchive(ms, ZipArchiveMode.Update, leaveOpen: true);

                ZipArchiveEntry edeleted = target.GetEntry("first.txt");

                Stream s = edeleted.Open();
                //invalid ops while entry open
                Assert.Throws<IOException>(() => edeleted.Open());
                Assert.Throws<InvalidOperationException>(() => { var x = edeleted.Length; });
                Assert.Throws<InvalidOperationException>(() => { var x = edeleted.CompressedLength; });
                Assert.Throws<IOException>(() => edeleted.Delete());
                s.Dispose();

                //invalid ops on stream after entry closed
                Assert.Throws<ObjectDisposedException>(() => s.ReadByte());

                Assert.Throws<InvalidOperationException>(() => { var x = edeleted.Length; });
                Assert.Throws<InvalidOperationException>(() => { var x = edeleted.CompressedLength; });

                edeleted.Delete();
                //invalid ops while entry deleted
                Assert.Throws<InvalidOperationException>(() => edeleted.Open());
                Assert.Throws<InvalidOperationException>(() => { edeleted.LastWriteTime = new DateTimeOffset(); });

                ZipArchiveEntry e = target.GetEntry("notempty/second.txt");

                await target.DisposeAsync(disposeAsync);

                Assert.Throws<ObjectDisposedException>(() => { var x = target.Entries; });
                Assert.Throws<ObjectDisposedException>(() => target.CreateEntry("dirka"));
                Assert.Throws<ObjectDisposedException>(() => e.Open());
                Assert.Throws<ObjectDisposedException>(() => e.Delete());
                Assert.Throws<ObjectDisposedException>(() => { e.LastWriteTime = new DateTimeOffset(); });
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task UpdateUncompressedArchive(bool disposeAsync)
        {
            var utf8WithoutBom = new Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            byte[] fileContent;
            using (var memStream = new MemoryStream())
            {
                var zip = new ZipArchive(memStream, ZipArchiveMode.Create);

                ZipArchiveEntry entry = zip.CreateEntry("testing", CompressionLevel.NoCompression);
                using (var writer = new StreamWriter(entry.Open(), utf8WithoutBom))
                {
                    writer.Write("hello");
                    writer.Flush();
                }
                await zip.DisposeAsync(disposeAsync);
                fileContent = memStream.ToArray();
            }
            byte compressionMethod = fileContent[8];
            Assert.Equal(0, compressionMethod); // stored => 0, deflate => 8
            using (var memStream = new MemoryStream())
            {
                memStream.Write(fileContent);
                memStream.Position = 0;
                var archive = new ZipArchive(memStream, ZipArchiveMode.Update);
                ZipArchiveEntry entry = archive.GetEntry("testing");
                using (var writer = new StreamWriter(entry.Open(), utf8WithoutBom))
                {
                    writer.Write("new");
                    writer.Flush();
                }
                await archive.DisposeAsync(disposeAsync);
                byte[] modifiedTestContent = memStream.ToArray();
                compressionMethod = modifiedTestContent[8];
                Assert.Equal(0, compressionMethod); // stored => 0, deflate => 8
            }
        }
    }
}
