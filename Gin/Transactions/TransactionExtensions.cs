using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gin.Transactions
{
    public static class TransactionExtensions
    {
        public static void BackForEach(this IEnumerable<TransactionStep> steps, Action<TransactionStep> proc)
        {
            for (int i = steps.Count() - 1; i >= 0; i--)
            {
                proc(steps.ElementAt(i));
            }
        }
    }
}
