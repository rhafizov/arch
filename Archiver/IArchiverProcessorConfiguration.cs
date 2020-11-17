namespace Archiver.Archiver
{
    public interface IArchiverProcessorConfiguration
    {
        public IArchiverDecompressor WitDecompressor();
        public IArchiverCompressor WithCompressor();
    }
}
