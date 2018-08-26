using System.IO;

namespace Encoder.Utils
{
   internal static class UtilityMethods
   {
      public static void SafeDeleteFile( string filePath )
      {
         if ( File.Exists( filePath ) )
         {
            try
            {
               File.Delete( filePath );
            }
            catch { }
         }
      }
   }
}
