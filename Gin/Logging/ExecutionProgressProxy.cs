using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gin.Logging
{
    public class ExecutionProgressProxy: ExecutionProgress
    {

        private Action<ExecutionProgressInfo, int, int> _onProgress;

        public ExecutionProgressProxy(Action<ExecutionProgressInfo, int, int> onProgress)
        {
            _onProgress = onProgress;
        }

        protected override void VisualizeProgress(ExecutionProgressInfo progressInfo)
        {
            if (_onProgress != null)
            {
                _onProgress(progressInfo, Current, TotalCost);
            }
        }
    }
}
