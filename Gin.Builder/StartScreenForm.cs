using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Gin.Builder
{
    public partial class StartScreenForm : Form
    {
        AutoResetEvent _cancelEvent;

        public StartScreenForm()
        {
            InitializeComponent();
        }

        public StartScreenForm(AutoResetEvent cancelEvent, string message)
        {
            _cancelEvent = cancelEvent;
            InitializeComponent();
            panelCancel.Visible = cancelEvent != null;
            labelMessage.Text = message;
        }

        private void linkLabelCancel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_cancelEvent != null)
            {
                _cancelEvent.Set();
            }
        }
    }
}
