using System;
using Gin.Attributes;
using Gin.Commands;
using System.Reflection;

namespace Gin.CommandTree
{
    public class TreeNodeData
    {
        public Command Command;
        public GinNameAttribute CommandAttribute;
        public PropertyInfo Property;
        public GinArgumentAttribute PropertyAttribute;
        public CommandTreeNode Node;
        public Type[] AcceptedTypes;
        public Type[] NotAcceptedTypes;
    }
}