using static Calculator.CalcController;

namespace Calculator
{
    /// <summary>
    /// Encapsulated mathematical operations and validations
    /// </summary>
    public class CalcMath : ICalcMath
    {
        private readonly int _maxOperandValue;

        public int MaxOperandLength { get; } = 6;

        public CalcMath()
        {
            _maxOperandValue = (1 << MaxOperandLength) - 1;
        }

        public CalcResult ExecuteBinaryOperation(int a, int b, Operator op)
        {
            try
            {
                var result = op switch
                {
                    Operator.Add => a + b,
                    Operator.Subtract => a - b,
                    _ => throw new NotImplementedException(),
                };

                if (result < 0)
                {
                    return CalcResult.Fail("NEG");
                }

                if (result > _maxOperandValue)
                {
                    return CalcResult.Fail("TOO BIG");
                }

                return CalcResult.Success(result);
            }
            catch (OverflowException)
            {
                return CalcResult.Fail("NEG");
            }
        }

        /// <returns>true when valid</returns>
        public bool CheckOperand(int operand)
        {
            return operand <= _maxOperandValue;
        }
    }
}
