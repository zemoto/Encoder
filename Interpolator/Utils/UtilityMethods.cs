using System;

namespace Interpolator.Utils
{
   internal static class UtilityMethods
   {
      public static double GetClosestMultiple( double multiple, double target )
      {
         var lessThan = multiple * Math.Floor( target / multiple );
         var greaterThan = lessThan + multiple;
         return target - lessThan < greaterThan - target ? lessThan : greaterThan;
      }
   }
}
