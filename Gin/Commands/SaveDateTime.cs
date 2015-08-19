using System;
using Gin.Attributes;
using Gin.Context;


namespace Gin.Commands
{
    [GinName(Name = "Сохранить дату/время", Description = "Создает константу типа DateTime", Group = "Данные")]
    public class SaveDateTime : Command
    {
        #region Аргументы команды

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранена входная константа")]
        [GinResult(Result = typeof(DateTime), Kind = CommandResultKind.Primitive, Description = "Дата")]
        public string ResultName { get; set; }

        [GinArgumentDateTime(Name = "Входное значение", Description = "Значение входного аргумента")]
        public DateTime Value { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            context.SaveResult(ResultName, Value);
            return CommandResult.Next;
        }
    }
}
