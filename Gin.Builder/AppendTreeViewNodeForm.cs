using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gin.Builder
{
    public partial class AppendTreeViewNodeForm : Form
    {
        public AppendTreeViewNodeForm()
        {
            InitializeComponent();
        }

        public AppendNodeType CustomDialogResult
        {
            get
            {
                if (radioCreateSequence.Checked)
                {
                    return AppendNodeType.Append;
                }
                else if (radioReplaceNode.Checked)
                {
                    return AppendNodeType.Replace;
                }
                return AppendNodeType.Nothing;
            }
        }
    }

    public enum AppendNodeType
    {
        Append,
        Replace,
        Nothing
    }
}
