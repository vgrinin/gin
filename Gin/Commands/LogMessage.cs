using Gin.Attributes;
using Gin.Context;

namespace Gin.Commands
{
    [GinName(Name = "Сохранить сообщение", Description = "Сохраняет сообщение в лог", Group = "Управление")]
    public class LogMessage: Command
    {

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Multiline = true, Name = "Текст сообщения", Description = "Текстовое сообщение")]
        public string MessageText { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            string absoluteMessageText = context.GetStringFrom(MessageText);
            context.Log.AddLogInformation(absoluteMessageText);

            return CommandResult.Next;
        }
    }
}
