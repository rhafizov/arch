using System;
using System.IO;

namespace ArchiverTestApp
{
    public interface IReader: IDisposable
    {
        bool HasNext();
        byte[] Read();
    }
}
