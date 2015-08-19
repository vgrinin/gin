using System.Collections.Generic;
using System.Linq;
using Gin.Attributes;
using Gin.Context;
using Gin.Transactions;
using Gin.Controls;
using Gin.Logging;

namespace Gin.Commands
{

    public delegate CommandResult CommandIterationHandler(Command command);

    [GinName(Name = "Последовательность команд", Description = "Выполняет несколько команд как одну", Group = "Управление")]
    public class CommandSequence : TransactionalCommand, IContainerCommand
    {
        #region Аргументы команды

        [GinArgumentCommand(IsEnumerable = true, Name = "Команды", Description = "Последовательность выполняемых команд")]
        [GinArgumentCommandAcceptNot(NotAcceptedType = typeof(UserInputControl))]
        public List<Command> Commands { get; set; }

        #endregion

        public CommandSequence()
        {
            Commands = new List<Command>();
        }

        public override CommandResult Do(IExecutionContext context)
        {
            AdjustReversibleUI();
            IterateCommands(command => 
            {
                QueryCancelEventArgs args = new QueryCancelEventArgs(); 
                context.Log.GetPendingCancel(args);
                if (args.Cancel)
                {
                    throw new PackageExecutionCancelledException();
                }
                CommandResult result = command.Execute(context);
                if (!(command is IContainerCommand))
                {
                    SetProgress(context, command, result);
                }
                return result;
            });

            return CommandResult.Next;
        }

        private void AdjustReversibleUI()
        {
            bool isFirst = true;
            IReversibleCommand lastCommand = null;
            foreach (Command command in Commands)
            {
                var reversibleCommand = command as IReversibleCommand;
                if (reversibleCommand != null)
                {
                    reversibleCommand.IsFirst = isFirst;
                    isFirst = false;
                    reversibleCommand.IsLast = false;
                    lastCommand = reversibleCommand;
                }
            }
            if (lastCommand != null)
            {
                lastCommand.IsLast = true;
            }
        }

        private void IterateCommands(CommandIterationHandler iteration)
        {
            if (Commands.Count > 0)
            {
                int current = 0;
                while (current < Commands.Count)
                {
                    Command command = Commands[current];
                    CommandResult result = iteration(command);
                    switch (result)
                    {
                        default:
                            current++;
                            break;
                        case CommandResult.Previous:
                            current--;
                            break;
                    }
                }
            }
        }

        public override CommandResult Do(IExecutionContext context, Transaction transaction)
        {
            AdjustReversibleUI();
            IterateCommands(command =>
            {
                CommandResult result;
                if (command is TransactionalCommand)
                {
                    result = ((TransactionalCommand)command).Execute(context, transaction);
                }
                else
                {
                    result = command.Execute(context);
                } 
                SetProgress(context, command, result);
                return result;
            });

            return CommandResult.Next;
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
                return Commands;
            }
        }

        public override int ProgressCost
        {
            get
            {
                return Commands.Sum(f => f.ProgressCost);
            }
        }

        public override void Visit(CommandVisitor visitor)
        {
            base.Visit(visitor);
            if (Commands != null)
            {
                foreach (var command in Commands)
                {
                    command.Visit(visitor);
                }
            }
        }
    }
}
