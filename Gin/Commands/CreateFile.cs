using Gin.Attributes;
using Gin.Context;
using Gin.Transactions;
using System.IO;
using System.Xml.Serialization;


namespace Gin.Commands
{
    [GinName(Name = "Создать файл", Description = "Создает файл, заданный на этапе построения пакета", Group = "Файловые операции")]
    public class CreateFile : TransactionalCommand, IContentCommand, ICanCreateFromFile
    {
        #region Аргументы команды

        [GinArgumentBrowseFile(AllowTemplates = true, Name = "Исходный файл", Description = "Исходный файл")]
        public string SourcePath { get; set; }

        [GinArgumentText(AllowTemplates = true, Name = "Путь к файлу", Description = "Полный путь к создаваемому файлу")]
        public string DestPath { get; set; }

        #endregion 

        public override CommandResult Do(IExecutionContext context)
        {
            string absoluteSourcePath = context.GetStringFrom(SourcePath);
            string absoluteDestPath = context.GetStringFrom(DestPath);
            File.Copy(absoluteSourcePath, absoluteDestPath, true);
            context.Log.AddLogInformation("Файл " + absoluteDestPath + " создан");
            return CommandResult.Next;
        }

        public override CommandResult Do(IExecutionContext context, Transaction transaction)
        {
            if (transaction != null)
            {
                string absoluteDestPath = context.GetStringFrom(DestPath);
                SingleFileStep step = transaction.CreateStep<SingleFileStep>(this);
                step.Init(context, DestPath);
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
            return File.Exists(fileName);
        }

        public void InitFromFile(string fileName)
        {
            SourcePath = fileName;
        }

        #endregion
    }
}
