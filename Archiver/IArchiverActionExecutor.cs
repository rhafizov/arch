namespace Archiver.Archiver
{
    public interface IArchiverActionExecutor
    {
        public void Execute(int maxChunksStoredInMemoryAtSameTime);
    }
}
