using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Attributes;
using Gin.Context;
using Gin.Controls;
using System.Windows.Forms;
//using Microsoft.SqlServer;
//using Microsoft.SqlServer.Management.Smo;
using System.Data;
using Gin.SQL.Util;

namespace Gin.SQL.Controls
{

    [GinName(Name = "Подключение к SQL-Server", Description = "Подключение к SQL-Server", Group = "Составные поля ввода")]
    public class ICSQLConnection : UserInputControl
    {

        [GinArgumentCheckBox(AllowTemplates = true, Name = "Фиксированное имя сервера", Description = "Фиксированное имя сервера")]
        public object FixedInstanceName { get; set; }

        [GinArgumentCheckBox(AllowTemplates = true, Name = "Фиксированное имя БД", Description = "Фиксированное имя БД")]
        public object FixedDBName { get; set; }

        [GinArgumentCheckBox(AllowTemplates = true, Name = "Фиксированный тип аутентификации", Description = "Фиксированный тип аутентификации")]
        public object FixedSqlAuth { get; set; }

        [GinArgumentCheckBox(AllowTemplates = true, Name = "Фиксированное имя пользователя", Description = "Фиксированное имя пользователя")]
        public object FixedUserName { get; set; }

        [GinResult(Result = typeof(SQLConnectionProperties), Kind = CommandResultKind.Complex, Description = "Свойства подключения к SQL-серверу")]
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
            SQLConnectionProperties val = (SQLConnectionProperties)Value;

            _control = new SQLConnectionControl(context);
            SQLConnectionControl control = (SQLConnectionControl)_control;
            control.Value = val;

            bool fixedInstanceName = context.GetBoolFrom(FixedInstanceName);
            bool fixedDBName = context.GetBoolFrom(FixedDBName);
            bool fixedSqlAuth = context.GetBoolFrom(FixedSqlAuth);
            bool fixedUserName = context.GetBoolFrom(FixedUserName);

            control.SetControlEnabled(SQLConnectionSubControl.UserName, !fixedUserName);
            control.SetControlEnabled(SQLConnectionSubControl.SqlAuthentication, !fixedSqlAuth);
            control.SetControlEnabled(SQLConnectionSubControl.DBName, !fixedDBName);
            control.SetControlEnabled(SQLConnectionSubControl.ServerName, !fixedInstanceName);

            return _control;
        }

        private SQLConnectionProperties _controlValue;
        protected SQLConnectionControl _control;

        [GinArgumentComplexValue(ValueType = typeof(SQLConnectionProperties), Name = "Значение", Description = "Значение поля ввода")]
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
                    _control.Value = (SQLConnectionProperties)value;
                }
                else
                {
                    _controlValue = (SQLConnectionProperties)value;
                }
            }
        }
        protected override bool Validate()
        {
            return true;
        }

    }
}
