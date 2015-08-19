using System;
using System.Windows.Forms;
using System.Configuration;
using Gin.Context;
using Gin.Logging;
using System.IO;

namespace Gin.Installer
{
    public class ProgramStarter
    {
        public void Start(string[] args)
        {
            IExecutionContext context = CreateExecutionContext();
            try
            {
                context.Log.AddLogInformation("ProgramStarter. Создан контекст выполнения");
                string fileName = GetFileNameFromArgs(args);
                context.Log.AddLogInformation("ProgramStarter. Путь к файлу пакета, полученному из аргументов командной строки<" + fileName + ">");

                LoadPackageForm loadForm = new LoadPackageForm(context, fileName);
                context.Log.AddLogInformation("ProgramStarter. Создана форма предзагрузки метаданных. Запускаем ее.");

                Application.Run(loadForm);
                context.Log.AddLogInformation("ProgramStarter. Произошел выход из формы предзагрузки метаданных. Создаем главную форму.");

                MainForm mainForm = new MainForm();
                context.Log.AddLogInformation("ProgramStarter. Создана главная форма. Инициализируем ее.");
                mainForm.InitFromWaitForm(context, loadForm.ExecutedPackage);
                context.Log.AddLogInformation("ProgramStarter. Запускаем главную форму.");
                Application.Run(mainForm);
                context.Log.AddLogInformation("ProgramStarter. Произошел выход из главной формы. Завершаем приложение.");
                Application.Exit();
            }
            catch (Exception ex)
            {
                context.Log.AddLogInformation("Поймано исключение в ProgramStarter.Start(). Подробности смотрите далее.");
                context.Log.AddLogException(ex);
            }
        }

        private IExecutionContext CreateExecutionContext()
        {
            string rootPath = ConfigurationManager.AppSettings["ROOT_PATH"];
            string logPath = ConfigurationManager.AppSettings["LOG_PATH"];
            IExecutionContext context = new ExecutionContext(rootPath);
            Logging.Logging log = new Logging.Logging();
            log.AutoFlushLoggers = true;
            log.AddLogger(new ExecutionLoggerTextFile(logPath));
            context.Log = log;
            return context;
        }

        private string GetFileNameFromArgs(string[] args)
        {
            string fileName = null;
            if (args.Length == 1)
            {
                fileName = args[0].Trim();
            }
            if (!File.Exists(fileName))
            {
                fileName = null;
            }
            return fileName;
        }
    }
}
