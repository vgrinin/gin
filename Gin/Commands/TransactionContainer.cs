using System;
using System.Collections.Generic;
using Gin.Attributes;
using Gin.Context;
using Gin.Transactions;
using Gin.Controls;


namespace Gin.Commands
{
    [GinName(Name = "Транзакция", Description = "Выполняет несколько команд как одну в рамках транзакции", Group = "Управление")]
    public class TransactionContainer : Command, IContainerCommand
    {

        #region Аргументы команды

        [GinArgumentText(MaxLength = 500, Name = "Имя транзакции", Description = "Имя транзакции - строка, обязательный параметр. Позволяет сохранять шаги транзакции на диске и обращаться к транзакции после ее выполнения.")]
        public string TransactionName { get; set; }

        [GinArgumentCommand(IsEnumerable = false, Name = "Тело транзакции", Description = "Команды, выполняемые в рамках транзакции")]
        [GinArgumentCommandAcceptNot(NotAcceptedType = typeof(UserInputControl))]
        public TransactionalCommand Command { get; set; }

        #endregion

        public TransactionContainer()
        {
            TransactionName = Guid.NewGuid().ToString("N");
        }

        public override CommandResult Do(IExecutionContext context)
        {
            Transaction transaction = new Transaction
            {
                TransactionName = TransactionName,
                TransactionState = TransactionState.Undefined
            };

            try
            {
                context.ExecutedPackage.AddTransaction(transaction);
                Command.Do(context, transaction);
                SetProgress(context, Command, CommandResult.Next);
                transaction.TransactionState = TransactionState.Active;
            }
            catch
            {
                transaction.Rollback();
            }
            finally
            {
                transaction.Save(context);
            }
            return CommandResult.Next;
        }

        public IEnumerable<Command> InnerCommands
        {
            get
            {
                return new List<Command>
                {
                    Command
                };
            }
        }

        public override int ProgressCost
        {
            get
            {
                return Command != null ? Command.ProgressCost : 0;
            }
        }

        public override void Visit(CommandVisitor visitor)
        {
            base.Visit(visitor);
            if (Command != null)
            {
                Command.Visit(visitor);
            }
        }

    }
}
