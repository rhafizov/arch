using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace ArchiverTestApp
{
    sealed class Compressor : Processor
    {
        public Compressor(int numberOfThreads) : base(numberOfThreads)
        {
        }

        public override byte[] Process(byte[] data)
        {
            
            List<Thread> threads = new List<Thread>();

            byte[][] processedData = new byte[_numberOfThreads][];
            int compressBlockSize = (data.Length % _numberOfThreads == 0) ? data.Length / _numberOfThreads : data.Length / (_numberOfThreads - 1);


            for (int i = 0; i < _numberOfThreads; i++)
            {
                int index = i;

                threads.Add(new Thread(() =>
                {
                    CompressChunk(processedData, data, index, compressBlockSize);
                }));
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

        private void CompressChunk(byte[][] compressedBytesArray, byte[] data, int i, int compressBlockSize)
        {
            Span<byte> uncompressedBlock;

            if (i == _numberOfThreads - 1)
            {
                uncompressedBlock = new Span<byte>(data, i * compressBlockSize, data.Length - i * compressBlockSize);
            }
            else
            {
                uncompressedBlock = new Span<byte>(data, i * compressBlockSize, compressBlockSize);
            }

            using (MemoryStream compressed = new MemoryStream())
            {
                using (GZipStream compressor = new GZipStream(compressed, CompressionMode.Compress))
                {
                    compressor.Write(uncompressedBlock);
                    compressor.Flush();

                    int compressedLength = (int)compressed.Length;
                    byte[] compressedDataWithLength = new byte[compressedLength + 4];
                    BitConverter.GetBytes(compressedLength).CopyTo(compressedDataWithLength, 0);
                    compressed.ToArray().CopyTo(compressedDataWithLength, 4);

                    compressedBytesArray[i] = compressedDataWithLength;
                }
            }
        }
    }
}
