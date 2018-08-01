using System;
using System.IO;

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

      public static void SafeDeleteFile( string filePath )
      {
         if ( File.Exists( filePath ) )
         {
            File.Delete( filePath );
         }
      }
   }
}
