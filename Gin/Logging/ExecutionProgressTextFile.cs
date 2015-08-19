using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Gin.Logging
{
    public class ExecutionProgressTextFile: ExecutionProgress
    {

        string _fileName = null;
        FileStream _fileStream = null;
        StreamWriter _streamWriter = null;


        public ExecutionProgressTextFile(string fileName)
        {
            _fileName = fileName;
        }

        protected override void VisualizeProgress(ExecutionProgressInfo progressInfo)
        {
            _fileStream = new FileStream(_fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            _streamWriter = new StreamWriter(_fileStream);
            _streamWriter.WriteLine(progressInfo.ProgressCost);
            _streamWriter.WriteLine(progressInfo.ModuleName);
            _streamWriter.WriteLine(progressInfo.Message);
            _streamWriter.Close();
        }

    }
}
