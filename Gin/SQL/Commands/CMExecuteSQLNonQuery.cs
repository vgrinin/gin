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
using System.Data.SqlClient;
using Gin.Util;
using System.IO;
using System.Xml.Serialization;
using Gin.Logging;

namespace Gin.SQL.Commands
{

    [GinName(Name = "Выполнить SQL-команду", Description = "Выполняет SQL-команду без возврата данных", Group = "SQL")]
    public class CMExecuteSQLNonQuery : Command, IContentCommand, ICanCreateFromFile
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

        [GinArgumentEnum(Name = "Тип команды", Description = "Тип команды. Игнорируется, если задан ScriptFilePath(для него тип = Text)", ListEnum = typeof(CommandType))]
        /// <summary>
        /// Тип команды. Игнорируется, если задан ScriptFilePath(для него тип = Text)
        /// </summary>
        public CommandType CommandType { get; set; }

        [GinArgumentBrowseFile(AllowTemplates = true, Name = "Файл", Description = "Путь к файлу, содержащему SQL-скрипт. Можно использовать шаблоны.")]
        /// <summary>
        /// Путь к файлу, содержащему SQL-скрипт. Можно использовать шаблоны.
        /// </summary>
        public string ScriptFilePath { get; set; }

        [GinArgumentInt(Name = "Таймаут", Description = "Таймаут выполнения команды в секундах")]
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
        [GinResult(Result = typeof(object), Kind = CommandResultKind.Primitive, Description = "Результат выполнения SQL-команды")]
        /// <summary>
        /// Переменная контекста, куда сохраняется RETURN_VALUE команды, если он был получен.
        /// </summary>
        public string ResultName { get; set; }

        [GinArgumentEnum(Name = "Тип результата", Description = "Тип ожидаемого RETURN_VALUE", ListEnum = typeof(SqlDbType))]
        /// <summary>
        /// Тип ожидаемого RETURN_VALUE.
        /// </summary>
        public SqlDbType ResultType { get; set; }

        [GinArgumentInt(Name = "Размер результата", Description = "Размер ожидаемого RETURN_VALUE")]
        /// <summary>
        /// Размер ожидаемого RETURN_VALUE.
        /// </summary>
        public int ResultSize { get; set; }

        #endregion

        IExecutionContext _context = null;

        public CMExecuteSQLNonQuery()
        {
            Parameters = new List<SqlParameterClass>();
            CommandTimeout = SQLConst.DEFAULT_SQL_COMMAND_TIMEOUT;
            CommandType = System.Data.CommandType.Text;
            ResultType = SqlDbType.Int;
        }

        public override CommandResult Do(IExecutionContext context)
        {
            _context = context;

            context.Log.AddLogInformation("Вход в метод CMExecuteSQLNonQuery.Do(ExecutionContext)");
            string absoluteConnectionString = context.GetStringFrom(ConnectionString);
            context.Log.AddLogInformation("ConnectionString = '" + absoluteConnectionString + "'", new ConnectionStringFilter());
            using (SqlConnection connection = new SqlConnection(absoluteConnectionString))
            {
                string commandText = null;
                CommandType commandType = CommandType.Text;
                if (ScriptFilePath == null)
                {
                    commandText = CommandText;
                    commandType = CommandType;
                    context.Log.AddLogInformation("CommandText считан из аргумента");
                }
                else
                {
                    string absoluteScriptFilePath = context.GetStringFrom(ScriptFilePath);
                    context.Log.AddLogInformation("ScriptFilePath = '" + absoluteScriptFilePath + "'");
                    commandText = IOUtil.ReadFile(absoluteScriptFilePath);
                    commandType = CommandType.Text;
                    context.Log.AddLogInformation("CommandText считан из файла '" + absoluteScriptFilePath + "'");
                }
                context.Log.AddLogInformation("CommandType = " + commandType);
                int commandTimeout = SQLConst.DEFAULT_SQL_COMMAND_TIMEOUT;
                if (CommandTimeout > 0)
                {
                    commandTimeout = CommandTimeout;
                }
                context.Log.AddLogInformation("CommandTimeout = " + commandTimeout);

                List<string> commandTexts = SplitCommandToBatches(commandText);
                connection.Open();
                context.Log.AddLogInformation("Открыли соединение");
                foreach (string text in commandTexts)
                {
                    SqlCommand command = new SqlCommand(text, connection);
                    command.CommandType = commandType;
                    command.CommandTimeout = commandTimeout;
                    AddSqlParameters(command);
                    command.ExecuteNonQuery();
                    context.Log.AddLogInformation("Выполнили очередную команду");
                    SaveReturnValueIfPresent(command);
                    CheckForPendingCancel(context);
                }

            }
            context.Log.AddLogInformation("Выход из метода CMExecuteSQLNonQuery.Do(ExecutionContext)");
            return CommandResult.Next;
        }

        private void AddSqlParameters(SqlCommand command)
        { 
            if (Parameters != null)
            {
                _context.Log.AddLogInformation("Обработка Parameters в количестве = " + Parameters.Count);
                foreach (SqlParameterClass parameter in Parameters)
                {
                    object value = _context.GetResult(parameter.ValueName);
                    SqlParameter sqlParameter = new SqlParameter(parameter.ParameterName, parameter.Type);
                    sqlParameter.Direction = parameter.Direction;
                    sqlParameter.Value = value != null ? value : DBNull.Value;
                    if (parameter.Size > 0)
                    {
                        sqlParameter.Size = parameter.Size;
                    }
                    command.Parameters.Add(sqlParameter);
                    _context.Log.AddLogInformation("SqlParameter " + sqlParameter.ParameterName + " добавлен");
                }
            }

            if (!String.IsNullOrEmpty(ResultName))
            {
                _context.Log.AddLogInformation("Ожидается RETURN_VALUE");
                SqlParameter ret_val = new SqlParameter("RETURN_VALUE", ResultType);
                ret_val.Direction = ParameterDirection.ReturnValue;
                if (ResultSize > 0)
                {
                    ret_val.Size = ResultSize;
                }
                command.Parameters.Add(ret_val);
                _context.Log.AddLogInformation("RETURN_VALUE добавлен в коллекцию параметров");
            }        
        }

        private void SaveReturnValueIfPresent(SqlCommand command)
        {
            if (!String.IsNullOrEmpty(ResultName))
            {
                object returnValue = null;
                if (command.Parameters.Contains("RETURN_VALUE"))
                {
                    _context.Log.AddLogInformation("RETURN_VALUE получен");
                    returnValue = command.Parameters["RETURN_VALUE"].Value;
                }
                _context.SaveResult(ResultName, returnValue);
            }
        }

        private List<string> SplitCommandToBatches(string commandText)
        {
            _context.Log.AddLogInformation("Начинаем разбивку текста команды на батчи");
            List<string> commandTexts = new List<string>();
            string tempCommandText = "";
            string[] textLines = commandText.Split(new string[] { "\n" }, StringSplitOptions.None);
            _context.Log.AddLogInformation("Обнаружено " + textLines.Count() + " строк");
            foreach (string line in textLines)
            {
                string trimmed = line.Trim().ToUpper() + " ";
                if (trimmed.StartsWith("GO") && (trimmed[2] == ' ' || trimmed[2] == '-'))
                {
                    if (tempCommandText.Trim().Length != 0)
                    {
                        commandTexts.Add(tempCommandText);
                    }
                    tempCommandText = "";
                    _context.Log.AddLogInformation("Обнаружен новый батч");
                }
                else
                {
                    tempCommandText += line + "\n";
                }
            }
            if (tempCommandText != "")
            {
                commandTexts.Add(tempCommandText);
                tempCommandText = "";
            }
            _context.Log.AddLogInformation("Всего в команде содержится " + commandTexts.Count + " батчей");
            return commandTexts;
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
