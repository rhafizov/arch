using Archiver;
using Archiver.Archiver;
using Archiver.Conveyer;
using System;

namespace ArchiverTestApp
{
    public class Archiver : IArchiverDecompressor, IArchiverCompressor, IArchiverCompressorWriter, IArchiverDecompressorWriter, IArchiverActionExecutor
    {
        private IProcessor _processor;
        private IReader _reader;
        private IWriter _writer;
        private int _blockSize;
        private int _numberOfThreads;

        private Archiver()
        { }

        public static IArchiverDecompressor WitDecompressor()
        {
            Archiver archiver = new Archiver();
            archiver._processor = new Decompressor();
            
            return archiver;
        }

        public static IArchiverCompressor WithCompressor(int blockSize, int numberOfThreads)
        {
            Archiver archiver = new Archiver();
            archiver._blockSize = blockSize;
            archiver._numberOfThreads = numberOfThreads;
            archiver._processor = new Compressor(archiver._numberOfThreads);

            return archiver;
        }
        IArchiverActionExecutor IArchiverDecompressorWriter.To(string file)
        {
            _writer = new LocalFileSystemFileWriter(file);
            return this;
        }

        IArchiverActionExecutor IArchiverCompressorWriter.To(string file)
        {
            _writer = new LocalFileSystemArchiveWriter(file);
            return this;
        }

        IArchiverDecompressorWriter IArchiverDecompressor.From(string file)
        {
            _reader = new LocalFileSystemArchiveReader(file);
            return this;
        }

        IArchiverCompressorWriter IArchiverCompressor.From(string file)
        {
            _reader = new LocalFileSystemFileReader(file, _blockSize);
            return this;
        }

        void IArchiverActionExecutor.Execute(int maxChunksStoredInMemoryAtSameTime)
        {
            try
            {
                using(ProducerConsumerQueuesConveyer conveyer = new ProducerConsumerQueuesConveyer(
                    chunk => _processor.Process(chunk),
                    processedChunk => _writer.Write(processedChunk), 
                    maxChunksStoredInMemoryAtSameTime))
                {
                    while (_reader.HasNext())
                    {
                        conveyer.EnqueueChunkToQueueOne(_reader.Read());
                    }
                }

            }
            catch
            {
                Console.Write("Something went wrong while executing the action...");
                throw;
        
            }
            finally
            {
                _reader?.Dispose();
                _writer?.Dispose();
            }
        }
    }
}
