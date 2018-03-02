using System.ComponentModel;
using System.Runtime.CompilerServices;
using VideoInterpolator.Annotations;

namespace VideoInterpolator.Utils
{
   internal class NotifyPropertyChanged : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      protected bool SetProperty<T>( ref T property, T value, [CallerMemberName] string propertyName = null )
      {
         if ( property == null && value == null || property != null && property.Equals( value ) )
         {
            return false;
         }

         property = value;
         OnPropertyChanged( propertyName );
         return true;
      }

      [NotifyPropertyChangedInvocator]
      protected void OnPropertyChanged( [CallerMemberName] string propertyName = null )
      {
         PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
      }
   }
}
