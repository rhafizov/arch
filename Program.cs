using ArchiverTestApp.Helpers;
using System;
using System.IO;

namespace ArchiverTestApp
{
    // ToDo: Add logging. Evaluate parallel writing to file from multiple threads. Evaluate benefits of MemoryMappedFile usage.
    enum ExitCode : int
    {
        Success = 0,
        Error = 1
    }
    class Program
    {
        private const int BLOCK_SIZE = 1024 * 1024 * 100;
        private const int MAX_CHUNKS_STORED_IN_RAM_AT_SAME_TIME = 4;
        private static string invalidInputErrorMessage = @" 
Error: unknown or command or invalid input.

Usage:
    Archiver [compress | decompress] [path/to/input/file] [path/to/out/file]

Here:
    compress            Perform compression of input file
    decompress          Perform decompression of input file

Examples:
    Archiver.exe compress C:/inputFileDir/inputFile.txt C:/outputFileDir/archivedFile
                                    ... Command will trigger archiving 'inputFile.txt'
                                        and saving archived file as 'archivedFile'

    Archiver.exe decompress C:/inputFileDir/archivedFile C:/outputFileDir/unzippedFile
                                    ... Command will trigger unzipping 'archivedFile'
                                        and saving unzipped file as 'unzippedFile'
                                      

";

        static void Main(string[] args)
        {

        //    args = new string[] { "compress", "TheFile.mp4", "TheFile.mp4.gz" };
              args = new string[] { "decompress", "TheFile.mp4.gz", "TheFile.mp4" };
            //     args = new string[] { "compress", "TheFile1.jpg", "TheFile1.jpg.gz" };
            //     args = new string[] { "decompress", "TheFile1.jpg.gz", "TheFile1.jpg" };

            string inputFileFullName = FileAccessValidationHelper.ValidateAndGetAbsoluteFilePath(args[1]);
            string outputFileFullName = FileAccessValidationHelper.CreateFileAndGetAbsoluteFilePath(args[2]);

            BlockSizeAndRamValidationHelper.Validate(BLOCK_SIZE, MAX_CHUNKS_STORED_IN_RAM_AT_SAME_TIME);

            try
            {
                switch (args[0])
                {
                    case ("compress"):
                        {
                            HashHelper.CalculateAndSaveFilesHash(inputFileFullName, outputFileFullName);
                            Archiver.WithCompressor(BLOCK_SIZE, Environment.ProcessorCount)
                                    .From(inputFileFullName)
                                    .To(outputFileFullName)
                                    .Execute(MAX_CHUNKS_STORED_IN_RAM_AT_SAME_TIME);
                            break;
                        }
                    case ("decompress"):
                        {
                            Archiver.WitDecompressor()
                                    .From(inputFileFullName)
                                    .To(outputFileFullName)
                                    .Execute(MAX_CHUNKS_STORED_IN_RAM_AT_SAME_TIME);
                            HashHelper.ValidateUncompressedFilesHash(outputFileFullName, inputFileFullName + HashHelper.HASH_FILE_EXTENTION);
                            break;
                        }
                    default:
                        Console.Write(invalidInputErrorMessage);
                        Environment.Exit((int)ExitCode.Error);
                        break;
                }

                Console.Write("Operation successfully completed");
                Environment.Exit((int)ExitCode.Success);
            }
            catch (Exception ex)
            {
                Console.Write("Something went wrong...");
                using (StreamWriter errorFile = File.CreateText("ErrorLog.txt"))
                {
                    errorFile.WriteLine($"{DateTime.Now}: Error. {ex.Message}");
                    errorFile.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }
            finally
            {
                Environment.Exit((int)ExitCode.Error);
            }
        }
    }
}
