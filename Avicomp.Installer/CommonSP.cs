using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Gin.Attributes;

namespace Avicomp.Installer
{
    public static class CommonSP
    {
        public static void rp_SetDBStatus(SqlConnection connection, int status)
        {
            SqlCommand command = new SqlCommand("rp_SetDBStatus", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("DBStatus", SqlDbType.Int).Value = status;
            command.ExecuteNonQuery();
        }

        public static void rp_BuildFormulas(SqlConnection connection)
        {
            SqlCommand command = new SqlCommand("rp_BuildFormulas", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 3600;
            command.ExecuteNonQuery();
        }

        public static void rp_UpdateSetStringName(SqlConnection connection)
        {
            SqlCommand command = new SqlCommand("rp_UpdateSetStringName", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 3600;
            command.ExecuteNonQuery();
        }

        public static int rp_AddSynchRegister(SqlConnection connection, string packagePath)
        {
            SqlCommand command = new SqlCommand("rp_AddSynchRegister", connection);
            command.CommandType = CommandType.StoredProcedure;
            string filePath = Path.GetDirectoryName(packagePath);
            string fileName = Path.GetFileName(packagePath);
            command.Parameters.Add("FilePath", SqlDbType.VarChar).Value = filePath;
            command.Parameters.Add("SynchronizationFileName", SqlDbType.VarChar).Value = fileName;
            command.Parameters.Add("SynchronizationType", SqlDbType.TinyInt).Value = 1;
            command.Parameters.Add("SynchronizationResult", SqlDbType.TinyInt).Value = 0;
            command.Parameters.Add("Comment", SqlDbType.VarChar).Value = "";
            command.Parameters.Add("Number", SqlDbType.Int).Direction = ParameterDirection.Output;
            command.Parameters.Add("Recipient", SqlDbType.Int).Value = DBNull.Value;
            command.Parameters.Add("RETURN_VALUE", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            command.ExecuteNonQuery();
            int synchID = (int)command.Parameters["RETURN_VALUE"].Value;
            return synchID;
        }

        public static void rp_UpdSynchRegister(SqlConnection connection, string packagePath, int synchID, int resType)
        {
            SqlCommand command = new SqlCommand("rp_UpdSynchRegister", connection);
            command.CommandType = CommandType.StoredProcedure;
            string filePath = Path.GetDirectoryName(packagePath);
            string fileName = Path.GetFileName(packagePath);
            command.Parameters.Add("FilePath", SqlDbType.VarChar).Value = filePath;
            command.Parameters.Add("SynchronizationFileName", SqlDbType.VarChar).Value = fileName;
            command.Parameters.Add("SynchronizationResult", SqlDbType.TinyInt).Value = resType;
            command.Parameters.Add("SynchID", SqlDbType.Int).Value = synchID;
            command.ExecuteNonQuery();
        }


        public static List<OSBBDatabase> GetOSBBDatabases(SqlConnection connection)
        {
            const string SELECT_OSBB_DBS_QUERY = @"
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
SET NOCOUNT OFF
";

            List<OSBBDatabase> result = new List<OSBBDatabase>();

            SqlCommand command = new SqlCommand(SELECT_OSBB_DBS_QUERY, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                OSBBDatabase db = new OSBBDatabase()
                {
                    Alias = (string)reader["DBS"],
                    DatabaseID = (int)reader["dbname"],
                    DBName = (string)reader["DbID"]
                };
                result.Add(db);
            }
            return result;
        }

        public static string GetDTSPath(SqlConnection connection)
        {
            string query = @"SELECT ISNULL(spv.ParamValue, '%PROGRAM%\DTS') AS DTSPath
FROM rp_GroupParams gp
INNER JOIN rp_SystemParams sp ON sp.grp_GroupParam = gp.GroupID
INNER JOIN rp_SystemParamValues spv ON spv.prm_Param = sp.ParamID
WHERE gp.GroupCode = 'TabDTS' AND sp.ParamCode = 'DTSPACKAGES'";
            SqlCommand command = new SqlCommand(query, connection);
            command.CommandType = CommandType.Text;
            string dtsPath = (string)command.ExecuteScalar();
            return dtsPath;
        }

    }

    [GinIncludeType()]
    public class OSBBDatabase
    {
        public string Alias;
        public string DBName;
        public int DatabaseID;
    }

}
