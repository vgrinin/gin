using System;
using System.Windows.Forms;
using Gin.Attributes;
using Gin.Context;

namespace Gin.Commands
{
    [GinName(Name = "Показать сообщение", Description = "Показывает сообщение в стандартном MessageBox", Group = "Управление")]
    public class ShowMessage: Command
    {

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Multiline = true, Name = "Текст сообщения", Description = "Текстовое сообщение, отображаемое в MessageBox")]
        public string MessageText { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            string absoluteMessageText = context.GetStringFrom(MessageText);
            if (context.ControlContainer != null)
            {
                context.ControlContainer.Invoke(new Action(() =>
                {
                    MessageBox.Show(context.ControlContainer, absoluteMessageText, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                MessageBox.Show(absoluteMessageText, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return CommandResult.Next;
        }
    }
}
