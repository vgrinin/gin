using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Logging;
using System.ServiceModel;


namespace Gin.WCF
{
    [ServiceContract(Namespace = "http://Gin.Service")]
    public interface IRemotePackageService
    {
        [OperationContract]
        string Invoke(string filePath);

        [OperationContract]
        RemotePackageState GetState(string cookie);

        [OperationContract]
        void Abort(string cookie);

        [OperationContract]
        void Commit(string cookie);

        [OperationContract]
        void Rollback(string cookie);
    }
}
