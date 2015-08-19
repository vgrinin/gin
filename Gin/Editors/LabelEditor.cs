using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gin.Editors
{
    public partial class LabelEditor : UserControl, IEditor
    {
        public LabelEditor()
        {
            InitializeComponent();
        }

        public LabelEditor(string value)
        {
            InitializeComponent();
            labelText.Text = value;
        }

        public object Value
        {
            get
            {
                return labelText.Text;
            }
            set
            {
                labelText.Text = (string)value;
            }
        }

    }
}
