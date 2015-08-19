using Gin.Attributes;
using Gin.Context;
using Microsoft.Win32;  

namespace Gin.Commands
{

    [GinName(Name = "Писать реестр", Description = "Пишет ключ реестра", Group = "Данные")]
    public class WriteRegistry : Command
    {

        #region Аргументы команды

        [GinArgumentText(Name = "Ключ реестра", Description = "Полный путь к ключу реестра")]
        public string KeyFullPath { get; set; }

        [GinArgumentText(Name = "Значение", Description = "Контекстное имя переменной, записанной в реестр.")]
        public string ValueName { get; set; }


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
            key.SetValue(stringKey, context.GetResult(ValueName));
            return CommandResult.Next;
        }
    }
}
