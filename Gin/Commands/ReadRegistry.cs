using Gin.Attributes;
using Gin.Context;
using Microsoft.Win32;  

namespace Gin.Commands
{

    public enum RegistryValueType
    {
        String,
        Binary,
        DWORD,
        MultiString,
        ExpandableString
    }

    [GinName(Name = "Читать реестр", Description = "Читает ключ реестра", Group = "Данные")]
    public class ReadRegistry : Command
    {

        #region Аргументы команды

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранен результат чтения ключа реестра.")]
        [GinResult(Result = typeof(object), Kind = CommandResultKind.Primitive, Description = "Значение ключа реестра")]
        public string ResultName { get; set; }

        [GinArgumentText(Name = "Ключ реестра", Description = "Полный путь к ключу реестра")]
        public string KeyFullPath { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            string stringBaseKey = KeyFullPath.Split('\\')[0];
            string stringKey = KeyFullPath.Substring(KeyFullPath.LastIndexOf('\\') + 1);
            string stringRelativePath = KeyFullPath.Substring(KeyFullPath.IndexOf('\\') + 1);
            stringRelativePath = stringRelativePath.Remove(stringRelativePath.LastIndexOf('\\'));
            RegistryKey baseKey;
            switch (stringBaseKey)
            {
                case "HKEY_CLASSES_ROOT":
                    baseKey = Registry.ClassesRoot;
                    break;
                case "HKEY_CURRENT_USER":
                    baseKey = Registry.CurrentUser;
                    break;
                case "HKEY_LOCAL_MACHINE":
                    baseKey = Registry.LocalMachine;
                    break;
                case "HKEY_USERS":
                    baseKey = Registry.Users;
                    break;
                case "HKEY_CURRENT_CONFIG":
                    baseKey = Registry.CurrentConfig;
                    break;
                default:
                    baseKey = Registry.LocalMachine;
                    break;
            }
            RegistryKey key = baseKey.OpenSubKey(stringRelativePath);
            context.SaveResult(ResultName, key.GetValue(stringKey));
            return CommandResult.Next;
        }
    }
}
