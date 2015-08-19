using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gin.Commands;

namespace Gin.Editors
{
    public partial class ArgumentHelpControl : UserControl
    {
        public ArgumentHelpControl()
        {
            InitializeComponent();
        }

        private void listContextNames_DoubleClick(object sender, EventArgs e)
        {
            Gin.Editors.ITemplatedEditor editor = (Gin.Editors.ITemplatedEditor)listContextNames.Tag;
            if (editor != null)
            {
                ResultInfo res = (ResultInfo)listContextNames.SelectedValue;
                string val = res.Name;
                if (val != null)
                {
                    editor.InsertAtCurrent("%" + val + "%");
                }
            }
        }

        public void SetHelp(string programName, string argName, string argDescr, bool allowTemplates, Gin.Editors.ITemplatedEditor editor, List<ResultInfo> resultInfos)
        {
            labelHelpArgumentDescription.Text = argDescr;
            labelHelpArgumentName.Text = argName + "(" + programName + ")";
            listContextNames.Visible = allowTemplates;
            listContextNames.DataSource = resultInfos;
            listContextNames.Tag = editor;
        }

        private void labelHelpArgumentName_Click(object sender, EventArgs e)
        {

        }

    }
}
