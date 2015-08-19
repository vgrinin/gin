using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Gin.Attributes;
using Gin.Commands;


namespace Gin
{
    public class GinMetaData
    {

        public const string TRANSACTION_STEP_BASE_CLASS_NAME = "Gin.Transactions.TransactionStep";
        public const string INPUT_CONTROL_BASE_CLASS_NAME = "Gin.Controls.UserInputControl";
        public const string PLUGIN_EXTENSION = "*.dll";

        public List<ExternalCommand> Commands { get; private set; }
        public Type[] IncludedTypes { get; private set; }

        private List<Type> _includedTypes;

        private GinMetaData()
        {
            Commands = new List<ExternalCommand>();
            _includedTypes = new List<Type>();
            LoadGin();
        }

        private static GinMetaData _instance = null;
        private static object _lockObject = new object();

        public static GinMetaData GetInstance()
        {
            lock (_lockObject)
            {
                if (_instance == null)
                {
                    _instance = new GinMetaData();
                }
            }
            return _instance;
        }

        private void LoadGin()
        {
            try
            {
                Assembly assembly = Assembly.GetAssembly(typeof(Command));
                LoadCommandsFrom(assembly);
                LoadIncludedTypesFrom(assembly);
                IncludeGinCustomTypes();
            }
            finally
            {
                IncludedTypes = _includedTypes.ToArray();
            }
        }

        private void LoadCommandsFrom(Assembly assembly)
        {
            foreach (Type type in assembly.GetExportedTypes())
            {
                if (ExternalCommand.ContainsCommand(type))
                {
                    ExternalCommand cmd = new ExternalCommand(type);
                    Commands.Add(cmd);
                    _includedTypes.Add(cmd.CommandType);
                }
            }
        }

        private void LoadIncludedTypesFrom(Assembly assembly)
        {
            foreach (Type type in assembly.GetExportedTypes())
            {
                bool includeThisType = false;
                if (type.BaseType != null)
                {
                    Type baseType = type.BaseType;
                    {
                        includeThisType |= baseType.FullName == TRANSACTION_STEP_BASE_CLASS_NAME;
                        includeThisType |= baseType.FullName == INPUT_CONTROL_BASE_CLASS_NAME;
                        includeThisType |= baseType.BaseType != null && baseType.BaseType.FullName == INPUT_CONTROL_BASE_CLASS_NAME;
                        includeThisType |= type.GetCustomAttributes(false).OfType<GinIncludeTypeAttribute>().Count() > 0;
                    }

                }
                if (includeThisType)
                {
                    _includedTypes.Add(type);
                }
            }
        }

        private void IncludeGinCustomTypes()
        {
            _includedTypes.Add(typeof(CustomInstallationParameter));
        }

        public void Plugin(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return;
            }
            try
            {
                string[] filePaths = Directory.GetFiles(folderPath, PLUGIN_EXTENSION);
                foreach (string filePath in filePaths)
                {
                    Assembly assembly = Assembly.LoadFrom(filePath);
                    LoadCommandsFrom(assembly);
                    LoadIncludedTypesFrom(assembly);
                }
            }
            finally
            {
                IncludedTypes = _includedTypes.ToArray();
            }
        }

        public ExternalCommand GetCommandByName(string name)
        {
            return Commands.FirstOrDefault(c => c.CommandType.Name == name);
        }

        public ExternalCommand[] GetAssumedCommands(string fileName)
        {
            List<ExternalCommand> list = new List<ExternalCommand>();

            foreach (ExternalCommand command in Commands)
            {
                if (command.Instance is ICanCreateFromFile)
                {
                    ExternalCommand item = command.Clone();
                    ICanCreateFromFile iCanCreate = (ICanCreateFromFile)item.Instance;
                    if (iCanCreate.IsAssumedCommand(fileName))
                    {
                        iCanCreate.InitFromFile(fileName);
                        list.Add(item);
                    }
                }
            }

            return list.ToArray();
        }
    }
}
