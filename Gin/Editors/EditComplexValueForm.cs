using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gin.Commands;



namespace Gin.Editors
{
    public partial class EditComplexValueForm : Form
    {

        public EditComplexValueForm()
        {
            InitializeComponent();
        }

        private void propertiesPanel_Resize(object sender, EventArgs e)
        {
            AdjustControlsSize();
        }

        private void AdjustControlsSize()
        {
            const int VSCROLLER_WIDTH = 17;

            foreach (Control control in propertiesPanel.Controls)
            {
                control.Width = propertiesPanel.Width - control.Padding.Left - control.Padding.Right - VSCROLLER_WIDTH;
            }
        }

        public void InitEditors(object initialValue, PackageBody body)
        {
            FormsHelper.CreateNodeEditor(null, initialValue, propertiesPanel, body,
                new PropertyHelpCallback((progName, argName, argDescr, allowTemplates, editor) =>
                {
                    argumentHelpControl.SetHelp(progName, argName, argDescr, allowTemplates, editor, body.GetResultInfos());
                }));
            AdjustControlsSize();
        }

        public object ReadEditorValue(Type valueType)
        {
            return FormsHelper.GetEditorValue(valueType, propertiesPanel);
        }
    }
}
