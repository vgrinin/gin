using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gin.Context;
using Gin.Util;


namespace Gin.Transactions
{
    public class SingleDirectoryStep : TransactionStep
    {
        public string SourcePath { get; set; }

        public string BackupPath { get; set; }

        public void Init(IExecutionContext context, string oldPath)
        {
            SourcePath = oldPath;
            if (Directory.Exists(oldPath))
            {
                string rollbackFileName = Guid.NewGuid().ToString("N") + ".rlb";
                string dataPath = GetPath(context.ExecutedPackage.TransactionsPath);
                string rollbackPath = Path.Combine(dataPath, rollbackFileName);
                BackupPath = rollbackPath;
                IOUtil.CopyDirectory(oldPath, rollbackPath, false);
            }
        }

        public override void Rollback()
        {
            Directory.Delete(SourcePath, true);
            if (Directory.Exists(BackupPath))
            {
                IOUtil.CopyDirectory(BackupPath, SourcePath, false);
            }
        }

        public override void Commit()
        {
            Directory.Delete(BackupPath, true);
        }

    }

}
