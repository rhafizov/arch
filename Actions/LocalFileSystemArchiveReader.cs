using System;

namespace ArchiverTestApp
{
    class LocalFileSystemArchiveReader : LocalFileSystemFileReader, IReader
    {
        public LocalFileSystemArchiveReader(string pathToFile) : base(pathToFile)
        {
        }
        new public byte[] Read()
        {
            _chunkSize = 4;
            _chunkSize = BitConverter.ToInt32(base.Read());
            return base.Read();
        }
    }
}
