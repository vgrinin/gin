using System.Collections.Generic;
using System.Windows.Forms;

namespace Gin.Context
{
    public interface IExecutionContext
    {
        void SaveResult(string key, object value, bool save);
        void SaveResult(string key, object value);
        object GetResult(string key);
        string GetStringFrom(string inputString);
        bool GetBoolFrom(object input);
        int GetIntFrom(object input);

        Logging.Logging Log { get; set; }
        Control ControlContainer { get; set; }

        string PackagesPath { get;}
        string TempPath { get; }

        List<CustomInstallationParameter> GetInstallationParameters();
        bool AskUserToSkipError(string message);
        Package ExecutedPackage { get; set; }
    }
}
