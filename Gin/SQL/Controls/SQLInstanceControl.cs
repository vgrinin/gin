using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gin.Context;

namespace Gin.SQL.Controls
{
    public partial class SQLInstanceControl : UserControl
    {

        private IExecutionContext _context;

        public SQLInstanceControl(IExecutionContext context)
        {
            _context = context;
            InitializeComponent();
        }

        public ICSQLInstance Value
        {
            get
            {
                ICSQLInstance result = null;
                Gin.Controls.UserControlUtil.ExecuteOrInvoke(new Action(() =>
                    {
                        result = new ICSQLInstance()
                        {
                            InstallNewInstance = radioCreateInstance.Checked,
                            InstanceName = !radioCreateInstance.Checked ? comboInstanceName.Text : textInstanceName.Text,
                            SqlDataDirectory = radioCreateInstance.Checked ? textSqlDirectory.Text : ""
                        };
                }), this);
                return result;
            }
            set
            {
                if(value == null)
                {
                    return;
                }
                Gin.Controls.UserControlUtil.ExecuteOrInvoke(new Action(() =>
                {
                    radioCreateInstance.Checked = value.InstallNewInstance;
                    radioUseExistingInstance.Checked = !value.InstallNewInstance;
                    panelChooseServer.Enabled = !value.InstallNewInstance;
                    panelCreateServer.Enabled = value.InstallNewInstance;
                    if (value.InstallNewInstance)
                    {
                        textInstanceName.Text = value.InstanceName;
                        textSqlDirectory.Text = value.SqlDataDirectory;
                    }
                    else
                    {
                        comboInstanceName.Text = value.InstanceName;
                    }
                }), this);
            }
        }

        private void radioUseExistingInstance_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            bool useExistingInstance = rb.Checked;
            if (panelChooseServer != null)
            {
                panelChooseServer.Enabled = useExistingInstance;
                panelCreateServer.Enabled = !useExistingInstance;
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                textSqlDirectory.Text = dialog.SelectedPath;
            }
        }
    }

}
