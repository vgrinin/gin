using System;
using Gin.Commands;
using Gin.Context;

namespace Gin.tests
{
    internal class EmptyCommand: Command
    {

        public Exception ThrowException = null;

        public override CommandResult Do(IExecutionContext context)
        {
            if (ThrowException != null)
            {
                throw ThrowException;
            }
            return CommandResult.Next;
        }
    }
}
