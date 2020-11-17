using System.IO;

namespace ArchiverTestApp
{
    class LocalFileSystemFileReader : IReader
    {
        private readonly FileStream _input;
        protected int _chunkSize;

        public LocalFileSystemFileReader(string pathToFile, int chunkSize)
        {
            _input = File.OpenRead(pathToFile);
            _chunkSize = chunkSize;
        }

        public LocalFileSystemFileReader(string pathToFile)
        {
            _input = File.OpenRead(pathToFile);
        }

        private LocalFileSystemFileReader()
        {

        }

        public bool HasNext()
        {
           return _input.Position != _input.Length;
        }

        public byte[] Read()
        {
            if(_input.Position == _input.Length)
            {
                throw new EndOfStreamException();
            }

            byte[] result = (_input.Length - _input.Position > _chunkSize) ? new byte[_chunkSize] : new byte[_input.Length - _input.Position];
            _input.Read(result);
            _input.Flush();

            return result;
        }

        public void Dispose()
        {
            _input.Dispose();
        }
    }
}
