using System;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Reflection;
using Gin.Context;
using Gin.Logging;
using Gin.Util;


namespace Gin.Installer
{
    public partial class LoadPackageForm : Form
    {

        private IExecutionContext _executionContext;
        private string _packageFileName;

        public LoadPackageForm(IExecutionContext context, string packageFileName)
        {
            try
            {
                context.Log.AddLogInformation("Вход в LoadPackageFor.ctor");
                InitializeComponent();
                _packageFileName = packageFileName;
                _executionContext = context;
                _executionContext.Log.SetProgress(new ExecutionProgressWindowsForms(progressWait, labelMessage));
                _executionContext.Log.ProgressTotalCost = 100;
                context.Log.AddLogInformation("Выход из LoadPackageFor.ctor");
            }
            catch (Exception ex)
            {
                _executionContext.Log.AddLogInformation("Поймано исключение в LoadPackageFor.ctor. Подробности смотрите далее.");
                _executionContext.Log.AddLogException(ex);
                CloseForm();
            }
        }

        public Package ExecutedPackage { get; private set; }

        private void LoadPackageForm_Load(object sender, EventArgs e)
        {
            try
            {
                _executionContext.Log.AddLogInformation("Вход в LoadPackageFor.FormLoad. Вызов CreateGinRoot().");
                CreateGinRoot();
                _executionContext.Log.AddLogInformation("CreateGinRoot() отработал. Вызов InvokeInitMetaData().");
                InvokeInitMetaData();
                _executionContext.Log.AddLogInformation("InvokeInitMetaData() отработал. Выход из LoadPackageFor.FormLoad");
            }
            catch (Exception ex)
            {
                _executionContext.Log.AddLogInformation("Поймано исключение в LoadPackageFor.FormLoad. Подробности смотрите далее.");
                _executionContext.Log.AddLogException(ex);
                CloseForm();
            }
        }

        private void CreateGinRoot()
        {
            string ginPath = ConfigurationManager.AppSettings["ROOT_PATH"];
            _executionContext.Log.AddLogInformation("ROOT_PATH = <" + ginPath + ">;");
            if (!Directory.Exists(ginPath))
            {
                Directory.CreateDirectory(ginPath);
            }
        }

        private void InvokeInitMetaData()
        {
            Action initAction = new Action(InitMetadata);
            IAsyncResult initState = initAction.BeginInvoke(InitMetadataCompleted, initAction);
        }

        private void InitMetadata()
        {
            _executionContext.Log.AddLogInformation("Вход в InitMetadata()");
            LogMessage("Загружаются метаданные...");
            string exePath = GetExePath();
            string pluginPath = Path.Combine(exePath, @"Plugins");
            _executionContext.Log.AddLogInformation("exePath = <" + exePath + ">; pluginPath = <" + pluginPath + ">;");
            _executionContext.Log.AddLogInformation("Запрашиваем экземпляр метаданных");
            GinMetaData metaData = GinMetaData.GetInstance();
            _executionContext.Log.AddLogInformation("Получили экземпляр метаданных");
            _executionContext.Log.AddLogInformation("Команд - " + (metaData.Commands != null ? metaData.Commands.Count.ToString() : "0"));
            _executionContext.Log.AddLogInformation("Типов - " + (metaData.IncludedTypes != null ? metaData.IncludedTypes.Length.ToString() : "0"));
            _executionContext.Log.AddLogInformation("Загружаем плагины из папки <" + pluginPath + ">");
            metaData.Plugin(pluginPath);
            _executionContext.Log.AddLogInformation("Команд - " + (metaData.Commands != null ? metaData.Commands.Count.ToString() : "0"));
            _executionContext.Log.AddLogInformation("Типов - " + (metaData.IncludedTypes != null ? metaData.IncludedTypes.Length.ToString() : "0"));
            GinSerializer.IncludeTypes(metaData.IncludedTypes);
            _executionContext.Log.AddLogInformation("Загрузили типы в GinSerializer");
            LogMessage("Метаданные загружены");
            _executionContext.Log.AddLogInformation("Выход из InitMetadata()");
        }

        private void InitMetadataCompleted(IAsyncResult state)
        {
            _executionContext.Log.AddLogInformation("Вход в InitMetadataCompleted()");
            try
            {
                Action initAction = (Action)state.AsyncState;
                initAction.EndInvoke(state);
                _executionContext.Log.AddLogInformation("Вызов InvokeLoadMainPackage()");
                InvokeLoadMainPackage();
            }
            catch (Exception ex)
            {
                _executionContext.Log.AddLogInformation("Поймано исключение в InitMetadataCompleted. Подробности смотрите далее.");
                _executionContext.Log.AddLogException(ex);
                Win32Util.ShowError(this, "Не удалось загрузить метаданные. Дальнейшая работа инсталляционного пакета невозможна. Подробности случившегося сбоя смотрите в лог-файле");
                CloseForm();
            }
            _executionContext.Log.AddLogInformation("Выход из InitMetadataCompleted()");
        }

        private void InvokeLoadMainPackage()
        {
            Win32Util.ExecuteOrInvoke(this, () =>
            {
                panelCancel.Visible = true;
            });
            Action actionLoadMain = new Action(LoadMainPackage);
            actionLoadMain.BeginInvoke(LoadMainPackageCallback, actionLoadMain);
        }

        private void LoadMainPackage()
        {
            _executionContext.Log.AddLogInformation("Вход в LoadMainPackage()");
            if (_packageFileName == null)
            {
                string exePath = GetExePath();
                string packagePath = Path.Combine(exePath, "package.gin");
                _packageFileName = packagePath;
                _executionContext.Log.AddLogInformation("Главный пакет ожидаем в <" + _packageFileName + ">");
            }
            if (File.Exists(_packageFileName))
            {
                _executionContext.Log.AddLogInformation("Файл главного пакета найден.");
                LogMessage("Загружается пакет...");
                ExecutedPackage = new Package(_executionContext);
                try
                {
                    _executionContext.Log.AddLogInformation("Начинаем загрузку.");
                    ExecutedPackage.Load(_packageFileName);
                    _executionContext.Log.AddLogInformation("Загрузка окончена.");
                }
                catch (PackageExecutionCancelledException)
                {
                    _executionContext.Log.AddLogInformation("Пользователь остановил загрузку.");
                    ExecutedPackage = null;
                    Win32Util.ShowError(this, "Загрузка пакета остановлена пользователем");
                    CloseForm();
                }
                catch(Exception ex)
                {
                    _executionContext.Log.AddLogInformation("Исключение при работе метода LoadMainPackage(). Подробности смотрите далее.");
                    _executionContext.Log.AddLogException(ex);
                    ExecutedPackage = null;
                    Win32Util.ShowError(this, "Не удалось запустить инсталляционный пакет, однако вы можете попробовать запустить другой инсталляцонный пакет. При повторении ошибки обратитесь к разработчику. Подробности случившегося сбоя смотрите в лог-файле");
                }
            }
            _executionContext.Log.AddLogInformation("Закрываем форму загрузки.");
            CloseForm();
            _executionContext.Log.AddLogInformation("Выход из LoadMainPackage()");
        }

        private void LoadMainPackageCallback(IAsyncResult state)
        {
            _executionContext.Log.AddLogInformation("Вход в LoadMainPackageCallback()");
            try
            {
                Action action = (Action)state.AsyncState;
                action.EndInvoke(state);
            }
            catch(Exception ex)
            {
                _executionContext.Log.AddLogInformation("Исключение при работе метода LoadMainPackageCallback(). Подробности смотрите далее.");
                _executionContext.Log.AddLogException(ex);
                Invoke(new Action(() =>
                {
                    ExecutedPackage = null;
                    Win32Util.ShowError(this, "Ошибка при загрузке пакета");
                }), null);
            }
            _executionContext.Log.AddLogInformation("Выход из LoadMainPackageCallback()");
        }

        #region Вспомогательные методы
        private string GetExePath()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string workPath = Path.GetDirectoryName(exePath);
#if DEBUG
            return Directory.GetParent(Directory.GetParent(workPath).FullName).FullName;
#else
            return workPath;
#endif
        }

        private void LogMessage(string message)
        {
            Win32Util.ExecuteOrInvoke(this, () =>
            {
                labelMessage.Text = message;
            });
        }

        private void CloseForm()
        {
            Win32Util.ExecuteOrInvoke(this, Close);
        }
        #endregion

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы действительно хотите остановить загрузку пакета?", "Прекращение загрузки", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                {
                    ExecutedPackage = null;
                    _executionContext.Log.SetPendingCancel();
                }
            }
            catch (Exception ex)
            {
                _executionContext.Log.AddLogInformation("Поймано исключение в LinkClicked. Подробности смотрите далее.");
                _executionContext.Log.AddLogException(ex);
                CloseForm();
            }

        }
    }
}
