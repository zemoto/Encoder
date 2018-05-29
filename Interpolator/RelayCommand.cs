using System;
using System.Windows.Input;

namespace Interpolator
{
   internal sealed class RelayCommand : ICommand
   {
      readonly Action _execute;
      readonly Func<bool> _canExecute;

      public RelayCommand( Action execute, Func<bool> canExecute = null )
      {
         _execute = execute;
         _canExecute = canExecute;
      }

      public bool CanExecute( object _ ) => _canExecute == null ? true : _canExecute();

      public event EventHandler CanExecuteChanged
      {
         add { if ( _canExecute != null ) CommandManager.RequerySuggested += value; }
         remove { if ( _canExecute != null ) CommandManager.RequerySuggested -= value; }
      }

      public void Execute( object _ )
      {
         _execute();
      }
   }
}
