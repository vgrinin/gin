using System;
using Gin.Attributes;
using Gin.Context;


namespace Gin.Commands
{
    [GinName(Name = "Читать переменную окружения", Description = "Читает environment-переменную", Group = "Данные")]
    public class ReadEnvironment : Command
    {

        #region Аргументы команды

        [GinArgumentText(Name = "Имя переменной", Description = "Имя читаемой переменной окружения")]
        public string VariableName { get; set; }

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранена переменная окружения")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Значение Environment-переменной")]
        public string ResultName { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            string result = Environment.GetEnvironmentVariable(VariableName);
            context.SaveResult(ResultName, result);
            return CommandResult.Next;
        }
    }
}
