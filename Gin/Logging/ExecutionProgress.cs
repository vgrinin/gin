using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gin.Logging
{

    public interface IProgressMyself { };

    public delegate void ProgressEvent(int percent);

    public class ExecutionProgressInfo
    {
        public string Message { get; set; }
        public int ProgressCost { get; set; }
        public string ModuleName { get; set; }
    }

    public class QueryCancelEventArgs: EventArgs
    {
        public bool Cancel { get; set; }
    }

    public class PackageExecutionCancelledException : Exception { }

    public class PackageAlreadyExecutedException : Exception
    {
        public PackageAlreadyExecutedException() : base("Пакет был выполнен ранее") { }
    }

    public abstract class ExecutionProgress
    {

        public const string PROGRESS_FILENAME = "progress.txt";

        protected int _totalCost = 1;
        public int TotalCost 
        {
            set
            {
                _totalCost = value;
            }
            get
            {
                return _totalCost;
            }
        }

        private int _currentProgress = 0;

        public int Current
        {
            set
            {
                _currentProgress = value;
            }
            get
            {
                return _currentProgress;
            }
        }


        public void Progress(ExecutionProgressInfo progressInfo)
        {
            _currentProgress += progressInfo.ProgressCost;
            VisualizeProgress(progressInfo);
        }

        protected abstract void VisualizeProgress(ExecutionProgressInfo progressInfo);

    }
}
