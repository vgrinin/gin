using System;
using Gin.Attributes;
using Gin.Context;
using Microsoft.Win32;


namespace Gin.Commands
{

    [GinName(Name = "Зарегистрировать тип файла", Description = "Зарегистрировать тип файла в проводнике", Group = "Системные")]
    internal class CMRegisterFileType : Command
    {

        #region Аргументы команды

        [GinArgumentText(Name = "Расширение", Description = "Расширение файла")]
        public string FileExt { get; set; }

        [GinArgumentText(Name = "Тип файла", Description = "Тип файла")]
        public string FileType { get; set; }

        [GinArgumentText(Name = "Описание типа файла", Description = "Описание типа файла")]
        public string TypeDescription { get; set; }

        [GinArgumentBrowseFile(Name = "Программа", Description = "Путь к исполняемой программе")]
        public string ProgramPath { get; set; }


        #endregion


        public override CommandResult Do(ExecutionContext context)
        {
            if (String.IsNullOrEmpty(FileExt) || String.IsNullOrEmpty(ProgramPath))
            {
                return CommandResult.Next;
            }

            if (!FileExt.StartsWith("."))
            {
                FileExt = "." + FileExt;
            }
            string denormFileExt = FileExt.Substring(1);
            if (String.IsNullOrEmpty(FileType))
            {
                FileType = "ft" + denormFileExt;
            }
            if (String.IsNullOrEmpty(TypeDescription))
            {
                TypeDescription = denormFileExt.ToUpper() + " file";
            }
            if (Registry.ClassesRoot.OpenSubKey(FileExt) == null)
            {
                RegistryKey key = Registry.ClassesRoot.CreateSubKey(FileExt);
                if(key==null)
                {
                    throw new InvalidOperationException("Не могу создать ключ реестра");
                }
                key.SetValue(null, FileType);
            }
            if (Registry.ClassesRoot.OpenSubKey(FileType) == null)
            {
                RegistryKey key = Registry.ClassesRoot.CreateSubKey(FileType);
                if (key == null)
                {
                    throw new InvalidOperationException("Не могу создать ключ реестра");
                }
                key.SetValue(null, TypeDescription);
            }
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts");
            if (regKey == null)
            {
                throw new InvalidOperationException("Не могу найти ключ реестра");
            }
            if (regKey.OpenSubKey(FileExt.ToUpper()) == null)
            {
                RegistryKey key = regKey.CreateSubKey(FileExt.ToUpper());
                if (key == null)
                {
                    throw new InvalidOperationException("Не могу создать ключ реестра");
                }
                key.SetValue("DefaultIcon", ProgramPath + ",0");
                RegistryKey shell = key.CreateSubKey("Shell");
                if (shell == null)
                {
                    throw new InvalidOperationException("Не могу создать ключ реестра");
                }
                shell.SetValue(null, "Open");
                RegistryKey open = shell.CreateSubKey("Open");
                shell.SetValue(null, "Default action");
                if (open == null)
                {
                    throw new InvalidOperationException("Не могу создать ключ реестра");
                }
                shell.SetValue(null, ProgramPath + " %1");
            }

            return CommandResult.Next;
        }
    }
}
