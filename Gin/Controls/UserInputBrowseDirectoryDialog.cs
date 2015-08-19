using System.Windows.Forms;
using Gin.Attributes;
using Gin.Context;


namespace Gin.Controls
{
    [GinName(Name = "Диалог <Открыть папку>", Description = "Диалог <Открыть папку>", Group = "Поля ввода")]
    public class UserInputBrowseDirectoryDialog : UserInputControl
    {

        [GinArgumentText(Name = "Заголовок", Description = "Заголовок текстового поля, описывающий вводимый пользователем путь")]
        public string Caption { get; set; }

        public override string ToString()
        {
            return "Открыть папку(" + Caption + ")";
        }

        public override Control Create(IExecutionContext context)
        {
            string absoluteInitialValue = context.GetStringFrom((string)Value);
            Control = new Editors.BrowseFolderEditor(Caption, absoluteInitialValue, null, null, null);
            return Control;
        }

        protected override bool Validate()
        {
            return true;
        }

        private object _controlValue;
        protected Control Control;

        [GinArgumentText(Name = "Значение", Description = "Значение поля ввода")]
        public override object Value
        {
            get
            {
                if (Control != null)
                {
                    return ((Editors.IEditor)Control).Value;
                }
                return _controlValue;
            }
            set
            {
                if (Control != null)
                {
                    ((Editors.IEditor)Control).Value = value;
                }
                else
                {
                    _controlValue = value;
                }
            }
        }
    }
}
