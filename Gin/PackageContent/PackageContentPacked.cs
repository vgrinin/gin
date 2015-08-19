using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Commands;
using Gin.Context;
using Gin.Util.GZip;
using System.IO;
using Gin.Util;
using Gin.Logging;

namespace Gin
{
    public class PackageContentPacked: PackageContent.PackageContent
    {


        public override void Load(string packageContentDirectory, string packageResultFilePath)
        {
            try
            {
                PackageContentDirectory = packageContentDirectory;
                using (FileStream unarchFile = File.OpenRead(packageResultFilePath))
                {
                    GZipReader reader = new GZipReader(unarchFile);
                    reader.OnProgress += new Action<ExecutionProgressInfo>(reader_OnProgress);
                    reader.ReadToEnd(packageContentDirectory);
                }
                PackageBodyFilePath = Path.Combine(packageContentDirectory, MAIN_PACKAGE_FILENAME);
            }
            catch (PackageExecutionCancelledException)
            {
                Clean();
            }
        }

        public override void LoadBody(string packageContentDirectory, string packageResultFilePath)
        {
            try
            {
                PackageContentDirectory = packageContentDirectory;
                using (FileStream unarchFile = File.OpenRead(packageResultFilePath))
                {
                    GZipReader reader = new GZipReader(unarchFile);
                    reader.ReadToEnd(packageContentDirectory, MAIN_PACKAGE_FILENAME);
                }
                PackageBodyFilePath = Path.Combine(packageContentDirectory, MAIN_PACKAGE_FILENAME);
            }
            catch (PackageExecutionCancelledException)
            {
                Clean();
            }
        }

        void reader_OnProgress(ExecutionProgressInfo obj)
        {
            OnProgressHandler(obj);
            QueryCancelEventArgs args = new QueryCancelEventArgs();
            OnQueryCancelHandler(args);
            if (args.Cancel)
            {
                throw new PackageExecutionCancelledException();
            }
        }

        public override string AddContent(string sourcePath)
        {
            if (!FileMap.ContainsKey(sourcePath))
            {
                string destFileName = GenerateContentFileName(sourcePath);
                AddContent(sourcePath, destFileName);
                return Path.Combine(ExecutionContext.PACKAGE_CONTENT_TAG, destFileName);
            }
            else
            {
                return Path.Combine(ExecutionContext.PACKAGE_CONTENT_TAG,FileMap[sourcePath]);
            }
        }


        public override void Save(string resultFilePath, string includedFilesPath)
        {
            using (FileStream archUsTar = File.Create(resultFilePath))
            {
                using (GZipWriter gZip = new GZipWriter(archUsTar))
                {
                    foreach (var file in FileMap)
                    {
                        string destFileName = file.Value;
                        string sourceFilePath = file.Key;
                        if (sourceFilePath.Contains(ExecutionContext.PACKAGE_SOURCE_TAG))
                        {
                            sourceFilePath = sourceFilePath.Replace(ExecutionContext.PACKAGE_SOURCE_TAG, includedFilesPath);
                        }

                        string incPath = PACKAGE_INCLUDED_FILES_DIRECTORY;
                        if (destFileName == MAIN_PACKAGE_FILENAME)
                        {
                            incPath = "";
                        }
                        if (File.Exists(sourceFilePath))
                        {
                            // сохранение файлов
                            using (FileStream sourceFileStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
                            {
                                destFileName = Path.Combine(incPath, destFileName);
                                gZip.Write(sourceFileStream, destFileName);
                            }
                        }
                        else if (Directory.Exists(sourceFilePath))
                        {
                            // сохранение папок
                            IOUtil.IterateDirectory(sourceFilePath, (fullPath) =>
                            {
                                string relativePath = fullPath.Substring(sourceFilePath.Length + 1);
                                relativePath = Path.Combine(destFileName, relativePath);
                                relativePath = Path.Combine(incPath, relativePath);
                                if (Directory.Exists(fullPath))
                                {
                                    gZip.Write(null, relativePath);
                                }
                                else
                                {
                                    using (FileStream sourceFileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                                    {
                                        gZip.Write(sourceFileStream, relativePath);
                                    }
                                }
                            });
                        }
                    }
                }
            }
        }

    }
}
