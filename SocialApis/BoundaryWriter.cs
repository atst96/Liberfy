using System.IO;
using System.Text;

namespace SocialApis
{
    internal class BoundaryWriter : StreamWriter
    {
        public string Boundary { get; }
        private BinaryWriter BinaryWriter { get; set; }

        public BoundaryWriter(Stream stream, string boundary) : base(stream)
        {
            this.Boundary = boundary;
            this.BinaryWriter = new BinaryWriter(stream);
        }

        public void WriteBeginBoundary()
        {
            this.WriteLine($"--{ this.Boundary }");
        }

        public void WriteDisposition(string name)
        {
            this.WriteBeginBoundary();

            this.WriteLine($"Content-Disposition: form-data; name=\"{ name }\"");
            this.WriteLine();
        }

        public void WriteDisposition(string name, string content)
        {
            this.WriteDisposition(name);
            this.WriteLine(content);
        }

        public void WriteDisposition(string name, int content)
        {
            this.WriteDisposition(name);
            this.WriteLine(content);
        }

        public void WriteDisposition(string name, long content)
        {
            this.WriteDisposition(name);
            this.WriteLine(content);
        }

        public void WriteDispositionBinary(string name, byte[] data)
        {
            this.WriteDisposition(name);
            this.Flush();

            this.BinaryWriter.Write(data);
            this.BinaryWriter.Flush();

            this.WriteLine();
        }

        public void WriteDispositionBinary(string name, Stream stream, int segmentSize)
        {
            this.WriteDisposition(name);
            this.Flush();

            byte[] data = new byte[segmentSize];
            stream.Read(data, 0, segmentSize);
            this.BaseStream.Write(data, 0, segmentSize);

            this.WriteLine();
        }

        public void CloseBoundary()
        {
            this.WriteLine($"--{ this.Boundary }--");
        }

        protected override void Dispose(bool disposing)
        {
            this.BinaryWriter?.Close();
            this.BinaryWriter = null;

            base.Dispose(disposing);
        }
    }
}
