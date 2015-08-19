using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Attributes;
using Gin.Context;
using Gin.Controls;
using System.Windows.Forms;
using System.Data;
using Gin.SQL.Util;
using Gin;


namespace Avicomp.Installer
{

    [GinName(Name = "OSBB SQL-Server", Description = "Подключение к OSBB SQL-Server, используя данные собранные скриптом для SQL-сервера", Group = "Составные поля ввода")]
    public class ICOSBBSQLConnection : UserInputControl
    {

        [GinResult(Result = typeof(SQLOSBBConnectionProperties), Kind = CommandResultKind.Complex, Description = "Свойства экземпляра OSBB SQL-сервера")]
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

        public override Control Create(IExecutionContext context)
        {
            SQLOSBBConnectionProperties val = (SQLOSBBConnectionProperties)Value;

            _control = new SQLOSBBConnectionControl2(context);
            SQLOSBBConnectionControl2 control = (SQLOSBBConnectionControl2)_control;
            control.Value = val;
            return _control;
        }

        private SQLOSBBConnectionProperties _controlValue;
        protected SQLOSBBConnectionControl2 _control;

        [GinArgumentComplexValue(ValueType = typeof(SQLOSBBConnectionProperties), Name = "Значение", Description = "Значение поля ввода")]
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
                    _control.Value = (SQLOSBBConnectionProperties)value;
                }
                else
                {
                    _controlValue = (SQLOSBBConnectionProperties)value;
                }
            }
        }
        protected override bool Validate()
        {
            return true;
        }

    }
}
