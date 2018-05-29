using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Interpolator
{
   internal abstract class ViewModelBase : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      protected void OnPropertyChanged( string propertyName )
      {
         PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
      }

      protected void SetProperty<T>( ref T property, T newValue, [CallerMemberName] string propertyName = null )
      {
         if ( Equals( property, newValue ) ) return;
         property = newValue;
         OnPropertyChanged( propertyName );
      }
   }
}