using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gin.Logging;

namespace Gin
{
    public class PackageContentEmpty : PackageContent.PackageContent
    {

        public override string AddContent(string sourcePath)
        {
            if (!FileMap.ContainsKey(sourcePath))
            {
                string destFileName = GenerateContentFileName(sourcePath);
                AddContent(sourcePath, destFileName);
                return sourcePath;
            }
            else
            {
                return sourcePath;
            }
        }


        public override void Save(string resultFilePath, string includedFilesPath)
        {
            if (ContainFileName(PackageContent.PackageContent.MAIN_PACKAGE_FILENAME))
            {
                string sourceFilePath = GetFilePath(PackageContent.PackageContent.MAIN_PACKAGE_FILENAME);
                File.Copy(sourceFilePath, resultFilePath, true);
            }
        }

        public override void Load(string packageContentDirectory, string packageResultFilePath)
        {
            PackageBodyFilePath = packageResultFilePath;
            PackageContentDirectory = packageContentDirectory;
            OnProgressHandler(new ExecutionProgressInfo()
            {
                Message = "Содержимое пакета загружено",
                ProgressCost = 100,
                ModuleName = "Empty content"
            });
        }
        public override void LoadBody(string packageContentDirectory, string packageResultFilePath)
        {
            PackageBodyFilePath = packageResultFilePath;
            PackageContentDirectory = packageContentDirectory;
            OnProgressHandler(new ExecutionProgressInfo()
            {
                Message = "Тело пакета загружено",
                ProgressCost = 100,
                ModuleName = "Empty content"
            });
        }
    }
}
