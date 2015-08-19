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
    public partial class ChooseDroppedCommandForm : Form
    {

        private ExternalCommand _selectedCommand = null;

        public ChooseDroppedCommandForm()
        {
            InitializeComponent();
        }

        public void InitDroppedCommands(ExternalCommand[] commands)
        {
            panelCommands.SuspendLayout();
            panelCommands.Controls.Clear();
            bool isFirstRadio = true;
            foreach (ExternalCommand command in commands)
            {
                RadioButton radio = new RadioButton();
                radio.Text = command.Metadata.Name;
                radio.Tag = command;
                radio.CheckedChanged += new EventHandler(radio_CheckedChanged);
                if (isFirstRadio)
                {
                    radio.Checked = true;
                    isFirstRadio = false;
                }
                panelCommands.Controls.Add(radio);
            }
            panelCommands.ResumeLayout();
            AdjustControlSize();
        }

        void radio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            if (radio.Checked)
            {
                _selectedCommand = (ExternalCommand)radio.Tag;
            }
        }

        public ExternalCommand SelectedExternalCommand 
        {
            get
            {
                return _selectedCommand;
            }
        }

        public bool ApplyToAllCommands 
        {
            get
            {
                return checkApplyAll.Checked;
            }
        }

        private void panelCommands_Resize(object sender, EventArgs e)
        {
            AdjustControlSize();
        }

        private void AdjustControlSize()
        {
            foreach (Control control in panelCommands.Controls)
            {
                control.Width = panelCommands.Width - control.Padding.Left - control.Padding.Right;
            }
        }
    }
}
