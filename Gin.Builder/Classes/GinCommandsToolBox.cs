using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gin;
using System.Drawing;

namespace Gin.Builder
{
    public class GinCommandsToolBox: ListView
    {

        private const string DEFAULT_GROUP_NAME = "Общая";

        private Rectangle dragBoxFromMouseDown;
        private Cursor MyNoDropCursor;
        private Cursor MyNormalCursor;
        private Point screenOffset;
        private ListViewItem draggedListItem;

        public GinCommandsToolBox(): base()
        {
            // навешиваем события для drag&drop функциональности
            this.MouseDown += new MouseEventHandler(GinCommandsToolBox_MouseDown);
            this.MouseMove += new MouseEventHandler(GinCommandsToolBox_MouseMove);
            this.MouseUp += new MouseEventHandler(GinCommandsToolBox_MouseUp);
            this.QueryContinueDrag += new QueryContinueDragEventHandler(GinCommandsToolBox_QueryContinueDrag);
            this.GiveFeedback += new GiveFeedbackEventHandler(GinCommandsToolBox_GiveFeedback);

            TryInitCursors();
        }

        void GinCommandsToolBox_GiveFeedback(object sender, GiveFeedbackEventArgs e)
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

        void GinCommandsToolBox_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView != null)
            {
                Form form = listView.FindForm();

                if (((Control.MousePosition.X - screenOffset.X) < form.DesktopBounds.Left) || ((Control.MousePosition.X - screenOffset.X) > form.DesktopBounds.Right) || ((Control.MousePosition.Y - screenOffset.Y) < form.DesktopBounds.Top) || ((Control.MousePosition.Y - screenOffset.Y) > form.DesktopBounds.Bottom))
                {
                    e.Action = DragAction.Cancel;
                }
            }
        }

        void GinCommandsToolBox_MouseUp(object sender, MouseEventArgs e)
        {
            dragBoxFromMouseDown = Rectangle.Empty;
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

        void GinCommandsToolBox_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    screenOffset = SystemInformation.WorkingArea.Location;
                    ExternalCommand command = (ExternalCommand)draggedListItem.Tag;
                    if (command != null)
                    {
                        DragDropEffects dropEffect = this.DoDragDrop(command, DragDropEffects.All | DragDropEffects.Link);
                    }
                }
            }
        }

        void GinCommandsToolBox_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                draggedListItem = this.GetItemAt(e.X, e.Y);
                if (draggedListItem != null)
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

        public void InitFromCommands(IEnumerable<ExternalCommand> commands)
        {
            this.Items.Clear();
            this.Groups.Clear();

            ListViewGroup groupDefault = this.Groups.Add(DEFAULT_GROUP_NAME, DEFAULT_GROUP_NAME);

            foreach (ExternalCommand command in commands)
            {
                ListViewGroup currentGroup = groupDefault;
                if (!String.IsNullOrEmpty(command.Metadata.Group))
                {
                    foreach (ListViewGroup group in this.Groups)
                    {
                        if (command.Metadata.Group == group.Header)
                        {
                            currentGroup = group;
                            break;
                        }
                    }
                    if (currentGroup == groupDefault)
                    {
                        currentGroup = this.Groups.Add(command.Metadata.Group, command.Metadata.Group);
                    }
                }

                ListViewItem newItem = new ListViewItem(currentGroup);
                newItem.Text = command.Metadata.Name;
                newItem.Tag = command;
                newItem.ImageIndex = 0;
                if (!String.IsNullOrEmpty(command.Metadata.Desription))
                {
                    newItem.ToolTipText = command.Metadata.Desription;
                }
                this.Items.Add(newItem);
            }
        }

        ~GinCommandsToolBox()
        {
            if (MyNormalCursor != null)
            {
                MyNormalCursor.Dispose();
            }
            if (MyNoDropCursor != null)
            {
                MyNoDropCursor.Dispose();
            }
        }
    }
}
