using System.Windows.Forms;
using Gin.Attributes;
using Gin.Context;


namespace Gin.Controls
{
    [GinName(Name = "Диалог <Сохранить файл>", Description = "Диалог <Сохранить файл>", Group = "Поля ввода")]
    public class UserInputSaveFileDialog: UserInputControl
    {
        [GinArgumentText(Name = "Заголовок", Description = "Заголовок текстового поля, описывающий вводимый пользователем путь")]
        public string Caption { get; set; }

        public override string ToString()
        {
            return "Сохранить файл(" + Caption + ")";
        }

        public override Control Create(IExecutionContext context)
        {
            string absoluteInitialValue = context.GetStringFrom((string)Value);
            _control = new Editors.BrowseFileEditor(true, Caption, absoluteInitialValue, null, null, null);
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
