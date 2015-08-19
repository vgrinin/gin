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
    [GinName(Name = "Извлечь поле строки", Description = "Извлекает поле из строки данных", Group = "Данные")]
    public class ExtractDataRowField : Command
    {

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Name = "Входной аргумент", Description = "Контекстное имя переменной, в которой сохранена строка данных из БД")]
        public string DataRowName { get; set; }

        [GinArgumentText(Name = "Имя поля", Description = "Имя поля в строке данных")]
        public string FieldName { get; set; }

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранено значение конкретного поля заданной строки данных")]
        [GinResult(Result = typeof(object), Kind = CommandResultKind.Primitive, Description = "Значение поля")]
        public string ResultName { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            DataRow row = (DataRow)context.GetResult(DataRowName);
            context.SaveResult(ResultName, row[FieldName]);

            return CommandResult.Next;
        }
    }
}
