using System.Collections.Generic;
using System.Linq;
using Gin.Attributes;
using Gin.Context;
using Gin.Transactions;
using Gin.Util;
using System.IO;


namespace Gin.Commands
{

    public class StringSubstitute
    {
        [GinArgumentText(Name = "Шаблон", Description = "Строка, которая будет заменена в исходном тексте")]
        public string Key { get; set; }
        [GinArgumentText(Name = "Значение", Description = "Строка, на которую будет заменен искомый шаблон")]
        public string Value { get; set; }

        public override string ToString()
        {
            return Key + "=>" + Value;
        }
    }

    [GinName(Name = "Создать текстовый файл", Description = "Создает текстовый файл с возможностью подстановки содержимого", Group = "Файловые операции")]
    public class CreateTextFile : TransactionalCommand, IContentCommand, ICanCreateFromFile
    {

        #region Аргументы команды

        /// <summary>
        /// Начальное содержимое файла, в котором подстановочные шаблоны, заключенные в фигурные скобки {SUBSTITUTE_NAME}, впоследствии будут заменены на значения подстановок SUBSTITUTE_VALUE, из массива Substitutes
        /// </summary>
        [GinArgumentText(AllowTemplates = true, Name = "Исходный файл", Description = "Путь к исходному файлу в котором подстановочные шаблоны, заключенные в фигурные скобки {SUBSTITUTE_NAME}, впоследствии будут заменены на значения подстановок SUBSTITUTE_VALUE, из массива подстановок")]
        public string SourceFilePath { get; set; }

        /// <summary>
        /// Массив подстановок
        /// </summary>
        [GinArgumentList(ListType = typeof(List<StringSubstitute>), Name = "Подстановки", Description = "Массив подстановок, в котором мы указываем какие подстановки нужно сделать в исходном текстовом файле. В значениях подстановок можно использовать шаблоны.")]
        public List<StringSubstitute> Substitutes { get; set; }

        /// <summary>
        /// Путь, куда будет сохранен готовый текстовый файл
        /// </summary>
        [GinArgumentText(AllowTemplates = true, Name = "Результирующий файл", Description = "Путь к результирующему файлу. Можно использовать шаблоны.")]
        public string DestFilePath { get; set; }

        #endregion

        public CreateTextFile()
        {
            Substitutes = new List<StringSubstitute>();
        }

        public override CommandResult Do(IExecutionContext context, Transaction transaction)
        {
            if (transaction != null)
            {
                SingleFileStep step = transaction.CreateStep<SingleFileStep>(this);
                string absoluteDestFilePath = context.GetStringFrom(DestFilePath);
                step.Init(context, absoluteDestFilePath);
            }
            Do(context);
            return CommandResult.Next;
        }

        public override void Rollback(TransactionStep step)
        {
            step.Rollback();
        }

        public override void Commit(TransactionStep step)
        {
            step.Commit();
        }

        public override CommandResult Do(IExecutionContext context)
        {
            string absoluteSourceFilePath = context.GetStringFrom(SourceFilePath);
            string fileContent = IOUtil.ReadFile(absoluteSourceFilePath);
            foreach (StringSubstitute subst in Substitutes)
            {
                string absoluteSubstValue = context.GetStringFrom(subst.Value);
                fileContent = fileContent.Replace(subst.Key, absoluteSubstValue);
            }
            string absoluteDestFilePath = context.GetStringFrom(DestFilePath);
            IOUtil.WriteFile(absoluteDestFilePath, fileContent);
            return CommandResult.Next;
        }

        #region IContentCommand Members

        public string ContentPath
        {
            get
            {
                return SourceFilePath;
            }
            set
            {
                SourceFilePath = value;
            }
        }

        #endregion


        #region ICanCreateFromFile Members

        public bool IsAssumedCommand(string fileName)
        {
            string[] allowedExtensions = new[] { ".txt", ".xml", ".conf", ".config", ".ini" };
            string ext = Path.GetExtension(fileName).ToLower();
            bool isAllowedExtension = allowedExtensions.Contains(ext);
            return File.Exists(fileName) && isAllowedExtension;
        }

        public void InitFromFile(string fileName)
        {
            SourceFilePath = fileName;
        }

        #endregion
    }
}
