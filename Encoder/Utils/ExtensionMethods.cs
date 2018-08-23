using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Encoder.Utils
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

      public static T GetAttribute<T>( this ICustomAttributeProvider property ) where T : Attribute
      {
         var attribute = property.GetCustomAttributes( typeof( T ), false ).FirstOrDefault();
         return attribute as T;
      }
   }
}
