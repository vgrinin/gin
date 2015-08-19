using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Security;
using Gin.Attributes;

namespace Gin.Util
{
    /// <summary>
    /// Класс описывает пользователя от имени которого запускается процесс
    /// </summary>
    public class RunAsUser
    {
        public string Name;
        public string Password;
        public string Domain;
    }


    public enum ProgramWindowType
    {
        [GinName(Name = "Стандартное окно")]
        WinForms,
        [GinName(Name = "Консоль")]
        Console,
        [GinName(Name = "Скрыто")]
        Hidden
    }

    /// <summary>
    /// Процесс, инкапсулирующий исполняемый файл
    /// </summary>
    public class ProcessWrapper
    {
        // Здесь живет ссылка на процесс
        private Process _process;

        private ProgramWindowType windowType;

        public string StandardOutput { get; private set; }
        public string StandardError { get; private set; }


        /// <summary>
        /// Создает процесс, в котором инкапсулирован исполняемый файл, 
        /// позволяет получать выходную текстовую информацию процесса
        /// </summary>
        /// <param name="fileName">Путь к исполняемому файлу</param>
        /// <param name="arguments">Аргументы запуска файла</param>
        /// <param name="user">Пользователь от имени которого запускаем файл</param>
        public ProcessWrapper(string fileName, string arguments, RunAsUser user, ProgramWindowType type)
        {
            windowType = type;
            _process = new Process();
            _process.StartInfo.FileName = fileName;
            _process.StartInfo.Arguments = arguments;
            _process.StartInfo.CreateNoWindow = type == ProgramWindowType.Hidden;
            _process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            _process.StartInfo.RedirectStandardOutput = type == ProgramWindowType.Hidden;
            _process.StartInfo.UseShellExecute = type != ProgramWindowType.Hidden;
            _process.StartInfo.RedirectStandardError = type == ProgramWindowType.Hidden;
            _process.StartInfo.RedirectStandardInput = type == ProgramWindowType.Hidden;
            if (type == ProgramWindowType.Hidden)
            {
                _process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                _process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            }
            _process.StartInfo.ErrorDialog = type != ProgramWindowType.Hidden;

            if (user != null)
            {
                _process.StartInfo.UserName = user.Name;
                string password = user.Password;
                SecureString secPassword = new SecureString();
                for (int i = 0; i < password.Length; i++)
                {
                    secPassword.AppendChar(password[i]);
                }
                _process.StartInfo.Password = secPassword;
                _process.StartInfo.Domain = user.Domain;
            }
        }
        /// <summary>
        /// Запускает процесс и возвращает результат его выполнения
        /// </summary>
        /// <returns></returns>
        public int GetExitCode()
        {
            _process.Start();
            _process.WaitForExit();

            if (windowType == ProgramWindowType.Hidden)
            {
                StandardOutput = _process.StandardOutput.ReadToEnd();
                StandardError = _process.StandardError.ReadToEnd();
            }
            return _process.ExitCode;

        }
    }
}
