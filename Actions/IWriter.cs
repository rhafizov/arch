using System;
using System.IO;

namespace ArchiverTestApp
{
    public interface IWriter: IDisposable
    {
        void Write(byte[] data);
    }
}
