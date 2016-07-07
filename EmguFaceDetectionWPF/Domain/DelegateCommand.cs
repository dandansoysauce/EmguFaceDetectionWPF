using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EmguFaceDetectionWPF.Domain
{
    public class DelegateCommand : ICommand
    {
        public Predicate<object> CanExecuteDelegate { get; set; }
        public Action<object> ExecuteDelegate { get; set; }

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute)
                   : this(execute, null) { }

        public DelegateCommand() { }

        public DelegateCommand(Action<object> execute,
                       Predicate<object> canExecute)
        {
            ExecuteDelegate = execute;
            CanExecuteDelegate = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate == null)
            {
                return true;
            }

            return CanExecuteDelegate(parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteDelegate?.Invoke(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
