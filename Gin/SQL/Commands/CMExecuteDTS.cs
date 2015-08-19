using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Attributes;
using Gin.Commands;
using Gin;
using Gin.Context;
using Gin.Logging;
using DTS;
using System.IO;
using Gin.SQL.Util;
using Gin.Util;


namespace Gin.SQL.Commands
{

    [GinName(Name = "Выполнить DTS", Description = "Выполняет DTS-пакет", Group = "SQL")]
    public class CMExecuteDTS : Command, IContentCommand, ICanCreateFromFile
    {

        #region Аргументы команды

        [GinArgumentBrowseFile(AllowTemplates = true, Name = "Файл", Description = "Полный путь к dts-файлу. Можно использовать шаблоны.")]
        public string PackageFilePath { get; set; }

        [GinArgumentList(ListType = typeof(List<DTSGlobalVariable>), Name = "Параметры", Description = "Параметры")]
        public List<DTSGlobalVariable> Parameters { get; set; }

        #endregion

        public CMExecuteDTS()
        {
            Parameters = new List<DTSGlobalVariable>();
        }

        public override CommandResult Do(IExecutionContext context)
        {
            string absolutePackageFilePath = context.GetStringFrom(PackageFilePath);
            string logFilePath = Path.Combine(context.ExecutedPackage.PackagePath, Guid.NewGuid().ToString("N") + ".log");
            DTSExecutor executor = new DTSExecutor(absolutePackageFilePath, logFilePath, false);
            executor.OnLogger += new LoggerEvent(context.Log.AddLogEvent);
            executor.OnProgress += new ProgressEvent(
                (percent)=>
                {
                    context.Log.SendProgress(new ExecutionProgressInfo()
                    {
                        Message = "Выполнение DTS-пакета",
                        ModuleName = "CMExecuteDTS",
                        ProgressCost = 0
                    });
                    //CheckForPendingCancel(context);
                });
            foreach (DTSGlobalVariable param in Parameters)
            {
                object absoluteVariableValue = context.GetResult(param.VariableValue);
                executor.SaveParameter(param.VariableName, absoluteVariableValue);
            }

            CancellingExecutor cnclexecutor = new CancellingExecutor(() =>
            {
                QueryCancelEventArgs args = new QueryCancelEventArgs();
                context.Log.GetPendingCancel(args);
                return args.Cancel;
            });

            cnclexecutor.Execute(() =>
            {
                executor.Execute();
            });


            return CommandResult.Next;
        }

        public string ContentPath
        {
            get
            {
                return PackageFilePath;
            }
            set
            {
                PackageFilePath = value;
            }
        }

        #region ICanCreateFromFile Members

        public bool IsAssumedCommand(string fileName)
        {
            return Path.GetExtension(fileName).ToLower() == ".dts";
        }

        public void InitFromFile(string fileName)
        {
            PackageFilePath = fileName;
        }

        #endregion

    }
}
