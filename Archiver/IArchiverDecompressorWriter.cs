namespace Archiver.Archiver
{
    public interface IArchiverDecompressorWriter
    {
       public IArchiverActionExecutor To(string file);

    }
}