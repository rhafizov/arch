namespace Archiver.Archiver
{
    public interface IArchiverDecompressor
    {
       public IArchiverDecompressorWriter From(string file);
    }
}
