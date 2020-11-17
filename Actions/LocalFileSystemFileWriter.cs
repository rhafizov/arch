using System.IO;

namespace ArchiverTestApp
{
    class LocalFileSystemFileWriter : IWriter
    {
        private readonly FileStream _output;

        public LocalFileSystemFileWriter(string pathToFile)
        {
            _output = File.OpenWrite(pathToFile);
        }

        public void Write(byte[] data)
        {
            _output.Seek(0, SeekOrigin.End);

            _output.Write(data);
            _output.Flush();

        }

        public void Dispose()
        {
            _output.Dispose();
        }
    }
}
