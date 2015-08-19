using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Attributes;
using Gin.Context;
using Gin.Transactions;
using System.IO;
using System.Xml.Serialization;
using Gin.Util;


namespace Gin.Commands
{
    [GinName(Name = "Создать новую папку", Description = "Создает папку по ее имени", Group = "Файловые операции")]
    public class CreateFolder : TransactionalCommand
    {

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Name = "Путь к папке", Description = "Полный путь к создаваемой папке")]
        public string DestPath { get; set; }

        [GinArgumentCheckBox(Name = "Создать пустую", Description = "Обнуляет содержимое папки, если она уже существует")]
        public bool CreateEmpty { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            DestPath = context.GetStringFrom(DestPath);
            if (CreateEmpty && Directory.Exists(DestPath))
            {
                Directory.Delete(DestPath, true);
            }
            Directory.CreateDirectory(DestPath);
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

    }
}
