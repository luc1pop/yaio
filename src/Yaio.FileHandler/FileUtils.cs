using MetadataExtractor.Formats.Exif;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaio.FileHandler
{
    static class FileUtils
    {
        public static string GetYearTargetDirectory(FileInfo fi, ProcessFileParameter param, string overrideRootDir = null)
        {
            DateTime creationDate;
            try
            {
                creationDate = GetCreationDate(fi, param);
            }
            catch (Exception ex)
            {
                param.Log(ex, "Failed to get date for file: " + fi.FullName, fi.FullName, null);
                throw;
            }
            string folderPath = param.ToDirectoryPath;
            if (overrideRootDir != null)
                folderPath = overrideRootDir;

            string toYearFolderPath = Path.Combine(folderPath,
                creationDate.ToString("yyyy"));

            if (param.CreateYearDirectory)
            {
                if (!Directory.Exists(toYearFolderPath))
                {
                    try
                    {
                        param.Log($"Creating directory: {toYearFolderPath}", fi.FullName, null);
                        Directory.CreateDirectory(toYearFolderPath);
                        folderPath = toYearFolderPath;
                    }
                    catch (Exception ex)
                    {
                        param.Log(ex, "Failed to create directory for file: " + fi.FullName + " " + toYearFolderPath, fi.FullName, null);
                        throw;
                    }
                }
                else
                {
                    folderPath = toYearFolderPath;
                }
            }
            return folderPath;
        }

        public  static Tuple<string, string> GetTargetDirectory(FileInfo fi, ProcessFileParameter param, string overrideRootDir = null)
        {
            DateTime creationDate;
            try
            {
                creationDate = GetCreationDate(fi, param);
            }
            catch (Exception ex)
            {
                param.Log(ex, "Failed to get date for file: " + fi.FullName, fi.FullName, null);
                throw;
            }

            string folderPath;
            string firstReturn = folderPath = GetYearTargetDirectory(fi, param, overrideRootDir);
            string sufix = "";
            if (!string.IsNullOrWhiteSpace(param.FolderSufix))
            {
                sufix = " " + param.FolderSufix?.Trim();
            }

            string toMonthFolderPath = Path.Combine(folderPath, creationDate.ToString("MM", CultureInfo.InvariantCulture) + sufix);

            if (param.CreateMonthDirectory)
            {
                if (!Directory.Exists(toMonthFolderPath))
                {
                    try
                    {
                        param.Log($"Creating directory: {toMonthFolderPath}", fi.FullName, null);
                        Directory.CreateDirectory(toMonthFolderPath);
                        folderPath = toMonthFolderPath;
                    }
                    catch (Exception ex)
                    {
                        param.Log(ex, "Failed to create directory for file: " + fi.FullName + " " + toMonthFolderPath, fi.FullName, null);
                        throw;
                    }
                }
                else
                {
                    folderPath = toMonthFolderPath;
                }
            }

            return Tuple.Create<string, string>(firstReturn, folderPath);
        }

        public static DateTime GetCreationDate(FileInfo fi, ProcessFileParameter param)
        {
            param.Log("Extracting creation date", fi.FullName, null);
            try
            {
                var directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(fi.FullName);
                var subIfdDirectory = directories?.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                try
                {
                    DateTime returnValue = MetadataExtractor.DirectoryExtensions.GetDateTime(subIfdDirectory,
                        ExifDirectoryBase.TagDateTimeOriginal);
                    param.Log("EXIF DateTimeOriginal succeeded: " + returnValue.ToString("G"), fi.FullName, null);
                    return returnValue;
                }
                catch (Exception ex)
                {
                    param.Log(ex, "Could read meta data for creation date: " + fi.FullName, fi.FullName, null, ProcessFileParameter.LogSeverity.Warning);
                }
                try
                {
                    DateTime returnValue = MetadataExtractor.DirectoryExtensions.GetDateTime(subIfdDirectory,
                        ExifDirectoryBase.TagDateTimeDigitized);
                    param.Log("EXIF DateTimeDigitized succeeded: " + returnValue.ToString("G"), fi.FullName, null);
                    return returnValue;
                }
                catch (Exception ex)
                {
                    param.Log(ex, "Could not get TagDateTimeOriginal time from exif: " + fi.FullName, fi.FullName, null, ProcessFileParameter.LogSeverity.Warning);
                }
            }
            catch (Exception ex)
            {
                param.Log(ex, "Could not get date and time from exif: " + fi.FullName, fi.FullName, null, ProcessFileParameter.LogSeverity.Warning);
            }

            try
            {
                var returnValue = fi.LastWriteTime;
                param.Log("File CreationTime succeeded: " + returnValue.ToString("G"), fi.FullName, null);
                return returnValue;
            }
            catch (Exception ex)
            {
                param.Log(ex, "Could not get Creation time from file: " + fi.FullName, fi.FullName, null, ProcessFileParameter.LogSeverity.Warning);
                throw;
            }
        }


        public static void WalkDirectoryTree(System.IO.DirectoryInfo root, IList<FileInfo> fileList,
           ProcessFileParameter param)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            param.CancelationToken.ThrowIfCancellationRequested();
            try
            {
                files = root.EnumerateFiles()
                        .Where(f => param.FileFilter.Contains(f.Extension.ToLower()))
                        .ToArray();

            }
            catch (UnauthorizedAccessException e)
            {
                param.Log(ProcessFileParameter.LogSeverity.Error, e.Message, null, null);
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                param.Log(ProcessFileParameter.LogSeverity.Error, e.Message, null, null);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    param.CancelationToken.ThrowIfCancellationRequested();
                    fileList.Add(fi);
                }

            }

            if (param.Recursive)
            {
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    param.CancelationToken.ThrowIfCancellationRequested();
                    WalkDirectoryTree(dirInfo, fileList, param);
                }
            }

        }


        /// <summary>
        /// https://stackoverflow.com/a/2637350/856777
        /// </summary>
        /// <param name="fileInfo1"></param>
        /// <param name="fileInfo2"></param>
        /// <returns></returns>
        public static bool FilesContentsAreEqual(FileInfo fileInfo1, FileInfo fileInfo2)
        {
            bool result;

            if (fileInfo1.Length != fileInfo2.Length)
            {
                result = false;
            }
            else
            {
                using (var file1 = fileInfo1.OpenRead())
                {
                    using (var file2 = fileInfo2.OpenRead())
                    {
                        result = StreamsContentsAreEqual(file1, file2);
                    }
                }
            }

            return result;
        }

        private static bool StreamsContentsAreEqual(Stream stream1, Stream stream2)
        {
            const int bufferSize = 1024 * sizeof(Int64);
            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];

            while (true)
            {
                int count1 = stream1.Read(buffer1, 0, bufferSize);
                int count2 = stream2.Read(buffer2, 0, bufferSize);

                if (count1 != count2)
                {
                    return false;
                }

                if (count1 == 0)
                {
                    return true;
                }

                int iterations = (int)Math.Ceiling((double)count1 / sizeof(Int64));
                for (int i = 0; i < iterations; i++)
                {
                    if (BitConverter.ToInt64(buffer1, i * sizeof(Int64)) != BitConverter.ToInt64(buffer2, i * sizeof(Int64)))
                    {
                        return false;
                    }
                }
            }
        }

       

    }
}
