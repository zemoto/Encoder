using System;
using System.Runtime.CompilerServices;
using Encoder.Utils;

namespace Encoder.Filters
{
   internal abstract class FilterViewModel : ViewModelBase
   {
      protected bool SetClampedProperty( ref int property, int newValue, int min, int max, [CallerMemberName] string propertyName = null )
      {
         newValue = Math.Min( max, Math.Max( min, newValue ) );
         return SetProperty( ref property, newValue, propertyName );
      }

      protected bool SetClampedProperty( ref double property, double newValue, double min, double max, [CallerMemberName] string propertyName = null )
      {
         newValue = Math.Min( max, Math.Max( min, newValue ) );
         return SetProperty( ref property, newValue, propertyName );
      }
   }
}
