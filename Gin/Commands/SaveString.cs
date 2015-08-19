using System.IO;
using Gin.Attributes;
using Gin.Context;


namespace Gin.Commands
{
    [GinName(Name = "Сохранить строку", Description = "Создает константную строку или строку согласно шаблона", Group = "Данные")]
    public class SaveString : Command
    {

        #region Аргументы команды

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранен результат выполнения команды.")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "константная строка")]
        public string ResultName { get; set; }

        [GinArgumentText(AllowTemplates = true, Multiline = true, Name = "Входное значение", Description = "Значение входной строки. Можно использовать шаблоны.")]
        public string Value { get; set; }

        [GinArgumentCheckBox(Name = "Файловый путь?", Description = "Является ли входной аргумент путем к файлу или папке? Позволяет избежать дублирования обратных слэшей при конкатенации строк, если часть результирующего пути введена пользователем и заранее неизвестно, содержит ли она закрывающий слэш.")]
        public bool IsPathCombine { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            string absoluteValue;
            if (IsPathCombine)
            {
                absoluteValue = "";
                string[] parts = Value.Split('\\');
                foreach (string part in parts)
                {
                    string absolutePart = context.GetStringFrom(part).Trim();
                    if (absolutePart.EndsWith(":"))
                    {
                        absolutePart += @"\";
                    }
                    absoluteValue = Path.Combine(absoluteValue, absolutePart);
                }
            }
            else
            {
                absoluteValue = context.GetStringFrom(Value);
            }
            context.SaveResult(ResultName, absoluteValue);
            return CommandResult.Next;
        }
    }
}