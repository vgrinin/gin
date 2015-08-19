using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gin.Commands;

namespace Gin.Logging
{
    public class ExecutionLoggerUserInfo: ExecutionLogger
    {

        private ListView _listBox;

        public ExecutionLoggerUserInfo(ListView listBox)
        {
            _listBox = listBox;
        }

        public override void Event(ExecutionLoggerEventType type, object data)
        {
            if (type == ExecutionLoggerEventType.UserInfo)
            {
                UserInfoData info = (UserInfoData)data;
                int imageIndex = (int)info.State;
                _listBox.Invoke(new Action(() =>
                {
                    if (_listBox.Items.ContainsKey(info.Guid))
                    {
                        _listBox.Items[info.Guid].ImageIndex = imageIndex;
                    }
                    else
                    {
                        _listBox.Items.Add(info.Guid, info.Message, imageIndex);
                    }
                }));
            }
        }

        public override void Flush(bool releaseResource)
        {
        }
    }
}
