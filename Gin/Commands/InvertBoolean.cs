using System;
using Gin.Attributes;
using Gin.Context;


namespace Gin.Commands
{
    [GinName(Name = "Инвертировать булеву", Description = "Вычисляет инверсию булевой переменной", Group = "Данные")]

    public class InvertBoolean : Command
    {

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Name = "Имя аргумента", Description = "Контекстное имя инвертируемой переменной")]
        public string ArgumentName { get; set; }

        [GinArgumentText(Multiline = false, Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранен результат инверсии")]
        [GinResult(Result = typeof(bool), Kind = CommandResultKind.Primitive, Description = "Инверсия булевой переменной")]
        public string ResultName { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {

            string argumentValueString = context.GetStringFrom(ArgumentName);
            bool argumentValue = Boolean.Parse(argumentValueString);
            context.SaveResult(ResultName, !argumentValue);

            return CommandResult.Next;
        }
    }
}
