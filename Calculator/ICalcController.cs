namespace Calculator
{
    public interface ICalcController
    {
        string DisplayText { get; }
        string ExpressionText { get; }

        void Calculate();
        void ClearAll();
        void ClearEntry();
        void InputDigit(int v);
        void SetOperator(CalcController.Operator op);
    }
}