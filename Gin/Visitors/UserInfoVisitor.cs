using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gin.Commands
{
    public class UserInfoVisitor: CommandVisitor
    {

        private List<UserInfoEmbedded> _list = new List<UserInfoEmbedded>();

        public List<UserInfoEmbedded> UserInfos
        {
            get
            {
                return _list;
            }
        }

        public override void Visit(Command command)
        {
            if (command is UserInfo)
            {
                UserInfo ui = (UserInfo)command;
                _list.Add(new UserInfoEmbedded()
                {
                    MessageGuid = ui.MessageGuid,
                    MessageText = ui.MessageText
                });
            }
            if (command.UserInfo != null && !String.IsNullOrEmpty(command.UserInfo.MessageText))
            {
                _list.Add(command.UserInfo);
            }
        }
    }
}
