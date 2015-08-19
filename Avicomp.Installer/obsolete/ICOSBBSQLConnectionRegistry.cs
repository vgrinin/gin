using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Controls;
using System.Windows.Forms;
using System.Data;
using Gin.SQL.Util;
using Gin;


namespace Avicomp.Installer
{

    [GinName(Name = "OSBB SQL-Server (реестр)", Description = "Подключение к OSBB SQL-Server, с использованием списка доступных конфигураций, прочитанных из реестра", Group = "Составные поля ввода")]
    public class ICOSBBSQLConnectionRegistry: UserInputControl
    {

        [GinResult(Result = typeof(OSBBInstanceConfig), Kind = CommandResultKind.Complex, Description = "Свойства экземпляра OSBB SQL-сервера")]
        [GinArgumentText(Name = "Результат", Description = "Контекстное имя переменной, куда будет сохранен результат")]
        public override string ResultName
        {
            get
            {
                return base.ResultName;
            }
            set
            {
                base.ResultName = value;
            }
        }

        public override Control Create(Gin.ExecutionContext context)
        {
            OSBBInstanceConfig val = (OSBBInstanceConfig)Value;

            _control = new SQLOSBBConnectionControl(context);
            SQLOSBBConnectionControl control = (SQLOSBBConnectionControl)_control;
            control.Value = val;
            return _control;
        }

        private OSBBInstanceConfig _controlValue;
        protected SQLOSBBConnectionControl _control;

        [GinArgumentComplexValue(ValueType = typeof(OSBBInstanceConfig), Name = "Значение", Description = "Значение поля ввода")]
        public override object Value
        {
            get
            {
                if (_control != null)
                {
                    return _control.Value;
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
                    _control.Value = (OSBBInstanceConfig)value;
                }
                else
                {
                    _controlValue = (OSBBInstanceConfig)value;
                }
            }
        }
        protected override bool Validate()
        {
            return true;
        }

    }
}