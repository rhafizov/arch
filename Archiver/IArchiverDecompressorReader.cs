namespace Archiver.Archiver
{
    public interface IArchiverDecompressorReader
    {
        public IArchiverDecompressorWriter From(string file);
    }
}