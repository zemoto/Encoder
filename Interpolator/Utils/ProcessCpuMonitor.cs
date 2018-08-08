using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Interpolator.Utils
{
   internal static class TotalCpuMonitor
   {
      private static readonly PerformanceCounter CpuUsageCounter;

      public static void Initialize() { /*Ensures the static constructor is called*/ }

      static TotalCpuMonitor()
      {
         CpuUsageCounter = new PerformanceCounter( "Processor Information", "% Processor Time", "_Total", true );
         CpuUsageCounter.NextValue();
      }

      public static int GetCurrentCpuUsage() => (int)CpuUsageCounter.NextValue();
   }

   internal sealed class ProcessCpuMonitor : IDisposable
   {
      private readonly PerformanceCounter _cpuUsageCounter;

      public ProcessCpuMonitor( Process process )
      {
         _cpuUsageCounter = new PerformanceCounter( "Process", "% Processor Time", GetInstanceName( process ), true );
         _cpuUsageCounter.NextValue();
      }

      public void Dispose()
      {
         _cpuUsageCounter?.Dispose();
      }

      public int GetCurrentCpuUsage()
      {
         try
         {
            return (int)( _cpuUsageCounter.NextValue() / Environment.ProcessorCount );
         }
         catch ( InvalidOperationException )
         {
            return 0;
         }
      }

      private static string GetInstanceName( Process process )
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
