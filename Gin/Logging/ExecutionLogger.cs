using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Commands;
using System.Reflection;


namespace Gin
{

    public delegate void LoggerEvent(ExecutionLoggerEventType type, string message);

    public enum ExecutionLoggerEventType
    {
        Information,
        Warning,
        Error, 
        UserInfo,
        Exception
    }

    public abstract class ExecutionLogger
    {

        public const string EXECUTION_LOG_FILENAME = "execution.log";

        public ExecutionLogger()
        {
        }

        public abstract void Event(ExecutionLoggerEventType type, object data);
        public abstract void Flush(bool releaseResource);

        public void Information(string message)
        {
            Event(ExecutionLoggerEventType.Information, message);
        }
        public void Warning(string message)
        {
            Event(ExecutionLoggerEventType.Warning, message);
        }
        public void Error(string message)
        {
            Event(ExecutionLoggerEventType.Error, message);
        }
        public void Exception(Exception exception)
        {
            if (exception is ReflectionTypeLoadException)
            {
                ReflectionTypeLoadException tlex = (ReflectionTypeLoadException)exception;
                if (tlex.LoaderExceptions != null)
                {
                    foreach (Exception item in tlex.LoaderExceptions)
                    {
                        if (item != null)
                        {
                            Event(ExecutionLoggerEventType.Exception, item.ToString() + " " + item.Message + "; StackTrace: " + item.StackTrace);
                        }
                    }
                }
            }

            Event(ExecutionLoggerEventType.Exception, exception.ToString() + " " + exception.Message + "; StackTrace: " + exception.StackTrace);
        }

        public void UserInfo(UserInfoData info)
        {
            Event(ExecutionLoggerEventType.UserInfo, info);
        }
    }
}
