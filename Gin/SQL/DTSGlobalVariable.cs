using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Attributes;
using Gin.Commands;
using System.Xml.Serialization;

namespace Gin.SQL.Commands
{

    public class DTSGlobalVariable
    {

        [GinArgumentText(Name = "Имя переменной", Description = "Имя переменной")]
        public string VariableName { get; set; }

        [GinArgumentText(Name = "Имя значения", Description = "Контекстное имя значения параметра")]
        public string VariableValue { get; set; }

    }
}
