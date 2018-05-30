using System.Diagnostics;

namespace Interpolator.Utils
{
   internal static class ExtensionMethods
   {
      public static void StartAsChildProcess( this Process process )
      {
         process.Start();
         ChildProcessWatcher.AddProcess( process );
      }
   }
}
