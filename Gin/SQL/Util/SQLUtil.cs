using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Gin;
using Gin.Commands;
using Gin.Context;
using Gin.SQL.Commands;


namespace Gin.SQL.Util
{
    public static class SQLUtil
    {
        public static string GetConnectionStringPart(string connectionString, string key)
        {
            Dictionary<string, string> parts = GetConnectionStringParts(connectionString);
            return parts[key];
        }

        public static Dictionary<string, string> GetConnectionStringParts(string connectionString)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (String.IsNullOrEmpty(connectionString))
            {
                return result;
            }

            string[] parts = connectionString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                string[] pair = part.Split('=');
                result[pair[0]] = pair[1];
            }

            return result;
        }

        public static SqlParameterClass AddSqlParameterToContext(this IExecutionContext context, string name, string value, SqlDbType type, int size, ParameterDirection direction)
        {
            string prefix = Guid.NewGuid().ToString("N") + "_";
            string contextName = prefix + name;
            string valueName = contextName + "_value";

            SaveString saveConst = new SaveString()
            {
                Value = value,
                ResultName = valueName
            };
            saveConst.Do(context);
            SqlParameterClass p = new SqlParameterClass()
            {
                ParameterName = name,
                Direction = direction,
                Type = type,
                Size = size,
                ValueName = valueName
            };
            return p;
        }
    }
}
