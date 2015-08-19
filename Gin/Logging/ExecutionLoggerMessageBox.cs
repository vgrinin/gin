using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gin.Logging
{
    public class ExecutionLoggerMessageBox: ExecutionLogger
    {

        private Control _owner;

        public ExecutionLoggerMessageBox(Control owner)
        {
            _owner = owner;
        }

        public override void Event(ExecutionLoggerEventType type, object data)
        {
            if (type == ExecutionLoggerEventType.Error)
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new Action(() =>
                    {
                        MessageBox.Show(_owner, (string)data, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                else
                {
                    MessageBox.Show(_owner, (string)data, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public override void Flush(bool releaseResource)
        {
        }
    }
}
