using System;
using System.IO;
using System.Security.Cryptography;

namespace ArchiverTestApp.Helpers
{
    class HashHelper
    {
        public const string HASH_FILE_EXTENTION = ".md5.txt";
        private static string CalculateMD5(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static void CalculateAndSaveFilesHash(string inputFileFullName, string outputFileFullName)
        {
            string md5Hash = CalculateMD5(inputFileFullName);
            
            using var hashFile = File.CreateText(outputFileFullName + HASH_FILE_EXTENTION);
            hashFile.WriteLine(md5Hash);
        }

        public static void ValidateUncompressedFilesHash(string decompressedFilePath, string existingMd5HashFile)
        {
            string md5Hash = CalculateMD5(decompressedFilePath);
            using var hashFile = File.OpenText(existingMd5HashFile);
            if (!md5Hash.Equals(hashFile.ReadLine()))
            {
                Console.WriteLine("Control sums of compressed and decompressed files do not match");
                Environment.Exit((int)ExitCode.Error);
            }
            Console.WriteLine("Control sums are identical");
        }
    }
}
