using System;
using System.IO;

namespace ArchiverTestApp.Helpers
{
    public static class FileAccessValidationHelper
    {
        public static string ValidateAndGetAbsoluteFilePath(string fileName)
        {
            string calculatedPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + fileName;
            
            bool fileExistInCurrentDirectory = File.Exists(calculatedPath);
            bool fileExistOnAbsolutePath = File.Exists(fileName);

            if (!fileExistInCurrentDirectory && !fileExistOnAbsolutePath)
            {
                Console.WriteLine($"The input file '{calculatedPath}' does not exist.");
                Environment.Exit((int)ExitCode.Error);
            }

            if (fileExistInCurrentDirectory)
            {
                return calculatedPath;
            }
           
            return fileName;          
        }


        public static string CreateFileAndGetAbsoluteFilePath(string fileName)
        {
            bool parentDirectoryExist = Directory.Exists(Path.GetDirectoryName(fileName));
           
            string calculatedPath = parentDirectoryExist ? fileName : Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + fileName;

            try
            {
                FileStream createStream = File.Create(calculatedPath);
                createStream.Close();
            }
            catch
            {
                Console.WriteLine($"Cannot create file '{calculatedPath}' .");
                Environment.Exit((int)ExitCode.Error);
            }
            return calculatedPath;
        }
    }
}
