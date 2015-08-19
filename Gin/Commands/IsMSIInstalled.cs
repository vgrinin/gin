using System;
using Gin.Attributes;
using Gin.Context;
using Microsoft.Win32;

namespace Gin.Commands
{

    [GinName(Name = "Установлен ли MSI пакет", Description = "Проверяет установлен ли в системе требуемый MSI-пакет", Group = "Данные")]
    public class IsMSIInstalled : Command
    {

        [GinArgumentEnum(Name = "Тип поиска", Description = "Указывает тип поиска по реестру установленных MSI-пакетов", ListEnum = typeof(MSISearchType))]
        public MSISearchType SearchType { get; set; }

        [GinArgumentText(Name = "Продукт", Description = "Искомый продукт. Тип значения определяется аргументом <Тип поиска>")]
        public string Product { get; set; }

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранен булев результат проверки")]
        [GinResult(Result = typeof(bool), Kind = CommandResultKind.Primitive, Description = "Установлен ли MSI-пакет")]
        public string ResultName { get; set; }

        public override CommandResult Do(IExecutionContext context)
        {
            bool result = false;
            RegistryKey key;
            switch (SearchType)
            {
                case MSISearchType.ProductCode:
                    key = Registry.ClassesRoot.OpenSubKey(@"Installer\Products\" + Product);
                    result = key != null;
                    break;
                case MSISearchType.ProductName:
                    key = Registry.ClassesRoot.OpenSubKey(@"Installer\Products");
                    string[] subKeys = key.GetSubKeyNames();
                    foreach (string subKey in subKeys)
                    {
                        RegistryKey key2 = key.OpenSubKey(subKey);
                        string keyValue = (string)key2.GetValue("ProductName");
                        if (keyValue == Product)
                        {
                            result = true;
                            break;
                        }
                    }
                    break;
                default:
                    throw new Exception("Добавь тип");
            }

            context.SaveResult(ResultName, result);

            return CommandResult.Next;
        }
    }
}
