using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gin.Attributes;
using Gin.Context;


namespace Gin.Controls
{
     [GinName(Name = "Да/нет", Description = "Поле ввода типа <да/нет>", Group = "Поля ввода")]
     public class UserInputCheckBox : UserInputControl
     {
        [GinArgumentText(Name = "Заголовок", Description = "Заголовок")]
        public string Caption { get; set; }

        public override string ToString()
        {
            return "CheckBox(" + Caption + ")";
        }

        public override Control Create(IExecutionContext context)
        {
            bool absoluteInitialValue = context.GetBoolFrom(Value);
            _control = new Editors.CheckBoxEditor(Caption, absoluteInitialValue, null, null, null);
            return _control;
        }

        protected override bool Validate()
        {
            return true;
        }

        private object _controlValue;
        protected Control _control;

        [GinArgumentCheckBox(Name = "Значение", Description = "Значение поля ввода")]
        public override object Value
        {
            get
            {
                if (_control != null)
                {
                    return ((Editors.IEditor)_control).Value;
                }
                else
                {
                    return _controlValue;
                }
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
