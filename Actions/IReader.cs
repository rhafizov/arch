using System;

namespace ArchiverTestApp
{
    public interface IReader: IDisposable
    {
        bool HasNext();
        byte[] Read();
    }
}
