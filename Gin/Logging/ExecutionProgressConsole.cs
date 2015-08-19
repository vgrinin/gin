using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gin.Logging
{
    public class ExecutionProgressConsole: ExecutionProgress
    {
        protected override void VisualizeProgress(ExecutionProgressInfo progressInfo)
        {
            Console.WriteLine("Progress in " + progressInfo.ModuleName + "(" + progressInfo.Message + ") is " + Current);
        }
    }
}
