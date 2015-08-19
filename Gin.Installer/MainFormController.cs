using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Logging;
using Gin.Logging;


namespace Gin.Installer
{

    enum MainFormState
    {
        PackageExecuting,
        Idle
    }

    class MainFormController
    {
        public void OpenPackageInvoke(string fileName)
        {
 
        }

        public void SetExecutionProgress(ExecutionProgress progress)
        {
 
        }

        public event Action<MainFormState> FormStateChanged;
    }
}
