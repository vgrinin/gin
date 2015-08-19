using System;
using System.Collections.Generic;
using System.Reflection;

namespace Gin.Commands
{

    public interface IMultipleContentCommand
    {
        IEnumerable<ContentPath> ContentPaths { get; }
    }


    public class ContentPath
    {
        private readonly Command _command;
        private readonly PropertyInfo _property;

        public ContentPath(Command command, PropertyInfo property)
        {
            if (!(command is IMultipleContentCommand))
            {
                throw new ArgumentException("Must be the IContentCommand");
            }
            _command = command;
            _property = property;
        }

        public string Name
        {
            get
            {
                return _property.Name;
            }
        }

        public string Value
        {
            get
            {
                return (string)_property.GetValue(_command, null);
            }

            set
            {
                _property.SetValue(_command, value, null);
            }
        }
    }
}
