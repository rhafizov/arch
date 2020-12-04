using System;

namespace ArchiverTestApp
{
    public interface IWriter: IDisposable
    {
        void Write(byte[] data);
    }
}
