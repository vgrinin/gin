using System.Linq;
using System.IO;
using Gin.Context;
using Gin.Util;
using Gin.Logging;


namespace Gin
{
    public class PackageContentDirect : PackageContent.PackageContent
    {

        public override void Load(string packageContentDirectory, string packageResultFilePath)
        {
            PackageBodyFilePath = packageResultFilePath;
            PackageContentDirectory = packageContentDirectory;
            try
            {
                string packageDirectory = Path.GetDirectoryName(packageResultFilePath);
                string packageIncludedDirectory = Path.Combine(packageDirectory, PACKAGE_INCLUDED_FILES_DIRECTORY);

                if (Directory.Exists(packageIncludedDirectory))
                {
                    int totalFilesCount = Directory.GetFiles(packageIncludedDirectory, "*.*", SearchOption.AllDirectories).Count();
                    int currentFilesCount = 0;
                    int oldProgressValue = 0;
                    packageContentDirectory = Path.Combine(packageContentDirectory, PACKAGE_INCLUDED_FILES_DIRECTORY);
                    IOUtil.CopyDirectory(packageIncludedDirectory, packageContentDirectory, false, (fileName) =>
                    {
                        currentFilesCount++;
                        int currentProgress = (currentFilesCount * 100) / totalFilesCount;
                        int currentCost = 0;
                        if (currentProgress > oldProgressValue)
                        {
                            currentCost = currentProgress - oldProgressValue;
                        }
                        oldProgressValue = currentProgress;
                        OnProgressHandler(new ExecutionProgressInfo()
                        {
                            Message = fileName,
                            ModuleName = "Direct content",
                            ProgressCost = currentCost
                        });
                        QueryCancelEventArgs args = new QueryCancelEventArgs();
                        OnQueryCancelHandler(args);
                        if (args.Cancel)
                        {
                            throw new PackageExecutionCancelledException();
                        }
                    });
                }
                OnProgressHandler(new ExecutionProgressInfo
                {
                    Message = "Содержимое пакета загружено",
                    ProgressCost = 100,
                    ModuleName = "Direct content"
                });
            }
            catch (PackageExecutionCancelledException)
            {
                Clean();
            }
        }

        public override void LoadBody(string packageContentDirectory, string packageResultFilePath)
        {
            PackageBodyFilePath = packageResultFilePath;
            PackageContentDirectory = packageContentDirectory;
            OnProgressHandler(new ExecutionProgressInfo
            {
                Message = "Тело пакета загружено",
                ProgressCost = 100,
                ModuleName = "Direct content"
            });
        }

        public override string AddContent(string sourcePath)
        {
            string result;
            if (!FileMap.ContainsKey(sourcePath))
            {
                string destFileName = GenerateContentFileName(sourcePath);
                FileMap.Add(sourcePath, destFileName);
                result = destFileName;
            }
            else
            {
                result = FileMap[sourcePath];
            }

            return Path.Combine(ExecutionContext.PACKAGE_CONTENT_TAG, result);
        }

        public override void Save(string resultFilePath, string includedFilesPath)
        {
            string resultDirectoryPath = Path.GetDirectoryName(resultFilePath);
            string resultFileName = Path.GetFileName(resultFilePath);
            string includedDirectoryPath = Path.Combine(resultDirectoryPath, PACKAGE_INCLUDED_FILES_DIRECTORY);
            if (!Directory.Exists(includedDirectoryPath))
            {
                Directory.CreateDirectory(includedDirectoryPath);
            }
            foreach (var file in FileMap)
            {
                string destFileName = file.Value;
                string sourceFilePath = file.Key;
                if (destFileName == MAIN_PACKAGE_FILENAME)
                {
                    continue;
                }
                if (sourceFilePath.Contains(ExecutionContext.PACKAGE_SOURCE_TAG))
                {
                    sourceFilePath = sourceFilePath.Replace(ExecutionContext.PACKAGE_SOURCE_TAG, includedFilesPath);
                }
                //string incPath = PACKAGE_INCLUDED_FILES_DIRECTORY;
                if (destFileName == MAIN_PACKAGE_FILENAME)
                {
                   // incPath = "";
                }
                if (File.Exists(sourceFilePath))
                {
                    // сохранение файлов
                    string destFilePath = Path.Combine(includedDirectoryPath, destFileName);
                    File.Copy(sourceFilePath, destFilePath);
                }
                else if (Directory.Exists(sourceFilePath))
                {
                    // сохранение папок
                    string destFilePath = Path.Combine(includedDirectoryPath, destFileName);
                    IOUtil.CopyDirectory(sourceFilePath, destFilePath, false);
                }
            }
            if (ContainFileName(MAIN_PACKAGE_FILENAME))
            {
                string sourceFilePath = GetFilePath(MAIN_PACKAGE_FILENAME);
                string destFilePath = Path.Combine(resultDirectoryPath, resultFileName);
                File.Copy(sourceFilePath, destFilePath, true);
            }
        }
    }
}
