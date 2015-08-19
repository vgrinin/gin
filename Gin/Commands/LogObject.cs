using System;
using System.IO;
using Gin.Attributes;
using Gin.Context;
using Gin.Util;
using System.Data;

namespace Gin.Commands
{
    [GinName(Name = "Сохранить объект в лог", Description = "Сохраняет объект в лог", Group = "Управление")]
    public class LogObject: Command
    {

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Multiline = false, Name = "Имя объекта", Description = "Имя объекта")]
        public string ObjectName { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            object absoluteObject = context.GetResult(ObjectName);
            string tempPath = Path.Combine(context.TempPath, Guid.NewGuid().ToString("N") + ".dat");
            if (absoluteObject is DataTable)
            {
                DataTable dataTable = (DataTable)absoluteObject;
                dataTable.WriteXml(tempPath);
            }
            else
            {
                GinSerializer.Serialize(absoluteObject, tempPath);
            }
            string objectStringValue = IOUtil.ReadFile(tempPath);
            File.Delete(tempPath);
            context.Log.AddLogInformation(ObjectName + "=" + objectStringValue);

            return CommandResult.Next;
        }
    }
}
