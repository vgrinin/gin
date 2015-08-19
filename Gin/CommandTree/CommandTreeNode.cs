using System.Collections.Generic;
using Gin.Attributes;
using Gin.Commands;
using System.Reflection;

namespace Gin.CommandTree
{
    public abstract class CommandTreeNode
    {
        public abstract CommandTreeNode AppendChild(Command command, GinNameAttribute commandAttribute);
        public abstract CommandTreeNode InsertAfter(Command command, GinNameAttribute commandAttribute, CommandTreeNode nodeAfter);
        public abstract CommandTreeNode AppendChild(Command command, GinNameAttribute commandAttribute, PropertyInfo property, GinArgumentAttribute propertyAttribute);
        public abstract void ClearChilds();
        public abstract bool HasChilds { get; }
        public abstract IEnumerable<CommandTreeNode> Childs { get; }
        public abstract CommandTreeNode Parent { get; }
        public abstract TreeNodeData Data { get; }
        public abstract void Rebuild(bool recursive);
        public abstract void Remove();
        public abstract string NodeName { get; set; }
        public abstract bool HasNested(CommandTreeNode node);
    }
}
