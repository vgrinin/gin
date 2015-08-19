using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Commands;
using System.IO;

namespace Gin.Transactions
{
    /// <summary>
    /// Шаг транзакции
    /// </summary>
    public abstract class TransactionStep
    {
        public const string STEPS_SUBFOLDER_NAME = @"steps";

        public int StepNumber { get; set; }
        public string TransactionName { get; set; }
        public TransactionalCommand Command { get; set; }

        public abstract void Commit();
        public abstract void Rollback();
        /// <summary>
        /// Возращает путь на диске, где можно сохранять любые файлы, необходимые для логики отката шага транзакции
        /// </summary>
        /// <returns>Путь к папке с файлами</returns>
        public string GetPath(string transactionsPath)
        {
            //string transactionsPath = context.ExecutedPackage.TransactionsPath;
            string transactionPath = Path.Combine(transactionsPath, TransactionName);
            string stepsPath = Path.Combine(transactionPath, STEPS_SUBFOLDER_NAME);
            string stepPath = Path.Combine(stepsPath, StepNumber.ToString());
            if (!Directory.Exists(stepPath))
            {
                Directory.CreateDirectory(stepPath);
            }
            return stepPath;
        }
    }
}
