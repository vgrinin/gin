using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Commands;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using Gin.Context;

namespace Gin.Transactions
{

    public enum TransactionState
    {
        Undefined,
        Active,
        Commited,
        Rollbacked
    }

    public class Transaction
    {
        public string TransactionName { get; set; }
        public TransactionState TransactionState { get; set; }
        public List<TransactionStep> Steps { get; set; }

        public Transaction()
        {
            Steps = new List<TransactionStep>();
            TransactionState = Transactions.TransactionState.Undefined;
        }

        public T CreateStep<T>(TransactionalCommand command) where T: TransactionStep, new()
        {
            T step = new T();
            int stepNumber = Steps.Count;
            step.StepNumber = stepNumber;
            step.Command = command;
            step.TransactionName = TransactionName;

            Steps.Add(step);
            return step;
        }

        public void Save(IExecutionContext context)
        {
            string transactionsPath = context.ExecutedPackage.TransactionsPath;
            string transactionPath = Path.Combine(transactionsPath, TransactionName + @"\");
            Directory.CreateDirectory(transactionPath);
            string dataFilePath = Path.Combine(transactionPath, @"data.xml");
            GinSerializer.Serialize(this, dataFilePath);
        }

        public void Rollback()
        {
            if (TransactionState != Transactions.TransactionState.Active)
            {
                throw new InvalidOperationException("Нельзя откатить не начатую транзакцию");
            }            
            Steps.BackForEach(s =>
            {
                s.Command.Rollback(s);
            });
            TransactionState = Transactions.TransactionState.Rollbacked;
        }

        public void Commit()
        {
            if (TransactionState != Transactions.TransactionState.Active)
            {
                throw new InvalidOperationException("Нельзя зафиксировать не начатую транзакцию");
            }
            Steps.BackForEach(s =>
            {
                s.Command.Commit(s);
            });
            TransactionState = Transactions.TransactionState.Commited;
        }
    }
}
