using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gin.Commands;
using Gin.Context;
using Gin.PackageContent;
using Gin.Transactions;
using System.Xml.Serialization;
using System.Security.Principal;
using System.Windows.Forms;
using System.Drawing;
using Gin.Logging;

namespace Gin
{
    public class Package
    {

        public const string TRANSACTIONS_SUBFOLDER_NAME = @"transactions";
        public const string PACKAGE_DATA_FILENAME = @"data.xml";
        public const string TRANSACTIONS_DATA_FILENAME = @"data.xml";
        public const string MESSAGE_UNEXISTENT_TRANSACTION = "Транзакция не существует";
        public const string TEMP_SUBFOLDER_NAME = @"temp";
        public const string UNNAMED_PACKAGE_NAME = "unnamed";

        private PackageBody _body;
        private IExecutionContext _context;
        private PackageContent.PackageContent _content;
        private PackageData _packageData;
        private SoftwareData _softwareData;
        private List<Transaction> _transactions;
        private bool _alreadyExecuted = false;

        public string SoftwarePath { get; private set; }
        public string PackagePath { get; private set; }
        public string PackageDataFile { get; private set; }
        public string SoftwareDataFile { get; private set; }
        public string TransactionsPath { get; private set; }

        public PackageData PackageData 
        {
            get 
            {
                return _packageData;
            }
        }

        public string GetContentPath(string fileName)
        {
            return _content.GetContent(fileName);
        }

        public PackageBody GetBody()
        {
            return _body;
        }

        public Package(IExecutionContext context)
        {
            _context = context;
            _context.ExecutedPackage = this;
            SoftwarePath = UNNAMED_PACKAGE_NAME;
        }

        public string Load(string filePath)
        {
            _context.Log.AddLogInformation("Вход в Package.Load(string)");
            LoadPackageContent(filePath);
            InternalLoadPackageBody();
            InternalLoadPackageInfo();
            _context.Log.AddLogInformation("Выход из Package.Load(string)");
            return _content.GetBody();
        }

        public void LoadPackageInfo(string filePath)
        {
            _context.Log.AddLogInformation("Вход в Package.LoadPackageInfo(string)");
            LoadPackageBody(filePath);
            InternalLoadPackageInfo();
            _context.Log.AddLogInformation("Выход из Package.LoadPackageInfo(string)");
        }

        private void InternalLoadPackageInfo()
        {
            InitPackagePaths();
            CheckPendingCancel();
            LoadPackageData();
            CheckPendingCancel();
            CheckAlreadyExecuted();
            LoadTransactions();
            CheckPendingCancel();
        }

        private void LoadPackageContent(string filePath)
        {
            string contentDirectoryPath = GetContentPath();

            PackageContentType contentType = PackageContent.PackageContent.GetContentType(filePath);
            if (contentType == PackageContentType.Wrong)
            {
                throw new Exception("Неверный тип файла");
            }
            _content = PackageContent.PackageContent.Create(contentType);
            _context.Log.AddLogInformation("Создали экземпляр PackageContent");
            _content.OnProgress += _context.Log.SendProgress;
            _content.OnQueryCancel += _context.Log.GetPendingCancel;
            _context.Log.CurrentProgress = 0;
            _context.Log.ProgressTotalCost = 100;
            _content.Load(contentDirectoryPath, filePath);
            CheckPendingCancel();
            _context.Log.AddLogInformation("Загрузили контент из '" + filePath + "', тип контента " + contentType);
        }

        private string GetContentPath()
        {
            string contentSubfolderName = Guid.NewGuid().ToString("N");
            string contentDirectoryPath = Path.Combine(_context.TempPath, contentSubfolderName);
            if (Directory.Exists(contentDirectoryPath))
            {
                Directory.Delete(contentDirectoryPath, true);
            }
            Directory.CreateDirectory(contentDirectoryPath);
            _context.Log.AddLogInformation("contentDirectoryPath = '" + contentDirectoryPath + "'");
            return contentDirectoryPath;
        }

        private void InternalLoadPackageBody()
        {
            string xmlFilePath = _content.GetBody();
            _context.Log.AddLogInformation("XML-конфигурация ожидается в '" + xmlFilePath + "'");
            _body = GinSerializer.Deserialize<PackageBody>(xmlFilePath);
            _context.Log.AddLogInformation("XML-конфигурация загружена");
        }

        private void LoadPackageBody(string filePath)
        {
            string contentDirectoryPath = GetContentPath();
            PackageContentType contentType = PackageContent.PackageContent.GetContentType(filePath);
            if (contentType == PackageContentType.Wrong)
            {
                throw new Exception("Неверный тип файла");
            }
            _content = PackageContent.PackageContent.Create(contentType);
            _context.Log.AddLogInformation("Создали экземпляр PackageContent");
            _content.LoadBody(contentDirectoryPath, filePath);
            InternalLoadPackageBody();
        }

        private void CheckPendingCancel()
        {
            QueryCancelEventArgs args = new QueryCancelEventArgs();
            _context.Log.GetPendingCancel(args);
            if (args.Cancel)
            {
                throw new PackageExecutionCancelledException();
            }
        }

        private void InitPackagePaths()
        {
            if (_body.SoftwareName == null)
            {
                _body.SoftwareName = UNNAMED_PACKAGE_NAME;
            }
            SoftwarePath = Path.Combine(_context.PackagesPath, _body.SoftwareName);
            PackagePath = Path.Combine(SoftwarePath, _body.PackageId);
            PackageDataFile = Path.Combine(PackagePath, PACKAGE_DATA_FILENAME);
            SoftwareDataFile = Path.Combine(SoftwarePath, PACKAGE_DATA_FILENAME);
            TransactionsPath = Path.Combine(PackagePath, TRANSACTIONS_SUBFOLDER_NAME);
            _context.Log.AddLogInformation("Инициализированы директории пакета");
            _context.Log.AddLogInformation("PackagePath = " + PackagePath);
            _context.Log.AddLogInformation("PackageDataFile = " + PackageDataFile);
            _context.Log.AddLogInformation("SoftwareDataFile = " + SoftwareDataFile);
            _context.Log.AddLogInformation("TransactionsPath = " + TransactionsPath);
        }

        private void LoadPackageData()
        {
            if (File.Exists(SoftwareDataFile))
            {
                _softwareData = GinSerializer.Deserialize<SoftwareData>(SoftwareDataFile);
                foreach (var item in _softwareData.InstallationParameters)
                {
                    _context.SaveResult(item.Name, item.Value, true);
                }
                _context.Log.AddLogInformation("Загружен SoftwareData");
            }
            if (String.IsNullOrEmpty(PackageDataFile) || !File.Exists(PackageDataFile))
            {
                return;
            }
            _context.Log.AddLogInformation("PackageData ожидается в '" + PackageDataFile + "'");
            _packageData = GinSerializer.Deserialize<PackageData>(PackageDataFile);
            _alreadyExecuted = true;
            _context.Log.AddLogInformation("Загружен PackageData");
        }

        private void CheckAlreadyExecuted()
        {
            _alreadyExecuted = (_packageData != null);
            _context.Log.AddLogInformation("_alreadyExecuted = " + _alreadyExecuted);
        }

        private void LoadTransactions()
        {
            _context.Log.AddLogInformation("Загружаем транзакции");
            _transactions = new List<Transaction>();
            if (String.IsNullOrEmpty(TransactionsPath) || !Directory.Exists(TransactionsPath))
            {
                _context.Log.AddLogInformation("Транзакции отсутствуют");
                return;
            }
            string[] transactionNames = Directory.GetDirectories(TransactionsPath);
            _context.Log.AddLogInformation("Найдено " + transactionNames.Length + " транзакций");
            foreach (string name in transactionNames)
            {
                string transactionPath = Path.Combine(TransactionsPath, name);
                string dataFilePath = Path.Combine(transactionPath, TRANSACTIONS_DATA_FILENAME);
                Transaction transaction = GinSerializer.Deserialize<Transaction>(dataFilePath);
                _transactions.Add(transaction);
            }
            _context.Log.AddLogInformation("Все транзакции загружены");
        }

        public void Execute()
        {
            _context.Log.AddLogInformation("Вход в Package.Execute()");
            AdjustMainControl();
            PrepareExecution();
            ExecuteRootCommand(_body.Command);
            _context.Log.AddLogInformation("Корневая команда выполнена");
            _context.Log.AddLogInformation("Выход из Package.Execute()");
        }

        private void AdjustMainControl()
        {
            if (_context.ControlContainer == null)
            {
                return;
            }
            if (!(_context.ControlContainer is IAdjustableControl))
            {
                return;
            }

            _context.ControlContainer.Invoke(new Action(() =>
            {
                IAdjustableControl iAdjust = (IAdjustableControl)_context.ControlContainer;
                if (!String.IsNullOrEmpty(_body.PackageName))
                {
                    iAdjust.Caption = _body.PackageName;
                }
                iAdjust.Height = _body.Height;
                iAdjust.Width = _body.Width;
            }));
        }

        private void PrepareExecution()
        {
            _context.Log.AddLogInformation("Вход в Package.PrepareExecution");
            _context.Log.ProgressTotalCost = 100;
            _context.Log.CurrentProgress = 0;
            
            if (_alreadyExecuted)
            {
                throw new PackageAlreadyExecutedException();
            };

            Directory.CreateDirectory(PackagePath);
            Directory.CreateDirectory(TransactionsPath);
            _context.Log.AddLogInformation("Выход из Package.PrepareExecution");
        }

        private void SaveCurrentPackageData()
        {
            string userName = "unknown";
            if (WindowsIdentity.GetCurrent() != null)
            {
                userName = WindowsIdentity.GetCurrent().Name;
            }
            var instParameters = _context.GetInstallationParameters();
            PackageData data = new PackageData()
            {
                InstallationDate = DateTime.Now,
                InstallationUserName = userName,
                InstallationParameters = instParameters
            };
            GinSerializer.Serialize(data, PackageDataFile);
            SoftwareData dataSoftware = new SoftwareData()
            {
                InstallationParameters = instParameters
            };

            GinSerializer.Serialize(dataSoftware, SoftwareDataFile);
        }

        private void ExecuteRootCommand(Command command)
        {
            int totalCost = command.ProgressCost;
            _context.Log.ProgressTotalCost = totalCost;
            try
            {
                command.Execute(_context);
                SaveCurrentPackageData();
                _context.Log.SendProgress(new ExecutionProgressInfo()
                {
                    Message = "Выполнение пакета завершено успешно",
                    ModuleName = "Package executor",
                    ProgressCost = 0
                });
            }
            catch (Exception ex)
            {
                _context.Log.AddLogException(ex);
                _context.Log.SendProgress(new ExecutionProgressInfo()
                {
                   Message = "Выполнение пакета завершено с ошибками",
                   ModuleName = "Package executor",
                   ProgressCost = 0
                });
                throw;
            }
        }

        public void Rollback(string transactionName)
        {
            _context.Log.AddLogInformation("Вход в Package.Rollback(string)");
            Transaction transaction = GetTransactionByName(transactionName);
            transaction.Rollback();
            _context.Log.AddLogInformation("Выход из Package.Rollback(string)");
        }

        public void Rollback()
        {
            _context.Log.AddLogInformation("Вход в Package.Rollback()");
            foreach (Transaction transaction in _transactions)
            {
                transaction.Rollback();
            }
            _context.Log.AddLogInformation("Выход из Package.Rollback()");
        }

        private Transaction GetTransactionByName(string transactionName)
        {
            Transaction transaction = _transactions.Where(t => t.TransactionName == transactionName).FirstOrDefault();
            if (transaction == null)
            {
                throw new ArgumentException(MESSAGE_UNEXISTENT_TRANSACTION);
            }
            return transaction;
        }

        public void Commit()
        {
            _context.Log.AddLogInformation("Вход в Package.Commit()");
            foreach (Transaction transaction in _transactions)
            {
                transaction.Commit();
            }
            _context.Log.AddLogInformation("Выход из Package.Commit()");
        }

        public void AddTransaction(Transaction transaction)
        {
            _context.Log.AddLogInformation("Вход в Package.AddTransaction(Transaction)");
            _transactions.Add(transaction);
            _context.Log.AddLogInformation("Выход из Package.AddTransaction(Transaction)");
        }

        ~Package()
        {
            if (_content != null)
            {
                _content.Clean();
            }
        }
    }
}
