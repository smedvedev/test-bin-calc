namespace Calculator
{
    public record CalcResult (bool IsSuccess, int Result, string Error)
    {
        public static CalcResult Success(int result)
        {
            return new CalcResult(true, result, string.Empty);
        }

        public static CalcResult Fail(string error)
        {
            return new CalcResult(false, 0, error);
        }
    }
}
