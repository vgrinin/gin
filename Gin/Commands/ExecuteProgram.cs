using System.Linq;
using Gin.Attributes;
using Gin.Context;
using Gin.Util;
using System.IO;


namespace Gin.Commands
{
    [GinName(Name = "Выполнить программу", Description = "Выполняет файл в отдельном процессе", Group = "Системные")]

    public class ExecuteProgram : Command, IContentCommand, ICanCreateFromFile
    {

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Name = "Исполняемый файл", Description = "Путь к исполняемому файлу программы. Можно использовать шаблоны.")]
        public string ProgramExePath { set; get; }

        [GinArgumentText(Multiline = false, Name = "Аргументы", Description = "Аргументы командной строки")]
        public string Arguments { get; set; }

        [GinArgumentText(AllowTemplates = true, Name = "Имя int-результата", Description = "Контекстное имя переменной, куда будет сохранено целочисленное значение результата выполнения программы (ExitCode)")]
        [GinResult(Result = typeof(int), Kind = CommandResultKind.Primitive, Description = "Целочисленный результат выполнения команды")]
        public string IntResultName { set; get; }

        [GinArgumentText(AllowTemplates = true, Name = "Имя string-результата", Description = "Контекстное имя переменной, куда будет сохранено строковое значение результата выполнения программы (все то, что программы выведет в стандартный выходной поток)")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Строковый результат выполнения команды")]
        public string StringResultName { set; get; }

        [GinArgumentText(AllowTemplates = true, Name = "Имя error-результата", Description = "Контекстное имя переменной, куда будет сохранено строковое значение ошибок выполнения программы (все то, что программы выведет в стандартный выходной поток ошибок)")]
        [GinResult(Result = typeof(string), Kind = CommandResultKind.Primitive, Description = "Строковое значение ошибки выполнения команды")]
        public string StringErrorName { set; get; }

        [GinArgumentEnum(Name = "Тип окна", Description = "Тип окна, в котором будет выполнена программа", ListEnum = typeof(ProgramWindowType))]
        public ProgramWindowType WindowType { get; set; }

        #endregion

        public ExecuteProgram()
        {
            _progressCost = 50;
        }

        public override CommandResult Do(IExecutionContext context)
        {
            string absoluteProgramPath = context.GetStringFrom(ProgramExePath);
            ProcessWrapper proc = new ProcessWrapper(absoluteProgramPath, Arguments, null, WindowType);
            int result = proc.GetExitCode();
            if (IntResultName != null)
            {
                context.SaveResult(IntResultName, result);
            }
            if (StringResultName != null)
            {
                context.SaveResult(StringResultName, proc.StandardOutput);
            }
            if (StringErrorName != null)
            {
                context.SaveResult(StringErrorName, proc.StandardError);
            }
            return CommandResult.Next;
        }

        public string ContentPath
        {
            get
            {
                return ProgramExePath;
            }
            set
            {
                ProgramExePath = value;
            }
        }

        #region ICanCreateFromFile Members

        public bool IsAssumedCommand(string fileName)
        {
            string[] allowedExtensions = new string[] { ".exe", ".com", ".bat", ".cmd", ".msi" };
            string ext = Path.GetExtension(fileName).ToLower();
            bool isAllowedExtension = allowedExtensions.Contains(ext);
            return File.Exists(fileName) && isAllowedExtension;
        }

        public void InitFromFile(string fileName)
        {
            ProgramExePath = fileName;
        }

        #endregion
    }
}
