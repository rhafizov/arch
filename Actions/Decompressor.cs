using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace ArchiverTestApp
{
    sealed class Decompressor : Processor
    {
        public Decompressor() : base()
        {
        }

        public override byte[] Process(byte[] data)
        {
            List<Thread> threads = new List<Thread>();
            
            int numberOfBlocksInChunk = NumberOfBlocksInChunk(data);

            byte[][] processedData = new byte[numberOfBlocksInChunk][];
            byte[][] compressedDataArrays = new byte[numberOfBlocksInChunk][];

            int countedDataSize = 0;
            for (int i = 0; i < numberOfBlocksInChunk; i++)
            {

                int blockSize = BitConverter.ToInt32(new Span<byte>(data).Slice(countedDataSize, 4));

                countedDataSize += 4;

                int index = i;
                int countedDataSizeCopy = countedDataSize;
                int blockSizeCopy = blockSize;

                threads.Add(new Thread(() =>
                {
                    DecompressChunk(processedData, data, index, countedDataSizeCopy, blockSizeCopy);
                }));

                countedDataSize += blockSize;
            }

            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            data = null;

            return JaggedArrayToFlatArray(processedData);
        }

        private int NumberOfBlocksInChunk(byte[] data)
        {
            int numberOfblocksInChunk = 0;
            int countedDataSize = 0;
            do
            {
                int blockSize = BitConverter.ToInt32(new Span<byte>(data).Slice(countedDataSize, 4));
                countedDataSize += 4;
                countedDataSize += blockSize;
                numberOfblocksInChunk++;

            } while (countedDataSize < data.Length);

            return numberOfblocksInChunk;
        }

        private void DecompressChunk(byte[][] compressedDataArrays, byte[] inputData, int index, int countedDataSize, int blockSize)
        {

            Span<byte> span = new Span<byte>(inputData, countedDataSize, blockSize);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (MemoryStream compressed = new MemoryStream())
                {
                    using (GZipStream decompress = new GZipStream(compressed, CompressionMode.Decompress))
                    {
                        compressed.Write(span);
                        compressed.Flush();
                        compressed.Position = 0;
                        decompress.CopyTo(memoryStream);
                        compressedDataArrays[index] = memoryStream.ToArray();
                    }
                } 
            }
        }
    }
}
