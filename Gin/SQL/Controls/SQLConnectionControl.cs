using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gin.Context;
using Gin.SQL.Util;
using System.Data.Sql;
using System.Data.SqlClient;

namespace Gin.SQL.Controls
{
    public partial class SQLConnectionControl : UserControl
    {

        private Control[] _controls = new Control[5];

        private bool _needRefreshDBS = true;
        private const string QUERY_DBS = @"SELECT [Name], 
       DbID 
FROM master..sysdatabases 
WHERE [Name] NOT IN ('master','tempdb','pubs','msdb','model') 
       and HAS_DBACCESS([Name]) = 1 ";

        private IExecutionContext _context;

        public SQLConnectionControl(IExecutionContext context)
        {
            InitializeComponent();
            _context = context;
            panelSqlAuthentication.Enabled = false;
            comboAuthentification.SelectedIndex = 0;
            _controls[(int)SQLConnectionSubControl.DBName] = comboDBName;
            _controls[(int)SQLConnectionSubControl.Password] = textPassword;
            _controls[(int)SQLConnectionSubControl.ServerName] = comboInstanceName;
            _controls[(int)SQLConnectionSubControl.SqlAuthentication] = comboAuthentification;
            _controls[(int)SQLConnectionSubControl.UserName] = textUserName;
        }

        private void comboAuthentification_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isSqlAuth = comboAuthentification.SelectedIndex == 0; ;
            panelSqlAuthentication.Enabled = isSqlAuth;
        }

        public SQLConnectionProperties Value
        {
            get
            {
                SQLConnectionProperties result = null;
                Gin.Controls.UserControlUtil.ExecuteOrInvoke(new Action(() =>
                    {
                        result = new SQLConnectionProperties()
                        {
                            DBName = comboDBName.Text,
                            InstanceName = comboInstanceName.Text,
                            Password = textPassword.Text,
                            UserName = textUserName.Text,
                            SqlAuthentication = comboAuthentification.SelectedIndex == 0
                        };
                    }), this);
                return result;
            }
            set
            {
                if (value != null)
                {
                    Gin.Controls.UserControlUtil.ExecuteOrInvoke(new Action(() =>
                    {
                        comboDBName.Text = value.DBName;
                        comboInstanceName.Text = value.InstanceName;
                        textPassword.Text = value.Password;
                        textUserName.Text = value.UserName;
                        comboAuthentification.SelectedIndex = value.SqlAuthentication ? 0 : 1;
                    }), this);
                }
            }
        }

        public void SetControlEnabled(SQLConnectionSubControl control, bool enabled)
        {
            _controls[(int)control].Enabled = enabled;
        }

        private void SQLConnectionControl_Resize(object sender, EventArgs e)
        {
            const int POSITION_1 = 115;
            const int POSITION_2 = 135;
            const int RIGHT_PADDING_1 = 7;
            comboInstanceName.Width = this.Width - POSITION_1 - RIGHT_PADDING_1;
            comboAuthentification.Width = this.Width - POSITION_1 - RIGHT_PADDING_1;
            comboDBName.Width = this.Width - POSITION_1 - RIGHT_PADDING_1;
            panelSqlAuthentication.Width = this.Width - POSITION_2 - RIGHT_PADDING_1;

            comboInstanceName.Left = POSITION_1;
            comboAuthentification.Left = POSITION_1;
            comboDBName.Left = POSITION_1;
            panelSqlAuthentication.Left = POSITION_2;
        }

        private void comboDBName_DropDown(object sender, EventArgs e)
        {
            if (_needRefreshDBS)
            {
                SqlConnection connection = null;
                try
                {
                    connection = new SqlConnection(Value.ConnectionString);
                    SqlCommand command = new SqlCommand(QUERY_DBS, connection);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable tbl = new System.Data.DataTable();
                    adapter.Fill(tbl);
                    comboDBName.DataSource = tbl;
                }
                catch (SqlException)
                {
                    _context.Log.AddLogError("Не удалось подключиться к SQL-серверу. Возможно, введена неверная пара 'логин/пароль' или сервер не существует");
                }
                finally
                {
                    _needRefreshDBS = false;
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void textControls_TextChanged(object sender, EventArgs e)
        {
            _needRefreshDBS = true;
        }
    }

    public enum SQLConnectionSubControl
    {
        ServerName = 0,
        SqlAuthentication = 1,
        UserName = 2,
        Password = 3,
        DBName = 4
    }
}
