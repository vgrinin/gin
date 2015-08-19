using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Attributes;
using Gin.Commands;
using System.Reflection;
using System.Windows.Forms;
using Gin.CommandTree;
using System.Collections;
using Gin.Attributes;
using Gin.CommandTree;
using Gin.Commands;

namespace Gin.Builder
{

    public delegate void SelectCommandTreeNodeDelegate(CommandTreeNode node, PackageBody body, Command command, PropertyInfo property);

    public abstract class CommandTree
    {

        protected PackageBody _packageBody;

        public abstract CommandTreeNode Root { get; }

        public PackageBody Body 
        {
            get
            {
                Rebuild();
                CommandTreeNode _root = Root.Childs.FirstOrDefault();
                if (_root != null)
                {
                    _packageBody.Command = _root.Data.Command;
                }
                return _packageBody;
            }
        }

        public abstract void ExpandAll();

        public event SelectCommandTreeNodeDelegate SelectCommandTreeNode;

        protected void SelectCommandTreeNodeHandler(CommandTreeNode node, PackageBody body, Command command, PropertyInfo property)
        {
            if (SelectCommandTreeNode != null)
            {
                SelectCommandTreeNode(node, body, command, property);
            }
        }

        public void LoadTree(PackageBody body)
        {
            CommandTreeNode node = Root;
            _packageBody = body;
            FillTreeNode(node, body.Command, null);
        }

        public void Rebuild()
        {
            CommandTreeNode _root = Root.Childs.FirstOrDefault();
            if (_root != null)
            {
                _root.Rebuild(true);
            }
        }

        #region Логика оперирования с узлами

        protected void RemoveNodes(TreeNodeData currentNodeData)
        {
            if (IsCommand(currentNodeData))
            {
                if (MessageBox.Show("Удалить команду <" + currentNodeData.CommandAttribute.Name + ">?", "Вы уверены?", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    currentNodeData.Node.Remove();
                }
                return;
            }
            if (IsProperty(currentNodeData) && currentNodeData.Node.HasChilds)
            {
                if (MessageBox.Show("Удалить все команды, вложенные в <" + currentNodeData.CommandAttribute.Name + ": " + currentNodeData.PropertyAttribute.Name + ">?", "Вы уверены?", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    currentNodeData.Node.ClearChilds();
                }
                return;
            }
        }

        protected bool InsertCommandIntoTree(TreeNodeData currentNodeData, Command newCommand)
        {
            if (IsProperty(currentNodeData))
            {
                if (IsNotAcceptableType(currentNodeData, newCommand))
                {
                    return false;
                }
                return InsertCommandIntoPropertyNode(currentNodeData, currentNodeData.Property, newCommand);
            }
            else
            {
                if (IsCommand(currentNodeData))
                {
                    CommandTreeNode parentNode = currentNodeData.Node.Parent;
                    if (parentNode == null)
                    {
                        return false;
                    }
                    if (IsNotAcceptableType(parentNode.Data, newCommand))
                    {
                        return false;
                    }
                    PropertyInfo property;
                    int nestedCommandsCount = NestedCommandsCount(currentNodeData, out property);
                    if (nestedCommandsCount == 1)
                    {
                        return InsertCommandIntoPropertyNode(currentNodeData, property, newCommand);
                    }
                    else
                    {
                        return InsertCommandIntoParentProperty(currentNodeData, newCommand);
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        private bool IsNotAcceptableType(TreeNodeData currentNodeData, Command newCommand)
        {
            bool accepted = false;
            if (currentNodeData.AcceptedTypes != null && currentNodeData.AcceptedTypes.Count() > 0)
            {
                foreach (var type in currentNodeData.AcceptedTypes)
                {
                    accepted |= type.IsInstanceOfType(newCommand);
                }
            }
            else
            {
                accepted |= true;
            }

            if (currentNodeData.NotAcceptedTypes != null && currentNodeData.NotAcceptedTypes.Count() > 0)
            {
                foreach (var type in currentNodeData.NotAcceptedTypes)
                {
                    accepted &= !type.IsInstanceOfType(newCommand);
                }
            }
            else
            {
                accepted &= true;
            }
            return !accepted;
        }

        private bool InsertCommandIntoParentProperty(TreeNodeData currentNodeData, Command newCommand)
        {
            //CommandTreeNode parent = currentNodeData.Node.Parent;
            //var commandAttr = newCommand.GetType().GetCustomAttributes(false).OfType<GinNameAttribute>().FirstOrDefault();
            //parent.InsertAfter(newCommand, commandAttr, currentNodeData.Node);

            CommandTreeNode parent = currentNodeData.Node.Parent;
            AppendCommandAfter(parent, newCommand, currentNodeData.Node);


            //CommandTreeNode parent = currentNodeData.Node.Parent;
            //return InsertCommandIntoPropertyNode(parent.Data, parent.Data.Property, newCommand);

            return true;
        }

        private bool InsertCommandIntoPropertyNode(TreeNodeData currentNodeData, PropertyInfo property, Command newCommand)
        {
            if (IsEmptyProperty(currentNodeData, property))
            {
                return ReplaceCommandIntoProperty(currentNodeData, newCommand);
            }
            if (IsEnumerableProperty(currentNodeData, property))
            {
                CommandTreeNode node = currentNodeData.Node;
                if (currentNodeData.Property == null || currentNodeData.Property != property)
                {
                    IEnumerable<CommandTreeNode> childs = currentNodeData.Node.Childs;
                    node = childs.Where(c => c.Data.Property == property).FirstOrDefault();
                }
                AppendCommandAfter(node, newCommand, null);
                return true;
            }
            else
            {
                AppendNodeType appendType = AskUserToDo();
                switch (appendType)
                {
                    case AppendNodeType.Append:
                        CommandTreeNode sequenceTreeNode = IntermediateWithSequence(currentNodeData);
                        AppendCommandAfter(sequenceTreeNode, newCommand, null);
                        return true;
                    case AppendNodeType.Replace:
                        return ReplaceCommandIntoProperty(currentNodeData, newCommand);
                }
            }
            return false;
        }

        private CommandTreeNode AppendCommandAfter(CommandTreeNode sequenceTreeNode, Command command, CommandTreeNode nodeAfter)
        {
            return FillTreeNode(sequenceTreeNode, command, nodeAfter);
        }

        private bool ReplaceCommandIntoProperty(TreeNodeData currentNodeData, Command newCommand)
        {
            DialogResult dialogResult = DialogResult.OK;
            if (!IsEmptyProperty(currentNodeData, currentNodeData.Property))
            {
                dialogResult = MessageBox.Show("Заменить?", "Заменить?", MessageBoxButtons.OKCancel);
            }
            if (dialogResult == DialogResult.OK)
            {
                currentNodeData.Node.ClearChilds();
                AppendCommandAfter(currentNodeData.Node, newCommand, null);
                return true;
            }
            return false;
        }

        private CommandTreeNode IntermediateWithSequence(TreeNodeData currentNodeData)
        {
            CommandTreeNode sequenceCommandsTreeNode = null;
            GinMetaData metaData = GinMetaData.GetInstance();
            ExternalCommand sequence = metaData.Commands.Where(c => c.Instance is CommandSequence).FirstOrDefault();
            if (sequence != null)
            {
                IEnumerable<CommandTreeNode> removedNodes = currentNodeData.Node.Childs;
                currentNodeData.Node.ClearChilds();
                CommandTreeNode sequenceTreeNode = AppendCommandAfter(currentNodeData.Node, sequence.Instance, null);
                sequenceCommandsTreeNode = sequenceTreeNode.Childs.FirstOrDefault();
                foreach (CommandTreeNode node in removedNodes)
                {
                    AppendCommandAfter(sequenceCommandsTreeNode, node.Data.Command, null);
                }
            }
            return sequenceCommandsTreeNode;
        }

        private AppendNodeType AskUserToDo()
        {
            AppendNodeType appendType = AppendNodeType.Nothing;
            AppendTreeViewNodeForm form = new AppendTreeViewNodeForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                appendType = form.CustomDialogResult;
            }
            return appendType;
        }

        protected bool IsProperty(TreeNodeData currentNodeData)
        {
            return (currentNodeData != null) && (currentNodeData.Command != null) && (currentNodeData.Property != null);
        }

        protected bool IsCommand(TreeNodeData currentNodeData)
        {
            return (currentNodeData != null) && (currentNodeData.Command != null) && (currentNodeData.Property == null);
        }

        protected bool IsEnumerableProperty(TreeNodeData currentNodeData, PropertyInfo property)
        {
            GinArgumentCommandAttribute attr = (GinArgumentCommandAttribute)property.GetCustomAttributes(typeof(GinArgumentCommandAttribute), false).FirstOrDefault();
            bool isEnumerable = attr != null && attr.IsEnumerable;
            return isEnumerable;
        }

        private int NestedCommandsCount(TreeNodeData currentNodeData, out PropertyInfo searchProperty)
        {
            Type commandType = currentNodeData.Command.GetType();
            int nestedCommandsCount = 0;
            searchProperty = null;
            foreach (var p in commandType.GetProperties())
            {
                bool isNestedCommand = p.GetCustomAttributes(typeof(GinArgumentCommandAttribute), false).FirstOrDefault() != null;
                if (isNestedCommand)
                {
                    nestedCommandsCount++;
                    searchProperty = p;
                }
            }
            if (nestedCommandsCount > 1)
            {
                searchProperty = null;
            }
            return nestedCommandsCount;
        }

        protected bool IsEmptyProperty(TreeNodeData currentNodeData, PropertyInfo property)
        {
            return !currentNodeData.Node.HasChilds;
        }

        private CommandTreeNode FillTreeNode(CommandTreeNode node, Command command, CommandTreeNode nodeAfter)
        {
            Type type = command.GetType();
            var commandAttr = type.GetCustomAttributes(false).OfType<GinNameAttribute>().FirstOrDefault();
            CommandTreeNode commandNode;
            if (nodeAfter == null)
            {
                commandNode = node.AppendChild(command, commandAttr);
            }
            else
            {
                commandNode = node.InsertAfter(command, commandAttr, nodeAfter);
            }
            var properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var propertyAttr = (GinArgumentCommandAttribute)property.GetCustomAttributes(typeof(GinArgumentCommandAttribute), false).FirstOrDefault();
                if (propertyAttr != null)
                {
                    CommandTreeNode argumentNode = commandNode.AppendChild(command, commandAttr, property, propertyAttr);
                    object nestedCommand = property.GetValue(command, null);
                    if (nestedCommand != null)
                    {
                        Type nestedType = nestedCommand.GetType();
                        bool isEnumerable = propertyAttr.IsEnumerable;
                        if (isEnumerable)
                        {
                            IEnumerable iEnum = (IEnumerable)nestedCommand;
                            foreach (var item in iEnum)
                            {
                                FillTreeNode(argumentNode, (Command)item, null);
                            }
                        }
                        else
                        {
                            FillTreeNode(argumentNode, (Command)nestedCommand, null);
                        }
                    }
                }
            }
            return commandNode;
        }

        #endregion

    }
}
