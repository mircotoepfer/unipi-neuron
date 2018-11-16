using System;
using System.Windows.Input;

namespace adcoso.iot.devicedrivers.neuron.application
{
    public class RelayCommand : ICommand
    {
        private readonly Action _executeWithoutParam;
        private readonly Action<object> _executeWithParam;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Raised when RaiseCanExecuteChanged is called.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Creates a new command that can always executeWithoutParam.
        /// </summary>
        /// <param name="executeWithoutParam">The execution logic.</param>
        public RelayCommand(Action executeWithoutParam) : this(executeWithoutParam, null)
        {
        }
        public RelayCommand(Action<object> executeWithParam) : this(executeWithParam, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="executeWithoutParam">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _executeWithoutParam = execute;
            _canExecute = canExecute;
        }
        public RelayCommand(Action<object> execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _executeWithParam = execute;
            _canExecute = canExecute;
        }
        /// <summary>
        /// Determines whether this <see cref="RelayCommand"/> can executeWithoutParam in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed, this object can be set to null.
        /// </param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        /// <summary>
        /// Executes the <see cref="RelayCommand"/> on the current command target.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed, this object can be set to null.
        /// </param>
        public void Execute(object parameter)
        {
            _executeWithParam?.Invoke(parameter);

            _executeWithoutParam?.Invoke();
        }

        /// <summary>
        /// Method used to raise the <see cref="CanExecuteChanged"/> event
        /// to indicate that the return value of the <see cref="CanExecute"/>
        /// method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
