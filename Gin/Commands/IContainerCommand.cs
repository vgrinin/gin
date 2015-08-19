using System.Collections.Generic;

namespace Gin.Commands
{
    public interface IContainerCommand
    {
        IEnumerable<Command> InnerCommands { get; }
    }
}
