using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Gin;
using Gin.Attributes;
using Gin.Commands;
using Gin.Context;
using Gin.Transactions;
using Gin.SQL.Util;
using Gin.Logging;
using Gin.Util;


namespace Gin.SQL.Commands
{

    [GinName(Name = "Создать бэкап", Description = "Создает бэкап SQL-базы", Group = "SQL")]
    /// <summary>
    /// Команда создает бэкап SQL-базы
    /// </summary>
    public class CMCreateSQLBackup : TransactionalCommand, IProgressMyself
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

        [GinArgumentBrowseFile(AllowTemplates = true, Name = "Файл", Description = "Полный путь к файлу бэкапа. Можно использовать шаблоны.", IsNewFile = true)]
        /// <summary>
        /// Полный путь к файлу бэкапа (можно использовать шаблоны)
        /// </summary>
        public string BackupFilePath { get; set; }

        [GinArgumentInt(Name = "Таймаут", Description = "Таймаут создания бэкапа в секундах")]
        /// <summary>
        /// Таймаут команды, Необязательный параметр, по умолчанию 3 минуты (настраивается в SQLConst)
        /// </summary>
        public int CommandTimeout { get; set; }

        #endregion


        #region Константы модуля

        private const string QUERY_CREATE_BACKUP_TEMPLATE = 
@"SET NOCOUNT ON
--{command_guid}
BACKUP DATABASE @BASENAME TO DISK = @FULLNAME
WITH INIT, NOUNLOAD, NAME = @NAME, 
NOSKIP, STATS = 5";        
        
        private const string MODULE_NAME = "Модуль создания бэкапов";

        private const string PROGRESS_MESSAGE = "Идет создание бэкапа";

        private const string BACKUP_EXTENSION = ".bak";

        private readonly TimeSpan POLLING_PERIOD = new TimeSpan(0, 0, 10);

        #endregion

        private SqlCommandProgress progress = null;

        public CMCreateSQLBackup()
        {
            _progressCost = 100;
            CommandTimeout = SQLConst.DEFAULT_SQL_COMMAND_TIMEOUT;
        }

        public override CommandResult Do(IExecutionContext context, Transaction transaction)
        {
            context.Log.AddLogInformation("Вход в SQLCreateBackup.Do(ExecutionContext,Transaction)");
            string absoluteBackupFilePath = context.GetStringFrom(BackupFilePath);
            context.Log.AddLogInformation("BackupFilePath = '" + absoluteBackupFilePath + "'");
            SingleFileStep step = null;
            if (transaction != null)
            {
                step = transaction.CreateStep<SingleFileStep>(this);
                context.Log.AddLogInformation("Шаг транзакции типа SingleFileStep создан");
                step.Init(context, absoluteBackupFilePath);
                context.Log.AddLogInformation("Шаг транзакции типа SingleFileStep настроен");
            }
            Do(context);
            context.Log.AddLogInformation("Выход из SQLCreateBackup.Do(ExecutionContext,Transaction)");
            return CommandResult.Next;
        }

        public override CommandResult Do(IExecutionContext context)
        {
            context.Log.AddLogInformation("Вход в SQLCreateBackup.Do(ExecutionContext)");
            try
            {
                string absoluteConnectionString = context.GetStringFrom(ConnectionString);
                context.Log.AddLogInformation("ConnectionString ='" + absoluteConnectionString + "'");
                string commandGuid = Guid.NewGuid().ToString("N");
                List<SqlParameterClass> parameters = InitParameters(context);
                context.Log.AddLogInformation("Параметры SQL-команды инициализированы");
                int startProgress = context.Log.CurrentProgress;
                progress = new SqlCommandProgress(commandGuid, absoluteConnectionString, POLLING_PERIOD);
                progress.OnProgress += new ProgressEvent((percent) =>
                {
                    IncProgress(context, percent, startProgress);
                });
                context.Log.AddLogInformation("Экземпляр SqlCommandProgress создан и запущен");

                CMExecuteSQLNonQuery query = new CMExecuteSQLNonQuery()
                {
                    CommandType = CommandType.Text,
                    CommandText = QUERY_CREATE_BACKUP_TEMPLATE.Replace("{command_guid}", commandGuid),
                    CommandTimeout = CommandTimeout,
                    ConnectionString = absoluteConnectionString,
                    Parameters = parameters
                };
                //query.Do(context);

                CancellingExecutor executor = new CancellingExecutor(() =>
                {
                    QueryCancelEventArgs args = new QueryCancelEventArgs();
                    context.Log.GetPendingCancel(args);
                    return args.Cancel;
                });

                executor.Execute(() =>
                {
                    query.Do(context);
                });



                context.Log.AddLogInformation("Экземпляр SQL-команды выполняющей бэкап БД выполнен");
                IncProgress(context, 100, startProgress);
            }
            finally
            {
                if (progress != null)
                {
                    progress.StopPolling();
                    context.Log.AddLogInformation("Экземпляр SqlCommandProgress остановлен");
                }
                context.Log.AddLogInformation("Выход из SQLCreateBackup.Do(ExecutionContext)");
            }

            return CommandResult.Next;
        }

        private List<SqlParameterClass> InitParameters(IExecutionContext context)
        {
            List<SqlParameterClass> parameters = new List<SqlParameterClass>();
       
            string absoluteDatabaseName = context.GetStringFrom(DatabaseName);
            context.Log.AddLogInformation("DatabaseName = '" + absoluteDatabaseName + "'");
            SqlParameterClass p = context.AddSqlParameterToContext("BASENAME", absoluteDatabaseName, SqlDbType.VarChar, 255, ParameterDirection.Input);
            parameters.Add(p);

            string absoluteBackupFilePath = context.GetStringFrom(BackupFilePath);
            context.Log.AddLogInformation("BackupFilePath = '" + absoluteBackupFilePath + "'");
            string backupName = Path.GetFileNameWithoutExtension(absoluteBackupFilePath);
            p = context.AddSqlParameterToContext("NAME", backupName, SqlDbType.VarChar, 255, ParameterDirection.Input);
            parameters.Add(p);
            p = context.AddSqlParameterToContext("FULLNAME", absoluteBackupFilePath, SqlDbType.VarChar, 255, ParameterDirection.Input);
            parameters.Add(p);
            return parameters;
        }

        private void IncProgress(IExecutionContext context, int percent, int startProgress)
        {
            string uiMessage = "Создание бэкапа БД завершено на " + percent.ToString() + "%";
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

        public override void Rollback(TransactionStep step)
        {
            step.Rollback();
        }

        public override void Commit(TransactionStep step)
        {
            step.Commit();
        }

    }
}