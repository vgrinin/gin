using System;
using Gin.Attributes;
using Gin.Context;

namespace Gin.Commands
{
    [GinName(Name = "Показать пользовательское событие", Description = "Показать пользовательское событие", Group = "События")]
    public class UserInfo: Command
    {

        #region Аргументы команды

        [GinArgumentText(Multiline = false, Name = "Текст сообщения", Description = "Текстовое сообщение, отображаемое в ходе выполнения пакета")]
        public string MessageText { get; set; }

        #endregion

        public string MessageGuid { get; set; }

        public UserInfo()
        {
            MessageGuid = Guid.NewGuid().ToString("N");
        }

        public override CommandResult Do(IExecutionContext context)
        {
            UserInfoData info = new UserInfoData
            {
                Message = MessageText,
                State =  UserInfoState.Success,
                Guid = MessageGuid
            };
            context.Log.AddUserInfo(info);
            return CommandResult.Next;
        }
    }

    public enum UserInfoState
    {
        Success = 1,
        Fault = 2,
        Running = 0,
        Dismiss = 3
    }

    public class UserInfoData
    {
        public string Message;
        public UserInfoState State;
        public string Guid;
        public override string ToString()
        {
            return Message;
        }
    }
}
