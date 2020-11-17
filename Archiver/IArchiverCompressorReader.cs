namespace Archiver.Archiver
{
    public interface IArchiverCompressorReader
    {
      public IArchiverCompressorWriter From(string file);
    }
}
