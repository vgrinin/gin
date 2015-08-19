using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Gin.Attributes;
using Gin.Context;


namespace Gin.Commands
{
        [GinName(Name = "Форматировать дату/время", Description = "Форматирует дату/время", Group = "Данные")]

    public class FormatDateTime: Command
    {

        #region Аргументы команды

        [GinArgumentDateTime(Name = "Значение даты", Description = "Значение даты")]
        public DateTime DateTimeValue { get; set; }

        [GinArgumentCheckBox(Name = "Текущая дата?", Description = "Использовать ли для форматирования текущую дату")]
        public bool UseCurrent { get; set; }

        [GinArgumentText(Multiline = false, Name = "Строка формата", Description = "Строка формата даты")]
        public string FormatString { get; set; }

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранена отформатированная дата")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Отформатированная дата")]
        public string ResultName { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            DateTime dateTime = UseCurrent ? DateTime.Now : DateTimeValue;
            context.SaveResult(ResultName, dateTime.ToString(FormatString));
            return CommandResult.Next;
        }
    }
}
