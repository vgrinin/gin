using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin;
using Gin.Attributes;
using Gin.Commands;
using Gin.Context;
using Gin.Transactions;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using Gin.Util;
using System.IO;
using System.Xml.Serialization;


namespace Gin.SQL.Commands
{

    [GinName(Name = "Выполнить SQL-запрос", Description = "Выполняет SQL-команду с возвратом данных", Group = "SQL")]
    public class CMExecuteSQLQuery : Command, IContentCommand, ICanCreateFromFile
    {

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Name = "Строка подключения к БД", Description = "Строка подключения к БД. Можно использовать шаблоны.")]
        /// <summary>
        /// Стркоа подключения к БД, можно использовать шаблоны
        /// </summary>
        public string ConnectionString { get; set; }

        [GinArgumentText(Multiline = true, Name = "Текст команды", Description = "Текст SQL-команды. Игнорируется, если задан ScriptFilePath. Шаблоны не используются")]
        /// <summary>
        /// Текст SQL-команды. Игнорируется, если задан ScriptFilePath. Шаблоны не используются.
        /// </summary>
        public string CommandText { get; set; }

        [GinArgumentEnum(ListEnum = typeof(CommandType), Name = "Тип команды", Description = "Тип команды. Игнорируется, если задан ScriptFilePath(для него тип = Text)")]
        /// <summary>
        /// Тип команды. Игнорируется, если задан ScriptFilePath(для него тип = Text)
        /// </summary>
        public CommandType CommandType { get; set; }

        [GinArgumentBrowseFile(AllowTemplates = true, Name = "Файл", Description = "Путь к файлу, содержащему SQL-скрипт. Можно использовать шаблоны.")]
        /// <summary>
        /// Путь к файлу, содержащему SQL-скрипт. Можно использовать шаблоны.
        /// </summary>
        public string ScriptFilePath { get; set; }

        [GinArgumentInt(Name = "Таймаут, сек", Description = "Таймаут выполнения команды в секундах")]
        /// <summary>
        /// Таймаут выполнения в секундах.
        /// </summary>
        public int CommandTimeout { get; set; }

        [GinArgumentList(ListType = typeof(List<SqlParameterClass>), Name = "Параметры", Description = "Список параметров")]
        /// <summary>
        /// Sql-параметры.
        /// </summary>
        public List<SqlParameterClass> Parameters { get; set; }

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранен результат выполнения команды RETURN_VALUE")]
        [GinResult(Result = typeof(object), Kind = CommandResultKind.Primitive, Description = "RETURN_VALUE SQL-команды")]
        /// <summary>
        /// Переменная контекста, куда сохраняется RETURN_VALUE команды, если он был получен.
        /// </summary>
        public string ResultName { get; set; }


        #endregion

        public CMExecuteSQLQuery()
        {
            Parameters = new List<SqlParameterClass>();
            CommandTimeout = SQLConst.DEFAULT_SQL_COMMAND_TIMEOUT;
            CommandType = System.Data.CommandType.Text;
        }

        public override CommandResult Do(IExecutionContext context)
        {
            context.Log.AddLogInformation("Вход в метод CMExecuteSQLQuery.Do(ExecutionContext)");
            string absoluteConnectionString = context.GetStringFrom(ConnectionString);
            context.Log.AddLogInformation("ConnectionString = '" + absoluteConnectionString + "'", new ConnectionStringFilter());

            using (SqlConnection connection = new SqlConnection(absoluteConnectionString))
            {
                SqlCommand command;
                if (ScriptFilePath == null)
                {
                    command = new SqlCommand(CommandText, connection);
                    command.CommandType = CommandType;
                }
                else
                {
                    string absoluteScriptFilePath = context.GetStringFrom(ScriptFilePath);
                    string commandText = IOUtil.ReadFile(absoluteScriptFilePath);
                    commandText = context.GetStringFrom(commandText);
                    command = new SqlCommand(commandText, connection);
                    command.CommandType = CommandType.Text;
                }
                if (CommandTimeout > 0)
                {
                    command.CommandTimeout = CommandTimeout;
                }
                else
                {
                    command.CommandTimeout = SQLConst.DEFAULT_SQL_COMMAND_TIMEOUT;
                }
                if (Parameters != null)
                {
                    foreach (SqlParameterClass parameter in Parameters)
                    {
                        object value = context.GetResult(parameter.ValueName);
                        SqlParameter sqlParameter = new SqlParameter(parameter.ParameterName, parameter.Type);
                        sqlParameter.Direction = parameter.Direction;
                        sqlParameter.Value = value != null ? value : DBNull.Value;
                        if (parameter.Size > 0)
                        {
                            sqlParameter.Size = parameter.Size;
                        }
                        command.Parameters.Add(sqlParameter);
                    }
                }
                connection.Open();
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataSet);
                DataTable table = null;
                if (dataSet.Tables.Count > 0)
                {
                    table = dataSet.Tables[0];
                }
                context.SaveResult(ResultName, table);
            }

            return CommandResult.Next;
        }

        public string ContentPath
        {
            get
            {
                return ScriptFilePath;
            }
            set
            {
                ScriptFilePath = value;
            }
        }


        #region ICanCreateFromFile Members

        public bool IsAssumedCommand(string fileName)
        {
            return Path.GetExtension(fileName).ToLower() == ".sql";
        }

        public void InitFromFile(string fileName)
        {
            ScriptFilePath = fileName;
        }

        #endregion
    }
}
