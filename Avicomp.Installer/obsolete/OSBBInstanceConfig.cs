using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin;
using Microsoft.Win32;
using System.Xml.Serialization;
using System.IO;

namespace Avicomp.Installer
{

    [GinIncludeType]
    public class OSBBInstanceConfig
    {
        private const string CONNECTION_STRING_SQL = @"Data Source={server_name};Persist Security Info=True;User ID={user_name};Initial Catalog={db_name};Password={user_password}";
        private const string OSBB_REGISTRY_ROOT = @"SOFTWARE\AviComp Services\OSBB\OSBBInstances\";

        private string _alias;

        [GinArgumentText(Name = "Имя конфигурации", Description = @"Имя конфигурации из реестра HKLM\SOFTWARE\AviComp Services\OSBB\OSBBInstances")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Имя конфигурации")]
        public string Alias
        {
            get 
            {
                return _alias;
            }
            set
            {
                RegistryKey baseKey = Registry.LocalMachine;
                RegistryKey instanceKey = baseKey.OpenSubKey(OSBB_REGISTRY_ROOT + value);
                if (instanceKey != null)
                {
                    BackupDirectory = (string)instanceKey.GetValue("BackupDirectory");
                    DBName = (string)instanceKey.GetValue("DBName");
                    InstanceName = (string)instanceKey.GetValue("InstanceName");
                    Password = (string)instanceKey.GetValue("Password");
                    ProgramDirectory = (string)instanceKey.GetValue("ProgramDirectory");
                    ServerName = (string)instanceKey.GetValue("ServerName");
                    UserName = (string)instanceKey.GetValue("UserName");
                    _alias = value;
                }
            }
        }
        [XmlIgnore]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Папка для бэкапов")]
        public string BackupDirectory { get; private set; }
        [XmlIgnore]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Имя базы")]
        public string DBName { get; private set; }
        [XmlIgnore]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Имя экземпляра")]
        public string InstanceName { get; private set; }
        [XmlIgnore]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Пароль")]
        public string Password { get; set; }
        [XmlIgnore]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Папка программы")]
        public string ProgramDirectory { get; private set; }
        [XmlIgnore]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Имя сервера")]
        public string ServerName { get; private set; }
        [XmlIgnore]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Имя пользователя")]
        public string UserName { get; set; }
        [XmlIgnore]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Строка подключения")]
        public string ConnectionString
        {
            get
            {
                string result = CONNECTION_STRING_SQL;
                result = result.Replace("{server_name}", ServerName).Replace("{db_name}", DBName);
                result = result.Replace("{user_name}", UserName).Replace("{user_password}", Password);
                return result;
            }
        }

        [XmlIgnore]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Полный путь к файлу бэкапа")]
        public string BackupFilePath
        {
            get
            {
                string dateTimeFormatted = DateTime.Now.ToString("ddMMyyyy");
                string fileName = InstanceName + "_" + dateTimeFormatted + "_1.bak";
                string result = Path.Combine(BackupDirectory, fileName);
                return result;
            }
        }

        [GinArgumentCheckBox(Name = "Создать резервную копию", Description = "Нужно ли создавать резервную копию")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "")]
        public bool CreateBackup { get; set; }

        public override string ToString()
        {
            return Alias;
        }
    }
}
