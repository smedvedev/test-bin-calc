namespace Calculator
{
    public interface ICalcMath
    {
        int MaxOperandLength { get; }

        bool CheckOperand(int operand);
        CalcResult ExecuteBinaryOperation(int a, int b, CalcController.Operator op);
    }
}