using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Commands;

namespace Gin.tests
{
    internal class TestVisitor: CommandVisitor
    {

        public Command VisitedCommand;

        public override void Visit(Command command)
        {
            VisitedCommand = command;
			// some comments
        }
    }
}
