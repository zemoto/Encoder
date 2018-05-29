using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Interpolator
{
   internal abstract class ViewModelBase : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      protected void SetProperty<T>( ref T property, T newValue, [CallerMemberName] string propertyName = null )
      {
         if ( Equals( property, newValue ) ) return;
         property = newValue;
         PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
      }
   }
}