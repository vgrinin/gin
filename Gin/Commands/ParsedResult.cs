using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gin.Attributes;
using Gin.Context;

namespace Gin.Commands
{
    public class ParsedResult
    {
        public string Name;
        public object Value;
        public Type Type;
        public string Description;

        private static string CleanPercents(string argument)
        {
            if (argument.StartsWith(ExecutionContext.InputTemplateTag))
            {
                argument = argument.Remove(0, 1);
            }
            if (argument.EndsWith(ExecutionContext.InputTemplateTag))
            {
                argument = argument.Remove(argument.Length - 1, 1);
            }
            return argument;
        }

        public static List<ParsedResult> GetObjectValueMembers(Type type, object objectValue, string objectName)
        {
            var result = new List<ParsedResult>();
            foreach (FieldInfo field in type.GetFields())
            {
                if (field.IsStatic)
                {
                    continue;
                }
                object fieldValue = objectValue != null ? field.GetValue(objectValue) : null;
                string cleanedArgumentName = CleanPercents(objectName);
                string newArgumentName = cleanedArgumentName + "." + field.Name;
                GinResultAttribute attr = field.GetCustomAttributes(false).OfType<GinResultAttribute>().FirstOrDefault();
                result.Add(new ParsedResult
                               {
                                   Name = newArgumentName,
                                   Value = fieldValue,
                                   Type = attr != null ? attr.Result : null,
                                   Description = attr != null ? attr.Description : null
                               });
            }

            foreach (PropertyInfo prop in type.GetProperties())
            {
                object propValue = objectValue != null ? prop.GetValue(objectValue, null) : null;
                string cleanedArgumentName = CleanPercents(objectName);
                string newArgumentName = cleanedArgumentName + "." + prop.Name;
                GinResultAttribute attr = prop.GetCustomAttributes(false).OfType<GinResultAttribute>().FirstOrDefault();
                result.Add(new ParsedResult
                               {
                                   Name = newArgumentName,
                                   Value = propValue,
                                   Type = attr != null ? attr.Result : null,
                                   Description = attr != null ? attr.Description : null
                               });
            }

            return result;
        }

    }
}
