using System;
using System.Windows.Input;

namespace SimpleGameEngine.Classes
{
    
    public interface IDelegateCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
    
    public class DelegateCommand: IDelegateCommand
    {
        private readonly Action _execute;
        private readonly Func<object, bool> _canExecute;

        public DelegateCommand(Action execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public DelegateCommand(Action execute)
        {
            _execute = execute;
            _canExecute = (p) => true;
        }
        
        public bool CanExecute(object parameter) => _canExecute(parameter);

        public void Execute(object parameter) => _execute();

        
        public event EventHandler CanExecuteChanged;
        
        public void RaiseCanExecuteChanged() => 
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public class DelegateCommand<T>: IDelegateCommand
    {
        private readonly Action<T> _execute;
        private readonly Func<object, bool> _canExecute;

        public DelegateCommand(Action<T> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public DelegateCommand(Action<T> execute)
        {
            _execute = execute;
            _canExecute = (p) => true;
        }
        
        public bool CanExecute(object parameter) => _canExecute(parameter);

        public void Execute(object parameter) => _execute((T)parameter);

        
        public event EventHandler CanExecuteChanged;
        
        public void RaiseCanExecuteChanged() => 
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}