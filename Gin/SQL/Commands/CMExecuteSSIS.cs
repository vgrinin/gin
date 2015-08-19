/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Gin;
using Gin.Logging;
using Gin.Commands;
using sql = Microsoft.SqlServer.Dts.Runtime;

namespace Gin.SQL.Commands
{
    [GinName(Name = "Выполнить SSIS пакет", Description = "Выполняет SSIS пакет", Group = "SQL")]
    public class CMExecuteSSIS: Command, IContentCommand
    {

        #region Аргументы команды

        [GinArgumentBrowseFile(Name = "Путь к пакету", Description = "Путь к пакету")]
        public string PackagePath { get; set; }

        [GinArgumentList(ListType = typeof(List<DTSGlobalVariable>), Name = "Имена параметров", Description = "Список имен параметров")]
        /// <summary>
        /// Контекстные имена параметров.
        /// </summary>
        public List<DTSGlobalVariable> ParameterNames { get; set; }

        #endregion

        public const string MODULE_NAME = "SSIS Package Executor";

        public override CommandResult Do(ExecutionContext context)
        {
            string absolutePackagePath = context.GetStringFrom(PackagePath);
            sql.Application ssisApplication = new sql.Application();
            SSISPackageEvents ssisEvents = new SSISPackageEvents();
            sql.Package ssisPackage = ssisApplication.LoadPackage(absolutePackagePath, ssisEvents);
            sql.DTSExecResult ssisResult = ssisPackage.Execute();
            return CommandResult.Next;
        }

        [XmlIgnore]
        public string ContentPath
        {
            get
            {
                return PackagePath;
            }
            set 
            {
                PackagePath = value;
            }
        }
    }

    public class SSISPackageEvents : sql.DefaultEvents
    {
        public event ProgressEvent OnProgressChange;
        public event LoggerEvent OnLogChange;

        private void SendProgressLog(int percent, string eventText)
        {
            SendProgress(percent, eventText);
            SendLog(eventText);
        }

        private void SendProgress(int percent, string eventText)
        {
            if (OnProgressChange != null)
            {
                OnProgressChange(percent);
            }
        }

        private void SendLog(string eventText)
        {
            if (OnLogChange != null)
            {
                OnLogChange(ExecutionLoggerEventType.Information, eventText);
            }
        }

        public override void OnCustomEvent(sql.TaskHost taskHost, string eventName, string eventText, ref object[] arguments, string subComponent, ref bool fireAgain)
        {
            base.OnCustomEvent(taskHost, eventName, eventText, ref arguments, subComponent, ref fireAgain);
            SendLog(eventText);
        }


        public override bool OnError(sql.DtsObject source, int errorCode, string subComponent, string description, string helpFile, int helpContext, string idofInterfaceWithError)
        {
            SendProgressLog(100, description);
            return base.OnError(source, errorCode, subComponent, description, helpFile, helpContext, idofInterfaceWithError);
        }

        public override void OnProgress(sql.TaskHost taskHost, string progressDescription, int percentComplete, int progressCountLow, int progressCountHigh, string subComponent, ref bool fireAgain)
        {
            base.OnProgress(taskHost, progressDescription, percentComplete, progressCountLow, progressCountHigh, subComponent, ref fireAgain);
            SendProgressLog(percentComplete, progressDescription);
        }

        public override void OnTaskFailed(sql.TaskHost taskHost)
        {
            base.OnTaskFailed(taskHost);
            SendProgressLog(100, "TaskFailed!!!");
        }

        public override void OnExecutionStatusChanged(sql.Executable exec, sql.DTSExecStatus newStatus, ref bool fireAgain)
        {
            base.OnExecutionStatusChanged(exec, newStatus, ref fireAgain);
            SendLog(newStatus.ToString());
        }
    }
}*/