using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gin.Logging
{
    public class ExecutionLoggerTextFile: ExecutionLogger
    {
        string _fileName = null;
        FileStream _fileStream = null;
        string _sessionName = "";

        public ExecutionLoggerTextFile(string fileName)
        {
            _fileName = fileName;
            if (!File.Exists(fileName))
            {
                _fileStream = File.Create(fileName);
                _fileStream.Close();
            }
            _fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            WriteString("Сессия открыта в " + DateTime.Now.ToString() + "\r\n");
        }

        public ExecutionLoggerTextFile(string fileName, string sessionName)
            : this(fileName)
        {
            _sessionName = sessionName + "; ";
        }

        public override void Event(ExecutionLoggerEventType type, object data)
        {
            string message = type.ToString().PadRight(11) + "; " + DateTime.Now.ToString() + "; " + _sessionName + data + "\r\n";
            WriteString(message);
        }

        private void WriteString(string message)
        {
            byte[] buffer = Encoding.Default.GetBytes(message);
            _fileStream.Write(buffer, 0, buffer.Length);
        }

        public override void Flush(bool releaseResource)
        {
            if (_fileStream != null)
            {
                if (releaseResource)
                {
                    WriteString("Сессия закрыта в " + DateTime.Now.ToString());
                    WriteString("\r\n\r\n\r\n");
                    _fileStream.Close();
                    _fileStream.Dispose();
                }
                else
                { 
                    _fileStream.Flush();
                }
            }
        }
    }
}
