using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gin.Context;

namespace Gin.Transactions
{

    public class FilesPair
    {
        public string BackupFilePath { get; set; }
        public string SourceFilePath { get; set; }
    }

    public class FileSetStep: TransactionStep
    {

        public List<FilesPair> Files { get; set; }

        public void Init(IExecutionContext context, List<string> oldFilePaths)
        {
            foreach (var oldFilePath in oldFilePaths)
            {
                FilesPair pair = new FilesPair();
                pair.SourceFilePath = oldFilePath;
                if (File.Exists(oldFilePath))
                {
                    string rollbackFileName = Guid.NewGuid().ToString("N") + ".rlb";
                    string dataPath = GetPath(context.ExecutedPackage.TransactionsPath);
                    string rollbackFilePath = Path.Combine(dataPath, rollbackFileName);
                    pair.BackupFilePath = rollbackFilePath;
                    File.Copy(oldFilePath, rollbackFilePath);
                }
                Files.Add(pair);
            }
        }

        public override void Rollback()
        {
            foreach (var pair in Files)
            {
                if (File.Exists(pair.BackupFilePath))
                {
                    File.Copy(pair.BackupFilePath, pair.SourceFilePath, true);
                }
                else
                {
                    File.Delete(pair.SourceFilePath);
                }
            } 
        }

        public override void Commit()
        {
            foreach (var pair in Files)
            {
                Directory.Delete(pair.BackupFilePath, true);
            }
        }

    }
}
