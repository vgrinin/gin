using System;

namespace Gin
{
    public class ConnectionStringFilter: LogMessageFilter
    {
        public override string Filter(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                return message;
            }
            string[] parts = message.Split(';');
            string result = "";
            foreach (string part in parts)
            {
                string[] keyValue = part.Split('=');
                if (keyValue[0] == "Password")
                {
                    result += keyValue[0] + "=**************;";
                }
                else
                {
                    result += part + ";";
                }
            }
            return result;
        }
    }
}
