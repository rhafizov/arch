using System;

namespace ArchiverTestApp
{
    class LocalFileSystemArchiveReader : LocalFileSystemFileReader
    {
        public LocalFileSystemArchiveReader(string pathToFile) : base(pathToFile)
        {
        }
        public override byte[] Read()
        {
            _chunkSize = 4;
            _chunkSize = BitConverter.ToInt32(base.Read());
            return base.Read();
        }
    }
}
