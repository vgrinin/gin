using System.Collections.Generic;
using System.Windows.Forms;
using Gin.Attributes;
using Gin.Context;
using Gin.Editors;

namespace Gin.Controls
{
    [GinName(Name = "Выбор из списка", Description = "Выбор из списка", Group = "Поля ввода")]
    public class UserInputComboBox : UserInputControl
    {
        [GinArgumentText(Name = "Заголовок", Description = "Заголовок списка, описывающий выбираемое пользователем значение")]
        public string Caption { get; set; }

        [GinArgumentText(Name = "Имя списка", Description = "Контекстное имя переменной, в которой хранится отображаемый список")]
        public string ListDataName { get; set; }

        [GinArgumentText(Name = "Имя отображаемого поля", Description = "Отображаемый список обычно представляет собой список записей с полями некоторой фиксированной структуры. Здесь мы задаем имя поля, которое будет видно пользователю в выпадающем списке")]
        public string DisplayMember { get; set; }

        [GinArgumentText(Name = "Имя поля - значения", Description = "Отображаемый список обычно представляет собой список записей с полями некоторой фиксированной структуры. Здесь мы задаем имя поля, которое будет сохранено в контексте в качестве выбора пользователя")]
        public string ValueMember { get; set; }

        [GinArgumentCheckBox(Name = "Выпадающий список", Description = "Выпадающий список без возможности ввести произвольное значение")]
        public bool DropDownOnly { set; get; }

        public override string ToString()
        {
            return "Список(" + Caption + ")";
        }

        public override Control Create(IExecutionContext context)
        {
            object list = context.GetResult(ListDataName);
            _control = new SimpleComboBoxEditor(Caption, list, DropDownOnly, DisplayMember, ValueMember, null, null, null);
            return _control;
        }

        protected override bool Validate()
        {
            return true;
        }

        private object _controlValue;
        protected Control _control;

        [GinArgumentList(ListType = typeof(List<ComboBoxItem>), Name = "Значение", Description = "Значение поля ввода")]
        public override object Value
        {
            get
            {
                if (_control != null)
                {
                    return ((IEditor)_control).Value;
                }
                return _controlValue;
            }
            set
            {
                if (_control != null)
                {
                    ((IEditor)_control).Value = value;
                }
                else
                {
                    _controlValue = value;
                }
            }
        }

    }
}
