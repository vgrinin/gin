using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gin.Attributes;
using Gin.Context;


namespace Gin.Commands
{
    [GinName(Name = "Читать поля структуры", Description = "Разбивает поля входной структуры(записи) на отдельные элементарные записи", Group = "Данные")]
    public class CMParseResult : Command
    {

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Name = "Имя аргумента", Description = "Контекстное имя входного аргумента команды. Входной аргумент обычно является структурой или классом. После выполнения команда создает в контексте исполнения новые переменные в количестве равном количеству полей входной структуры. Имена новых переменных формируются из имени входной переменной и имени поля, разделенных точками - %ИМЯ_ПЕРЕМЕННОЙ.ИМЯ_ПОЛЯ%")]
        [GinResult(Kind = CommandResultKind.Dynamic)]
        public string ArgumentName { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            object argument = context.GetResult(ArgumentName);
            List<ParsedResult> list = GetObjectValueMembers(argument.GetType(), argument, ArgumentName);
            foreach (var item in list)
            {
                 context.SaveResult(ExecutionContext.GetPercentedKey(item.Name), item.Value);
            }
            return CommandResult.Next;
        }

        private static string CleanPercents(string argument)
        {
            if (argument.StartsWith(ExecutionContext.INPUT_TEMPLATE_TAG))
            {
                argument = argument.Remove(0, 1);
            }
            if (argument.EndsWith(ExecutionContext.INPUT_TEMPLATE_TAG))
            {
                argument = argument.Remove(argument.Length - 1, 1);
            }
            return argument;
        }

        public static List<ParsedResult> GetObjectValueMembers(Type type, object objectValue, string objectName)
        {
            List<ParsedResult> result = new List<ParsedResult>();
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

    public class ParsedResult
    {
        public string Name;
        public object Value;
        public Type Type;
        public string Description;
    }
}
