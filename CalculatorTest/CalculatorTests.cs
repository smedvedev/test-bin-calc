using Calculator;
using Xunit;

namespace Binary_calculatorTests
{
    public class CalculatorTests
    {
        private readonly ICalcController _calculator;

        public CalculatorTests()
        {
            // Arrange
            _calculator = new CalcController(new CalcMath());
        }

        [Fact]
        public void Test_InputDigit()
        {
            // Act
            _calculator.InputDigit(1);
            _calculator.InputDigit(0);
            _calculator.InputDigit(1);

            // Assert
            Assert.Equal("101", _calculator.DisplayText);
        }

        [Fact]
        public void Test_SetOperator()
        {
            // Act
            _calculator.InputDigit(1);
            _calculator.SetOperator(CalcController.Operator.Add);

            // Assert
            Assert.Equal("1 Add", _calculator.ExpressionText);
            Assert.Equal("0", _calculator.DisplayText);
        }

        [Fact]
        public void Test_ManyCalculate()
        {
            // Arrange
            _calculator.InputDigit(1);
            _calculator.SetOperator(CalcController.Operator.Add);
            _calculator.InputDigit(1);

            // Act
            _calculator.Calculate();
            _calculator.Calculate();
            _calculator.Calculate();

            // Assert
            Assert.Equal("11 Add 1", _calculator.ExpressionText);
            Assert.Equal("100", _calculator.DisplayText);
        }

        [Fact]
        public void Test_TooBig()
        {
            // Arrange
            Enumerable.Repeat(0, 6).ToList().ForEach(_ => _calculator.InputDigit(1));
            _calculator.SetOperator(CalcController.Operator.Add);
            _calculator.InputDigit(1);

            // Act
            _calculator.Calculate();

            // Assert
            Assert.Equal("TOO BIG", _calculator.DisplayText);
        }

        [Fact]
        public void Test_Neg()
        {
            // Arrange
            _calculator.InputDigit(1);
            _calculator.SetOperator(CalcController.Operator.Subtract);
            _calculator.InputDigit(1);
            _calculator.InputDigit(1);

            // Act
            _calculator.Calculate();

            // Assert
            Assert.Equal("NEG", _calculator.DisplayText);
        }

        [Fact]
        public void Test_ClearAll()
        {
            // Arrange
            _calculator.InputDigit(1);
            _calculator.SetOperator(CalcController.Operator.Add);
            _calculator.InputDigit(1);
            _calculator.Calculate();

            // Act
            _calculator.ClearAll();

            // Assert
            Assert.Equal("0", _calculator.DisplayText);
        }

        [Fact]
        public void Test_ClearEntry()
        {
            // Arrange
            _calculator.InputDigit(1);

            // Act
            _calculator.ClearEntry();

            // Assert
            Assert.Equal("0", _calculator.DisplayText);
        }

        [Fact]
        public void Test_InputLimited()
        {
            // Act
            Enumerable.Repeat(0, 100).ToList().ForEach(_ => _calculator.InputDigit(1));

            // Assert
            Assert.Equal("111111", _calculator.DisplayText);
        }
    }
}
