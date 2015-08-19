using Gin.Attributes;
using Gin.Context;

namespace Gin.Commands
{
    [GinIgnoreType()]
    public abstract class CompareNumbers: Command
    {

        #region Аргументы команды

        [GinArgumentText(AllowTemplates = true, Name = "Первый операнд", Description = "Что сравниваем")]
        public string FirstOperandName { get; set; }

        [GinArgumentText(AllowTemplates = true, Name = "Второй операнд", Description = "С чем сравниваем")]
        public string SecondOperandName { get; set; }

        [GinArgumentText(Name = "Имя результата", Description = "Контекстное имя переменной, куда будет сохранен булев результат проверки")]
        [GinResult(Result = typeof(bool), Kind = CommandResultKind.Primitive, Description = "true/false")]
        public string ResultName { get; set; }

        #endregion

        public override CommandResult Do(IExecutionContext context)
        {
            bool result = Compare(Subtract(context));
            context.SaveResult(ResultName, result);
            return CommandResult.Next;
        }

        protected abstract bool Compare(int compareResult);

        private int Subtract(IExecutionContext context)
        {
            NumericOperand firstOperand = NumericOperand.Create(context.GetResult(FirstOperandName));
            NumericOperand secondOperand = NumericOperand.Create(context.GetResult(SecondOperandName));
            return (firstOperand - secondOperand);
        }

    }

    [GinName(Name = "Число меньше чем", Description = "Возвращает истину если число меньше заданного", Group = "Данные")]
    public class NumberLessThan : CompareNumbers
    {
        protected override bool Compare(int compareResult)
        {
            return compareResult < 0;
        }
    }

    [GinName(Name = "Число больше чем", Description = "Возвращает истину если число больше заданного", Group = "Данные")]
    public class NumberGreaterThan : CompareNumbers
    {
        protected override bool Compare(int compareResult)
        {
            return compareResult > 0;
        }
    }

    [GinName(Name = "Число равно", Description = "Возвращает истину если число равно заданному", Group = "Данные")]
    public class NumberEquals : CompareNumbers
    {
        protected override bool Compare(int compareResult)
        {
            return compareResult == 0;
        }
    }

    [GinName(Name = "Число меньше чем или равно", Description = "Возвращает истину если число меньше заданного или равно ему", Group = "Данные")]
    public class NumberLessThanOrEquals : CompareNumbers
    {
        protected override bool Compare(int compareResult)
        {
            return compareResult <= 0;
        }
    }

    [GinName(Name = "Число больше чем или равно", Description = "Возвращает истину если число больше заданного или равно ему", Group = "Данные")]
    public class NumberGreaterThanOrEquals : CompareNumbers
    {
        protected override bool Compare(int compareResult)
        {
            return compareResult >= 0;
        }
    }

    [GinName(Name = "Число не равно", Description = "Возвращает истину если число не равно заданному", Group = "Данные")]
    public class NumberNotEquals : CompareNumbers
    {
        protected override bool Compare(int compareResult)
        {
            return compareResult != 0;
        }
    }

}
