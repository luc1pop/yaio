using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaio.FileHandler
{
    public static class ArrangeFilesS
    {
        static public async Task<ProcessResult> ArrangeFiles(ProcessFileParameter param)
        {
            return await Task.Run<ProcessResult>(() =>
            {
                try
                {
                    var retValue = new ProcessResult();
                    ProcessFiles.ProcessFile(param, ArrangeFile, new Dictionary<string, SortedSet<string>>());
                    param.Log(ProcessFileParameter.LogSeverity.Info, $"Finished processing files in {param.FromDirectoryPath}", null, null);
                    return retValue;
                }
                catch (Exception ex)
                {
                    param.Log(ProcessFileParameter.LogSeverity.Error, ex.Message, null, null);
                    throw;
                }
            });
        }

        public static void ArrangeFile(FileInfo fi, ProcessFileParameter param, object extraParam)
        {
            Dictionary<string, SortedSet<string>> cache = extraParam as Dictionary<string, SortedSet<string>>;
            if (cache == null)
                throw new Exception("No cache specified");

            string toFolderPath = param.ToDirectoryPath;
            if (param.SearchForDuplicatesRecursivelyInYearFolder)
            {
                param.CancelationToken.ThrowIfCancellationRequested();
                try
                {
                    toFolderPath = FileUtils.GetYearTargetDirectory(fi, param);
                }
                catch (Exception ex)
                {
                    param.Log(ex, "Failed to get directory for file: " + fi.FullName, fi.FullName, null);
                    return;
                }
                var toDirectory = new DirectoryInfo(toFolderPath);
                if (!cache.ContainsKey(toDirectory.FullName))
                {
                    List<FileInfo> fileList = new List<FileInfo>();
                    param.Log(ProcessFileParameter.LogSeverity.Info, $"Extracting all filenames from {toDirectory.FullName}", fi.FullName, "");
                    FileUtils.WalkDirectoryTree(toDirectory, fileList, param);
                    cache[toDirectory.FullName] = new SortedSet<string>(fileList.Select(f => f.FullName).ToList());
                }
                var alreadyExistingFile = cache[toDirectory.FullName].Where(f => (new FileInfo(f)).Name.ToUpperInvariant() == fi.Name.ToUpperInvariant()).FirstOrDefault();
                if (alreadyExistingFile != null)
                {
                    if (param.DeleteDuplicateFilesFromProcessFolder)
                    {
                        FileInfo alreadyExistFile = new FileInfo(alreadyExistingFile);
                        if (FileUtils.FilesContentsAreEqual(fi, alreadyExistFile))
                        {
                            if (param.DeleteDuplicateFilesFromProcessFolder)
                            {
                                param.Log(ProcessFileParameter.LogSeverity.Warning, $"Files {alreadyExistFile.FullName} is the same as {fi.FullName}. Deleting {fi.FullName}", fi.FullName, alreadyExistFile.FullName);
                                try
                                {
                                    File.Delete(fi.FullName);
                                    return;
                                }
                                catch (Exception ex)
                                {
                                    param.Log(ex, $"Failed to delete file {fi.FullName}", fi.FullName, alreadyExistFile.FullName);
                                    return;
                                }
                            }

                        }
                    }
                }

            }

            Tuple<string, string> yearAndFolderPath;
            try
            {
                yearAndFolderPath = FileUtils.GetTargetDirectory(fi, param);
                toFolderPath = yearAndFolderPath.Item2;
            }
            catch (Exception ex)
            {
                param.Log(ex, "Failed to get directory for file: " + fi.FullName, fi.FullName, null);
                return;
            }


            var toFileInfo = new FileInfo(Path.Combine(toFolderPath, fi.Name));
            if (File.Exists(toFileInfo.FullName))
            {
                param.Log(ProcessFileParameter.LogSeverity.Warning, $"File {toFileInfo.FullName} already exists.", fi.FullName, toFileInfo.FullName);
                if (FileUtils.FilesContentsAreEqual(fi, toFileInfo))
                {
                    if (param.DeleteDuplicateFilesFromProcessFolder)
                    {
                        param.Log(ProcessFileParameter.LogSeverity.Warning, $"Files {toFileInfo.FullName} is the same as {fi.FullName}. Deleting {fi.FullName}", fi.FullName, toFileInfo.FullName);
                        try
                        {
                            File.Delete(fi.FullName);
                            return;
                        }
                        catch (Exception ex)
                        {
                            param.Log(ex, $"Failed to delete file {fi.FullName}", fi.FullName, toFileInfo.FullName);
                            return;
                        }
                    }
                    else
                    {
                        param.Log(ProcessFileParameter.LogSeverity.Warning, $"Files {toFileInfo.FullName} is the same as {fi.FullName}. No move", fi.FullName, toFileInfo.FullName);
                        return;
                    }
                }
                else
                {
                    param.Log(ProcessFileParameter.LogSeverity.Warning, $"Files {toFileInfo.FullName} is the different as {fi.FullName}. Finding unique name.", fi.FullName, toFileInfo.FullName);
                    int index = 0;
                    while (true)
                    {
                        toFileInfo = new FileInfo(Path.Combine(toFolderPath, (index++).ToString() + fi.Name));
                        if (!File.Exists(toFileInfo.FullName))
                        {
                            try
                            {
                                param.Log($"Moving from {fi.FullName} to {toFileInfo.FullName}", fi.FullName, toFileInfo.FullName);

                                File.Move(fi.FullName, toFileInfo.FullName);
                                if (cache.ContainsKey(yearAndFolderPath.Item1))
                                {
                                    cache[yearAndFolderPath.Item1].Add(toFileInfo.FullName);
                                }
                                return;
                            }
                            catch (Exception ex)
                            {
                                param.Log(ex, $"Failed to move file {fi.FullName} to {toFileInfo.FullName}", fi.FullName, toFileInfo.FullName);
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                try
                {
                    param.Log($"Moving from {fi.FullName} to {toFileInfo.FullName}", fi.FullName, toFileInfo.FullName);
                    File.Move(fi.FullName, toFileInfo.FullName);
                    if (cache.ContainsKey(yearAndFolderPath.Item1))
                    {
                        cache[yearAndFolderPath.Item1].Add(toFileInfo.FullName);
                    }
                    return;
                }
                catch (Exception ex)
                {
                    param.Log(ex, $"Failed to move file {fi.FullName} to {toFileInfo.FullName}", fi.FullName, toFileInfo.FullName);
                    return;
                }
            }
        }

     
    }
}
