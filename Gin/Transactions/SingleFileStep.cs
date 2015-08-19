using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gin.Context;

namespace Gin.Transactions
{
    public class SingleFileStep: TransactionStep
    {
        public string BackupFilePath { get; set; }

        public string SourceFilePath { get; set; }

        public void Init(IExecutionContext context, string oldFilePath)
        {
            SourceFilePath = oldFilePath;
            if (File.Exists(oldFilePath))
            {
                string rollbackFileName = Guid.NewGuid().ToString("N") + ".rlb";
                string dataPath = GetPath(context.ExecutedPackage.TransactionsPath);
                string rollbackFilePath = Path.Combine(dataPath, rollbackFileName);
                BackupFilePath = rollbackFilePath;
                File.Copy(oldFilePath, rollbackFilePath);
            }
        }

        public override void Rollback()
        {
            if (File.Exists(BackupFilePath))
            {
                File.Copy(BackupFilePath, SourceFilePath, true);
            }
            else
            {
                File.Delete(SourceFilePath);
            }
        }

        public override void Commit()
        {
            Directory.Delete(BackupFilePath, true);
        }

    }
}
