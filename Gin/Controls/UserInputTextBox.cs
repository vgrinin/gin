using System.Windows.Forms;
using Gin.Attributes;
using Gin.Context;


namespace Gin.Controls
{
    [GinName(Name = "Текст", Description = "Текстовое поле ввода", Group = "Поля ввода")]
    public class UserInputTextBox : UserInputControl
    {

        [GinArgumentText(Name = "Заголовок", Description = "Заголовок текстового поля, описывающий вводимый текстовый параметр")]
        public string Caption { get; set; }

        [GinArgumentCheckBox(Name = "Поле ввода пароля", Description = "Вводимые символы не будут отображаться на экране")]
        public bool Password { get; set; }

        public override Control Create(IExecutionContext context)
        {
            string absoluteValue = context.GetStringFrom((string)Value);
            _control = new Editors.TextEditor(Caption, absoluteValue, 0, "", null, null);
            return _control;
        }

        protected override bool Validate()
        {
            return true;
        }

        public override string ToString()
        {
            return "TextBox(" + Caption + ")";
        }
        private object _controlValue;
        protected Control _control;

        [GinArgumentText(AllowTemplates = true, Name = "Значение", Description = "Значение поля ввода")]
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
