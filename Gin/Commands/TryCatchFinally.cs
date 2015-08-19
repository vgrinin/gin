using System;
using System.Collections.Generic;
using Gin.Attributes;
using Gin.Context;
using Gin.Transactions;
using Gin.Controls;


namespace Gin.Commands
{
    [GinName(Name = "Try Catch Finally", Description = "Выполняет команду в блоке try-catch-finally", Group = "Управление")]
    public class TryCatchFinally : TransactionalCommand, IContainerCommand
    {

        #region Аргументы команды

        [GinArgumentCommand(IsEnumerable = false, Name = "Блок Try", Description = "Команда, выполняемая в блоке Try. Если в процессе ее выполнения произойдет ошибка, управление будет передано в команду Catch. Команда Finally будет выполнена в любом случае в самом конце, либо после блока Try, либо после блока Catch, если он сработает.")]
        [GinArgumentCommandAcceptNot(NotAcceptedType = typeof(UserInputControl))]
        public Command Try { get; set; }

        [GinArgumentCommand(IsEnumerable = false, Name = "Блок Catch", Description = "Команда, выполняемая в блоке Catch. Выполняется только если в блоке Try возникла ошибка.")]
        [GinArgumentCommandAcceptNot(NotAcceptedType = typeof(UserInputControl))]
        public Command Catch { get; set; }

        [GinArgumentCommand(IsEnumerable = false, Name = "Блок Finally", Description = "Команда, выполняемая в блоке Finally. Выполняется в любом случае в самом конце, либо после блока Try, либо после блока Catch, если он сработает.")]
        [GinArgumentCommandAcceptNot(NotAcceptedType = typeof(UserInputControl))]
        public Command Finally { get; set; }

        #endregion


        public override CommandResult Do(IExecutionContext context)
        {
            try
            {
                ExecuteCommandIfExist(Try, context);
            }
            catch
            {
                ExecuteCommandIfExist(Catch, context);
            }
            finally
            {
                ExecuteCommandIfExist(Finally, context);
            }
            return CommandResult.Next;
        }

        private void ExecuteCommandIfExist(Command command, IExecutionContext context)
        {
            if (command != null)
            {
                command.Execute(context);
            }
        }

        public override CommandResult Do(IExecutionContext context, Transaction transaction)
        {
            try
            {
                ExecuteCommandTransactionallyIfExist(Try, context, transaction);
            }
            catch
            {
                ExecuteCommandTransactionallyIfExist(Catch, context, transaction);
            }
            finally
            {
                ExecuteCommandTransactionallyIfExist(Finally, context, transaction);
            }
            return CommandResult.Next;
        }

        private void ExecuteCommandTransactionallyIfExist(Command command, IExecutionContext context, Transaction transaction)
        {
            if (command != null)
            {
                if (command is TransactionalCommand)
                {
                    ((TransactionalCommand)command).Execute(context, transaction);
                }
                else
                {
                    command.Execute(context);
                }
            }
        }

        public override void Rollback(TransactionStep step)
        {
        }

        public override void Commit(TransactionStep step)
        {
        }

        public IEnumerable<Command> InnerCommands
        {
            get
            {
                List<Command> list = new List<Command>();
                if (Try != null)
                {
                    list.Add(Try);
                }
                if (Catch != null)
                {
                    list.Add(Catch);
                }
                if (Finally != null)
                {
                    list.Add(Finally);
                }
                return list;
            }
        }

        public override int ProgressCost
        {
            get
            {
                int tryCost = Try != null ? Try.ProgressCost : 0;
                int catchCost = Catch != null ? Catch.ProgressCost : 0;
                int finallyCost = Finally != null ? Finally.ProgressCost : 0;
                int maxCost = Math.Max(tryCost, Math.Max(catchCost, finallyCost));
                return maxCost;
            }
        }

        public override void Visit(CommandVisitor visitor)
        {
            base.Visit(visitor);
            if (Try != null)
            {
                Try.Visit(visitor);
            }
            if (Catch != null)
            {
                Catch.Visit(visitor);
            }
            if (Finally != null)
            {
                Finally.Visit(visitor);
            }
        }
    }
}
