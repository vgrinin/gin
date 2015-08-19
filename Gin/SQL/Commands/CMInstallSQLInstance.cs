using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gin.Attributes;
using Gin.Commands;
using System.Xml.Serialization;
using Gin.Context;

namespace Gin.SQL.Commands
{
    [GinName(Name = "Установить экземпляр SQL", Description = "Инсталлирует экземпляр SQL-сервера", Group = "SQL")]
    public class CMInstallSQLInstance : Command, IContentCommand
    {

        #region Аргументы команды 

        [GinArgumentBrowseFile(Name = "Файл SQL-инсталлятора", Description = "Путь к файлу инсталлятора SQL Server")]
        public string SetupFilePath { get; set; }

        [GinArgumentText(AllowTemplates = true, Name = "Имя экземпляра", Description = "Имя создаваемого экземпляра SQL сервера. Можно использовать шаблоны.")]
        public string InstanceName { get; set; }

        [GinArgumentText(AllowTemplates = true, Name = "Пароль", Description = "Пароль пользователя sa. Можно использовать шаблоны.")]
        public string SAPassword { get; set; }

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранен ExitCode инсталлятора SQL Server")]
        [GinResult(Result = typeof(int), Kind = CommandResultKind.Primitive, Description = "ExitCode инсталляции SQL-экземпляра")]
        public string ResultName { get; set; }

        #endregion

        private const string ARGUMENTS_EXE_STRING = @"INSTANCENAME={INSTANCE} ADDLOCAL=SQL_Engine SAPWD={PWD} SQLACCOUNT={USER} AGTACCOUNT={USER} SQLBROWSERACCOUNT={USER}";

        private const string DEFAULT_INSTANCE_ACCOUNT = @"NT AUTHORITY\SYSTEM";

        public override CommandResult Do(IExecutionContext context)
        {

            string absoluteInstanceName = context.GetStringFrom(InstanceName);
            string absoluteSAPassword = context.GetStringFrom(SAPassword);

            string cmdParameters = ARGUMENTS_EXE_STRING.
                Replace("{INSTANCE}", absoluteInstanceName).
                Replace("{PWD}", absoluteSAPassword).
                Replace("{USER}", "\"" + DEFAULT_INSTANCE_ACCOUNT + "\"");

            string intResultName = Guid.NewGuid().ToString("N");
            ExecuteProgram exec = new ExecuteProgram()
            {
                Arguments = "/qb " + cmdParameters,
                ProgramExePath = SetupFilePath,
                WindowType = Gin.Util.ProgramWindowType.WinForms,
                IntResultName = intResultName
            };
            exec.Do(context);
            int intResult = (int)context.GetResult(intResultName);

            context.SaveResult(ResultName, intResult);

            return CommandResult.Next;
        }

        public string ContentPath
        {
            get
            {
                return SetupFilePath;
            }
            set
            {
                SetupFilePath = value;
            }
        }
    }
}
