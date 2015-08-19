using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gin.Commands
{
    public abstract class CommandVisitor
    {
        public abstract void Visit(Command command);
    }
}
