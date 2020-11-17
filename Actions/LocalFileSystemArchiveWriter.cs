using System;

namespace ArchiverTestApp
{
    class LocalFileSystemArchiveWriter : LocalFileSystemFileWriter, IWriter
    {
        public LocalFileSystemArchiveWriter(string pathToFile) : base(pathToFile)
        {
        }

        new public void Write(byte[] data)
        {
            base.Write(BitConverter.GetBytes(data.Length));
            base.Write(data);
        }
    }
}
