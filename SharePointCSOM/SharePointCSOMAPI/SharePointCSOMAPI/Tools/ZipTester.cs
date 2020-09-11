using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SharePointCSOMAPI.Tools
{
    public class TestStream : Stream
    {
        private Stream stream;

        public TestStream(Stream stream)
        {
            this.stream = stream;
        }

        public override bool CanRead { get { return stream.CanRead; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return stream.CanWrite; } }

        public override long Length { get { return stream.Length; } }

        public override long Position { get { return stream.Position; } set => throw new NotSupportedException(); }

        public override void Flush()
        {
            stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }
    }
    class ZipTester
    {

        public static void DeleteTest()
        {
            using (ZipArchive archive = ZipFile.Open(@"C:\Users\xluo\Desktop\NewTest.zip", ZipArchiveMode.Update))
            {
                archive.Entries[0].Delete();
            }
        }
        public static void CreateTest()
        {
            using (ZipArchive archive = ZipFile.Open(@"C:\Users\xluo\Desktop\asdf.zip", ZipArchiveMode.Create))
            {
                var e = archive.CreateEntry("1", CompressionLevel.NoCompression);
                using (var writer = e.Open())
                {
                    var buffer = Encoding.UTF8.GetBytes("12345");
                    writer.Write(buffer, 0, buffer.Length);
                }

                var e2 = archive.CreateEntry("2", CompressionLevel.NoCompression);
                using (var writer = e2.Open())
                {
                    var buffer = Encoding.UTF8.GetBytes("12345");
                    writer.Write(buffer, 0, buffer.Length);
                }

                var e3 = archive.CreateEntry("3", CompressionLevel.NoCompression);
                using (var writer = e3.Open())
                {
                    var buffer = Encoding.UTF8.GetBytes("12345");
                    writer.Write(buffer, 0, buffer.Length);
                }

                //using (StreamWriter writer = new StreamWriter(e.Open()))
                //{

                //    writer.WriteLine("Information about this package.");
                //    writer.WriteLine("========================");
                //}

                //var e2 = archive.CreateEntry("2");
                //using (StreamWriter writer = new StreamWriter(e2.Open()))
                //{
                //    writer.WriteLine("Information about this package.");
                //    writer.WriteLine("========================");
                //}

                //var e3 = archive.CreateEntry("3");
                //using (StreamWriter writer = new StreamWriter(e3.Open()))
                //{
                //    writer.WriteLine("Information about this package.");
                //    writer.WriteLine("========================");
                //}
            }
        }


        public static void Test()
        {


        }
    }
}
