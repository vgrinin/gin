using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Gin;
using Gin.Attributes;
using Gin.Commands;
using System.Xml.Serialization;

namespace Gin.SQL.Commands
{
    public class SqlParameterClass
    {

        [GinArgumentText(Name = "Имя параметра", Description = "Имя параметра")]
        public string ParameterName { get; set; }

        [GinArgumentEnum(Name = "Тип", Description = "Тип параметра", ListEnum = typeof(SqlDbType))]
        public SqlDbType Type { get; set; }

        [GinArgumentText(AllowTemplates = true, Name = "Значение параметра", Description = "Контекстное имя переменной, из которой считывается значение параметра.")]
        public string ValueName { get; set; }

        [GinArgumentInt(Name = "Размер параметра", Description = "Размер SQL-параметра")]
        public int Size { get; set; }

        [GinArgumentEnum(Name = "Направление параметра", Description = "Направление SQL-параметра", ListEnum = typeof(ParameterDirection))]
        public ParameterDirection Direction { get; set; }

        public override string ToString()
        {
            return Direction.ToString() + " " + Type.ToString() + " " + ParameterName + " = (" + ValueName + ")";
        }
    }
}