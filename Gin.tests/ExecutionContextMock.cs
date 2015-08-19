using System.Collections.Generic;
using System.Windows.Forms;
using Gin.Context;

namespace Gin.tests
{
    public class ExecutionContextMock: IExecutionContext
    {

        private readonly Dictionary<string, object> _results = new Dictionary<string, object>();

        public const string INPUT_TEMPLATE_TAG = "%";

        public bool SkipError;

        public static string GetPercentedKey(string key)
        {
            if (!key.StartsWith(INPUT_TEMPLATE_TAG))
            {
                key = INPUT_TEMPLATE_TAG + key;
            }
            if (!key.EndsWith(INPUT_TEMPLATE_TAG))
            {
                key = key + INPUT_TEMPLATE_TAG;
            }
            return key;
        }

        #region IExecutionContext Members

        public void SaveResult(string key, object value, bool save)
        {
            _results[GetPercentedKey(key)] = value;
        }

        public void SaveResult(string key, object value)
        {
            _results[GetPercentedKey(key)] = value;
        }

        public object GetResult(string key)
        {
            key = GetPercentedKey(key);
            if (_results.ContainsKey(key))
            {
                return _results[key];
            }
            return null;
        }

        public string GetStringFrom(string inputString)
        {
            return inputString;
        }

        public bool GetBoolFrom(object input)
        {
            return (input is bool) ? (bool)input : (bool)(GetResult((string)input));
        }

        public int GetIntFrom(object input)
        {
            return (input is int) ? (int)input : (int)(GetResult((string)input));
        }

        public Logging.Logging Log { get; set; }

        public Control ControlContainer { get; set; }

        public string PackagesPath
        {
            get { throw new System.NotImplementedException(); }
        }

        public string TempPath
        {
            get { throw new System.NotImplementedException(); }
        }

        public List<CustomInstallationParameter> GetInstallationParameters()
        {
            throw new System.NotImplementedException();
        }

        public bool AskUserToSkipError(string message)
        {
            return SkipError;
        }

        public Package ExecutedPackage { get; set; }

        #endregion
    }
}
