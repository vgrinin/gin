using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gin.Context;
using Gin.Logging;
using System.Threading;


namespace Gin.WCF
{
    public class RemotePackage
    {

        public const string COOKIES_PATH = @"cookies";
        public const string PACKAGE_FILENAME = @"package.gin";
        private Thread _worker;
        private Package _package;
        private IExecutionContext _context;
        private object _locker = new object();
        private RemotePackageState _state = new RemotePackageState();
        private ExecutionProgress _progressProxy;
        private ExecutionProgressInfo _progressInfo;

        private string _ginRoot;
        private string _packageFilePath;
        private string _cookie;
        private string _cookiePath;

        /*
        public RemotePackage(string ginRoot, string cookie, string packageFilePath)
        {
            _ginRoot = ginRoot;
            _cookie = cookie;
            string cookiesPath = Path.Combine(_ginRoot, COOKIES_PATH);
            _cookiePath = Path.Combine(cookiesPath, cookie);
            _packageFilePath = Path.Combine(_cookiePath, PACKAGE_FILENAME);

            if (packageFilePath != null)
            {
                if (File.Exists(packageFilePath))
                {

                    string sourcePackageDirectoryPath = Path.GetDirectoryName(packageFilePath);
                    string includedSourceDirectoryPath = Path.Combine(sourcePackageDirectoryPath, PackageContent.PACKAGE_INCLUDED_FILES_DIRECTORY);
                    string includedDestDirectoryPath = Path.Combine(_cookiePath, PackageContent.PACKAGE_INCLUDED_FILES_DIRECTORY);
                    Directory.CreateDirectory(_cookiePath);
                    File.Copy(packageFilePath, _packageFilePath);
                    if (Directory.Exists(includedSourceDirectoryPath))
                    {
                        Directory.CreateDirectory(includedDestDirectoryPath);
                        Gin.Util.IOUtil.CopyDirectory(includedSourceDirectoryPath, _cookiePath, false);
                    }
                }
            }

            _state.State = PackageExecutionState.Preparing;

            _context = CreateContext(cookie);
            _package = new Package(_context);
        }
        */

        public RemotePackage(string ginRoot, string cookie, string packageFilePath)
        {
            _ginRoot = ginRoot;
            _cookie = cookie;
            string cookiesPath = Path.Combine(_ginRoot, COOKIES_PATH);
            _cookiePath = Path.Combine(cookiesPath, cookie);
            if (packageFilePath == null)
            {
                _packageFilePath = Path.Combine(_cookiePath, PackageContent.PackageContent.MAIN_PACKAGE_FILENAME);
            }
            else
            {
                Directory.CreateDirectory(_cookiePath);
                _packageFilePath = packageFilePath;
            }

            _state.State = PackageExecutionState.Preparing;
            _context = CreateContext(cookie);
            _package = new Package(_context);
        }

        private IExecutionContext CreateContext(string cookie)
        {
            IExecutionContext context = new Gin.Context.ExecutionContext(_ginRoot);
            Logging.Logging loggingObject = new Logging.Logging();

            Directory.CreateDirectory(_cookiePath);

            string mainLoggerPath = Path.Combine(_ginRoot, Gin.ExecutionLogger.EXECUTION_LOG_FILENAME);
            ExecutionLogger mainFileLogger = new ExecutionLoggerTextFile(mainLoggerPath);
            loggingObject.AddLogger(mainFileLogger);

            string textLoggerPath = Path.Combine(_cookiePath, Gin.ExecutionLogger.EXECUTION_LOG_FILENAME);
            ExecutionLogger textFileLogger = new ExecutionLoggerTextFile(textLoggerPath);
            loggingObject.AddLogger(textFileLogger);
            _state.LogPath = textLoggerPath;

            _progressProxy = new ExecutionProgressProxy(OnProgress);
            loggingObject.SetProgress(_progressProxy);

            context.Log = loggingObject;

            return context;
        }

        private void OnProgress(ExecutionProgressInfo progressInfo, int current, int totalCost)
        {
            _progressInfo = progressInfo;
            _state.Progress = current * 100 / totalCost;
            _state.ProgressMessage = progressInfo.ModuleName + " : " + progressInfo.Message;
        }

        public void Invoke()
        {
            lock (_locker)
            {
                if (_worker != null)
                {
                    throw new InvalidOperationException("Пакет уже запущен");
                }
                _worker = new Thread(Worker);
                _worker.Start();
            }
        }

        public RemotePackageState GetState()
        {
            return _state;
        }

        public void Abort()
        {
            if (_worker != null)
            {
                _worker.Abort();
            }
            _state.State = PackageExecutionState.Aborted;
        }

        public void Commit()
        {
            _package.LoadPackageInfo(_packageFilePath);
            _package.Commit();
            _state.State = PackageExecutionState.Fixed;
        }

        public void Rollback()
        {
            _package.LoadPackageInfo(_packageFilePath);
            _package.Rollback();
            _state.State = PackageExecutionState.RolledBack;
        }

        public void AutoRollback()
        {
            _package.LoadPackageInfo(_packageFilePath);
            if (DateTime.Now > _package.PackageData.InstallationDate.AddSeconds((double)_package.GetBody().AutoRollback))
            {
                _package.Rollback();
                _state.State = PackageExecutionState.RolledBack;
            }
        }

        private void Worker()
        {
            try
            {
                _state.State = PackageExecutionState.Executing;
                _packageFilePath = _package.Load(_packageFilePath);
                string newPackageFilePath = Path.Combine(_cookiePath, PackageContent.PackageContent.MAIN_PACKAGE_FILENAME);
                File.Copy(_packageFilePath, newPackageFilePath);
                _package.Execute();
                _state.State = PackageExecutionState.Complete;
            }
            catch (Exception e)
            {
                _state.ExecuteException = e;
            }
        }
    }
}
