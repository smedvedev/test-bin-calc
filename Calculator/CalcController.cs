using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Machine;

namespace Calculator
{
    /// <summary>
    /// Encapsulates user experience flow.
    /// </summary>
    public class CalcController : ICalcController
    {
        private enum CalcState
        {
            OperandEntering,
            Result,
            Error,
        }

        private enum CalcEvent
        {
            Digit,
            Operation,
            Equals,
            ClearEntry,
            ClearAll,
        }

        public enum Operator { None, Add, Subtract }

        public string DisplayText => GetDisplayText();
        public string ExpressionText { get; private set; } = "";

        private Operator _currentOperator;
        private int _prevOperand = 0;
        private int _currentOperand = 0;
        private int _result = 0;
        private CalcState _currentState;
        private string? _lastError;
        private readonly IStateMachine<CalcState, CalcEvent> _stateMachine;
        private readonly ICalcMath _calcMath;

        public CalcController(ICalcMath calcMath)
        {
            _calcMath = calcMath;

            var builder = new StateMachineDefinitionBuilder<CalcState, CalcEvent>();

            builder.In(CalcState.OperandEntering)
                .On(CalcEvent.Digit).Execute((int digit) => ExecuteAddDigit(digit))
                .On(CalcEvent.Operation).Execute((Operator op) => ExecuteSetOperator(op))
                .On(CalcEvent.Equals)
                    .If(() => _currentOperator == Operator.None).Execute(() => { })
                    .If(TryExecuteCalculate).Goto(CalcState.Result)
                    .Otherwise().Goto(CalcState.Error)
                .On(CalcEvent.ClearEntry).Execute(() => _currentOperand = 0)
                .On(CalcEvent.ClearAll).Execute(ExecuteClearAll);

            builder.In(CalcState.Result)
                .On(CalcEvent.Digit)
                    .Goto(CalcState.OperandEntering).Execute((int digit) => { ExecuteClearAll(); ExecuteAddDigit(digit); })
                .On(CalcEvent.Operation)
                    .Goto(CalcState.OperandEntering).Execute((Operator op) => { _currentOperand = _result; ExecuteSetOperator(op); })
                .On(CalcEvent.Equals)
                    .If(() => !TryExecuteCalculate()).Goto(CalcState.Error)
                .On(CalcEvent.ClearEntry)
                .On(CalcEvent.ClearAll).Goto(CalcState.OperandEntering).Execute(ExecuteClearAll);

            builder.In(CalcState.Error)
                .On(CalcEvent.ClearAll).Goto(CalcState.OperandEntering).Execute(ExecuteClearAll)
                .On(CalcEvent.ClearEntry).Goto(CalcState.OperandEntering).Execute(ExecuteClearAll);

            _stateMachine = builder
                .WithInitialState(CalcState.OperandEntering)
                .Build()
                .CreatePassiveStateMachine();

            _stateMachine.TransitionCompleted += _stateMachine_TransitionCompleted;
            _stateMachine.Start();
        }

        public void InputDigit(int v)
        {
            _stateMachine.Fire(CalcEvent.Digit, v);
        }

        public void SetOperator(Operator op)
        {
            _stateMachine.Fire(CalcEvent.Operation, op);
        }

        public void Calculate()
        {
            _stateMachine.Fire(CalcEvent.Equals);
        }

        public void ClearAll()
        {
            _stateMachine.Fire(CalcEvent.ClearAll);
        }

        public void ClearEntry()
        {
            _stateMachine.Fire(CalcEvent.ClearEntry);
        }

        private string GetDisplayText()
        {
            return _currentState switch
            {
                CalcState.OperandEntering => Convert.ToString(_currentOperand, 2),
                CalcState.Result => Convert.ToString(_result, 2),
                CalcState.Error => _lastError ?? "",
                _ => throw new NotImplementedException()
            };
        }

        private void _stateMachine_TransitionCompleted(object? sender, Appccelerate.StateMachine.Machine.Events.TransitionCompletedEventArgs<CalcState, CalcEvent> e)
        {
            _currentState = e.NewStateId;
        }

        private bool TryExecuteCalculate()
        {
            var result = _calcMath.ExecuteBinaryOperation(_prevOperand, _currentOperand, _currentOperator);
            (var success, _result, _lastError) = result;
            BuildExpressionText(CalcState.Result);
            _prevOperand = _result;

            return success;
        }

        private void BuildExpressionText(CalcState forState)
        {
            ExpressionText = forState switch
            {
                CalcState.OperandEntering when _currentOperator != Operator.None =>
                    $"{ToBin(_prevOperand)} {_currentOperator}",
                CalcState.Error or CalcState.Result =>
                    $"{ToBin(_prevOperand)} {_currentOperator} {ToBin(_currentOperand)}",
                _ => "",
            };
        }

        private static string ToBin(int val) => Convert.ToString(val, 2);

        private void ExecuteClearAll()
        {
            _currentOperand = 0;
            _prevOperand = 0;
            _currentOperator = Operator.None;
            BuildExpressionText(CalcState.OperandEntering);
        }

        private void ExecuteSetOperator(Operator op)
        {
            _currentOperator = op;
            _prevOperand = _currentOperand;
            _currentOperand = 0;
            BuildExpressionText(CalcState.OperandEntering);
        }

        private void ExecuteAddDigit(int digit)
        {
            if (digit < 0 || digit > 1)
                throw new ArgumentOutOfRangeException(nameof(digit), "Should be 0 or 1.");

            var newVal = _currentOperand << 1;
            newVal |= digit;

            if (!_calcMath.CheckOperand(newVal))
            {
                //  don't add more digits
                return;
            }

            _currentOperand = newVal;

            BuildExpressionText(CalcState.OperandEntering);
        }
    }
}