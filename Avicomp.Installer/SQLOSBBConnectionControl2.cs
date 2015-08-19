using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using Gin;
using Gin.Context;


namespace Avicomp.Installer
{
    public partial class SQLOSBBConnectionControl2 : UserControl
    {

        IExecutionContext _context;

        public SQLOSBBConnectionControl2(IExecutionContext context)
        {
            InitializeComponent();
            _context = context;
        }

        public SQLOSBBConnectionProperties Value
        {
            get
            {
                SQLOSBBConnectionProperties result = null;
                Gin.Controls.UserControlUtil.ExecuteOrInvoke(new Action(() =>
                    {
                        result = new SQLOSBBConnectionProperties();
                        result.Password = textPassword.Text;
                        result.UserName = textUserName.Text;
                        result.CreateBackup = checkCreateBackup.Checked;
                        result.BackupFilePath = textBackupPath.Text;
                        result.InstanceName = textServerName.Text;
                        result.DBName = (string)comboDBName.SelectedValue;
                        result.SqlAuthentication = true;
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
            const int POSITION_1 = 118;
            const int POSITION_2 = 27;
            const int RIGHT_PADDING_1 = 7;
            comboDBName.Width = this.Width - POSITION_1 - RIGHT_PADDING_1;
            textServerName.Width = comboDBName.Width;
            panelSqlAuthentication.Width = this.Width - POSITION_1 - RIGHT_PADDING_1;
            textBackupPath.Width = this.Width - POSITION_2 - RIGHT_PADDING_1 * 2 - buttonBrowse.Width;
            comboDBName.Left = POSITION_1;
            textServerName.Left = POSITION_1;
            textBackupPath.Left = POSITION_2;
            panelSqlAuthentication.Left = POSITION_1;
            buttonBrowse.Left = this.Width - RIGHT_PADDING_1 - buttonBrowse.Width;
            checkCreateBackup.Left = POSITION_2;
        }

        private void comboInstanceName_SelectedIndexChanged(object sender, EventArgs e)
        {
            SQLOSBBConnectionProperties config = Value;
            RefreshControls(config);
        }

        private void RefreshControls(SQLOSBBConnectionProperties config)
        {
            textServerName.Text = config.InstanceName;
            comboDBName.SelectedItem = config.DBName;
            textPassword.Text = config.Password;
            textUserName.Text = config.UserName;
            checkCreateBackup.Checked = config.CreateBackup;
            textBackupPath.Text = config.BackupFilePath;
            textBackupPath.Enabled = config.CreateBackup;
            buttonBrowse.Enabled = config.CreateBackup;
        }

        private const string QUERY_OSBB_DBS = @"
DECLARE @dbname sysname, 
        @sql nvarchar(1000), 
        @dbid int
       
SET NOCOUNT ON
CREATE TABLE #gsysobjects(dbname sysname, 
                          DbID int, id int, 
                          [name] sysname, 
                          NameDB varchar(50))

CREATE TABLE #isexist(isexist int)

DECLARE curDB CURSOR FOR
SELECT [Name], 
       DbID 
FROM master..sysdatabases 
WHERE [Name] NOT IN ('master','tempdb','pubs','msdb','model') 
       and HAS_DBACCESS([Name]) = 1 
OPEN curDB
FETCH NEXT FROM curDB 
INTO @dbname, @dbid

WHILE @@FETCH_STATUS = 0
BEGIN
  DELETE #isexist
  SET @sql = 
             'IF EXISTS(SELECT * FROM [' + 
             @dbname+'].dbo.sysobjects s JOIN [' + 
             @dbname+'].dbo.syscolumns c ON c.id = s.id  AND c.name = ''DBName'' ' +
             ' WHERE s.xtype = ''u'' AND s.name = ''rp_DBProperties''   ) ' +
             ' INSERT INTO #isexist(isexist) VALUES(1) '
  PRINT @sql
  EXEC(@sql)
 
  IF EXISTS(SELECT * FROM #isexist) 
  BEGIN
    SET @sql = ' INSERT INTO #gsysobjects (dbname, dbid, id, name, namedb) SELECT ' + 
              ''''+ @dbname + '''' + ',' + CAST(@dbid as varchar)+', id, name, DBName FROM [' + 
              @dbname+'].dbo.sysobjects, ['+ @dbname + 
              '].dbo.rp_DBProperties WHERE xtype = ''u'' AND name = ''rp_DBProperties'''
    PRINT @sql
    EXEC (@sql)
  END  
   
  FETCH NEXT FROM curDB INTO @dbname, @dbid
END

CLOSE curDB
DEALLOCATE curDB
SELECT dbname + CASE WHEN namedb is null then '' else ' '  + namedb END DBS, * 
FROM #gsysobjects
order by dbname

DROP TABLE #gsysobjects
DROP TABLE #isexist
SET NOCOUNT OFF";

        private bool _needRefreshDBS = true;
        private void comboDBName_DropDown(object sender, EventArgs e)
        {
            if (_needRefreshDBS)
            {
                SqlConnection connection = null;
                comboDBName.DataSource = null;
                try
                {
                    connection = new SqlConnection(Value.ConnectionString);
                    SqlCommand command = new SqlCommand(QUERY_OSBB_DBS, connection);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable tbl = new System.Data.DataTable();
                    adapter.Fill(tbl);
                    comboDBName.DataSource = tbl;
                    comboDBName.ValueMember = "dbname";
                    comboDBName.DisplayMember = "dbname";
                }
                catch(SqlException)
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

        private void textServerName_TextChanged(object sender, EventArgs e)
        {
            _needRefreshDBS = true;
        }

        private void textUserName_TextChanged(object sender, EventArgs e)
        {
            _needRefreshDBS = true;
        }

        private void textPassword_TextChanged(object sender, EventArgs e)
        {
            _needRefreshDBS = true;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".bak";
            dialog.AddExtension = true;
            if (textBackupPath.Text != "")
            {
                dialog.FileName = textBackupPath.Text;
            }
            else
            {
                dialog.FileName = GenerateBackupFileName();
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBackupPath.Text = dialog.FileName;
            }
        }

        private string GenerateBackupFileName()
        {
            return comboDBName.Text + "_" + DateTime.Now.ToString("yyyyMMdd") + ".bak";
        }

        private void checkCreateBackup_CheckedChanged(object sender, EventArgs e)
        {
            textBackupPath.Enabled = checkCreateBackup.Checked;
            buttonBrowse.Enabled = checkCreateBackup.Checked;
        }
    }
}
