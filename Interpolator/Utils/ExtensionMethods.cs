using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Interpolator.Utils
{
   internal static class ExtensionMethods
   {
      public static void StartAsChildProcess( this Process process )
      {
         process.Start();
         ChildProcessWatcher.AddProcess( process );
      }

      public static void ForEach<T>( this ObservableCollection<T> collection, Action<T> action )
      {
         collection.ToList().ForEach( action );
      }

      public static string GetInstanceName( this Process process )
      {
         string processName = Path.GetFileNameWithoutExtension( process.ProcessName );

         var vategory = new PerformanceCounterCategory( "Process" );
         var instances = vategory.GetInstanceNames().Where( x => x.StartsWith( processName ) );

         foreach ( string instance in instances )
         {
            using ( PerformanceCounter cnt = new PerformanceCounter( "Process", "ID Process", instance, true ) )
            {
               int val = (int)cnt.RawValue;
               if ( val == process.Id )
               {
                  return instance;
               }
            }
         }
         return null;
      }
   }
}
