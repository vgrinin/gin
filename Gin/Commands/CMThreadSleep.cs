using System.Threading;
using Gin.Attributes;
using Gin.Context;


namespace Gin.Commands
{
    [GinName(Name = "Ожидание", Description = "Ожидает заданное количество миллисекунд", Group = "Управление")]
    public class CMThreadSleep: Command
    {

        #region Аргументы команды

        [GinArgumentInt(Description = "Время ожидания, мсек", Name = "Время ожидания, мсек")] 
        public int MilliSeconds { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            Thread.Sleep(MilliSeconds);
            return CommandResult.Next;
        }
    }
}
