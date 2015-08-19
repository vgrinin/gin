using Gin.Attributes;
using Gin.Context;


namespace Gin.Commands
{
    [GinName(Name = "Сохранить целое число", Description = "Создает константу типа int", Group = "Данные")]
    public class SaveInt : Command
    {
        #region Аргументы команды

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранена входная константа")]
        [GinResult(Result = typeof(int), Kind = CommandResultKind.Primitive, Description = "Целое значение")]
        public string ResultName { get; set; }

        [GinArgumentInt(Name = "Входное значение", Description = "Значение входного аргумента")]
        public int Value { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            context.SaveResult(ResultName, Value);
            return CommandResult.Next;
        }
    }
}
