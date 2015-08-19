using System;
using System.Collections.Generic;
using DTS;
using System.IO;
using Gin.Logging;
using System.Runtime.InteropServices.ComTypes;


namespace Gin.SQL.Util
{
    public class DTSExecutor
    {


        #region

        class PackageEventsSink : PackageEvents
        {

            public void OnQueryCancel(string eventSource, ref bool pbCancel)
            {
            }

            public void OnStart(string eventSource)
            {
            }

            public void OnProgress(string eventSource, string progressDescription, int percentComplete, int progressCountLow, int progressCountHigh)
            {
                //p_OnProgress(EventSource, ProgressDescription, PercentComplete, ProgressCountLow, ProgressCountHigh);
            }

            public void OnError(string eventSource, int errorCode, string source, string description, string helpFile, int helpContext, string dofInterfaceWithError, ref bool pbCancel)
            {
            }

            public void OnFinish(string eventSource)
            {
            }

        }

        #endregion

        #region event handlers

        public event ProgressEvent OnProgress;

        private void ProgressHandler(int percent)
        {
            if (OnProgress != null)
            {
                OnProgress(percent);
            }
        }

        public event LoggerEvent OnLogger;

        private void LoggerHandler(ExecutionLoggerEventType type, string message)
        {
            if (OnLogger != null)
            {
                OnLogger(type, message);
            }
        }

        #endregion

        private const int TOTAL_PROGRESS_COST = 100;
        private readonly string _packageFilePath;
        private readonly string _logFilePath;
        private readonly Dictionary<string, object> _parameters;
        private readonly bool _supressErrors;

        public DTSExecutor(string packageFilePath, string dtsLogFilePath, bool supressErrors)
        {
            _parameters = new Dictionary<string, object>();
            _packageFilePath = packageFilePath;
            _logFilePath = dtsLogFilePath;
            _supressErrors = supressErrors;
        }

        public void SaveParameter(string name, object value)
        {
            _parameters[name] = value;
        }

        public void Execute()
        {
            Package2 dtsPackage = new Package2();
            try
            {
                LoggerHandler(ExecutionLoggerEventType.Information, "Будем загружать DTS-пакет из файла '" + _packageFilePath + "'");
                object pVarPersistStgOfHost = "";
                dtsPackage.LoadFromStorageFile(_packageFilePath, "", "", "", "", ref pVarPersistStgOfHost);
                LoggerHandler(ExecutionLoggerEventType.Information, "Пакет загружен");
                LoggerHandler(ExecutionLoggerEventType.Information, "Готовы задать " + _parameters.Count + " параметров");
                foreach (var parameter in _parameters)
                {
                    dtsPackage.GlobalVariables.Remove(parameter.Key);
                    dtsPackage.GlobalVariables.AddGlobalVariable(parameter.Key, parameter.Value);
                    LoggerHandler(ExecutionLoggerEventType.Information, "Параметр " + parameter.Key + " добавлен к пакету");
                }

                dtsPackage.FailOnError = true;
                if (_logFilePath != null)
                {
                    dtsPackage.LogFileName = _logFilePath;
                }

                IConnectionPointContainer cnnctPtCont = (IConnectionPointContainer)dtsPackage;
                IConnectionPoint cnnctPt;
                PackageEventsSink pes = new PackageEventsSink();
                Guid guid = new Guid("10020605-EB1C-11CF-AE6E-00AA004A34D5");  // UUID of PackageEvents Interface
                cnnctPtCont.FindConnectionPoint(ref guid, out cnnctPt);
                int iCookie;
                cnnctPt.Advise(pes, out iCookie);



                _stepsCount = dtsPackage.Steps.Count;
                LoggerHandler(ExecutionLoggerEventType.Information, "Пакет содержит " + _stepsCount + " шагов. Начинаем выполнять пакет.");
                dtsPackage.Execute();
                LoggerHandler(ExecutionLoggerEventType.Information, "Пакет выполнен");
                dtsPackage.UnInitialize();
                cnnctPt.Unadvise(iCookie); 

                LoggerHandler(ExecutionLoggerEventType.Information, "Пакет Uninitialized");
                if (_logFilePath != null && File.Exists(_logFilePath))
                {
                    string dtsLog = Gin.Util.IOUtil.ReadFile(_logFilePath);
                    LoggerHandler(ExecutionLoggerEventType.Information, dtsLog);
                    LoggerHandler(ExecutionLoggerEventType.Information, "Слили файл DTS-лога в основной лог");
                }
            }
            catch (Exception ex)
            {
                LoggerHandler(ExecutionLoggerEventType.Error, ex.Message + " at: " + ex.StackTrace);
                if (_logFilePath != null && File.Exists(_logFilePath))
                {
                    string dtsLog = Gin.Util.IOUtil.ReadFile(_logFilePath);
                    LoggerHandler(ExecutionLoggerEventType.Error, dtsLog);
                }
                ProgressHandler(TOTAL_PROGRESS_COST);
                if (!_supressErrors)
                {
                    throw;
                }
            }
            finally
            {
                if (_logFilePath != null && File.Exists(_logFilePath))
                {
                    File.Delete(_logFilePath);
                }
            }
        }

        private int _stepsCount;
    }

}
