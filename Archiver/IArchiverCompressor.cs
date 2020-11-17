namespace Archiver.Archiver
{
    public interface IArchiverCompressor
    {
        public IArchiverCompressorWriter From(string file);
    }
}
