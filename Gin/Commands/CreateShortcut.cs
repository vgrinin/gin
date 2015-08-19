using System;
using Gin.Attributes;
using Gin.Context;
using Gin.Transactions;
using IWshRuntimeLibrary;
using System.IO;

namespace Gin.Commands
{

    public enum ShortcutPlace
    {
        [GinName(Name = "Рабочий стол")]
        Desktop,
        [GinName(Name = "Меню быстрого запуска")]
        Startup,
        [GinName(Name = "Меню ПУСК")]
        StartMenu,
        [GinName(Name = "Программы")]
        Programs
    }

        [GinName(Name = "Создать ярлык", Description = "Создает ярлык для целевого файла", Group = "Файловые операции")]
    public class CreateShortcut: TransactionalCommand
    {

        private const string LNK_EXTENSION = ".lnk";

        #region Аргументы команды

        /// <summary>
        /// Файл, на который ссылается ярлык
        /// </summary>       
        [GinArgumentBrowseFile(AllowTemplates = true, Name = "Файл", Description = "Целевой файл")]
        public string FilePath {get;set;}

        /// <summary>
        /// Название ярлыка
        /// </summary>
        [GinArgumentText(AllowTemplates = true, Name = "Название", Description = "Название ярлыка", Multiline = false)]
        public string ShortcutName { get; set; }

        /// <summary>
        /// Куда разместить ярлык
        /// </summary>
        [GinArgumentEnum(Name = "Размещение", Description = "Куда разместить ярлык.", ListEnum = typeof(ShortcutPlace))]
        public ShortcutPlace Place { get; set; }

        /// <summary>
        /// Сделать ли ярлык доступным для всех пользователей. Может быть булевым значением, либо строкой-шаблоном, указывающей откуда из контекста выполнения брать булево значение
        /// </summary>
        [GinArgumentCheckBox(AllowTemplates = true, Name = "Для всех", Description = "Ярлык доступен для всех пользователей?")]
        public object AllUsers { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context, Transaction transaction)
        {
            if (transaction != null)
            {
                SingleFileStep step = transaction.CreateStep<SingleFileStep>(this);
                bool absoluteAllUsers = context.GetBoolFrom(AllUsers);
                string absoluteShortcutName = context.GetStringFrom(ShortcutName);
                string linkPathName = GetLinkPathName(absoluteShortcutName, absoluteAllUsers, Place);
                step.Init(context, linkPathName);
            }
            Do(context);
            return CommandResult.Next;
        }

        public override void Rollback(TransactionStep step)
        {
            step.Rollback();
        }

        public override void Commit(TransactionStep step)
        {
            step.Commit();
        }

        public override CommandResult Do(IExecutionContext context)
        {
            string absoluteFilePath = context.GetStringFrom(FilePath);

            bool absoluteAllUsers = context.GetBoolFrom(AllUsers);
            string absoluteShortcutName = context.GetStringFrom(ShortcutName);
            string linkPathName = GetLinkPathName(absoluteShortcutName, absoluteAllUsers, Place);

            WshShell shell = new WshShell();
            IWshShortcut link = (IWshShortcut)shell.CreateShortcut(linkPathName);
            link.TargetPath = absoluteFilePath;
            link.Save();

            return CommandResult.Next;
        }

        private string GetLinkPathName(string absoluteShortcutName, bool absoluteAllUsers, ShortcutPlace place)
        {

            if (!absoluteShortcutName.EndsWith(LNK_EXTENSION))
            {
                absoluteShortcutName += LNK_EXTENSION;
            }

            Environment.SpecialFolder folderType = Environment.SpecialFolder.DesktopDirectory;
            switch (place)
            {
                case ShortcutPlace.Desktop:
                    //folderType = absoluteAllUsers ? Environment.SpecialFolder.CommonDesktopDirectory : Environment.SpecialFolder.DesktopDirectory;
                    folderType = Environment.SpecialFolder.DesktopDirectory;
                    break;
                case ShortcutPlace.Startup:
                    //folderType = absoluteAllUsers ? Environment.SpecialFolder.CommonStartup : Environment.SpecialFolder.Startup;
                    folderType = Environment.SpecialFolder.Startup;
                    break;
                case ShortcutPlace.StartMenu:
                    //folderType = absoluteAllUsers ? Environment.SpecialFolder.CommonStartMenu : Environment.SpecialFolder.StartMenu;
                    folderType = Environment.SpecialFolder.StartMenu;
                    break;
                case ShortcutPlace.Programs:
                    //folderType = absoluteAllUsers ? Environment.SpecialFolder.CommonPrograms : Environment.SpecialFolder.Programs;
                    folderType = Environment.SpecialFolder.Programs;
                    break;
            }

            string lnkPath = Environment.GetFolderPath(folderType);
            string linkPathName = Path.Combine(lnkPath, absoluteShortcutName);

            return linkPathName;
        }
    }
}
