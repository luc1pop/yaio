using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using MetadataExtractor.Formats.Exif;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Dynamic;
using System.Globalization;

namespace Yaio.FileHandler
{

    public static class ProcessFiles
    {
        public delegate void ProcessFileDelegate(FileInfo fi, ProcessFileParameter param, object extraParam);

        public static void ProcessFile(ProcessFileParameter param, ProcessFileDelegate proc, object extraParam)
        {
            if (!Directory.Exists(param.FromDirectoryPath))
            {
                throw new DirectoryNotFoundException(string.Format("The directory '{0}' is not found", param.FromDirectoryPath));
            }
            if (!Directory.Exists(param.ToDirectoryPath))
            {
                throw new DirectoryNotFoundException(string.Format("The directory '{0}' is not found", param.ToDirectoryPath));
            }

            var fromDirectory = new DirectoryInfo(param.FromDirectoryPath);
            param.CancelationToken.ThrowIfCancellationRequested();
            List<FileInfo> fileList = new List<FileInfo>();
            FileUtils.WalkDirectoryTree(fromDirectory, fileList, param);
            param.SetNrOfFiles(fileList.Count);
            int fileIndex = 0;
            foreach (var fi in fileList)
            {
                param.CancelationToken.ThrowIfCancellationRequested();
                param.ProcessingFile(fileIndex++);
                proc.Invoke(fi, param, extraParam);
            }
        }





    }
}
