using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gin.SQL.Util;
using Microsoft.Win32;  
using Avicomp.Installer;
using System.Data.SqlClient;
using Gin;

namespace Avicomp.Installer
{
    public partial class SQLOSBBConnectionControl : UserControl
    {

        private ExecutionContext _context;

        public SQLOSBBConnectionControl(ExecutionContext context)
        {
            _context = context;
            InitializeComponent();
            LoadInstanceList();
        }

        public OSBBInstanceConfig Value
        {
            get
            {
                OSBBInstanceConfig result = null;
                Gin.Controls.UserControlUtil.ExecuteOrInvoke(new Action(() =>
                    {
                        result = (OSBBInstanceConfig)comboInstanceName.SelectedItem;
                        result.Password = textPassword.Text;
                        result.UserName = textUserName.Text;
                        result.CreateBackup = checkCreateBackup.Checked;
                    }), this);
                return result;
            }
            set
            {
                if (value != null)
                {
                    Gin.Controls.UserControlUtil.ExecuteOrInvoke(new Action(() =>
                    {
                        RefreshControls(value);
                    }), this);
                }
            }
        }

        private void SQLConnectionControl_Resize(object sender, EventArgs e)
        {
            const int POSITION_1 = 170;
            const int RIGHT_PADDING_1 = 7;
            comboInstanceName.Width = this.Width - POSITION_1 - RIGHT_PADDING_1;
            panelSqlAuthentication.Width = this.Width - POSITION_1 - RIGHT_PADDING_1;

            comboInstanceName.Left = POSITION_1;
            panelSqlAuthentication.Left = POSITION_1;
        }

        private void LoadInstanceList()
        {
            try
            {
                const string OSBB_REGISTRY_ROOT = @"SOFTWARE\AviComp Services\OSBB\OSBBInstances";
                RegistryKey baseKey = Registry.LocalMachine;
                RegistryKey key = baseKey.OpenSubKey(OSBB_REGISTRY_ROOT);
                if (key == null)
                {
                    return;
                }
                string[] instances = key.GetSubKeyNames();
                List<OSBBInstanceConfig> configs = new List<OSBBInstanceConfig>();
                foreach (string instanceAlias in instances)
                {
                    OSBBInstanceConfig config = new OSBBInstanceConfig()
                    {
                        Alias = instanceAlias
                    };
                    configs.Add(config);
                }
                comboInstanceName.DataSource = configs;
            }
            catch
            {
                _context.Log.AddLogError("Не удалось прочитать из реестра список экземпляров OSBB-серверов. Возможно, у Вас нет доступа для чтения ветки реестра или ветка реестра не создана при установке ППО.");
            }
        }

        private void comboInstanceName_SelectedIndexChanged(object sender, EventArgs e)
        {
            OSBBInstanceConfig config = (OSBBInstanceConfig)comboInstanceName.SelectedItem;
            RefreshControls(config);
        }

        private void RefreshControls(OSBBInstanceConfig config)
        {
            comboInstanceName.SelectedItem = config;
            textPassword.Text = config.Password;
            textUserName.Text = config.UserName;
            checkCreateBackup.Checked = config.CreateBackup;
        }
    }
}
