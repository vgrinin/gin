using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin;
using Gin.Attributes;
using Gin.Context;
using Gin.Controls;
using System.Windows.Forms;
//using Microsoft.SqlServer;
//using Microsoft.SqlServer.Management.Smo;
using System.Data;

namespace Gin.SQL.Controls
{

    [GinName(Name = "Экземпляр SQL-Server", Description = "Экземпляр SQL-Server", Group = "Составные поля ввода")]
    public class ICChooseSQLInstance : UserInputControl
    {

        [GinResult(Result = typeof(ICSQLInstance), Kind = CommandResultKind.Complex, Description = "Свойства экземпляра SQL-сервера")]
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

        public override string ToString()
        {
            return "Экземпляр SQL-Server";
        }

        public override Control Create(IExecutionContext context)
        {
            ICSQLInstance val = (ICSQLInstance)Value;

            _control = new SQLInstanceControl(context);

            SQLInstanceControl control = (SQLInstanceControl)_control;
            control.Value = val; 
            return _control;
        }

        protected override bool Validate()
        {
            return true;
        }

        private ICSQLInstance _controlValue;
        protected SQLInstanceControl _control;

        [GinArgumentComplexValue(ValueType = typeof(ICSQLInstance), Name = "Значение", Description = "Значение поля ввода")]
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
                    _control.Value = (ICSQLInstance)value;
                }
                else
                {
                    _controlValue = (ICSQLInstance)value;
                }
            }
        }
    }


    [GinIncludeType()]
    public class ICSQLInstance
    {
        [GinArgumentCheckBox(Name = "Создать экземпляр", Description = "Создаем новый, а не подключаемся к существующему")]
        [GinResult(Result = typeof(bool), Kind = CommandResultKind.Primitive, Description = "Создать ли экземпляр")]
        public bool InstallNewInstance { get; set; }

        [GinArgumentText(Name = "Имя экземпляра", Description = "Имя экземпляра")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Имя экземпляра")]
        public string InstanceName { get; set; }

        [GinArgumentBrowseFolder(Name = "Папка", Description = "Путь к папке с данными")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Путь к папке с данными")]
        public string SqlDataDirectory { get; set; }

        public override string ToString()
        {
            return InstanceName + (InstallNewInstance ? ("(" + SqlDataDirectory + ")"): "");
        }
    }
}
