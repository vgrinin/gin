using Gin.Attributes;
using Gin.Context;


namespace Gin.Commands
{
    [GinName(Name = "Сохранить булеву переменную", Description = "Создает константу типа bool", Group = "Данные")]
    public class SaveBool : Command
    {
        #region Аргументы команды

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранена входная константа")]
        [GinResult(Result  = typeof(bool), Kind = CommandResultKind.Primitive, Description = "true/false")]
        public string ResultName { get; set; }

        [GinArgumentCheckBox(Name = "Входное значение", Description = "Значение входного аргумента")]
        public bool Value { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            context.SaveResult(ResultName, Value);
            return CommandResult.Next;
        }
    }
}
