using System;
using System.Collections.Generic;
using Gin.Attributes;
using Gin.Context;
using Gin.Transactions;
using Gin.Controls;

namespace Gin.Commands
{
    [GinName( Name = "Если", Description = "Условный оператор if-then-else", Group = "Управление")]
    public class ExecuteIf : TransactionalCommand, IContainerCommand
    {

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Name = "Имя аргумента", Description = "Контекстное имя булева входного аргумента команды")]
        public string ArgumentName { get; set; }

        [GinArgumentCommand(IsEnumerable = false, Name = "То", Description = "Блок, выполняемый в случае истинного входного аргумента")]
        [GinArgumentCommandAcceptNot(NotAcceptedType = typeof(UserInputControl))]
        public Command Then { get; set; }

        [GinArgumentCommand(IsEnumerable = false, Name = "Иначе", Description = "Блок, выполняемый в случае ложного входного аргумента")]
        [GinArgumentCommandAcceptNot(NotAcceptedType = typeof(UserInputControl))]
        public Command Else { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            if ((bool)context.GetResult(ArgumentName))
            {
                ExecuteCommandIfExist(Then, context);
            }
            else
            {
                ExecuteCommandIfExist(Else, context);
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
            if ((bool)context.GetResult(ArgumentName))
            {
                ExecuteCommandTransactionallyIfExist(Then, context, transaction);
            }
            else
            {
                ExecuteCommandTransactionallyIfExist(Else, context, transaction);
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
                if (Then != null)
                {
                    list.Add(Then);
                }
                if (Else != null)
                {
                    list.Add(Else);
                }
                return list;
            }
        }

        public override int ProgressCost
        {
            get
            {
                int thenCost = Then != null ? Then.ProgressCost : 0;
                int elseCost = Else != null ? Else.ProgressCost : 0;
                int maxCost = Math.Max(thenCost, elseCost);
                return maxCost;
            }
        }

        public override void Visit(CommandVisitor visitor)
        {
            base.Visit(visitor);
            if (Then != null)
            {
                Then.Visit(visitor);
            }
            if (Else != null)
            {
                Else.Visit(visitor);
            }
        }

    }
}
