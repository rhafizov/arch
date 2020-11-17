using System;

namespace ArchiverTestApp.Helpers
{

    class BlockSizeAndRamValidationHelper
    {
        private static long GetAvailableRamBytes()
        {
            long installedMemory;
            var gcMemoryInfo = GC.GetGCMemoryInfo();
            installedMemory = gcMemoryInfo.TotalAvailableMemoryBytes;
            return installedMemory;
        }

        public static void Validate(int blockSize, int maxChunksStoredInMemoryAtSameTime)
        {
            long availableRam = GetAvailableRamBytes();
            if (availableRam < blockSize*maxChunksStoredInMemoryAtSameTime*2)
            {
                Console.WriteLine($"Available RAM: {availableRam}");
                Console.WriteLine($"Configured blockSize: {blockSize}");
                Console.WriteLine($"Available RAM should be greater than blockSize * 2.");
                Environment.Exit((int)ExitCode.Error);
            }
        }
    }
}
