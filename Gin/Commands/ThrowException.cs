using System;
using Gin.Attributes;
using Gin.Context;

namespace Gin.Commands
{
    [GinName(Name = "Выбросить исключение", Description = "Генерирует исключение", Group = "Управление")]
    public class ThrowException : Command
    {

        #region Аргументы команды

        [GinArgumentText(Multiline = true, Name = "Сообщение", Description = "Сообщение об ошибке, описывающее исключение.")]
        public string Message { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            throw new Exception(Message);
        }
    }
}
