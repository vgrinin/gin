using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gin.Attributes;
using Gin.Commands;
using System.Reflection;
using Gin.CommandTree;
using Gin.Attributes;
using Gin.CommandTree;
using Gin.Commands;


namespace Gin.Builder
{

    public class TreeViewTreeNode: CommandTreeNode
    {

        protected TreeNode _node;

        public override string NodeName 
        {
            get
            {
                return _node.Text;
            }
            set
            {
                _node.Text = value;
            }
        }

        public override bool HasNested(CommandTreeNode node)
        {
            const int MAX_NESTED_LEVELS_SEARCH = 20;
            int nestedLevel = MAX_NESTED_LEVELS_SEARCH;
            CommandTreeNode currentNode = node.Parent;
            while (nestedLevel > 0 && currentNode != null)
            {
                if (currentNode == this)
                {
                    return true;
                }
                currentNode = currentNode.Parent;
            }
            return false;
        }

        public TreeViewTreeNode(TreeNode node)
        {
            _node = node;
        }

        private string GetCommandName(Command command, GinNameAttribute commandAttribute)
        {
            return command.GetHumanReadableName();
        }

        public override CommandTreeNode AppendChild(Command command, GinNameAttribute commandAttribute)
        {
            TreeNode node = new TreeNode()
            {
                Text = GetCommandName(command, commandAttribute),
                ToolTipText = commandAttribute.Description,
                ImageIndex = 0,
                Tag = new TreeNodeData()
                {
                    Command = command,
                    CommandAttribute = commandAttribute,
                    Property = null,
                    PropertyAttribute = null,
                    AcceptedTypes = null,
                    NotAcceptedTypes = null
                }
            };
            _node.Nodes.Add(node);
            TreeViewTreeNode treeNode = new TreeViewTreeNode(node);
            ((TreeNodeData)node.Tag).Node = treeNode;
            return treeNode;
        }

        public override CommandTreeNode InsertAfter(Command command, GinNameAttribute commandAttribute, CommandTreeNode nodeAfter)
        {
            TreeNode node = new TreeNode()
            {
                Text = GetCommandName(command, commandAttribute),
                ToolTipText = commandAttribute.Description,
                ImageIndex = 0,
                Tag = new TreeNodeData()
                {
                    Command = command,
                    CommandAttribute = commandAttribute,
                    Property = null,
                    PropertyAttribute = null,
                    AcceptedTypes = null,
                    NotAcceptedTypes = null
                }
            };
            TreeViewTreeNode afterNode = (TreeViewTreeNode)nodeAfter;
            int index = _node.Nodes.IndexOf(afterNode._node);
            if (index >= 0)
            {
                _node.Nodes.Insert(index + 1, node);
            }
            else
            {
                _node.Nodes.Add(node);
            }
            TreeViewTreeNode treeNode = new TreeViewTreeNode(node);
            ((TreeNodeData)node.Tag).Node = treeNode;
            return treeNode;
        }

        public override CommandTreeNode AppendChild(Command command, GinNameAttribute commandAttribute, PropertyInfo property, GinArgumentAttribute propertyAttribute)
        {
            var acceptNot = property.GetCustomAttributes(typeof(GinArgumentCommandAcceptNotAttribute), false);
            List<Type> notAcceptedTypes = null;
            if (acceptNot != null)
            {
                notAcceptedTypes = new List<Type>();
                foreach (var item in acceptNot)
                {
                    notAcceptedTypes.Add(((GinArgumentCommandAcceptNotAttribute)item).NotAcceptedType);
                }
            }
            var acceptOnly = property.GetCustomAttributes(typeof(GinArgumentCommandAcceptOnlyAttribute), false);
            List<Type> acceptedTypes = null;
            if (acceptOnly != null)
            {
                acceptedTypes = new List<Type>();
                foreach (var item in acceptOnly)
                {
                    acceptedTypes.Add(((GinArgumentCommandAcceptOnlyAttribute)item).AcceptedType);
                }
            }
            TreeNode node = new TreeNode()
            {
                Text = propertyAttribute.Name,
                ToolTipText = propertyAttribute.Description,
                ImageIndex = 1,
                SelectedImageIndex = 1,
                Tag = new TreeNodeData()
                {
                    Command = command,
                    CommandAttribute = commandAttribute,
                    Property = property,
                    PropertyAttribute = propertyAttribute,
                    AcceptedTypes = acceptedTypes.ToArray(),
                    NotAcceptedTypes = notAcceptedTypes.ToArray()
                }
            };
            _node.Nodes.Add(node);
            TreeViewTreeNode treeNode = new TreeViewTreeNode(node);
            ((TreeNodeData)node.Tag).Node = treeNode;
            return treeNode;
        }

        public override void ClearChilds()
        {
            List<TreeNode> collectionCopy = new List<TreeNode>();
            foreach (TreeNode node in _node.Nodes)
            {
                collectionCopy.Add(node);
            }
            foreach (TreeNode node in collectionCopy)
            {
                TreeNodeData data = (TreeNodeData)node.Tag;
                TreeViewTreeNode currentNode = (TreeViewTreeNode)data.Node;
                currentNode._node.Remove();
            }
        }

        public override bool HasChilds
        {
            get
            {
                return _node.Nodes.Count > 0;
            }
        }

        public override IEnumerable<CommandTreeNode> Childs
        {
            get
            {
                List<CommandTreeNode> result = new List<CommandTreeNode>();
                foreach (TreeNode node in _node.Nodes)
                {
                    TreeNodeData data = (TreeNodeData)node.Tag;
                    TreeViewTreeNode currentNode = (TreeViewTreeNode)data.Node;
                    result.Add(currentNode);
                }
                return result;
            }
        }

        public override CommandTreeNode Parent 
        {
            get
            {
                TreeNode parentNode = _node.Parent;
                if (parentNode == null)
                {
                    return null;
                }
                TreeNodeData data = parentNode.Tag as TreeNodeData;
                if (data == null)
                {
                    return null;
                }
                return data.Node;
            }
        }

        public override TreeNodeData Data 
        {
            get
            {
                return (_node.Tag as TreeNodeData);
            }
        }

        public override void Remove()
        {
            _node.Remove();
        }

        public override void Rebuild(bool recursive)
        {
            TreeNodeData data = (TreeNodeData)_node.Tag;
            if (IsCommand(data))
            {
                ClearAllNestedCommands(data.Command);

                foreach (TreeNode nestedNode in _node.Nodes)
                {
                    data = (TreeNodeData)nestedNode.Tag;
                    if (IsProperty(data))
                    {
                        if (IsEnumerableProperty(data, data.Property))
                        {
                            IEnumerable<CommandTreeNode> nestedCommandNodes = data.Node.Childs;
                            
                            Type _listType = data.Property.PropertyType;
                            object commands = _listType.GetConstructor(new Type[0]).Invoke(null);
                            MethodInfo addMethod = _listType.GetMethod("Add");

                            foreach (var nestedCommandNode in nestedCommandNodes)
                            {
                                addMethod.Invoke(commands, new object[] { nestedCommandNode.Data.Command });
                                if (recursive)
                                {
                                    nestedCommandNode.Rebuild(recursive);
                                }
                            }
                            data.Property.SetValue(data.Command, commands, null);
                        }
                        else
                        {
                            if (data.Node.HasChilds)
                            {
                                CommandTreeNode nestedCommandNode = data.Node.Childs.FirstOrDefault();
                                Command nestedCommand = nestedCommandNode.Data.Command;
                                data.Property.SetValue(data.Command, nestedCommand, null);
                                if (recursive)
                                {
                                    nestedCommandNode.Rebuild(recursive);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ClearAllNestedCommands(Command command)
        {
            Type commandType = command.GetType();
            var properties = commandType.GetProperties();
            foreach (var property in properties)
            {
                bool isNestedCommand = property.GetCustomAttributes(typeof(GinArgumentCommandAttribute), false).Count() == 1;
                if (isNestedCommand)
                {
                    object defaultValue = null;
                    property.SetValue(command, defaultValue, null);
                }
            }
        }

        private bool IsProperty(TreeNodeData data)
        {
            return data != null && data.Command != null && data.Property != null;
        }

        private bool IsCommand(TreeNodeData data)
        {
            return data != null && data.Command != null && data.Property == null;
        }

        private bool IsEnumerableProperty(TreeNodeData data, PropertyInfo property)
        {
            GinArgumentCommandAttribute attr = (GinArgumentCommandAttribute)property.GetCustomAttributes(typeof(GinArgumentCommandAttribute), false).FirstOrDefault();
            bool isEnumerable = attr != null && attr.IsEnumerable;
            return isEnumerable;
        }
    }
}
