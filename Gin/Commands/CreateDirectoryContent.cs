using System.IO;
using Gin.Attributes;
using Gin.Context;
using Gin.Transactions;
using System.Xml.Serialization;
using Gin.Util;


namespace Gin.Commands
{
    
    [GinName(Name = "Создать папку с содержимым", Description = "Создает точную копию папки, заданной на этапе построения пакета", Group = "Файловые операции")]
    public class CreateDirectoryContent : TransactionalCommand, IContentCommand, ICanCreateFromFile
    {

        #region Аргументы команды

        /// <summary>
        /// Путь к исходной папке на компьютере создателя пакета. Все содержимое папки будет помещено в пакет в виде контента и станет доступным конечному потребителю пакета.
        /// </summary>        
        [GinArgumentBrowseFolder(AllowTemplates = true, Name = "Исходная папка", Description = "Полный путь к исходной папке")]
        public string SourcePath { get; set; }

        /// <summary>
        /// Путь к целевой папке на компьютере потребителя пакета. (Можно использовать шаблоны. Если папка должна существовать только в контексте выполнения пакета, а после его окончания стать недоступной потребителю обновления, то следует в качестве папки верхнего уровня использовать шаблон %PACKAGE_CONTENT%)
        /// </summary>
        [GinArgumentBrowseFolder(AllowTemplates = true, Name = "Папка назначения", Description = "Полный путь к целевой папке")]
        public string DestPath { get; set; }

        [GinArgumentCheckBox(AllowTemplates = true, Name = "Очистить, если есть? ", Description = "Нужно ли очистить папку назначения, если такая уже существует")]
        public object ClearFirst { get; set; }

        /// <summary>
        /// Булев аргумент, указывающий, что данная исходная папка SourcePath, будет не скопирована, а перемещена из временной папки пакета в целевое назначение DestPath, что сделает ее недоступной при при повторном использовании команды CreateDirectoryContent с тем же значением SourcePath. Данный параметр следует устанавливать в true для массивных папок, использование которых ограничено одним разом - это ускорит загрузку таких папок при выполнении пакета.
        /// </summary>
        [GinArgumentCheckBox(AllowTemplates = true, Name = "Быстрое извлечение", Description = "Позволяет сделать быстрый перенос папки. После выполнения команды повторный перенос становится невозможен.")]
        public object MoveFast { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            string absoluteDestPath = context.GetStringFrom(DestPath);
            string absoluteSourcePath = context.GetStringFrom(SourcePath);
            bool asboluteMoveFast = context.GetBoolFrom(MoveFast);
            bool asboluteClearFirst = context.GetBoolFrom(ClearFirst);
            context.Log.AddLogInformation("Начинаем создавать папку " + absoluteDestPath);
            if (asboluteClearFirst && Directory.Exists(absoluteDestPath))
            {
                Directory.Delete(absoluteDestPath, true);
            }
            if (!Directory.Exists(absoluteDestPath))
            {
                Directory.CreateDirectory(absoluteDestPath);
            }
            IOUtil.CopyDirectory(absoluteSourcePath, absoluteDestPath, asboluteMoveFast);
            context.Log.AddLogInformation("Папка " + absoluteDestPath + " создана");
            return CommandResult.Next;
        }

        public override CommandResult Do(IExecutionContext context, Transaction transaction)
        {
            if (transaction != null)
            {
                string absoluteDestPath = context.GetStringFrom(DestPath);
                SingleDirectoryStep step = transaction.CreateStep<SingleDirectoryStep>(this);
                step.Init(context, absoluteDestPath);
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

        [XmlIgnore]
        public string ContentPath
        {
            get
            {
                return SourcePath;
            }
            set
            {
                SourcePath = value;
            }
        }

        #region ICanCreateFromFile Members

        public bool IsAssumedCommand(string fileName)
        {
            return Directory.Exists(fileName);
        }

        public void InitFromFile(string fileName)
        {
            SourcePath = fileName;
        }

        #endregion
    }
}
