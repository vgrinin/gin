using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Gin.Attributes;
using Gin.Commands;
using System.Globalization;


namespace Gin
{
    
    public class CommandMetadata
    {
        public string Name { get; set; }
        public string Desription { get; set; }
        public string Group { get; set; }
    }

    public class ExternalCommand
    {
        public const string BASE_CLASS_NAME = "Gin.Commands.Command";
        public const string MESSAGE_PROPERTY_NOT_EXIST = "У типа {0} отсутствует свойство {1}";

        public Type CommandType { get; private set; }
        public Command Instance { get; private set; }
        public PropertyInfo[] Properties { get; private set; }
        public ConstructorInfo Constructor { get; private set; }
        public CommandMetadata Metadata { get; private set; }

        public override string ToString()
        {
            return Metadata.Name;
        }

        private ExternalCommand() { }

        public static bool ContainsCommand(Type type)
        {
            if (type.BaseType == null) return false;

            bool isInvalidInheritance = (type.BaseType.FullName != BASE_CLASS_NAME);
            if(isInvalidInheritance)
            {
                if (type.BaseType.BaseType == null) return false;
                isInvalidInheritance = (type.BaseType.BaseType.FullName != BASE_CLASS_NAME);
            }
            if (isInvalidInheritance) return false;

            if (type.IsAbstract)
            {
                return false;
            }

            bool ignoreType = type.GetCustomAttributes(false).OfType<GinIgnoreTypeAttribute>().Count() > 0;
            if (ignoreType)
            {
                return false;
            }

            ConstructorInfo _default = type.GetConstructor(new Type[0]);
            if (_default == null)
            {
                return false;
            }

            return true;
        }

        public ExternalCommand(Type type)
        {
            CommandType = type;
            Constructor = type.GetConstructor(new Type[0]);
            InitInstance();
            Properties = type.GetProperties();
            Metadata = GetCommandMetadata(type);
        }

        public ExternalCommand(Command command)
        {
            CommandType = command.GetType();
            Constructor = CommandType.GetConstructor(new Type[0]);
            Instance = command;
            Properties = CommandType.GetProperties();
            Metadata = GetCommandMetadata(CommandType);
        }

        private void InitInstance()
        {
            Instance = (Command)Constructor.Invoke(null);
        }

        private CommandMetadata GetCommandMetadata(Type command)
        {
            CommandMetadata result = new CommandMetadata()
            {
                Desription = command.Name,
                Name = command.Name
            };

            GinNameAttribute attr = command.GetCustomAttributes(true).OfType<GinNameAttribute>().FirstOrDefault();
            if (attr != null)
            {
                result.Desription = attr.Description;
                result.Name = attr.Name;
                result.Group = attr.Group;
            }
            return result;
        }

        public object GetProperty(string propertyName)
        {
            PropertyInfo property = GetPropertyInfo(propertyName);
            return property.GetValue(Instance, null);
        }

        public void SetProperty(string propertyName, object value)
        {
            PropertyInfo property = GetPropertyInfo(propertyName);
            property.SetValue(Instance, value, null);
        }

        private PropertyInfo GetPropertyInfo(string propertyName)
        { 
            PropertyInfo property = Properties.FirstOrDefault();
            if (property == null)
            {
                throw new ArgumentException(String.Format(MESSAGE_PROPERTY_NOT_EXIST, CommandType.Name, propertyName));
            }
            return property;
        }

        public ExternalCommand Clone()
        {
            return new ExternalCommand()
            {
                Constructor = this.Constructor,
                CommandType = this.CommandType,
                Properties = this.Properties,
                Metadata = this.Metadata,
                Instance = (Command)Constructor.Invoke(new object[0])
            };
        }
    }

}
