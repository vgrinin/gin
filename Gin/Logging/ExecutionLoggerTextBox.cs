using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Gin.Logging
{
    public class ExecutionLoggerTextBox: ExecutionLogger
    {

        private TextBox _textBox;

        public ExecutionLoggerTextBox(TextBox textBox)
        {
            _textBox = textBox;
        }

        public override void Event(ExecutionLoggerEventType type, object data)
        {
            string message = type.ToString() + ": " + data + "\r\n";
            if (_textBox.InvokeRequired)
            {
                _textBox.Invoke(new Action(() => 
                { 
                    _textBox.AppendText(message); 
                }));
            }
            else
            {
                _textBox.AppendText(message);
            }
        }

        public override void Flush(bool releaseResource)
        {
        }
    }
}
