using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace Gin.Context
{

    public class ExecutionContext: IExecutionContext
    {

        public const string TEMP_SUBFOLDER_NAME = @"temp";
        public const string PACKAGES_SUBFOLDER_NAME = @"packages";

        public const string INPUT_TEMPLATE_TAG = "%";
        public const string PACKAGE_SOURCE_TAG = INPUT_TEMPLATE_TAG + "PACKAGE_SOURCE" + INPUT_TEMPLATE_TAG;
        public const string PACKAGE_CONTENT_TAG = INPUT_TEMPLATE_TAG + "PACKAGE_CONTENT" + INPUT_TEMPLATE_TAG;
        public const string GIN_EXE_TAG = INPUT_TEMPLATE_TAG + "GIN_EXE" + INPUT_TEMPLATE_TAG;

        private readonly Dictionary<string, object> _results;
        private readonly List<string> _saveResults;

        public string GinPath { get; private set; }
        public string PackagesPath { get; private set; }
        public string TempPath { get; private set; }

        public Control ControlContainer { get; set; }
        public Package ExecutedPackage { get; set; }

        public Logging.Logging Log { get; set; }

/*
        private string GetExecutedDllDirectory()
        {
            string executedDllFilePath = Assembly.GetExecutingAssembly().Location;
            string executedDllDirectory = Path.GetDirectoryName(executedDllFilePath);
            if (executedDllDirectory != null && executedDllDirectory.EndsWith(@"\"))
            {
                executedDllDirectory = executedDllDirectory.Remove(executedDllDirectory.Length - 1);
            }
            return executedDllDirectory;
        }
*/

        public ExecutionContext(string rootPath)
        {
            _results = new Dictionary<string, object>();
            _saveResults = new List<string>();
            InitGeneralPaths(rootPath);
        }

        private void InitGeneralPaths(string rootPath)
        {
            GinPath = rootPath;
            PackagesPath = Path.Combine(GinPath, PACKAGES_SUBFOLDER_NAME);
            TempPath = Path.Combine(GinPath, TEMP_SUBFOLDER_NAME);
        }

        public void SaveResult(string key, object value)
        {
            SaveResult(key, value, false);
        }

        public void SaveResult(string key, object value, bool save)
        {
            if (key == null)
            {
                return;
            }
            _results[GetPercentedKey(key)] = value;
            if (save)
            {
                _saveResults.Add(key);
            }
        }

        public object GetResult(string key)
        {
            if (key == null)
            {
                return null;
            }
            key = GetPercentedKey(key);
            if (_results.ContainsKey(key))
            {
                return _results[key];
            }
            return null;
        }

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

        public string GetStringFrom(string inputString)
        {
            if (inputString == null)
            {
                return null;
            }
            if (inputString.StartsWith(PACKAGE_CONTENT_TAG) && ExecutedPackage != null)
            {
                string contentFileName = inputString.Remove(0, PACKAGE_CONTENT_TAG.Length + 1);
                inputString = ExecutedPackage.GetContentPath(contentFileName);
                return inputString;
            }
            if (inputString.StartsWith(GIN_EXE_TAG))
            {
                string executedDllFilePath = Assembly.GetExecutingAssembly().Location;
                string executedDllDirectory = Path.GetDirectoryName(executedDllFilePath);
                if(executedDllDirectory.EndsWith(@"\"))
                {
                    executedDllDirectory = executedDllDirectory.Remove(executedDllDirectory.Length - 1);
                }
                inputString = inputString.Replace(GIN_EXE_TAG, executedDllDirectory);
                return inputString;
            }

            foreach (var input in _results)
            {
                inputString = inputString.Replace(input.Key, input.Value != null ? input.Value.ToString() : "");
            }
            return inputString;
        }

        public bool GetBoolFrom(object input)
        {
            if (input == null)
            {
                return false;
            }
            bool result = (input is bool) ? (bool)input : (bool)(GetResult((string)input));
            return result;
        }

        public int GetIntFrom(object input)
        {
            if (input == null)
            {
                return 0;
            }
            int result = (input is int) ? (int)input : (int)(GetResult((string)input));
            return result;
        }


        public List<CustomInstallationParameter> GetInstallationParameters()
        {
            List<CustomInstallationParameter> result = new List<CustomInstallationParameter>();

            foreach (string item in _saveResults.Distinct())
            {
                object value = _results[GetPercentedKey(item)];
                result.Add(new CustomInstallationParameter
                {
                    Name = item,
                    Value = value
                });
            }

            return result;
        }

        public bool AskUserToSkipError(string message)
        {
            return MessageBox.Show(ControlContainer, "Произошла ошибка <" + message + ">. Хотите пропустить?", "Ошибка", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK;
        }
    }
}
