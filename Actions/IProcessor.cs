namespace Archiver
{
    public interface IProcessor
    {
        byte[] Process(byte[] data);
    }
}
