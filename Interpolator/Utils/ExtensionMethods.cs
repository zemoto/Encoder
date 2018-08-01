using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
   }
}
