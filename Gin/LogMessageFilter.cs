using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gin
{
    public abstract class LogMessageFilter
    {
        public abstract string Filter(string message);
    }
}
