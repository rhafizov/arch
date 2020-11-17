using Archiver;
using System;

namespace ArchiverTestApp
{
    public abstract class Processor : IProcessor
    {
        protected int _numberOfThreads;

        protected Processor(int numberOfThreads)
        {
            _numberOfThreads = numberOfThreads;
        }

        protected Processor()
        {
        }

        public abstract byte[] Process(byte[] data);

        protected byte[] JaggedArrayToFlatArray(byte[][] processedData)
        {
            int compressedDataLength = 0;

            foreach (byte[] d in processedData)
            {
                compressedDataLength += d.Length;
            }

            byte[] result = new byte[compressedDataLength];

            Span<byte> resultSpan = new Span<byte>(result);
            int prevLength = 0;

            for (int i = 0; i < processedData.Length; i++)
            {
                Span<byte> span = new Span<byte>(processedData[i]);

                Span<byte> slice = resultSpan.Slice(prevLength, processedData[i].Length);
                prevLength += processedData[i].Length;

                span.CopyTo(slice);
                processedData[i] = null;
            }

            return result;
        }
    }
}
