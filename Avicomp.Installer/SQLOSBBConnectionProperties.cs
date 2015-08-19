using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin;
using Gin.Attributes;

namespace Avicomp.Installer
{
    [GinIncludeType()]
    public class SQLOSBBConnectionProperties
    {
        public const string CONNECTION_STRING_SQL = @"Data Source={server_name};Persist Security Info=True;User ID={user_name};Initial Catalog={db_name};Password={user_password}";

        public const string CONNECTION_STRING_WINDOWS = @"Data Source={server_name};Initial Catalog={db_name};Integrated Security=SSPI";

        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Строка подключения")]
        public string ConnectionString
        {
            get
            {
                string result = SqlAuthentication ? CONNECTION_STRING_SQL : CONNECTION_STRING_WINDOWS;
                result = result.Replace("{server_name}", InstanceName).Replace("{db_name}", DBName);
                if (SqlAuthentication)
                {
                    result = result.Replace("{user_name}", UserName).Replace("{user_password}", Password);
                }
                return result;
            }
        }

        [GinArgumentText(Name = "Имя сервера", Description = "Имя сервера")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Имя сервера")]
        public string InstanceName { get; set; }

        [GinArgumentText(Name = "Имя базы", Description = "Имя базы")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Имя базы")]
        public string DBName { get; set; }

        [GinArgumentCheckBox(Name = "Авторизация SQL-Server", Description = "Авторизация SQL-Server")]
        [GinResult(Result = typeof(bool), Kind = CommandResultKind.Primitive, Description = "Авторизация SQL-Server")]
        public bool SqlAuthentication { get; set; }

        [GinArgumentText(Name = "Имя пользователя", Description = "Имя пользователя")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Имя пользователя")]
        public string UserName { get; set; }

        [GinArgumentText(Name = "Пароль", Description = "Пароль")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Пароль")]
        public string Password { get; set; }


        [GinArgumentCheckBox(AllowTemplates = true, Name = "Создать резервную копию", Description = "Создать резервную копию")]
        [GinResult(Result = typeof(bool), Kind = CommandResultKind.Primitive, Description = "Создать резервную копию")]
        public bool CreateBackup { get; set; }

        [GinArgumentText(AllowTemplates = true, Name = "Путь к резервной копии", Description = "Полный путь к файлу резервной копии")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Путь к резервной копии")]
        public string BackupFilePath { get; set; }

        public override string ToString()
        {
            return ConnectionString;
        }
    }

}
