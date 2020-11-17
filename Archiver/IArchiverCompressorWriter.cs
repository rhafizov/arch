namespace Archiver.Archiver
{
    public interface IArchiverCompressorWriter
    {
       public IArchiverActionExecutor To(string file);
    }
}
