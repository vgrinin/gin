using System;
using System.Windows.Forms;
using Gin.Context;
using Gin.Logging;
using Gin.Util;
using System.Configuration;

namespace Gin.Installer
{
    public partial class MainForm : Form, IAdjustableControl
    {

        private const string DEFAULT_PRODUCT_NAME = "Gin installer";
        private IExecutionContext _context;
        private Package _package;

        public MainForm()
        {
            InitializeComponent();
            Text = DEFAULT_PRODUCT_NAME;
            MainFormController controller = new MainFormController();
            controller.FormStateChanged += new Action<MainFormState>(formStateChanged);
        }

        void formStateChanged(MainFormState obj)
        {
            switch (obj)
            {
                case MainFormState.PackageExecuting:
                    buttonBrowse.Enabled = false;
                    linkLabel1.Enabled = true;
                    break;
                case MainFormState.Idle:
                    buttonBrowse.Enabled = true;
                    linkLabel1.Enabled = false;
                    break;
                default:
                    buttonBrowse.Enabled = true;
                    linkLabel1.Enabled = true;
                    break;
            }
        }

        public void InitFromWaitForm(IExecutionContext context, Package package)
        {
            _context = context;
            _context.Log.SetProgress(new ExecutionProgressWindowsForms(MainProgressBar, MainProgressLabel));
            _context.ControlContainer = panelMain;
            _package = package;
        }

        private IExecutionContext CreateExecutionContext()
        {
            string rootPath = ConfigurationManager.AppSettings["ROOT_PATH"];
            string logPath = ConfigurationManager.AppSettings["LOG_PATH"];
            IExecutionContext context = new ExecutionContext(rootPath);
            context.ControlContainer = panelMain;
            Logging.Logging log = new Logging.Logging();
            log.AutoFlushLoggers = true;
            log.AddLogger(new ExecutionLoggerTextFile(logPath));
            log.AddLogger(new ExecutionLoggerMessageBox(this));
            log.SetProgress(new ExecutionProgressWindowsForms(MainProgressBar, MainProgressLabel));
            context.Log = log;
            return context;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _context.Log.AddLogInformation("Вход в MainForm.FormLoad()");
            try
            {
                panelCancel.Visible = false;
                if (_package != null)
                {
                    _context.Log.AddLogInformation("Есть пакет для запуска");
                    buttonBrowse.Enabled = false;
                    panelCancel.Visible = true;
                    Action action = new Action(_package.Execute);
                    _context.Log.AddLogInformation("Запускаем пакет");
                    ExecutePackageAsyncState state = new ExecutePackageAsyncState()
                    {
                        action = action,
                        package = _package
                    };
                    action.BeginInvoke(execCallback, state);
                }
                else
                {
                    _context.Log.AddLogInformation("Нет пакета для запуска");
                    buttonBrowse.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                _context.Log.AddLogInformation("Поймано исключение в MainForm.FormLoad. Подробности смотрите далее.");
                _context.Log.AddLogException(ex);
                Win32Util.ShowError(this, "Ошибка при выполнении пакета. Подробности можно найти в журнале инсталлятора.");
            }

            _context.Log.AddLogInformation("Выход из MainForm.FormLoad()");
        }

        private void LoadAndExecute(string packagePath)
        {
            _context.Log.AddLogInformation("Вход в LoadAndExecute('" + packagePath + "')");
            try
            {
                buttonBrowse.Enabled = false;
                panelCancel.Visible = true;
                _package = new Package(_context);
                panelCancel.Visible = true;
                buttonBrowse.Enabled = false;
                _context.Log.AddLogInformation("Начинаем загрузку пакета <" + packagePath + ">");
                Func<string, string> load = new Func<string, string>(_package.Load);
                load.BeginInvoke(packagePath, loadCallback, load);
            }
            catch (Exception ex)
            {
                _context.Log.AddLogInformation("Поймано исключение в LoadAndExecute. Подробности смотрите далее.");
                _context.Log.AddLogException(ex);
                Win32Util.ShowError(this, "Ошибка при выполнении пакета. Подробности можно найти в журнале инсталлятора.");
            }
            _context.Log.AddLogInformation("Выход из LoadAndExecute()");
        }

        void loadCallback(IAsyncResult result)
        {
            _context.Log.AddLogInformation("Вход в loadCallback");
            try
            {
                Func<string, string> load = (Func<string, string>)result.AsyncState;
                load.EndInvoke(result);

                _context.Log.AddLogInformation("Пакет загружен.");
                Action exec = new Action(_package.Execute);
                ExecutePackageAsyncState state = new ExecutePackageAsyncState()
                {
                    action = exec,
                    package = _package
                };
                _context.Log.AddLogInformation("Начинаем выполнение пакета");
                exec.BeginInvoke(execCallback, state);
            }
            catch (PackageExecutionCancelledException)
            {
                Win32Util.ExecuteOrInvoke(this, () =>
                {
                    buttonBrowse.Enabled = true;
                    panelCancel.Visible = false;
                    MessageBox.Show(this, "Прервано пользователем", "Ошибка при выполнении пакета", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    panelMain.Controls.Clear();
                });
            }
            catch (Exception ex)
            {
                _context.Log.AddLogInformation("Поймано исключение в loadCallback. Подробности смотрите далее.");
                _context.Log.AddLogException(ex);
                Win32Util.ShowError(this, "Ошибка при выполнении пакета. Подробности можно найти в журнале инсталлятора.");
            }
            _context.Log.AddLogInformation("Выход из loadCallback");
        }
        
        void execCallback(IAsyncResult result)
        {
            _context.Log.AddLogInformation("Вход в execCallback");
            try
            {
                ExecutePackageAsyncState state = (ExecutePackageAsyncState)result.AsyncState;
                state.action.EndInvoke(result);
                Win32Util.ExecuteOrInvoke(this, () =>
                {
                    buttonBrowse.Enabled = true;
                    panelCancel.Visible = false;
                    MessageBox.Show(this, "Пакет выполнен успешно", "Выполнение пакета завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    panelMain.Controls.Clear();
                });
            }
            catch (Exception ex)
            {
                _context.Log.AddLogInformation("Поймано исключение в execCallback. Подробности смотрите далее.");
                _context.Log.AddLogException(ex);
                Win32Util.ShowError(this, "Ошибка при выполнении пакета. Подробности можно найти в журнале инсталлятора.");
            }
            _context.Log.AddLogInformation("Выход из execCallback");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите закрыть инсталлятор?", "Закрыть программу", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            _context.Log.AddLogInformation("Вход в buttonBrowse_Click");
            string fileName = null;
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Инсталляционные пакеты(*.gin)|*.gin|Все файлы(*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    fileName = dlg.FileName;
                    _context = CreateExecutionContext();
                    _context.Log.AddLogInformation("Будем загружать пакет <" + dlg.FileName + ">");
                    LoadAndExecute(fileName);
                }
            }
            catch (Exception ex)
            {
                _context.Log.AddLogInformation("Поймано исключение в buttonBrowse_Click. Подробности смотрите далее.");
                _context.Log.AddLogException(ex);
                _context.Log.AddLogError("Не удалось загрузить файл '" + fileName + "'. Возможно он не существует или не является инсталляционным пакетом.");
                buttonBrowse.Enabled = true;
            }
            _context.Log.AddLogInformation("Выход из buttonBrowse_Click");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _context.Log.AddLogInformation("Вход в linkLabel1_LinkClicked");
            try
            {
                if (MessageBox.Show("Вы действительно хотите остановить загрузку пакета?", "Прекращение загрузки", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                {
                    _context.Log.SetPendingCancel();
                }
            }
            catch (Exception ex)
            {
                _context.Log.AddLogInformation("Поймано исключение в LinkClicked. Подробности смотрите далее.");
                _context.Log.AddLogException(ex);
            }
            _context.Log.AddLogInformation("Выход из linkLabel1_LinkClicked");
        }

        #region IAdjustableControl Members


        public string Caption
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        int IAdjustableControl.Width
        {
            get
            {
                return this.Width;
            }
            set
            {
                const int FORM_SIDE_BORDERS = 6;
                this.Width = value + FORM_SIDE_BORDERS;
            }
        }

        int IAdjustableControl.Height
        {
            get
            {
                return panelMain.Height;
            }
            set
            {
                const int FORM_CAPTION_HEIGHT = 33;
                this.Height = value + TopSplitContainer.Panel1.Height + MainSplitContainer.Panel2.Height + FORM_CAPTION_HEIGHT;
            }
        }

        #endregion
    }

    internal class ExecutePackageAsyncState
    {
        public Action action;
        public Package package;
    }
}
