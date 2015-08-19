using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Gin.Attributes;
using Gin.Context;
using Gin.Util;
using Gin.SQL.Util;
using Gin.Commands;
using Gin;
using Gin.Logging;

namespace Avicomp.Installer
{

    public enum OSBBSynchronizationType
    {
        [GinName(Name = "Текстовый файл")]
        TextFile = 0,
        [GinName(Name = "Эталонная база")]
        EtalonDB = 1
    }

    [GinName(Name = "Синхронизация", Description = "Выполняет синхронизацию OSBB", Group = "OSBB")]
    /// <summary>
    /// Команда проводит синхронизацию БД OSBB
    /// </summary>
    public class CMSynchronize : Command, IContentCommand
    {

        #region Агрументы команды

        [GinArgumentText(AllowTemplates = true, Name = "Строка подключения к БД", Description = "Строка подключения к БД")]
        public string ConnectionString { get; set; }

        [GinArgumentBrowseFolder(AllowTemplates = true, Name = "Путь к пакету", Description = "Полный путь к файлам пакета синхронизации")]
        public string PackagePath { get; set; }

        [GinArgumentText(AllowTemplates = true, Name = "Строка подключения к эталонной БД", Description = "Строка подключения к эталонной БД")]
        public string ConnectionStringEtalon { get; set; }

        [GinArgumentEnum(ListEnum = typeof(OSBBSynchronizationType), Name = "Тип синхронизации", Description = "Тип синхронизации")]
        public OSBBSynchronizationType SyncType { get; set; }

        [GinArgumentCheckBox(AllowTemplates = true, Name = "Загрузка PICK", Description = "Нужна ли загрузка PICK")]
        public object LoadPick { get; set; }

        [GinArgumentCheckBox(AllowTemplates = true, Name = "Загрузка иерархии", Description = "Загрузить иерархию?")]
        public object LoadHierarchy { get; set; }

        [GinArgumentCheckBox(AllowTemplates = true, Name = "Обновить коды", Description = "Нужно ли обновить коды?")]
        public object UpdateDisplayCode { get; set; }

        [GinArgumentCheckBox(AllowTemplates = true, Name = "Проверка диапазонов формул", Description = "Нужна ли проверка диапазонов формул?")]
        public object CheckFormulaDiapasons { get; set; }

        [GinArgumentCheckBox(AllowTemplates = true, Name = "Обновить SetStrings", Description = "Обновить SetStrings?")]
        public object UpdateSetStrings { get; set; }

        #endregion

        #region Константы команды

        private string[] PackageList = { "ImpCreateTempTab", "ImpOuterControls", "ImpExcelModels", "ImpFormulas", "ImpReferences", "ImpReportModels", "ImpSets", "ImpTargetForms", "ImpTextExchange", "ImpDelOWN", "ImpNSI" };

        private string[] PackageListNet = { "ImpCreateTempTab", "ImpExcelModelsNet", "ImpFormulasNet", "ImpReferencesNet", "ImpReportModelsNet", "ImpSetsNet", "ImpTextExchangeNet", "ImpNSI" };

        #endregion

        private IExecutionContext _context = null;


        public CMSynchronize()
        {
            _progressCost = 500;
            LoadPick = false;
            LoadHierarchy = false;
            UpdateDisplayCode = false; CheckFormulaDiapasons = false;
            UpdateSetStrings = false;
        }

        public override CommandResult Do(IExecutionContext context)
        {
            _context = context;

            context.Log.AddLogInformation("SyncType = " + SyncType);
            string absoluteConnectionCtring = context.GetStringFrom(ConnectionString);
            context.Log.AddLogInformation("ConnectionString = '" + absoluteConnectionCtring + "'");
            string absolutePackagePath = context.GetStringFrom(PackagePath);
            context.Log.AddLogInformation("PackagePath = '" + absolutePackagePath + "'");

            using (SqlConnection connection = new SqlConnection(absoluteConnectionCtring))
            {
                connection.Open();
                context.Log.AddLogInformation("Открыли соединение с БД, выполняем rp_AddSynchRegister");
                int synchID = CommonSP.rp_AddSynchRegister(connection, absolutePackagePath);
                context.Log.AddLogInformation("rp_AddSynchRegister выполнена, SynchID = " + synchID);
                int synchResult = 0;

                try
                {
                    ExecuteDTSPackages(connection, context);

                    if (SyncType == OSBBSynchronizationType.TextFile)
                    {
                        UpdateExcelTemplates(connection, absolutePackagePath);
                    }

                    bool absoluteUpdateSetStrings = context.GetBoolFrom(UpdateSetStrings);
                    if (absoluteUpdateSetStrings)
                    {
                        _context.Log.AddLogInformation("Начинаем выполнение rp_UpdateSetStringName");
                        CommonSP.rp_UpdateSetStringName(connection);
                        _context.Log.AddLogInformation("rp_UpdateSetStringName выполнена");
                    }

                    bool absoluteCheckFormulaDiapasons = context.GetBoolFrom(CheckFormulaDiapasons);
                    if (absoluteCheckFormulaDiapasons)
                    {
                        _context.Log.AddLogInformation("Начинаем выполнение rp_BuildFormulas");
                        CommonSP.rp_BuildFormulas(connection);
                        _context.Log.AddLogInformation("rp_BuildFormulas выполнена");
                    }
                    synchResult = 1;
                }
                finally
                {
                    _context.Log.AddLogInformation("Начинаем выполнение rp_UpdSynchRegister");
                    CommonSP.rp_UpdSynchRegister(connection, absolutePackagePath, synchID, synchResult);
                    _context.Log.AddLogInformation("rp_UpdSynchRegister выполнена");
                }

            }

            return CommandResult.Next;
        }


        private void ExecuteDTSPackages(SqlConnection connection, IExecutionContext context)
        {
            string dtsPath = CommonSP.GetDTSPath(connection);
            _context.Log.AddLogInformation("DTSDirectoryPath = '" + dtsPath + "'");
            bool absoluteLoadPick = _context.GetBoolFrom(LoadPick);
            bool absoluteLoadHierarchy = _context.GetBoolFrom(LoadHierarchy);
            bool absoluteUpdateDisplayCode = _context.GetBoolFrom(UpdateDisplayCode);
            _context.Log.AddLogInformation("LoadPick, LoadHierarchy, UpdateDisplayCode = '" + absoluteLoadPick + ", " + absoluteLoadHierarchy + ", " + absoluteUpdateDisplayCode + "'");
            string logFilePath = Path.Combine(_context.ExecutedPackage.PackagePath, Guid.NewGuid().ToString("N") + ".log");
            _context.Log.AddLogInformation("logFilePath = '" + logFilePath + "'");
            string absoluteEtalonConnectionCtring = _context.GetStringFrom(ConnectionStringEtalon);
            _context.Log.AddLogInformation("EtalonConnectionCtring = '" + absoluteEtalonConnectionCtring + "'");
            string absolutePackagePath = _context.GetStringFrom(PackagePath);
            if (!absolutePackagePath.EndsWith(@"\"))
            {
                absolutePackagePath += @"\";
            }
            string[] _packList = SyncType == OSBBSynchronizationType.TextFile ? PackageList : PackageListNet;
            foreach (string pack in _packList)
            {
                CheckForPendingCancel(context);
                string packPath = Path.Combine(dtsPath, pack + ".dts");
                _context.Log.AddLogInformation("Начинаем выполнение пакета " + packPath);
                DTSExecutor executor = new DTSExecutor(packPath, logFilePath, false);
                executor.OnLogger += new LoggerEvent(_context.Log.AddLogEvent);
                executor.OnProgress += new ProgressEvent(executor_OnProgress);
                _context.Log.AddLogInformation("Создали экземпляр DTSExecutor");

                var connectionStringParts = SQLUtil.GetConnectionStringParts(connection.ConnectionString);
                var etalonConnectionStringParts = SQLUtil.GetConnectionStringParts(absoluteEtalonConnectionCtring);

                _context.Log.AddLogInformation("Разбили на части обе ConnectionStrings");

                executor.SaveParameter("DataSource", connectionStringParts["Data Source"]);
                executor.SaveParameter("Catalog", connectionStringParts["Initial Catalog"]);
                executor.SaveParameter("UserID", connectionStringParts["User ID"]);
                executor.SaveParameter("Password", connectionStringParts["Password"]);
                executor.SaveParameter("DataSource_S", SyncType == OSBBSynchronizationType.EtalonDB ? etalonConnectionStringParts["DataSource"] : "");
                executor.SaveParameter("Catalog_S", SyncType == OSBBSynchronizationType.EtalonDB ? etalonConnectionStringParts["Initial Catalog"] : "");
                executor.SaveParameter("UserID_S", SyncType == OSBBSynchronizationType.EtalonDB ? etalonConnectionStringParts["UserID"] : "");
                executor.SaveParameter("Password_S", SyncType == OSBBSynchronizationType.EtalonDB ? etalonConnectionStringParts["Password"] : "");
                executor.SaveParameter("FilePath", SyncType == OSBBSynchronizationType.TextFile ? absolutePackagePath : "");
                executor.SaveParameter("Pick", absoluteLoadPick ? 1 : 4);
                executor.SaveParameter("Hierarchy", absoluteLoadHierarchy ? 1 : 4);
                executor.SaveParameter("UpdateDysplayCodeOfCatalogs", absoluteUpdateDisplayCode ? 1 : 0);
                _context.Log.AddLogInformation("Почти начали выполнять DTSExecutor.Execute()");
                CommonSP.rp_SetDBStatus(connection, 1);
                executor.Execute();
                CommonSP.rp_SetDBStatus(connection, 0);
                _context.Log.AddLogInformation("Выполнили DTSExecutor.Execute()");
            }
        }

        void executor_OnProgress(int percent)
        {
            _context.Log.SendProgress(new ExecutionProgressInfo()
            {
                Message = "",
                ModuleName = "",
                ProgressCost = 0
            });
        }

        private void UpdateExcelTemplates(SqlConnection connection, string dataPath)
        {
            _context.Log.AddLogInformation("Начинаем импортировать XLT-шаблоны");
            IOUtil.IterateDirectory(dataPath, new Action<string>(
            path =>
            {
                string extension = Path.GetExtension(path);
                if (extension == ".xlt")
                {
                    _context.Log.AddLogInformation("Найден файл '" + path + "'");
                    string stringGuid = Path.GetFileNameWithoutExtension(path);
                    Guid rowGuid = Guid.Empty;
                    try
                    {
                        rowGuid = new Guid(stringGuid);
                    }
                    catch
                    {
                        _context.Log.AddLogInformation("Имя файла не является коректным GUID-ом");
                    }
                    if (rowGuid != Guid.Empty)
                    {
                        _context.Log.AddLogInformation("Подготавливаем запись XLT-файла в БД");
                        SqlCommand selectCommand = new SqlCommand("select * from rp_ExcelFiles where RowGUID = @row_guid", connection);
                        selectCommand.Parameters.Add("row_guid", SqlDbType.UniqueIdentifier).Value = rowGuid;
                        SqlDataAdapter daExcelFiles = new SqlDataAdapter(selectCommand);
                        SqlCommandBuilder cbExcelFiles = new SqlCommandBuilder(daExcelFiles);
                        DataSet dsExcelFiles = new DataSet("ExcelFiles");
                        daExcelFiles.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                        _context.Log.AddLogInformation("Получаем из БД строку соответсвующую текущему файлу");
                        daExcelFiles.Fill(dsExcelFiles, "ExcelFiles");
                        int rowsCount = dsExcelFiles.Tables["ExcelFiles"].Rows.Count;
                        _context.Log.AddLogInformation("Найдено подходящих строк " + rowsCount);
                        if (rowsCount == 1)
                        {
                            DataRow rowExcelFile = dsExcelFiles.Tables["ExcelFiles"].Rows[0];
                            FileStream fsExcelFile = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
                            byte[] bytesExcelFile = new byte[fsExcelFile.Length];
                            fsExcelFile.Read(bytesExcelFile, 0, System.Convert.ToInt32(fsExcelFile.Length));
                            fsExcelFile.Close();
                            _context.Log.AddLogInformation("Прочитали с диска текущий файл");
                            rowExcelFile["TemplateFileContent"] = System.Text.Encoding.Default.GetString(bytesExcelFile);
                            daExcelFiles.Update(dsExcelFiles, "ExcelFiles");
                            _context.Log.AddLogInformation("Обновили содержимое строки в БД");
                        }
                    }
                }
            }));
        }

        #region IContentCommand Members

        public string ContentPath
        {
            get
            {
                return PackagePath;
            }
            set
            {
                PackagePath = value;
            }
        }

        #endregion

    }
}