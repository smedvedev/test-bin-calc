using Calculator;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BinaryCalculator
{
    public class CalculatorViewModel : ObservableObject
    {
        private readonly ICalcController _calculator;

        public RelayCommand<string> AddDigitCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand SubtractCommand { get; }
        public RelayCommand CalculateCommand { get; }
        public RelayCommand ClearAllCommand { get; }
        public RelayCommand ClearEntryCommand { get; }

        public string CurrentNumber => _calculator.DisplayText;
        public string ExpressionText => _calculator.ExpressionText;

        public CalculatorViewModel(ICalcController calculator)
        {
            _calculator = calculator;

            AddDigitCommand = new RelayCommand<string>(HandleDigitInput);
            AddCommand = new RelayCommand(HandleAdd);
            SubtractCommand = new RelayCommand(HandleSubtract);
            CalculateCommand = new RelayCommand(HandleCalculate);
            ClearAllCommand = new RelayCommand(HandleClear);
            ClearEntryCommand = new RelayCommand(HandleClearEntry);
        }

        private void HandleDigitInput(string digit)
        {
            _calculator.InputDigit(int.Parse(digit));
            Display();
        }

        private void HandleAdd()
        {
            _calculator.SetOperator(CalcController.Operator.Add);
            Display();
        }

        private void HandleSubtract()
        {
            _calculator.SetOperator(CalcController.Operator.Subtract);
            Display();
        }

        private void HandleCalculate()
        {
            _calculator.Calculate();
            Display();
        }

        private void HandleClear()
        {
            _calculator.ClearAll();
            Display();
        }

        private void HandleClearEntry()
        {
            _calculator.ClearEntry();
            Display();
        }

        private void Display()
        {
            OnPropertyChanged(nameof(CurrentNumber));
            OnPropertyChanged(nameof(ExpressionText));
        }
    }
}