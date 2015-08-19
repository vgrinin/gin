using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using Gin;
using Gin.Logging;

namespace Gin.SQL.Util
{

    public class SqlCommandProgress
    {

        private const string SQL_GET_PROGRESS_TEMPLATE =
@"USE master
SELECT command,
  start_time,
  GetDate() now,
  dateadd(millisecond,estimated_completion_time, getdate()) AS est_completion_time,
  percent_complete
FROM sys.dm_exec_requests r
  CROSS APPLY sys.dm_exec_sql_text(r.sql_handle) s
WHERE 1=1 
  AND r.command IN ('RESTORE DATABASE', 'BACKUP DATABASE', 'RESTORE LOG', 'BACKUP LOG')
  AND s.text LIKE '%{command_guid}%'";

        private string _commandTextGuid;
        private Timer _timer;
        private string _connectionString;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="textGuid">GUID-строка, встречающаяся в тексте SQL-команды в качестве комментария
        /// В каждой команде, использующей SqlCommandProgress необходимо предусмотреть вставку в исходный код
        /// запроса этой GUID-строки в виде комментария</param>
        /// <param name="pollingIntervalSeconds">Интервал опроса сервера в секундах</param>
        public SqlCommandProgress(string commandTextGuid, string connectionString, TimeSpan pollingInterval)
        {
            _commandTextGuid = commandTextGuid;
            _connectionString = connectionString;
            _timer = new Timer(callback, null, pollingInterval, pollingInterval);
        }

        public event ProgressEvent OnProgress;

        private void callback(object state)
        {
            int percentComplete = (int)GetPercentComplete();

            if (OnProgress != null)
            {
                OnProgress(percentComplete);
            }
        }

        private float GetPercentComplete()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    SQL_GET_PROGRESS_TEMPLATE.Replace("{command_guid}", _commandTextGuid), 
                    connection);
                connection.Open();
                SqlDataReader reader = null;
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return (float)reader["percent_complete"];
                }
            }

            return 0;
        }

        public void StopPolling()
        {
            _timer.Dispose();
        }

    }
}
