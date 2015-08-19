using System;
using System.Collections.Generic;
using Avicomp.Common.Excel;
using Gin.Attributes;
using Gin.Context;
using Gin.Util;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using Gin.SQL.Util;
using Gin;
using Gin.Commands;


namespace Avicomp.Installer
{

    public enum SecretType
    {
        [GinName(Name = "Авто")]
        Auto = -1,
        [GinName(Name = "Не секретные")]
        NoSecret = 0,
        [GinName(Name = "Секретные")]
        Secret = 1,
        [GinName(Name = "Совершенно секретные")]
        VerySecret = 3
    }


    [GinName(Name = "Импорт связок", Description = "Импортирует Excel-файл со связками", Group = "OSBB")]
    public class CMImportSets : Command, IContentCommand
    {

        #region private consts

        private enum ImportType
        {
            ClearTable = 1,
            ImportRows = 2
        }

        public enum SetType
        {
            [GinName(Name = "Дерево")]
            Tree = 0,
            [GinName(Name = "Заголовок отчета")]
            Header = 1,
            [GinName(Name = "Боковик отчета")]
            Model = 2,
            [GinName(Name = "Бюджетная роспись")]
            Budget = 3
        }

        #endregion

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Name = "Строка подключения", Description = "Строкаа подключения к БД")]
        public string ConnectionString { get; set; }

        [GinArgumentBrowseFile(AllowTemplates = true, Name = "Excel-файл", Description = "Путь к Excel-файлу")]
        public string ExcelFilePath { get; set; }

        [GinArgumentText(AllowTemplates = true, Name = "Имя листа", Description = "Имя импортируемого листа книги Excel")]
        public string WorksheetName { get; set; }

        [GinArgumentInt(AllowTemplates = true, Name = "Со строки", Description = "Номер первой строки листа для импорта")]
        public object StartRowNumber { get; set; }

        [GinArgumentInt(AllowTemplates = true, Name = "По строку", Description = "Номер последней строки листа для импорта")]
        public object EndRowNumber { get; set; }

        [GinArgumentCheckBox(AllowTemplates = true, Name = "Пропускать дубликаты", Description = "Пропускать дубликаты")]
        public object IgnoreDuplicates { get; set; }

        [GinArgumentInt(AllowTemplates = true, Name = "Версия НСИ", Description = "Номер версии НСИ")]
        public object NSIVersion { get; set; }

        [GinArgumentCheckBox(AllowTemplates = true, Name = "Удалить предыдущие значения", Description = "Удалить предыдущие значения")]
        public object DeletePreviousSets { get; set; }

        [GinArgumentText(Name = "Имя связки", Description = "Имя свзяки")]
        public string SetName { get; set; }

        [GinArgumentEnum(ListEnum = typeof(SecretType), Name = "Секретность", Description = "Тип секретности")]
        public SecretType Secret { get; set; }

        [GinArgumentEnum(ListEnum = typeof(SetType), Name = "Тип связки", Description = "Тип связки")]
        public SetType ImportSetType { get; set; }

        #endregion

        #region private members

        private IExecutionContext _context;

        #endregion

        public CMImportSets()
        {
            StartRowNumber = 0;
            EndRowNumber = 0;
            IgnoreDuplicates = false;
            NSIVersion = 0;
            DeletePreviousSets = false;
            Secret = SecretType.Auto;
        }

        public override CommandResult Do(IExecutionContext context)
        {
            _context = context;
            _context.Log.AddLogInformation("Вход в CMImportSets.Do(ExecutionContext)");

            string absoluteExcelFilePath = context.GetStringFrom(ExcelFilePath);
            string absoluteWorksheetName = context.GetStringFrom(WorksheetName);
            int absoluteStartRowNumber = context.GetIntFrom(StartRowNumber);
            int absoluteEndRowNumber = context.GetIntFrom(EndRowNumber);
            _context.Log.AddLogInformation("Обрабатываем файл :'" + absoluteExcelFilePath + "'");
            _context.Log.AddLogInformation("Лист " + absoluteWorksheetName + ", строки " + absoluteStartRowNumber + "-" + absoluteEndRowNumber);
            string absoluteConnectionString = _context.GetStringFrom(ConnectionString);
            _context.Log.AddLogInformation("ConnectionString = '" + absoluteConnectionString + "'");
            string absoluteSetName = _context.GetStringFrom(SetName);
            _context.Log.AddLogInformation("SetName = '" + absoluteSetName + "'");

            _context.Log.AddLogInformation("Начинаем формирование фрагмента из Excel-файла");
            string fileName = GetBlockOfExcelData(absoluteExcelFilePath, absoluteWorksheetName, absoluteStartRowNumber, absoluteEndRowNumber);
            _context.Log.AddLogInformation("Формирование фрагмента завершено. Файл сохранен в '" + fileName + "'");

            using (SqlConnection connection = new SqlConnection(absoluteConnectionString))
            {
                connection.Open();
                _context.Log.AddLogInformation("Соединение с БД установлено");

                int? setTypeID = rp_GetSetTypeByName(connection, absoluteSetName, ImportSetType);
                _context.Log.AddLogInformation("SetTypeID = " + setTypeID);
                if (setTypeID == null)
                {
                    throw new ArgumentException("Проверьте наименование связки");
                }

                ClearImportSetStrings(connection);
                _context.Log.AddLogInformation("Очистили таблицу импорта. Начинаем выполнение DTS-пакета.");

                string dtsPath = CommonSP.GetDTSPath(connection);
                string packDTSPath = Path.Combine(dtsPath, "rp_ImportExcel.dts");

                ExecuteDTS(connection, fileName, packDTSPath);
                _context.Log.AddLogInformation("Выполнили DTS пакет. Начинаем импортировать связки.");

                // если не было ошибок идем дальше

                try
                {
                    rp_ImportSetStrings(connection, (int)setTypeID, null, null);
                    _context.Log.AddLogInformation("Связки импортированы. Начинаем устанавливать ключи шаблона.");
                }
                catch (Exception ex)
                {
                    _context.Log.AddLogError("Ошибка при выполнении процедуры импорта: " + ex.Message);
                }

                try
                {
                    rp_SetModelKeys(connection, null);
                    _context.Log.AddLogInformation("Ключи шаблона установлены.");
                }
                catch(Exception ex)
                {
                    _context.Log.AddLogInformation("Ошибка при установке ключей шаблона: " + ex.Message);
                }
            }

            IOUtil.TryDeleteFile(fileName);
            _context.Log.AddLogInformation("Файл '" + fileName + "' удален. Выход из CMImportSets.Do(ExecutionContext).");

            return CommandResult.Next;   
        }

        private string GetBlockOfExcelData(string fileName, string sheetName, int rowStart, int rowEnd)
        {
            const int MAX_COLUMN_NUMBER = 256;
            ExcelFactory factory = ExcelFactory.CreateInstance(ExcelApplicationType.TMS, ExcelGarbageCollectionStyle.DesktopApplication);
            ExcelApplication app = factory.AddApplication();
            ExcelWorkbook bookInitial = app.AddWorkbook("initial", fileName);
            ExcelWorksheet sheetInitial = bookInitial[sheetName];
            ExcelWorkbook bookResult = app.AddWorkbook("final");
            _context.Log.AddLogInformation("XLS-файл успешно загружен в память. Создан пустая результирующая книга.");
            bookResult.CopyRange(bookInitial, sheetInitial.Number, rowStart, 1, rowEnd, MAX_COLUMN_NUMBER, 2, 1, 1);
            _context.Log.AddLogInformation("Диапазон скопирован в новую книгу");
            ExcelWorksheet sheetResult = bookResult[1];

            for (int i = 0; i < MAX_COLUMN_NUMBER; i++)
            {
                sheetResult.WriteValue(1, i + 1, 1);
            }
            _context.Log.AddLogInformation("Заполнили единицами первую строку результирующего листа");

            string fileNameResult = Path.Combine(_context.ExecutedPackage.PackagePath, Guid.NewGuid().ToString("N") + ".xls");
            _context.Log.AddLogInformation("Готовы сохранить результирующий файл в '" + fileNameResult + "'");
            bookResult.Save(fileNameResult);
            _context.Log.AddLogInformation("Результирующий файл сохранен.");

            return fileNameResult;
        }

        private int? rp_GetSetTypeByName(SqlConnection connection, string setName, SetType setType)
        {
            const string SQL_QUERY_GET_SETTYPE = 
@"SELECT 
  SetTypeID 
FROM 
  rp_SetTypes 
WHERE 1=1
  AND SetType = @SetType 
  AND SetName = @SetName";
            SqlCommand command = new SqlCommand(SQL_QUERY_GET_SETTYPE, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.Add("SetType", SqlDbType.Int).Value = (int)setType;
            command.Parameters.Add("SetName", SqlDbType.VarChar).Value = setName;
            object setTypeID = command.ExecuteScalar();
            return (int?)setTypeID;
        }

        private void ClearImportSetStrings(SqlConnection connection)
        {
            SqlCommand command = new SqlCommand("rp_ImportSetStrings", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("ImportAction", SqlDbType.Int).Value = (int)ImportType.ClearTable;
            command.ExecuteNonQuery();
        }

        private void ExecuteDTS(SqlConnection connection, string excelFilePath, string absoluteDTSFilePath)
        {
            string logFilePath = Path.Combine(_context.ExecutedPackage.PackagePath, Guid.NewGuid().ToString("N") + ".log");
            _context.Log.AddLogInformation("Готовимся выполнить DTS-пакет '" + absoluteDTSFilePath + "'");
            _context.Log.AddLogInformation("Лог выполнения будем сохранять в файл '" + logFilePath + "'");

            Dictionary<string,string> connectionStringParts = SQLUtil.GetConnectionStringParts(connection.ConnectionString);
            if (!connectionStringParts.ContainsKey("Password") || !connectionStringParts.ContainsKey("User ID"))
            {
                throw new ArgumentException("Строка подключения должна содержать логин и пароль пользователя БД");
            }

            _context.Log.AddLogInformation("Разбили строку подключения на части. Строка содержит параметры SQL-аутентификации.");
            
            DTSExecutor executor = new DTSExecutor(absoluteDTSFilePath, logFilePath, false);
            executor.SaveParameter("ServerName", connectionStringParts["Data Source"]);
            executor.SaveParameter("DatabaseName", connectionStringParts["Initial Catalog"]);
            executor.SaveParameter("UserLogin", connectionStringParts["User ID"]);
            executor.SaveParameter("UserPassword", connectionStringParts["Password"]);
            executor.SaveParameter("ExcelExtendedProperties", "Excel 8.0;HDR=1;");
            executor.SaveParameter("ExcelFileName", excelFilePath);
            _context.Log.AddLogInformation("Добавили DTS-параметры.");
            executor.Execute();
            _context.Log.AddLogInformation("DTS выполнен успешно");
        }

        private void rp_ImportSetStrings(SqlConnection connection, int setTypeID, int? modelSectionID, int? etalonSetTypeID)
        {
            int absoluteNSIVersion = _context.GetIntFrom(NSIVersion);
            bool absoluteIgnoreDuplicates = _context.GetBoolFrom(IgnoreDuplicates);
            bool absoluteDeletePreviousSets = _context.GetBoolFrom(DeletePreviousSets);
            int absoluteStartRowNumber = _context.GetIntFrom(StartRowNumber);
            _context.Log.AddLogInformation("Готовимся начать выполнение хранимой процедуры rp_ImportSetStrings");
            SqlCommand command = new SqlCommand("rp_ImportSetStrings", connection);
            command.CommandTimeout = 3600;
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("ImportAction", SqlDbType.Int).Value = (int)ImportType.ImportRows;
            command.Parameters.Add("SetTypeID", SqlDbType.Int).Value = setTypeID;
            command.Parameters.Add("VersionNumber", SqlDbType.Int).Value = absoluteNSIVersion;
            command.Parameters.Add("StartLine", SqlDbType.Int).Value = absoluteStartRowNumber;
            command.Parameters.Add("ModelSectionID", SqlDbType.Int).Value = modelSectionID==null? DBNull.Value: (object)modelSectionID;
            command.Parameters.Add("TextColumn", SqlDbType.Int).Value = DBNull.Value;
            command.Parameters.Add("IgnoreStringDuplicates", SqlDbType.Bit).Value = absoluteIgnoreDuplicates;
            command.Parameters.Add("EtalonSetTypeID", SqlDbType.Int).Value = etalonSetTypeID == null ? DBNull.Value : (object)etalonSetTypeID;
            command.Parameters.Add("ClearSet", SqlDbType.Bit).Value = absoluteDeletePreviousSets;
            command.Parameters.Add("MultColumnName", SqlDbType.VarChar).Value = DBNull.Value;
            command.Parameters.Add("SecretType", SqlDbType.Int).Value = SecretTypeToInt(Secret);
            _context.Log.AddLogInformation("Готовы начать выполнение хранимой процедуры rp_ImportSetStrings");
            using (SqlDataReader reader = command.ExecuteReader())
            {
                _context.Log.AddLogInformation("Хранимая процедура rp_ImportSetStrings выполнилась");
                if (reader.HasRows)
                {
                    _context.Log.AddLogInformation("Результат содержит строки. Начинаем их вывод.");
                    while (reader.Read())
                    {
                        if (reader["ExcelBlankID"] == DBNull.Value)
                        {
                            _context.Log.AddLogWarning("Строка Excel " + reader["ExcelBlankID"] + " - " + reader["ErrorString"]);
                        }
                        else
                        {
                            _context.Log.AddLogWarning((string)reader["ErrorString"]);
                        }
                    }
                }
                _context.Log.AddLogInformation("Вывод результата закончен.");
                reader.Close();
                _context.Log.AddLogInformation("Закрыли SqlDataReader");
            }
        }

        private void rp_SetModelKeys(SqlConnection connection, int? modelSectionID)
        {
            _context.Log.AddLogInformation("Готовимся начать выполнение хранимой процедуры rp_SetModelKeys");
            SqlCommand command = new SqlCommand("rp_SetModelKeys", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("ModelSectionID", SqlDbType.Int).Value = modelSectionID == null ? DBNull.Value : (object)modelSectionID;
            command.Parameters.Add("ReportModelID", SqlDbType.Int).Value = DBNull.Value;
            command.ExecuteNonQuery();
        }


        private int? SecretTypeToInt(SecretType type)
        {
            if (type == SecretType.Auto)
            {
                return null;
            }
            return (int)type;
        }


        #region IContentCommand Members

        public string ContentPath
        {
            get 
            {
                return ExcelFilePath;
            }
            set
            {
                ExcelFilePath = value;
            }
        }

        #endregion
    }
}
