using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SkillBase.ViewModels.Common
{
    internal class UICommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        Action<object> userAction { get; set; }
        Predicate<object> canExecutePredicate { get; set; }

        public UICommand(Action<object> userAction) : this(userAction, null) { }

        public UICommand(Action<object> action, Predicate<object> canExecute)
        {
            if (action == null)
            {
                throw new Exception($"{this.ToString()} parameter exception. Action can't be null.");
            }
            else
            {
                userAction = action;
                canExecutePredicate = canExecute;
            }
        }

        public bool CanExecute(object? parameter)
        {
            return canExecutePredicate == null ? true : canExecutePredicate.Invoke(parameter);
        }

        public void Execute(object? parameter)
        {
            userAction.Invoke(parameter);
        }
    }
}