using System;
using System.Windows.Input;

namespace VideoInterpolator.Utils
{
   internal sealed class RelayCommand : ICommand
   {
      public event EventHandler CanExecuteChanged
      {
         add => CommandManager.RequerySuggested += value;
         remove => CommandManager.RequerySuggested -= value;
      }

      private readonly Action _methodToExecute;
      private readonly Func<bool> _canExecuteEvaluator;
      public RelayCommand( Action methodToExecute, Func<bool> canExecuteEvaluator = null )
      {
         _methodToExecute = methodToExecute;
         _canExecuteEvaluator = canExecuteEvaluator;
      }

      public bool CanExecute( object parameter )
      {
         if ( _canExecuteEvaluator == null )
         {
            return true;
         }

         bool result = _canExecuteEvaluator.Invoke();
         return result;
      }

      public void Execute( object parameter )
      {
         _methodToExecute.Invoke();
      }
   }
}
