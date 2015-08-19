using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Gin.Commands
{
 /*   [GinName(Name = "Путь к спец.папке", Description = "Читает путь к специальной папке", Group = "Данные")]
    public class GetSpecialFolder : Command
    {

        #region Аргументы команды

        [GinArgumentEnum(ListEnum = typeof(Environment.SpecialFolder), Name = "Имя папки", Description = "Имя папки читаемой из переменной окружения")]
        public Environment.SpecialFolder Folder { get; set; }

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранена переменная окружения")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Значение Environment-переменной")]
        public string ResultName { get; set; }

        #endregion

        public override CommandResult Do(ExecutionContext context)
        {
            string result = Environment.GetFolderPath(Folder);
            context.SaveResult(ResultName, result);
            return CommandResult.Next;
        }
    }*/
}
