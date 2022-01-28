using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CivCulture_ViewModel.Utilities
{
    public class RelayCommand : ICommand
    {
        private Action<object> executeAction;
        private Func<object, bool> canExecuteFunction;

        public RelayCommand(Action<object> executeAction, Func<object, bool> canExecuteFunction = null)
        {
            this.executeAction = executeAction;
            this.canExecuteFunction = canExecuteFunction;
        }

        #region ICommand
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return canExecuteFunction == null ? true : canExecuteFunction(parameter);
        }

        public void Execute(object parameter)
        {
            executeAction(parameter);
        }
        #endregion
    }
}
