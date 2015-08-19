using System;

namespace Gin.Commands
{
    public abstract class NumericOperand
    {
        public static NumericOperand Create(object operand)
        {
            if (operand is ulong)
            {
                return new ULongCompareOperand
                {
                    Value = (ulong)operand
                };
            }
            if (operand is double)
            {
                return new DoubleCompareOperand
                {
                    Value = (double)operand
                };
            }
            if (operand is decimal)
            {
                return new DecimalCompareOperand
                {
                    Value = (decimal)operand
                };
            }
            if (operand is float)
            {
                return new FloatCompareOperand
                {
                    Value = (float)operand
                };
            }
            if (operand is sbyte ||
                operand is byte ||
                operand is char ||
                operand is short ||
                operand is ushort ||
                operand is int ||
                operand is uint ||
                operand is long)
            {
                return new LongCompareOperand
                {
                    Value = (long)operand
                };
            }
            return null;
        }

        public abstract int Subtract(NumericOperand operand2);

        public static int operator -(NumericOperand operand1, NumericOperand operand2)
        {
            return operand1.Subtract(operand2);
        }
    }

    public class FloatCompareOperand : NumericOperand
    {
        public float Value { get; set; }

        public override int Subtract(NumericOperand operand2)
        {
            return Math.Sign(Value - ((FloatCompareOperand)operand2).Value);
        }
    }

    public class DoubleCompareOperand : NumericOperand
    {
        public double Value { get; set; }

        public override int Subtract(NumericOperand operand2)
        {
            return Math.Sign(Value - ((DoubleCompareOperand)operand2).Value);
        }
    }

    public class DecimalCompareOperand : NumericOperand
    {
        public decimal Value { get; set; }

        public override int Subtract(NumericOperand operand2)
        {
            return Math.Sign(Value - ((DecimalCompareOperand)operand2).Value);
        }

    }

    public class LongCompareOperand : NumericOperand
    {
        public long Value { get; set; }

        public override int Subtract(NumericOperand operand2)
        {
            return Math.Sign(Value - ((LongCompareOperand)operand2).Value);
        }
    }

    public class ULongCompareOperand : NumericOperand
    {
        public ulong Value { get; set; }

        public override int Subtract(NumericOperand operand2)
        {
            return Math.Sign((double)(Value - ((ULongCompareOperand)operand2).Value));
        }

    }
}