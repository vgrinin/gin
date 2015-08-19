using System.Windows.Forms;
using Gin.Attributes;
using Gin.Commands;
using Gin.Context;

namespace Gin.Controls
{
    // 11/03/2012 Унаследовал от Command чтобы не писать путанной логики в дереве команд для единообразного отображения в дереве полей ввода
    public abstract class UserInputControl: Command
    {

        [GinArgumentText(Name = "Результат", Description = "Контекстное имя переменной, куда будет сохранен результат")]
        [GinResult(Kind = CommandResultKind.Primitive, Description = "Значение контрола", Result = typeof(object))]
        public virtual string ResultName { get; set; }

        public abstract object Value { get; set; }

        public abstract Control Create(IExecutionContext context);

        protected abstract bool Validate();

        public override CommandResult Do(IExecutionContext context)
        {
            return CommandResult.Next;
        }

    }
}