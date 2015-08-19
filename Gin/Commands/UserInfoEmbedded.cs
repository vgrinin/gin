using System;
using Gin.Attributes;

namespace Gin.Commands
{

    [GinIncludeType]
    public class UserInfoEmbedded
    {
        [GinArgumentText(Multiline = false, Name = "Текст сообщения", Description = "Текстовое сообщение, отображаемое в ходе выполнения пакета")]
        public string MessageText { get; set; }

        //[GinArgumentCheckBox(AllowTemplates = true, Name = "Успешно", Description = "Признак успешности операции")]
        //public object OperationSuccess { get; set; }

        public string MessageGuid { get; set; }

        public UserInfoEmbedded()
        {
            MessageGuid = Guid.NewGuid().ToString("N");
        }

        public override string ToString()
        {
            return "[" + MessageText + "]";
        }
    }
}
