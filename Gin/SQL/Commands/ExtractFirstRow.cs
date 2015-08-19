using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml.Serialization;
using Gin.Attributes;
using Gin.Context;


namespace Gin.Commands
{
    [GinName(Name = "Извлечь первую строку", Description = "Извлекает первую строку из таблицы", Group = "Данные")]
    public class ExtractFirstRow : Command
    {

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Name = "Входной аргумент", Description = "Контекстное имя переменной, в которой сохранена таблица БД")]
        public string TableName { get; set; }

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранена первая строка таблицы")]
        [GinResult(Result = typeof(DataRow), Kind = CommandResultKind.Primitive, Description = "Первая строка таблицы")]
        public string ResultName { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            DataTable table = (DataTable)context.GetResult(TableName);
            DataRow row = table.Rows[0];
            context.SaveResult(ResultName, row);

            return CommandResult.Next;
        }
    }
}
