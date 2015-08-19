using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;


namespace Gin.WCF
{
    public enum PackageExecutionState
    {
        Executing,
        Aborted,
        Complete,
        Fixed,
        RolledBack,
        Preparing
    }

    public class RemotePackageState
    {
        public string PackageName;
        public DateTime? Started;
        public DateTime? AutoRollback;
        public string LogPath;
        public int Progress;
        public string ProgressMessage;
        public PackageExecutionState State;
        public Exception ExecuteException;
    }
}
