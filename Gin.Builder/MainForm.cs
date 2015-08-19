using System;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using Gin.CommandTree;
using Gin.Commands;
using Gin.Context;
using Gin.Editors;
using Gin.Logging;
using Gin.PackageContent;
using Gin.Util;
using System.Configuration;
using System.Threading;
using System.Diagnostics;
using Gin.Visitors;

namespace Gin.Builder
{
    public partial class MainForm : Form
    {
        private GinMetaData _metaData;
        private string _packageFilePath;
        private CommandTree _tree;

        private const string FOLDER_TEMPLATES = "Шаблоны";
        private const string FOLDER_PACKAGES = "Пакеты";

        public MainForm()
        {
            InitializeComponent();
        }

        private void ExecuteAsyncWithWait(Action method, AutoResetEvent cancelEvent, string message)
        {
            StartScreenForm form = new StartScreenForm(cancelEvent, message);

            method.BeginInvoke(ar =>
            {
                try
                {
                    Win32Util.ExecuteOrInvoke(form, form.Close);
                    method.EndInvoke(ar);
                }
                catch (Exception ex)
                {
                    Win32Util.ShowError(this, "Не удалось выполнить операцию. Описание ошибки приведено далее. \r\n" + ex.Message + "\r\n" + ex.StackTrace);
                }
            }, null);

            if (form.ShowDialog() == DialogResult.Cancel && cancelEvent != null)
            {
                cancelEvent.Set();
            }
        }

        private void InitMetadata()
        {
            string ginPath = ConfigurationManager.AppSettings["ROOT_PATH"];
            if (!Directory.Exists(ginPath))
            {
                Directory.CreateDirectory(ginPath);
            }
            string rootPath = GetRootPath();
            string pluginPath = Path.Combine(rootPath, @"Plugins");
            _metaData = GinMetaData.GetInstance();
            _metaData.Plugin(pluginPath);
            GinSerializer.IncludeTypes(_metaData.IncludedTypes);
        }

        private string GetRootPath()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string workPath = Path.GetDirectoryName(exePath);
#if DEBUG
            return Directory.GetParent(Directory.GetParent(workPath).FullName).FullName;
#else
            return workPath;
#endif

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                ExecuteAsyncWithWait(InitMetadata, null, "Загрузка метаданных");
                listCommands.InitFromCommands(_metaData.Commands);
            }
            catch (Exception ex)
            {
                Win32Util.ShowError(this, "Не удалось запустить редактор пакетов. Описание ошибки приведено далее. \r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void MenuItemOpenPackageClick(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _packageFilePath = dialog.FileName;
                    ExecuteAsyncWithWait(() => LoadPackage(_packageFilePath), null, "Загрузка пакета");
                }
            }
            catch (Exception ex)
            {
                Win32Util.ShowError(this, "Не удалось открыть пакет. Описание ошибки приведено далее. \r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void LoadPackage(string packageFilePath)
        {
            string rootPath = ConfigurationManager.AppSettings["ROOT_PATH"];
            string logPath = ConfigurationManager.AppSettings["LOG_PATH"];
            IExecutionContext context = new Gin.Context.ExecutionContext(rootPath);
            Logging.Logging log = new Logging.Logging();
            log.AutoFlushLoggers = true;
            log.AddLogger(new ExecutionLoggerTextFile(logPath));
            context.Log = log;
            Package pkg = new Package(context);
            SetCurrentStatus("Загрузка пакета");
            SetFilePathStatus(packageFilePath);
            pkg.Load(packageFilePath);
            BuildPackageTree(pkg);
        }

        private void LoadPackageAsync(string packageFilePath)
        {
            string rootPath = ConfigurationManager.AppSettings["ROOT_PATH"];
            string logPath = ConfigurationManager.AppSettings["LOG_PATH"];
            IExecutionContext context = new Gin.Context.ExecutionContext(rootPath);
            Logging.Logging log = new Logging.Logging();
            log.AutoFlushLoggers = true;
            log.AddLogger(new ExecutionLoggerTextFile(logPath));
            context.Log = log;
            Package pkg = new Package(context);
            SetCurrentStatus("Загрузка пакета");
            SetFilePathStatus(packageFilePath);
            pkg.Load(packageFilePath);
            BuildPackageTree(pkg);
        }

        private void BuildPackageTree(Package package)
        {
            Invoke(new Action(() =>
            {
                if (_tree != null)
                {
                    ((IDisposable)_tree).Dispose();
                }
                _tree = new TreeViewCommandTree(treeCommands);
                _tree.LoadTree(package.GetBody());
                _tree.ExpandAll();
                _tree.SelectCommandTreeNode += TreeSelectCommandTreeNode;
            }));
            SetCurrentStatus("Пакет загружен");
        }

        void TreeSelectCommandTreeNode(CommandTreeNode node, PackageBody body, Command command, PropertyInfo property)
        {
            argumentHelp.SetHelp("", "", "", false, null, null);
            panelCommandProperties.SuspendLayout();

            panelCommandProperties.Controls.Clear();

            if (property == null)
            {
                panelCommandProperties.SuspendLayout();
                try
                {
                    if (command == null)
                    {
                        FormsHelper.CreateNodeEditor(node, body, panelCommandProperties, _tree.Body, (propertyProgramName, propertyName, propertyDescription, allowTemplates, editor) => argumentHelp.SetHelp(propertyProgramName, propertyName, propertyDescription, allowTemplates, editor, _tree.Body.GetResultInfos()));
                    }
                    else
                    {
                        FormsHelper.CreateNodeEditor(node, command, panelCommandProperties, _tree.Body, (propertyProgramName, propertyName, propertyDescription, allowTemplates, editor) => argumentHelp.SetHelp(propertyProgramName, propertyName, propertyDescription, allowTemplates, editor, _tree.Body.GetResultInfos()));
                    }
                }
                finally
                {
                    panelCommandProperties.ResumeLayout();
                }
            }

            ResizePropertiesPanelItems();
            panelCommandProperties.ResumeLayout();
        }

        private void menuItemSavePackageAs_Click(object sender, EventArgs e)
        {
            try
            {
                SavePackageAs();
            }
            catch (Exception ex)
            {
                Win32Util.ShowError(this, "Не удалось сохранить пакет. Описание ошибки приведено далее. \r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void menuItemSavePackage_Click(object sender, EventArgs e)
        {
            try
            {
                if (_packageFilePath == null)
                {
                    SavePackageAs();
                }
                else
                {
                    SavePackage(_packageFilePath, PackageContentType.Empty);
                }
            }
            catch (Exception ex)
            {
                Win32Util.ShowError(this, "Не удалось сохранить пакет. Описание ошибки приведено далее. \r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void SavePackageAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".gin";
            dialog.AddExtension = true;
            dialog.FileName = "package.gin";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _packageFilePath = dialog.FileName;
                SavePackage(_packageFilePath, PackageContentType.Empty);
            }
        }

        private void SavePackage(string filePath, PackageContentType type)
        {
            bool isDevelopment = ConfigurationManager.AppSettings.AllKeys.Contains("DEVELOPER") && ConfigurationManager.AppSettings["DEVELOPER"] == "true";
            string templatesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FOLDER_TEMPLATES);
            bool isTemplate = filePath.StartsWith(templatesPath);
            string newFilePath = CorrectPackageFilePath(filePath);

            bool saveFile = true;

            if (!isDevelopment && isTemplate)
            {
                if (MessageBox.Show("Нельзя сохранять пакет в папку с шаблонами. Хотите вместо этого сохранить пакет а папку <" + newFilePath + ">?", "Недопустимое расположение файла", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                {
                    filePath = newFilePath;
                    CreateNewPackageDirectory(filePath);
                    saveFile = true;
                }
                else
                {
                    MessageBox.Show("Пакет не сохранен", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    saveFile = false;
                }
            }

            if (saveFile)
            {
                _packageFilePath = filePath;
                SetFilePathStatus(_packageFilePath);
                PackageBody body = _tree.Body;
                body.ContentType = type;
                PackageBuilder builder = new PackageBuilder(body, type);
                bool saveSuccess = false;
                ExecuteAsyncWithWait((() =>
                {
                    saveSuccess = builder.Save(_packageFilePath, true);
                }), null, "Сохранение пакета");
                if (!saveSuccess)
                {
                    string message = "";
                    foreach (PackageErrorInfo error in builder.Errors)
                    {
                        message += error.Description + "\r\n";
                    }
                    MessageBox.Show(this, "Пакет не сохранен. Обнаружены ошибки: \r\n" + message, "Пакет не сохранен", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private string CorrectPackageFilePath(string filePath)
        {
            string templatesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FOLDER_TEMPLATES);
            if (!filePath.StartsWith(templatesPath))
            {
                return filePath;
            }
            string relativeName = filePath.Substring(templatesPath.Length);
            string fileName = Path.GetFileName(filePath);
            if (relativeName.StartsWith(@"\"))
            {
                relativeName = relativeName.Substring(1);
            }
            string relativePath = Path.GetDirectoryName(relativeName);
            if (relativePath.EndsWith(@"\"))
            {
                relativePath = relativePath.Substring(1);
            }
            relativePath += "_" + DateTime.Now.ToString("yyyyMMdd");

            string packagesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FOLDER_PACKAGES);

            const int MAX_TRY_COUNT = 999;
            int tryCount = 0;
            string newRelativePath = relativePath;
            while (true && tryCount < MAX_TRY_COUNT)
            {
                relativeName = Path.Combine(newRelativePath, fileName);
                string newFilePath = Path.Combine(packagesPath, relativeName);
                if (!File.Exists(newFilePath))
                {
                    filePath = newFilePath;
                    break;
                }
                tryCount++;
                newRelativePath = relativePath + "_" + String.Format("{0:D3}", tryCount);
            }

            return filePath;
        }

        private void CreateNewPackageDirectory(string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        private void MenuItemExportClick(object sender, EventArgs e)
        {
            try
            {
                ChooseExportTypeForm form = new ChooseExportTypeForm();
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SavePackage(_packageFilePath, form.GetResultContentType());
                }
            }
            catch (Exception ex)
            {
                Win32Util.ShowError(this, "Не удалось экспортировать пакет. Описание ошибки приведено далее. \r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void menuItemNewPackage_Click(object sender, EventArgs e)
        {
            try
            {
                PackageBody body = new PackageBody();
                body.Command = new CommandSequence();
                if (_tree != null)
                {
                    ((IDisposable)_tree).Dispose();
                }
                _tree = new TreeViewCommandTree(treeCommands);
                _tree.LoadTree(body);
                _tree.ExpandAll();
                _tree.SelectCommandTreeNode += new SelectCommandTreeNodeDelegate(TreeSelectCommandTreeNode);
                _packageFilePath = null;
                SetFilePathStatus(_packageFilePath);
            }
            catch (Exception ex)
            {
                Win32Util.ShowError(this, "Не удалось создать новый пакет. Описание ошибки приведено далее. \r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void panelCommandProperties_Resize(object sender, EventArgs e)
        {
            ResizePropertiesPanelItems();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть приложение?", "Закрыть", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_packageFilePath))
            {
                Win32Util.ShowError(this, "Откройте сначала пакет, чтобы его выполнить");
                return;
            }
            try
            {
                string workPath = GetRootPath();
                string installerFullPath = Path.Combine(workPath, "Gin.Installer.exe");

                Process process = new Process();
                process.StartInfo.FileName = installerFullPath;
                process.StartInfo.Arguments = @"""" + _packageFilePath + @"""";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.ErrorDialog = true;
                process.Start();
            }
            catch (Exception ex)
            {
                Win32Util.ShowError(this, "Не удалось запустить инсталлятор. Описание ошибки приведено далее. \r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ResizePropertiesPanelItems()
        {
            try
            {
                const int VSCROLLER_WIDTH = 17;

                foreach (Control control in panelCommandProperties.Controls)
                {
                    control.Width = panelCommandProperties.Width - control.Padding.Left - control.Padding.Right - VSCROLLER_WIDTH;
                }
            }
            catch { }
        }
        
        private void SetFilePathStatus(string filePath)
        {
            statusFilePath.Text = filePath;
        }

        private void SetCurrentStatus(string message)
        {
            statusAppStatus.Text = message;
        }

    }
}
