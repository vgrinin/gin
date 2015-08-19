using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gin.tests
{

    internal class LoggedEvent
    {
        public ExecutionLoggerEventType Type;
        public object Data;
    }

    internal class ExecutionLoggerMock: ExecutionLogger
    {
        public readonly List<LoggedEvent> Events = new List<LoggedEvent>();

        public override void Event(ExecutionLoggerEventType type, object data)
        {
            Events.Add(new LoggedEvent
                            {
                                Data = data,
                                Type = type
                            });
        }

        public override void Flush(bool releaseResource)
        {
        }
    }
}
