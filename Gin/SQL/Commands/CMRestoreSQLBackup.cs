using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Gin;
using Gin.Attributes;
using Gin.Context;
using Gin.Logging;
using Gin.Commands;
using Gin.Transactions;
using Gin.SQL.Util;
using Gin.Util;


namespace Gin.SQL.Commands
{
    [GinName(Name = "Восстановить бэкап", Description = "Восстанавливает бэкап SQL-базы", Group = "SQL")]
    public class CMRestoreSQLBackup : TransactionalCommand, IContentCommand, IProgressMyself, ICanCreateFromFile
    {

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Name = "Строка подключения к БД", Description = "Строка подключения к БД. Можно использовать шаблоны.")]
        /// <summary>
        /// Строка подключения к БД (можно использовать шаблоны)
        /// </summary>
        public string ConnectionString { get; set; }

        [GinArgumentText(AllowTemplates = true, Name = "Имя БД", Description = "Имя базы данных. Можно использовать шаблоны.")]
        /// <summary>
        /// Имя базы данных (можно использовать шаблоны)
        /// </summary>
        public string DatabaseName { get; set; }

        [GinArgumentBrowseFile(AllowTemplates = true, Name = "Файл", Description = "Полный путь к файлу бэкапа. Можно использовать шаблоны.")]
        /// <summary>
        /// Полный путь к файлу бэкапа (можно использовать шаблоны)
        /// </summary>
        public string BackupFilePath { get; set; }

        [GinArgumentBrowseFolder(AllowTemplates = true, Name = "Путь к файлам БД", Description = "Путь к папке, куда будут помещены MDF/LDF-файлы базы. Можно использовать шаблоны.")]
        /// <summary>
        /// Путь, куда будут помещены файлы данных
        /// </summary>
        public string DataBaseFullPath { get; set; }

        [GinArgumentInt(Name = "Таймаут", Description = "Таймаут восстановления бэкапа в секундах")]
        /// <summary>
        /// Таймаут команды, Необязательный папаметр, по умолчанию 3 минуты (настраивается в SQLConst)
        /// </summary>
        public int CommandTimeout { get; set; }


        #endregion

        #region Константы модуля

        private const string QUERY_RESTORE_BACKUP_TEMPLATE =
@"USE MASTER
--{command_guid}
RESTORE DATABASE @BASENAME FROM DISK = @FULLNAME
WITH MOVE @MDFLOGICALNAME TO @MDFFILEPATH,
MOVE @LDFLOGICALNAME TO @LDFFILEPATH,
REPLACE";

        private const string RESTORE_FILELIST_TEMPLATE = 
@"RESTORE FILELISTONLY FROM  DISK = @FULLNAME WITH  NOUNLOAD";

        private const string MODULE_NAME = "Модуль восстановления бэкапов";

        private const string PROGRESS_MESSAGE = "Идет восстановление бэкапа";

        private readonly TimeSpan POLLING_PERIOD = new TimeSpan(0, 0, 10);

        #endregion


        public CMRestoreSQLBackup()
        {
            CommandTimeout = SQLConst.DEFAULT_SQL_COMMAND_TIMEOUT;
        }

        public override CommandResult Do(IExecutionContext context, Transaction transaction)
        {
            context.Log.AddLogInformation("Вход в SQLRestoreBackup.Do(ExecutionContext,Transaction)");
            FileSetStep step = null;
            if (transaction != null)
            {
                step = transaction.CreateStep<FileSetStep>(this);
                context.Log.AddLogInformation("Шаг транзакции типа SingleFileStep создан");
                List<string> files = new List<string>();
                _commandGuid = Guid.NewGuid().ToString("N");
                _cmd = CreateMainCommand(context, _commandGuid);

                step.Init(context, files);
                context.Log.AddLogInformation("Шаг транзакции типа SingleFileStep настроен");
            }
            Do(context);
            context.Log.AddLogInformation("Выход из SQLRestoreBackup.Do(ExecutionContext,Transaction)");
            return CommandResult.Next;
        }

        CMExecuteSQLNonQuery _cmd = null;
        string _commandGuid = null;

        public override CommandResult Do(IExecutionContext context)
        {
            context.Log.AddLogInformation("Вход в SQLRestoreBackup.Do(ExecutionContext)");
            SqlCommandProgress progress = null;
            try
            {
                string absoluteConnectionString = context.GetStringFrom(ConnectionString);

                if (_cmd == null)
                {
                    _commandGuid = Guid.NewGuid().ToString("N");
                    _cmd = CreateMainCommand(context, _commandGuid);
                }

                int startProgress = context.Log.CurrentProgress;
                progress = new SqlCommandProgress(_commandGuid, absoluteConnectionString, POLLING_PERIOD);
                progress.OnProgress += new ProgressEvent((percent) =>
                {
                    IncProgress(context, percent, startProgress);
                });
                context.Log.AddLogInformation("Объект Progress запущен");


                CancellingExecutor executor = new CancellingExecutor(() =>
                {
                    QueryCancelEventArgs args = new QueryCancelEventArgs();
                    context.Log.GetPendingCancel(args);
                    return args.Cancel;
                });

                executor.Execute(() =>
                {
                    _cmd.Do(context);
                });
                
                context.Log.AddLogInformation("SQL-команда RESTORE_DATABASE выполнена");
            }
            finally
            {
                if (progress != null)
                {
                    progress.StopPolling();
                    context.Log.AddLogInformation("Объект Progress остановлен");
                }
            }
            context.Log.AddLogInformation("Выход из SQLRestoreBackup.Do(ExecutionContext)");
            return CommandResult.Next;
        }

        private CMExecuteSQLNonQuery CreateMainCommand(IExecutionContext context, string commandGuid)
        {

            List<SqlParameterClass> parameters = InitFilelistParameters(context);
            context.Log.AddLogInformation("Параметры SQL-команды RESTORE_FILELIST инициализированы");
            string resultSetCtxName = Guid.NewGuid().ToString("N");
            string absoluteConnectionString = context.GetStringFrom(ConnectionString);
            CMExecuteSQLQuery query = new CMExecuteSQLQuery()
            {
                CommandType = CommandType.Text,
                CommandText = RESTORE_FILELIST_TEMPLATE.Replace("{command_guid}", commandGuid),
                CommandTimeout = CommandTimeout,
                ConnectionString = absoluteConnectionString,
                Parameters = parameters,
                ResultName = resultSetCtxName
            };
            query.Do(context);
            context.Log.AddLogInformation("SQL-команда RESTORE_FILELIST выполнена");

            parameters = InitRestoreParameters(context, resultSetCtxName);
            context.Log.AddLogInformation("Параметры SQL-команды RESTORE_DATABASE инициализированы");
            CMExecuteSQLNonQuery cmd = new CMExecuteSQLNonQuery()
            {
                CommandType = CommandType.Text,
                CommandText = QUERY_RESTORE_BACKUP_TEMPLATE,
                CommandTimeout = CommandTimeout,
                ConnectionString = absoluteConnectionString,
                Parameters = parameters
            };
            return cmd;
        }

        private void IncProgress(IExecutionContext context, int percent, int startProgress)
        {
            string uiMessage = "Восстановление бэкапа БД завершено на " + percent.ToString() + "%";
            context.Log.AddLogInformation(uiMessage);
            // счисаем прогресс пакета на основании прогресса команды
            int percentLocal = _progressCost * percent / 100;
            int currentPackageProgress = startProgress + percentLocal;
            if (currentPackageProgress > context.Log.CurrentProgress)
            {
                int additivePercents = currentPackageProgress - context.Log.CurrentProgress;
                // в прогрессбаре отображаем текущий прогресс всего пакета
                context.Log.SendProgress(new ExecutionProgressInfo()
                {
                    Message = uiMessage,
                    ModuleName = MODULE_NAME,
                    ProgressCost = additivePercents
                });
            }
        }

        private List<SqlParameterClass> InitFilelistParameters(IExecutionContext context)
        {
            List<SqlParameterClass> parameters = new List<SqlParameterClass>();

            string absoluteBackupFilePath = context.GetStringFrom(BackupFilePath);
            SqlParameterClass p = context.AddSqlParameterToContext("FULLNAME", absoluteBackupFilePath, SqlDbType.VarChar, 1024, ParameterDirection.Input);
            parameters.Add(p);

            return parameters;
        }

        private List<SqlParameterClass> InitRestoreParameters(IExecutionContext context, string resultSetCtxName)
        {
            List<SqlParameterClass> parameters = new List<SqlParameterClass>();

            DataTable fileList = (DataTable)context.GetResult(resultSetCtxName);
            var files = fileList.AsEnumerable();
            var rows = files.AsEnumerable();
            var rowDataFile = rows.FirstOrDefault(r => (string)r["Type"] == "D");
            if (rowDataFile == null)
            {
                throw new Exception("Бэкап не содержит файла MDF базы данных");
            }
            var rowLogFile = rows.FirstOrDefault(r => (string)r["Type"] == "L");
            if (rowLogFile == null)
            {
                throw new Exception("Бэкап не содержит файла LDF журнала транзакций");
            }
            string ldfFileLogicalName = (string)rowLogFile["LogicalName"];
            string mdfFileLogicalName = (string)rowDataFile["LogicalName"];
            string ldfFilePhysicalName = (string)rowLogFile["PhysicalName"];
            string mdfFilePhysicalName = (string)rowDataFile["PhysicalName"];

            if (DataBaseFullPath != null)
            {
                string mdfFileOnlyName = Path.GetFileName(mdfFilePhysicalName);
                string ldfFileOnlyName = Path.GetFileName(ldfFilePhysicalName);

                DataBaseFullPath = context.GetStringFrom(DataBaseFullPath);

                mdfFilePhysicalName = Path.Combine(DataBaseFullPath, mdfFileOnlyName);
                ldfFilePhysicalName = Path.Combine(DataBaseFullPath, ldfFileOnlyName);
            }

            SqlParameterClass p = context.AddSqlParameterToContext("BASENAME", DatabaseName, SqlDbType.VarChar, 1024, ParameterDirection.Input);
            parameters.Add(p);

            string absoluteSourcePath = context.GetStringFrom(BackupFilePath);
            p = context.AddSqlParameterToContext("FULLNAME", absoluteSourcePath, SqlDbType.VarChar, 1024, ParameterDirection.Input);
            parameters.Add(p);
            p = context.AddSqlParameterToContext("MDFLOGICALNAME", mdfFileLogicalName, SqlDbType.VarChar, 1024, ParameterDirection.Input);
            parameters.Add(p);

            p = context.AddSqlParameterToContext("MDFFILEPATH", mdfFilePhysicalName, SqlDbType.VarChar, 1024, ParameterDirection.Input);
            parameters.Add(p);

            p = context.AddSqlParameterToContext("LDFLOGICALNAME", ldfFileLogicalName, SqlDbType.VarChar, 1024, ParameterDirection.Input);
            parameters.Add(p);

            p = context.AddSqlParameterToContext("LDFFILEPATH", ldfFilePhysicalName, SqlDbType.VarChar, 1024, ParameterDirection.Input);
            parameters.Add(p);

            return parameters;
        }

        public override void Rollback(TransactionStep step)
        {
            step.Rollback();
        }

        public override void Commit(TransactionStep step)
        {
            step.Commit();
        }

        public string ContentPath
        {
            get
            {
                return BackupFilePath;
            }
            set
            {
                BackupFilePath = value;
            }
        }

        #region ICanCreateFromFile Members

        public bool IsAssumedCommand(string fileName)
        {
            return Path.GetExtension(fileName).ToLower() == ".bak";
        }

        public void InitFromFile(string fileName)
        {
            BackupFilePath = fileName;
        }

        #endregion

    }
}
