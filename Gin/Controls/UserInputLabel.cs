using System.Windows.Forms;
using Gin.Attributes;
using Gin.Context;


namespace Gin.Controls
{
    [GinName(Name = "Неизменяемый текст", Description = "Неизменяемый текст", Group = "Поля ввода")]
    public class UserInputLabel : UserInputControl
    {

        public override string ToString()
        {
            return "Простой текст(" + Value + ")";
        }

        public override Control Create(IExecutionContext context)
        {
            string absoluteInitialValue = context.GetStringFrom((string)Value);
            _control = new Editors.LabelEditor(absoluteInitialValue);
            return _control;
        }

        protected override bool Validate()
        {
            return true;
        }

        private object _controlValue;
        protected Control _control;

        [GinArgumentText(Name = "Значение", Description = "Значение поля ввода")]
        public override object Value
        {
            get
            {
                if (_control != null)
                {
                    return ((Editors.IEditor)_control).Value;
                }
                return _controlValue;
            }
            set
            {
                if (_control != null)
                {
                    ((Editors.IEditor)_control).Value = value;
                }
                else
                {
                    _controlValue = value;
                }
            }
        }
    }
}
