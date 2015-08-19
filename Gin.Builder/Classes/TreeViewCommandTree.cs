using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gin.Commands;
using System.Reflection;
using System.Drawing;
using Gin.CommandTree;
using Gin.CommandTree;
using Gin.Commands;

namespace Gin.Builder
{
    public class TreeViewCommandTree: CommandTree, IDisposable
    {

        private TreeView _tree;
        private TreeNode _rootNode;

        private TreeNode _nodeWhereToDrop;

        private Cursor MyNoDropCursor;
        private Cursor MyNormalCursor;

        public TreeViewCommandTree(TreeView tree)
        {
            _tree = tree;
            _tree.SuspendLayout();
            _tree.Nodes.Clear();
            _rootNode = new TreeNode("Пакет", 2, 2);
            _tree.Nodes.Add(_rootNode);
            _tree.AllowDrop = true;

            _tree.AfterSelect += new TreeViewEventHandler(_tree_AfterSelect);
            _tree.DragEnter += new DragEventHandler(_tree_DragEnter);
            _tree.DragLeave += new EventHandler(_tree_DragLeave);
            _tree.DragOver += new DragEventHandler(_tree_DragOver);
            _tree.DragDrop += new DragEventHandler(_tree_DragDrop);
            _tree.KeyUp += new KeyEventHandler(_tree_KeyUp);
            _tree.MouseDown += new MouseEventHandler(_tree_MouseDown);
            _tree.MouseMove += new MouseEventHandler(_tree_MouseMove);
            _tree.MouseUp += new MouseEventHandler(_tree_MouseUp);
            _tree.QueryContinueDrag += new QueryContinueDragEventHandler(_tree_QueryContinueDrag);
            _tree.GiveFeedback += new GiveFeedbackEventHandler(_tree_GiveFeedback);
            TryInitCursors();

            _tree.ResumeLayout();
        }

        private void TryInitCursors()
        {
            try
            {
                MyNormalCursor = new Cursor("3dwarro.cur");
                MyNoDropCursor = new Cursor("3dwno.cur");
            }
            catch
            {
            }
        }

        void _tree_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && sender is TreeView)
            {
                TreeNode node = ((TreeView)sender).SelectedNode as TreeNode;
                if (node == null)
                {
                    return;
                }
                TreeNodeData data = (TreeNodeData)node.Tag;
                RemoveNodes(data);
            }
        }

        public override CommandTreeNode Root
        {
            get 
            {
                return new TreeViewTreeNode(_rootNode);
            }
        }

        public override void ExpandAll()
        {
            _tree.ExpandAll();
        }


        #region Drag&drop events

        private Rectangle dragBoxFromMouseDown;
        private TreeNode draggedTreeNode;
        private Point screenOffset;

        private void _tree_DragDrop(object sender, DragEventArgs e)
        {
            bool needRebuild = false;
            if (e.Data.GetDataPresent(typeof(ExternalCommand)))
            {
                ExternalCommand command = (ExternalCommand)e.Data.GetData(typeof(ExternalCommand));

                if (e.Effect == DragDropEffects.Move)
                {
                    if (_nodeWhereToDrop != null)
                    {
                        Command newInstance = command.Clone().Instance;
                        InsertCommandIntoTree((TreeNodeData)_nodeWhereToDrop.Tag, newInstance);
                        needRebuild = true;
                    }
                }
            }
            if (e.Data.GetDataPresent(typeof(TreeNodeData)))
            {
                TreeNodeData data = (TreeNodeData)e.Data.GetData(typeof(TreeNodeData));

                if (e.Effect == DragDropEffects.Move || e.Effect == DragDropEffects.Copy)
                {
                    if (_nodeWhereToDrop != null)
                    {
                        Command cmd = data.Command;
                        if (e.Effect == DragDropEffects.Copy)
                        {
                            cmd = data.Command.Copy();
                        }
                        ExternalCommand command = new ExternalCommand(cmd);
                        TreeNodeData dataWhereToDrop = (TreeNodeData)_nodeWhereToDrop.Tag;
                        if (data.Node.HasNested(dataWhereToDrop.Node))
                        {
                            return;
                        }
                        bool commandMoved = InsertCommandIntoTree(dataWhereToDrop, command.Instance);
                        needRebuild = commandMoved;
                        if (commandMoved && e.Effect == DragDropEffects.Move)
                        {
                            data.Node.Remove();
                        }
                    }
                }
            }
            if (e.Data.GetDataPresent("FileDrop"))
            {
                if (e.Effect == DragDropEffects.Move)
                {
                    if (_nodeWhereToDrop != null)
                    {
                        string[] fileNames = (string[])e.Data.GetData("FileDrop");
                        GinMetaData metadata = GinMetaData.GetInstance();
                        bool applyToAllCommands = false;
                        ExternalCommand selectedCommand = null;
                        int lastIndex = -1;
                        for (int i = 0; i < fileNames.Length; i++)
                        {
                            string fileName = fileNames[i];
                            ExternalCommand[] commands = metadata.GetAssumedCommands(fileName);
                            if (commands.Length == 0)
                            {
                                return;
                            }
                            if (commands.Length == 1)
                            {
                                InsertCommandIntoTree((TreeNodeData)_nodeWhereToDrop.Tag, commands[0].Instance);
                                needRebuild = true;
                                return;
                            }
                            if (commands.Length > 1)
                            {
                                ChooseDroppedCommandForm form = new ChooseDroppedCommandForm();
                                form.InitDroppedCommands(commands);
                                if (form.ShowDialog() == DialogResult.OK)
                                {
                                    InsertCommandIntoTree((TreeNodeData)_nodeWhereToDrop.Tag, form.SelectedExternalCommand.Instance);
                                    needRebuild = true;
                                    applyToAllCommands = form.ApplyToAllCommands;
                                    if (applyToAllCommands)
                                    {
                                        selectedCommand = form.SelectedExternalCommand;
                                        lastIndex = i;
                                        break;
                                    }
                                }
                            }
                        }
                        if (applyToAllCommands)
                        {
                            for (int i = lastIndex + 1; i < fileNames.Length; i++)
                            {
                                string fileName = fileNames[i];
                                InsertCommandIntoTree((TreeNodeData)_nodeWhereToDrop.Tag, selectedCommand.Instance);
                                needRebuild = true;
                            }
                        }
                    }
                }
            }
            if (needRebuild)
            {
                ((TreeNodeData)_rootNode.Nodes[0].Tag).Node.Rebuild(true);
            }
        }

        private void _tree_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ExternalCommand)) 
                && !e.Data.GetDataPresent(typeof(TreeNodeData)) 
                && !e.Data.GetFormats().Contains("FileDrop"))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

           /* TreeNodeData d = (TreeNodeData)e.Data.GetData(typeof(TreeNodeData));
            if (d != null && (d.Property != null || d.Node.HasChilds))
            {
                e.Effect = DragDropEffects.None;
                return;
            }*/

            if ((e.KeyState & (8 + 32)) == (8 + 32) && (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // KeyState 8 + 32 = CTL + ALT
                // Link drag-and-drop effect.
                e.Effect = DragDropEffects.Copy;
            }
            else if ((e.KeyState & 32) == 32 && (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // ALT KeyState for link.
                e.Effect = DragDropEffects.Copy;
            }
            else if ((e.KeyState & 4) == 4 && (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // SHIFT KeyState for move.
                e.Effect = DragDropEffects.Move;
            }
            else if ((e.KeyState & 8) == 8 && (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                // CTL KeyState for copy.
                e.Effect = DragDropEffects.Copy;
            }
            else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // By default, the drop action should be move, if allowed.
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

            _nodeWhereToDrop = _tree.GetNodeAt(_tree.PointToClient(new Point(e.X, e.Y)));

            if (_nodeWhereToDrop != null)
            {
                // сообщаем что будем сбрасывать здесь
            }
        }

        private void _tree_DragLeave(object sender, EventArgs e)
        {
            // do nothing?
        }

        private void _tree_DragEnter(object sender, DragEventArgs e)
        {
            // do nothing?
        }

        private void _tree_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                draggedTreeNode = _tree.GetNodeAt(e.X, e.Y);
                if (draggedTreeNode != null)
                {
                    Size dragSize = SystemInformation.DragSize;
                    dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
                }
                else
                {
                    dragBoxFromMouseDown = Rectangle.Empty;
                }
            }
        }

        private void _tree_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    screenOffset = SystemInformation.WorkingArea.Location;
                    TreeNodeData data = (TreeNodeData)draggedTreeNode.Tag;
                    if (data != null && data.Property == null)
                    {
                        DragDropEffects dropEffect = _tree.DoDragDrop(data, DragDropEffects.All | DragDropEffects.Link);
                    }
                }
            }
        }

        private void _tree_MouseUp(object sender, MouseEventArgs e)
        {
            dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void _tree_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            TreeView treeView = sender as TreeView;
            if (treeView != null)
            {
                Form form = treeView.FindForm();

                if (((Control.MousePosition.X - screenOffset.X) < form.DesktopBounds.Left) || ((Control.MousePosition.X - screenOffset.X) > form.DesktopBounds.Right) || ((Control.MousePosition.Y - screenOffset.Y) < form.DesktopBounds.Top) || ((Control.MousePosition.Y - screenOffset.Y) > form.DesktopBounds.Bottom))
                {
                    e.Action = DragAction.Cancel;
                }
            }
        }

        private void _tree_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (MyNoDropCursor != null && MyNormalCursor != null)
            {
                e.UseDefaultCursors = false;
                if ((e.Effect & DragDropEffects.Move) == DragDropEffects.Move)
                {
                    Cursor.Current = MyNormalCursor;
                }
                else
                {
                    Cursor.Current = MyNoDropCursor;
                }
            }
        }

        #endregion

        #region other events

        private void _tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is TreeNodeData)
            {
                TreeNodeData data = (TreeNodeData)e.Node.Tag;
                SelectCommandTreeNodeHandler(data.Node, _packageBody, data.Command, data.Property);
            }
            else
            {
                SelectCommandTreeNodeHandler(null, _packageBody, null, null);
            }
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            _tree.AfterSelect -= new TreeViewEventHandler(_tree_AfterSelect);
            _tree.DragEnter -= new DragEventHandler(_tree_DragEnter);
            _tree.DragLeave -= new EventHandler(_tree_DragLeave);
            _tree.DragOver -= new DragEventHandler(_tree_DragOver);
            _tree.DragDrop -= new DragEventHandler(_tree_DragDrop);
            _tree.KeyUp -= new KeyEventHandler(_tree_KeyUp);
            _tree.MouseDown -= new MouseEventHandler(_tree_MouseDown);
            _tree.MouseMove -= new MouseEventHandler(_tree_MouseMove);
            _tree.MouseUp -= new MouseEventHandler(_tree_MouseUp);
            _tree.QueryContinueDrag -= new QueryContinueDragEventHandler(_tree_QueryContinueDrag);
            _tree.GiveFeedback -= new GiveFeedbackEventHandler(_tree_GiveFeedback);
        }

        #endregion
    }
}
