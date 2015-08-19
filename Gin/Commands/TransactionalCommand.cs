using Gin.Context;
using Gin.Transactions;

namespace Gin.Commands
{
    /// <summary>
    /// Команда, поддерживающая транзакции
    /// </summary>
    public abstract class TransactionalCommand : Command
    {
        /// <summary>
        /// Тело команды, здесь пишем исполняющийся командой код. Здесь же пишем логику сохранения состояния 
        /// до выполнения команды. Это состояние создается при помощи метода CreateStep экземпляра класса Transaction,
        /// и после настройки дополнительных параметров экземпляра подкласса TransactionStep возвращается в качестве 
        /// результата метода Do. Основная логика команды (не поддерживающая транзакций), должна быть описана 
        /// в методе Do базового класса Command, и метод этот вызывается в конце метода Do класса TransactionalCommand
        /// </summary>
        /// <param name="context">Контекст выполнения</param>
        /// <param name="transaction">Родительская транзакция</param>
        /// <returns></returns>
        public abstract CommandResult Do(IExecutionContext context, Transaction transaction);
        
        /// <summary>
        /// Метод, откатывающий транзакцию.
        /// </summary>
        /// <param name="step"></param>
        public abstract void Rollback(TransactionStep step);

        public abstract void Commit(TransactionStep step);

        public CommandResult Execute(IExecutionContext context, Transaction transaction)
        {
            return Do(context, transaction);
        }

    }
}
