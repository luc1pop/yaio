using MetadataExtractor.Formats.Exif;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaio.FileHandler
{
    public class ExtractInfoFromFiles
    {
        private static void ExtractInfoFile(FileInfo fi, ProcessFileParameter param, object extraParam)
        {
            string toFolderPath = param.ToDirectoryPath;
            try
            {
                toFolderPath = FileUtils.GetTargetDirectory(fi, param, Path.Combine(param.ToDirectoryPath, new DirectoryInfo(param.FromDirectoryPath).Name, "Thumbnail")).Item2;
            }
            catch (Exception ex)
            {
                param.Log(ex, "Failed to get directory for file: " + fi.FullName, fi.FullName, null);
                return;
            }

            string thumbNailPath = Path.Combine(toFolderPath,
                    Path.GetFileNameWithoutExtension(fi.Name) + "_thumb.jpg");
            try
            {
                if (!File.Exists(thumbNailPath))
                    WriteThumbImage(fi.FullName, thumbNailPath, param);
            }
            catch (Exception ex)
            {
                param.Log(ex, "Failed to create thumbnail for file: " + fi.FullName, fi.FullName, null);
                return;
            }

            try
            {
                param.Log("Extracting tags", fi.FullName, null);
                var directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(fi.FullName);
                DateTime creationDate;
                try
                {
                    creationDate = FileUtils.GetCreationDate(fi, param);
                }
                catch (Exception ex)
                {
                    param.Log(ex, "Failed to get date for file: " + fi.FullName, fi.FullName, null);
                    throw;
                }

                var infoDatabase = extraParam as Dictionary<string, Dictionary<string, Dictionary<string, object>>>;

                if (infoDatabase == null) { throw new ArgumentNullException(nameof(infoDatabase)); }

                dynamic imageInfo = new ExpandoObject();
                var imageDictionary = (IDictionary<string, object>)imageInfo;
                foreach (var directory in directories)
                {
                    foreach (var tag in directory.Tags)
                    {
                        try
                        {
                            imageDictionary.Add(directory.Name + " - " + tag.Name, tag.Description);
                        }
                        catch (Exception ex)
                        {
                            param.Log(ex, $"Failed to add {directory.Name} + - + {tag.Name} for file:  + {fi.FullName}", fi.FullName, null);
                        }
                    }
                }


                imageDictionary.Add("ImageFilePath", fi.FullName);
                imageDictionary.Add("ThumbnailFilePath", thumbNailPath);

                var yearKey = creationDate.ToString("yyyy");
                var monthkey = creationDate.ToString("MM");
                if (!infoDatabase.ContainsKey(yearKey))
                {
                    infoDatabase.Add(yearKey, new Dictionary<string, Dictionary<string, object>>());
                }
                if (!infoDatabase[yearKey].ContainsKey(monthkey))
                {
                    infoDatabase[yearKey].Add(monthkey, new Dictionary<string, object>());
                }
                if (infoDatabase[yearKey][monthkey].ContainsKey(fi.Name))
                {
                    param.Log(ProcessFileParameter.LogSeverity.Warning, $"There is already information about the file {fi.Name} in the database", fi.FullName, null);
                }
                infoDatabase[yearKey][monthkey].Add(fi.Name, imageDictionary);

            }
            catch (Exception ex)
            {
                param.Log(ex, "Failed to create extract information for file: " + fi.FullName, fi.FullName, null);
                return;
            }
        }

        public static async Task<ProcessResult> ExtractInfo(ProcessFileParameter param)
        {
            return await Task.Run<ProcessResult>(() =>
            {
                try
                {
                    var retValue = new ProcessResult();
                    var rootDirInfo = new DirectoryInfo(param.FromDirectoryPath);
                    string jsonFile = Path.Combine(param.ToDirectoryPath, rootDirInfo.Name + ".json");
                    string indexFile = Path.Combine(param.ToDirectoryPath,
                        rootDirInfo.Name + ".html");
                    string jsonText = "";
                    var yearMonthDict = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
                    dynamic infoDatabase = new ExpandoObject();
                    if (File.Exists(jsonFile))
                    {
                        jsonText = File.ReadAllText(jsonFile);
                        yearMonthDict = JObject.Parse(jsonText).SelectToken("Images")
                        .ToObject<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>();



                    }
                    ProcessFiles.ProcessFile(param, ExtractInfoFile, yearMonthDict);
                    StringBuilder indexString = new StringBuilder();
                    indexString.AppendLine("<html>");
                    indexString.AppendLine("<head>");
                    indexString.AppendLine("</head>");
                    indexString.AppendLine("<body>");
                    indexString.AppendLine("<style>");
                    indexString.AppendLine(param.PrefixHtml);
                    foreach (var year in yearMonthDict.OrderBy(a => a.Key))
                    {
                        indexString.AppendLine($"<h1>{year.Key}</h1>");
                        foreach (var month in year.Value.OrderBy(a => a.Key))
                        {
                            indexString.AppendLine($"<h2>{month.Key}</h2>");
                            foreach (var image in month.Value.OrderBy(a => a.Key))
                            {
                                dynamic tmpImage = image.Value as dynamic;
                                if (tmpImage != null)
                                {
                                    string filePath = tmpImage.ThumbnailFilePath;
                                    filePath = filePath.Replace(param.ToDirectoryPath, ".");
                                    indexString.AppendLine($"<a href='{tmpImage.ImageFilePath}'> <img src='{filePath}'></a>");
                                }
                                else
                                {
                                    param.Log(ProcessFileParameter.LogSeverity.Warning, "Image could not be cast to expandoObject", null, null);
                                }
                            }
                            indexString.AppendLine("<hr>");
                        }
                    }
                    indexString.AppendLine("</body>");
                    indexString.AppendLine("</html>");

                    infoDatabase.Images = yearMonthDict;
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(infoDatabase);
                    File.WriteAllText(jsonFile, json);
                    File.WriteAllText(indexFile, indexString.ToString());

                    param.Log(ProcessFileParameter.LogSeverity.Info, $"Finished processing files in {param.FromDirectoryPath}", null, null);
                    return retValue;
                }
                catch (Exception ex)
                {
                    param.Log(ex, "Failed to extract information", null, null);
                    throw;
                }
            });
        }


        public static void WriteThumbImage(string path, string thumbFileName, ProcessFileParameter param)
        {
            int orientation = 1;
            try
            {
                var directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(path);
                var subIfdDirectories = directories.OfType<ExifIfd0Directory>();
                foreach (var subDir in subIfdDirectories)
                {
                    try
                    {
                        orientation = MetadataExtractor.DirectoryExtensions.GetInt32(subDir,
                            ExifDirectoryBase.TagOrientation);
                        param.Log("EXIF TagOrientation succeeded: " + orientation.ToString(), path, null);
                    }
                    catch (Exception ex)
                    {
                        param.Log(ex, "Could not get TagOrientation time from exif: " + path, path, null);
                    }

                }
            }
            catch (Exception ex)
            {
                param.Log(ex, "Could not get tranformation time from exif: " + path, path, null);
            }

            param.Log("Creating thumbnail", path, thumbFileName);

            Bitmap srcBmp = new Bitmap(path);
            float ratio = (float)srcBmp.Width / (float)srcBmp.Height;
            SizeF newSize = new SizeF((float)param.MaxWithHeight, (float)param.MaxWithHeight / ratio);
            Bitmap target = new Bitmap((int)newSize.Width, (int)newSize.Height);
            using (Graphics graphics = Graphics.FromImage(target))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(srcBmp, 0, 0, newSize.Width, newSize.Height);


                //Value   0th Row 0th Column
                //1   top left side
                //2   top right side
                //3   bottom right side
                //4   bottom left side
                //5   left side   top
                //6   right side  top
                //7   right side  bottom
                //8   left side   bottom

                //case `jpegexiforient - n "$i"` in
                //1) transform = ""; ;
                //2) transform = "-flip horizontal"; ;
                //3) transform = "-rotate 180"; ;
                //4) transform = "-flip vertical"; ;
                //5) transform = "-transpose"; ;
                //6) transform = "-rotate 90"; ;
                //7) transform = "-transverse"; ;
                //8) transform = "-rotate 270"; ;
                //*) transform = ""; ;

                RotateFlipType flip = OrientationToFlipType(orientation);
                if (flip != RotateFlipType.RotateNoneFlipNone) // don't flip of orientation is correct
                {
                    target.RotateFlip(flip);
                }
                target.Save(thumbFileName, ImageFormat.Jpeg);
            }

        }


        private static RotateFlipType OrientationToFlipType(int orientation)
        {
            switch (orientation)
            {
                case 1:
                    return RotateFlipType.RotateNoneFlipNone;
                    break;
                case 2:
                    return RotateFlipType.RotateNoneFlipX;
                    break;
                case 3:
                    return RotateFlipType.Rotate180FlipNone;
                    break;
                case 4:
                    return RotateFlipType.Rotate180FlipX;
                    break;
                case 5:
                    return RotateFlipType.Rotate90FlipX;
                    break;
                case 6:
                    return RotateFlipType.Rotate90FlipNone;
                    break;
                case 7:
                    return RotateFlipType.Rotate270FlipX;
                    break;
                case 8:
                    return RotateFlipType.Rotate270FlipNone;
                    break;
                default:
                    return RotateFlipType.RotateNoneFlipNone;
            }
        }



    }
}
