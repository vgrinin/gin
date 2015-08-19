using System;
using Gin.Attributes;
using Gin.Context;

namespace Gin.Commands
{
    public abstract class CompareStrings: Command
    {

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Name = "Первый операнд", Description = "Что сравниваем")]
        public string FirstOperandName { get; set; }

        [GinArgumentText(AllowTemplates = true, Name = "Второй операнд", Description = "С чем сравниваем")]
        public string SecondOperandName { get; set; }

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранен булев результат проверки")]
        [GinResult(Result = typeof(bool), Kind = CommandResultKind.Primitive, Description = "Результат сравнения строк")]
        public string ResultName { get; set; }

        #endregion 

        public CompareStrings()
        {
            _progressCost = 1;
        }

        protected abstract bool Compare(string operand1, string operand2);

        public override CommandResult Do(IExecutionContext context)
        {
            string operand1 = (string)(context.GetResult(FirstOperandName));
            string operand2 = (string)(context.GetResult(SecondOperandName));

            bool result = Compare(operand1, operand2);
            context.SaveResult(ResultName, result);
            return CommandResult.Next;
        }
    }

    [GinName(Name = "Строка начинается с", Description = "Возвращает истину если строка начинается с заданной", Group = "Данные")]
    public class StringStartsWith : CompareStrings
    {
        protected override bool Compare(string operand1, string operand2)
        {
            return operand1.StartsWith(operand2);
        }
    }

    [GinName(Name = "Строка оканчивается с", Description = "Возвращает истину если строка оканчивается заданной", Group = "Данные")]
    public class StringEndsWith : CompareStrings
    {
        protected override bool Compare(string operand1, string operand2)
        {
            return operand1.EndsWith(operand2);
        }
    }

    [GinName(Name = "Строка содержит", Description = "Возвращает истину если строка содержит заданную", Group = "Данные")]
    public class StringContains : CompareStrings
    {
        protected override bool Compare(string operand1, string operand2)
        {
            return operand1.Contains(operand2);
        }
    }

    [GinName(Name = "Строка равна", Description = "Возвращает истину если строка равна заданной", Group = "Данные")]
    public class StringEquals : CompareStrings
    {
        protected override bool Compare(string operand1, string operand2)
        {
            return operand1.Equals(operand2);
        }
    }

    [GinName(Name = "Строка не равна", Description = "Возвращает истину если строка не равна заданной", Group = "Данные")]
    public class StringNotEquals : CompareStrings
    {
        protected override bool Compare(string operand1, string operand2)
        {
            return !operand1.Equals(operand2);
        }
    }

    [GinName(Name = "Строка пустая", Description = "Возвращает истину если первый операнд не существует или оказался пустой строкой", Group = "Данные")]
    public class StringIsEmpty : CompareStrings
    {
        protected override bool Compare(string operand1, string operand2)
        {
            return String.IsNullOrEmpty(operand1);
        }
    }

}
