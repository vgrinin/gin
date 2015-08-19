using Gin.Attributes;
using Gin.Context;
using Microsoft.Win32;


namespace Gin.Commands
{
        [GinName(Name = "Установлен ли COM объект", Description = "Узнает установлен ли в системе требуемый COM-объект", Group = "Данные")]
    public class IsCOMInstalled: Command
    {

        #region Аргументы команды

        [GinArgumentText(Name = "CLASSID компонента", Description = "CLASSID искомого COM-объекта")]
        public string ClassId { get; set; }

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранен булев результат проверки")]
        [GinResult(Result = typeof(bool), Kind = CommandResultKind.Primitive, Description = "Установлен ли COM-объект")]
        public string ResultName { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {

            RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"CLSID\" + ClassId);
            context.SaveResult(ResultName, key != null);

            return CommandResult.Next;
        }
    }
}
